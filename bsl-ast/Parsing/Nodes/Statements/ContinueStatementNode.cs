using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ContinueStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken ContinueKeyword { get; internal set; } = default;
	}
}
