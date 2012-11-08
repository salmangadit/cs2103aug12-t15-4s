using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using log4net;

namespace WhiteBoard
{
    //@author U096089W
    class AutoCompletor
    {
        private List<string> lineSet = new List<string>();
        private List<string> wordSet = new List<string>();
        protected static readonly ILog Log = LogManager.GetLogger(typeof(AutoCompletor));

        public AutoCompletor()
        {
            GenerateQuerySet(FileHandler.Instance.ViewAll());
            FileHandler.Instance.FileUpdateEvent += new FileUpdate(Update);
        }

        // for unit testing purposes only
        public AutoCompletor(List<Task> tasks)
        {
            GenerateQuerySet(tasks);
        }

        public List<string> Query(string query)
        {
            Debug.Assert(query != null, Constants.DEBUG__QUERY_STRING_NULL);

            if (query.Length < 1)
            {
                return new List<string>();
            }

            HashSet<string> resultSet = new HashSet<string>();

            foreach (string word in SortByLength(wordSet))
            {
                if (word.ToLower().StartsWith(query.TrimStart().ToLower()))
                {
                    resultSet.Add(word.ToLower());
                }
            }

            foreach (string line in SortByLength(lineSet))
            {
                if (line.Trim().ToLower().Contains(query.Trim().ToLower()))
                {
                    resultSet.Add(line);
                }
            }

            return resultSet.ToList<string>();
        }

        private void Update(UpdateType update, Task task, Task uneditedTask)
        {
            Debug.Assert(task != null, Constants.FILEHANDLER_UPDATE_DEBUG_NULL);

            Debug.Assert(!(update == UpdateType.Edit && uneditedTask == null), Constants.FILEHANDLER_UPDATE_DEBUG_UNEDITED_NULL);

            Log.Debug(String.Format(Constants.FILEHANDLER_UPDATE_LOG_UPDATE_TRIGGER, update, task.Id));

            switch (update)
            {
                case UpdateType.Add:
                    AddToSets(task);
                    break;

                case UpdateType.Archive:
                    RemoveFromSets(task);
                    break;

                case UpdateType.Delete:
                    RemoveFromSets(task);
                    break;

                case UpdateType.Edit:
                    RemoveFromSets(uneditedTask);
                    AddToSets(task);
                    break;

                case UpdateType.Unarchive:
                    AddToSets(task);
                    break;

                default:
                    throw new NotImplementedException(Constants.FILEHANDLER_NO_SUCH_UPDATE_TYPE);
            }
        }

        private void GenerateQuerySet(List<Task> tasks)
        {
            Log.Debug(Constants.LOG_LINE_SET);
            foreach (Task task in tasks)
            {
                lineSet.Add(task.Description);
            }

            Log.Debug(Constants.LOG_WORD_SET);
            foreach (string line in lineSet)
            {
                // replace one or more whitespace in description with single whitespace
                string processedLine = Regex.Replace(line, @"\s+", " ");

                string[] wordsInDescription = processedLine.Split(' ');

                foreach (string word in wordsInDescription)
                {
                    wordSet.Add(word);
                }
            }
        }

        // sorting for better visual appeal and branch prediction
        private static IEnumerable<string> SortByLength(IEnumerable<string> e)
        {
            Log.Debug(Constants.LOG_SORT_SET);

            // sorts the array and returns a copy
            var sorted = from s in e
                         orderby s.Length ascending
                         select s;

            return sorted;
        }

        private void AddToSets(Task task)
        {
            Log.Debug(String.Format("Adding Task with description {0} to sets", task.Description));

            lineSet.Add(task.Description);

            // replace one or more whitespace in description with single whitespace
            string processedLine = Regex.Replace(task.Description, @"\s+", " ");

            string[] wordsInDescription = processedLine.Split(' ');

            foreach (string word in wordsInDescription)
            {
                wordSet.Add(word);
            }
        }

        private void RemoveFromSets(Task task)
        {
            Log.Debug(String.Format(Constants.LOG_ADD_TO_SET, task.Description));

            lineSet.Remove(task.Description);

            // replace one or more whitespace in description with single whitespace
            string processedLine = Regex.Replace(task.Description, @"\s+", " ");

            string[] wordsInDescription = processedLine.Split(' ');

            foreach (string word in wordsInDescription)
            {
                wordSet.Remove(word);
            }
        }

    }

}
