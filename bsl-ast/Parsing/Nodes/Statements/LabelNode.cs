using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class LabelNode(BslNode parent) : BslNode(parent)
	{
		public BslToken MarkToken { get; internal set; } = default;
		public BslToken IdentifierToken { get; internal set; } = default;
		public BslToken EndOfMarkToken { get; internal set; } = default;
	}
}
