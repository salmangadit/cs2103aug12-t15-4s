using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    class EditCommand : Command
    {
        List<Task> editTasksDetails;
        List<Task> uneditedTasks;

        public EditCommand(FileHandler fileHandler, Task editTaskDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            editTasksDetails = new List<Task>();
            uneditedTasks = new List<Task>();
            this.editTasksDetails.Add(editTaskDetails);
            this.commandType = CommandType.Edit;
        }

        public EditCommand(FileHandler fileHandler, List<Task> editTasksDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            editTasksDetails = new List<Task>();
            uneditedTasks = new List<Task>();
            this.editTasksDetails = editTasksDetails;
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
            List<Task> editedTasks = new List<Task>();

            foreach (Task editTaskDetails in editTasksDetails)
            {
                Debug.Assert(editTaskDetails.Id > 0, "Task ID is invalid");

                int editedTaskId = editTaskDetails.Id;
                Task uneditedTask = fileHandler.GetTaskFromFile(editedTaskId);

                uneditedTasks.Add(uneditedTask);

                string editedTaskDescription = (editTaskDetails.Description == null) ? uneditedTask.Description : editTaskDetails.Description;
                DateTime? editedTaskStartTime = (editTaskDetails.StartTime == null) ? uneditedTask.StartTime : editTaskDetails.StartTime;
                DateTime? editedTaskEndTime = (editTaskDetails.EndTime == null) ? uneditedTask.EndTime : editTaskDetails.EndTime;

                Task editedTask = new Task(editedTaskId, editedTaskDescription, editedTaskStartTime, editedTaskEndTime);

                if (!isFloatingTask(editedTask) && (((DateTime)editedTask.StartTime).DayOfYear < DateTime.Now.DayOfYear))
                {
                    throw new ApplicationException("Task cannot start in the past");
                }

                if (!isFloatingTask(editedTask) && (editedTask.StartTime > editedTask.EndTime))
                {
                    throw new ApplicationException("Task cannot begin after it ends!");
                }

                if (editedTask.Description.Equals(String.Empty))
                {
                    throw new ApplicationException("Please provide a task description");
                }

                bool isTaskWrittern = fileHandler.WriteEditedTaskToFile(editedTask);

                if (isTaskWrittern)
                {
                    Log.Debug("Edit Command was executed for" + editedTaskId);
                }
                else
                {
                    Log.Debug("Edit Command failed for" + editedTaskId);
                    throw new SystemException("Unable To Edit Task" + editedTaskId);
                }

                editedTasks.Add(editedTask);
            }

            return editedTasks;
        }

        private bool isFloatingTask(Task taskToAdd)
        {
            if ((taskToAdd.StartTime == null || taskToAdd.EndTime == null) && !(taskToAdd.StartTime != null && taskToAdd.EndTime == null))
            {
                return true;
            }

            return false;
        }

        public override List<Task> Undo()
        {
            foreach (Task uneditedTask in uneditedTasks)
            {
                Debug.Assert(uneditedTask.Id > 0, "Invalid task ID");

                bool isTaskWrittern = fileHandler.WriteEditedTaskToFile(uneditedTask);

                if (isTaskWrittern)
                {
                    Log.Debug("Edit Command Undo was executed for" + uneditedTask.Id);
                }
                else
                {
                    Log.Debug("Edit Command Undo failed for" + uneditedTask.Id);
                    throw new SystemException("Unable To Undo Edit Command for Task" + uneditedTask.Id);
                }
            }

            return screenState;
        }
    }
}
