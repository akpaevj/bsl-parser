using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions.Literals
{
    public class StringLiteralExpressionNode(BslNode parent) : LiteralExpressionNode(parent)
    {
		public List<BslToken> Parts { get; internal set; } = [];
		public string Value { get; internal set; } = string.Empty;
	}
}
