using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;


namespace WhiteBoard
{
    class CommandParser
    {
        private string[] COMMAND_DATE = { "BY", "ON", "BEFORE", "AT", "FROM", "BETWEEN" };
        private string[] COMMAND_MODIFY = { "MODIFY", "CHANGE", "UPDATE" };
        private string[] COMMAND_NEW_DATE = { "START", "END" };
        private string[] COMMAND_TASKS_DAY = { "ON", "AT" };
        private string[] COMMAND_TASKS_RANGE = { "ON", "FROM", "BETWEEN" };
        private string[] COMMAND_TASKS_ENDING = { "BY", "BEFORE", "ENDING" };
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
        private DateTime? deadlineDate = null;
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string[] userCommandArray;
        private FileHandler fileHandler;
        private Task taskToAdd;
        private int taskId = 0;
        private int checkId = 0;

        private bool archiveFlag = false;
        private int dateKeywordFlag = 0;
        private int modifyFlag = 0;
        private int tasksFlag = 0;
        private int dateFlag = 0;
        private int currentIndex = 0;
        private int nextIndex = 0;
        private int previousIndex = 0;
        private int taskIndex = 0;
        private int startDateIndex = 0;
        private int endDateIndex = 0;

        private List<string> stringList = new List<string>();
        private List<string> startEndDate = new List<string>();
        private List<string> userCommand = new List<string>();

        private List<Task> screenState;
        private Stack<Command> taskHistory;

        public CommandParser(FileHandler filehandler, List<Task> screenState, Stack<Command> taskHistory)
        {
            fileHandler = filehandler;
            this.screenState = screenState;
            this.taskHistory = taskHistory;
        }

        /// <summary>
        /// Splits usercommand string and adds to list
        /// </summary>
        /// <param name="usercommand">The string input by user5</param>
        private void SplitString(string usercommand)
        {
            inputCommand = usercommand;
            inputCommand = Regex.Replace(inputCommand, @"\s+", " ");
            userCommandArray = inputCommand.Split(' ');
            foreach (string str in userCommandArray)
            {
                str.Trim();
                userCommand.Add(str);
            }
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
            SearchCommand search = new SearchCommand(fileHandler, searchString, screenState);
            taskHistory.Push(search);
            return search;
        }

        /*      Salman im commenting out the undo function
         *      Im expecting an UndoCommand Class which accepts the last command object executed as one 
         *      of the parameters
         *      Sai could implement this class for me?
         */
        private Command ParseUndo()
        {
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
            currentIndex = 0;
            nextIndex = currentIndex + 1;
            if (userCommand.Count>1)
            {
                checkId = IsValidTaskId(userCommand[nextIndex]);
                if (checkId > 0)
                {
                    taskId = checkId;
                    archiveFlag = false;
                    DeleteCommand delete = new DeleteCommand(fileHandler, taskId, screenState);
                    taskHistory.Push(delete);
                    return delete;
                }
                else
                {
                    return ParseNewTask();
                }
            }
            else
            {
                return ParseNewTask();
            }
        }

