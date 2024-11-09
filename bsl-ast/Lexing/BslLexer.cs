using BSL.AST.Diagnostics;
using BSL.AST.Parsing;
using BSL.AST.Parsing.Preprocessing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace BSL.AST.Lexing
{
    public class BslLexer
	{
		private readonly BslLexerOptions _options;
		private readonly BslLexerState _state = new();

		private readonly List<BslToken> _tokens = [];

		private readonly List<Diagnostic> _diagnostics = [];
		public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

		private readonly List<Region> _regions = [];
		public IReadOnlyList<Region> Regions => _regions;

		private readonly List<Using> _usings = [];
		public IReadOnlyList<Using> Usings => _usings;

		private readonly List<Insert> _inserts = [];
		public IReadOnlyList<Insert> Inserts => _inserts;

		private readonly List<Delete> _deletes = [];
		public IReadOnlyList<Delete> Deletes => _deletes;

		public bool IsNative { get; internal set; } = false;

		public BslLexer(BslLexerOptions options)
		{
			_options = options;

			_state.CompileContexts.Add([_options.CompileContexts]);
			_state.CurrentCompileContext = _options.CompileContexts;
		}

		public IReadOnlyList<BslToken> Tokenize(ReadOnlySpan<char> source)
        {
			while (_state.CurrentPosition < source.Length)
            {
				_state.LeadingTrivias = Trivias(source);

				if (_state.CurrentPosition == source.Length)
                    break;

                var peek = source[_state.CurrentPosition];

                if (_state.ReadMultilineString || peek == '"')
                    _tokens.Add(StringLiteral(source));
				else if (peek == '.')
					AddSimpleToken(source, TokenType.DOT);
				else if (peek == ',')
					AddSimpleToken(source, TokenType.COMMA);
				else if (peek == ';')
					AddSimpleToken(source, TokenType.SEMICOLON);
				else if (peek == '=')
					AddSimpleToken(source, TokenType.EQUAL);
				else if (peek == '(')
					AddSimpleToken(source, TokenType.OPEN_PARENT);
				else if (peek == ')')
					AddSimpleToken(source, TokenType.CLOSE_PARENT);
				else if (peek == '[')
					AddSimpleToken(source, TokenType.OPEN_BRACKET);
				else if (peek == ']')
					AddSimpleToken(source, TokenType.CLOSE_BRACKET);
				else if (peek == '\'')
                    _tokens.Add(DateTimeLiteral(source));
                else if (char.IsDigit(peek))
                    _tokens.Add(NumericLiteral(source));
                else if (peek == '/')
                    if (NextChar(source) == '/')
                        throw new NotImplementedException();
                    else
                        AddSimpleToken(source, TokenType.DIVIDE);
                else if (peek == '+')
                    AddSimpleToken(source, TokenType.PLUS);
                else if (peek == '-')
                    AddSimpleToken(source, TokenType.MINUS);
				else if (peek == '?')
					AddSimpleToken(source, TokenType.QUESTION_MARK);
				else if (peek == '*')
                    AddSimpleToken(source, TokenType.MULTIPLY);
                else if (peek == '%')
                    AddSimpleToken(source, TokenType.MODULO);
                else if (peek == '>')
                    if (NextChar(source) == '=')
                        AddSimpleToken(source, TokenType.GREATER_OR_EQUAL, 2);
                    else
                        AddSimpleToken(source, TokenType.GREATER);
                else if (peek == '<')
                {
                    var nextChar = NextChar(source);

                    if (nextChar == '=')
                        AddSimpleToken(source, TokenType.LESS_OR_EQUAL, 2);
                    else if (nextChar == '>')
                        AddSimpleToken(source, TokenType.NOT_EQUAL, 2);
                    else
                        AddSimpleToken(source, TokenType.LESS);
                }
                else if (peek == '&')
                    AddSimpleToken(source, TokenType.AMPERSAND);
				else if (peek == '~')
					AddSimpleToken(source, TokenType.MARK);
				else if (peek == ':')
					AddSimpleToken(source, TokenType.END_OF_MARK);
				else if (TryGetCharSequence(source, out var position))
                {
                    if (TryGetKeyword(source, position, out var token))
                        _tokens.Add(token!);
                    else if (TryGetBooleanLiteral(source, position, out token))
                        _tokens.Add(token!);
                    else if (TryGetUndefinedLiteral(source, position, out token))
                        _tokens.Add(token!);
                    else if (TryGetNullLiteral(source, position, out token))
                        _tokens.Add(token!);
                    else
                        _tokens.Add(GetIdentifier(source, position));
                }
                else
                    AddUnknownToken(source, position);

                var lastToken = _tokens[^1];
				lastToken.CompileContext = _state.CurrentCompileContext;

				lastToken.LeadingTrivias = _state.LeadingTrivias;
				lastToken.TrailingTrivias = Trivias(source, true);
			}

			var eofToken = new BslToken("", TokenType.EOF, new SourcePosition(_state.CurrentPosition, _state.CurrentPosition, _state.CurrentLine, _state.CurrentColumn))
			{
				CompileContext = BslCompileContexts.All,
				LeadingTrivias = _state.LeadingTrivias
			};
			_tokens.Add(eofToken);

			foreach (var region in _state.Regions)
				_diagnostics.Add(SyntaxDiagnosticsFactory.EndRegionExpected(region.StartTrivia.Position));

			return _tokens;
        }

        private BslToken StringLiteral(ReadOnlySpan<char> source)
        {
			var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;
			var line = _state.CurrentLine;

            var endOffset = 0;

			while (_state.CurrentPosition < source.Length)
			{
				if (source[_state.CurrentPosition] == '\r')
                {
                    _state.ReadMultilineString = true;
					break;
				}

				var nextCh = NextChar(source);

				if (source[_state.CurrentPosition] == '"')
					_state.CountedQuotes++;

				_state.CurrentPosition++;
				_state.CurrentColumn++;

				if (_state.CountedQuotes > 1 && _state.CountedQuotes % 2 == 0 && nextCh != '"')
                {
                    endOffset = 1;
					_state.ReadMultilineString = false;
					break;
				}
			}

            if (!_state.ReadMultilineString)
				_state.CountedQuotes = 0;

			var position = new SourcePosition(startPosition, _state.CurrentPosition, line, startColumn);
			var value = source[(startPosition + 1)..(_state.CurrentPosition - endOffset)];

			return new(value.ToString().Replace("\"\"", "\""), TokenType.STRING, position);
        }

		private BslToken DateTimeLiteral(ReadOnlySpan<char> source)
        {
			var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;
            var started = false;

			while (_state.CurrentPosition < source.Length)
			{
                var ch = source[_state.CurrentPosition];

				_state.CurrentPosition++;
				_state.CurrentColumn++;

				if (ch == '\'' && !started)
                {
					started = true;
                    continue;
				}

				if (ch == '\'' && started)
                    break;
			}

			var position = new SourcePosition(startPosition, _state.CurrentPosition, _state.CurrentLine, startColumn);
			var value = source[startPosition.._state.CurrentPosition];

			return new(value.ToString(), TokenType.DATE, position);
        }

        private bool TryGetCharSequence(ReadOnlySpan<char> source, out SourcePosition position)
        {
            var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;

			while (_state.CurrentPosition < source.Length)
            {
                var ch = source[_state.CurrentPosition];

                if (char.IsLetterOrDigit(ch) || ch == '_')
                {
					_state.CurrentPosition++;
					_state.CurrentColumn++;
				}
				else
                    break;
            }

            if (startPosition != _state.CurrentPosition)
            {
				position = new SourcePosition(startPosition, _state.CurrentPosition, _state.CurrentLine, startColumn);
                return true;
			}

            position = default;
            return false;
        }

        private static bool TryGetBooleanLiteral(ReadOnlySpan<char> source, SourcePosition position, out BslToken? token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

            if (ParserHelper.BilingualTokenValueIs(charSequence, "ИСТИНА", "TRUE"))
            {
				token = new BslToken(source[position.StartPosition..position.EndPosition].ToString(), TokenType.TRUE, position);
                return true;
            }
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЛОЖЬ", "FALSE"))
            {
                token = new BslToken(source[position.StartPosition..position.EndPosition].ToString(), TokenType.FALSE, position);
				return true;
            }

            token = null;

            return false;
        }

        private BslToken NumericLiteral(ReadOnlySpan<char> source)
        {
            var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;

			while (_state.CurrentPosition < source.Length)
            {
                var c = source[_state.CurrentPosition];

                if (char.IsDigit(c) || c == '.')
				{
					_state.CurrentPosition++;
					_state.CurrentColumn++;
				}
				else
                    break;
            }

            var position = new SourcePosition(startPosition, _state.CurrentPosition, _state.CurrentLine, startColumn);
            var value = source[position.StartPosition..position.EndPosition];

            return new(value.ToString(), TokenType.NUMBER, position);
        }

        private static bool TryGetUndefinedLiteral(ReadOnlySpan<char> source, SourcePosition position, out BslToken? token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

            if (ParserHelper.BilingualTokenValueIs(charSequence, "НЕОПРЕДЕЛЕНО", "UNDEFINED"))
            {
                token = new BslToken(charSequence.ToString(), TokenType.UNDEFINED, position);
                return true;
            }

            token = null;

            return false;
        }

        private static bool TryGetNullLiteral(ReadOnlySpan<char> source, SourcePosition position, out BslToken? token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

            if (charSequence.Equals("NULL", StringComparison.Ordinal))
            {
                token = new BslToken(source[position.StartPosition..position.EndPosition].ToString(), TokenType.NULL, position);
                return true;
            }

            token = null;

            return false;
        }

        private static bool TryGetKeyword(ReadOnlySpan<char> source, SourcePosition position, out BslToken? token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

			TokenType? type = null;

			if (ParserHelper.BilingualTokenValueIs(charSequence, "ПРОЦЕДУРА", "PROCEDURE"))
				type = TokenType.PROCEDURE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ФУНКЦИЯ", "FUNCTION"))
				type = TokenType.FUNCTION;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦПРОЦЕДУРЫ", "ENDPROCEDURE"))
				type = TokenType.END_PROCEDURE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦФУНКЦИИ", "ENDFUNCTION"))
				type = TokenType.END_FUNCTION;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЭКСПОРТ", "EXPORT"))
				type = TokenType.EXPORT;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЕСЛИ", "IF"))
                type = TokenType.IF;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ТОГДА", "THEN"))
                type = TokenType.THEN;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИНАЧЕЕСЛИ", "ELSIF"))
                type = TokenType.ELSE_IF;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИНАЧЕ", "ELSE"))
                type = TokenType.ELSE;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦЕСЛИ", "ENDIF"))
                type = TokenType.END_IF;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ВОЗВРАТ", "RETURN"))
				type = TokenType.RETURN;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ДЛЯ", "FOR"))
                type = TokenType.FOR;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "КАЖДОГО", "EACH"))
                type = TokenType.EACH;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИЗ", "IN"))
                type = TokenType.IN;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПО", "TO"))
                type = TokenType.TO;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПОКА", "WHILE"))
                type = TokenType.WHILE;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЦИКЛ", "DO"))
                type = TokenType.DO;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦЦИКЛА", "ENDDO"))
                type = TokenType.END_DO;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "И", "AND"))
				type = TokenType.AND;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИЛИ", "OR"))
				type = TokenType.OR;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "НЕ", "NOT"))
				type = TokenType.NOT;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПОПЫТКА", "TRY"))
				type = TokenType.TRY;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИСКЛЮЧЕНИЕ", "EXCEPT"))
				type = TokenType.EXCEPT;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ВЫЗВАТЬИСКЛЮЧЕНИЕ", "RAISE"))
				type = TokenType.RAISE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦПОПЫТКИ", "ENDTRY"))
				type = TokenType.END_TRY;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПРОДОЛЖИТЬ", "CONTINUE"))
				type = TokenType.CONTINUE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПРЕРВАТЬ", "BREAK"))
				type = TokenType.BREAK;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "НОВЫЙ", "NEW"))
				type = TokenType.NEW;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПЕРЕМ", "VAR"))
                type = TokenType.VAR;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПЕРЕЙТИ", "GOTO"))
                type = TokenType.GO_TO;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ВЫПОЛНИТЬ", "EXECUTE"))
                type = TokenType.EXECUTE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЗНАЧ", "VAL"))
				type = TokenType.VAL;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ДОБАВИТЬОБРАБОТЧИК", "ADDHANDLER"))
				type = TokenType.ADD_HANDLER;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "УДАЛИТЬОБРАБОТЧИК", "REMOVEHANDLER"))
				type = TokenType.REMOVE_HANDLER;

			if (type == null)
            {
				token = null;
                return false;
			}
            else
            {
				var value = source[position.StartPosition..position.EndPosition];
				token = new BslToken(value.ToString(), (TokenType)type, position);
                return true;
			}
        }

        private static BslToken GetIdentifier(ReadOnlySpan<char> source, SourcePosition position)
        {
			var value = source[position.StartPosition..position.EndPosition];
			return new BslToken(value.ToString(), TokenType.IDENTIFIER, position);
        }

		private void AddUnknownToken(ReadOnlySpan<char> source, SourcePosition position = default)
		{
            if (position.Equals(default))
            {
                var startPosition = _state.CurrentPosition;
                var startColumn = _state.CurrentColumn;

				while (_state.CurrentPosition < source.Length)
				{
					var c = source[_state.CurrentPosition];

					if (c == ';' || c == '\r')
						break;
					else
                    {
						_state.CurrentPosition++;
						_state.CurrentColumn++;
					}
				}

                position = new SourcePosition(startPosition, _state.CurrentPosition, _state.CurrentLine, startColumn);
			}

			var value = source[position.StartPosition..position.EndPosition];
			var token = new BslToken(value.ToString(), TokenType.UNKNOWN, position);

            _tokens.Add(token);
		}

        private char NextChar(ReadOnlySpan<char> source)
            => _state.CurrentPosition + 1 >= source.Length ? '\0' : source[_state.CurrentPosition + 1];

        private void AddSimpleToken(ReadOnlySpan<char> source, TokenType tokenType, int length = 1)
        {
            var startPosition = _state.CurrentPosition;
            var startColumn = _state.CurrentColumn;
            _state.CurrentPosition += length;
			_state.CurrentColumn += length;

			var position = new SourcePosition(startPosition, _state.CurrentPosition, _state.CurrentLine, startColumn);

			var value = source[position.StartPosition..position.EndPosition];
			_tokens.Add(new BslToken(value.ToString(), tokenType, position));
        }

        private List<BslTrivia> Trivias(ReadOnlySpan<char> source, bool trailing = false)
        {
            var items = new List<BslTrivia>();

            while (_state.CurrentPosition < source.Length)
            {
                var peek = source[_state.CurrentPosition];

                if (peek == '\r')
                {
                    EndOfLineTrivia(source, items);

                    if (trailing)
                        break;
                }
                else if (char.IsWhiteSpace(peek))
                    WhiteSpaceTrivia(source, items);
                else if (peek == '/' && NextChar(source) == '/')
                    CommentTrivia(source, items);
				else if (peek == '#')
					PreprocessingTrivia(source, items);
				else
                    break;
            }

            return items;
        }

		private void WhiteSpaceTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
		{
			var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;

			while (_state.CurrentPosition < source.Length)
			{
				var c = source[_state.CurrentPosition];

				if (char.IsWhiteSpace(c) && c != '\r')
				{
					_state.CurrentPosition++;
					_state.CurrentColumn++;
				}
				else
					break;
			}

			var position = new SourcePosition(startPosition, _state.CurrentPosition, _state.CurrentLine, startColumn);
			var value = source[position.StartPosition..position.EndPosition];

			trivias.Add(new(BslTriviaKind.WhiteSpace, value.ToString(), position));
		}

		private void EndOfLineTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
		{
			var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;

			if (NextChar(source) == '\n')
				_state.CurrentPosition++;

			_state.CurrentPosition++;

			var position = new SourcePosition(startPosition, _state.CurrentPosition, _state.CurrentLine, startColumn);
			_state.CurrentLine++;
			_state.CurrentColumn = 0;

			var value = source[position.StartPosition..position.EndPosition];

			trivias.Add(new(BslTriviaKind.EndOfLine, value.ToString(), position));
		}

		private void CommentTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
		{
			var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;
			var line = _state.CurrentLine;

			while (_state.CurrentPosition < source.Length)
			{
				if (source[_state.CurrentPosition] == '\r')
					break;

				_state.CurrentPosition++;
				_state.CurrentColumn++;
			}

			var position = new SourcePosition(startPosition, _state.CurrentPosition, line, startColumn);
			var value = source[position.StartPosition..position.EndPosition];

            trivias.Add(new(BslTriviaKind.Comment, value.ToString(), position));
		}

        private void PreprocessingTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
        {
			var startPosition = _state.CurrentPosition;
			var startColumn = _state.CurrentColumn;
			var line = _state.CurrentLine;

			while (_state.CurrentPosition < source.Length)
			{
				if (source[_state.CurrentPosition] == '\r')
					break;

				_state.CurrentPosition++;
				_state.CurrentColumn++;
			}

			var position = new SourcePosition(startPosition, _state.CurrentPosition, line, startColumn);
			var value = source[position.StartPosition..position.EndPosition];

            var preprocessorText = value[1..];

            if (preprocessorText.Length > 0)
            {
				var startTextRange = preprocessorText.FirstWordRange();
				var startText = preprocessorText[startTextRange];

				if (ParserHelper.BilingualTokenValueIs(startText, "ОБЛАСТЬ", "REGION"))
				{
					var trivia = new BslTrivia(BslTriviaKind.RegionDirective, preprocessorText[startTextRange.End.Value..].Trim().ToString(), position);
					trivias.Add(trivia);

					_state.Regions.Push(new Region()
					{
						StartTrivia = trivia,
						Name = trivia.Value
					});
				}
				else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦОБЛАСТИ", "ENDREGION"))
				{
					var trivia = new BslTrivia(BslTriviaKind.EndRegionDirective, preprocessorText.ToString(), position);
					trivias.Add(trivia);

					if (_state.Regions.TryPop(out var region))
					{
						region.FinishTrivia = trivia;
						_regions.Add(region);
					}
					else
						_diagnostics.Add(SyntaxDiagnosticsFactory.UnexpectedEndRegion(trivia.Position));
				}
				else if (ParserHelper.BilingualTokenValueIs(startText, "ЕСЛИ", "IF"))
				{
					trivias.Add(new(BslTriviaKind.IfPreprocessorTrivia, preprocessorText.ToString(), position));
					SetCompileContext(preprocessorText, true);
				}
				else if (ParserHelper.BilingualTokenValueIs(startText, "ИНАЧЕЕСЛИ", "ELSIF"))
				{
					trivias.Add(new(BslTriviaKind.ElseIfPreprocessorTrivia, preprocessorText.ToString(), position));
					SetCompileContext(preprocessorText, false);
				}
				else if (ParserHelper.BilingualTokenValueIs(startText, "ИНАЧЕ", "ELSE"))
				{
					trivias.Add(new(BslTriviaKind.ElsePreprocessorTrivia, preprocessorText.ToString(), position));

					var contexts = _state.CompileContexts[^1];
					var previousContexts = _state.CompileContexts[^2];

					var accumulatedContexts = previousContexts[^1];
					foreach (var context in contexts)
						accumulatedContexts ^= context;

					_state.CurrentCompileContext = accumulatedContexts;
					contexts.Add(_state.CurrentCompileContext);
				}
				else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦЕСЛИ", "ENDIF"))
				{
					trivias.Add(new(BslTriviaKind.EndIfPreprocessorTrivia, preprocessorText.ToString(), position));

					_state.CompileContexts.RemoveAt(_state.CompileContexts.Count - 1);
					_state.CurrentCompileContext = _state.CompileContexts[^1][^1]; // Просто сбросим контекст на предыдущий
				}
				else
                {
                    if (_options.LanguageKind == BslKind.OneC)
                    {
						if (ParserHelper.BilingualTokenValueIs(startText, "ВСТАВКА", "INSERT"))
						{
							var trivia = new BslTrivia(BslTriviaKind.InsertDirective, preprocessorText.ToString(), position);
							trivias.Add(trivia);

							_state.Inserts.Push(new Insert()
							{
								StartTrivia = trivia
							});
						}	
						else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦВСТАВКИ", "ENDINSERT"))
						{
							var trivia = new BslTrivia(BslTriviaKind.EndInsertDirective, preprocessorText.ToString(), position);
							trivias.Add(trivia);

							if (_state.Inserts.TryPop(out var insert))
							{
								insert.FinishTrivia = trivia;
								_inserts.Add(insert);
							}
							else
								_diagnostics.Add(SyntaxDiagnosticsFactory.UnexpectedEndInsert(trivia.Position));
						}
						else if (ParserHelper.BilingualTokenValueIs(startText, "УДАЛЕНИЕ", "DELETE"))
						{
							var trivia = new BslTrivia(BslTriviaKind.DeleteDirective, preprocessorText.ToString(), position);
							trivias.Add(trivia);

							_state.Deletes.Push(new Delete()
							{
								StartTrivia = trivia
							});
						}
						else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦУДАЛЕНИЯ", "ENDDELETE"))
						{
							var trivia = new BslTrivia(BslTriviaKind.EndDeleteDirective, preprocessorText.ToString(), position);
							trivias.Add(trivia);

							if (_state.Deletes.TryPop(out var item))
							{
								item.FinishTrivia = trivia;
								_deletes.Add(item);
							}
							else
								_diagnostics.Add(SyntaxDiagnosticsFactory.UnexpectedEndDelete(trivia.Position));
						}
                        else
							trivias.Add(new(BslTriviaKind.UnknownDirective, value.ToString(), position));
					}
                    else
                    {
						if (ParserHelper.BilingualTokenValueIs(startText, "ИСПОЛЬЗОВАТЬ", "USE"))
						{
							var trivia = new BslTrivia(BslTriviaKind.UseDirective, preprocessorText[startTextRange.End.Value..].Trim().ToString(), position);
							trivias.Add(trivia);

							_usings.Add(new Using()
							{
								Trivia = trivia,
								Path = trivia.Value
							});
						}
						else if (ParserHelper.BilingualTokenValueIs(startText, "NATIVE", "NATIVE"))
						{
							var trivia = new BslTrivia(BslTriviaKind.Native, preprocessorText.ToString(), position);
							trivias.Add(trivia);

							IsNative = true;
						}
						else
							trivias.Add(new(BslTriviaKind.UnknownDirective, value.ToString(), position));
					}
                }
			}
            else
				trivias.Add(new(BslTriviaKind.UnknownDirective, value.ToString(), position));
		}

		private void SetCompileContext(ReadOnlySpan<char> preprocessorCondition, bool isIf)
		{
			var (Context, Errors) = BslDirectiveConditionCompiler.Compile(preprocessorCondition, new BslParserOptions()
			{
				LanguageKind = _options.LanguageKind
			});
			_state.CurrentCompileContext = Context;

			if (Errors.Count > 0)
				_diagnostics.AddRange(Errors);

			if (isIf)
				_state.CompileContexts.Add([_state.CurrentCompileContext]);
			else
				_state.CompileContexts[^1].Add(Context);
		}
	}
}