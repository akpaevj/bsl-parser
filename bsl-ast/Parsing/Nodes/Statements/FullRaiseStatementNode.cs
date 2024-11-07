using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class FullRaiseStatementNode(BslNode parent) : RaiseStatementNode(parent)
	{
		public ArgumentsNode Arguments { get; internal set; } = null!;
	}
}
