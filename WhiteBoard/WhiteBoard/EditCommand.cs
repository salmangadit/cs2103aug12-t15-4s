using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    class EditCommand : Command
    {
        Task editTaskDetails;

        public EditCommand(FileHandler fileHandler, Task editTaskDetails)
            : base(fileHandler)
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

        public override Task Execute()
        {
            int editedTaskId = editTaskDetails.Id;
            Task oldTask = FileHandler.GetTaskFromFile(editedTaskId);

            string editedTaskDescription = (editTaskDetails.Description == null) ? oldTask.Description : editTaskDetails.Description;
            DateTime editedTaskStartTime = (editTaskDetails.StartTime == null) ? oldTask.StartTime : editTaskDetails.StartTime;
            DateTime editedTaskEndTime = (editTaskDetails.EndTime == null) ? oldTask.EndTime : editTaskDetails.EndTime;
            DateTime editedTaskDeadline = (editTaskDetails.Deadline == null) ? oldTask.Deadline : editTaskDetails.Deadline;

            Task editedTask = new Task(editedTaskId, editedTaskDescription, editedTaskStartTime, editedTaskEndTime, editedTaskDeadline);

            FileHandler.WriteEditedTaskToFile(editedTask);

            return editedTask;
        }
    }
}
