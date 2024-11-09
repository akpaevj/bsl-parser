using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions.Arithmetic;
using BSL.AST.Parsing.Nodes.Expressions.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Tests.Parser
{
    public class LiteralExpressionTests
    {
        [Fact]
        public void FalseBoolLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<BoolLiteralExpressionNode>("ЛОЖЬ", node =>
            {
                Assert.False(node.Value);
            });

        [Fact]
        public void TrueBoolLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<BoolLiteralExpressionNode>("ИСТИНА", node =>
            {
                Assert.True(node.Value);
            });

        [Fact]
        public void DateLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<DateLiteralExpressionNode>("'0001-01-01'", node =>
            {
                Assert.Equal(new DateTime(1, 1, 1), node.Value);
            });

        [Fact]
        public void DateTimeWithoutSecondsLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<DateLiteralExpressionNode>("'0001-01-01 01:01'", node =>
            {
                Assert.Equal(new DateTime(1, 1, 1, 1, 1, 0), node.Value);
            });

        [Fact]
        public void DateTimeLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<DateLiteralExpressionNode>("'0001-01-01 01:01:01'", node =>
            {
                Assert.Equal(new DateTime(1, 1, 1, 1, 1, 1), node.Value);
            });

        [Fact]
        public void NullLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<NullLiteralExpressionNode>("NULL");

        [Fact]
        public void UndefinedLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<UndefinedLiteralExpressionNode>("НЕОПРЕДЕЛЕНО");

        [Fact]
        public void NumberLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<NumberLiteralExpressionNode>("12345", node =>
            {
                Assert.Equal(12345, node.Value);
            });

        [Fact]
        public void FloatingPointNumberLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<NumberLiteralExpressionNode>("12.345", node =>
            {
                Assert.Equal(12.345m, node.Value);
            });

        [Fact]
        public void StringLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<StringLiteralExpressionNode>("\"Привет, \"\"мир!\"\"\"", node =>
            {
                Assert.Equal("Привет, \"мир!\"", node.Value);
            });

        [Fact]
        public void Multiline1StringLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<StringLiteralExpressionNode>("\"Привет, \"\r\n\"\"\"мир!\"\"\"", node =>
            {
                Assert.Equal("Привет, \r\n\"мир!\"", node.Value);
            });

        [Fact]
        public void Multiline2StringLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<StringLiteralExpressionNode>("\"Привет, \r\n|\"\"мир!\"\"\"", node =>
            {
                Assert.Equal("Привет, \r\n\"мир!\"", node.Value);
            });

        [Fact]
        public void Multiline3StringLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<StringLiteralExpressionNode>("\"Привет, \"\r\n\"\"\"мир!\"\"\r\n| не привет!\"", node =>
            {
                Assert.Equal("Привет, \r\n\"мир!\"\r\n не привет!", node.Value);
            });

        [Fact]
        public void Multiline4StringLiteralExpressionNodeTest()
            => TestHelper.LiteralExpressionNodeTest<StringLiteralExpressionNode>("\"Привет, \r\n|\"\"мир!\"\"\"\r\n\" не привет!\"", node =>
            {
                Assert.Equal("Привет, \r\n\"мир!\"\r\n не привет!", node.Value);
            });
    }
}
