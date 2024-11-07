using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSL.AST.Lexing;

namespace BSL.AST.Tests.Lexer
{
    public class KeywordTests
    {
        [Fact]
        public void IfKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ЕСЛИ", "IF", TokenType.IF);

        [Fact]
        public void ThenKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ТОГДА", "THEN", TokenType.THEN);

        [Fact]
        public void ElseIfKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ИНАЧЕЕСЛИ", "ELSIF", TokenType.ELSE_IF);

        [Fact]
        public void ElseKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ИНАЧЕ", "ELSE", TokenType.ELSE);

        [Fact]
        public void EndIfKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("КОНЕЦЕСЛИ", "ENDIF", TokenType.END_IF);

        [Fact]
        public void ForKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ДЛЯ", "FOR", TokenType.FOR);

        [Fact]
        public void EachKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("КАЖДОГО", "EACH", TokenType.EACH);

        [Fact]
        public void InKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ИЗ", "IN", TokenType.IN);

        [Fact]
        public void ToKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПО", "TO", TokenType.TO);

        [Fact]
        public void WhileKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПОКА", "WHILE", TokenType.WHILE);

        [Fact]
        public void DoKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ЦИКЛ", "DO", TokenType.DO);

        [Fact]
        public void EndDoKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("КОНЕЦЦИКЛА", "ENDDO", TokenType.END_DO);

        [Fact]
        public void ProcedureKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПРОЦЕДУРА", "PROCEDURE", TokenType.PROCEDURE);

        [Fact]
        public void FunctionKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ФУНКЦИЯ", "FUNCTION", TokenType.FUNCTION);

        [Fact]
        public void EndProcedureKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("КОНЕЦПРОЦЕДУРЫ", "ENDPROCEDURE", TokenType.END_PROCEDURE);

        [Fact]
        public void EndFunctionKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("КОНЕЦФУНКЦИИ", "ENDFUNCTION", TokenType.END_FUNCTION);

        [Fact]
        public void VarKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПЕРЕМ", "VAR", TokenType.VAR);

        [Fact]
        public void GoToKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПЕРЕЙТИ", "GOTO", TokenType.GO_TO);

        [Fact]
        public void ReturnKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ВОЗВРАТ", "RETURN", TokenType.RETURN);

        [Fact]
        public void ContinueKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПРОДОЛЖИТЬ", "CONTINUE", TokenType.CONTINUE);

        [Fact]
        public void BreakKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПРЕРВАТЬ", "BREAK", TokenType.BREAK);

        [Fact]
        public void AndKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("И", "AND", TokenType.AND);

        [Fact]
        public void OrKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ИЛИ", "OR", TokenType.OR);

        [Fact]
        public void NotKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("НЕ", "NOT", TokenType.NOT);

        [Fact]
        public void TryKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ПОПЫТКА", "TRY", TokenType.TRY);

        [Fact]
        public void ExceptKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ИСКЛЮЧЕНИЕ", "EXCEPT", TokenType.EXCEPT);

        [Fact]
        public void RaiseKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ВЫЗВАТЬИСКЛЮЧЕНИЕ", "RAISE", TokenType.RAISE);

        [Fact]
        public void EndTryKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("КОНЕЦПОПЫТКИ", "ENDTRY", TokenType.END_TRY);

        [Fact]
        public void NewKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("НОВЫЙ", "NEW", TokenType.NEW);

        [Fact]
        public void ExecuteKeyword()
            => TestHelper.CheckBilingualSingleTokenWithoutValue("ВЫПОЛНИТЬ", "EXECUTE", TokenType.EXECUTE);

		[Fact]
		public void ValKeyword()
			=> TestHelper.CheckBilingualSingleTokenWithoutValue("ЗНАЧ", "VAL", TokenType.VAL);

		[Fact]
		public void AddHandlerKeyword()
			=> TestHelper.CheckBilingualSingleTokenWithoutValue("ДОБАВИТЬОБРАБОТЧИК", "ADDHANDLER", TokenType.ADD_HANDLER);

		[Fact]
		public void RemoveHandlerKeyword()
			=> TestHelper.CheckBilingualSingleTokenWithoutValue("УДАЛИТЬОБРАБОТЧИК", "REMOVEHANDLER", TokenType.REMOVE_HANDLER);
	}
}
