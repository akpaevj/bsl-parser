using BSL.AST.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Nodes.Expressions.Arithmetic
{
    public class UnaryExpressionNode(BslNode parent) : ExpressionNode(parent)
	{
		public BslToken OperatorToken { get; internal set; } = default;
		public ExpressionNode Operand { get; internal set; } = null!;

		public bool IsNegative => OperatorToken.Type == TokenType.MINUS;
	}
}