        /// <summary>
        /// Checks whic task to mark as done based on the Task ID
        /// </summary>
        /// /// <returns>DeleteCommand Object with the corresponding Task ID</returns>
        private Command ParseDone()
        {
            currentIndex = 0;
            nextIndex = currentIndex + 1;
            checkId = IsValidTaskId(userCommand[nextIndex]);
            if (checkId > 0)
            {
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
            return ParseNewTask();
        }
        /// <summary>
        /// Parses the command and extracts the parameters for the View Command
        /// </summary>
        private Command ParseView()
        {
            tasksFlag = 0;
            currentIndex = 0;
            nextIndex = currentIndex + 1;

            if (userCommand.Count > 1)
            {
                if (String.Equals(userCommand[nextIndex], COMMAND_ALL, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == 2)
                {
                    startDate = endDate = deadlineDate = null;
                    archiveFlag = false;
                    tasksFlag = 1;
                }

                else if (String.Equals(userCommand[nextIndex], COMMAND_ARCHIVE, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == 2)
                {
                    startDate = endDate = deadlineDate = null;
                    archiveFlag = true;
                    tasksFlag = 1;
                }

                else if (String.Equals(userCommand[nextIndex], COMMAND_WEEK, StringComparison.CurrentCultureIgnoreCase) && userCommand.Count == 2)
                {
                    DayOfWeek today = DateTime.Now.DayOfWeek;
                    int days = today - DayOfWeek.Monday;
                    DateTime temp_start = DateTime.Now.AddDays(-days);
                    DateTime temp_end = temp_start.AddDays(6);
                    startDate = temp_start;
                    endDate = temp_end;
                    deadlineDate = null;
                    archiveFlag = false;
                    tasksFlag = 1;
                    //Need to make time 12:00 AM?
                }

                if ((tasksFlag == 0) && (currentIndex < userCommand.Count - 1) && (IsValidDate(userCommand[nextIndex])))
                {
                    deadlineDate = DateTime.Parse(userCommand[nextIndex]);
                    startDate = endDate = null;
                    archiveFlag = false;
                    tasksFlag = 1;
                }


                if (tasksFlag == 0)
                {
                    dateKeywordFlag = 0;
                    currentIndex = 1;
                    int enddateflag = 0;

                    foreach (string keyword in COMMAND_TASKS_RANGE)
                    {
                        if (String.Equals(keyword, userCommand[currentIndex], StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                        {
                            nextIndex = currentIndex + 1;
                            dateKeywordFlag = 1;
                        }
                    }
                    if (dateKeywordFlag == 0)
                    {
                        foreach (string keyword in COMMAND_TASKS_ENDING)
                        {
                            if (String.Equals(keyword, userCommand[currentIndex], StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                            {
                                nextIndex = currentIndex + 1;
                                enddateflag = 1;
                            }
                        }
                    }

                    if (dateKeywordFlag != 0)
                    {
                        if (IsDateRange(userCommand[nextIndex]) == 2)
                        {
                            startEndDate.Sort((a, b) => a.CompareTo(b));
                            startDate = DateTime.Parse(startEndDate[0]);
                            endDate = DateTime.Parse(startEndDate[1]);
                            deadlineDate = null;
                            archiveFlag = false;
                            tasksFlag = 1;
                        }
                    }
                    else if (enddateflag == 1)
                    {
                        if (IsValidDate(userCommand[nextIndex]))
                        {
                            endDate = DateTime.Parse(userCommand[nextIndex]);
                            startDate = DateTime.Now;
                            startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                            deadlineDate = null;
                            archiveFlag = false;
                            tasksFlag = 1;
                        }
                    }
                }

                if (tasksFlag == 0)
                {
                    currentIndex = 0;
                    nextIndex = currentIndex + 1;

                    foreach (string keyword in COMMAND_TASKS_DAY)
                    {
                        if (String.Equals(keyword, userCommand[nextIndex], StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                        {
                            if (IsValidDate(userCommand[nextIndex + 1]))
                            {
                                deadlineDate = DateTime.Parse(userCommand[nextIndex + 1]);
                                startDate = endDate = null;
                                archiveFlag = false;
                                tasksFlag = 1;
                            }
                        }
                    }
                }

                if (tasksFlag == 1)
                {
                    Task viewtaskdetails = new Task(0, null, startDate, endDate, deadlineDate, archiveFlag);
                    ViewCommand view = new ViewCommand(fileHandler, viewtaskdetails, screenState);
                    taskHistory.Push(view);
                    return view;
                }


                else
                {
                    return ParseNewTask();
                }
            }
            else
            {
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

            foreach (string keyword in COMMAND_MODIFY)
            {
                if (String.Equals(keyword, userCommand[currentIndex], StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                {
                    checkId = IsValidTaskId(userCommand[nextIndex]);
                    if (checkId > 0)
                    {
                        taskId = checkId;
                        modifyFlag = 1;
                    }
                }
            }

            if (modifyFlag != 0)
            {
                currentIndex = 0;
                int date_count = 0;

                foreach (string str in userCommand)
                {
                    foreach (string keyword in COMMAND_NEW_DATE)
                    {
                        if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                        {
                            if (IsValidDate(userCommand[currentIndex + 1]) && date_count <= 2)
                            {
                                nextIndex = currentIndex + 1;

                                if (String.Equals(str, COMMAND_NEW_DATE[0], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    startDateIndex = currentIndex;
                                    startDate = DateTime.Parse(userCommand[nextIndex]);
                                }
                                if (String.Equals(str, COMMAND_NEW_DATE[1], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    endDateIndex = currentIndex;
                                    endDate = DateTime.Parse(userCommand[nextIndex]);
                                }
                                date_count++; ;
                            }
                        }
                    }
                    currentIndex++;
                }

                if (date_count > 0)
                {
                    dateFlag = 1;
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
                }

                else if (dateFlag == 1)
                {
                    taskIndex = 2;
                    if (String.Equals(userCommand[taskIndex], COMMAND_RANGE, StringComparison.CurrentCultureIgnoreCase))
                    {
                        taskIndex += 1;
                    }

                    if (startDateIndex > 0 && endDateIndex > 0)
                    {
                        if (startDateIndex < endDateIndex)
                        {
                            taskDescription = ConvertToString(userCommand, stringList, taskIndex, startDateIndex - 1);
                        }
                        else
                        {
                            taskDescription = ConvertToString(userCommand, stringList, taskIndex, endDateIndex - 1);
                        }
                    }
                    else if (startDateIndex > 0)
                    {
                        taskDescription = ConvertToString(userCommand, stringList, taskIndex, startDateIndex - 1);
                    }
                    else
                    {
                        taskDescription = ConvertToString(userCommand, stringList, taskIndex, endDateIndex - 1);
                    }

                    if (taskDescription == String.Empty)
                    {
                        taskDescription = null;
                    }
                }

                else
                {
                    taskDescription = null;
                }
                Task taskToEdit = new Task(taskId, taskDescription, startDate, endDate, deadlineDate);
                EditCommand edit = new EditCommand(fileHandler, taskToEdit, screenState);
                taskHistory.Push(edit);
                return edit;
            }
            else
            {
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

            foreach (string str in userCommand)
            {
                foreach (string keyword in COMMAND_DATE)
                {
                    if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                    {
                        nextIndex = currentIndex + 1;
                        previousIndex = currentIndex - 1;
                        dateKeywordFlag = 1;
                    }
                }

                if (dateKeywordFlag != 0)
                {
                    if (IsDateRange(userCommand[nextIndex]) == 2)
                    {
                        deadlineDate = null;
                        startEndDate.Sort((a, b) => a.CompareTo(b));
                        startDate = DateTime.Parse(startEndDate[0]);
                        endDate = DateTime.Parse(startEndDate[1]);
                        dateFlag = 1;
                    }
                    else if (IsValidDate(userCommand[nextIndex]))
                    {
                        deadlineDate = DateTime.Parse(userCommand[nextIndex]);
                        startDate = endDate = null;
                        dateFlag = 1;
                    }
                    else
                    {
                        dateFlag = 0;
                    }
                    if (dateFlag == 1)
                    {
                        taskDescription = ConvertToString(userCommand, stringList, 0, previousIndex);
                    }
                    dateKeywordFlag = 0;
                }
                currentIndex++;
            }

            if (dateFlag == 0)
            {
                taskDescription = inputCommand;
                deadlineDate = startDate = endDate = null;
            }

            taskToAdd = new Task(0, taskDescription, startDate, endDate, deadlineDate);
            AddCommand add = new AddCommand(fileHandler, taskToAdd, screenState);
            taskHistory.Push(add);
            return add;
        }

        /// Checks if a date range is given and extracts the start and end date
        /// </summary>
        /// <param name="checkdate">The string to be parsed and checked</param>
        /// <returns>Returns 2 if valid start and end dates are found</returns>
        private int IsDateRange(string checkdate)
        {
            string[] templist;
            string[] separators = { "-", ",", COMMAND_RANGE };
            int isRange = 0;
            int startdateindex = nextIndex;

            if (startdateindex < (userCommand.Count - 2))
            {
                int enddateindex = startdateindex + 2;

                if (String.Equals(userCommand[startdateindex + 1], COMMAND_RANGE, StringComparison.CurrentCultureIgnoreCase)
                    || String.Equals(userCommand[startdateindex + 1], COMMAND_RANGE_AND, StringComparison.CurrentCultureIgnoreCase)
                    || String.Equals(userCommand[startdateindex + 1], COMMAND_RANGE_ALT))
                {
                    if ((IsValidDate(checkdate)) && (IsValidDate(userCommand[enddateindex])))
                    {
                        startEndDate.Add(checkdate);
                        startEndDate.Add(userCommand[enddateindex]);
                        isRange = DATE_COUNT;
                    }
                    return isRange;
                }
            }

            else if (startdateindex < userCommand.Count - 1)
            {
                int enddateindex = startdateindex + 1;

                if (IsValidDate(userCommand[startdateindex]) && IsValidDate(userCommand[enddateindex]))
                {
                    startEndDate.Add(checkdate);
                    startEndDate.Add(userCommand[enddateindex]);
                    isRange = DATE_COUNT;
                    return isRange;
                }
            }

            else
            {
                templist = checkdate.ToUpper().Split(separators, StringSplitOptions.None);

                foreach (string str in templist)
                {
                    if (IsValidDate(str))
                    {
                        startEndDate.Add(str);
                        isRange++;
                    }
                    else
                    {
                        isRange--;
                    }
                }
                return isRange;
            }
            return isRange;
        }


        /// <summary>
        /// Checks whether the string is a task ID
        /// </summary>
        /// <param name="str">The string to be checked</param>
        /// <returns>Returns true if string is a valid task ID</returns>
        private int IsValidTaskId(string str)
        {
            int taskid = -1;
            if (str[0] != 'T')
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
        /// Checks if the date is a valid date
        /// </summary>
        /// <param name="datestring">A string containing the date information</param>
        /// <returns>Returns true or false depending on whether the date is valid</returns>
        public bool IsValidDate(string datestring)
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
        /// Takes the strings upto the specified index from the list and concatenates them
        /// into a single string
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
            tempstring = String.Join(" ", templist.ToArray());
            return tempstring;
        }
    }
}
