using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ExecuteStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken ExecuteKeyword { get; internal set; } = default;
		public BslToken OpenParent { get; internal set; } = default;
		public ExpressionNode Expression { get; internal set; } = null!;
		public BslToken CloesParent { get; internal set; } = default;
	}
}
