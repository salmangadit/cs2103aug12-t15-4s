using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    abstract class Command
    {
        protected FileHandler fileHandler;
        protected CommandType commandType;

        public Command(FileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
        }

        public abstract CommandType CommandType
        {
            get;
        }

        public abstract ToDo Execute();

    }
}
