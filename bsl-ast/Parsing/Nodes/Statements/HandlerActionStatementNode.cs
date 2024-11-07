using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;
using System.Linq.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class HandlerActionStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken HandlerActionKeyword { get; internal set; } = default;
		public ExpressionNode Event { get; internal set; } = null!;
		public BslToken CommaToken { get; internal set; } = default;
		public ExpressionNode Handler { get; internal set; } = null!;

		public bool IsAdding => HandlerActionKeyword.Type == TokenType.ADD_HANDLER;
	}
}
