using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    abstract class Command
    {
        protected FileHandler fileHandler;
        protected CommandType commandType;
        protected ObservableCollection<Task> screenState;

        public Command(FileHandler fileHandler, ObservableCollection<Task> screenState)
        {
            this.fileHandler = fileHandler;
            this.screenState = screenState;
        }

        public abstract CommandType CommandType
        {
            get;
        }

        public abstract List<Task> Execute();
        public abstract ObservableCollection<Task> Undo();
    }
}
