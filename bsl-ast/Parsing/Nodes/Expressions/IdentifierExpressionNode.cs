using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
    public class IdentifierExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public BslToken IdentifierToken { get; internal set; } = default;
	}
}
