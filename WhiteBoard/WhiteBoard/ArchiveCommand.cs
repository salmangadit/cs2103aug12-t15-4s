using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

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

        public ArchiveCommand(FileHandler fileHandler, List<int> taskIdToArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToArchive = new List<int>();
            this.taskIdsToArchive = taskIdToArchive;
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
            bool isTaskArchived = false;

            foreach (int taskIdToArchive in taskIdsToArchive)
            {
                isTaskArchived = fileHandler.ArchiveTaskInFile(taskIdToArchive);
            }

            if (!isTaskArchived)
                throw new ApplicationException("Unable To Archive All Tasks");

            return null;
        }

        public override List<Task> Undo()
        {
            foreach (int taskIdToArchive in taskIdsToArchive)
            {
                fileHandler.UnarchiveTaskInFile(taskIdToArchive);
            }
            return screenState;
        }

        public List<int> GetArchivedTaskIds()
        {
            return taskIdsToArchive;
        }
    }
}
