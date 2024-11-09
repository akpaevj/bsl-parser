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
using BSL.AST.Parsing.Nodes.Expressions.Arithmetic;

namespace BSL.AST.Tests.Parser
{
	public class OtherExpressionTests
	{
		[Fact]
		public void SimpleInvocationExpressionTest()
		{
			var source = "МояМеременная()";
			TestHelper.ExpressionNodeTest<InvocationExpressionNode>(source);
		}

		[Fact]
		public void InvocationExpressionTest()
		{
			var source = "МояМеременная(1, \"2\")";

			TestHelper.ExpressionNodeTest<InvocationExpressionNode>(source, node =>
			{
				Assert.Equal(2, node.Arguments.Items.Count);
			});
		}

		[Fact]
		public void SkippedArgumentsInvocationExpressionTest()
		{
			var source = "МояМеременная(1, , , \"2\")";

			TestHelper.ExpressionNodeTest<InvocationExpressionNode>(source, node =>
			{
				Assert.Equal(4, node.Arguments.Items.Count);
			});
		}

		[Fact]
		public void ElementAccessExpressionTest()
			=> TestHelper.ExpressionNodeTest<ElementAccessExpressionNode>("МояМеременная[1]", expression =>
			{
				Assert.IsType<NumberLiteralExpressionNode>(expression.Argument);
			});

		[Fact]
		public void ComplexExpressionTest()
			=> TestHelper.ExpressionNodeTest<AddExpressionNode>("Объект.Реквизит.МетодОбъектаРеквизита() + 1");

		[Fact]
		public void NewObjectExpressionTest()
		{
			var source = "Новый Объект()";

			TestHelper.ExpressionNodeTest<NewObjectExpressionNode>(source, node =>
			{
				Assert.False(node.FunctionalForm);
			});
		}

		[Fact]
		public void FunctionalNewObjectExpressionTest()
		{
			var source = "Новый(Тип(\"Какойто\"), МассивПараметров)";

			TestHelper.ExpressionNodeTest<NewObjectExpressionNode>(source, node =>
			{
				Assert.True(node.FunctionalForm);
			});
		}

		[Fact]
		public void ConditionalExpressionTest()
		{
			var source = "?(ИСТИНА, ИСТИНА, ЛОЖЬ)";
			TestHelper.ExpressionNodeTest<ConditionalExpressionNode>(source);
		}

		[Fact]
		public void InvalidAwaitExpressionTest()
		{
			var source =
				@"Процедура МояПроцедура()
					Ждать Привет();
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, null, true);
		}

		[Fact]
		public void InvocationExpressionWithSkippedArgumentTest()
		{
			var source = "ПредметОбъект.УстановитьСтатус(\"Подтвержден\", );";

			TestHelper.StatementNodeTest<ExpressionStatementNode>(source);
		}
	}
}
