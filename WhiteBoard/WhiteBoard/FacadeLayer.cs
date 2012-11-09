using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    //@author U095146E
    class FacadeLayer
    {
        #region Private Fields
        private Stack<Command> history;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for controller
        /// </summary>
        public FacadeLayer()
        {
            history = new Stack<Command>();
        }
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Parses user string and returns a command object
        /// </summary>
        /// <param name="userString">Command entered by user</param>
        /// <param name="screenState">State of screen as user sent command</param>
        /// <returns>Command object created after parsing</returns>
        public Command GetCommandObject(string userString, List<Task> screenState)
        {
            CommandParser commandParser = new CommandParser(screenState, history);
            return commandParser.ParseCommand(userString);
        }

        /// <summary>
        /// Returns all tasks on screen
        /// </summary>
        /// <param name="screenState">Current state of screen</param>
        /// <returns>Command object created after parsing</returns>
        public Command GetAllTasks(List<Task> screenState)
        {
            CommandParser commandParser = new CommandParser(screenState, history);
            return commandParser.ParseCommand(Constants.USER_COMMAND_VIEW_ALL);
        }

        /// <summary>
        /// Performs undo operation
        /// </summary>
        /// <param name="screenState">Current state of screen</param>
        /// <returns>Command object created after parsing</returns>
        public Command Undo(List<Task> screenState)
        {
            CommandParser commandParser = new CommandParser(screenState, history);
            return commandParser.ParseCommand(Constants.USER_COMMAND_UNDO);
        }
        #endregion    
    }
}
