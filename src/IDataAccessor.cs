using ProjectControlSystem.src.data;

namespace ProjectControlSystem.src
{
	public interface IDataAccessor
	{
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
		DataAccessResult<int> AddUser(User user);
		DataAccessResult<int> UpdateTask(ProjectTask task);
		DataAccessResult<int> AddTask(ProjectTask task);
		DataAccessResult<ProjectTask> GetTaskByID(int taskID);
		DataAccessResult<ProjectTask> GetTaskByName(int userID, string taskName);
	}
}
