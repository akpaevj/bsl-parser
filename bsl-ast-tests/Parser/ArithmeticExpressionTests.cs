using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions.Arithmetic;
using BSL.AST.Parsing.Nodes.Expressions.Literals;
using BSL.AST.Parsing.Nodes.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Tests.Parser
{
	public class ArithmeticExpressionTests
    {
        [Fact]
        public void AddExpressionNodeTest()
            => TestHelper.BinaryExpressionNodeTest<AddExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("А = 1 + 2;");

        [Fact]
        public void DivideExpressionNodeTest()
            => TestHelper.BinaryExpressionNodeTest<DivideExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("А = 1 / 2;");

        [Fact]
        public void ModuloExpressionNodeTest()
            => TestHelper.BinaryExpressionNodeTest<ModuloExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("А = 1 % 2;");

        [Fact]
        public void SubtractExpressionNodeTest()
            => TestHelper.BinaryExpressionNodeTest<SubtractExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("А = 1 - 2;");

        [Fact]
        public void MultiplyExpressionNodeTest()
            => TestHelper.BinaryExpressionNodeTest<MultiplyExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("А = 1 * 2;");

        [Fact]
        public void PositiveUnaryExpressionNodeTest()
            => TestHelper.CheckAssignmentStatementNode<UnaryExpressionNode>("А = +1;", expression =>
            {
                Assert.False(expression.IsNegative);
                Assert.IsType<NumberLiteralExpressionNode>(expression.Operand);
            });

        [Fact]
        public void NegativeUnaryExpressionNodeTest()
            => TestHelper.CheckAssignmentStatementNode<UnaryExpressionNode>("А = -1;", expression =>
            {
				Assert.True(expression.IsNegative);
				Assert.IsType<NumberLiteralExpressionNode>(expression.Operand);
            });

    }
}
