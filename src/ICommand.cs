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
