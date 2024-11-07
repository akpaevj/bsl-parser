using BSL.AST.Parsing.Nodes;
using BSL.AST.Parsing.Preprocessing;

namespace BSL.AST.Parsing
{
	/// <summary>
	/// Текущее состояние автомата
	/// </summary>
	internal class BslParserState
	{
		/// <summary>
		/// Выполняется парсинг диалекта 1Скрипт
		/// </summary>
		public bool IsOneScript { get; internal set; } = false;
		/// <summary>
		/// Находимся в методе
		/// </summary>
		public bool InMethod { get; internal set; } = false;
		/// <summary>
		/// Находимся в цикле
		/// </summary>
		public bool InLoop { get; internal set; } = false;
		/// <summary>
		/// Находимся в методе с модификаторов Асинх
		/// </summary>
		public bool InAsyncMethod { get; internal set; } = false;
		/// <summary>
		/// Чтение объявления переменных модуля уже началось
		/// </summary>
		public bool VarSectionIsStarted { get; internal set; } = false;
		/// <summary>
		/// Чтение инструкций в блоке метода уже началось
		/// </summary>
		public bool MethodStatementsAreStarted { get; internal set; } = false;
		/// <summary>
		/// Чтение процедур и функций уже началось
		/// </summary>
		public bool MethodsSectionIsStarted { get; internal set; } = false;
		/// <summary>
		/// Чтение инструкций в блоке модуля уже началось
		/// </summary>
		public bool StatementsSectionIsStarted { get; internal set; } = false;
		/// <summary>
		/// Стек текущих областей
		/// </summary>
		public Stack<Region> Regions { get; internal set; } = [];
		/// <summary>
		/// Стек контекстов компиляции
		/// </summary>
		public Stack<BslCompileContexts> CompileContexts { get; internal set; } = [];
		/// <summary>
		/// Текущий контекст компиляции
		/// </summary>
		public BslCompileContexts CurrentCompileContext { get; internal set; } = BslCompileContexts.None;
		/// <summary>
		/// Экземпляр текущего модуля
		/// </summary>
		public ModuleNode CurrentModule { get; internal set; } = null!;
	}
}