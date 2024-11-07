using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BSL.AST.Lexing
{
	public struct BslTrivia(BslTriviaKind kind, string value, SourcePosition position)
    {
        public BslTriviaKind Kind { get; internal set; } = kind;
        public string Value { get; internal set; } = value;
		public SourcePosition Position { get; internal set; } = position;
    }
}