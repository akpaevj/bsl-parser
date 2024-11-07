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

        // 1C
        IfDirective,
		ElseIfDirective,
		ElseDirective,
		EndIfDirective,
        InsertDirective,
        EndInsertDirective,
        DeleteDirective,
        EndDeleteDirective,
        
        // OneScript
        UseDirective,
        Native
    }
}