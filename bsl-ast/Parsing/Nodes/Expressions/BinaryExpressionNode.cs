using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
    public abstract class BinaryExpressionNode(BslNode parent) : ExpressionNode(parent)
    {
        public ExpressionNode Left { get; internal set; } = null!;
        public BslToken OperatorToken { get; internal set; } = null!;
        public ExpressionNode Right { get; internal set; } = null!;
    }
}
