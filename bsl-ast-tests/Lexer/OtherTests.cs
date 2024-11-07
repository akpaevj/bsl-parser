using BSL.AST.Lexing;

namespace BSL.AST.Tests.Lexer
{
    public class OtherTests
    {
		[Fact]
        public void WhitespaceTrivia()
        {
            var value = "   \t\t ";
            TestHelper.CheckSingleTrivia(value, BslTriviaKind.WhiteSpace, TokenType.EOF, value);
        }

		[Fact]
		public void CommentTrivia()
		{
			var value = "// Привет, как дела";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.Comment, TokenType.EOF, value);
		}

		[Fact]
		public void EndOfLineTrivia()
		{
			var value = "\r\n";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.EndOfLine, TokenType.EOF, value, 2);
		}

		[Fact]
		public void InvalidUseTrivia()
		{
			var value = "#Использовать \".\"";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.UnknownDirective, TokenType.EOF, value);
		}

		[Fact]
		public void NativeDirectiveTrivia()
		{
			var value = "#Native";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.Native, TokenType.EOF, "Native", 1, Parsing.BslKind.OneScript);
		}

		[Fact]
		public void RegionTrivia()
		{
			var value = "#Область ПрограммныйИнтерфейс";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.RegionDirective, TokenType.EOF, "ПрограммныйИнтерфейс");
		}

		[Fact]
		public void EndRegionTrivia()
		{
			var value = "#КонецОбласти";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.EndRegionDirective, TokenType.EOF, "КонецОбласти");
		}

		[Fact]
		public void PreprocessorTrivia()
		{
			var value = "#Если Сервер Тогда";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.IfDirective, TokenType.EOF, "Если Сервер Тогда");
		}

		[Fact]
		public void EndPreprocessorTrivia()
		{
			var value = "#КонецЕсли";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.EndIfDirective, TokenType.EOF, "КонецЕсли");
		}

		[Fact]
		public void InvalidInsertTrivia()
		{
			var value = "#Вставка";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.UnknownDirective, TokenType.EOF, value, 1, Parsing.BslKind.OneScript);
		}

		[Fact]
		public void InsertTrivia()
		{
			var value = "#Вставка";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.InsertDirective, TokenType.EOF, "Вставка");
		}

		[Fact]
		public void EndInsertTrivia()
		{
			var value = "#КонецВставки";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.EndInsertDirective, TokenType.EOF, "КонецВставки");
		}

		[Fact]
		public void DeleteTrivia()
		{
			var value = "#Удаление";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.DeleteDirective, TokenType.EOF, "Удаление");
		}

		[Fact]
		public void EndDeleteTrivia()
		{
			var value = "#КонецУдаления";
			TestHelper.CheckSingleTrivia(value, BslTriviaKind.EndDeleteDirective, TokenType.EOF, "КонецУдаления");
		}

		[Fact]
		public void Identifier1()
		{
			var value = "СП_ПолучитьИмя";
			TestHelper.CheckSingleToken(value, TokenType.IDENTIFIER, value);
		}

		[Fact]
		public void Identifier2()
		{
			var value = "_ПолучитьИмя";
			TestHelper.CheckSingleToken(value, TokenType.IDENTIFIER, value);
		}

		[Fact]
		public void Identifier3()
		{
			var value = "Получить_Имя";
			TestHelper.CheckSingleToken(value, TokenType.IDENTIFIER, value);
		}

		[Fact]
		public void Identifier4()
		{
			var value = "ПолучитьИмя";
			TestHelper.CheckSingleToken(value, TokenType.IDENTIFIER, value);
		}

		[Fact]
		public void Ampersand()
			=> TestHelper.CheckSingleTokenWithoutValue("&", TokenType.AMPERSAND);

		[Fact]
		public void QuestionMark()
			=> TestHelper.CheckSingleTokenWithoutValue("?", TokenType.QUESTION_MARK);

		[Fact]
		public void IfKeyword()
			=> TestHelper.CheckBilingualSingleTokenWithoutValue("ЭКСПОРТ", "EXPORT", TokenType.EXPORT);
	}
}