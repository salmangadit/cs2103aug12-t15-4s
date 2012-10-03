using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    class AddCommand : Command
    {
        Task taskToAdd;

        public AddCommand(FileHandler fileHandler, Task taskToAdd)
            : base(fileHandler)
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

        public override Task Execute()
        {
            fileHandler.WriteToFile(taskToAdd);

            return taskToAdd;
        }
    }
}
