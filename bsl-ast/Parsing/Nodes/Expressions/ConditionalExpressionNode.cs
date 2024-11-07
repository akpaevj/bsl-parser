using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
	public class ConditionalExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public BslToken QuestionMarkToken { get; internal set; } = default;
		public BslToken OpenParenToken { get; internal set; } = default;
		public ExpressionNode Condition { get; internal set; } = null!;
		public BslToken FirstCommaToken { get; internal set; } = default;
		public ExpressionNode IfTrue { get; internal set; } = null!;
		public BslToken SecondCommaToken { get; internal set; } = default;
		public ExpressionNode IfFalse { get; internal set; } = null!;
		public BslToken CloseParenToken { get; internal set; } = default;
	}
}
