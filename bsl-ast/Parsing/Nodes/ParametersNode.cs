using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes
{
    public class ParametersNode(BslNode parent) : SeparatedListNode<ParameterNode>(parent)
    {
        public BslToken OpenParenToken { get; internal set; } = default;
        public BslToken CloseParenToken { get; internal set; } = default;
    }
}
