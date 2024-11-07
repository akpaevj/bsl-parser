namespace BSL.AST.Parsing.Nodes.Expressions
{
	public class InvocationExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public ExpressionNode Member { get; internal set; } = null!;
		public ArgumentsNode Arguments { get; internal set; } = null!;
	}
}
