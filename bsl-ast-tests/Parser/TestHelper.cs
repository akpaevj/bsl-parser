using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes.Expressions;
using BSL.AST.Parsing.Nodes.Statements;
using BSL.AST.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSL.AST.Parsing.Nodes.Expressions.Literals;
using BSL.AST.Parsing.Nodes;
using BSL.AST.Diagnostics;

namespace BSL.AST.Tests.Parser
{
    public class TestHelper
	{
		public static (ModuleNode Module, IReadOnlyList<Diagnostic> Errors) ParseModule(string source)
		{
            var parser = new BslParser();
			var module = parser.ParseModule(source);

			return (module, parser.Diagnostics);
		}

		public static void CheckAssignmentStatementNode<T>(string source, Action<T> handler)
		{
			var (Module, Errors) = ParseModule(source);

			Assert.Empty(Errors);

			var node = Assert.Single(Module.Children);
			var statement = Assert.IsType<AssignmentStatementNode>(node);

			var expression = Assert.IsType<T>(statement.Expression);

			handler(expression);
		}

		public static void CheckExpressionStatementNode<T>(string source, Action<T> handler)
		{
			var (Module, Errors) = ParseModule(source);

			Assert.Empty(Errors);

			var node = Assert.Single(Module.Children);
			var statement = Assert.IsType<ExpressionStatementNode>(node);

			var expression = Assert.IsType<T>(statement.Expression);

			handler(expression);
		}

		public static void BinaryExpressionNodeTest<T, T1, T2>(string source) where T : BinaryExpressionNode
			=> CheckAssignmentStatementNode<T>(source, expression =>
			{
				Assert.IsType<T1>(expression.Left);
				Assert.IsType<T2>(expression.Right);
			});

		public static void LiteralExpressionNodeTest<T>(string source, Action<T>? handler = default) where T : LiteralExpressionNode
			=> CheckAssignmentStatementNode<T>(source, expression =>
			{
				handler?.Invoke(expression);
			});

		public static T ExactSingleStatementNodeTest<T>(string source, bool errorsMustOccur = false)
		{
			var statement = SingleBslNodeTest(source, errorsMustOccur);

			return Assert.IsType<T>(statement);
		}

		public static BslNode SingleBslNodeTest(string source, bool errorsMustOccur = false)
		{
			var module = ModuleNodeTest(source, errorsMustOccur);
			return Assert.Single(module.Children);
		}

		public static ModuleNode ModuleNodeTest(string source, bool errorsMustOccur = false, BslKind language = BslKind.OneC)
		{
			var parser = new BslParser();
			var module = parser.ParseModule(source, options =>
			{
				options.LanguageKind = language;
			});

			if (errorsMustOccur)
				Assert.NotEmpty(parser.Diagnostics);
			else
				Assert.Empty(parser.Diagnostics);

			return module;
		}
	}
}
