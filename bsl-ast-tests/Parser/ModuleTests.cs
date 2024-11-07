using BSL.AST.Parsing.Nodes;
using BSL.AST.Parsing.Nodes.Expressions.Literals;
using BSL.AST.Parsing.Nodes.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Tests.Parser
{
	public class ModuleTests
	{
		[Fact]
		public void EmptyProcedureNodeWithoutArgumentsTest()
		{
			var source = 
				@"Процедура ПолучитьИдентификатор()
				КонецПроцедуры";

			var module = TestHelper.ModuleNodeTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);
			Assert.Equal("ПолучитьИдентификатор", procedure.IdentifierToken.Text);
			Assert.False(procedure.IsAsync);
			Assert.False(procedure.IsFunction);
			Assert.False(procedure.IsExported);
			Assert.Empty(procedure.Parameters.Items);
			Assert.Empty(procedure.Body.Statements);
		}

		[Fact]
		public void ExportedEmptyProcedureNodeWithoutArgumentsTest()
		{
			var source =
			@"Процедура ПолучитьИдентификатор() Экспорт
				КонецПроцедуры";

			var module = TestHelper.ModuleNodeTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);
			Assert.Equal("ПолучитьИдентификатор", procedure.IdentifierToken.Text);
			Assert.False(procedure.IsAsync);
			Assert.False(procedure.IsFunction);
			Assert.True(procedure.IsExported);
			Assert.Empty(procedure.Parameters.Items);
			Assert.Empty(procedure.Body.Statements);
		}

		[Fact]
		public void EmptyFunctionNodeWithoutArgumentsTest()
		{
			var source =
				@"Функция ПолучитьИдентификатор()
				КонецФункции";

			var module = TestHelper.ModuleNodeTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);
			Assert.Equal("ПолучитьИдентификатор", procedure.IdentifierToken.Text);
			Assert.False(procedure.IsAsync);
			Assert.True(procedure.IsFunction);
			Assert.False(procedure.IsExported);
			Assert.Empty(procedure.Parameters.Items);
			Assert.Empty(procedure.Body.Statements);
		}

		[Fact]
		public void EmptyAsyncProcedureNodeWithoutArgumentsTest()
		{
			var source =
				@"Асинх Процедура ПолучитьИдентификатор()
				КонецПроцедуры";

			var module = TestHelper.ModuleNodeTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);
			Assert.Equal("ПолучитьИдентификатор", procedure.IdentifierToken.Text);
			Assert.True(procedure.IsAsync);
			Assert.False(procedure.IsFunction);
			Assert.False(procedure.IsExported);
			Assert.Empty(procedure.Parameters.Items);
			Assert.Empty(procedure.Body.Statements);
		}

		[Fact]
		public void ProcedureNodeTest()
		{
			var source =
				@"Процедура ПолучитьИдентификатор(Параметр1, Параметр2 = 2)
					А = Параметр2 + 1;
				КонецПроцедуры";

			var module = TestHelper.ModuleNodeTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);
			Assert.Equal("ПолучитьИдентификатор", procedure.IdentifierToken.Text);
			Assert.False(procedure.IsAsync);
			Assert.False(procedure.IsFunction);
			Assert.False(procedure.IsExported);

			Assert.Collection(procedure.Parameters.Items,
				i =>
				{
					Assert.Equal("Параметр1", i.IdentifierToken.Text);
					Assert.False(i.HasDefaultValue);
				},
				i =>
				{
					Assert.Equal("Параметр2", i.IdentifierToken.Text);

					var expression = Assert.IsType<NumberLiteralExpressionNode>(i.DefaultExpression);
					Assert.True(i.HasDefaultValue);
					Assert.Equal(2, expression.Value);
				});
			

			Assert.Single(procedure.Body.Statements);
		}

		[Fact]
		public void AttributeNodeTest()
		{
			var source =
				@"&НаСервере
				Процедура ПолучитьИдентификатор()
				КонецПроцедуры";

			var module = TestHelper.ModuleNodeTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);
			
			var attribute = Assert.Single(procedure.Attributes);
			Assert.Equal("НаСервере", attribute.IdentifierToken.Text);
		}

		[Fact]
		public void AttributeNodeWithValueTest()
		{
			var source =
				@"&Перед(""ПолучитьИдентификатор"")
				Процедура Расш1_ПолучитьИдентификатор()
				КонецПроцедуры";

			var module = TestHelper.ModuleNodeTest(source);
			var child = Assert.Single(module.Children);

			var procedure = Assert.IsType<MethodNode>(child);
			
			var attribute = Assert.Single(procedure.Attributes);
			Assert.Equal("Перед", attribute.IdentifierToken.Text);

			var parameter = Assert.Single(attribute.Parameters.Items);
			Assert.Null(parameter.NameToken);
			var literal = Assert.IsType<StringLiteralExpressionNode>(parameter.ValueExpression);
			Assert.Equal("ПолучитьИдентификатор", literal.Value);
		}

		[Fact]
		public void FormModuleTest()
		{
			var source =
				@"&НаКлиенте
				Перем СтартовыйКПП, СтартовоеНаименование, СтартовыйАдрес;

				&НаКлиенте
				Процедура ПриОткрытии(Отказ)
	
					Если ВладелецФормы = Неопределено Тогда
						Отказ = Истина;
						ТекстПредупреждения = НСтр(""ru = 'Данная форма вспомогательная, предназначена для редактирования данных
														|из форм регламентированных отчетов!';
														|en = 'Данная форма вспомогательная, предназначена для редактирования данных
														|из форм регламентированных отчетов!'"");
						ПоказатьПредупреждение(, ТекстПредупреждения, , НСтр(""ru = 'Форма ввода реквизитов ОП.';
																			|en = 'Форма ввода реквизитов ОП.'""));
						Возврат;	
					КонецЕсли;
	
					СправочникиВидыКонтактнойИнформацииФактАдресОрганизации = ВладелецФормы.СтруктураРеквизитовФормы.СправочникиВидыКонтактнойИнформации.ТолькоНациональныйАдрес;
	
					СтартовыйКПП			= СокрЛП(КПП);
					СтартовоеНаименование	= ?(Элементы.Наименование.Видимость, СокрЛП(Наименование), СокрЛП(НаименованиеВиноградной));
					СтартовыйАдрес			= СокрЛП(АдресПредставление);	
	
				КонецПроцедуры";

			TestHelper.ModuleNodeTest(source);
		}
	}
}
