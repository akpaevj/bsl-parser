using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class TryStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken TryKeyword { get; internal set; } = default;
		public BlockNode TryBlock { get; internal set; } = null!;
		public BslToken ExceptKeyword { get; internal set; } = default;
		public BlockNode ExceptBlock { get; internal set; } = null!;
		public BslToken EndTryKeyword { get; internal set; } = default;
	}
}
