using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    class ArchiveCommand : Command
    {
        List<int> taskIdsToArchive;

        public ArchiveCommand(FileHandler fileHandler, int taskIdToArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToArchive = new List<int>();
            this.taskIdsToArchive.Add(taskIdToArchive);
            this.commandType = CommandType.Archive;
        }

        public ArchiveCommand(FileHandler fileHandler, List<int> taskIdsToArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToArchive = new List<int>();
            this.taskIdsToArchive = taskIdsToArchive;
            this.commandType = CommandType.Archive;
        }

        public ArchiveCommand(FileHandler fileHandler, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToArchive = new List<int>();
            foreach (Task task in screenState)
            {
                this.taskIdsToArchive.Add(task.Id);
            }
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
            if (taskIdsToArchive.Count == 0)
            {
                throw new ApplicationException("No tasks to archive");
            }

            foreach (int taskIdToArchive in taskIdsToArchive)
            {
                Debug.Assert(taskIdToArchive > 0, "Invalid task ID");
                bool isTaskArchived;

                try
                {
                    isTaskArchived = fileHandler.ArchiveTaskInFile(taskIdToArchive);
                }
                catch (SystemException e)
                {
                    Log.Debug("Caught a System Exception" + e);
                    throw new ApplicationException("There are no tasks to archive");
                }

                if (isTaskArchived)
                {
                    Log.Debug("Archive Command was executed for" + taskIdToArchive);
                }
                else
                {
                    Log.Debug("Archive Command failed for" + taskIdToArchive);
                    throw new ApplicationException("Unable To Archive Task with ID T" + taskIdToArchive);
                }
            }

            return new List<Task>();
        }

        public override List<Task> Undo()
        {
            foreach (int taskIdToArchive in taskIdsToArchive)
            {
                Debug.Assert(taskIdToArchive > 0, "Invalid task ID");
                bool isTaskUnarchived = fileHandler.UnarchiveTaskInFile(taskIdToArchive);

                if (isTaskUnarchived)
                {
                    Log.Debug("Archive Command was undone for" + taskIdToArchive);
                }
                else
                {
                    Log.Debug("Archive Command Undo failed for" + taskIdToArchive);
                    throw new ApplicationException("Unable To Undo Archive Task with ID T" + taskIdToArchive);
                }

            }
            return screenState;
        }

        public List<int> GetArchivedTaskIds()
        {
            return taskIdsToArchive;
        }
    }
}
