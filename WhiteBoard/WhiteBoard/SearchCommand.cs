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
        const int NEAR_MISS_MIN_SEARCH_COUNT = 1;
        const int NEAR_MISS_MAXIMUM_EDIT_DISTANCE = 2;

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

            if (directResults.Count < NEAR_MISS_MIN_SEARCH_COUNT)
            {
                resultSet.Clear();

                resultSet.AddRange(getNearMiss(listOfTasks));
                searchResults.Add(SearchResultType.NearMiss, resultSet.Except(directResults).ToList());
            }

            if (searchResults.Count == 0)
            {
                throw new ApplicationException("No match found!");
            }

            return resultSet.Distinct().ToList();
        }

        private List<Task> getDirectHit(List<Task> tasks)
        {
            List<Task> directHitTasks = new List<Task>();

            foreach (Task task in tasks)
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
            Dictionary<Task, int> nearMissTasks = new Dictionary<Task, int>();

            foreach (Task task in tasks)
            {
                int editDistance = ComputeEditDistance(searchString.Trim().ToLower(), task.Description.Trim().ToLower());

                if (editDistance <= NEAR_MISS_MAXIMUM_EDIT_DISTANCE)
                {
                    nearMissTasks.Add(task, editDistance);
                }
            }

            // sort by best match
            List<Task> sortedNearMissTasks = (from entry in nearMissTasks orderby entry.Value ascending select entry.Key).ToList();

            return sortedNearMissTasks;
        }

        private int ComputeEditDistance(string query, string taskDescription)
        {
            int queryLength = query.Length;
            int taskDescriptionLength = taskDescription.Length;
            int[,] editDistance = new int[queryLength + 1, taskDescriptionLength + 1];

            if (queryLength == 0)
            {
                return taskDescriptionLength;
            }

            if (taskDescriptionLength == 0)
            {
                return queryLength;
            }

            for (int i = 0; i <= queryLength; editDistance[i, 0] = i++)
            {
            }

            for (int j = 0; j <= taskDescriptionLength; editDistance[0, j] = j++)
            {
            }

            for (int i = 1; i <= queryLength; i++)
            {
                for (int j = 1; j <= taskDescriptionLength; j++)
                {
                    int cost = (taskDescription[j - 1] == query[i - 1]) ? 0 : 1;

                    editDistance[i, j] = Math.Min(
                        Math.Min(editDistance[i - 1, j] + 1, editDistance[i, j - 1] + 1),
                        editDistance[i - 1, j - 1] + cost);
                }
            }

            return editDistance[queryLength, taskDescriptionLength];
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
