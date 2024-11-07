using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes;

namespace BSL.AST.Parsing
{
	public class SkippedTokensNode(BslNode parent) : BslNode(parent)
	{
		public List<BslToken> Tokens { get; internal set; } = [];
	}
}