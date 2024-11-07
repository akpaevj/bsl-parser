using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class IfStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken IfKeyword { get; internal set; } = default;
		public ExpressionNode Condition { get; internal set; } = null!;
		public BslToken ThenKeyword { get; internal set; } = default;
		public BlockNode IfBlock { get; internal set; } = null!;
		public List<ElseIfClauseNode> ElseIfClauses { get; internal set; } = [];
		public ElseClauseNode? ElseClause { get; internal set; } = null;
		public BslToken EndIfKeyword { get; internal set; } = default;
	}
}
