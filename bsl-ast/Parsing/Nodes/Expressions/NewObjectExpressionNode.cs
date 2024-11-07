using BSL.AST.Lexing;

namespace BSL.AST.Parsing.Nodes.Expressions
{
	public class NewObjectExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public BslToken NewKeyword { get; internal set; } = default;
		public BslToken? IdentifierToken { get; internal set; } = default;
		public ArgumentsNode Arguments { get; internal set; } = null!;

		public bool FunctionalForm => IdentifierToken == null;
	}
}
