using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    //@author U095146E
    class ViewCommand : Command
    {
        #region Private Fields
        Task viewTaskDetails;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for ViewCommand Object
        /// </summary>
        /// <param name="fileHandler">Reference to fileHandler Singleton </param>
        /// <param name="viewTaskDetails">Task object with parameters for current view command </param>
        /// <param name="screenState">Reference to screenState from UI</param>
        public ViewCommand(FileHandler fileHandler, Task viewTaskDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            this.viewTaskDetails = viewTaskDetails;
            this.commandType = CommandType.View;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns type of Command - CommandType.View
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
        /// Executes View Command by checking required criteria
        /// </summary>
        /// <returns>List of Tasks that match view criteria</returns>
        public override List<Task> Execute()
        {
            Debug.Assert(viewTaskDetails != null, "Task Details not set");

            List<Task> tasksToView = new List<Task>();

            if (viewTaskDetails.Archive == true)
            {
                Log.Debug("View archived tasks");
                tasksToView = fileHandler.ViewArchive();
            }
            else if (viewTaskDetails.Archive == false && viewTaskDetails.StartTime == null && viewTaskDetails.EndTime == null)
            {
                Log.Debug("View all tasks");
                tasksToView = fileHandler.ViewAll();
            }
            else if (viewTaskDetails.Archive == false && viewTaskDetails.EndTime == null)
            {
                Log.Debug("View tasks ending on" + viewTaskDetails.StartTime);
                tasksToView = fileHandler.ViewTasks(viewTaskDetails.StartTime);
            }
            else if (viewTaskDetails.Archive == false)
            {
                Log.Debug(String.Format("View tasks starting from {0} and ending on {1}", viewTaskDetails.StartTime, viewTaskDetails.EndTime));
                tasksToView = fileHandler.ViewTasks(viewTaskDetails.StartTime, viewTaskDetails.EndTime);
            }
            else
            {
                Log.Debug("Invalid condition for Viewing Tasks");
                throw new NotImplementedException(Constants.INVALID_VIEW);
            }

            if (tasksToView.Count == 0)
            {
                throw new ApplicationException(Constants.EMPTY_VIEW);
            }

            return tasksToView;
        }

        /// <summary>
        /// Performs undo operation for view command by restoring previous screen state
        /// </summary>
        /// <returns>Screen state as a list of tasks</returns>
        public override List<Task> Undo()
        {
            return screenState;
        }
        #endregion
    }
}
