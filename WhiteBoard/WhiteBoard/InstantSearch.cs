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
    /// <summary>
    /// Returns Instant Search results to be displayed
    /// </summary>
    class InstantSearch
    {
        #region Private Fields
        private List<Task> tasks;
        private Dictionary<int, string> descriptionSet;
        private static readonly ILog Log = LogManager.GetLogger(typeof(InstantSearch));
        #endregion

        #region Constructors
        public InstantSearch()
        {
            tasks = FileHandler.Instance.ViewAll();
            descriptionSet = new Dictionary<int, string>();
            GenerateDescriptionSet(tasks);
            FileHandler.Instance.FileUpdateEvent += new FileUpdate(Update);
        }
        #endregion

        #region Public Class Methods
        public List<Task> GetTasksWithDescription(string searchDescription)
        {
            Debug.Assert(searchDescription != null, Constants.INSTANT_SEARCH_DEBUG_NULL);

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
        #endregion

        #region Event Handlers
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
        #endregion

        #region Private Class Helper Methods
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

        private void GenerateDescriptionSet(List<Task> tasks)
        {
            Log.Debug(Constants.LOG_DESCRIPTION_SET);

            foreach (Task task in tasks)
            {
                descriptionSet.Add(task.Id, task.Description);
            }
        }

        private void AddToSets(Task task)
        {
            Log.Debug(String.Format(Constants.LOG_ADD_TO_SET, task.Description, task.Id));

            tasks.Add(task);
            descriptionSet.Add(task.Id, task.Description);
        }

        private void RemoveFromSets(Task task)
        {
            Log.Debug(String.Format(Constants.LOG_REMOVE_FROM_SET, task.Id));

            tasks.Remove(task);
            descriptionSet.Remove(task.Id);
        }
        #endregion
    }

}
