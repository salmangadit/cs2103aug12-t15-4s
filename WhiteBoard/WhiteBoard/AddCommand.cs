using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    class AddCommand : Command
    {
        ToDo taskToAdd;

        public AddCommand(FileHandler fileHandler, ToDo taskToAdd)
            : base(fileHandler)
        {
            this.taskToAdd = taskToAdd;
        }

        public override ToDo Execute()
        {
            fileHandler.WriteToFile(taskToAdd);

            return taskToAdd;
        }
    }
}
