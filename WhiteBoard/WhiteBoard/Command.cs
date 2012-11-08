using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using log4net;

namespace WhiteBoard
{
    //@author U096089W
    abstract class Command
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Command));
        protected FileHandler fileHandler;
        protected CommandType commandType;
        protected List<Task> screenState;

        public Command(FileHandler fileHandler, List<Task> screenState)
        {
            this.fileHandler = fileHandler;
            this.screenState = screenState;
        }

        public abstract CommandType CommandType
        {
            get;
        }

        public abstract List<Task> Execute();
        public abstract List<Task> Undo();
    }
}
