namespace ProjectControlSystem.src.commands
{
	internal class GetTasksCommand : ICommand
	{
		private string? m_errorMessage = null;
		private readonly IDataAccessor m_dataAccessor;
		private readonly SessionData m_sessionData;

		public GetTasksCommand(IDataAccessor dataAccessor, SessionData sessionData)
		{
			m_dataAccessor = dataAccessor;
			m_sessionData = sessionData;
		}

		public bool Execute(string arguments)
		{
			if (m_sessionData.currentUser == null)
			{
				m_errorMessage = "Вы не авторизированы";
				return false;
			}

			var getTasks = m_dataAccessor.GetTasksOfUser(m_sessionData.currentUser);

			if (getTasks.IsSuccess && getTasks.Result != null)
			{
				if (getTasks.Result.Count() == 0)
				{
					Console.WriteLine("У вас нет задач");
					return true;
				}	
				
				Console.WriteLine("Ваши задачи:\n");

				foreach (var task in getTasks.Result)
				{
					Console.WriteLine($"Задача: {task.Name} Статус: {task.TaskState} К проекту №{task.ProjectID}\nОписание: {task.Description}\n");
				}

				return true;
			}
			else
			{
				m_errorMessage = getTasks.ErrorMessage;
				return false;
			}
		}

		public static string GetDescription() => "Чтобы получить текущие назначеные задачи";

		public static string GetName() => "get_tasks";

		public static string GetParameters() => "";

		public string GetCommandDescription() => GetDescription();

		public string GetCommandName() => GetName();

		public string GetCommandParameters() => GetParameters();

		public string? GetErrorMessage() => m_errorMessage;

		public int GetPermissionLevel() => 1;
	}
}
