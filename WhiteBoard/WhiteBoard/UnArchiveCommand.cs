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
    /// Unarchive Task in file
    /// </summary>
    class UnArchiveCommand : Command
    {
        #region Private Fields
        private List<int> taskIdsToUnArchive;
        private List<Task> tasksUnArchived;
        #endregion

        #region Constructors
        public UnArchiveCommand(FileHandler fileHandler, int taskIdToUnArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToUnArchive = new List<int>();
            tasksUnArchived = new List<Task>();
            this.taskIdsToUnArchive.Add(taskIdToUnArchive);
            this.commandType = CommandType.UnArchive;
        }

        public UnArchiveCommand(FileHandler fileHandler, List<int> taskIdsToUnArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToUnArchive = new List<int>();
            tasksUnArchived = new List<Task>();
            this.taskIdsToUnArchive = taskIdsToUnArchive;
            this.commandType = CommandType.UnArchive;
        }

        public UnArchiveCommand(FileHandler fileHandler, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToUnArchive = new List<int>();
            tasksUnArchived = new List<Task>();
            foreach (Task task in screenState)
            {
                this.taskIdsToUnArchive.Add(task.Id);
            }
            this.commandType = CommandType.UnArchive;
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
        /// Unarchives the Task(s) with given Id
        /// </summary>
        /// <returns>The Unarchived Task(s)</returns>
        public override List<Task> Execute()
        {
            if (taskIdsToUnArchive.Count == 0)
            {
                throw new ApplicationException(Constants.UNARCHIVE_COMMAND_NO_TASKS);
            }

            foreach (int taskIdToUnArchive in taskIdsToUnArchive)
            {
                Debug.Assert(taskIdToUnArchive > 0, Constants.DEBUG_INVALID_TASK_ID);
                bool isTaskUnArchived;

                try
                {
                    tasksUnArchived.Add(fileHandler.GetTaskFromFile(taskIdToUnArchive));
                    isTaskUnArchived = fileHandler.UnarchiveTaskInFile(taskIdToUnArchive);
                }
                catch (SystemException e)
                {
                    Log.Debug(Constants.LOG_SYSTEM_EXCEPTION + e);
                    throw new ApplicationException(Constants.UNARCHIVE_COMMAND_NO_TASKS);
                }

                if (isTaskUnArchived)
                {
                    Log.Debug(Constants.UNARCHIVE_COMMAND_LOG_EXECUTED + taskIdToUnArchive);
                }
                else
                {
                    Log.Debug(Constants.UNARCHIVE_COMMAND_LOG_FAILED + taskIdToUnArchive);
                    throw new ApplicationException(Constants.UNARCHIVE_COMMAND_UNABLE + taskIdToUnArchive);
                }
            }

            return tasksUnArchived;
        }

        //@author U095146E
        /// <summary>
        /// Unarchives the archived Task(s)
        /// </summary>
        /// <returns>The screen state before archiving the Task(s)</returns>
        public override List<Task> Undo()
        {
            foreach (int taskIdToArchive in taskIdsToUnArchive)
            {
                Debug.Assert(taskIdToArchive > 0, Constants.DEBUG_INVALID_TASK_ID);
                bool isTaskArchived = fileHandler.ArchiveTaskInFile(taskIdToArchive);

                if (isTaskArchived)
                {
                    Log.Debug(Constants.UNARCHIVE_COMMAND_UNDO_LOG_EXECUTED + taskIdToArchive);
                }
                else
                {
                    Log.Debug(Constants.UNARCHIVE_COMMAND_UNDO_LOG_FAILED + taskIdToArchive);
                    throw new ApplicationException(Constants.UNARCHIVE_COMMAND_UNDO_UNABLE + taskIdToArchive);
                }

            }
            return screenState;
        }

        /// <summary>
        /// Returns the unarchived task Id
        /// </summary>
        /// <returns>Unarchived Task id</returns>
        public List<int> GetUnArchivedTaskIds()
        {
            return taskIdsToUnArchive;
        }
        #endregion
    }
}
