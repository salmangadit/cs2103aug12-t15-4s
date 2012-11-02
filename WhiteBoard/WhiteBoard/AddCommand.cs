using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using log4net;

namespace WhiteBoard
{
    class AddCommand : Command
    {
        Task taskToAdd;
        List<Task> addedTask = new List<Task>();

        public AddCommand(FileHandler fileHandler, Task taskToAdd, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.taskToAdd = taskToAdd;
            this.commandType = CommandType.Add;
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
            Debug.Assert(taskToAdd != null, "Task to add is null");

            if (taskToAdd.StartTime == null)
            {
                if (taskToAdd.EndTime != null)
                {
                    throw new ApplicationException("Task can't have an end time without start time");
                }
            }

            if (!isFloatingTask(taskToAdd) && (((DateTime)taskToAdd.StartTime).DayOfYear < DateTime.Now.DayOfYear))
            {
                throw new ApplicationException("Task cannot start in the past");
            }

            if (!isFloatingTask(taskToAdd) && (taskToAdd.StartTime > taskToAdd.EndTime))
            {
                throw new ApplicationException("Task cannot begin after it ends!");
            }

            if (taskToAdd.Description.Equals(String.Empty))
            {
                throw new ApplicationException("Please provide a task description");
            }

            fileHandler.AddTaskToFile(taskToAdd);
            addedTask.Add(taskToAdd);

            Log.Debug("Add Command was executed for task" + taskToAdd.Id);

            return addedTask;
        }

        private bool isFloatingTask(Task taskToAdd)
        {
            if ((taskToAdd.StartTime == null || taskToAdd.EndTime == null) && !(taskToAdd.StartTime == null && taskToAdd.EndTime != null))
            {
                return true;
            }

            return false;
        }

        public override List<Task> Undo()
        {
            Debug.Assert(addedTask[0] != null, "No task in added task list");

            if (fileHandler.DeleteTaskFromFile(addedTask[0].Id))
            {
                Log.Debug("Add Command Undone for task" + addedTask[0].Id);
                return screenState;
            }
            else
            {
                Log.Debug("Add Command Undo Failed for task" + addedTask[0].Id);
                throw new SystemException("Add Command Undo Failed for task" + addedTask[0].Id);
            }
        }
    }
}
