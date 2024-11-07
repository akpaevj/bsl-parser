using BSL.AST.Lexing;

namespace BSL.AST.Parsing
{
	public class Using
	{
		public BslTrivia Trivia { get; internal set; } = default;

		public string Path { get; internal set; } = string.Empty;
	}
}