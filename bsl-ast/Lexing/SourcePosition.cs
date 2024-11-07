namespace BSL.AST.Lexing
{
    public struct SourcePosition(int startPosition, int endPosition, int line, int startColumn)
    {
        public int StartPosition { get; internal set; } = startPosition;
		public int EndPosition { get; internal set; } = endPosition;
		public int Line { get; internal set; } = line;
		public int StartColumn { get; internal set; } = startColumn;
	}
}
