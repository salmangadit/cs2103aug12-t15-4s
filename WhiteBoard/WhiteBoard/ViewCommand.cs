using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    class ViewCommand : Command
    {
        Task viewTaskDetails;

        public ViewCommand(FileHandler fileHandler, Task viewTaskDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.viewTaskDetails = viewTaskDetails;
            this.commandType = CommandType.View;
        }

        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }

        public override List<Task> Execute()
        {
            Debug.Assert(viewTaskDetails != null, "Task Details not set");

            List<Task> tasksToView = new List<Task>();

            if (viewTaskDetails.Archive == true)
            {
                Log.Debug("View archived tasks");
                tasksToView = fileHandler.ViewArchive();
            }
            else if (viewTaskDetails.Archive == false && viewTaskDetails.StartTime == null && viewTaskDetails.EndTime == null)
            {
                Log.Debug("View all tasks");
                tasksToView = fileHandler.ViewAll();
            }
            else if (viewTaskDetails.Archive == false && viewTaskDetails.EndTime == null)
            {
                Log.Debug("View tasks ending on" + viewTaskDetails.StartTime);
                tasksToView = fileHandler.ViewTasks(viewTaskDetails.StartTime);
            }
            else if (viewTaskDetails.Archive == false)
            {
                // change time of end to 2359
                // this should be done in parser, looks messy to do it here
                DateTime endTime = (DateTime)viewTaskDetails.EndTime;
                viewTaskDetails.EndTime = endTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                Log.Debug(String.Format("View tasks starting from {0} and ending on {1}", viewTaskDetails.StartTime, viewTaskDetails.EndTime));
                tasksToView = fileHandler.ViewTasks(viewTaskDetails.StartTime, viewTaskDetails.EndTime);
            }
            else
            {
                Log.Debug("Invalid condition for Viewing Tasks");
                throw new NotImplementedException("There is no such criteria for file viewing!");
            }

            if (tasksToView.Count == 0)
            {
                throw new ApplicationException("There are no tasks to view");
            }

            return tasksToView;
        }

        public override List<Task> Undo()
        {
            return screenState;
        }
    }
}
