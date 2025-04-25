using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectControlSystem.src
{
	public interface ICommand
	{
		bool Execute(string arguments);
		
		string GetCommandName();
		string GetCommandDescription();
		string GetCommandParameters();

		int GetPermissionLevel();
		string? GetErrorMessage();
	}
}
