using BSL.AST.Parsing.Nodes.Expressions;
using BSL.AST.Parsing.Nodes.Statements;
using BSL.AST.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSL.AST.Parsing.Nodes.Expressions.Literals;
using BSL.AST.Parsing.Nodes;

namespace BSL.AST.Tests.Parser
{
	public class OtherExpressionTests
	{
		[Fact]
		public void SimpleInvocationExpressionTest()
		{
			var source = "МояМеременная();";

			var statement = TestHelper.ExactSingleStatementNodeTest<ExpressionStatementNode>(source);
			Assert.IsType<InvocationExpressionNode>(statement.Expression);
		}

		[Fact]
		public void InvocationExpressionTest()
		{
			var source = "МояМеременная(1, \"2\");";

			var statement = TestHelper.ExactSingleStatementNodeTest<ExpressionStatementNode>(source);
			var invocation = Assert.IsType<InvocationExpressionNode>(statement.Expression);
			Assert.Equal(2, invocation.Arguments.Items.Count);
		}

		[Fact]
		public void SkippedArgumentsInvocationExpressionTest()
		{
			var source = "МояМеременная(1, , , \"2\");";

			var statement = TestHelper.ExactSingleStatementNodeTest<ExpressionStatementNode>(source);
			var invocation = Assert.IsType<InvocationExpressionNode>(statement.Expression);
			Assert.Equal(4, invocation.Arguments.Items.Count);
		}

		[Fact]
		public void ElementAccessExpressionTest()
		{
			var source = "А = МояМеременная[1];";

			var assignment = TestHelper.ExactSingleStatementNodeTest<AssignmentStatementNode>(source);
			var access = Assert.IsType<ElementAccessExpressionNode>(assignment.Expression);
			Assert.IsType<NumberLiteralExpressionNode>(access.Argument);
		}

		[Fact]
		public void ComplexExpressionTest()
		{
			var source = "А = Объект.Реквизит.МетодОбъектаРеквизита() + 1;";

			TestHelper.ModuleNodeTest(source);
		}

		[Fact]
		public void NewObjectExpressionTest()
		{
			var source = "А = Новый Объект();";

			var assignment = TestHelper.ExactSingleStatementNodeTest<AssignmentStatementNode>(source);
			var newObject = Assert.IsType<NewObjectExpressionNode>(assignment.Expression);
			Assert.False(newObject.FunctionalForm);
		}

		[Fact]
		public void FunctionalNewObjectExpressionTest()
		{
			var source = "А = Новый(Тип(\"Какойто\"), МассивПараметров);";

			var assignment = TestHelper.ExactSingleStatementNodeTest<AssignmentStatementNode>(source);
			var newObject = Assert.IsType<NewObjectExpressionNode>(assignment.Expression);
			Assert.True(newObject.FunctionalForm);
		}

		[Fact]
		public void InvalidNewObjectExpressionTest()
		{
			var source = "А = Новый Объект().Привет();";
			TestHelper.ModuleNodeTest(source, true);
		}

		[Fact]
		public void ConditionalExpressionTest()
		{
			var source = "А = ?(ИСТИНА, ИСТИНА, ЛОЖЬ);";

			var assignment = TestHelper.ExactSingleStatementNodeTest<AssignmentStatementNode>(source);
			Assert.IsType<ConditionalExpressionNode>(assignment.Expression);
		}

		[Fact]
		public void AwaitExpressionTest()
		{
			var source =
				@"Асинх Процедура МояПроцедура()
					Ждать Привет();
				КонецПроцедуры";

			var method = TestHelper.ExactSingleStatementNodeTest<MethodNode>(source);
			var statement = Assert.IsType<ExpressionStatementNode>(method.Body.Statements.First());
			Assert.IsType<AwaitExpressionNode>(statement.Expression);
		}

		[Fact]
		public void InvalidAwaitExpressionTest()
		{
			var source =
				@"Процедура МояПроцедура()
					Ждать Привет();
				КонецПроцедуры";

			TestHelper.ModuleNodeTest(source, true);
		}

		[Fact]
		public void InvocationExpressionWithSkippedArgumentTest()
		{
			var source = "ПредметОбъект.УстановитьСтатус(\"Подтвержден\", );";

			TestHelper.ExactSingleStatementNodeTest<ExpressionStatementNode>(source);
		}
	}
}
