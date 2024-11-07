using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSL.AST.Lexing;
using BSL.AST.Parsing;

namespace BSL.AST.Tests.Lexer
{
    internal static class TestHelper
    {
        internal static void CheckBilingualSingleToken(string sourceRu, string sourceEn, TokenType expectedTokenType, string expectedValue)
        {
            CheckSingleToken(sourceRu, expectedTokenType, expectedValue);
            CheckSingleToken(sourceEn, expectedTokenType, expectedValue);
        }

        internal static void CheckBilingualSingleTokenWithoutValue(string sourceRu, string sourceEn, TokenType expectedTokenType)
        {
            CheckSingleTokenWithoutValue(sourceRu, expectedTokenType);
            CheckSingleTokenWithoutValue(sourceEn, expectedTokenType);
        }

        internal static void CheckSingleToken(string source, TokenType expectedTokenType, string expectedValue)
        {
			var lexer = new BslLexer();
			var tokens = lexer.Tokenize(source);

            Assert.Equal(expectedTokenType == TokenType.EOF ? 1 : 2, tokens.Count);
            var token = tokens[0];
            var value = source[token.Position.StartPosition..token.Position.EndPosition];

            Assert.Equal(expectedValue, value);
            Assert.Equal(expectedTokenType, token.Type);
            Assert.Equal(0, token.Position.StartPosition);
            Assert.Equal(source.Length, token.Position.EndPosition);
            Assert.Equal(1, token.Position.Line);
        }

		internal static void CheckMultiline(string source, params string[] expectedValues)
		{
			var lexer = new BslLexer();
			var tokens = lexer.Tokenize(source);

            var stringTokens = tokens.Where(c => c.Type == TokenType.STRING).ToList();

            for (int i = 0; i < stringTokens.Count; i++)
                Assert.Equal(expectedValues[i], stringTokens[i].Text);
		}

		internal static void CheckSingleTrivia(string source, BslTriviaKind triviaKind, TokenType expectedTokenType, string expectedValue, int line = 1, BslKind bslKind = BslKind.OneC, bool leading = true)
		{
			var lexer = new BslLexer();
			var tokens = lexer.Tokenize(source, bslKind);

			Assert.Equal(expectedTokenType == TokenType.EOF ? 1 : 2, tokens.Count);
			var token = tokens[0];
			var trivia = Assert.Single(leading ? token.LeadingTrivias : token.TrailingTrivias);
			Assert.Equal(triviaKind, trivia.Kind);
			var triviaValue = trivia.Value;

			Assert.Equal(expectedValue, triviaValue);
			Assert.Equal(expectedTokenType, token.Type);
			Assert.Equal(line, token.Position.Line);
		}

		internal static void CheckSingleTokenWithoutValue(string source, TokenType expectedTokenType)
        {
            var lexer = new BslLexer();
            var tokens = lexer.Tokenize(source);

			Assert.Equal(expectedTokenType == TokenType.EOF ? 1 : 2, tokens.Count);
			var token = tokens[0];

			Assert.Equal(expectedTokenType, token.Type);
            Assert.Equal(0, token.Position.StartPosition);
            Assert.Equal(source.Length, token.Position.EndPosition);
            Assert.Equal(1, token.Position.Line);
        }
    }
}
