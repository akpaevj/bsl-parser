using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ExpressionStatementNode(BslNode parent) : StatementNode(parent)
	{
		public ExpressionNode Expression { get; internal set; } = null!;
	}
}
