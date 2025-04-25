namespace ProjectControlSystem.src.commands
{
	internal class AuthorizeCommand : ICommand
	{
		private readonly IDataAccessor m_accessor;
		private readonly SessionData m_sessionData;
		private string? errorMessage = null;

		public AuthorizeCommand(IDataAccessor accessor, SessionData sessionData)
		{
			m_accessor = accessor;
			m_sessionData = sessionData;
		}

		public bool Execute(string arguments)
		{
			var loginData = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (loginData.Length < 2)
			{
				return InvalidInputResult();
			}

			string login = loginData[0];
			string password = loginData[1];

			var user = m_accessor.GetUser(login, password);

			if (user == null)
			{
				errorMessage = "Пользователь не найден";
				return false;
			}
			else
			{
				m_sessionData.currentUser = user;

				Console.WriteLine("Вход выполнен, используйте help для просмотра доступных команд");

				return true;
			}
		}

		bool InvalidInputResult()
		{
			errorMessage = "Неверный формат ввода, используйте следующий формат ввода: \n" + GetName() + " " + GetParameters();
			return false;
		}

		public static string GetName() => "login";

		public static string GetDescription() => "Для входа в систему через логин и пароль";

		public static string GetParameters() => "<логин> <пароль>";

		public string GetCommandName() => GetName();

		public string GetCommandDescription() => GetDescription();

		public string GetCommandParameters() => GetParameters();

		public int GetPermissionLevel() => 0;

		public string? GetErrorMessage() => errorMessage;
	}
}
