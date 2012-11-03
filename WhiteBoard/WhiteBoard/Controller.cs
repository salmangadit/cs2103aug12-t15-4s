using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class Controller
    {
        Stack<Command> history;

        public Controller()
        {
            history = new Stack<Command>();
        }

        public Command GetCommandObject(string userString, List<Task> screenState)
        {
            CommandParser commandParser = new CommandParser(screenState, history);
            return commandParser.ParseCommand(userString);
        }

        public Command GetAllTasks(List<Task> screenState)
        {
            CommandParser commandParser = new CommandParser(screenState, history);
            return commandParser.ParseCommand("VIEW ALL");
        }

        public Command Undo(List<Task> screenState)
        {
            CommandParser commandParser = new CommandParser(screenState, history);
            return commandParser.ParseCommand("UNDO");
        }
    }
}
