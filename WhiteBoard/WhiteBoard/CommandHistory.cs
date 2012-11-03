using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    class CommandHistory
    {
        List<string> commandHistory;
        int currIndex = -1;

        public CommandHistory()
        {
            commandHistory = new List<string>();
        }

        public void AddToHistory(string command)
        {
            commandHistory.Add(command);
            currIndex = -1;
        }

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
    }
}
