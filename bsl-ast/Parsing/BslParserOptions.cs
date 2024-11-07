using BSL.AST.Parsing.Preprocessing;

namespace BSL.AST.Parsing
{
	public class BslParserOptions
	{
		public BslKind LanguageKind { get; set; } = BslKind.OneC;
		public BslCompileContexts CompileContexts { get; set; } = BslCompileContexts.None;
		public bool VariableDeclarationsSectionAllowed { get; set; } = true;
		public bool MainProgramSectionAllowed { get; set; } = true;
	}
}