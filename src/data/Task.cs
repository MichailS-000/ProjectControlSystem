using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectControlSystem.src.data
{
	public class ProjectTask
	{
		public int TaskID { get; set; }
		public int UserID { get; set; }
		public string Name { get; set; }
		public string TaskState { get; set; }
		public string Description { get; set; }	
		public int ProjectID { get; set; }

		public ProjectTask(int userID, string name, string taskState, int projectID, string description, int taskID)
		{
			UserID = userID;
			Name = name;
			TaskState = taskState;
			ProjectID = projectID;
			Description = description;
			TaskID = taskID;
		}
	}
}
