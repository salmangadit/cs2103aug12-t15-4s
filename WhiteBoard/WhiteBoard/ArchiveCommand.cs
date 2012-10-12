using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class ArchiveCommand : Command
    {
        int taskIdToArchive;

        public ArchiveCommand(FileHandler fileHandler, int taskIdToArchive, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.taskIdToArchive = taskIdToArchive;
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
            bool isTaskArchived = fileHandler.ArchiveTaskInFile(taskIdToArchive);

            if (!isTaskArchived)
                throw new ApplicationException("Unable To Archive Task");

            return null;
        }

        public override List<Task> Undo()
        {
            //fileHandler.Unarchive(taskIdToArchive);
            //return screenState;
            throw new NotImplementedException("Cannot work till 'unarchive' method is ready");
        }

        public int GetArchivedTaskId()
        {
            return taskIdToArchive;
        }
    }
}
