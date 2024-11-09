using BSL.AST.Parsing.Preprocessing;

namespace BSL.AST.Lexing
{
    public class BslLexerState
	{
		/// <summary>
		/// Стек директив вставок
		/// </summary>
		public Stack<Insert> Inserts { get; internal set; } = [];
		/// <summary>
		/// Стек директив удаления
		/// </summary>
		public Stack<Delete> Deletes { get; internal set; } = [];
		/// <summary>
		/// Стек текущих областей
		/// </summary>
		public Stack<Region> Regions { get; internal set; } = [];
		/// <summary>
		/// Стек контекстов компиляции. IsIfDirective - признак того, что котекст вычислен в директиве "Если". 
		/// Необходимо для корректного вычисления контекста в директиве "Иначе"
		/// </summary>
		public List<List<BslCompileContexts>> CompileContexts { get; internal set; } = [];
		/// <summary>
		/// Текущий контекст компиляции
		/// </summary>
		public BslCompileContexts CurrentCompileContext { get; internal set; } = BslCompileContexts.None;
		/// <summary>
		/// Признак чтения мультистроки
		/// </summary>
		public bool ReadMultilineString { get; internal set; } = false;
		/// <summary>
		/// Количество собранных двойных кавычек, используется при парсинге строкового литерала
		/// </summary>
		public int CountedQuotes { get; internal set; } = 0;
		/// <summary>
		/// Коллекция leading trivias, используемая при чтении очередного токена
		/// </summary>
		public List<BslTrivia> LeadingTrivias { get; internal set; } = [];
		/// <summary>
		/// Текущая позиция в потоке символов модуля
		/// </summary>
		public int CurrentPosition { get; internal set; } = 0;
		/// <summary>
		/// Оффсет от начала текущей строки
		/// </summary>
		public int CurrentColumn { get; internal set; } = 0;
		/// <summary>
		/// Номер текущей строки
		/// </summary>
		public int CurrentLine { get; internal set; } = 1;
	}
}