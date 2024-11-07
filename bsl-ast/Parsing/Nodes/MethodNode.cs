using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Statements;

namespace BSL.AST.Parsing.Nodes
{
    public class MethodNode(BslNode parent) : BslNode(parent)
    {
        public BslToken? AsyncToken { get; internal set; } = default;
        public BslToken MethodKeyword { get; internal set; } = default;
        public BslToken IdentifierToken { get; internal set; } = default;
        public ParametersNode Parameters { get; internal set; } = null!;
        public BslToken? ExportKeyword { get; internal set; } = default;
        public BlockNode Body { get; internal set; } = null!;
        public BslToken EndMethodKeyword { get; internal set; } = default;

        public bool IsFunction => MethodKeyword.Type == TokenType.FUNCTION;
        public bool IsAsync => AsyncToken != null;
		public bool IsExported => ExportKeyword != null;
        public List<AttributeNode> Attributes { get; internal set; } = [];
	}
}
