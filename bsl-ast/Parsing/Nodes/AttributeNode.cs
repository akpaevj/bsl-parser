using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Nodes
{
	public class AttributeNode(BslNode parent) : BslNode(parent)
	{
		public BslToken AmpersandToken { get; internal set; } = default;
		public BslToken IdentifierToken { get; internal set; } = default;
		public AttributeParametersNode Parameters { get; internal set; } = null!;
	}
}
