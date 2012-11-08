using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    //@author U095146E
    class UndoCommand : Command
    {
        #region Private Fields
        private Command commandToUndo;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for UndoCommand Object
        /// </summary>
        /// <param name="fileHandler">Reference to fileHandler Singleton </param>
        /// <param name="viewTaskDetails">Task object with parameters for current view command </param>
        /// <param name="screenState">Reference to screenState from UI</param>
        public UndoCommand(FileHandler fileHandler, Command commandToUndo, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.commandToUndo = commandToUndo;
            this.commandType = CommandType.Undo;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns type of Command - CommandType.Undo
        /// </summary>
        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Executes Undo Command by checking required criteria
        /// </summary>
        /// <returns>List of Tasks that match undo criteria</returns>
        public override List<Task> Execute()
        {
            Debug.Assert(commandToUndo != null, "Command to undo not set!");
            Log.Debug("Executing Undo Command");
            return commandToUndo.Undo();
        }

        public override List<Task> Undo()
        {
            throw new NotImplementedException(Constants.INVALID_UNDO);
        }

        /// <summary>
        /// Returns type of command that was undone
        /// </summary>
        public CommandType GetUndoCommandType()
        {
            return commandToUndo.CommandType;
        }
        #endregion  
    }
}
