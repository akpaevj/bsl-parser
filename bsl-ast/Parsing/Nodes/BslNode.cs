using BSL.AST.Lexing;
using System.Collections;

namespace BSL.AST.Parsing.Nodes
{
	public abstract class BslNode(BslNode? parent = null)
	{
		public BslNode? Parent { get; internal set; } = parent;
		public SourcePosition Position { get; internal set; }
	}
}
