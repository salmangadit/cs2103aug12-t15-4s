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
    /// Archive Task in file
    /// </summary>
    class ArchiveCommand : Command
    {
        #region Private Fields
        private List<int> taskIdsToArchive;
        private List<Task> tasksArchived;
        #endregion

        #region Constructors
        public ArchiveCommand(FileHandler fileHandler, int taskIdToArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToArchive = new List<int>();
            tasksArchived = new List<Task>();
            this.taskIdsToArchive.Add(taskIdToArchive);
            this.commandType = CommandType.Archive;
        }

        public ArchiveCommand(FileHandler fileHandler, List<int> taskIdsToArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToArchive = new List<int>();
            tasksArchived = new List<Task>();
            this.taskIdsToArchive = taskIdsToArchive;
            this.commandType = CommandType.Archive;
        }

        public ArchiveCommand(FileHandler fileHandler, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToArchive = new List<int>();
            tasksArchived = new List<Task>();
            foreach (Task task in screenState)
            {
                this.taskIdsToArchive.Add(task.Id);
            }
            this.commandType = CommandType.Archive;
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
            if (taskIdsToArchive.Count == 0)
            {
                throw new ApplicationException(Constants.ARCHIVE_COMMAND_NO_TASKS);
            }

            foreach (int taskIdToArchive in taskIdsToArchive)
            {
                Debug.Assert(taskIdToArchive > 0, Constants.DEBUG_INVALID_TASK_ID);
                bool isTaskArchived;

                try
                {
                    tasksArchived.Add(fileHandler.GetTaskFromFile(taskIdToArchive));
                    isTaskArchived = fileHandler.ArchiveTaskInFile(taskIdToArchive);
                }
                catch (SystemException e)
                {
                    Log.Debug(Constants.LOG_SYSTEM_EXCEPTION + e);
                    throw new ApplicationException(Constants.ARCHIVE_COMMAND_NO_TASKS);
                }

                if (isTaskArchived)
                {
                    Log.Debug(Constants.ARCHIVE_COMMAND_LOG_EXECUTED + taskIdToArchive);
                }
                else
                {
                    Log.Debug(Constants.ARCHIVE_COMMAND_LOG_FAILED + taskIdToArchive);
                    throw new ApplicationException(Constants.ARCHIVE_COMMAND_UNABLE + taskIdToArchive);
                }
            }

            return tasksArchived;
        }

        //@author U095146E
        public override List<Task> Undo()
        {
            foreach (int taskIdToArchive in taskIdsToArchive)
            {
                Debug.Assert(taskIdToArchive > 0, Constants.DEBUG_INVALID_TASK_ID);
                bool isTaskUnarchived = fileHandler.UnarchiveTaskInFile(taskIdToArchive);

                if (isTaskUnarchived)
                {
                    Log.Debug(Constants.ARCHIVE_COMMAND_UNDO_LOG_EXECUTED + taskIdToArchive);
                }
                else
                {
                    Log.Debug(Constants.ARCHIVE_COMMAND_UNDO_LOG_FAILED + taskIdToArchive);
                    throw new ApplicationException(Constants.ARCHIVE_COMMAND_UNDO_UNABLE + taskIdToArchive);
                }

            }
            return screenState;
        }

        public List<int> GetArchivedTaskIds()
        {
            return taskIdsToArchive;
        }
        #endregion
    }
}
