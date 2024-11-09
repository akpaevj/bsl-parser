using BSL.AST.Parsing;
using BSL.AST.Parsing.Nodes;
using BSL.AST.Parsing.Nodes.Expressions;
using BSL.AST.Parsing.Nodes.Expressions.Literals;

namespace BSL.AST.Tests.Parser
{
	public class OScriptSpecificTests
	{
		/// <summary>
		/// ОСкрипт позволяет разворачивать цевочку выражений сразу после создания нового объекта, без присваивания результата оператора New новой переменной
		/// </summary>
		[Fact]
		public void NewObjectExpressionTest()
		{
			var source = "Новый Объект().Привет()";
			TestHelper.ExpressionNodeTest<AccessMemberExpressionNode>(source, node =>
			{
				Assert.IsType<NewObjectExpressionNode>(node.Left);
			}, BslKind.OneScript);
		}

		[Fact]
		public void AttributeParameterNodeWithIdentifierTest()
		{
			var source =
				@"&КастомнаяАннотация(Параметр)
				Процедура Расш1_ПолучитьИдентификатор()
				КонецПроцедуры";

			var module = TestHelper.OscriptModuleTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);

			var attribute = Assert.Single(procedure.Attributes);
			Assert.Equal("КастомнаяАннотация", attribute.IdentifierToken.Text);

			var parameter = Assert.Single(attribute.Parameters.Items);
			Assert.Null(parameter.ValueExpression);
			Assert.Equal("Параметр", parameter.NameToken!.Text);
		}

		[Fact]
		public void AttributeParameterNodeFullTest()
		{
			var source =
				@"&КастомнаяАннотация(Параметр = 1)
				Процедура Расш1_ПолучитьИдентификатор()
				КонецПроцедуры";

			var module = TestHelper.OscriptModuleTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);

			var attribute = Assert.Single(procedure.Attributes);
			Assert.Equal("КастомнаяАннотация", attribute.IdentifierToken.Text);

			var parameter = Assert.Single(attribute.Parameters.Items);
			Assert.Equal("Параметр", parameter.NameToken!.Text);
			var valueExpression = Assert.IsType<NumberLiteralExpressionNode>(parameter.ValueExpression);
			Assert.Equal(1, valueExpression.Value);
		}
	}
}
