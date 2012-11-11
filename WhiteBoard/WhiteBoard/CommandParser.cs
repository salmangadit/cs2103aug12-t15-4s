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
    //@author U095159L
    class CommandParser
    {
        #region Protected Fields
        protected static readonly ILog Log = LogManager.GetLogger(typeof(CommandParser));
        #endregion

        #region Private Fields
        private string[] userCommandArray;
        private string inputCommand;
        private string timeString;
        private string correctFormat = String.Empty;
        private string taskDescription = null;

        private bool archiveFlag = false;

        private int taskId = 0;
        private int checkId = 0;
        private int checkDate = 0;
        private int currentIndex = 0;
        private int nextIndex = 0;
        private int taskDescriptionIndex = 0;
        private int startDateIndex = 0;
        private int endDateIndex = 0;
        private int startTimeFlag = 0;
        private int endTimeFlag = 0;
        private int firstDayFlag = 0;
        private int secondDayFlag = 0;
        private int userError = 0;

        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private FileHandler fileHandler;
        private Task taskToAdd;

        private List<string> stringList = new List<string>();
        private List<DateHolder> startEndDate = new DateHolder[2].ToList();
        private List<TimeSpan> startEndTime = new TimeSpan[2].ToList();
        private List<string> userCommand = new List<string>();
        private List<Task> screenState;

        private Stack<Command> taskHistory;
        #endregion

        #region Constructors
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
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Method to return UserCommand list for unit testing
        /// </summary>
        /// <returns>userCommand list</returns>
        public List<string> ReturnUserCommandListForTesting()
        {
            Debug.Assert(userCommand != null, "User command was null");
            return userCommand;
        }

        /// <summary>
        /// Parses the user command and determines the action to be done i.e Add, Modify, Delete etc.
        /// </summary>
        /// <returns>Returns a command object with details of the Task item</returns>
        public Command ParseCommand(string usercommand)
        {
            SplitString(usercommand);
            switch (userCommand[0].ToString().ToUpper())
            {
                case Constants.COMMAND_CASE_SEARCH:
                    {
                        return ParseSearch();
                    }
                case Constants.COMMAND_CASE_UNDO:
                    {
                        return ParseUndo();
                    }
                case Constants.COMMAND_CASE_DELETE:
                case Constants.COMMAND_CASE_REMOVE:
                    {
                        return ParseDelete();
                    }
                case Constants.COMMAND_CASE_MARK:
                    {
                        return ParseDone();
                    }
                case Constants.COMMAND_CASE_VIEW:
                    {
                        return ParseView();
                    }
                case Constants.COMMAND_CASE_MODIFY:
                case Constants.COMMAND_CASE_UPDATE:
                    {
                        return ParseModify();
                    }
                default:
                    {
                        return ParseNewTask();
                    }
            }
        }
        #endregion

        #region Private Class Helper Methods
        /// <summary>
        /// Splits usercommand string, removes extra blankspaces and adds each split word to list
        /// </summary>
        /// <param name="usercommand">The string input by user</param>
        private void SplitString(string usercommand)
        {
            Debug.Assert(usercommand != null, "User command was null");

            if (usercommand == null)
            {
                return;
            }

            inputCommand = usercommand;
            inputCommand = Regex.Replace(inputCommand, @"\s+", " ");                        //Replace multiple blank spaces with single space
            inputCommand = inputCommand.Trim();                                             //Remove leading and trailing whitespaces

            Log.Debug("Extra white spaces removed. Input command : " + inputCommand);

            userCommandArray = inputCommand.Split(' ');

            foreach (string str in userCommandArray)
            {
                str.Trim();
                userCommand.Add(str);
            }
        }

        /// <summary>
        /// Returns the string to be searched
        /// </summary>
        /// <returns>The information that is to be searched</returns>
        private Command ParseSearch()
        {
            string searchstring = ConvertToString(userCommand, stringList, 1, userCommand.Count - 1);

            Log.Debug(String.Format("Search keyword entered. The search term is : {0}", searchstring));

            SearchCommand search = new SearchCommand(fileHandler, searchstring, screenState);
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

            if (userCommand.Count == 1)
            {
                Command previouscommand = (taskHistory.Count > 0 ? taskHistory.Pop() : null);
                if (previouscommand != null)
                {
                    UndoCommand undo = new UndoCommand(fileHandler, previouscommand, screenState);
                    return undo;
                }
                else
                    return null;
            }
            else
                return ParseNewTask();
        }

        /// <summary>
        /// Checks which tasks to delete based on the Task ID or whether to delete all tasks on screen
        /// </summary>
        /// <returns>DeleteCommand object along with the corresponding Task ID</returns>
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
                else if (String.Equals(userCommand[nextIndex], Constants.COMMAND_ALL, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == Constants.COMMAND_LENGTH)     //Delete all tasks on screen
                {
                    Log.Debug("Delete all command");

                    DeleteCommand delete = new DeleteCommand(fileHandler, screenState);
                    taskHistory.Push(delete);
                    return delete;
                }
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
        /// Checks which task to mark as done based on the Task ID or whether to archive all tasks on screen
        /// </summary>
        /// /// <returns>DeleteCommand Object with the corresponding Task ID</returns>
        private Command ParseDone()
        {
            string command = String.Join(" ", userCommand.ToArray());

            Log.Debug("Archive keyword entered. Checking task ID");

            currentIndex = 0;
            nextIndex = currentIndex + 1;
            checkId = IsValidTaskId(userCommand[nextIndex]);    //Checks if a valid integer ID is entered
            if (checkId > 0)
            {
                Log.Debug("Valid task ID entered. The ID is: " + checkId);

                string temp = ConvertToString(userCommand, stringList, nextIndex + 1, userCommand.Count - 1);   //Gets the substring starting from the index following the taskID
                temp = temp.Trim();
                if (String.Equals(temp, Constants.COMMAND_MARK, StringComparison.CurrentCultureIgnoreCase)
                    || String.Equals(temp, Constants.COMMAND_MARK_AS, StringComparison.CurrentCultureIgnoreCase)) //Checks whether to archive a task
                {
                    archiveFlag = true;
                    taskId = checkId;
                    ArchiveCommand markdone = new ArchiveCommand(fileHandler, taskId, screenState);
                    taskHistory.Push(markdone);
                    return markdone;
                }
                else if(String.Equals(temp, Constants.COMMAND_UNARCHIVE, StringComparison.CurrentCultureIgnoreCase)) //Checks whether to unarchive a a task
                {
                    taskId = checkId;
                    UnArchiveCommand unarchive = new UnArchiveCommand(fileHandler,taskId,screenState);
                    taskHistory.Push(unarchive);
                    return unarchive;
                }
            }
            else if ((String.Equals(command, Constants.COMMAND_MARK_ALL, StringComparison.CurrentCultureIgnoreCase)) ||  //Check whether to mark all tasks on screen as done
                    (String.Equals(command, Constants.COMMAND_MARK_ALL_AS, StringComparison.CurrentCultureIgnoreCase)))
            {
                Log.Debug("Mark all as done");

                ArchiveCommand markall = new ArchiveCommand(fileHandler, screenState);
                taskHistory.Push(markall);
                return markall;
            }

            Log.Debug("Task ID not valid. Calling ParseNewTask()");

            return ParseNewTask();
        }

        /// <summary>
        /// Parses the command and extracts the parameters for the View Command
        /// </summary>
        /// <returns>View command object containing the date and time for which the user has requested to view</returns>
        private Command ParseView()
        {
            int datefoundflag = 0;      //datefoundflag is set to 1 if atleast one date/time is found
            currentIndex = 0;
            nextIndex = currentIndex + 1;

            Log.Debug("View keyword entered. Checking parameters");

            if (userCommand.Count > 1)
            {
                if (String.Equals(userCommand[nextIndex], Constants.COMMAND_ALL, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == Constants.COMMAND_LENGTH)            //View All
                {
                    startDate = endDate = null;
                    archiveFlag = false;
                    datefoundflag = 1;
                }

                else if (String.Equals(userCommand[nextIndex], Constants.COMMAND_ARCHIVE, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == Constants.COMMAND_LENGTH)   //View Archive
                {
                    startDate = endDate = null;
                    archiveFlag = true;
                    datefoundflag = 1;
                }

                else if (String.Equals(userCommand[nextIndex], Constants.COMMAND_WEEK, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == Constants.COMMAND_LENGTH)      //View Week
                {
                    AssignWeek();
                    archiveFlag = false;
                    datefoundflag = 1;
                }

                else
                {
                    startDate = null;
                    endDate = null;
                    if (ParseForDates(userCommand, nextIndex, false) > 0)
                    {
                        datefoundflag = 1;
                        AssignDates();
                    }
                }

                if (datefoundflag == 1)
                {
                    Log.Debug(String.Format("Valid parameters. Request to view tasks with startdate: {0} and/or enddate: {1}",
                        startDate.ToString(),
                        endDate.ToString()));
                    if (endDate != null && (endDate.Value.Hour == 0 && endDate.Value.Minute == 0))
                    {
                        //All tasks for the entire day should be shown, hence we set the time to 23:59:59
                        endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    }
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
            int datefoundflag = 0;      //datefoundflag is set to 1 if atleast one date/time is found
            int validtaskid = 0;
            currentIndex = 0;
            nextIndex = currentIndex + 1;

            Log.Debug("Modify keyword entered. Checking task ID");

            foreach (string keyword in Constants.COMMAND_MODIFY)
            {
                if (String.Equals(keyword, userCommand[currentIndex], StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                {
                    checkId = IsValidTaskId(userCommand[nextIndex]);
                    if (checkId > 0)
                    {
                        Log.Debug("Valid task ID entered. The ID is: " + checkId);

                        taskId = checkId;
                        validtaskid = 1;
                    }
                }
            }

            if (validtaskid != 0)
            {
                currentIndex = 0;
                int invertdates = 0;
                int descindex = 2;

                foreach (string str in userCommand)
                {
                    foreach (string keyword in Constants.COMMAND_NEW_DATE)
                    {
                        if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                        {
                            if (datefoundflag == 0)
                            {
                                if (String.Equals(str, Constants.COMMAND_NEW_DATE[0], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    //Keyword for new start date found
                                    Log.Debug("Keyword for new start date found. Parsing for dates");

                                    startDateIndex = currentIndex + 1;
                                    if (ParseForDates(userCommand, startDateIndex, true) > 0)
                                    {
                                        Log.Debug("New dates found");

                                        datefoundflag = 1;
                                    }
                                }

                                if (String.Equals(str, Constants.COMMAND_NEW_DATE[1], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    //Keyword for new end date found
                                    Log.Debug("Keyword for new end date found. Parsing for dates");

                                    endDateIndex = currentIndex + 1;
                                    if (ParseForDates(userCommand, endDateIndex, true) > 0)
                                    {
                                        Log.Debug("New dates found");

                                        datefoundflag = 1;
                                        invertdates = 1;
                                    }
                                }
                            }
                        }
                    }
                    currentIndex++;
                }

                if (datefoundflag == 0 && nextIndex < userCommand.Count - 1) //if no new dates are found, get only the new task description if any
                {
                    if (String.Equals(userCommand[nextIndex + 1], Constants.COMMAND_RANGE, StringComparison.CurrentCultureIgnoreCase) && nextIndex < userCommand.Count - 2)
                    {
                        taskDescription = ConvertToString(userCommand, stringList, nextIndex + 2, userCommand.Count - 1);
                    }
                    else
                    {
                        taskDescription = ConvertToString(userCommand, stringList, nextIndex + 1, userCommand.Count - 1);
                    }

                    Log.Debug(String.Format("No new dates found. New task description is : {0}", taskDescription));
                }

                else if (datefoundflag == 1)
                {
                    AssignDates();
                    if (invertdates == 1)   //end date was parsed before start date, need to swap values
                    {
                        DateTime? temp = startDate;
                        startDate = endDate;
                        endDate = temp;
                    }
                    taskDescription = ConvertToString(userCommand, stringList, descindex, taskDescriptionIndex);
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
        /// <returns>Returns an AddCommand object containing the description and start and end DateTime values</returns>
        private Command ParseNewTask()
        {
            int datefoundflag = 0;      //datefoundflag is set to 1 if atleast one date/time is found
            int keywordfound = 0;       //keywordfound is set to 1 if one of the keywords in COMMAND_DATE is found
            currentIndex = 0;

            Log.Debug("Parsing for adding a new task");

            foreach (string str in userCommand)
            {
                if (datefoundflag == 0)
                {
                    if ((String.Equals(str, "TODAY", StringComparison.CurrentCultureIgnoreCase)) || (String.Equals(str, "TOMORROW", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        nextIndex = currentIndex;
                        keywordfound = 1;
                    }

                    if (keywordfound == 0)
                    {
                        foreach (string keyword in Constants.COMMAND_DATE)
                        {
                            if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                            {
                                nextIndex = currentIndex + 1;
                                keywordfound = 1;
                            }
                        }
                    }

                    if (keywordfound == 1 && datefoundflag == 0)
                    {
                        if (ParseForDates(userCommand, nextIndex, false) > 0)
                        {
                            datefoundflag = 1;
                            AssignDates();
                        }
                    }
                    keywordfound = 0;
                    currentIndex++;
                }
            }
            if (datefoundflag == 1 && userError == 0)
            {
                Log.Debug(String.Format("Valid date parameters. Add new task with startdate: {0} and/or enddate: {1}",
                        startDate.ToString(),
                        endDate.ToString()));

                taskDescription = ConvertToString(userCommand, stringList, 0, taskDescriptionIndex);
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
        /// Assigns the start date with the monday of the current week and 
        /// the end date with the sunday of the same week
        /// User for the "View Week" command
        /// </summary>
        private void AssignWeek()
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            int days = today - DayOfWeek.Monday;
            DateTime temp_start = DateTime.Now.AddDays(-days);
            DateTime temp_end = temp_start.AddDays(6);      //Adds six days from Monday to get Saturday
            startDate = temp_start;
            startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);        //Zeroes the hour, minute and seconds fields
            endDate = temp_end;
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

            List<string> sublist = commandlist.GetRange(startdateindex, userCommand.Count - (startdateindex)); //Split the command starting from the index following the keyword
            firstsublist = sublist;

            Log.Debug(String.Format("User command split at keyword : ", firstsublist.ToString()));

            foreach (string str in sublist)
            {
                if (splitindex < 0)
                {
                    if (!modify)    //If the ParseModify() method is calling this method
                    {
                        if (String.Equals(str, Constants.COMMAND_RANGE, StringComparison.CurrentCultureIgnoreCase)
                            || String.Equals(str, Constants.COMMAND_RANGE_TILL, StringComparison.CurrentCultureIgnoreCase)
                            || String.Equals(str, Constants.COMMAND_RANGE_AND, StringComparison.CurrentCultureIgnoreCase)
                            || String.Equals(str, Constants.COMMAND_RANGE_ALT))
                        {
                            splitindex = i;
                        }
                    }
                    else            //If any other method is calling this method
                    {
                        if (String.Equals(str, Constants.COMMAND_NEW_DATE[0], StringComparison.CurrentCultureIgnoreCase)
                        || String.Equals(str, Constants.COMMAND_NEW_DATE[1], StringComparison.CurrentCultureIgnoreCase))
                        {
                            splitindex = i;
                        }
                    }
                }
                i++;
            }

            if (splitindex > 0)     //If a keyword is found that denotes multiple dates or a date range then split the command further
            {
                secondsublist = sublist.GetRange(splitindex, sublist.Count - (splitindex));
                firstsublist = sublist.GetRange(0, (sublist.Count - secondsublist.Count));

                Log.Debug(String.Format("Date range keyword found. Command split into two. \"{0}\" and \"{1}\"",
                    firstsublist.ToString(),
                    secondsublist.ToString()));
            }

            foreach (string word in firstsublist)
            {
                //If the "PM" of the time is separated with a blank space, check the previous string and parse the time
                if (startTimeFlag == 0 && (String.Equals(word, "PM", StringComparison.CurrentCultureIgnoreCase) || (String.Equals(word, "AM", StringComparison.CurrentCultureIgnoreCase))))
                {
                    string temp = word;
                    if (IsTime((temp = String.Format("{0} {1}", firstsublist[firstsublist.IndexOf(word) - 1], word))))
                    {
                        startEndTime[0] = ParseTime(temp);
                        if (firstDayFlag == 0)
                        {
                            taskDescriptionIndex = (userCommand.IndexOf(word) - 2);  //Index of last word of task description
                        }
                        startTimeFlag = 1;
                    }
                }
            }

            foreach (string word in firstsublist)   //Parse the first part of the command for day and time
            {
                if (firstDayFlag == 0 && (checkDate = IsValidDate(word)) > 0)
                {
                    DateHolder date = new DateHolder(word, checkDate);
                    startEndDate[0] = date;
                    endDate = null;
                    firstDayFlag = 1;
                    if (startTimeFlag == 0)
                    {
                        taskDescriptionIndex = (userCommand.IndexOf(word) - 1);     //Index of last word of task description
                    }
                }

                if (startTimeFlag == 0 && IsTime(word))
                {
                    startEndTime[0] = ParseTime(word);
                    startTimeFlag = 1;
                    if (firstDayFlag == 0)
                    {
                        taskDescriptionIndex = (userCommand.IndexOf(word) - 1);
                    }
                }
            }

            if (splitindex > 0 && (startTimeFlag > 0 || firstDayFlag > 0))
            {
                //If a date or time was found, then parse the second part of the command for another date or time
                Log.Debug("Date found in first part. Parsing second part");

                foreach (string word in secondsublist)
                {
                    if (endTimeFlag == 0 && (String.Equals(word, Constants.COMMAND_PM, StringComparison.CurrentCultureIgnoreCase)
                        || (String.Equals(word, Constants.COMMAND_AM, StringComparison.CurrentCultureIgnoreCase))))
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
                    if (secondDayFlag == 0 && (checkDate = IsValidDate(word)) > 0)
                    {
                        DateHolder date = new DateHolder(word, checkDate);
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
        /// Method to check the various date and times parsed and assign them accordingly
        /// to the start and end dates.
        /// </summary>
        private void AssignDates()
        {
            if (startEndDate[0] != null)    //If there is a start date
            {
                startDate = startEndDate[0].ConvertToDateTime();
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                if (startTimeFlag == 1)     //And there is a start time
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startEndTime[0].Hours, startEndTime[0].Minutes, 0);
                }
                if (startEndDate[1] == null)
                {
                    if (endTimeFlag == 1)   //If there is no end date, but there is an end time, add it for the startDate's date
                    {
                        endDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startEndTime[1].Hours, startEndTime[1].Minutes, 0);
                    }
                }
                else if (startEndDate[1] != null)
                {
                    endDate = startEndDate[1].ConvertToDateTime();
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 0, 0, 0);
                    if (endTimeFlag == 1)   //Or if there is an end date, add the end time to the endDate
                    {
                        endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, startEndTime[1].Hours, startEndTime[1].Minutes, 0);
                    }
                }
            }
            else if (startEndDate[0] == null)   //Id there is no start date
            {
                if (startTimeFlag == 1)     //But if there is a start time, assume it is today and add the time to it
                {
                    startDate = DateTime.Now;
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startEndTime[0].Hours, startEndTime[0].Minutes, 0);
                    if (endTimeFlag == 1)
                    {
                        if (startEndDate[1] != null)    //If there is no end date, but there is an end time, add it for the startDate's date
                        {
                            endDate = startEndDate[1].ConvertToDateTime();
                            endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, startEndTime[1].Hours, startEndTime[1].Minutes, 0);
                        }
                        else   //Or if there is an end date, add the end time to the endDate
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

        /// <summary>
        /// Checks whether the string is a valid integer for the task ID
        /// </summary>
        /// <param name="str">The string to be checked</param>
        /// <returns>Returns -1 if string is not a valid task ID otherwise returns the ID</returns>
        private int IsValidTaskId(string str)
        {
            int taskid = -1;
            for (int i = 0; i < str.Length; ++i)
            {
                int temp = (int)Char.GetNumericValue(str[i]);
                if (!(temp >= 0 && temp <= 9))
                {
                    return -1;
                }
            }
            taskid = Convert.ToInt32(str);
            return taskid;
        }

        /// <summary>
        /// Checks whether the string is a valid time
        /// </summary>
        /// <param name="time">The string to be checked</param>
        /// <returns>Returns true if time is valid</returns>
        private bool IsTime(string time)
        {
            timeString = time;
            //Pad certain time with zeroes to make the recognisable as a time format
            if (timeString.Length == 3)
            {
                timeString = timeString.PadLeft(4, '0');
            }
            else if (time.Length == 5 && (!(time.Contains(':'))) && (!(time.Contains(' '))))
            {
                timeString = timeString.PadLeft(6, '0');
            }
            else if (timeString.Length == 6 && timeString.Contains(' '))
            {
                timeString = timeString.PadLeft(7, '0');
            }
            bool istime = false;
            DateTime temp;
            foreach (string format in Constants.TIME_FORMATS)
            {
                if (!istime)
                {
                    istime = DateTime.TryParseExact(timeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
                    correctFormat = format;
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
            DateTime temp = new DateTime();
            if (IsTime(time))
            {
                temp = DateTime.ParseExact(timeString, correctFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
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
            foreach (string str in Constants.DAYS_OF_WEEK)
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
        private bool IsDate(string datestring)
        {
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-gb");
            bool isdate = false;
            DateTime temp;
            foreach (string format in Constants.DATE_FORMATS)
            {
                if (!isdate)
                {
                    isdate = DateTime.TryParseExact(datestring, format, cultureinfo, DateTimeStyles.None, out temp);
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

            foreach (string keyword in Constants.COMMAND_KEYWORD_REMOVE)
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
        #endregion
    }
}
