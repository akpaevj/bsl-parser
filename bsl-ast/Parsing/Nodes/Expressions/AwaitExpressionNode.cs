using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
	public class AwaitExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public BslToken AwaitToken { get; internal set; } = default;
		public ExpressionNode Expression { get; internal set; } = null!;
	}
}
