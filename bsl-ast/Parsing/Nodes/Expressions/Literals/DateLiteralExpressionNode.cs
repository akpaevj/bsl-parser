using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions.Literals
{
    public class DateLiteralExpressionNode(BslNode parent) : LiteralExpressionNode(parent)
    {
		public BslToken ValueToken { get; internal set; } = default;
		public DateTime Value { get; internal set; } = DateTime.MinValue;
    }
}
