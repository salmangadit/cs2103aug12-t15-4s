using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    class AddCommand : Command
    {
        ToDo taskToAdd;

        public AddCommand(ToDo taskToAdd)
        {
            this.taskToAdd = taskToAdd;
        }

        public override ToDo Execute()
        {
            fileHandler.WriteToFile(taskToAdd.ToString());

            return taskToAdd;
        }
    }
}
