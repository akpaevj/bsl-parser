using BSL.AST.Lexing;

namespace BSL.AST.Diagnostics
{
	public class Diagnostic(string reporterCode, string errorCode, DiagnosticLevel level, string message, SourcePosition position)
    {
        public string ReporterCode { get; internal set; } = reporterCode;
        public string ErrorCode { get; internal set; } = errorCode;
        public DiagnosticLevel Level { get; internal set; } = level;
        public string Message { get; internal set; } = message;
        public SourcePosition Position { get; internal set; } = position;
    }
}
