using ProjectControlSystem.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectControlSystem.src
{
	internal class CommandsManager
	{
		private readonly static Dictionary<string, int> permissionsLevels = new()
		{
			{"user", 1 },
			{"admin", 2 }
		};

		private readonly Dictionary<string, ICommand> m_commands = new();

		public void OutHelp()
		{
			Console.WriteLine("Существующие команды: ");

			Console.WriteLine("\nhelp\n- Для просмотра доступных команд");
			Console.WriteLine("\nexit\n- Для выхода из программы");

			foreach (var command in m_commands)
			{
				Console.WriteLine($"\n {command.Value.GetCommandName()} {command.Value.GetCommandParameters()}\n- {command.Value.GetCommandDescription()}\nРазрешено использовать пользователям с уровнем доступа {command.Value.GetPermissionLevel()}");
			}
		}

		public void RegisterCommand(string commandName, ICommand command)
		{
			m_commands.Add(commandName.ToLower(), command);
		}

		public void ExecuteCommand(string command, User? user)
		{
			var argv = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (argv.Length < 1)
			{
				return;
			}

			string commandName = command.Split()[0];
			string commandArguments = command.Replace(commandName, "");
			commandName = commandName.ToLower();

			int permissionsLevel = 0;

			if (!m_commands.ContainsKey(commandName))
			{
				Console.WriteLine($"Комманды {commandName} не существует");
				return;
			}

			if (user != null)
			{
				if (permissionsLevels.ContainsKey(user.Role))
				{
					permissionsLevel = permissionsLevels[user.Role];
				}
				else
				{
					Console.WriteLine($"Ошибка: неизвестный статус пользователя: \"{user.Role}\"");
				}
			}

			if (permissionsLevel >= m_commands[commandName].GetPermissionLevel())
			{
				var cmd = m_commands[commandName];
				if (!cmd.Execute(commandArguments))
				{
					Console.WriteLine(cmd.GetErrorMessage());
				}
			}
			else
			{
				Console.WriteLine($"У вас нет прав, чтобы использовать эту команду");
			}

		}
	}
}
