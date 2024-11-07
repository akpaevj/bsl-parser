using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions.Literals
{
    public class NumberLiteralExpressionNode(BslNode parent) : LiteralExpressionNode(parent)
    {
		public BslToken ValueToken { get; internal set; } = default;
		public decimal Value { get; internal set; } = 0;
    }
}
