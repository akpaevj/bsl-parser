using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public abstract class RaiseStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken RaiseKeword { get; internal set; } = default;
	}
}
