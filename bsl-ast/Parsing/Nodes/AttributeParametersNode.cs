using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes
{
	public class AttributeParametersNode(BslNode parent) : SeparatedListNode<AttributeParameterNode>(parent)
	{
		public BslToken OpenParenToken { get; internal set; } = default;
		public BslToken CloseParenToken { get; internal set; } = default;
	}
}
