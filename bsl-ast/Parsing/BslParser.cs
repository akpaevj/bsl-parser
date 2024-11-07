using BSL.AST.Diagnostics;
using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes;
using BSL.AST.Parsing.Nodes.Expressions;
using BSL.AST.Parsing.Nodes.Expressions.Arithmetic;
using BSL.AST.Parsing.Nodes.Expressions.Literals;
using BSL.AST.Parsing.Nodes.Expressions.Logical;
using BSL.AST.Parsing.Nodes.Statements;
using BSL.AST.Parsing.Preprocessing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BSL.AST.Parsing
{
	public class BslParser
    {
		public const string DIAGNOSTICS_REPORTER_CODE = "BP";

		private BslParserOptions _options = null!;

		private int _currentTokenIndex = 0;
		private IReadOnlyList<BslToken> _tokens = null!;
		private readonly BslParserState _state = new();

		private readonly List<Diagnostic> _diagnostics = [];
        public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

		public ModuleNode ParseOscriptModule(ReadOnlySpan<char> source)
			=> ParseModule(source, builder =>
			{
				builder.LanguageKind = BslKind.OneScript;
			});

		public ModuleNode ParseModule(ReadOnlySpan<char> source)
		{
			return ParseModule(source, new BslParserOptions());
		}

		public ModuleNode ParseModule(ReadOnlySpan<char> source, Action<BslParserOptions> optionsBuilder)
		{
			var options = new BslParserOptions();
			optionsBuilder(options);

			return ParseModule(source, options);
		}

		public ModuleNode ParseModule(ReadOnlySpan<char> source, BslParserOptions options)
        {
			_options = options;

			if (_options.LanguageKind == BslKind.OneScript)
				_state.IsOneScript = true;

			var lexer = new BslLexer();
            _tokens = lexer.Tokenize(source);

            return Module();
        }

		private ModuleNode Module()
        {
			var module = new ModuleNode();
			_state.CurrentModule = module;

			while (true)
			{
				var attributes = ReadAttributes(module);

				var peek = PeekTokenType();

				if (peek == TokenType.EOF)
				{
					HandleTokenTrivias(PeekToken());
					break;
				}

				try
				{
					BslNode? child = peek switch
					{
						TokenType.VAR => VariableDeclarationStatement(module, attributes),
						TokenType.PROCEDURE or TokenType.FUNCTION => Method(module, attributes, false),
						var type when type == TokenType.IDENTIFIER && NextIdentifierIsBilingualValue("АСИНХ", "ASYNC") => Method(module, attributes, true),
						var _ when attributes.Count > 0 => null,
						_ => Statement(module)
					};

					if (child == null)
					{
						module.Children.AddRange(attributes);
						_diagnostics.Add(SyntaxDiagnosticsFactory.MethodDefinitionExpected(PeekToken().Position));
					}
					else
						module.Children.Add(child);
				}
				catch (BslSyntaxErrorException ex)
				{
					_diagnostics.Add(ex.Error);
					SkipTill(module, TokenType.SEMICOLON);
				}
			}


			foreach (var region in _state.Regions)
				_diagnostics.Add(SyntaxDiagnosticsFactory.EndRegionExpected(region.StartTrivia.Position));

			return module;
        }

		internal IfElseIfDirectiveNode ParseIfElseIfDirective(ReadOnlySpan<char> source)
		{
			var lexer = new BslLexer();
			_tokens = lexer.Tokenize(source);

			var node = new IfElseIfDirectiveNode()
			{
				IfElseIfToken = ReadToken()
			};

			try
			{
				node.Expression = Expression(node);
				node.ThenToken = ReadThen();
			}
			catch (BslSyntaxErrorException ex)
			{
				_diagnostics.Add(ex.Error);
			}

			return node;
		}

		#region Statements

		private BslNode Statement(BslNode parent)
			=> PeekTokenType() switch
			{
				TokenType.VAR => VariableDeclarationStatement(parent, []),
				TokenType.IDENTIFIER => ExpressionStatement(parent),
				TokenType.FOR => ForStatement(parent),
				TokenType.WHILE => WhileStatement(parent),
				TokenType.BREAK => BreakStatement(parent),
				TokenType.CONTINUE => ContinueStatement(parent),
				TokenType.GO_TO => GoToStatement(parent),
				TokenType.MARK => Label(parent),
				TokenType.RAISE => RaiseStatement(parent),
				TokenType.ADD_HANDLER or TokenType.REMOVE_HANDLER => HandlerActionStatement(parent),
				TokenType.TRY => TryStatement(parent),
				TokenType.IF => IfStatement(parent),
				TokenType.RETURN => ReturnStatement(parent),
				TokenType.EXECUTE => ExecuteStatement(parent),
				TokenType.SEMICOLON => new StatementNode(parent)
				{
					SemicolonToken = ReadToken()
				},
				_ => throw new BslSyntaxErrorException(SyntaxDiagnosticsFactory.StatementExpected(PeekToken().Position))
			};

		private VariableDeclarationStatementNode VariableDeclarationStatement(BslNode parent, List<AttributeNode> attributes)
        {
			var node = new VariableDeclarationStatementNode(parent)
			{
				Attributes = attributes,
				VariableKeyword = ReadToken()
			};

			node.Identifiers = new SeparatedListNode<BslToken>(node);
			FillSeparatedList(node.Identifiers, TokenType.COMMA, _ => ReadIdentifier());

			if (PeekTokenType() == TokenType.EXPORT)
				node.ExportKeyword = ReadToken();

			node.SemicolonToken = ReadSemicolon();

            return node;
        }

		private StatementNode ExpressionStatement(BslNode parent)
		{
			if (PeekTokenType(1) == TokenType.EQUAL)
				return AssignmentStatement(parent);

			var node = new ExpressionStatementNode(parent);
			node.Expression = Expression(node);
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private BreakStatementNode BreakStatement(BslNode parent)
			=> new(parent)
			{
				BreakKeyword = ReadToken(),
				SemicolonToken = ReadSemicolon()
			};

		private ContinueStatementNode ContinueStatement(BslNode parent)
			=> new(parent)
			{
				ContinueKeyword = ReadToken(),
				SemicolonToken = ReadSemicolon()
			};

		private BlockNode Block(BslNode parent, params TokenType[] endTokens)
		{
			var node = new BlockNode(parent);

			while (true)
			{
				var peek = PeekTokenType();

				if (endTokens.Contains(peek) || peek == TokenType.EOF)
					break;

				node.Statements.Add(Statement(node));
			}

			return node;
		}

		private WhileStatementNode WhileStatement(BslNode parent)
		{
			var node = new WhileStatementNode(parent)
			{
				WhileKeyword = ReadToken(),
			};

			node.Condition = Expression(node);
			node.DoKeyword = ReadDo();
			node.Body = Block(node, TokenType.END_DO);
			node.EndDoKeyword = ReadEndDo();
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private StatementNode ForStatement(BslNode parent)
		{
			var forToken = ReadToken();

			if (PeekTokenType() == TokenType.EACH)
				return ForEachStatement(parent, forToken);

			var node = new ForStatementNode(parent)
			{
				ForKeyword = forToken,
				IdentifierToken = ReadIdentifier(),
				EqualToken = ReadOrThrow(TokenType.EQUAL, t => SyntaxDiagnosticsFactory.EqualExpected(t.Position))
			};

			node.InitExpression = Expression(node);
			node.ToKeyword = ReadOrThrow(TokenType.TO, t => SyntaxDiagnosticsFactory.ToExpected(t.Position));
			node.Condition = Expression(node);
			node.DoKeyword = ReadDo();
			node.Body = Block(node, TokenType.END_DO);
			node.EndDoKeyword = ReadEndDo();
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private ForEachStatementNode ForEachStatement(BslNode parent, BslToken forToken)
		{
			var node = new ForEachStatementNode(parent)
			{
				ForKeyword = forToken,
				EachKeyword = ReadToken(),
				IdentifierToken = ReadIdentifier(),
				InKeyword = ReadOrThrow(TokenType.IN, t => SyntaxDiagnosticsFactory.InExpected(t.Position))
			};

			node.Collection = Expression(node);
			node.DoKeyword = ReadDo();
			node.Body = Block(node, TokenType.END_DO);
			node.EndDoKeyword = ReadEndDo();
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private AssignmentStatementNode AssignmentStatement(BslNode parent)
		{
			var node = new AssignmentStatementNode(parent)
			{
				IdentifierToken = ReadIdentifier(),
				EqualToken = ReadToken()
			};

			node.Expression = Expression(node);
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private GoToStatementNode GoToStatement(BslNode parent)
			=> new(parent)
			{
				GoToKeyword = ReadToken(),
				MarkToken = ReadOrThrow(TokenType.MARK, t => SyntaxDiagnosticsFactory.MarkExpected(t.Position)),
				IdentifierToken = ReadIdentifier(),
				SemicolonToken = ReadSemicolon()
			};

		private LabelNode Label(BslNode parent)
		{
			var node = new LabelNode(parent)
			{
				MarkToken = ReadToken(),
				IdentifierToken = ReadIdentifier(),
				EndOfMarkToken = ReadOrThrow(TokenType.END_OF_MARK, t => SyntaxDiagnosticsFactory.EndMarkExpected(t.Position)),
			};

			return node;
		}

		private RaiseStatementNode RaiseStatement(BslNode parent)
		{
			var raiseKeyword = ReadToken();

			if (PeekTokenType() == TokenType.OPEN_PARENT)
			{
				var node = new FullRaiseStatementNode(parent)
				{
					RaiseKeword = raiseKeyword
				};

				node.Arguments = Arguments(node);
				node.SemicolonToken = ReadSemicolon();

				return node;
			}
			else
			{
				var node = new ShortRaiseStatementNode(parent)
				{
					RaiseKeword = raiseKeyword
				};

				if (PeekTokenType() != TokenType.SEMICOLON && !NextIsOperatorCloseParentOrEOF())
					node.Expression = Expression(node);

				node.SemicolonToken = ReadSemicolon();

				return node;
			}
		}

		private HandlerActionStatementNode HandlerActionStatement(BslNode parent)
		{
			var node = new HandlerActionStatementNode(parent)
			{
				HandlerActionKeyword = ReadToken()
			};

			node.Event = Expression(node);
			node.CommaToken = ReadComma();
			node.Handler = Expression(node);
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private TryStatementNode TryStatement(BslNode parent)
		{
			var node = new TryStatementNode(parent)
			{
				TryKeyword = ReadToken()
			};
			node.TryBlock = Block(node, TokenType.EXCEPT);
			
			node.ExceptKeyword = ReadOrThrow(TokenType.EXCEPT, t => SyntaxDiagnosticsFactory.ExceptExpected(t.Position));
			node.ExceptBlock = Block(node, TokenType.END_TRY);
			
			node.EndTryKeyword = ReadOrThrow(TokenType.END_TRY, t => SyntaxDiagnosticsFactory.EndTryExpected(t.Position));
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private ElseIfClauseNode ElseIfClause(BslNode parent)
			=> new(parent)
			{
				ElseIfKeyword = ReadToken(),
				Condition = Expression(parent),
				ThenKeyword = ReadThen(),
				Block = Block(parent, TokenType.ELSE_IF, TokenType.ELSE, TokenType.END_IF)
			};

		private ElseClauseNode ElseClause(BslNode parent)
			=> new(parent)
			{
				ElseKeyword = ReadToken(),
				Block = Block(parent, TokenType.END_IF)
			};

		private IfStatementNode IfStatement(BslNode parent)
		{
			var node = new IfStatementNode(parent)
			{
				IfKeyword = ReadToken()
			};

			node.Condition = Expression(node);
			node.ThenKeyword = ReadThen();
			node.IfBlock = Block(node, TokenType.ELSE_IF, TokenType.ELSE, TokenType.END_IF);

			while (PeekTokenType() == TokenType.ELSE_IF)
				node.ElseIfClauses.Add(ElseIfClause(node));

			if (PeekTokenType() == TokenType.ELSE)
				node.ElseClause = ElseClause(node);

			node.EndIfKeyword = ReadOrThrow(TokenType.END_IF, t => SyntaxDiagnosticsFactory.EndIfExpected(t.Position));
			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private ReturnStatementNode ReturnStatement(BslNode parent)
		{
			var node = new ReturnStatementNode(parent)
			{
				ReturnKeyword = ReadToken()
			};

			if (PeekTokenType() != TokenType.SEMICOLON && !NextIsOperatorCloseParentOrEOF())
				node.Expression = Expression(node);

			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		private ExecuteStatementNode ExecuteStatement(BslNode parent)
		{
			var node = new ExecuteStatementNode(parent)
			{
				ExecuteKeyword = ReadToken()
			};

			var hasParents = PeekTokenType() == TokenType.OPEN_PARENT;

			if (hasParents)
				node.OpenParent = ReadOpenParent();

            node.Expression = Expression(node);

			if (hasParents)
				node.CloesParent = ReadCloseParent();

			node.SemicolonToken = ReadSemicolon();

			return node;
		}

		#endregion

		#region Expressions

		private ExpressionNode Expression(BslNode parent, int precedence = 1)
		{
			if (precedence == 3)
			{
				if (PeekTokenType() == TokenType.NOT)
					return NotExpression(parent);
				else
					return Expression(parent, precedence + 1);
			}
			else if (precedence < 7)
				return LoopPrecedenceLevel(parent, precedence switch
				{
					1 => [TokenType.OR],
					2 => [TokenType.AND],
					4 => [TokenType.EQUAL, TokenType.NOT_EQUAL, TokenType.GREATER, TokenType.GREATER_OR_EQUAL, TokenType.LESS, TokenType.LESS_OR_EQUAL],
					5 => [TokenType.PLUS, TokenType.MINUS],
					_ => [TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.MODULO],
				}, precedence);
			else if (precedence == 7)
			{
				var tokenType = PeekTokenType();

				if (tokenType == TokenType.PLUS || tokenType == TokenType.MINUS)
					return UnaryExpression(parent);
				else
					return Expression(parent, precedence + 1);
			}
			else
				return PeekTokenType() switch
				{
					TokenType.IDENTIFIER => NextIdentifierIsBilingualValue("ЖДАТЬ", "AWAIT") ? AwaitExpression(parent) : IdentifierExpression(parent),
					TokenType.STRING => StringLiteralExpression(parent),
					TokenType.DATE => DateLiteralExpression(parent),
					TokenType.NUMBER => NumberLiteralExpression(parent),
					TokenType.FALSE or TokenType.TRUE => BoolLiteralExpression(parent),
					TokenType.UNDEFINED => UndefinedLiteralExpression(parent),
					TokenType.NULL => NullLiteralExpression(parent),
					TokenType.OPEN_PARENT => ParenthesizedExpression(parent),
					TokenType.NEW => NewObjectExpression(parent),
					TokenType.QUESTION_MARK => ConditionalExpression(parent),
					TokenType.NOT => NotExpression(parent),
					_ => throw new BslSyntaxErrorException(SyntaxDiagnosticsFactory.ExpressionExpected(PeekToken().Position))
				};
		}

		private ExpressionNode LoopPrecedenceLevel(BslNode parent, TokenType[] tokenTypes, int precedence)
		{
			ExpressionNode expression = Expression(parent, precedence + 1);

			while (tokenTypes.Contains(PeekTokenType()))
				expression = BinaryExpression(parent, expression, precedence + 1);

			return expression;
		}

		private BinaryExpressionNode BinaryExpression(BslNode parent, ExpressionNode left, int precedence)
		{
			BinaryExpressionNode node = PeekTokenType() switch
			{
				TokenType.OR => new OrExpressionNode(parent),
				TokenType.AND => new AndExpressionNode(parent),
				TokenType.EQUAL => new EqualsExpressionNode(parent),
				TokenType.NOT_EQUAL => new NotEqualsExpressionNode(parent),
				TokenType.GREATER => new GreaterExpressionNode(parent),
				TokenType.GREATER_OR_EQUAL => new GreaterOrEqualsExpressionNode(parent),
				TokenType.LESS => new LessExpressionNode(parent),
				TokenType.LESS_OR_EQUAL => new LessOrEqualsExpressioNode(parent),
				TokenType.PLUS => new AddExpressionNode(parent),
				TokenType.MINUS => new SubtractExpressionNode(parent),
				TokenType.MULTIPLY => new MultiplyExpressionNode(parent),
				TokenType.DIVIDE => new DivideExpressionNode(parent),
				TokenType.MODULO => new ModuloExpressionNode(parent),
				_ => throw new BslSyntaxErrorException(SyntaxDiagnosticsFactory.BinaryExpressionExpected(PeekToken().Position))
			};

			node.Left = left;
			node.OperatorToken = ReadToken();
			node.Right = Expression(node, precedence);

			return node;
		}

		private StringLiteralExpressionNode StringLiteralExpression(BslNode parent)
		{
			var node = new StringLiteralExpressionNode(parent);

			var valueBuilder = new StringBuilder();

			while (PeekTokenType() == TokenType.STRING)
				node.Parts.Add(ReadToken());

			node.Value = valueBuilder.AppendJoin("\r\n", node.Parts.Select(c => c.Text)).ToString();

			return node;
		}

		private DateLiteralExpressionNode DateLiteralExpression(BslNode parent)
		{
			var node = new DateLiteralExpressionNode(parent)
			{
				ValueToken = ReadToken()
			};
			var digits = string.Concat(node.ValueToken.Text.Where(char.IsDigit));

			var format = digits.Length switch
			{
				8 => "yyyyMMdd",
				12 => "yyyyMMddHHmm",
				14 => "yyyyMMddHHmmss",
				_ => null
			};

			var isError = string.IsNullOrEmpty(format);

			if (!isError)
				try
				{
					node.Value = DateTime.ParseExact(digits, format!, null);
				}
				catch
				{
					isError = true;
				}

			if (isError)
				_diagnostics.Add(SyntaxDiagnosticsFactory.InvalidDateLiteralFormat(node.ValueToken.Position));

			return node;
		}

		private NumberLiteralExpressionNode NumberLiteralExpression(BslNode parent)
		{
			var node = new NumberLiteralExpressionNode(parent)
			{
				ValueToken = ReadToken()
			};

			if (decimal.TryParse(node.ValueToken.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." }, out var decimalValue))
				node.Value = decimalValue;
			else
				_diagnostics.Add(SyntaxDiagnosticsFactory.InvalidNumberLiteralFormat(node.ValueToken.Position));

			return node;
		}

		private BoolLiteralExpressionNode BoolLiteralExpression(BslNode parent)
		{
			var node = new BoolLiteralExpressionNode(parent)
			{
				ValueToken = ReadToken()
			};
			node.Value = node.ValueToken.Type == TokenType.TRUE;

			return node;
		}

		private UndefinedLiteralExpressionNode UndefinedLiteralExpression(BslNode parent)
		{
			var node = new UndefinedLiteralExpressionNode(parent)
			{
				ValueToken = ReadToken()
			};
			
			return node;
		}

		private UnaryExpressionNode UnaryExpression(BslNode parent)
		{
			var node = new UnaryExpressionNode(parent)
			{
				OperatorToken = ReadToken()
			};
			node.Operand = Expression(node);

			return node;
		}

		private NotExpressionNode NotExpression(BslNode parent)
		{
			var node = new NotExpressionNode(parent)
			{
				OperatorToken = ReadToken()
			};
			node.Operand = Expression(node);

			return node;
		}

		private NullLiteralExpressionNode NullLiteralExpression(BslNode parent)
		{
			var node = new NullLiteralExpressionNode(parent)
			{
				ValueToken = ReadToken()
			};

			return node;
		}

		private ParenthesizedExpressionNode ParenthesizedExpression(BslNode parent)
		{
			var node = new ParenthesizedExpressionNode(parent)
			{
				OpenParenToken = ReadToken()
			};

			node.Expression = Expression(node);
			node.CloseParenToken = ReadCloseParent();

			return node;
		}

		private ExpressionNode PostExpressionNode(BslNode parent, ExpressionNode node)
			=> PeekTokenType() switch
			{
				TokenType.DOT => AccessMemberExpression(parent, node),
				TokenType.OPEN_PARENT => InvocationExpression(parent, node),
				TokenType.OPEN_BRACKET => ElementAccessExpression(parent, node),
				_ => node
			};

		private ExpressionNode IdentifierExpression(BslNode parent)
			=> PostExpressionNode(parent, new IdentifierExpressionNode(parent)
			{
				IdentifierToken = ReadToken()
			});

		private ExpressionNode ElementAccessExpression(BslNode parent, ExpressionNode member)
		{
			var node = new ElementAccessExpressionNode(parent)
			{
				Member = member,
				OpenBracketToken = ReadToken()
			};

			node.Argument = Expression(node);
			node.CloseBracketToken = ReadOrThrow(TokenType.CLOSE_BRACKET, t => SyntaxDiagnosticsFactory.CloseBracketExpected(t.Position));

			return PostExpressionNode(parent, node);
		}

		private ExpressionNode InvocationExpression(BslNode parent, ExpressionNode member)
			=> PostExpressionNode(parent, new InvocationExpressionNode(parent)
			{
				Member = member,
				Arguments = Arguments(parent)
			});

		private ExpressionNode AccessMemberExpression(BslNode parent, ExpressionNode left)
        {
			var node = new AccessMemberExpressionNode(parent)
			{
				Left = left,
				OperatorToken = ReadToken()
			};

			node.Right = IdentifierExpression(node);

			return PostExpressionNode(parent, node);
        }

		private ExpressionNode NewObjectExpression(BslNode parent)
		{
			var node = new NewObjectExpressionNode(parent)
			{
				NewKeyword = ReadToken()
			};

			if (PeekTokenType() == TokenType.IDENTIFIER)
				node.IdentifierToken = ReadIdentifier();

			if (PeekTokenType() == TokenType.OPEN_PARENT)
				node.Arguments = Arguments(node);

			if (node.IdentifierToken == null && node.Arguments == null)
				throw new BslSyntaxErrorException(SyntaxDiagnosticsFactory.NewObjectArgumentsExpected(PeekToken().Position));

			if (_options.LanguageKind == BslKind.OneScript)
				return PostExpressionNode(parent, node);
			else
				return node;
		}

		private ExpressionNode ConditionalExpression(BslNode parent)
		{
			var node = new ConditionalExpressionNode(parent)
			{
				QuestionMarkToken = ReadToken(),
				OpenParenToken = ReadOpenParent()
			};

			node.Condition = Expression(node);
			node.FirstCommaToken = ReadComma();
			node.IfTrue = Expression(node);
			node.SecondCommaToken = ReadComma();
			node.IfFalse = Expression(node);

			node.CloseParenToken = ReadCloseParent();

			return PostExpressionNode(parent, node);
		}

		private AwaitExpressionNode AwaitExpression(BslNode parent)
		{
			var node = new AwaitExpressionNode(parent)
			{
				AwaitToken = ReadToken(),
				Expression = Expression(parent)
			};

			if (!_state.InAsyncMethod)
				_diagnostics.Add(SyntaxDiagnosticsFactory.UnexpectedAwaitOperator(node.AwaitToken.Position));

			return node;
		}

		#endregion

		#region Other nodes

		private ArgumentsNode Arguments(BslNode parent)
		{
			var node = new ArgumentsNode(parent)
			{
				OpenParenToken = ReadToken()
			};

			if (PeekTokenType() != TokenType.CLOSE_PARENT)
				FillSeparatedList(node, TokenType.COMMA, list =>
				{
					var peek = PeekTokenType();

					if (peek == TokenType.COMMA || peek == TokenType.CLOSE_PARENT)
						return null;
					else
						return Expression(list);
				});

			node.CloseParenToken = ReadCloseParent();

			return node;
		}

		private AttributeParameterNode AttributeParameter(BslNode parent)
		{
			var node = new AttributeParameterNode(parent);

			if (_options.LanguageKind == BslKind.OneC)
				node.ValueExpression = Expression(node);
            else
			{
				if (PeekTokenType() == TokenType.IDENTIFIER)
				{
					node.NameToken = ReadToken();

					if (PeekTokenType() == TokenType.EQUAL)
					{
						node.EqualToken = ReadToken();
						node.ValueExpression = Expression(node);
					}
				}
				else
					node.ValueExpression = Expression(node);
			}

			return node;
		}

		private AttributeParametersNode AttributeParameters(BslNode parent)
		{
			var node = new AttributeParametersNode(parent)
			{
				OpenParenToken = ReadOpenParent()
			};

			if (PeekTokenType() != TokenType.CLOSE_PARENT)
				FillSeparatedList(node, TokenType.COMMA, AttributeParameter);

			node.CloseParenToken = ReadCloseParent();

			return node;
		}

		private AttributeNode Attribute(BslNode parent)
		{
			var node = new AttributeNode(parent)
			{
				AmpersandToken = ReadToken(),
				IdentifierToken = ReadIdentifier()
			};

			if (PeekTokenType() == TokenType.OPEN_PARENT)
				node.Parameters = AttributeParameters(node);

			return node;
		}

		private ParameterNode Parameter(BslNode parent)
		{
			var node = new ParameterNode(parent);

			if (_options.LanguageKind == BslKind.OneScript)
				node.Attributes = ReadAttributes(parent);

			if (PeekTokenType() == TokenType.VAL)
				node.ValKeyword = ReadToken();

			node.IdentifierToken = ReadIdentifier();

			if (PeekTokenType() == TokenType.EQUAL)
			{
				node.EqualToken = ReadToken();
				node.DefaultExpression = Expression(node);
			}

			return node;
		}

		private ParametersNode Parameters(BslNode parent)
		{
			var node = new ParametersNode(parent)
			{
				OpenParenToken = ReadOpenParent()
			};

			if (PeekTokenType() != TokenType.CLOSE_PARENT)
				FillSeparatedList(node, TokenType.COMMA, Parameter);

			node.CloseParenToken = ReadCloseParent();

			return node;
		}

		private MethodNode Method(BslNode parent, List<AttributeNode> attributes, bool isAsync)
		{
			var node = new MethodNode(parent)
			{
				Attributes = attributes
			};

			if (isAsync)
				node.AsyncToken = ReadToken();

			_state.InAsyncMethod = node.IsAsync;

			node.MethodKeyword = ReadToken();
			node.IdentifierToken = ReadIdentifier();
			node.Parameters = Parameters(node);

			if (PeekTokenType() == TokenType.EXPORT)
				node.ExportKeyword = ReadToken();

			var endToken = node.MethodKeyword.Type == TokenType.PROCEDURE ? TokenType.END_PROCEDURE : TokenType.END_FUNCTION;
			node.Body = Block(node, endToken);

			node.EndMethodKeyword = ReadOrThrow(endToken, t =>
			{
				if (endToken == TokenType.END_PROCEDURE)
					return SyntaxDiagnosticsFactory.EndProcedureExpected(t.Position);
				else
					return SyntaxDiagnosticsFactory.EndFunctionExpected(t.Position);
			});

			return node;
		}

		private List<AttributeNode> ReadAttributes(BslNode parent)
		{
			var items = new List<AttributeNode>();

			while (PeekTokenType() == TokenType.AMPERSAND)
				items.Add(Attribute(parent));

			return items;
		}

		private void FillSeparatedList<TNode, TItem>(TNode node, TokenType separator, Func<TNode, TItem> itemBuilder) where TNode : SeparatedListNode<TItem>
		{
			var separators = new List<BslToken>();
			var items = new List<TItem>();

			while (PeekTokenType() != TokenType.EOF)
			{
				items.Add(itemBuilder(node));

				if (PeekTokenType() == separator)
					separators.Add(ReadToken());
				else
					break;
			}

			node.Separators = separators;
			node.Items = items;
		}

		#endregion

		#region Tokens reading

		private void HandleTrivias(List<BslTrivia> trivias)
		{
			foreach(var trivia in trivias)
			{
				if (trivia.Kind == BslTriviaKind.RegionDirective)
				{
					_state.Regions.Push(new Region()
					{
						StartTrivia = trivia,
						Name = trivia.Value
					});
				}
				else if (trivia.Kind == BslTriviaKind.EndRegionDirective)
				{
					if (_state.Regions.TryPop(out var region))
					{
						region.FinishTrivia = trivia;
						_state.CurrentModule.Regions.Add(region);
					}
					else
						_diagnostics.Add(SyntaxDiagnosticsFactory.UnexpectedEndRegion(trivia.Position));
				}
				else if (trivia.Kind == BslTriviaKind.UseDirective)
					_state.CurrentModule.Usings.Add(new Using()
					{
						Trivia = trivia,
						Path = trivia.Value
					});
				else if (trivia.Kind == BslTriviaKind.IfDirective || trivia.Kind == BslTriviaKind.ElseIfDirective)
				{
					var context = BslDirectiveConditionCompiler.Compile(trivia.Value);

					if (context.Errors.Count > 0)
						_diagnostics.AddRange(context.Errors);
					else
					{

					}
				}
			}
		}

		private void HandleTokenTrivias(BslToken token)
		{
			HandleTrivias(token.LeadingTrivias);
			HandleTrivias(token.TrailingTrivias);
		}

		private void SkipTill(BslNode parent, TokenType tokenType)
		{
			var node = new SkippedTokensNode(parent);

			while (PeekTokenType() != TokenType.EOF)
			{
				node.Tokens.Add(ReadToken());

				if (tokenType == PeekTokenType())
					break;
			}
		}

		private BslToken PeekToken(int offset = 0)
		{
			if (_currentTokenIndex + offset >= _tokens.Count)
				return _tokens[^1];
			else
				return _tokens[_currentTokenIndex + offset];
		}

		private TokenType PeekTokenType(int offset = 0)
			=> PeekToken(offset).Type;

		private BslToken ReadToken()
		{
			var token = PeekToken();

			if (token.Type != TokenType.EOF)
			{
				HandleTokenTrivias(token);
				_currentTokenIndex++;
			}

			return token;
		}

		private BslToken ReadOrThrow(TokenType tokenType, Func<BslToken, Diagnostic> errorBuilder)
		{
			var peek = PeekToken();

			if (tokenType == peek.Type)
				return ReadToken();
			else
				throw new BslSyntaxErrorException(errorBuilder(peek));
		}

		private BslToken ReadOpenParent()
			=> ReadOrThrow(TokenType.OPEN_PARENT, t => SyntaxDiagnosticsFactory.OpenParentExpected(t.Position));

		private BslToken ReadComma()
			=> ReadOrThrow(TokenType.COMMA, t => SyntaxDiagnosticsFactory.CommaExpected(t.Position));

		private BslToken ReadCloseParent()
			=> ReadOrThrow(TokenType.CLOSE_PARENT, t => SyntaxDiagnosticsFactory.CloseParentExpected(t.Position));

		private BslToken ReadIdentifier()
			=> ReadOrThrow(TokenType.IDENTIFIER, t => SyntaxDiagnosticsFactory.IdentifierExpected(t.Position));

		private BslToken? ReadSemicolon()
		{
			// Если сразу за предполагаемой точкой с запятой следует токен закрытия операторных скобок или конец файла, то точка с запятой может быть пропущена
			if (NextIsOperatorCloseParentOrEOF())
				return null;
			else
				return ReadOrThrow(TokenType.SEMICOLON, t => SyntaxDiagnosticsFactory.SemicolonExpected(t.Position));
		}

		private BslToken ReadDo()
			=> ReadOrThrow(TokenType.DO, t => SyntaxDiagnosticsFactory.DoExpected(t.Position));

		private BslToken ReadEndDo()
			=> ReadOrThrow(TokenType.END_DO, t => SyntaxDiagnosticsFactory.EndDoExpected(t.Position));

		private BslToken ReadThen()
			=> ReadOrThrow(TokenType.THEN, t => SyntaxDiagnosticsFactory.ThenExpected(t.Position));

		private bool NextIsOperatorCloseParentOrEOF()
		{
			var peek = PeekTokenType();

			return
				peek == TokenType.END_IF ||
				peek == TokenType.END_DO ||
				peek == TokenType.ELSE_IF ||
				peek == TokenType.ELSE ||
				peek == TokenType.END_FUNCTION ||
				peek == TokenType.END_PROCEDURE ||
				peek == TokenType.END_TRY ||
				peek == TokenType.EXCEPT ||
				peek == TokenType.EOF;
		}

		private bool NextIdentifierIsBilingualValue(string ru, string en)
		{
			var peek = PeekToken();
			return peek.Type == TokenType.IDENTIFIER && ParserHelper.BilingualTokenValueIs(peek.Text, ru, en);
		}

		#endregion
	}
}