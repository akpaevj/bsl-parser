using BSL.AST.Parsing.Nodes;

namespace BSL.AST.Parsing
{
	/// <summary>
	/// Текущее состояние автомата
	/// </summary>
	internal class BslParserState
	{
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
	}
}