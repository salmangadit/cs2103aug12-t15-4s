using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class AddCommand : Command
    {
        Task taskToAdd;
        List<Task> addedTask = new List<Task>();

        public AddCommand(FileHandler fileHandler, Task taskToAdd, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.taskToAdd = taskToAdd;
            this.commandType = CommandType.Add;
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
            fileHandler.AddTaskToFile(taskToAdd);
            addedTask.Add(taskToAdd);

            return addedTask;
        }

        public override List<Task> Undo()
        {
            if (fileHandler.DeleteTaskFromFile(addedTask[0].Id))
                return screenState;
            
            return null;
        }
    }
}
