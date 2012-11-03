using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using log4net;

namespace WhiteBoard
{
    class InstantSearch
    {
        private List<Task> tasks;
        private Dictionary<int, string> descriptionSet;
        protected static readonly ILog Log = LogManager.GetLogger(typeof(InstantSearch));

        public InstantSearch()
        {
            tasks = FileHandler.Instance.ViewAll();
            descriptionSet = new Dictionary<int, string>();
            GenerateDescriptionSet(tasks);
            FileHandler.Instance.FileUpdateEvent += new FileUpdate(Update);
        }

        public List<Task> GetTasksWithDescription(string searchDescription)
        {
            Debug.Assert(searchDescription != null, "Query string is null");

            if (searchDescription.Length < 1)
            {
                return new List<Task>();
            }

            List<Task> resultSet = new List<Task>();

            foreach (KeyValuePair<int, string> entry in descriptionSet)
            {
                if (entry.Value.Trim().ToLower().Contains(searchDescription.Trim().ToLower()))
                {
                    Task task = getTaskWithId(entry.Key);
                    resultSet.Add(task);
                }
            }

            return resultSet;
        }

        private Task getTaskWithId(int id)
        {
            foreach (Task task in tasks)
            {
                if (task.Id == id)
                {
                    return task;
                }
            }

            return null;
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

        private void GenerateDescriptionSet(List<Task> tasks)
        {
            Log.Debug("Generating description set");

            foreach (Task task in tasks)
            {
                descriptionSet.Add(task.Id, task.Description);
            }
        }

        private void AddToSets(Task task)
        {
            Log.Debug(String.Format("Adding Task with description {0} and Id {1} to sets", task.Description, task.Id));

            tasks.Add(task);
            descriptionSet.Add(task.Id, task.Description);
        }

        private void RemoveFromSets(Task task)
        {
            Log.Debug(String.Format("Removing Task with id {0} from sets", task.Id));

            tasks.Remove(task);
            descriptionSet.Remove(task.Id);
        }

    }

}
