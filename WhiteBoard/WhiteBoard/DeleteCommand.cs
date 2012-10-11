using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class DeleteCommand : Command
    {
        int taskIdToDelete;

        public DeleteCommand(FileHandler fileHandler, int taskIdToDelete, ObservableCollection<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.taskIdToDelete = taskIdToDelete;
            this.commandType = CommandType.Delete;
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
            bool isTaskDeleted = fileHandler.DeleteTaskFromFile(taskIdToDelete);

            if (!isTaskDeleted)
                throw new ApplicationException("Unable To Delete Task");

            return null;
        }

        public override ObservableCollection<Task> Undo()
        {
            // salman to add his code here
            return null;
        }
    }
}
