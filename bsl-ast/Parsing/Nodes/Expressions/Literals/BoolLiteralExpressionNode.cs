using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions.Literals
{
    public class BoolLiteralExpressionNode(BslNode parent) : LiteralExpressionNode(parent)
    {
        public BslToken ValueToken { get; internal set; } = default;
        public bool Value { get; internal set; } = false;
	}
}
