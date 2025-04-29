using System.Text.RegularExpressions;

namespace ProjectControlSystem.src.commands
{
	internal class ChangeTaskStatusCommand : ICommand
	{
		private string? m_errorMessage = null;
		private readonly IDataAccessor m_dataAccessor;
		private readonly SessionData m_sessionData;

		public ChangeTaskStatusCommand(IDataAccessor dataAccessor, SessionData sessionData)
		{
			m_dataAccessor = dataAccessor;
			m_sessionData = sessionData;
		}

		public bool Execute(string arguments)
		{
			Regex regex = new(@"^""(.+?)"" ""(.+?)""$");
			var match = regex.Match(arguments.Trim());

			if (!match.Success)
			{
				return InvalidInputResult();
			}

			string taskName = match.Groups[1].Value;
			string taskStatus = match.Groups[2].Value;

			if (taskStatus.Length > 0 && m_sessionData.currentUser != null)
			{
				var getTask = m_dataAccessor.GetTaskByName(m_sessionData.currentUser.Id, taskName);

				if (getTask.IsSuccess)
				{
					var task = getTask.Result;
					task.TaskState = taskStatus;

					var updateTask = m_dataAccessor.UpdateTask(task);

					if (updateTask.IsSuccess)
					{
						Console.WriteLine("Статус задачи изменён");
						return true;
					}
					else
					{
						m_errorMessage = updateTask.ErrorMessage;
						return false;
					}
				}
				else
				{
					m_errorMessage = getTask.ErrorMessage;
					return false;
				}
			}
			else
			{
				return InvalidInputResult();
			}
		}

		bool InvalidInputResult()
		{
			m_errorMessage = "Неверный формат ввода, используйте следующий формат ввода: \n" + GetName() + " " + GetParameters();
			return false;
		}

		public static string GetDescription() => "Чтобы изменить статус задачи";

		public static string GetName() => "task_status";

		public static string GetParameters() => "\"<имя задачи>\" \"<статус(To Do | In Progress | Done)>\"";

		public string GetCommandDescription() => GetDescription();

		public string GetCommandName() => GetName();

		public string GetCommandParameters() => GetParameters();

		public string? GetErrorMessage() => m_errorMessage;

		public int GetPermissionLevel() => 1;
	}
}
