namespace BSL.AST.Lexing
{
    public record BslToken(string Text, TokenType Type, SourcePosition Position)
    {
        public List<BslTrivia> LeadingTrivias { get; internal set; } = [];
        public List<BslTrivia> TrailingTrivias { get; internal set; } = [];
    }
}
