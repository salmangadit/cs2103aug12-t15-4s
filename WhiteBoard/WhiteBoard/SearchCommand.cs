using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    enum SearchResultType
    {
        Direct,
        NearMiss
    }

    class SearchCommand : Command
    {
        string searchString;
        const int NEAR_MISS_END_LENGTH = 2;
        const int NEAR_MISS_START_LENGTH = 1;
        const int NEAR_MISS_START_INDEX = 0;

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

            List<Task> resultSet = new List<Task>();

            // to be decided V0.5 whether to use Dictionary as return, so it looks nice on GUI
            Dictionary<SearchResultType, List<Task>> searchResults = new Dictionary<SearchResultType, List<Task>>();

            resultSet.AddRange(getDirectHit(listOfTasks));
            searchResults.Add(SearchResultType.Direct, resultSet);

            List<Task> directResults = resultSet.Distinct().ToList();
            resultSet.Clear();

            resultSet.AddRange(getNearMiss(listOfTasks));
            searchResults.Add(SearchResultType.NearMiss, resultSet.Except(directResults).ToList());

            if (searchResults.Count == 0)
            {
                throw new ApplicationException("No match found!");
            }

            return resultSet.Distinct().ToList();
        }

        private List<Task> getDirectHit(List<Task> tasks)
        {
            List<Task> directHitTasks = new List<Task>();

            foreach(Task task in tasks)
            {
                if (task.Description.Trim().ToLower().Contains(searchString.Trim().ToLower()))
                {
                    directHitTasks.Add(task);
                }
            }

            return directHitTasks;
        }

        private List<Task> getNearMiss(List<Task> tasks)
        {
            List<Task> nearMissTasks = new List<Task>();

            List<string> searchQueries = getNearMissQueries();

            foreach (string searchQuery in searchQueries)
            {
                foreach (Task task in tasks)
                {
                    if (task.Description.Trim().ToLower().Contains(searchQuery.Trim().ToLower()))
                    {
                        nearMissTasks.Add(task);
                    }
                }
            }

            return nearMissTasks;
        }

        private List<string> getNearMissQueries()
        {
            List<string> nearMissQueries = new List<string>();

            for (int i = searchString.Length - NEAR_MISS_START_LENGTH; i >= searchString.Length - NEAR_MISS_END_LENGTH; i--)
            {
                nearMissQueries.Add(searchString.Substring(NEAR_MISS_START_INDEX, i));
            }

            return nearMissQueries;
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
