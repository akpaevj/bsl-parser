namespace BSL.AST.Parsing.Nodes.Statements
{
	public class BlockNode(BslNode parent) : BslNode(parent)
	{
		public List<BslNode> Statements { get; internal set; } = [];
	}
}
