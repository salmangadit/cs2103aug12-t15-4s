﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace WhiteBoard
{
    //@author U096089W
    /// <summary>
    /// Search for tasks within file
    /// Implements near miss for powerful search
    /// </summary>
    class SearchCommand : Command
    {
        #region Private Fields
        private string searchString;
        const int NEAR_MISS_MIN_SEARCH_COUNT = 1;
        const int NEAR_MISS_MAXIMUM_EDIT_DISTANCE = 2;
        #endregion

        #region Constructors
        public SearchCommand(FileHandler fileHandler, string searchString, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.searchString = searchString;
            this.commandType = CommandType.Search;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the type of Command
        /// </summary>
        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Executes the Search for the given query
        /// </summary>
        /// <returns>The matching Task(s)</returns>
        public override List<Task> Execute()
        {
            if (String.IsNullOrWhiteSpace(searchString))
            {
                throw new ApplicationException(Constants.SEARCH_NO_KEYWORD);
            }

            List<Task> listOfTasks = fileHandler.ViewAll();

            if (listOfTasks.Count == 0)
            {
                throw new ApplicationException(Constants.SEARCH_NO_TASKS);
            }

            List<Task> resultSet = new List<Task>();

            resultSet.AddRange(getDirectHit(listOfTasks));

            if (resultSet.Count < NEAR_MISS_MIN_SEARCH_COUNT)
            {
                resultSet.AddRange(getNearMiss(listOfTasks));
            }

            return resultSet.Distinct().ToList();
        }
        //@author U095146E
        /// <summary>
        /// Restores screen state before search was executed
        /// </summary>
        /// <returns>The screen state before archiving the Task(s)</returns>
        public override List<Task> Undo()
        {
            return screenState;
        }

        public string GetSearchString()
        {
            return searchString;
        }
        #endregion

        //@author U096089W
        #region Private Class Helper Methods
        /// <summary>
        /// Gets Tasks which match the search query exactly
        /// </summary>
        /// <param name="tasks">Tasks to search for</param>
        /// <returns>Matching Tasks for given search query</returns>
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

        /// <summary>
        /// Gets Tasks which "almost" match the search query
        /// </summary>
        /// <param name="tasks">Tasks to search for</param>
        /// <returns>"Almost" Matching Tasks for given search query</returns>
        private List<Task> getNearMiss(List<Task> tasks)
        {
            Dictionary<Task, int> nearMissTasks = new Dictionary<Task, int>();

            foreach (Task task in tasks)
            {
                int editDistance = ComputeEditDistance(searchString.Trim().ToLower(), task.Description.Trim().ToLower());

                // add task which descriptions within threshold
                if (editDistance <= NEAR_MISS_MAXIMUM_EDIT_DISTANCE)
                {
                    nearMissTasks.Add(task, editDistance);
                    continue;
                }

                // if task doesn't fit in, check for words within that might

                // replace one or more whitespace in description with single whitespace
                string processedLine = Regex.Replace(task.Description, @"\s+", " ");

                string[] wordsInDescription = processedLine.Split(' ');

                foreach (string word in wordsInDescription)
                {
                    editDistance = ComputeEditDistance(searchString.Trim().ToLower(), word.Trim().ToLower());

                    if (editDistance <= NEAR_MISS_MAXIMUM_EDIT_DISTANCE)
                    {
                        nearMissTasks.Add(task, editDistance);
                        break;
                    }
                }
            }

            // sort by best match
            List<Task> sortedNearMissTasks = (from entry in nearMissTasks orderby entry.Value ascending select entry.Key).ToList();

            return sortedNearMissTasks;
        }

        /// <summary>
        /// Computes the Edit Distance for a given pair of strings
        /// </summary>
        /// <param name="source">source string</param>
        /// <param name="target">target string to convert to</param>
        /// <returns>the edit distance</returns>
        private int ComputeEditDistance(string source, string target)
        {
            int queryLength = source.Length;
            int taskDescriptionLength = target.Length;
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
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    editDistance[i, j] = Math.Min(
                        Math.Min(editDistance[i - 1, j] + 1, editDistance[i, j - 1] + 1),
                        editDistance[i - 1, j - 1] + cost);
                }
            }

            return editDistance[queryLength, taskDescriptionLength];
        }
        #endregion
    }
}
