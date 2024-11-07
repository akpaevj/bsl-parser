using BSL.AST.Lexing;

namespace BSL.AST.Diagnostics
{
    /// <summary>
    /// Исключение, выбрасываемые при ошибках синтаксического разбора конструкций
    /// </summary>
    /// <param name="error">Диагностированная ошибка</param>
    /// <param name="skipTo">Список типов токенов, до одного из которых следует выполнить пропуск последующих</param>
    internal class BslSyntaxErrorException(Diagnostic error, params TokenType[] skipTo) : Exception
    {
		internal Diagnostic Error { get; private set; } = error;
		internal TokenType[] SkipTo { get; private set; } = skipTo.Length == 0 ? [TokenType.SEMICOLON] : skipTo;
	}
}