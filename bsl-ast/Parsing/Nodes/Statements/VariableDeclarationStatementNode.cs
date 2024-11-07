using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Statements
{


    public class VariableDeclarationStatementNode(BslNode parent) : StatementNode(parent)
    {
		public BslToken VariableKeyword { get; internal set; } = default;
		public SeparatedListNode<BslToken> Identifiers { get; internal set; } = null!;
		public BslToken? ExportKeyword { get; internal set; } = default;

		public List<AttributeNode> Attributes { get; internal set; } = [];

		public bool IsExported => ExportKeyword != null;
	}
}
