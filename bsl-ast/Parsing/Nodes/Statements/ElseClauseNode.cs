using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ElseClauseNode(BslNode parent) : BslNode(parent)
	{
		public BslToken ElseKeyword { get; internal set; } = default;
		public BlockNode Block { get; internal set; } = null!;
	}
}
