using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    internal class Constants
    {
        //@author U096089W
        public static const int TASK_OVERDUE_START_X = 0;
        public static const int TASK_OVERDUE_START_Y = 0;
        public static const int TASK_OVERDUE_END_X = 0;
        public static const int TASK_OVERDUE_END_Y = 1;
        public static const double TASK_OVERDUE_GRADIENT_WHITE = 0.0;
        public static const int TASK_OVERDUE_GRADIENT_LIGHT_GRAY = 1;

        public static const int TASK_NORMAL_START_X = 0;
        public static const int TASK_NORMAL_START_Y = 0;
        public static const int TASK_NORMAL_END_X = 0;
        public static const int TASK_NORMAL_END_Y = 2;
        public static const double TASK_NORMAL_GRADIENT_WHITE = 0.0;
        public static const int TASK_NORMAL_GRADIENT_LIGHT_GRAY = 2;

        public static const string SEARCH_NO_KEYWORD = "Enter a keyword to search for";
        public static const string SEARCH_NO_TASKS = "Nothing to search, add some tasks first";
        public static const string SEARCH_NO_MATCH = "No match found!";

        public static const string INSTANT_SEARCH_DEBUG_NULL = "Query string is null";

        public static const string FILEHANDLER_UPDATE_DEBUG_NULL = "Task to update was null";
        public static const string FILEHANDLER_UPDATE_DEBUG_UNEDITED_NULL = "Unedited Task was null";
        public static const string FILEHANDLER_UPDATE_LOG_UPDATE_TRIGGER = "FileHandler triggered Update event {0} for task {1}";
        public static const string FILEHANDLER_NO_SUCH_UPDATE_TYPE = "No such update type";

        public static const string LOG_DESCRIPTION_SET = "Generating description set";
        public static const string LOG_LINE_SET = "Generating line set";
        public static const string LOG_WORD_SET = "Generating word set";
        public static const string LOG_ADD_TO_SET = "Adding Task with description {0} and Id {1} to sets";
        public static const string LOG_SORT_SET = "Sorting set";
        public static const string LOG_REMOVE_FROM_SET = "Removing Task with id {0} from sets";
        public static const string LOG_SYSTEM_EXCEPTION = "Caught a System Exception";

        public static const string DEBUG_INVALID_TASK_ID = "Task ID is invalid";
        public static const string DEBUG__QUERY_STRING_NULL = "Query string is null";

        public static const string TASK_NO_START_GOT_END = "Task can't have an end time without start time";
        public static const string TASK_START_IN_PAST = "Task cannot start in the past";
        public static const string TASK_BEGIN_AFTER_END = "Task cannot begin after it ends!";
        public static const string TASK_NO_DESCRIPTION = "Please provide a task description";

        public static const string EDIT_COMMAND_UNABLE = "Unable To Edit Task with ID T";
        public static const string EDIT_COMMAND_LOG_EXECUTED = "Edit Command was executed for";
        public static const string EDIT_COMMAND_LOG_FAILED = "Edit Command failed for";
        public static const string EDIT_COMMAND_UNDO_LOG_EXECUTED = "Edit Command Undo was executed for";
        public static const string EDIT_COMMAND_UNDO_LOG_FAILED = "Edit Command Undo failed for";
        public static const string EDIT_COMMAND_UNDO_UNABLE = "Unable To Undo Edit Command for Task";

        public static const string ADD_COMMAND_NO_TASKS = "No tasks to add";
        public static const string ADD_COMMAND_UNABLE = "Unable To Add Task with ID T";
        public static const string ADD_COMMAND_LOG_EXECUTED = "Add Command was executed for";
        public static const string ADD_COMMAND_LOG_FAILED = "Add Command failed for";
        public static const string ADD_COMMAND_UNDO_LOG_EXECUTED = "Add Command Undo was executed for";
        public static const string ADD_COMMAND_UNDO_LOG_FAILED = "Add Command Undo failed for";
        public static const string ADD_COMMAND_UNDO_UNABLE = "Unable To Undo Add Command for Task";

        public static const string DELETE_COMMAND_NO_TASKS = "No tasks to delete";
        public static const string DELETE_COMMAND_UNABLE = "Unable To Delete Task with ID T";
        public static const string DELETE_COMMAND_LOG_EXECUTED = "Delete Command was executed for";
        public static const string DELETE_COMMAND_LOG_FAILED = "Delete Command failed for";
        public static const string DELETE_COMMAND_UNDO_LOG_EXECUTED = "Delete Command Undo was executed for";

        public static const string ARCHIVE_COMMAND_NO_TASKS = "No tasks to archive";
        public static const string ARCHIVE_COMMAND_UNABLE = "Unable To Archive Task with ID T";
        public static const string ARCHIVE_COMMAND_LOG_EXECUTED = "Archive Command was executed for";
        public static const string ARCHIVE_COMMAND_LOG_FAILED = "Archive Command failed for";
        public static const string ARCHIVE_COMMAND_UNDO_LOG_EXECUTED = "Archive Command Undo was executed for";
        public static const string ARCHIVE_COMMAND_UNDO_LOG_FAILED = "Archive Command Undo failed for";
        public static const string ARCHIVE_COMMAND_UNDO_UNABLE = "Unable To Undo Archive Command for Task";


    }
}
