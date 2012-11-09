using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{

    internal class Constants
    {
        #region Constant strings
        //@author U095146E
        public static string INVALID_VIEW = "There is no such criteria for file viewing!";
        public static string EMPTY_VIEW = "There are no tasks to view";
        public static string INVALID_UNDO = "An undo for an undo is not supported!";
        public static string INVALID_DATE_FORMAT = "Invalid date entered. Please re-enter command";
        public static string MESSAGE_COMMAND_UNDO = "Command type undone: ";
        public static string MESSAGE_COMMAND_SEARCH = "Search results for: ";
        public static string MESSAGE_COMMAND_ARCHIVE = "Archived task(s) with Id: ";
        public static string MESSAGE_COMMAND_DELETE = "Deleted task with Id: ";
        public static string MESSAGE_COMMAND_EDIT = "Task Edited with Id: ";
        public static string MESSAGE_COMMAND_ADD = "Task Added!";
        public static string USER_COMMAND_VIEW_ALL = "VIEW ALL";
        public static string USER_COMMAND_UNDO = "UNDO";

        //@author U096089W
        public static string SEARCH_NO_KEYWORD = "Enter a keyword to search for";
        public static string SEARCH_NO_TASKS = "Nothing to search, add some tasks first";
        public static string SEARCH_NO_MATCH = "No match found!";
        public static string INSTANT_SEARCH_DEBUG_NULL = "Query string is null";
        public static string FILEHANDLER_UPDATE_DEBUG_NULL = "Task to update was null";
        public static string FILEHANDLER_UPDATE_DEBUG_UNEDITED_NULL = "Unedited Task was null";
        public static string FILEHANDLER_UPDATE_LOG_UPDATE_TRIGGER = "FileHandler triggered Update event {0} for task {1}";
        public static string FILEHANDLER_NO_SUCH_UPDATE_TYPE = "No such update type";
        public static string LOG_DESCRIPTION_SET = "Generating description set";
        public static string LOG_LINE_SET = "Generating line set";
        public static string LOG_WORD_SET = "Generating word set";
        public static string LOG_ADD_TO_SET = "Adding Task with description {0} and Id {1} to sets";
        public static string LOG_SORT_SET = "Sorting set";
        public static string LOG_REMOVE_FROM_SET = "Removing Task with description {0} and id {1} from sets";
        public static string LOG_SYSTEM_EXCEPTION = "Caught a System Exception";
        public static string DEBUG_INVALID_TASK_ID = "Task ID is invalid";
        public static string DEBUG__QUERY_STRING_NULL = "Query string is null";
        public static string TASK_NO_START_GOT_END = "Task can't have an end time without start time";
        public static string TASK_START_IN_PAST = "Task cannot start in the past";
        public static string TASK_BEGIN_AFTER_END = "Task cannot begin after it ends!";
        public static string TASK_NO_DESCRIPTION = "Please provide a task description";
        public static string EDIT_COMMAND_UNABLE = "Unable To Edit Task with ID T";
        public static string EDIT_COMMAND_LOG_EXECUTED = "Edit Command was executed for";
        public static string EDIT_COMMAND_LOG_FAILED = "Edit Command failed for";
        public static string EDIT_COMMAND_UNDO_LOG_EXECUTED = "Edit Command Undo was executed for";
        public static string EDIT_COMMAND_UNDO_LOG_FAILED = "Edit Command Undo failed for";
        public static string EDIT_COMMAND_UNDO_UNABLE = "Unable To Undo Edit Command for Task";
        public static string ADD_COMMAND_NO_TASKS = "No tasks to add";
        public static string ADD_COMMAND_UNABLE = "Unable To Add Task with ID T";
        public static string ADD_COMMAND_LOG_EXECUTED = "Add Command was executed for";
        public static string ADD_COMMAND_LOG_FAILED = "Add Command failed for";
        public static string ADD_COMMAND_UNDO_LOG_EXECUTED = "Add Command Undo was executed for";
        public static string ADD_COMMAND_UNDO_LOG_FAILED = "Add Command Undo failed for";
        public static string ADD_COMMAND_UNDO_UNABLE = "Unable To Undo Add Command for Task";
        public static string DELETE_COMMAND_NO_TASKS = "No tasks to delete";
        public static string DELETE_COMMAND_UNABLE = "Unable To Delete Task with ID T";
        public static string DELETE_COMMAND_LOG_EXECUTED = "Delete Command was executed for";
        public static string DELETE_COMMAND_LOG_FAILED = "Delete Command failed for";
        public static string DELETE_COMMAND_UNDO_LOG_EXECUTED = "Delete Command Undo was executed for";
        public static string ARCHIVE_COMMAND_NO_TASKS = "No tasks to archive";
        public static string ARCHIVE_COMMAND_UNABLE = "Unable To Archive Task with ID T";
        public static string ARCHIVE_COMMAND_LOG_EXECUTED = "Archive Command was executed for";
        public static string ARCHIVE_COMMAND_LOG_FAILED = "Archive Command failed for";
        public static string ARCHIVE_COMMAND_UNDO_LOG_EXECUTED = "Archive Command Undo was executed for";
        public static string ARCHIVE_COMMAND_UNDO_LOG_FAILED = "Archive Command Undo failed for";
        public static string ARCHIVE_COMMAND_UNDO_UNABLE = "Unable To Undo Archive Command for Task";
        public static string UNARCHIVE_COMMAND_NO_TASKS = "No tasks to unarchive";
        public static string UNARCHIVE_COMMAND_UNABLE = "Unable To UnArchive Task with ID T";
        public static string UNARCHIVE_COMMAND_LOG_EXECUTED = "UnArchive Command was executed for";
        public static string UNARCHIVE_COMMAND_LOG_FAILED = "UnArchive Command failed for";
        public static string UNARCHIVE_COMMAND_UNDO_LOG_EXECUTED = "UnArchive Command Undo was executed for";
        public static string UNARCHIVE_COMMAND_UNDO_LOG_FAILED = "UnArchive Command Undo failed for";
        public static string UNARCHIVE_COMMAND_UNDO_UNABLE = "Unable To Undo UnArchive Command for Task";

        //@author U094776M
        public static string FILENAME = "TasksList.xml";
        public static string UNABLE_TO_SERIALIZE = "Unable to serialize null values";
        public static string ERROR_GENERATING_DOC = "There was an error generating the XML document";
        public static string ERROR_FILE_EMPTY = "File is empty";

        //@author U095159L
        public static string[] DAYS_OF_WEEK = { "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "TODAY", "TOMORROW" };
        public static string[] COMMAND_DATE = { "BY", "ON", "BEFORE", "AT", "FROM", "BETWEEN" };
        public static string[] COMMAND_MODIFY = { "MODIFY", "UPDATE" };
        public static string[] COMMAND_NEW_DATE = { "START", "END" };
        public static string[] COMMAND_VIEW_DAY = { "ON", "AT" };
        public static string[] COMMAND_VIEW_RANGE = { "ON", "FROM", "BETWEEN" };
        public static string[] COMMAND_VIEW_ENDING = { "BY", "BEFORE", "ENDING" };
        public static string[] COMMAND_KEYWORD_REMOVE = { "BY", "ON", "BEFORE", "AT", "FROM", "BETWEEN", "START", "END" };

        public static string[] DATE_FORMATS = { "dd/mm/yyyy", "dd-mm-yyyy", "dd.mm.yyyy","dd/mm/yy","dd-mm-yy","dd.mm.yy", "d/m/yyyy","dd/m/yyyy","d/mm/yyyy", "d/m/yy","dd/m/yy",
                                                "d/mm/yy", "d.m.yyyy","dd.m.yyyy","d.mm.yyyy","d.m.yy","dd.m.yy","d.mm.yy","d-m-yyyy","dd-m-yyyy","d-mm-yyyy","d-m-yy","dd-m-yy","d-mm-yy"};

        public static string[] TIME_FORMATS = { "hh.mm", "h.mm", "h.mm tt","hh.mm tt","h.mmtt","hh.mmtt","hh:mm", "h:mm", "h:mm tt","hh:mm tt",
                                                "h:mmtt","hh:mmtt","hhmmtt","hmmtt","hhmm tt","hmm tt","hhmm","hmm", "htt","hhtt", "h tt","hh tt","HH:mm"};

        public const string COMMAND_PM = "PM";
        public const string COMMAND_AM = "AM";
        public const string COMMAND_CASE_SEARCH = "SEARCH:";
        public const string COMMAND_CASE_MODIFY = "MODIFY";
        public const string COMMAND_CASE_UPDATE = "UPDATE";
        public const string COMMAND_CASE_UNDO = "UNDO";
        public const string COMMAND_CASE_MARK = "MARK";
        public const string COMMAND_CASE_DELETE = "DELETE";
        public const string COMMAND_CASE_REMOVE = "REMOVE";
        public const string COMMAND_CASE_VIEW = "VIEW";
        public const string COMMAND_MARK_ALL_AS = "MARK ALL AS DONE";
        public const string COMMAND_MARK_ALL = "MARK ALL DONE";
        public const string COMMAND_UNARCHIVE = "AS NOT DONE";
        public const string COMMAND_MARK = "DONE";
        public const string COMMAND_MARK_AS = "AS DONE";
        public const string COMMAND_ALL = "ALL";
        public const string COMMAND_ARCHIVE = "ARCHIVE";
        public const string COMMAND_WEEK = "WEEK";
        public const string COMMAND_RANGE = "TO";
        public const string COMMAND_RANGE_TILL = "TILL";
        public const string COMMAND_RANGE_ALT = "-";
        public const string COMMAND_RANGE_AND = "AND";
        #endregion

        #region Constant numbers
        //@author U096089W
        public static int TASK_OVERDUE_START_X = 0;
        public static int TASK_OVERDUE_START_Y = 0;
        public static int TASK_OVERDUE_END_X = 0;
        public static int TASK_OVERDUE_END_Y = 1;
        public static double TASK_OVERDUE_GRADIENT_WHITE = 0.0;
        public static int TASK_OVERDUE_GRADIENT_LIGHT_GRAY = 1;
        public static int TASK_NORMAL_START_X = 0;
        public static int TASK_NORMAL_START_Y = 0;
        public static int TASK_NORMAL_END_X = 0;
        public static int TASK_NORMAL_END_Y = 2;
        public static double TASK_NORMAL_GRADIENT_WHITE = 0.0;
        public static int TASK_NORMAL_GRADIENT_LIGHT_GRAY = 2;

        //@author U095146E
        public static int MAX_TOAST_WIDTH = 400;
        public static int TOAST_WIDTH_PER_CHARACTER = 10;
        public static int TOAST_ANIMATION_MILLISECONDS = 100;
        public static int TOAST_DISPLAY_DURATION_SECONDS = 3;
        public static int AUTOCOMPLETE_MAX_ITEMS = 10;
        public static int AUTOCOMPLETE_MAX_HEIGHT = 180;
        public static int AUTOCOMPLETE_PER_ITEM_HEIGHT = 18;
        
        //@author U095159L
        public static int DATE_COUNT = 2;
        public static int TODAY_ID = 7;
        public static int TOMORROW_ID = 8;
        #endregion
    }
}
