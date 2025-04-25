using ProjectControlSystem.src.data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectControlSystem.src
{
	internal class SqlDataAccessor : IDataAccessor
	{
		private readonly SqlConnection m_conn;

		public SqlDataAccessor(string connectionString)
		{
			m_conn = new SqlConnection(connectionString);
			m_conn.Open();
		}

		~SqlDataAccessor()
		{
			m_conn?.Close();
		}

		public bool AddUser(User user)
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

					return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.Message);
				return false;
			}
		}

		public IEnumerable<ProjectTask> GetTasksOfUser(User user)
		{
			throw new NotImplementedException();
		}

		public User? GetUser(string login, string password)
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
							return new User(
								reader.GetInt32(reader.GetOrdinal("Id")),
								reader.GetString(reader.GetOrdinal("Login")),
								reader.GetString(reader.GetOrdinal("Password")),
								reader.GetString(reader.GetOrdinal("Role")));
						}
						else
						{
							return null;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.Message);
				return null;
			}
		}
	}
}
