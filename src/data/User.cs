namespace ProjectControlSystem.src.data
{
	public class User
	{
		public int Id { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }

		public User(int id, string login, string password, string role)
		{
			Id = id;
			Login = login;
			Password = password;
			Role = role;
		}

		public User(string login, string password, string role)
		{
			Id = 0;
			Login = login;
			Password = password;
			Role = role;
		}
	}
}
