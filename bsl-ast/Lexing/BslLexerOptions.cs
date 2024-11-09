using BSL.AST.Parsing;
using BSL.AST.Parsing.Preprocessing;

namespace BSL.AST.Lexing
{
	public class BslLexerOptions
	{
		/// <summary>
		/// Диалект языка
		/// </summary>
		public BslKind LanguageKind { get; set; } = BslKind.OneC;
		/// <summary>
		/// Список флагов контекстов, в рамках которых может быть скомпилирован модуль
		/// </summary>
		public BslCompileContexts CompileContexts { get; set; } = BslCompileContexts.All;
	}
}