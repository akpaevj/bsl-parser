using BSL.AST.Parsing.Nodes.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Nodes
{
	public class ModuleNode : BslNode
	{
		public List<BslNode> Children { get; internal set; } = [];

		public List<Using> Usings { get; internal set; } = [];
		public List<Region> Regions { get; internal set; } = [];
	}
}
