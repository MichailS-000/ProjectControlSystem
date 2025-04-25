using ProjectControlSystem.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectControlSystem.src
{
	public interface IDataAccessor
	{
		User? GetUser(string login, string password);
		IEnumerable<ProjectTask> GetTasksOfUser(User user);
		bool AddUser(User user);
		bool SetTask(Task task);
		bool AddTask(Task task);
	}
}
