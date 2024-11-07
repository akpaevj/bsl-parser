using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions.Logical
{
    public class NotExpressionNode(BslNode parent) : ExpressionNode(parent)
    {
		public BslToken OperatorToken { get; internal set; } = default;
		public ExpressionNode Operand { get; internal set; } = null!;
	}
}
