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
		public class DataAccessResult
		{
			public bool IsSuccess { get; }
			public string? ErrorMessage { get; }

			public DataAccessResult(bool isSuccess, string? errorMessage)
			{
				IsSuccess = isSuccess;
				ErrorMessage = errorMessage;
			}
		}

		public class DataAccessResult<T>
		{
			public bool IsSuccess { get; }
			public T? Result { get; }
			public string? ErrorMessage { get; }

			public DataAccessResult(bool isSuccess, T? result, string? errorMessage)
			{
				IsSuccess = isSuccess;
				Result = result;
				ErrorMessage = errorMessage;
			}
		}

		DataAccessResult<int> GetUserID(string login);
		DataAccessResult<User> AuthUser(string login, string password);
		DataAccessResult<IEnumerable<ProjectTask>> GetTasksOfUser(User user);
		DataAccessResult AddUser(User user);
		DataAccessResult SetTask(ProjectTask task);
		DataAccessResult AddTask(ProjectTask task);
		DataAccessResult<ProjectTask> GetTask(int taskID);
		DataAccessResult<ProjectTask> GetTask(int userID, string taskName);
	}
}
