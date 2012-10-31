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

        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }

        public override List<Task> Execute()
        {
            foreach (int taskIdToArchive in taskIdsToArchive)
            {
                Debug.Assert(taskIdToArchive > 0, "Invalid task ID");

                bool isTaskArchived = fileHandler.ArchiveTaskInFile(taskIdToArchive);

                if (isTaskArchived)
                {
                    Log.Debug("Archive Command was executed for" + taskIdToArchive);
                }
                else
                {
                    Log.Debug("Archive Command failed for" + taskIdToArchive);
                    throw new SystemException("Unable To Archive Task" + taskIdToArchive);
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
                    throw new SystemException("Unable To Undo Archive Task" + taskIdToArchive);
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
