using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class BreakStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken BreakKeyword { get; internal set; } = default;
	}
}
