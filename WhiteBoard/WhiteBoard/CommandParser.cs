using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using log4net;


namespace WhiteBoard
{
    class CommandParser
    {
        private string[] DAYS_OF_WEEK = { "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "TODAY", "TOMORROW" };
        private string[] COMMAND_DATE = { "BY", "ON", "BEFORE", "AT", "FROM", "BETWEEN" };
        private string[] COMMAND_MODIFY = { "MODIFY", "CHANGE", "UPDATE" };
        private string[] COMMAND_NEW_DATE = { "START", "END" };
        private string[] COMMAND_VIEW_DAY = { "ON", "AT" };
        private string[] COMMAND_VIEW_RANGE = { "ON", "FROM", "BETWEEN" };
        private string[] COMMAND_VIEW_ENDING = { "BY", "BEFORE", "ENDING" };
        private string[] COMMAND_KEYWORD_REMOVE = { "BY", "ON", "BEFORE", "AT", "FROM", "BETWEEN", "START", "END" };
        private const string COMMAND_MARK = "DONE";
        private const string COMMAND_MARK_AS = "AS DONE";
        private const string COMMAND_ALL = "ALL";
        private const string COMMAND_ARCHIVE = "ARCHIVE";
        private const string COMMAND_WEEK = "WEEK";
        private const string COMMAND_RANGE = "TO";
        private const string COMMAND_RANGE_ALT = "-";
        private const string COMMAND_RANGE_AND = "AND";
        private const int DATE_COUNT = 2;

        private string searchString;
        private string inputCommand;
        private string taskDescription = null;
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string[] userCommandArray;
        private FileHandler fileHandler;
        private Task taskToAdd;
        private int taskId = 0;
        private int checkId = 0;
        private int checkFirstDate = 0;

        private bool archiveFlag = false;
        private int modifyFlag = 0;
        private int viewFlag = 0;
        private int dateFlag = 0;
        private int currentIndex = 0;
        private int nextIndex = 0;
        private int previousIndex = 0;
        private int startDateIndex = 0;
        private int endDateIndex = 0;
        private int startTimeFlag = 0;
        private int endTimeFlag = 0;
        private int firstDayFlag = 0;
        private int secondDayFlag = 0;
        private int userError = 0;

        private List<string> stringList = new List<string>();
        private List<DateHolder> startEndDate = new DateHolder[2].ToList();
        private List<TimeSpan> startEndTime = new TimeSpan[2].ToList();
        private List<string> userCommand = new List<string>();

        private List<Task> screenState;
        private Stack<Command> taskHistory;

        protected static readonly ILog Log = LogManager.GetLogger(typeof(CommandParser));

        public CommandParser()
        {
        }

        public CommandParser(List<Task> screenState, Stack<Command> taskHistory)
        {
            fileHandler = FileHandler.Instance;

            Debug.Assert(screenState != null, "Screen State was null");
            Debug.Assert(taskHistory != null, "Task History was null");

            this.screenState = screenState;
            this.taskHistory = taskHistory;
        }

        /// <summary>
        /// Splits usercommand string and adds to list
        /// </summary>
        /// <param name="usercommand">The string input by user5</param>
        private void SplitString(string usercommand)
        {
            Debug.Assert(usercommand != null, "User command was null");

            if (usercommand == null)
            {
                return;
            }

            inputCommand = usercommand;
            inputCommand = Regex.Replace(inputCommand, @"\s+", " ");

            Log.Debug("Extra white spaces removed. Input command : " + inputCommand);

            userCommandArray = inputCommand.Split(' ');

            foreach (string str in userCommandArray)
            {
                str.Trim();
                userCommand.Add(str);
            }
        }

        public List<string> ReturnUserCommandListForTesting()
        {
            Debug.Assert(userCommand != null, "User command was null");
            return userCommand;
        }
        /// <summary>
        /// Parses the user command and determines the action to be done i.e Add, Modify, Delete etc.
        /// </summary>
        /// <returns>Returns a command object with details of the ToDo item</returns>
        public Command ParseCommand(string usercommand)
        {
            SplitString(usercommand);
            switch (userCommand[0].ToString().ToUpper())
            {
                case "SEARCH:":
                    {
                        return ParseSearch();
                    }
                case "UNDO:":
                    {
                        return ParseUndo();
                    }
                case "DELETE":
                case "REMOVE":
                    {
                        return ParseDelete();
                    }
                case "MARK":
                    {
                        return ParseDone();
                    }
                case "VIEW":
                    {
                        return ParseView();
                    }
                case "MODIFY":
                case "CHANGE":
                case "UPDATE":
                    {
                        return ParseModify();
                    }
                default:
                    {
                        return ParseNewTask();
                    }
            }
        }

        /// <summary>
        /// Returns the string to be searched
        /// </summary>
        /// <returns>The information that is to be searched</returns>
        private Command ParseSearch()
        {
            searchString = ConvertToString(userCommand, stringList, 1, userCommand.Count - 1);

            Log.Debug(String.Format("Search keyword entered. The search term is : {0}", searchString));

            SearchCommand search = new SearchCommand(fileHandler, searchString, screenState);
            taskHistory.Push(search);
            return search;
        }

        /// <summary>
        /// Returns an undo command object containing the last executed command
        /// from the taskHistory stack
        /// </summary>
        /// <returns>UndoCommand Object</returns>
        private Command ParseUndo()
        {
            Log.Debug("Undo keyword entered.");

            Command lastcommand = (taskHistory.Count > 0 ? taskHistory.Pop() : null);
            if (lastcommand != null)
            {
                UndoCommand undo = new UndoCommand(fileHandler, lastcommand, screenState);
                return undo;
            }
            else
                return null;
        }

        /// <summary>
        /// Checks which task to delete based on the Task ID
        /// </summary>
        /// <returns>DeleteCommand Object with the corresponding Task ID</returns>
        private Command ParseDelete()
        {
            Log.Debug("Delete keyword entered. Checking task ID");

            currentIndex = 0;
            nextIndex = currentIndex + 1;
            if (userCommand.Count > 1)
            {
                checkId = IsValidTaskId(userCommand[nextIndex]);
                if (checkId > 0)
                {
                    Log.Debug("Valid task ID entered. The ID is: " + checkId);

                    taskId = checkId;
                    archiveFlag = false;
                    DeleteCommand delete = new DeleteCommand(fileHandler, taskId, screenState);
                    taskHistory.Push(delete);
                    return delete;
                }
                //else if (String.Equals(userCommand[nextIndex], COMMAND_ALL, StringComparison.CurrentCulture))
                //{
                //    Log.Debug("Delete all command");

                //    DeleteCommand delete = new DeleteCommand(fileHandler, screenState, true);
                //}
                else
                {
                    Log.Debug("Task ID not valid and Delete All not called. Calling ParseNewTask()");
                    return ParseNewTask();
                }
            }
            else
            {
                Log.Debug("Task ID not valid. Calling ParseNewTask()");
                return ParseNewTask();
            }
        }

        /// <summary>
        /// Checks whic task to mark as done based on the Task ID
        /// </summary>
        /// /// <returns>DeleteCommand Object with the corresponding Task ID</returns>
        private Command ParseDone()
        {
            Log.Debug("Archive keyword entered. Checking task ID");
            currentIndex = 0;
            nextIndex = currentIndex + 1;
            checkId = IsValidTaskId(userCommand[nextIndex]);
            if (checkId > 0)
            {
                Log.Debug("Valid task ID entered. The ID is: " + checkId);

                string temp = ConvertToString(userCommand, stringList, nextIndex + 1, userCommand.Count - 1);
                temp = temp.Trim();
                if (String.Equals(temp, COMMAND_MARK, StringComparison.CurrentCultureIgnoreCase)
                    || String.Equals(temp, COMMAND_MARK_AS, StringComparison.CurrentCultureIgnoreCase))
                {
                    archiveFlag = true;
                    taskId = checkId;
                    ArchiveCommand markdone = new ArchiveCommand(fileHandler, taskId, screenState);
                    taskHistory.Push(markdone);
                    return markdone;
                }
            }
            Log.Debug("Task ID not valid. Calling ParseNewTask()");

            return ParseNewTask();
        }
        /// <summary>
        /// Parses the command and extracts the parameters for the View Command
        /// </summary>
        private Command ParseView()
        {
            viewFlag = 0;
            currentIndex = 0;
            nextIndex = currentIndex + 1;

            Log.Debug("View keyword entered. Checking parameters");

            if (userCommand.Count > 1)
            {
                if (String.Equals(userCommand[nextIndex], COMMAND_ALL, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == 2)
                {
                    startDate = endDate = null;
                    archiveFlag = false;
                    viewFlag = 1;
                }

                else if (String.Equals(userCommand[nextIndex], COMMAND_ARCHIVE, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == 2)
                {
                    startDate = endDate = null;
                    archiveFlag = true;
                    viewFlag = 1;
                }

                else if (String.Equals(userCommand[nextIndex], COMMAND_WEEK, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == 2)
                {
                    DayOfWeek today = DateTime.Now.DayOfWeek;
                    int days = today - DayOfWeek.Monday;
                    DateTime temp_start = DateTime.Now.AddDays(-days);
                    DateTime temp_end = temp_start.AddDays(6);
                    startDate = temp_start;
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                    endDate = temp_end;
                    archiveFlag = false;
                    viewFlag = 1;
                }

                else
                {
                    startDate = null;
                    endDate = null;
                    if (ParseForDates(userCommand, nextIndex, false) > 0)
                    {
                        viewFlag = 1;
                        AssignDates();
                    }
                }

                if (viewFlag == 1)
                {
                    Log.Debug(String.Format("Valid parameters. Request to view tasks with startdate: {0} and/or enddate: {1}",
                        startDate.ToString(),
                        endDate.ToString()));

                    Task viewtaskdetails = new Task(0, null, startDate, endDate, archiveFlag);
                    ViewCommand view = new ViewCommand(fileHandler, viewtaskdetails, screenState);
                    taskHistory.Push(view);
                    return view;
                }

                else
                {
                    Log.Debug("Invalid view parameters. Calling ParseNewTask()");

                    return ParseNewTask();
                }
            }
            else
            {
                Log.Debug("Invalid view parameters. Calling ParseNewTask()");

                return ParseNewTask();
            }
        }

        /// <summary>
        /// Parses the user command and determines the date and the taskdescription
        /// </summary>
        /// <returns>Returns a command object with details of the ToDo item</returns>
        private Command ParseModify()
        {
            currentIndex = 0;
            nextIndex = currentIndex + 1;

            Log.Debug("Modify keyword entered. Checking task ID");

            foreach (string keyword in COMMAND_MODIFY)
            {
                if (String.Equals(keyword, userCommand[currentIndex], StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                {
                    checkId = IsValidTaskId(userCommand[nextIndex]);
                    if (checkId > 0)
                    {
                        Log.Debug("Valid task ID entered. The ID is: " + checkId);

                        taskId = checkId;
                        modifyFlag = 1;
                    }
                }
            }

            if (modifyFlag != 0)
            {
                currentIndex = 0;
                int invertdates = 0;
                int descindex = 2;

                foreach (string str in userCommand)
                {
                    foreach (string keyword in COMMAND_NEW_DATE)
                    {
                        if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                        {
                            if (dateFlag == 0)
                            {
                                if (String.Equals(str, COMMAND_NEW_DATE[0], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Log.Debug("Keyword for new start date found. Parsing for dates");

                                    startDateIndex = currentIndex + 1;
                                    if (ParseForDates(userCommand, startDateIndex, true) > 0)
                                    {
                                        Log.Debug("New dates found");

                                        dateFlag = 1;
                                    }
                                }

                                if (String.Equals(str, COMMAND_NEW_DATE[1], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Log.Debug("Keyword for new end date found. Parsing for dates");

                                    endDateIndex = currentIndex + 1;
                                    if (ParseForDates(userCommand, endDateIndex, true) > 0)
                                    {
                                        Log.Debug("New dates found");

                                        dateFlag = 1;
                                        invertdates = 1;
                                    }
                                }
                            }
                        }
                    }
                    currentIndex++;
                }

                if (dateFlag == 0 && nextIndex < userCommand.Count - 1)
                {
                    if (String.Equals(userCommand[nextIndex + 1], COMMAND_RANGE, StringComparison.CurrentCultureIgnoreCase) && nextIndex < userCommand.Count - 2)
                    {
                        taskDescription = ConvertToString(userCommand, stringList, nextIndex + 2, userCommand.Count - 1);
                    }
                    else
                    {
                        taskDescription = ConvertToString(userCommand, stringList, nextIndex + 1, userCommand.Count - 1);
                    }

                    Log.Debug(String.Format("No new dates found. New task description is : {0}", taskDescription));
                }

                else if (dateFlag == 1)
                {
                    AssignDates();
                    if (invertdates == 1)
                    {
                        DateTime? temp = startDate;
                        startDate = endDate;
                        endDate = temp;
                    }
                    taskDescription = ConvertToString(userCommand, stringList, descindex, previousIndex);
                    if (taskDescription == String.Empty)
                    {
                        taskDescription = null;
                    }

                    Log.Debug(String.Format("New start and/or end dates. startdate: {0} and enddate: {1}",
                        startDate.ToString(),
                        endDate.ToString()));
                    Log.Debug(String.Format("New task description : {0}", taskDescription));
                }

                else
                {
                    taskDescription = null;
                }
                Task taskToEdit = new Task(taskId, taskDescription, startDate, endDate);
                EditCommand edit = new EditCommand(fileHandler, taskToEdit, screenState);
                taskHistory.Push(edit);
                return edit;
            }
            else
            {
                Log.Debug("Task ID not valid. Calling ParseNewTask()");

                return ParseNewTask();
            }

        }

        /// <summary>
        /// Parses the user command and determines the date and the taskdescription
        /// </summary>
        private Command ParseNewTask()
        {
            currentIndex = 0;
            dateFlag = 0;
            int calldaterange = 0;

            Log.Debug("Parsing for adding a new task");

            foreach (string str in userCommand)
            {
                if (dateFlag == 0)
                {
                    if ((String.Equals(str, "TODAY", StringComparison.CurrentCultureIgnoreCase)) || (String.Equals(str, "TOMORROW", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        nextIndex = currentIndex;
                        calldaterange = 1;
                    }

                    if (calldaterange == 0)
                    {
                        foreach (string keyword in COMMAND_DATE)
                        {
                            if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                            {
                                nextIndex = currentIndex + 1;
                                calldaterange = 1;
                            }
                        }
                    }
                    if (calldaterange == 1 && dateFlag == 0)
                    {
                        if (ParseForDates(userCommand, nextIndex, false) > 0)
                        {
                            dateFlag = 1;
                            AssignDates();
                        }
                    }
                    calldaterange = 0;
                    currentIndex++;
                }
            }
            if (dateFlag == 1 && userError == 0)
            {
                Log.Debug(String.Format("Valid date parameters. Add new task with startdate: {0} and/or enddate: {1}",
                        startDate.ToString(),
                        endDate.ToString()));

                taskDescription = ConvertToString(userCommand, stringList, 0, previousIndex);
            }

            else
            {
                taskDescription = inputCommand;
                startDate = endDate = null;

                Log.Debug("No valid date parameters found.");
            }

            Log.Debug(String.Format("The description of the task : {0}", taskDescription));

            taskToAdd = new Task(0, taskDescription, startDate, endDate);
            AddCommand add = new AddCommand(fileHandler, taskToAdd, screenState);
            taskHistory.Push(add);
            return add;
        }

        /// <summary>
        /// Method to check the various date and times parsed and assign them accordingly
        /// to the start and end dates.
        /// </summary>
        private void AssignDates()
        {
            if (startEndDate[0] != null)
            {
                startDate = startEndDate[0].DateParse();
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                if (startTimeFlag == 1)
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startEndTime[0].Hours, startEndTime[0].Minutes, 0);
                }
                if (startEndDate[1] == null)
                {
                    if (endTimeFlag == 1)
                    {
                        endDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startEndTime[1].Hours, startEndTime[1].Minutes, 0);
                    }
                }
                else if (startEndDate[1] != null)
                {
                    endDate = startEndDate[1].DateParse();
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 0, 0, 0);
                    if (endTimeFlag == 1)
                    {
                        endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, startEndTime[1].Hours, startEndTime[1].Minutes, 0);
                    }
                }
            }
            else if (startEndDate[0] == null)
            {
                if (startTimeFlag == 1)
                {
                    startDate = DateTime.Now;
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startEndTime[0].Hours, startEndTime[0].Minutes, 0);
                    if (endTimeFlag == 1)
                    {
                        if (startEndDate[1] != null)
                        {
                            endDate = startEndDate[1].DateParse();
                            endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, startEndTime[1].Hours, startEndTime[1].Minutes, 0);
                        }
                        else
                        {
                            endDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startEndTime[1].Hours, startEndTime[1].Minutes, 0);
                        }
                    }
                }
                else
                {
                    userError = 1;
                }
            }

        }

        /// Checks if a date range is given and extracts the start and end date
        /// </summary>
        /// <param name="checkdate">The string to be parsed and checked</param>
        /// <returns>Returns >0 if valid start and/or end dates are found</returns>
        private int ParseForDates(List<string> commandlist, int index, bool modify)
        {
            Debug.Assert(index >= 0, "Index was negative");

            if (index < 0)
            {
                return 0;
            }

            List<string> firstsublist = new List<string>();
            List<string> secondsublist = new List<string>();
            int splitindex = -1;
            int i = 0;
            int startdateindex = index;

            List<string> sublist = commandlist.GetRange(startdateindex, userCommand.Count - (startdateindex));
            firstsublist = sublist;

            Log.Debug(String.Format("User command split at keyword : ", firstsublist.ToString()));

            foreach (string str in sublist)
            {
                if (splitindex < 0)
                {
                    if (!modify)
                    {
                        if (String.Equals(str, COMMAND_RANGE, StringComparison.CurrentCultureIgnoreCase)
                            || String.Equals(str, COMMAND_RANGE_AND, StringComparison.CurrentCultureIgnoreCase)
                            || String.Equals(str, COMMAND_RANGE_ALT))
                        {
                            splitindex = i;
                        }
                    }
                    else
                    {
                        if (String.Equals(str, COMMAND_NEW_DATE[0], StringComparison.CurrentCultureIgnoreCase)
                        || String.Equals(str, COMMAND_NEW_DATE[1], StringComparison.CurrentCultureIgnoreCase))
                        {
                            splitindex = i;
                        }
                    }
                }
                i++;
            }

            if (splitindex > 0)
            {
                secondsublist = sublist.GetRange(splitindex, sublist.Count - (splitindex));
                firstsublist = sublist.GetRange(0, (sublist.Count - secondsublist.Count));

                Log.Debug(String.Format("Date range keyword found. Command split into two. \"{0}\" and \"{1}\"",
                    firstsublist.ToString(),
                    secondsublist.ToString()));
            }

            foreach (string word in firstsublist)
            {
                if (startTimeFlag == 0 && (String.Equals(word, "PM", StringComparison.CurrentCultureIgnoreCase) || (String.Equals(word, "AM", StringComparison.CurrentCultureIgnoreCase))))
                {
                    string temp = word;
                    if (IsTime((temp = String.Format("{0} {1}", firstsublist[firstsublist.IndexOf(word) - 1], word))))
                    {
                        startEndTime[0] = ParseTime(temp);
                        if (firstDayFlag == 0)
                        {
                            previousIndex = (userCommand.IndexOf(word) - 2);
                        }
                        startTimeFlag = 1;
                    }
                }
            }

            foreach (string word in firstsublist)
            {
                if (firstDayFlag == 0 && (checkFirstDate = IsValidDate(word)) > 0)
                {
                    DateHolder date = new DateHolder(word, checkFirstDate);
                    startEndDate[0] = date;
                    endDate = null;
                    firstDayFlag = 1;
                    if (startTimeFlag == 0)
                    {
                        previousIndex = (userCommand.IndexOf(word) - 1);
                    }
                }

                if (startTimeFlag == 0 && IsTime(word))
                {
                    startEndTime[0] = ParseTime(word);
                    startTimeFlag = 1;
                    if (firstDayFlag == 0)
                    {
                        previousIndex = (userCommand.IndexOf(word) - 1);
                    }
                }
            }

            if (splitindex > 0 && (startTimeFlag > 0 || firstDayFlag > 0))
            {
                Log.Debug("Date found in first part. Parsing second part");

                foreach (string word in secondsublist)
                {
                    if (endTimeFlag == 0 && (String.Equals(word, "PM", StringComparison.CurrentCultureIgnoreCase) || (String.Equals(word, "AM", StringComparison.CurrentCultureIgnoreCase))))
                    {
                        string temp = word;
                        if (IsTime((temp = String.Format("{0} {1}", secondsublist[secondsublist.IndexOf(word) - 1], word))))
                        {
                            startEndTime[1] = ParseTime(temp);
                            endTimeFlag = 1;
                        }
                    }
                }

                foreach (string word in secondsublist)
                {
                    if (secondDayFlag == 0 && (checkFirstDate = IsValidDate(word)) > 0)
                    {
                        DateHolder date = new DateHolder(word, checkFirstDate);
                        startEndDate[1] = date;
                        secondDayFlag = 1;
                    }

                    if (endTimeFlag == 0 && IsTime(word))
                    {
                        startEndTime[1] = ParseTime(word);
                        endTimeFlag = 1;
                    }
                }
            }
            if (startEndDate[0] == null && startEndDate[1] == null && startTimeFlag == 0 && endTimeFlag == 0)
            {
                Log.Debug("No dates found.");

                return 0;
            }
            else
                return 1;
        }


        /// <summary>
        /// Checks whether the string is a task ID
        /// </summary>
        /// <param name="str">The string to be checked</param>
        /// <returns>Returns true if string is a valid task ID</returns>
        private int IsValidTaskId(string str)
        {
            int taskid = -1;
            if (char.ToUpperInvariant(str[0])!='T')
            {
                return -1;
            }
            else
            {
                for (int i = 1; i < str.Length; ++i)
                {
                    int temp = (int)Char.GetNumericValue(str[i]);
                    if (!(temp >= 0 && temp <= 9))
                    {
                        return -1;
                    }
                }
                str = str.Substring(1);
                taskid = Convert.ToInt32(str);
                return taskid;
            }
        }

        /// <summary>
        /// Checks whether the string is a valid time
        /// </summary>
        /// <param name="time">The string to be checked</param>
        /// <returns>Returns true if time is valid</returns>
        private bool IsTime(string time)
        {
            string[] timeformat = { "hh.mm", "h.mm", "h.mm tt","hh.mm tt","h.mmtt","hh.mmtt",
                                    "hh:mm", "h:mm", "h:mm tt","hh:mm tt", "h:mmtt","hh:mmtt",
                                    "hhmmtt","hmmtt","hhmm","hmm", "htt","hhtt",
                                    "HH:mm"};
            bool istime = false;
            DateTime temp;
            foreach (string format in timeformat)
            {
                if (!istime)
                {
                    istime = DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
                }
            }
            return istime;
        }

        /// <summary>
        /// Returns a valid TimeSpan object
        /// </summary>
        /// <param name="time">The string to be converted to time</param>
        /// <returns>The timespan object</returns>
        private TimeSpan ParseTime(string time)
        {
            string[] timeformat = { "hh.mm", "h.mm", "h.mm tt","hh.mm tt","h.mmtt","hh.mmtt",
                                    "hh:mm", "h:mm", "h:mm tt","hh:mm tt", "h:mmtt","hh:mmtt",
                                    "hhmmtt","hmmtt","hhmm","hmm", "htt","hhtt",
                                    "HH:mm"};

            string correctformat = String.Empty;
            bool istime = false;
            DateTime temp = new DateTime();
            foreach (string format in timeformat)
            {
                if (!istime)
                {
                    istime = DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
                    correctformat = format;
                }
            }
            if (istime)
            {
                temp = DateTime.ParseExact(time, correctformat, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            Debug.Assert(temp != null, "Time returned was null");
            return temp.TimeOfDay;
        }

        /// <summary>
        /// Checks if the string is a valid day of the week and assigns the correct date
        /// </summary>
        /// <param name="checkday">String</param>
        /// <returns>False if not a valid day</returns>
        private bool IsDay(string checkday)
        {
            foreach (string str in DAYS_OF_WEEK)
            {
                if (String.Equals(checkday, str, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the date is a valid date
        /// </summary>
        /// <param name="datestring">A string containing the date information</param>
        /// <returns>Returns true or false depending on whether the date is valid</returns>
        public bool IsDate(string datestring)
        {
            string[] dateformat = { "dd/mm/yyyy", "dd-mm-yyyy", "dd.mm.yyyy",
                                    "d/m/yyyy","dd/m/yyyy","d/mm/yyyy",
                                    "d.m.yyyy","dd.m.yyyy","d.mm.yyyy",
                                    "d-m-yyyy","dd-m-yyyy","d-mm-yyyy" };
            bool isdate = false;
            DateTime temp;
            foreach (string format in dateformat)
            {
                if (!isdate)
                {
                    isdate = DateTime.TryParseExact(datestring, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
                }
            }
            return isdate;
        }

        /// <summary>
        /// Checks if the string "checkdate" is a valid day or date
        /// </summary>
        /// <param name="checkdate">The string to be checked</param>
        /// <returns>Returns 1 if it is a day, 2 if it is a date or 0 if neither</returns>
        private int IsValidDate(string checkdate)
        {
            if (IsDay(checkdate))
            {
                return 1;
            }

            else if (IsDate(checkdate))
            {
                return 2;
            }

            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Takes the strings upto the specified index from the list and concatenates them
        /// into a single string
        /// This method also removes the keyword from the end of the task description
        /// </summary>
        /// <param name="list">The original list</param>
        /// <param name="templist">The list containing the strings upto the index</param>
        /// <param name="index">The specified index upto which the strings are retrieved</param>
        /// <returns></returns>
        private string ConvertToString(List<string> list, List<string> templist, int startindex, int endindex)
        {
            string tempstring;
            templist.Clear();
            for (int i = startindex; i <= endindex; ++i)
            {
                templist.Add(list[i]);
            }

            foreach (string keyword in COMMAND_KEYWORD_REMOVE)
            {
                if (templist.Count > 0)
                {
                    if (String.Equals(keyword, templist.Last(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        templist.RemoveAt(templist.Count - 1);
                    }
                }
            }
            tempstring = String.Join(" ", templist.ToArray());
            return tempstring;
        }
    }
}
