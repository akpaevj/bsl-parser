using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes
{
    public class ParameterNode(BslNode parent) : BslNode(parent)
    {
        public BslToken? ValKeyword { get; internal set; } = default;
        public BslToken IdentifierToken { get; internal set; } = default;
		public BslToken? EqualToken { get; internal set; } = default;
		public ExpressionNode? DefaultExpression { get; internal set; } = null;

        public bool HasDefaultValue => DefaultExpression != null;
		public List<AttributeNode> Attributes { get; internal set; } = [];
	}
}
