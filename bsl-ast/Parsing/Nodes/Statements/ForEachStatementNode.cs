using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;

namespace BSL.AST.Parsing.Nodes.Statements
{
	public class ForEachStatementNode(BslNode parent) : StatementNode(parent)
	{
		public BslToken ForKeyword { get; internal set; } = default;
		public BslToken EachKeyword { get; internal set; } = default;
		public BslToken IdentifierToken { get; internal set; } = default;
		public BslToken InKeyword { get; internal set; } = default;
		public ExpressionNode Collection { get; internal set; } = null!;
		public BslToken DoKeyword { get; internal set; } = default;
		public BlockNode Body { get; internal set; } = null!;
		public BslToken EndDoKeyword { get; internal set; } = default;
	}
}
