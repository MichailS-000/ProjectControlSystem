using ProjectControlSystem.src.data;
namespace ProjectControlSystem.src.commands
{
	internal class AddUserCommand : ICommand
	{
		private readonly IDataAccessor m_dataAccessor;
		private string? m_errorMessage = null;

		public AddUserCommand(IDataAccessor data)
		{
			m_dataAccessor = data;
		}

		public bool Execute(string arguments)
		{
			var loginData = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (loginData.Length < 3)
			{
				return InvalidInputResult();
			}

			string login = loginData[0];
			string password = loginData[1];
			string role = loginData[2];

			User user = new(login, password, role);

			var addUserResult = m_dataAccessor.AddUser(user);

			if (addUserResult.IsSuccess)
			{
				Console.WriteLine("Новый пользователь добавлен");
				return true;
			}
			else
			{
				m_errorMessage = addUserResult.ErrorMessage;
				return false;
			}
		}

		bool InvalidInputResult()
		{
			m_errorMessage = "Неверный формат ввода, используйте следующий формат ввода: \n" + GetName() + " " + GetParameters();
			return false;
		}

		public static string GetDescription() => "Чтобы зарегистрировать нового пользователя";

		public static string GetName() => "add_user";

		public static string GetParameters() => "<логин пользователя> <пароль пользователя> <роль пользователя(user | admin)>";

		public string GetCommandDescription() => GetDescription();

		public string GetCommandName() => GetName();

		public string GetCommandParameters() => GetParameters();

		public string? GetErrorMessage() => m_errorMessage;

		public int GetPermissionLevel() => 2;
	}
}
