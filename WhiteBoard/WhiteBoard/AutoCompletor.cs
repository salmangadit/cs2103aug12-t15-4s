using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using log4net;

namespace WhiteBoard
{
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

        // for testing purposes only
        public AutoCompletor(List<Task> tasks)
        {
            GenerateQuerySet(tasks);
        }

        public List<string> Query(string query)
        {
            Debug.Assert(query != null, "Query string is null");

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
            Debug.Assert(task != null, "Task to update was null");

            Debug.Assert(!(update == UpdateType.Edit && uneditedTask == null), "Unedited Task was null");

            Log.Debug(String.Format("FileHandler triggered Update event {0} for task {1}", update, task.Id));

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
                    throw new NotImplementedException("No such update type");
            }
        }

        private void GenerateQuerySet(List<Task> tasks)
        {
            Log.Debug("Generating line set");
            foreach (Task task in tasks)
            {
                lineSet.Add(task.Description);
            }

            Log.Debug("Generating word set");
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
            Log.Debug("Sorting set");

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
            Log.Debug(String.Format("Removing Task with description {0} from sets", task.Description));

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
