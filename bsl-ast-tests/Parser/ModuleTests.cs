using BSL.AST.Parsing.Nodes;
using BSL.AST.Parsing.Nodes.Expressions;
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
		public void AwaitExpressionTest()
		{
			var source =
				@"Асинх Процедура МояПроцедура()
					Ждать Привет();
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				var statement = Assert.IsType<ExpressionStatementNode>(method.Body.Statements.First());
				Assert.IsType<AwaitExpressionNode>(statement.Expression);
			});
		}

		[Fact]
		public void ReturnStatementTest()
		{
			var source =
				@"Процедура Привет()
					Возврат;
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				var returnStatement = Assert.IsType<ReturnStatementNode>(method.Body.Statements.First());
				Assert.Null(returnStatement.Expression);
			});
		}

		[Fact]
		public void ReturnStatementWithExpressionTest()
		{
			var source =
				@"Функция Привет()
					Возврат ""Привет"";
				КонецФункции";

			TestHelper.MethodNodeTest(source, method =>
			{
				var returnStatement = Assert.IsType<ReturnStatementNode>(method.Body.Statements.First());
				Assert.NotNull(returnStatement.Expression);
			});
		}

		[Fact]
		public void EmptyProcedureNodeWithoutArgumentsTest()
		{
			var source = 
				@"Процедура ПолучитьИдентификатор()
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				Assert.Equal("ПолучитьИдентификатор", method.IdentifierToken.Text);
				Assert.False(method.IsAsync);
				Assert.False(method.IsFunction);
				Assert.False(method.IsExported);
				Assert.Empty(method.Parameters.Items);
				Assert.Empty(method.Body.Statements);
			});
		}

		[Fact]
		public void ExportedEmptyProcedureNodeWithoutArgumentsTest()
		{
			var source =
			@"Процедура ПолучитьИдентификатор() Экспорт
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				Assert.Equal("ПолучитьИдентификатор", method.IdentifierToken.Text);
				Assert.False(method.IsAsync);
				Assert.False(method.IsFunction);
				Assert.True(method.IsExported);
				Assert.Empty(method.Parameters.Items);
				Assert.Empty(method.Body.Statements);
			});
		}

		[Fact]
		public void EmptyFunctionNodeWithoutArgumentsTest()
		{
			var source =
				@"Функция ПолучитьИдентификатор()
				КонецФункции";

			TestHelper.MethodNodeTest(source, method =>
			{
				Assert.Equal("ПолучитьИдентификатор", method.IdentifierToken.Text);
				Assert.False(method.IsAsync);
				Assert.True(method.IsFunction);
				Assert.False(method.IsExported);
				Assert.Empty(method.Parameters.Items);
				Assert.Empty(method.Body.Statements);
			});
		}

		[Fact]
		public void EmptyAsyncProcedureNodeWithoutArgumentsTest()
		{
			var source =
				@"Асинх Процедура ПолучитьИдентификатор()
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				Assert.Equal("ПолучитьИдентификатор", method.IdentifierToken.Text);
				Assert.True(method.IsAsync);
				Assert.False(method.IsFunction);
				Assert.False(method.IsExported);
				Assert.Empty(method.Parameters.Items);
				Assert.Empty(method.Body.Statements);
			});
		}

		[Fact]
		public void ProcedureNodeTest()
		{
			var source =
				@"Процедура ПолучитьИдентификатор(Параметр1, Параметр2 = 2)
					А = Параметр2 + 1;
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				Assert.Equal("ПолучитьИдентификатор", method.IdentifierToken.Text);
				Assert.False(method.IsAsync);
				Assert.False(method.IsFunction);
				Assert.False(method.IsExported);

				Assert.Collection(method.Parameters.Items,
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


				Assert.Single(method.Body.Statements);
			});
		}

		[Fact]
		public void AttributeNodeTest()
		{
			var source =
				@"&НаСервере
				Процедура ПолучитьИдентификатор()
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				var attribute = Assert.Single(method.Attributes);
				Assert.Equal("НаСервере", attribute.IdentifierToken.Text);
			});
		}

		[Fact]
		public void AttributeNodeWithValueTest()
		{
			var source =
				@"&Перед(""ПолучитьИдентификатор"")
				Процедура Расш1_ПолучитьИдентификатор()
				КонецПроцедуры";

			TestHelper.MethodNodeTest(source, method =>
			{
				var attribute = Assert.Single(method.Attributes);
				Assert.Equal("Перед", attribute.IdentifierToken.Text);

				var parameter = Assert.Single(attribute.Parameters.Items);
				Assert.Null(parameter.NameToken);
				var literal = Assert.IsType<StringLiteralExpressionNode>(parameter.ValueExpression);
				Assert.Equal("ПолучитьИдентификатор", literal.Value);
			});
		}

		[Fact]
		public void MultipleMethodsTest()
		{
			var source =
				@"Функция Идентификатор()
					Возврат 1;
				КонецФункции
				
				Функция ПолучитьИдентификатор()
					Возврат Идентификатор();
				КонецФункции";

			TestHelper.ParseServerCommonModule(source);
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

			TestHelper.ParseClientCommonModule(source);
		}

// 		[Fact]
// 		public void NestedPreprocessorDirective()
// 		{
// 			var source =
// 				@"Процедура ПередНачаломРаботыСистемы()
// 	
// 				#Если Сервер Тогда
//
// 				#Иначе
//
// 				#Если МобильныйКлиент Тогда
// 					Если ОсновнойСерверДоступен() = Ложь Тогда
// 						Возврат;
// 					КонецЕсли;
// 				#КонецЕсли
// 	
// 					// СтандартныеПодсистемы
// 				#Если МобильныйКлиент Тогда
// 					Выполнить(""СтандартныеПодсистемыКлиент.ПередНачаломРаботыСистемы()"");
// 				#Иначе
// 					СтандартныеПодсистемыКлиент.ПередНачаломРаботыСистемы();
// 				#КонецЕсли
// 					// Конец СтандартныеПодсистемы
//
// 				#КонецЕсли
// 	
// 				КонецПроцедуры";
//
// 			var result = TestHelper.ParseClientServerCommonModule(source);
// 			Assert.Empty(result.Errors);
// 		}
	}
}
