using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using log4net;

namespace WhiteBoard
{
    //@author U096089W
    /// <summary>
    /// To add Task to file
    /// </summary>
    class AddCommand : Command
    {
        #region Private Fields
        private List<Task> tasksToAdd;
        #endregion

        #region Constructors
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
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the type of Command
        /// </summary>
        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Adds the Task(s) passed in through the constructor
        /// </summary>
        /// <returns>The added Task(s)</returns>
        public override List<Task> Execute()
        {
            if (tasksToAdd.Count == 0)
            {
                throw new ApplicationException(Constants.ADD_COMMAND_NO_TASKS);
            }

            foreach (Task taskToAdd in tasksToAdd)
            {
                if (taskToAdd.StartTime == null)
                {
                    if (taskToAdd.EndTime != null)
                    {
                        throw new ApplicationException(Constants.TASK_NO_START_GOT_END);
                    }
                }

                if (!isFloatingTask(taskToAdd) && (((DateTime)taskToAdd.StartTime).Date < DateTime.Now.Date))
                {
                    throw new ApplicationException(Constants.TASK_START_IN_PAST);
                }

                if (!isFloatingTask(taskToAdd) && (taskToAdd.StartTime > taskToAdd.EndTime))
                {
                    throw new ApplicationException(Constants.TASK_BEGIN_AFTER_END);
                }

                if (taskToAdd.Description.Equals(String.Empty))
                {
                    throw new ApplicationException(Constants.TASK_NO_DESCRIPTION);
                }

                bool isTaskAdded = fileHandler.AddTaskToFile(taskToAdd);

                if (isTaskAdded)
                {
                    Log.Debug(Constants.ADD_COMMAND_LOG_EXECUTED + taskToAdd.Id);
                }
                else
                {
                    Log.Debug(Constants.ADD_COMMAND_LOG_FAILED + taskToAdd.Id);
                    throw new ApplicationException(Constants.ADD_COMMAND_UNABLE + taskToAdd.Id);
                }
            }

            return tasksToAdd;
        }

        //@author U095146E
        /// <summary>
        /// Remove the Task that was added
        /// </summary>
        /// <returns>The screen state before adding the Task</returns>
        public override List<Task> Undo()
        {
            foreach (Task taskAdded in tasksToAdd)
            {
                if (fileHandler.DeleteTaskFromFile(taskAdded.Id))
                {
                    Log.Debug(Constants.ADD_COMMAND_UNDO_LOG_EXECUTED + taskAdded.Id);
                }
                else
                {
                    Log.Debug(Constants.ADD_COMMAND_UNDO_LOG_FAILED + taskAdded.Id);
                    throw new SystemException(Constants.ADD_COMMAND_UNDO_UNABLE + taskAdded.Id);
                }
            }

            return screenState;
        }
        #endregion

        //@author U096089W
        #region Private Class Helper Methods
        /// <summary>
        /// Checks if a given Task is a floating task
        /// </summary>
        /// <param name="taskToAdd">The Task that is being added</param>
        /// <returns>True if task is floating, false if not</returns>
        private bool isFloatingTask(Task taskToAdd)
        {
            if ((taskToAdd.StartTime == null || taskToAdd.EndTime == null) && !(taskToAdd.StartTime == null && taskToAdd.EndTime != null))
            {
                return true;
            }

            return false;
        }
        #endregion

    }
}
