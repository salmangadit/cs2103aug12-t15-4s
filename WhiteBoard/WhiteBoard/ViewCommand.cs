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

            if (viewTaskDetails.Archive == true)
            {
                Log.Debug("View archived tasks");
                return fileHandler.ViewArchive();
            }
            else if (viewTaskDetails.Archive == false && viewTaskDetails.StartTime == null && viewTaskDetails.EndTime == null)
            {
                Log.Debug("View all tasks");
                return fileHandler.ViewAll();
            }
            else if (viewTaskDetails.Archive == false && viewTaskDetails.EndTime == null)
            {
                Log.Debug("View tasks starting from" + viewTaskDetails.StartTime);
                return fileHandler.ViewTasks(viewTaskDetails.StartTime);
            }
            else if (viewTaskDetails.Archive == false)
            {
                Log.Debug(String.Format("View tasks starting from {0} and ending on {1}", viewTaskDetails.StartTime, viewTaskDetails.EndTime));
                return fileHandler.ViewTasks(viewTaskDetails.StartTime, viewTaskDetails.EndTime);
            }
            else
            {
                Log.Debug("Invalid condition for Viewing Tasks");
                throw new NotImplementedException("There is no such criteria for file viewing!");
            }
        }

        public override List<Task> Undo()
        {
            return screenState;
        }
    }
}
