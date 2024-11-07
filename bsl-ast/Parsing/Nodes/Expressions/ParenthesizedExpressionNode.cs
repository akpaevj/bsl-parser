using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
	public class ParenthesizedExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public BslToken OpenParenToken { get; internal set; } = default;
		public ExpressionNode Expression { get; internal set; } = null!;
		public BslToken CloseParenToken { get; internal set; } = default;
	}
}
