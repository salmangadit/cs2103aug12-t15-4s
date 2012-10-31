using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    class UndoCommand : Command
    {
        Command commandToUndo;

        public UndoCommand(FileHandler fileHandler, Command commandToUndo, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.commandToUndo = commandToUndo;
            this.commandType = CommandType.Undo;
        }

        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }

        public override List<Task> Execute()
        {
            Debug.Assert(commandToUndo != null, "Command to undo not set!");
            Log.Debug("Executing Undo Command");
            return commandToUndo.Undo();
        }

        public override List<Task> Undo()
        {
            throw new NotImplementedException();
        }

        public CommandType GetUndoCommandType()
        {
            return commandToUndo.CommandType;
        }
    }
}
