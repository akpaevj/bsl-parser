using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;
using BSL.AST.Parsing.Nodes.Expressions.Literals;
using BSL.AST.Parsing.Nodes.Expressions.Logical;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace BSL.AST.Tests.Parser
{
	public class LogicalExpressionTests
    {
		[Fact]
		public void AndExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<AndExpressionNode, BoolLiteralExpressionNode, BoolLiteralExpressionNode>("ИСТИНА И ИСТИНА");

		[Fact]
		public void EqualsExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<EqualsExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("1 = 1");

		[Fact]
		public void NotEqualsExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<NotEqualsExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("1 <> 1");

		[Fact]
		public void OrExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<OrExpressionNode, BoolLiteralExpressionNode, BoolLiteralExpressionNode>("ИСТИНА ИЛИ ИСТИНА");

		[Fact]
		public void NotExpressionNodeTest()
			=> TestHelper.ExpressionNodeTest<NotExpressionNode>("НЕ ИСТИНА;", node =>
			{
				Assert.IsType<BoolLiteralExpressionNode>(node.Operand);
			});

		[Fact]
		public void GreaterExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<GreaterExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("1 > 2");

		[Fact]
		public void GreaterOrEqualsExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<GreaterOrEqualsExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("1 >= 2");

		[Fact]
		public void LessExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<LessExpressionNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("1 < 2");

		[Fact]
		public void LessOrEqualsExpressionNodeTest()
			=> TestHelper.BinaryExpressionNodeTest<LessOrEqualsExpressioNode, NumberLiteralExpressionNode, NumberLiteralExpressionNode>("1 <= 2");

		[Fact]
		public void ComplexLogicalExpressionNodeTest()
			=> TestHelper.ExpressionNodeTest<NotExpressionNode>("НЕ (1 > 2 ИЛИ (2 < 3 И ИСТИНА))", node =>
			{
				var parenthesized = Assert.IsType<ParenthesizedExpressionNode>(node.Operand);
				var or = Assert.IsType<OrExpressionNode>(parenthesized.Expression);
				var greater = Assert.IsType<GreaterExpressionNode>(or.Left);
				var innerParenthesized = Assert.IsType<ParenthesizedExpressionNode>(or.Right);
				var and = Assert.IsType<AndExpressionNode>(innerParenthesized.Expression);
				var less = Assert.IsType<LessExpressionNode>(and.Left);
				Assert.IsType<BoolLiteralExpressionNode>(and.Right);
			});
	}
}
