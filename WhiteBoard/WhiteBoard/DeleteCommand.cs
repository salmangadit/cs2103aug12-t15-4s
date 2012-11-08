using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    //@author U096089W
    /// <summary>
    /// Deletes Task in file
    /// </summary>
    class DeleteCommand : Command
    {
        #region Private Fields
        private List<int> taskIdsToDelete;
        private List<Task> tasksToDelete;
        #endregion

        #region Constructors
        public DeleteCommand(FileHandler fileHandler, int taskIdToDelete, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToDelete = new List<int>();
            tasksToDelete = new List<Task>();
            this.taskIdsToDelete.Add(taskIdToDelete);
            this.commandType = CommandType.Delete;
        }

        public DeleteCommand(FileHandler fileHandler, List<int> taskIdsToDelete, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToDelete = new List<int>();
            tasksToDelete = new List<Task>();
            this.taskIdsToDelete = taskIdsToDelete;
            this.commandType = CommandType.Delete;
        }

        public DeleteCommand(FileHandler fileHandler, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToDelete = new List<int>();
            tasksToDelete = new List<Task>();
            foreach (Task task in screenState)
            {
                this.taskIdsToDelete.Add(task.Id);
            }
            this.commandType = CommandType.Delete;
        }
        #endregion

        #region Public Properties
        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }
        #endregion

        #region Public Class Methods
        public override List<Task> Execute()
        {
            if (taskIdsToDelete.Count == 0)
            {
                throw new ApplicationException(Constants.DELETE_COMMAND_NO_TASKS);
            }

            foreach (int taskIdToDelete in taskIdsToDelete)
            {
                Debug.Assert(taskIdToDelete > 0, Constants.DEBUG_INVALID_TASK_ID);

                bool isTaskDeleted;

                try
                {
                    tasksToDelete.Add(fileHandler.GetTaskFromFile(taskIdToDelete));
                    isTaskDeleted = fileHandler.DeleteTaskFromFile(taskIdToDelete);
                }
                catch (SystemException e)
                {
                    Log.Debug(Constants.LOG_SYSTEM_EXCEPTION + e);
                    throw new ApplicationException(Constants.DELETE_COMMAND_NO_TASKS);
                }

                if (isTaskDeleted)
                {
                    Log.Debug(Constants.DELETE_COMMAND_LOG_EXECUTED + taskIdToDelete);
                }
                else
                {
                    Log.Debug(Constants.DELETE_COMMAND_LOG_FAILED + taskIdToDelete);
                    throw new ApplicationException(Constants.DELETE_COMMAND_UNABLE + taskIdToDelete);
                }
            }

            return tasksToDelete;
        }

        public override List<Task> Undo()
        {
            foreach (Task task in tasksToDelete)
            {
                Debug.Assert(task.Id > 0, Constants.DEBUG_INVALID_TASK_ID);
                fileHandler.AddTaskToFile(task, task.Id);
                Log.Debug(Constants.DELETE_COMMAND_UNDO_LOG_EXECUTED + task.Id);
            }
            return screenState;
        }

        public List<int> GetDeletedTaskId()
        {
            return taskIdsToDelete;
        }
        #endregion
    }
}
