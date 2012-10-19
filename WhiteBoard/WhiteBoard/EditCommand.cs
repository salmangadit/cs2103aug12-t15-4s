using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class EditCommand : Command
    {
        Task editTaskDetails;
        Task uneditedTask;

        public EditCommand(FileHandler fileHandler, Task editTaskDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.editTaskDetails = editTaskDetails;
            this.commandType = CommandType.Edit;
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
            int editedTaskId = editTaskDetails.Id;
            uneditedTask = fileHandler.GetTaskFromFile(editedTaskId);

            string editedTaskDescription = (editTaskDetails.Description == null) ? uneditedTask.Description : editTaskDetails.Description;
            DateTime? editedTaskStartTime = (editTaskDetails.StartTime == null) ? uneditedTask.StartTime : editTaskDetails.StartTime;
            DateTime? editedTaskEndTime = (editTaskDetails.EndTime == null) ? uneditedTask.EndTime : editTaskDetails.EndTime;

            Task editedTask = new Task(editedTaskId, editedTaskDescription, editedTaskStartTime, editedTaskEndTime);

            fileHandler.WriteEditedTaskToFile(editedTask);

            List<Task> editedTasks = new List<Task>();
            editedTasks.Add(editedTask);

            return editedTasks;
        }

        public override List<Task> Undo()
        {
            fileHandler.WriteEditedTaskToFile(uneditedTask);
            return screenState;
        }
    }
}
