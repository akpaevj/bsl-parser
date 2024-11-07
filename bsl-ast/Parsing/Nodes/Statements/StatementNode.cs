using BSL.AST.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class StatementNode(BslNode parent) : BslNode(parent)
	{
		public BslToken? SemicolonToken { get; internal set; } = null;
	}
}
