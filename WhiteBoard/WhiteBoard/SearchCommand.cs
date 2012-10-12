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

        public SearchCommand(FileHandler fileHandler, string searchString, List<Task> screenState)
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
            List<Task> listOfTasks = fileHandler.ViewAll();
            List<Task> tasksContainingSearchString = new List<Task>();

            for (int i = 0; i < listOfTasks.Count; i++)
            {
                if(listOfTasks[i].Description.Contains(searchString))
                {
                    tasksContainingSearchString.Add(listOfTasks[i]);
                }
            }

            return tasksContainingSearchString;
        }

        public override List<Task> Undo()
        {
            return screenState;
        }

        public string GetSearchString()
        {
            return searchString;
        }
    }
}
