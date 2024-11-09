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
			TestHelper.StatementNodeTest<AssignmentStatementNode>(source);
		}

		[Fact]
		public void VariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная;";

			TestHelper.StatementNodeTest<VariableDeclarationStatementNode>(source, node =>
			{
				Assert.False(node.IsExported);
			});
		}

		[Fact]
		public void ExportedVariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная Экспорт;";

			TestHelper.StatementNodeTest<VariableDeclarationStatementNode>(source, node =>
			{
				Assert.True(node.IsExported);
			});
		}

		[Fact]
		public void MultipleVariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная, МояПеременная2;";

			TestHelper.StatementNodeTest<VariableDeclarationStatementNode>(source, node =>
			{
				Assert.Equal(2, node.Identifiers.Items.Count);
			});
		}

		[Fact]
		public void ExportedMultipleVariableDeclarationStatementTest()
		{
			var source = "Перем МояМеременная, МояПеременная2 Экспорт;";

			TestHelper.StatementNodeTest<VariableDeclarationStatementNode>(source, node =>
			{
				Assert.True(node.IsExported);
				Assert.Equal(2, node.Identifiers.Items.Count);
			});
		}

		[Fact]
		public void EmptyForStatementTest()
		{
			var source =
				@"Для Сч = 0 По 10 Цикл
				КонецЦикла;";

			TestHelper.StatementNodeTest<ForStatementNode>(source, node =>
			{
				Assert.Empty(node.Body.Statements);
			});
		}

		[Fact]
		public void ForStatementTest()
		{
			var source =
				@"Для Сч = 0 По 10 Цикл
					А = 1;
					А = А + 1;
				КонецЦикла;";

			TestHelper.StatementNodeTest<ForStatementNode>(source, node =>
			{
				Assert.Equal(2, node.Body.Statements.Count);
			});
		}

		[Fact]
		public void EmptyForEachStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
				КонецЦикла;";

			TestHelper.StatementNodeTest<ForEachStatementNode>(source, node =>
			{
				Assert.Empty(node.Body.Statements);
			});
		}

		[Fact]
		public void ForEachStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
					А = 1;
					А = А + 1;
				КонецЦикла;";

			TestHelper.StatementNodeTest<ForEachStatementNode>(source, node =>
			{
				Assert.Equal(2, node.Body.Statements.Count);
			});
		}

		[Fact]
		public void BreakStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
					Прервать;
				КонецЦикла;";

			TestHelper.StatementNodeTest<ForEachStatementNode>(source, node =>
			{
				var child = Assert.Single(node.Body.Statements);
				Assert.IsType<BreakStatementNode>(child);
			});
		}

		[Fact]
		public void ContinueStatementTest()
		{
			var source =
				@"Для Каждого Строка Из МояТаблица Цикл
					Продолжить;
				КонецЦикла;";

			TestHelper.StatementNodeTest<ForEachStatementNode>(source, node =>
			{
				var child = Assert.Single(node.Body.Statements);
				Assert.IsType<ContinueStatementNode>(child);
			});
		}

		[Fact]
		public void WhileStatementTest()
		{
			var source =
				@"Пока Истина Цикл
					Продолжить;
				КонецЦикла;";

			TestHelper.StatementNodeTest<WhileStatementNode>(source);
		}

		[Fact]
		public void LabelTest()
		{
			var source = @"~МояМетка:";

			TestHelper.StatementNodeTest<LabelNode>(source);
		}

		[Fact]
		public void GoToStatementTest()
		{
			var source = "Перейти ~МояМетка;";

			TestHelper.StatementNodeTest<GoToStatementNode>(source);
		}

		[Fact]
		public void ShortStatementWithoutExpressionTest()
		{
			var source = "ВызватьИсключение;";

			TestHelper.StatementNodeTest<ShortRaiseStatementNode>(source);
		}

		[Fact]
		public void ShortStatementTest()
		{
			var source = "ВызватьИсключение \"Текст исключения\";";

			TestHelper.StatementNodeTest<ShortRaiseStatementNode>(source);
		}

		[Fact]
		public void FullStatementTest()
		{
			var source = "ВызватьИсключение(\"Текст исключения\", КатегорияОшибки.ОшибкаСети);";

			TestHelper.StatementNodeTest<FullRaiseStatementNode>(source);
		}

		[Fact]
		public void AddHandlerStatementTest()
		{
			var source = "ДобавитьОбработчик Объект.Событие, МойОбработчик;";

			TestHelper.StatementNodeTest<HandlerActionStatementNode>(source);
		}

		[Fact]
		public void RemoveHandlerStatementTest()
		{
			var source = "УдалитьОбработчик Объект.Событие, МойОбработчик;";

			TestHelper.StatementNodeTest<HandlerActionStatementNode>(source);
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

			TestHelper.StatementNodeTest<TryStatementNode>(source);
		}

		[Fact]
		public void IfStatementTest()
		{
			var source =
				@"Если Истина Тогда
					Сообщить(""Если"");
				КонецЕсли;";

			TestHelper.StatementNodeTest<IfStatementNode>(source, node =>
			{
				Assert.Empty(node.ElseIfClauses);
				Assert.Null(node.ElseClause);
			});
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

			TestHelper.StatementNodeTest<IfStatementNode>(source, node =>
			{
				Assert.Empty(node.ElseIfClauses);
				Assert.NotNull(node.ElseClause);
			});
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

			TestHelper.StatementNodeTest<IfStatementNode>(source, node =>
			{
				Assert.Single(node.ElseIfClauses);
				Assert.Null(node.ElseClause);
			});
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

			TestHelper.StatementNodeTest<IfStatementNode>(source, node =>
			{
				Assert.Equal(3, node.ElseIfClauses.Count);
				Assert.NotNull(node.ElseClause);
			});
		}

		[Fact]
		public void SkippedSemicolonTest()
		{
			var source =
				@"Если Количество() = 0 Тогда
					Возврат
				КонецЕсли;";

			TestHelper.StatementNodeTest<IfStatementNode>(source);
		}

		[Fact]
		public void ExecuteStatementTest()
		{
			var source = "Выполнить(ИмяМетода + \"(\" + ПараметрыСтрока + \")\");";
			TestHelper.StatementNodeTest<ExecuteStatementNode>(source);
		}

		[Fact]
		public void ExecuteStatementWithoutParentsTest()
		{
			var source = "Выполнить ИмяМетода + \"(\" + ПараметрыСтрока + \")\";";
			TestHelper.StatementNodeTest<ExecuteStatementNode>(source);
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

			TestHelper.StatementNodeTest<IfStatementNode>(source);
		}
	}
}
