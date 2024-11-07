using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
	public class ElementAccessExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public ExpressionNode Member { get; internal set; } = null!;
		public BslToken OpenBracketToken { get; internal set; } = default;
		public ExpressionNode Argument { get; internal set; } = null!;
		public BslToken CloseBracketToken { get; internal set; } = default;
	}
}
