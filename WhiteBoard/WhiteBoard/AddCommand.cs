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
        //Task taskToAdd;
        List<Task> tasksToAdd;

        public AddCommand(FileHandler fileHandler, Task taskToAdd, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            tasksToAdd = new List<Task>();
            tasksToAdd.Add(taskToAdd);
            this.commandType = CommandType.Add;
        }

        public AddCommand(FileHandler fileHandler, List<Task> tasksToAdd, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            tasksToAdd = new List<Task>();
            this.tasksToAdd = tasksToAdd;
            this.commandType = CommandType.Archive;
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
            if (tasksToAdd.Count == 0)
            {
                throw new ApplicationException("No tasks to add");
            }

            foreach (Task taskToAdd in tasksToAdd)
            {

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

                bool isTaskAdded = fileHandler.AddTaskToFile(taskToAdd);

                if (isTaskAdded)
                {
                    Log.Debug("Add Command was executed for" + taskToAdd.Id);
                }
                else
                {
                    Log.Debug("Add Command failed for" + taskToAdd.Id);
                    throw new ApplicationException("Unable To Add Task with ID T" + taskToAdd.Id);
                }
            }

            return tasksToAdd;
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
            foreach (Task taskAdded in tasksToAdd)
            {
                if (fileHandler.DeleteTaskFromFile(taskAdded.Id))
                {
                    Log.Debug("Add Command Undone for task" + taskAdded.Id);
                }
                else
                {
                    Log.Debug("Add Command Undo Failed for task" + taskAdded.Id);
                    throw new SystemException("Add Command Undo Failed for task" + taskAdded.Id);
                }
            }

            return screenState;
        }
    }
}
