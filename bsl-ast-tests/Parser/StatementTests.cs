using BSL.AST.Parsing.Nodes.Statements;
using BSL.AST.Parsing.Nodes;

namespace BSL.AST.Tests.Parser
{
	public class StatementTests
	{
		[Fact]
		public void AssignmentStatementTest()
		{
			var source = "А = 1;";
			TestHelper.ExactSingleStatementNodeTest<AssignmentStatementNode>(source);
		}

		[Fact]
		public void VariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная;";

			var statement = TestHelper.ExactSingleStatementNodeTest<VariableDeclarationStatementNode>(source);
			Assert.False(statement.IsExported);
		}

		[Fact]
		public void ExportedVariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная Экспорт;";

			var statement = TestHelper.ExactSingleStatementNodeTest<VariableDeclarationStatementNode>(source);
			Assert.True(statement.IsExported);
		}

		[Fact]
		public void MultipleVariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная, МояПеременная2;";
			TestHelper.ExactSingleStatementNodeTest<VariableDeclarationStatementNode>(source);
		}

		[Fact]
		public void ExportedMultipleVariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная, МояПеременная2 Экспорт;";

			var statement = TestHelper.ExactSingleStatementNodeTest<VariableDeclarationStatementNode>(source);
			Assert.True(statement.IsExported);
		}

		[Fact]
		public void EmptyForStatementTest()
		{
			var source =
				@"Для Сч = 0 По 10 Цикл
				КонецЦикла;";

			var statement = TestHelper.ExactSingleStatementNodeTest<ForStatementNode>(source);
			Assert.Empty(statement.Body.Statements);
		}

		[Fact]
		public void ForStatementTest()
		{
			var source =
				@"Для Сч = 0 По 10 Цикл
					А = 1;
					А = А + 1;
				КонецЦикла;";

			var statement = TestHelper.ExactSingleStatementNodeTest<ForStatementNode>(source);
			Assert.Equal(2, statement.Body.Statements.Count);
		}

		[Fact]
		public void EmptyForEachStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
				КонецЦикла;";

			var statement = TestHelper.ExactSingleStatementNodeTest<ForEachStatementNode>(source);
			Assert.Empty(statement.Body.Statements);
		}

		[Fact]
		public void ForEachStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
					А = 1;
					А = А + 1;
				КонецЦикла;";

			var statement = TestHelper.ExactSingleStatementNodeTest<ForEachStatementNode>(source);
			Assert.Equal(2, statement.Body.Statements.Count);
		}

		[Fact]
		public void BreakStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
					Прервать;
				КонецЦикла;";

			var statement = TestHelper.ExactSingleStatementNodeTest<ForEachStatementNode>(source);
			var child = Assert.Single(statement.Body.Statements);
			Assert.IsType<BreakStatementNode>(child);
		}

		[Fact]
		public void ContinueStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
					Продолжить;
				КонецЦикла;";

			var statement = TestHelper.ExactSingleStatementNodeTest<ForEachStatementNode>(source);
			var child = Assert.Single(statement.Body.Statements);
			Assert.IsType<ContinueStatementNode>(child);
		}

		[Fact]
		public void WhileStatementTest()
		{
			var source =
				@"Пока Истина Цикл
					Продолжить;
				КонецЦикла;";

			TestHelper.ExactSingleStatementNodeTest<WhileStatementNode>(source);
		}

		[Fact]
		public void LabelTest()
		{
			var source = @"~МояМетка:";

			TestHelper.ExactSingleStatementNodeTest<LabelNode>(source);
		}

		[Fact]
		public void GoToStatementTest()
		{
			var source = "Перейти ~МояМетка;";
			TestHelper.ExactSingleStatementNodeTest<GoToStatementNode>(source);
		}

		[Fact]
		public void ShortStatementWithoutExpressionTest()
		{
			var source = "ВызватьИсключение;";
			TestHelper.ExactSingleStatementNodeTest<ShortRaiseStatementNode>(source);
		}

		[Fact]
		public void ShortStatementTest()
		{
			var source = "ВызватьИсключение \"Текст исключения\";";
			TestHelper.ExactSingleStatementNodeTest<ShortRaiseStatementNode>(source);
		}

		[Fact]
		public void FullStatementTest()
		{
			var source = "ВызватьИсключение(\"Текст исключения\", КатегорияОшибки.ОшибкаСети);";
			TestHelper.ExactSingleStatementNodeTest<FullRaiseStatementNode>(source);
		}

		[Fact]
		public void AddHandlerStatementTest()
		{
			var source = "ДобавитьОбработчик Объект.Событие, МойОбработчик;";
			TestHelper.ExactSingleStatementNodeTest<HandlerActionStatementNode>(source);
		}

		[Fact]
		public void RemoveHandlerStatementTest()
		{
			var source = "УдалитьОбработчик Объект.Событие, МойОбработчик;";
			TestHelper.ExactSingleStatementNodeTest<HandlerActionStatementNode>(source);
		}

		[Fact]
		public void TryStatementTest()
		{
			var source =
				@"Попытка
					Сообщить(1);
				Исключение
					Сообщить(ОписаниеОшибки());
				КонецПопытки;";

			TestHelper.ExactSingleStatementNodeTest<TryStatementNode>(source);
		}

		[Fact]
		public void IfStatementTest()
		{
			var source =
				@"Если Истина Тогда
					Сообщить(""Если"");
				КонецЕсли;";

			var ifStatement = TestHelper.ExactSingleStatementNodeTest<IfStatementNode>(source);
			Assert.Empty(ifStatement.ElseIfClauses);
			Assert.Null(ifStatement.ElseClause);
		}

		[Fact]
		public void IfStatementWithElseTest()
		{
			var source =
				@"Если Истина Тогда
					Сообщить(""Если"");
				Иначе
					Сообщить(""Иначе"");
				КонецЕсли;";

			var ifStatement = TestHelper.ExactSingleStatementNodeTest<IfStatementNode>(source);
			Assert.Empty(ifStatement.ElseIfClauses);
			Assert.NotNull(ifStatement.ElseClause);
		}

		[Fact]
		public void ElseIfStatementTest()
		{
			var source =
				@"Если Истина Тогда
					Сообщить(""Если"");
				ИначеЕсли Ложь Тогда
					Сообщить(""Иначе если"");
				КонецЕсли;";

			var ifStatement = TestHelper.ExactSingleStatementNodeTest<IfStatementNode>(source);
			Assert.Single(ifStatement.ElseIfClauses);
			Assert.Null(ifStatement.ElseClause);

		}

		[Fact]
		public void ComplexIfStatementTest()
		{
			var source =
				@"Если Истина Тогда
					Сообщить(""Если"");
				ИначеЕсли Ложь Тогда
					Сообщить(""Иначе если"");
				ИначеЕсли Ложь Тогда
					Сообщить(""Иначе если 2"");
				ИначеЕсли Ложь Тогда
					Сообщить(""Иначе если 3"");
				Иначе
					Сообщить(""Иначе"");
				КонецЕсли;";

			var ifStatement = TestHelper.ExactSingleStatementNodeTest<IfStatementNode>(source);
			Assert.Equal(3, ifStatement.ElseIfClauses.Count);
			Assert.NotNull(ifStatement.ElseClause);
		}

		[Fact]
		public void ReturnStatementTest()
		{
			var source =
				@"Процедура Привет()
					Возврат;
				КонецПроцедуры";

			var module = TestHelper.ModuleNodeTest(source);
			var procedure = Assert.IsType<MethodNode>(module.Children.First());

			var returnStatement = Assert.IsType<ReturnStatementNode>(procedure.Body.Statements.First());
			Assert.Null(returnStatement.Expression);
		}

		[Fact]
		public void ReturnStatementWithExpressionTest()
		{
			var source =
				@"Функция Привет()
					Возврат ""Привет"";
				КонецФункции";

			var module = TestHelper.ModuleNodeTest(source);
			var function = Assert.IsType<MethodNode>(module.Children.First());

			var returnStatement = Assert.IsType<ReturnStatementNode>(function.Body.Statements.First());
			Assert.NotNull(returnStatement.Expression);
		}

		[Fact]
		public void SkippedSemicolonTest()
		{
			var source =
				@"Если Количество() = 0 Тогда
					Возврат
				КонецЕсли;";

			TestHelper.ModuleNodeTest(source);
		}

		[Fact]
		public void ExecuteStatementTest()
		{
			var source = "Выполнить(ИмяМетода + \"(\" + ПараметрыСтрока + \")\");";
			TestHelper.ModuleNodeTest(source);
		}

		[Fact]
		public void ExecuteStatementWithoutParentsTest()
		{
			var source = "Выполнить ИмяМетода + \"(\" + ПараметрыСтрока + \")\";";
			TestHelper.ModuleNodeTest(source);
		}

		[Fact]
		public void CommentJustAfterKeywordTest()
		{
			var source =
				@"Если Истина Тогда
					А = 1;
				ИначеЕсли Ложь Тогда//обычный подраздел
					ПодразделНайден = Ложь;
				КонецЕсли";

			TestHelper.ModuleNodeTest(source);
		}
	}
}
