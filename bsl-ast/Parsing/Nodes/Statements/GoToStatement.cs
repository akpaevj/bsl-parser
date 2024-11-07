using BSL.AST.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class GoToStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken GoToKeyword { get; internal set; } = default;
		public BslToken MarkToken { get; internal set; } = default;
		public BslToken IdentifierToken { get; internal set; } = default;
	}
}
