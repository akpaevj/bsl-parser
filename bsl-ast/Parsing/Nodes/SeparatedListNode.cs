using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes
{
    public class SeparatedListNode<T>(BslNode parent) : BslNode(parent)
    {
        public IReadOnlyList<BslToken> Separators { get; internal set; } = [];
        public IReadOnlyList<T> Items { get; internal set; } = [];
    }
}
