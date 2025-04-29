using ProjectControlSystem.src.data;
using System.Data.SqlClient;

namespace ProjectControlSystem.src
{
	internal class SqlDataAccessor : IDataAccessor
	{
		private readonly SqlConnection m_conn;

		public SqlDataAccessor()
		{
			string dbFolder = AppDomain.CurrentDomain.BaseDirectory;
			string dbPath = Path.Combine(dbFolder, @"db\Database.mdf");

			string connectionString;

			connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;
                             AttachDbFilename={dbPath};
                             Integrated Security=True;
                             Connect Timeout=30";

			m_conn = new SqlConnection(connectionString);
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
				using (var command = new SqlCommand(addTaskCommand, m_conn))
				{
					command.Parameters.Add("@UserId", System.Data.SqlDbType.Int).Value = task.UserID;
					command.Parameters.Add("@TaskName", System.Data.SqlDbType.NVarChar).Value = task.Name;
					command.Parameters.Add("@TaskState", System.Data.SqlDbType.NVarChar).Value = task.TaskState;
					command.Parameters.Add("@ProjectId", System.Data.SqlDbType.Int).Value = task.ProjectID;
					command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = task.Description;

					command.ExecuteNonQuery();

					return new IDataAccessor.DataAccessResult<int>(true, 0, null);
				}
			}
			catch (SqlException ex) when (ex.Number == 547)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Задача не может быть в состоянии \"{task.TaskState}\"");
			}
			catch (SqlException ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Ошибка код: {ex.Number} сообщение: {ex.Message}");
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
				using (var command = new SqlCommand(addUserCommand, m_conn))
				{
					command.Parameters.Add("@Login", System.Data.SqlDbType.NVarChar).Value = user.Login;
					command.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar).Value = user.Password;
					command.Parameters.Add("@Role", System.Data.SqlDbType.NVarChar).Value = user.Role;

					command.ExecuteNonQuery();

					return new IDataAccessor.DataAccessResult<int>(true, 0, null);
				}
			}
			catch (SqlException ex) when (ex.Number == 547)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Пользователь не может иметь роль \"{user.Role}\"");
			}
			catch (SqlException ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Ошибка код: {ex.Number} сообщение: {ex.Message}");
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
				using (var command = new SqlCommand(getTaskByIDCommand, m_conn))
				{
					command.Parameters.Add("@TaskId", System.Data.SqlDbType.Int).Value = taskID;

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
				using (var command = new SqlCommand(getAllTasksOfUserCommand, m_conn))
				{
					command.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = user.Id;

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
				using (var command = new SqlCommand(getUserCommand, m_conn))
				{
					command.Parameters.Add("@Login", System.Data.SqlDbType.NVarChar).Value = login;
					command.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar).Value = password;

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
				using (var command = new SqlCommand(updateTaskCommand, m_conn))
				{
					command.Parameters.Add("@UserId", System.Data.SqlDbType.Int).Value = task.UserID;
					command.Parameters.Add("@TaskName", System.Data.SqlDbType.NVarChar).Value = task.Name;
					command.Parameters.Add("@TaskState", System.Data.SqlDbType.NVarChar).Value = task.TaskState;
					command.Parameters.Add("@ProjectId", System.Data.SqlDbType.NVarChar).Value = task.ProjectID;
					command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = task.Description;
					command.Parameters.Add("@TaskID", System.Data.SqlDbType.Int).Value = task.TaskID;

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
			catch (SqlException ex) when (ex.Number == 547)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Задача не может быть в состоянии \"{task.TaskState}\"");
			}
			catch (SqlException ex)
			{
				return new IDataAccessor.DataAccessResult<int>(false, -1, $"Ошибка код: {ex.Number} сообщение: {ex.Message}");
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
				using (var command = new SqlCommand(getUserIdByLoginCommand, m_conn))
				{
					command.Parameters.Add("@Login", System.Data.SqlDbType.NVarChar).Value = login;
				
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
				using (var command = new SqlCommand(getTaskCommand, m_conn))
				{
					command.Parameters.Add("@UserId", System.Data.SqlDbType.Int).Value = userID;
					command.Parameters.Add("@TaskName", System.Data.SqlDbType.NVarChar).Value = taskName;

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
