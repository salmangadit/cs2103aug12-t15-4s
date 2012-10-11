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

        public AddCommand(FileHandler fileHandler, Task taskToAdd, ObservableCollection<Task> screenState)
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

            List<Task> addedTask = new List<Task>();
            addedTask.Add(taskToAdd);

            return addedTask;
        }

        public override ObservableCollection<Task> Undo()
        {
            if (fileHandler.DeleteTaskFromFile(taskToAdd.Id))
                return screenState;
            
            return null;
        }
    }
}
