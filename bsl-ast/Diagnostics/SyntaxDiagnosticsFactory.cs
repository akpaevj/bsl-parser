using BSL.AST.Lexing;
using BSL.AST.Parsing;
using BSL.AST.Properties;

namespace BSL.AST.Diagnostics
{
	/// <summary>
	/// Фабрика диагностик, используемых при разборе синтаксических конструкций
	/// </summary>
	internal static class SyntaxDiagnosticsFactory
    {
		internal static Diagnostic InvalidIdentifier(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0001", DiagnosticLevel.ERROR, Resources.InvalidIdentifier, Position);
		internal static Diagnostic InvalidDateLiteralFormat(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0002", DiagnosticLevel.ERROR, Resources.InvalidDateLiteralFormat, Position);
		internal static Diagnostic InvalidNumberLiteralFormat(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0003", DiagnosticLevel.ERROR, Resources.InvalidNumberLiteralFormat, Position);
		internal static Diagnostic InvalidStringLiteralFormat(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0004", DiagnosticLevel.ERROR, Resources.InvalidStringLiteralFormat, Position);
		internal static Diagnostic ExpressionExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0005", DiagnosticLevel.ERROR, Resources.ExpressionExpected, Position);
		internal static Diagnostic StatementExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0006", DiagnosticLevel.ERROR, Resources.StatementExpected, Position);
		internal static Diagnostic BinaryExpressionExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0007", DiagnosticLevel.ERROR, Resources.BinaryExpressionExpected, Position);
		internal static Diagnostic SemicolonExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0008", DiagnosticLevel.ERROR, Resources.SemicolonExpected, Position);
		internal static Diagnostic IdentifierExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0009", DiagnosticLevel.ERROR, Resources.IdentifierExpected, Position);
		internal static Diagnostic CommaExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0010", DiagnosticLevel.ERROR, Resources.CommaExpected, Position);
		internal static Diagnostic CloseParentExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0011", DiagnosticLevel.ERROR, Resources.CloseParentExpected, Position);
		internal static Diagnostic ToExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0012", DiagnosticLevel.ERROR, Resources.ToExpected, Position);
		internal static Diagnostic DoExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0013", DiagnosticLevel.ERROR, Resources.DoExpected, Position);
		internal static Diagnostic EndDoExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0014", DiagnosticLevel.ERROR, Resources.EndDoExpected, Position);
		internal static Diagnostic InExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0015", DiagnosticLevel.ERROR, Resources.InExpected, Position);
		internal static Diagnostic CloseBracketExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0016", DiagnosticLevel.ERROR, Resources.CloseBracketExpected, Position);
		internal static Diagnostic NewObjectArgumentsExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0017", DiagnosticLevel.ERROR, Resources.NewObjectArgumentsExpected, Position);
		internal static Diagnostic LiteralExpressionExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0018", DiagnosticLevel.ERROR, Resources.LiteralExpressionExpected, Position);
		internal static Diagnostic OpenParentExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0019", DiagnosticLevel.ERROR, Resources.OpenParentExpected, Position);
		internal static Diagnostic EndProcedureExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0020", DiagnosticLevel.ERROR, Resources.EndProcedureTokenExpected, Position);
		internal static Diagnostic EndFunctionExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0021", DiagnosticLevel.ERROR, Resources.EndFunctionTokenExpected, Position);
		internal static Diagnostic UnexpectedVariableDeclaration(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0022", DiagnosticLevel.ERROR, Resources.UnexpectedVariableDeclaration, Position);
		internal static Diagnostic EqualExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0023", DiagnosticLevel.ERROR, Resources.EqualExpected, Position);
		internal static Diagnostic MarkExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0024", DiagnosticLevel.ERROR, Resources.MarkExpected, Position);
		internal static Diagnostic EndMarkExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0025", DiagnosticLevel.ERROR, Resources.EndMarkExpected, Position);
		internal static Diagnostic ExceptExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0026", DiagnosticLevel.ERROR, Resources.ExceptExpected, Position);
		internal static Diagnostic EndTryExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0027", DiagnosticLevel.ERROR, Resources.EndTryExpected, Position);
		internal static Diagnostic MethodDefinitionExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0028", DiagnosticLevel.ERROR, Resources.MethodDefinitionExpected, Position);
		internal static Diagnostic ThenExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0029", DiagnosticLevel.ERROR, Resources.ThenExpected, Position);
		internal static Diagnostic EndIfExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0030", DiagnosticLevel.ERROR, Resources.EndIfExpected, Position);
		internal static Diagnostic UnexpectedEndRegion(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0031", DiagnosticLevel.ERROR, Resources.UnexpectedEndRegion, Position);
		internal static Diagnostic UnexpectedAwaitOperator(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0032", DiagnosticLevel.ERROR, Resources.UnexpectedAwaitOperator, Position);
		internal static Diagnostic EndRegionExpected(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0033", DiagnosticLevel.ERROR, Resources.EndRegionExpected, Position);
		internal static Diagnostic UnexpectedPreprocessorExpression(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0034", DiagnosticLevel.ERROR, Resources.UnexpectedPreprocessorExpression, Position);
		internal static Diagnostic UnexpectedPreprocessorDirective(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0035", DiagnosticLevel.ERROR, Resources.UnexpectedPreprocessorDirective, Position);
		internal static Diagnostic UnexpectedEndInsert(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0036", DiagnosticLevel.ERROR, Resources.UnexpectedEndInsert, Position);
		internal static Diagnostic UnexpectedEndDelete(SourcePosition Position)
			=> new(BslParser.DIAGNOSTICS_REPORTER_CODE, "0037", DiagnosticLevel.ERROR, Resources.UnexpectedEndDelete, Position);
	}
}
