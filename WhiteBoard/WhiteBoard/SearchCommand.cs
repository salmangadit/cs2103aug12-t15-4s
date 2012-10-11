using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class SearchCommand : Command
    {
        string searchString;

        public SearchCommand(FileHandler fileHandler, string searchString, ObservableCollection<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.searchString = searchString;
            this.commandType = CommandType.Search;
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
            
        }

        public override ObservableCollection<Task> Undo()
        {
            //salman to add his code here 
            return null;
        }
    }
}
