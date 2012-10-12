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
         
        public ArchiveCommand(FileHandler fileHandler, int taskIdToArchive, ObservableCollection<Task> screenState)
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

        public override ObservableCollection<Task> Undo()
        {
            // salman to add his code here
            return null;
        }
    }
}
