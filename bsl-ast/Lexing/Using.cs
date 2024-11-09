namespace BSL.AST.Lexing
{
    public class Using
    {
        public BslTrivia Trivia { get; internal set; } = default;

        public string Path { get; internal set; } = string.Empty;
    }
}