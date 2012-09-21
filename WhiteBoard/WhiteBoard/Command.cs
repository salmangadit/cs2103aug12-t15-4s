using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    abstract class Command
    {
        protected FileHandler fileHandler;

        public void Command(FileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
        }

        public abstract ToDo Execute();
    }
}
