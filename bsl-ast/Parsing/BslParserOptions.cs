using BSL.AST.Parsing.Preprocessing;

namespace BSL.AST.Parsing
{
	public class BslParserOptions
	{
		/// <summary>
		/// Диалект языка. 1Скрипт или встроенный язык 1С
		/// </summary>
		public BslKind LanguageKind { get; set; } = BslKind.OneC;
		/// <summary>
		/// Список всех контекстов, в рамках которых может компилироваться модуль
		/// </summary>
		public BslCompileContexts CompileContexts { get; set; } = BslCompileContexts.All;
		/// <summary>
		/// Контекст, для которого осуществляется построение экземпляра дерева
		/// </summary>
		public BslCompileContexts ActualContext { get; set; } = BslCompileContexts.All;
		public bool VariableDeclarationsSectionAllowed { get; set; } = true;
		public bool MainProgramSectionAllowed { get; set; } = true;
		public bool ProceduresAndFunctionsSectionAllowed { get; set; } = true;
	}
}