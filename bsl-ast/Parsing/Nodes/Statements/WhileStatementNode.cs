using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class WhileStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken WhileKeyword { get; internal set; } = default;
		public ExpressionNode Condition { get; internal set; } = null!;
		public BslToken DoKeyword { get; internal set; } = default;
		public BlockNode Body { get; internal set; } = null!;
		public BslToken EndDoKeyword { get; internal set; } = default;
	}
}
