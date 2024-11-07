using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ElseIfClauseNode(BslNode parent) : BslNode(parent)
	{
		public BslToken ElseIfKeyword { get; internal set; } = default;
		public ExpressionNode Condition { get; internal set; } = null!;
		public BslToken ThenKeyword { get; internal set; } = default;
		public BlockNode Block { get; internal set; } = null!;
	}
}
