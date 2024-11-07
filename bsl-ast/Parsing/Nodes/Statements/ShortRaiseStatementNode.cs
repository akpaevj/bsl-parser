using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ShortRaiseStatementNode(BslNode parent) : RaiseStatementNode(parent)
	{
		public ExpressionNode? Expression { get; internal set; } = null!;
	}
}
