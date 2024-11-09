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
using System.Linq.Expressions;

namespace BSL.AST.Tests.Parser
{
    public class TestHelper
	{
		public static (ModuleNode Module, IReadOnlyList<Diagnostic> Errors) ParseOscriptModule(string source)
		{
			var parser = new BslParser();
			var module = parser.ParseOscriptModule(source);

			return (module, parser.Diagnostics);
		}

		public static (List<ModuleNode> Modules, IReadOnlyList<Diagnostic> Errors) ParseClientServerCommonModule(string source)
		{
			var (Modules, Errors) = ParseCommonModule(source, new()
			{
				Client = true,
				Server = true,
			});

			return (Modules, Errors);
		}

		public static (ModuleNode Module, IReadOnlyList<Diagnostic> Errors) ParseClientCommonModule(string source)
		{
			var (Modules, Errors) = ParseCommonModule(source, new()
			{
				Client = true
			});

			return (Modules[0], Errors);
		}

		public static (ModuleNode Module, IReadOnlyList<Diagnostic> Errors) ParseServerCommonModule(string source)
		{
			var (Modules, Errors) = ParseCommonModule(source, new()
			{
				Server = true
			});

			return (Modules[0], Errors);
		}

		public static (List<ModuleNode> Modules, IReadOnlyList<Diagnostic> Errors) ParseCommonModule(string source, CommonModuleSettings settings)
		{
			var parser = new BslParser();
			var modules = parser.ParseCommonModule(source, settings);

			return (modules, parser.Diagnostics);
		}

		public static (List<ModuleNode> Modules, IReadOnlyList<Diagnostic> Errors) ParseFormModule(string source)
		{
			var parser = new BslParser();
			var modules = parser.ParseFormModule(source);

			return (modules, parser.Diagnostics);
		}

		public static (List<ModuleNode> Modules, IReadOnlyList<Diagnostic> Errors) ParseApplicationModule(string source)
		{
			var parser = new BslParser();
			var modules = parser.ParseApplicationModule(source);

			return (modules, parser.Diagnostics);
		}

		public static (ExpressionNode Expression, IReadOnlyList<Diagnostic> Errors) ParseExpression(string source, BslKind bslKind = BslKind.OneC)
		{
			var parser = new BslParser();
			var expression = parser.ParseExpression(source, bslKind);

			return (expression, parser.Diagnostics);
		}

		public static (BlockNode Block, IReadOnlyList<Diagnostic> Errors) ParseCodeBlock(string source)
		{
			var parser = new BslParser();
			var block = parser.ParseCodeBlock(source);

			return (block, parser.Diagnostics);
		}

		public static void MethodNodeTest(string source, Action<MethodNode>? handler = null, bool errorsMustOccur = false)
		{
			var (Module, Errors) = ParseServerCommonModule(source);

			if (errorsMustOccur)
				Assert.NotEmpty(Errors);
			else
				Assert.Empty(Errors);

			var node = Assert.Single(Module.Children);
			var method = Assert.IsType<MethodNode>(node);

			handler?.Invoke(method);
		}

		public static void ExpressionNodeTest<T>(string source, Action<T>? handler = null, BslKind bslKind = BslKind.OneC, bool errorsMustOccur = false)
		{
			var (Expression, Errors) = ParseExpression(source, bslKind);

			if (errorsMustOccur)
				Assert.NotEmpty(Errors);
			else
				Assert.Empty(Errors);

			var expression = Assert.IsType<T>(Expression);

			handler?.Invoke(expression);
		}

		public static void StatementNodeTest<T>(string source, Action<T>? handler = null)
		{
			var (Block, Errors) = ParseCodeBlock(source);

			Assert.Empty(Errors);

			var child = Assert.Single(Block.Statements);
			var statement = Assert.IsType<T>(child);

			handler?.Invoke(statement);
		}

		public static void BinaryExpressionNodeTest<T, T1, T2>(string source) where T : BinaryExpressionNode
			=> ExpressionNodeTest<T>(source, expression =>
			{
				Assert.IsType<T1>(expression.Left);
				Assert.IsType<T2>(expression.Right);
			});

		public static void LiteralExpressionNodeTest<T>(string source, Action<T>? handler = default) where T : LiteralExpressionNode
			=> ExpressionNodeTest<T>(source, expression =>
			{
				handler?.Invoke(expression);
			});

		public static ModuleNode OscriptModuleTest(string source)
		{
			var (Module, Errors) = ParseOscriptModule(source);

			Assert.Empty(Errors);

			return Module;
		}
	}
}
