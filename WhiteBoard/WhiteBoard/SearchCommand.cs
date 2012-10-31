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
            if (String.IsNullOrWhiteSpace(searchString))
            {
                throw new ApplicationException("Enter a keyword to search for");
            }

            List<Task> listOfTasks = fileHandler.ViewAll();

            if (listOfTasks.Count == 0)
            {
                throw new ApplicationException("Nothing to search, add some tasks first");
            }

            List<Task> tasksContainingSearchString = new List<Task>();

            for (int i = 0; i < listOfTasks.Count; i++)
            {
                if (listOfTasks[i].Description.Trim().ToLower().Contains(searchString.Trim().ToLower()))
                {
                    tasksContainingSearchString.Add(listOfTasks[i]);
                }
            }

            if (tasksContainingSearchString.Count == 0)
            {
                throw new ApplicationException("No match found!");
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
