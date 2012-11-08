using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using log4net;

namespace WhiteBoard
{
    //@author U096089W
    /// <summary>
    /// Abstract Class for all commands
    /// </summary>
    abstract class Command
    {
        #region Protected Fields
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Command));
        protected FileHandler fileHandler;
        protected CommandType commandType;
        protected List<Task> screenState;
        #endregion

        #region Constructors
        public Command(FileHandler fileHandler, List<Task> screenState)
        {
            this.fileHandler = fileHandler;
            this.screenState = screenState;
        }
        #endregion

        #region Public Properties
        public abstract CommandType CommandType
        {
            get;
        }
        #endregion

        #region Public Class Methods
        public abstract List<Task> Execute();
        public abstract List<Task> Undo();
        #endregion
    }
}
