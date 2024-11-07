using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes
{
	public class AttributeParameterNode(BslNode parent) : BslNode(parent)
	{
		public BslToken? NameToken { get; internal set; } = default;
		public BslToken? EqualToken { get; internal set; } = default;
		public ExpressionNode? ValueExpression { get; internal set; } = default;
	}
}
