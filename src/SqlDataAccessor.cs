using ProjectControlSystem.src.data;
using Microsoft.Data.Sqlite;

namespace ProjectControlSystem.src
{
	internal class SqlDataAccessor : IDataAccessor
	{
		private readonly SqliteConnection m_conn;

		public SqlDataAccessor()
		{
			string dbFolder = AppDomain.CurrentDomain.BaseDirectory;
			string dbPath = Path.Combine(dbFolder, @"db\Database.db");

			string connectionString;

			connectionString = $@"Data Source={dbPath}";

			m_conn = new SqliteConnection(connectionString);
			m_conn.Open();
		}

		~SqlDataAccessor()
		{
			m_conn?.Close();
		}

		public IDataAccessor.DataAccessResult<int> AddTask(ProjectTask task)
		{
			const string addTaskCommand = "INSERT INTO Tasks (UserId, TaskName, TaskState, ProjectID, Description) VALUES" +
				" (@UserId, @TaskName, @TaskState, @ProjectId, @Description)";

			try
			{
				using (var command = new SqliteCommand(addTaskCommand, m_conn))
				{
					command.Parameters.Add("@UserId", SqliteType.Integer).Value = task.UserID;
					command.Parameters.Add("@TaskName", SqliteType.Text).Value = task.Name;
					command.Parameters.Add("@TaskState", SqliteType.Text).Value = task.TaskState;
					command.Parameters.Add("@ProjectId", SqliteType.Integer).Value = task.ProjectID;
					command.Parameters.Add("@Description", SqliteType.Text).Value = task.Description;

					command.ExecuteNonQuery();

					return new IDataAccessor.DataAccessResult<int>(true, 0, null);
				}
			}
			catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Задача не может быть в состоянии \"{task.TaskState}\"");
			}
			catch (SqliteException ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Ошибка код: {ex.SqliteErrorCode} сообщение: {ex.Message}");
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, ex.Message);
			}
		}

		public IDataAccessor.DataAccessResult<int> AddUser(User user)
		{
			const string addUserCommand = "INSERT INTO Users (Login, Password, Role) VALUES (@Login, @Password, @Role)";

			try
			{
				using (var command = new SqliteCommand(addUserCommand, m_conn))
				{
					command.Parameters.Add("@Login", SqliteType.Text).Value = user.Login;
					command.Parameters.Add("@Password", SqliteType.Text).Value = user.Password;
					command.Parameters.Add("@Role", SqliteType.Text).Value = user.Role;

					command.ExecuteNonQuery();

					return new IDataAccessor.DataAccessResult<int>(true, 0, null);
				}
			}
			catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
			{
				if (ex.Message.Contains("CHECK"))
				{
					return new IDataAccessor.DataAccessResult<int>(false, -1, $"Пользователь не может иметь роль \"{user.Role}\"");
				}
				else if (ex.Message.Contains("UNIQUE"))
				{
					return new IDataAccessor.DataAccessResult<int>(false, -1, "Пользователь с таким логином уже существует");
				}
				else
				{
					return new IDataAccessor.DataAccessResult<int>(false, -1, $"Ошибка код: {ex.SqliteErrorCode} сообщение: {ex.Message}");
				}
			}
			catch (SqliteException ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Ошибка код: {ex.SqliteErrorCode} сообщение: {ex.Message}");
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, ex.Message);
			}
		}

		public IDataAccessor.DataAccessResult<ProjectTask> GetTaskByID(int taskID)
		{
			const string getTaskByIDCommand = "SELECT * FROM Tasks WHERE TaskID = @TaskId";

			try
			{
				using (var command = new SqliteCommand(getTaskByIDCommand, m_conn))
				{
					command.Parameters.Add("@TaskId", SqliteType.Integer).Value = taskID;

					using (var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
					{
						if (reader.Read())
						{
							ProjectTask projectTask = new(
								reader.GetInt32(reader.GetOrdinal("UserId")),
								reader.GetString(reader.GetOrdinal("TaskName")),
								reader.GetString(reader.GetOrdinal("TaskState")),
								reader.GetInt32(reader.GetOrdinal("ProjectID")),
								reader.GetString(reader.GetOrdinal("Description")),
								reader.GetInt32(reader.GetOrdinal("TaskID")));

							return new IDataAccessor.DataAccessResult<ProjectTask>(true, projectTask, null);
						}
						else
						{
							return new IDataAccessor.DataAccessResult<ProjectTask>(false, null, "Задача не найдена");
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<ProjectTask>(false, null, ex.Message);
			}
		}

		public IDataAccessor.DataAccessResult<IEnumerable<ProjectTask>> GetTasksOfUser(User user)
		{
			const string getAllTasksOfUserCommand = "SELECT * FROM Tasks WHERE UserId = @UserId";

			HashSet<ProjectTask> userTasks = new();

			try
			{
				using (var command = new SqliteCommand(getAllTasksOfUserCommand, m_conn))
				{
					command.Parameters.Add("@UserId", SqliteType.Text).Value = user.Id;

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var task = new ProjectTask(
								reader.GetInt32(reader.GetOrdinal("UserId")),
								reader.GetString(reader.GetOrdinal("TaskName")),
								reader.GetString(reader.GetOrdinal("TaskState")),
								reader.GetInt32(reader.GetOrdinal("ProjectID")),
								reader.GetString(reader.GetOrdinal("Description")),
								reader.GetInt32(reader.GetOrdinal("TaskID")));

							userTasks.Add(task);
						}

						return new IDataAccessor.DataAccessResult<IEnumerable<ProjectTask>>(true, userTasks, null);
					}
				}
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<IEnumerable<ProjectTask>>(false, null, ex.Message);
			}
		}

		public IDataAccessor.DataAccessResult<User> AuthUser(string login, string password)
		{
			const string getUserCommand = "SELECT * FROM Users WHERE Login = @Login AND Password = @Password";

			try
			{
				using (var command = new SqliteCommand(getUserCommand, m_conn))
				{
					command.Parameters.Add("@Login", SqliteType.Text).Value = login;
					command.Parameters.Add("@Password", SqliteType.Text).Value = password;

					using (var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
					{
						if (reader.Read())
						{
							User user = new(
								reader.GetInt32(reader.GetOrdinal("Id")),
								reader.GetString(reader.GetOrdinal("Login")),
								reader.GetString(reader.GetOrdinal("Password")),
								reader.GetString(reader.GetOrdinal("Role")));

							return new IDataAccessor.DataAccessResult<User>(true, user, null);
						}
						else
						{
							return new IDataAccessor.DataAccessResult<User>(false, null, "Неверный логин или пароль");
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<User>(false, null, ex.Message);
			}
		}

		public IDataAccessor.DataAccessResult<int> UpdateTask(ProjectTask task)
		{
			const string updateTaskCommand = "UPDATE Tasks SET UserId = @UserId, TaskName = @TaskName, TaskState = @TaskState, ProjectID = @ProjectId, Description = @Description WHERE TaskID = @TaskID";

			try
			{
				using (var command = new SqliteCommand(updateTaskCommand, m_conn))
				{
					command.Parameters.Add("@UserId", SqliteType.Integer).Value = task.UserID;
					command.Parameters.Add("@TaskName", SqliteType.Text).Value = task.Name;
					command.Parameters.Add("@TaskState", SqliteType.Text).Value = task.TaskState;
					command.Parameters.Add("@ProjectId", SqliteType.Text).Value = task.ProjectID;
					command.Parameters.Add("@Description", SqliteType.Text).Value = task.Description;
					command.Parameters.Add("@TaskID", SqliteType.Integer).Value = task.TaskID;

					if (command.ExecuteNonQuery() > 0)
					{
						return new IDataAccessor.DataAccessResult<int>(true, 0, null);
					}
					else
					{
						return new IDataAccessor.DataAccessResult<int>(false, -1, "Задача не найдена");
					}
				}
			}
			catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Задача не может быть в состоянии \"{task.TaskState}\"");
			}
			catch (SqliteException ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Ошибка код: {ex.SqliteErrorCode} сообщение: {ex.Message}");
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, ex.Message);
			}
		}

		public IDataAccessor.DataAccessResult<int> GetUserID(string login)
		{
			const string getUserIdByLoginCommand = "SELECT Id FROM Users WHERE Login = @Login";

			try
			{
				using (var command = new SqliteCommand(getUserIdByLoginCommand, m_conn))
				{
					command.Parameters.Add("@Login", SqliteType.Text).Value = login;
				
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							int id = reader.GetInt32(reader.GetOrdinal("Id"));

							return new IDataAccessor.DataAccessResult<int>(true, id, null);
						}
						else
						{
							return new IDataAccessor.DataAccessResult<int>(false, -1, "Пользователь не найден");
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, ex.Message);
			}
		}

		public IDataAccessor.DataAccessResult<ProjectTask> GetTaskByName(int userID, string taskName)
		{
			const string getTaskCommand = "SELECT * FROM Tasks WHERE UserId = @UserId AND TaskName = @TaskName";

			try
			{
				using (var command = new SqliteCommand(getTaskCommand, m_conn))
				{
					command.Parameters.Add("@UserId", SqliteType.Integer).Value = userID;
					command.Parameters.Add("@TaskName", SqliteType.Text).Value = taskName;

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							ProjectTask projectTask = new(
								reader.GetInt32(reader.GetOrdinal("UserId")),
								reader.GetString(reader.GetOrdinal("TaskName")),
								reader.GetString(reader.GetOrdinal("TaskState")),
								reader.GetInt32(reader.GetOrdinal("ProjectID")),
								reader.GetString(reader.GetOrdinal("Description")),
								reader.GetInt32(reader.GetOrdinal("TaskID")));

							return new IDataAccessor.DataAccessResult<ProjectTask>(true, projectTask, null);
						}
						else
						{
							return new IDataAccessor.DataAccessResult<ProjectTask>(false, null, "Задача не найдена");
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new IDataAccessor.DataAccessResult<ProjectTask>(false, null, ex.Message);
			}
		}
	}
}
