using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ReturnStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken ReturnKeyword { get; internal set; } = default;
		public ExpressionNode? Expression { get; internal set; } = null;
	}
}
