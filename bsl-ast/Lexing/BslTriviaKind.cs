namespace BSL.AST.Lexing
{
	public enum BslTriviaKind
    {
        // Common
        UnknownDirective,
        Comment,
        WhiteSpace,
        EndOfLine,
        RegionDirective,
        EndRegionDirective,
        IfPreprocessorTrivia,
        ElseIfPreprocessorTrivia,
        ElsePreprocessorTrivia,
        EndIfPreprocessorTrivia,

        // 1C
        InsertDirective,
        EndInsertDirective,
        DeleteDirective,
        EndDeleteDirective,
        
        // OneScript
        UseDirective,
        Native
    }
}