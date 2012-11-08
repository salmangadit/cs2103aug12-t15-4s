using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    //@author U095146E
    class CommandHistory
    {
        #region Private Fields
        private List<string> commandHistory;
        private int currIndex = -1;
        #endregion

        #region Constructors
        public CommandHistory()
        {
            commandHistory = new List<string>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Adds command entered by user to command history List
        /// </summary>
        /// <param name="command">Command entered by user</param>
        public void AddToHistory(string command)
        {
            if (command.Split(' ')[0].ToLower() == "search:")
                return;

            commandHistory.Add(command);
            currIndex = -1;
        }
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Returns previous string on list
        /// </summary>
        public string UpClick()
        {
            if (commandHistory.Count == 0)
                return null;

            if (currIndex == -1)
                currIndex = commandHistory.Count - 1;
            else if (currIndex == 0)
                return null;
            else
                currIndex -= 1;

            return commandHistory[currIndex];
        }
        /// <summary>
        /// Returns next string on list
        /// </summary>
        public string DownClick()
        {
            if (commandHistory.Count == 0)
                return null;

            if (currIndex == -1)
                return null;
            else if (currIndex == commandHistory.Count - 1)
                return null;
            else
                currIndex += 1;

            return commandHistory[currIndex];
        }
        #endregion
    }
}
