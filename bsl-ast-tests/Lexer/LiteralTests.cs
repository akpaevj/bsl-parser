using BSL.AST.Lexing;

namespace BSL.AST.Tests.Lexer
{
    public class LiteralTests
    {
        [Fact]
        public void StringLiteral()
            => TestHelper.CheckSingleToken("\"BSL \"\"MyTest\"\" language\"", TokenType.STRING, "\"BSL \"\"MyTest\"\" language\"");

		[Fact]
		public void MultilineString1Literal()
		{
			var text = "\"my text\r\n\t|text2\r\n\t|fdsf\"";
			TestHelper.CheckMultiline(text, "my text", "text2", "fdsf");
		}

		[Fact]
		public void MultilineString2Literal()
        {
            var text = "\"my text\"\r\n\"text2\"\r\n\"fdsf\"";
			TestHelper.CheckMultiline(text, "my text", "text2", "fdsf");
		}

		[Fact]
		public void MultilineString3Literal()
		{
			var text = "\"my text\"\r\n\"text2\r\n\t|fdsf\"";
			TestHelper.CheckMultiline(text, "my text", "text2", "fdsf");
		}

		[Fact]
        public void DateTimeLiteral()
            => TestHelper.CheckSingleToken("\'00010101\'", TokenType.DATE, "\'00010101\'");

		[Fact]
		public void InvalidDateTimeLiteral()
			=> TestHelper.CheckSingleTokenWithoutValue("\'0001fs0101\'", TokenType.DATE);

		[Fact]
        public void TrueBooleanLiteral()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("Истина", "True", TokenType.TRUE);

        [Fact]
        public void FalseBooleanLiteral()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("Ложь", "False", TokenType.FALSE);

        [Fact]
        public void NumberLiteral()
            => TestHelper.CheckSingleToken("1.223", TokenType.NUMBER, "1.223");

		[Fact]
        public void UndefinedLiteral()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("Неопределено", "Undefined", TokenType.UNDEFINED);

        [Fact]
        public void NullLiteral()
            => TestHelper.CheckSingleTokenWithoutValue("NULL", TokenType.NULL);
    }
}