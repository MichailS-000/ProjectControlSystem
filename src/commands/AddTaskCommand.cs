using ProjectControlSystem.src.data;
using System.Text.RegularExpressions;

namespace ProjectControlSystem.src.commands
{
	internal class AddTaskCommand : ICommand
	{

		private string? m_errorMessage;
		private readonly IDataAccessor m_dataAccessor;

		public AddTaskCommand(IDataAccessor dataAccessor)
		{
			m_dataAccessor = dataAccessor;
		}

		public bool Execute(string arguments)
		{
			Regex regex = new(@"^""(.+?)"" (\S+) (\S+) ""(.+?)""$");
			var match = regex.Match(arguments.Trim());

			if (!match.Success)
			{
				return InvalidInputResult();
			}

			string taskName = match.Groups[1].Value;
			string userLogin = match.Groups[2].Value;
			
			if (!int.TryParse(match.Groups[3].Value, out int projectId))
			{
				InvalidInputResult();
			}

			string taskDescription = match.Groups[4].Value;

			var getUserId = m_dataAccessor.GetUserID(userLogin);

			int userId;

			if (getUserId.IsSuccess)
			{
				userId = getUserId.Result;
			}
			else
			{
				m_errorMessage = "Пользователь не найден";
				return false;
			}

			ProjectTask task = new(userId, taskName, "To Do", projectId, taskDescription, -1);

			var addTaskResult = m_dataAccessor.AddTask(task);

			if (addTaskResult.IsSuccess)
			{
				Console.WriteLine("Задача успешно добавлена");
				return true;
			}
			else
			{
				m_errorMessage = addTaskResult.ErrorMessage;
				return false;
			}
		}

		bool InvalidInputResult()
		{
			m_errorMessage = "Неверный формат ввода, используйте " + GetName() + " " + GetParameters();
			return false;
		}

		public static string GetDescription() => "Чтобы добавить новую задачу для пользователя";

		public static string GetName() => "add_task";

		public static string GetParameters() => "\"<имя задачи>\" <логин пользователя> <идентификатор проекта> \"<описание задачи>\"";

		public string GetCommandDescription() => GetDescription();

		public string GetCommandName() => GetName();

		public string GetCommandParameters() => GetParameters();

		public string? GetErrorMessage() => m_errorMessage;

		public int GetPermissionLevel() => 2;
	}
}
