namespace BSL.AST.Parsing
{
	/// <summary>
	/// Настройки модуля
	/// </summary>
	public record CommonModuleSettings
	{
		/// <summary>
		/// Формирует глобальный контекст
		/// </summary>
		public bool Global { get; set; } = false;
		/// <summary>
		/// Доступен клиенту
		/// </summary>
		public bool Client { get; set; } = false;
		/// <summary>
		/// Доступен серверу
		/// </summary>
		public bool Server { get; set; } = false;
		/// <summary>
		/// Доступен внешнему соединению
		/// </summary>
		public bool ExternalConnection { get; set; } = false;
		/// <summary>
		/// Серверный модуль. Доступен для вызова с клиента
		/// </summary>
		public bool ServerCall { get; set; } = false;
	}
}