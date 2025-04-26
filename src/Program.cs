using ProjectControlSystem.src.commands;

namespace ProjectControlSystem.src
{
	class Program
	{
		static readonly SessionData sessionData = new();
		static readonly IDataAccessor data = new SqlDataAccessor();
		static readonly CommandsManager commandsManager = new();
		
		static void RegisterCommands()
		{
			commandsManager.RegisterCommand(AuthorizeCommand.GetName(), new AuthorizeCommand(data, sessionData));
			commandsManager.RegisterCommand(AddUserCommand.GetName(), new AddUserCommand(data));
			commandsManager.RegisterCommand(AddTaskCommand.GetName(), new AddTaskCommand(data));
			commandsManager.RegisterCommand(GetTasksCommand.GetName(), new GetTasksCommand(data, sessionData));
			commandsManager.RegisterCommand(ChangeTaskStatusCommand.GetName(), new ChangeTaskStatusCommand(data, sessionData));
		}

		public static void Main()
		{
			RegisterCommands();

			Console.WriteLine($"Добро пожаловать в систему контроля проектов, пожалуйста авторизируйтесь\n{AuthorizeCommand.GetName()} {AuthorizeCommand.GetParameters()}\nДля выхода используйте exit");

			while (true)
			{
				Console.Write($"ProjectControl{(sessionData.currentUser == null ? string.Empty : "@" + sessionData.currentUser.Login)}>>");
				
				string? command = Console.ReadLine();

				if (string.IsNullOrEmpty(command))
				{
					continue;
				}

				if (command == "help")
				{
					commandsManager.OutHelp();
					continue;
				}
				if (command == "exit")
				{
					break;
				}

				commandsManager.ExecuteCommand(command, sessionData.currentUser);
			}
		}
	}
}