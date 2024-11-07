using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions.Literals
{
    public class NullLiteralExpressionNode(BslNode parent) : LiteralExpressionNode(parent)
    {
		public BslToken ValueToken { get; internal set; } = default;
	}
}
