using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSL.AST.Lexing;

namespace BSL.AST.Tests.Lexer
{
    public class OperatorTests
	{
		[Fact]
		public void Plus()
			=> TestHelper.CheckSingleTokenWithoutValue("+", TokenType.PLUS);

		[Fact]
		public void Minus()
			=> TestHelper.CheckSingleTokenWithoutValue("-", TokenType.MINUS);

		[Fact]
		public void Divide()
			=> TestHelper.CheckSingleTokenWithoutValue("/", TokenType.DIVIDE);

		[Fact]
		public void Multiply()
			=> TestHelper.CheckSingleTokenWithoutValue("*", TokenType.MULTIPLY);

		[Fact]
		public void Remainder()
			=> TestHelper.CheckSingleTokenWithoutValue("%", TokenType.MODULO);

		[Fact]
		public void Mark()
			=> TestHelper.CheckSingleTokenWithoutValue("~", TokenType.MARK);

		[Fact]
		public void EndOfMark()
			=> TestHelper.CheckSingleTokenWithoutValue(":", TokenType.END_OF_MARK);

		[Fact]
		public void Equal()
			=> TestHelper.CheckSingleTokenWithoutValue("=", TokenType.EQUAL);

		[Fact]
		public void NotEqual()
			=> TestHelper.CheckSingleTokenWithoutValue("<>", TokenType.NOT_EQUAL);

		[Fact]
		public void Less()
			=> TestHelper.CheckSingleTokenWithoutValue("<", TokenType.LESS);

		[Fact]
		public void LessOrEqual()
			=> TestHelper.CheckSingleTokenWithoutValue("<=", TokenType.LESS_OR_EQUAL);

		[Fact]
		public void Greater()
			=> TestHelper.CheckSingleTokenWithoutValue(">", TokenType.GREATER);

		[Fact]
		public void GreaterOrEqual()
			=> TestHelper.CheckSingleTokenWithoutValue(">=", TokenType.GREATER_OR_EQUAL);

		[Fact]
		public void Dot()
			=> TestHelper.CheckSingleTokenWithoutValue(".", TokenType.DOT);

		[Fact]
		public void Comma()
			=> TestHelper.CheckSingleTokenWithoutValue(",", TokenType.COMMA);

		[Fact]
		public void Semicolon()
			=> TestHelper.CheckSingleTokenWithoutValue(";", TokenType.SEMICOLON);

		[Fact]
		public void OpenParenthesis()
			=> TestHelper.CheckSingleTokenWithoutValue("(", TokenType.OPEN_PARENT);

		[Fact]
		public void CloseParenthesis()
			=> TestHelper.CheckSingleTokenWithoutValue(")", TokenType.CLOSE_PARENT);

		[Fact]
		public void OpenSquareBracket()
			=> TestHelper.CheckSingleTokenWithoutValue("[", TokenType.OPEN_BRACKET);

		[Fact]
		public void CloseSquareBracket()
			=> TestHelper.CheckSingleTokenWithoutValue("]", TokenType.CLOSE_BRACKET);
	}
}
