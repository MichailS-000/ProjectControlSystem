

using ProjectControlSystem.src.commands;

namespace ProjectControlSystem.src
{
	class Program
	{
		static readonly SessionData sessionData = new();
		static readonly IDataAccessor data = new SqlDataAccessor("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=E:\\Projects\\TestWork\\ProjectControlSystem\\bd\\Database.mdf;Integrated Security=True");
		static readonly CommandsManager commandsManager = new();
		
		static void RegisterCommands()
		{
			commandsManager.RegisterCommand(AuthorizeCommand.GetName(), new AuthorizeCommand(data, sessionData));
			commandsManager.RegisterCommand(AddUserCommand.GetName(), new AddUserCommand(data));
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