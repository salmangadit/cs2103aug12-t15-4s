using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WhiteBoard
{
    class AutoCompletor
    {
        private List<string> lineSet = new List<string>();
        private List<string> wordSet = new List<string>();

        public AutoCompletor()
        {
            GenerateLineSet(FileHandler.Instance.ViewAll());
            GenerateWordSet();
            FileHandler.Instance.FileUpdateEvent += new FileUpdate(Update);
        }

        public List<string> Query(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("Query cannot be null");
            }

            if (query.Length < 1)
            {
                return null;
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
                    throw new NotSupportedException("No such update type");
            }
        }

        private void GenerateLineSet(List<Task> tasks)
        {
            foreach (Task task in tasks)
            {
                lineSet.Add(task.Description);
            }
        }

        private void GenerateWordSet()
        {
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
            // sorts the array and returns a copy
            var sorted = from s in e
                         orderby s.Length ascending
                         select s;

            return sorted;
        }

        private void AddToSets(Task task)
        {
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
