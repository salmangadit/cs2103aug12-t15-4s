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
        Task taskToDelete;

        public DeleteCommand(FileHandler fileHandler, int taskIdToDelete, List<Task> screenState)
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
            taskToDelete = fileHandler.GetTaskFromFile(taskIdToDelete);
            bool isTaskDeleted = fileHandler.DeleteTaskFromFile(taskIdToDelete);

            if (!isTaskDeleted)
                throw new ApplicationException("Unable To Delete Task");

            return null;
        }

        public override List<Task> Undo()
        {
            fileHandler.AddTaskToFile(taskToDelete);
            return screenState;
        }

        public int GetDeletedTaskId()
        {
            return taskIdToDelete;
        }
    }
}
