using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
	public class ArgumentsNode(BslNode parent) : SeparatedListNode<ExpressionNode?>(parent)
	{
		public BslToken OpenParenToken { get; internal set; } = default;
		public BslToken CloseParenToken { get; internal set; } = default;
	}
}
