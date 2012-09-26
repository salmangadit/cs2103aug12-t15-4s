using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    class Controller
    {
        FileHandler fileHandler;

        public Controller()
        {
            fileHandler = new FileHandler();
        }

        public Command GetCommandObject(string userString)
        {
            CommandParser commandParser = new CommandParser(userString, fileHandler);
            return commandParser.ParseCommand();
        }
    }
}
