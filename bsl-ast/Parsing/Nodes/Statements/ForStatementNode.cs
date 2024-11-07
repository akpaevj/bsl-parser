using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ForStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken ForKeyword { get; internal set; } = default;
		public BslToken IdentifierToken { get; internal set; } = default;
		public BslToken EqualToken { get; internal set; } = default;
		public ExpressionNode InitExpression { get; internal set; } = null!;
		public BslToken ToKeyword { get; internal set; } = default;
		public ExpressionNode Condition { get; internal set; } = null!;
		public BslToken DoKeyword { get; internal set; } = default;
		public BlockNode Body { get; internal set; } = null!;
		public BslToken EndDoKeyword { get; internal set; } = default;
	}
}
