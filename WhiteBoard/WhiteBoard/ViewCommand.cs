using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class ViewCommand : Command
    {
        Task tasksToView;

        public ViewCommand(FileHandler fileHandler, Task viewTaskDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.tasksToView = viewTaskDetails;
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
            if (tasksToView.Archive == true)
            {
                return fileHandler.ViewArchive();
            }
            else if (tasksToView.Archive == false && tasksToView.Deadline == null && tasksToView.StartTime == null && tasksToView.EndTime == null)
            {
                return fileHandler.ViewAll();
            }
            else if (tasksToView.Archive == false && tasksToView.Deadline == null)
            {
                return fileHandler.ViewTasks(tasksToView.StartTime, tasksToView.EndTime);
            }
            else if (tasksToView.Archive == false && tasksToView.StartTime == null && tasksToView.EndTime == null)
            {
                return fileHandler.ViewTasks(tasksToView.Deadline);
            }
            else
            {
                throw new NotImplementedException("There is no such criteria for file viewing!");
            }
        }

        public override List<Task> Undo()
        {
            return screenState;
        }
    }
}
