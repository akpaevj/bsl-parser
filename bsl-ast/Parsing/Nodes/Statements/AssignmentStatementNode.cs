using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class AssignmentStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken IdentifierToken { get; internal set; } = default;
		public BslToken EqualToken { get; internal set; } = default;
		public ExpressionNode Expression { get; internal set; } = null!;
	}
}
