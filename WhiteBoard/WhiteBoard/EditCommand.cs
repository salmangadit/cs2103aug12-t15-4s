using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WhiteBoard
{
    //@author U096089W
    /// <summary>
    /// Modifies the given Task in file
    /// </summary>
    class EditCommand : Command
    {
        List<Task> editTasksDetails;
        List<Task> uneditedTasks;

        public EditCommand(FileHandler fileHandler, Task editTaskDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            editTasksDetails = new List<Task>();
            uneditedTasks = new List<Task>();
            this.editTasksDetails.Add(editTaskDetails);
            this.commandType = CommandType.Edit;
        }

        public EditCommand(FileHandler fileHandler, List<Task> editTasksDetails, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            editTasksDetails = new List<Task>();
            uneditedTasks = new List<Task>();
            this.editTasksDetails = editTasksDetails;
            this.commandType = CommandType.Edit;
        }

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

        /// <summary>
        /// Modifies the Task(s) with given Id to have new data
        /// </summary>
        /// <returns>The Edited Task(s)</returns>
        public override List<Task> Execute()
        {
            List<Task> editedTasks = new List<Task>();

            foreach (Task editTaskDetails in editTasksDetails)
            {
                Debug.Assert(editTaskDetails.Id > 0, Constants.DEBUG_INVALID_TASK_ID);

                int editedTaskId = editTaskDetails.Id;
                Task uneditedTask = fileHandler.GetTaskFromFile(editedTaskId);

                uneditedTasks.Add(uneditedTask);

                string editedTaskDescription = (editTaskDetails.Description == null) ? uneditedTask.Description : editTaskDetails.Description;
                DateTime? editedTaskStartTime = (editTaskDetails.StartTime == null) ? uneditedTask.StartTime : editTaskDetails.StartTime;
                DateTime? editedTaskEndTime = (editTaskDetails.EndTime == null) ? uneditedTask.EndTime : editTaskDetails.EndTime;

                Task editedTask = new Task(editedTaskId, editedTaskDescription, editedTaskStartTime, editedTaskEndTime);

                if (editedTask.StartTime == null)
                {
                    if (editedTask.EndTime != null)
                    {
                        throw new ApplicationException(Constants.TASK_NO_START_GOT_END);
                    }
                }

                if (!isFloatingTask(editedTask) && (editedTaskStartTime != uneditedTask.StartTime && (((DateTime)editedTask.StartTime).Date < DateTime.Now.Date)))
                {
                    throw new ApplicationException(Constants.TASK_START_IN_PAST);
                }

                if (!isFloatingTask(editedTask) && (editedTask.StartTime > editedTask.EndTime))
                {
                    throw new ApplicationException(Constants.TASK_BEGIN_AFTER_END);
                }

                if (editedTask.Description.Equals(String.Empty))
                {
                    throw new ApplicationException(Constants.TASK_NO_DESCRIPTION);
                }

                bool isTaskWrittern = fileHandler.WriteEditedTaskToFile(editedTask);

                if (isTaskWrittern)
                {
                    Log.Debug(Constants.EDIT_COMMAND_LOG_EXECUTED + editedTaskId);
                }
                else
                {
                    Log.Debug(Constants.EDIT_COMMAND_LOG_FAILED + editedTaskId);
                    throw new ApplicationException(Constants.EDIT_COMMAND_UNABLE + editedTaskId);
                }

                editedTasks.Add(editedTask);
            }

            return editedTasks;
        }

        /// <summary>
        /// Checks if a given Task is a floating task
        /// </summary>
        /// <param name="taskToAdd">The Task that is being added</param>
        /// <returns>True if task is floating, false if not</returns>
        private bool isFloatingTask(Task taskToAdd)
        {
            if ((taskToAdd.StartTime == null || taskToAdd.EndTime == null) && !(taskToAdd.StartTime == null && taskToAdd.EndTime != null))
            {
                return true;
            }

            return false;
        }

        //@author U095146E
        /// <summary>
        /// Restores the edited Task(s)
        /// </summary>
        /// <returns>The screen state before archiving the Task(s)</returns>
        public override List<Task> Undo()
        {
            foreach (Task uneditedTask in uneditedTasks)
            {
                Debug.Assert(uneditedTask.Id > 0, Constants.DEBUG_INVALID_TASK_ID);

                bool isTaskWrittern = fileHandler.WriteEditedTaskToFile(uneditedTask);

                if (isTaskWrittern)
                {
                    Log.Debug(Constants.EDIT_COMMAND_UNDO_LOG_EXECUTED + uneditedTask.Id);
                }
                else
                {
                    Log.Debug(Constants.EDIT_COMMAND_UNDO_LOG_FAILED + uneditedTask.Id);
                    throw new SystemException(Constants.EDIT_COMMAND_UNDO_UNABLE + uneditedTask.Id);
                }
            }

            return screenState;
        }
    }
}
