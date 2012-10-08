using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace WhiteBoard
{
    class CommandParser
    {
        private string[] COMMAND_DATE = { "BY", "ON", "BEFORE", "AT", "FROM", "BETWEEN" };
        private string[] COMMAND_MODIFY = { "MODIFY", "CHANGE", "UPDATE" };
        private const int DATE_COUNT = 2;
        private const string COMMAND_RANGE = "TO";
        private const string COMMAND_RANGE_ALT = "-";

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

        private int dateKeywordFlag = 0;
        private int modifyKeywordFlag = 0;
        private int dateFlag = 0;
        private int currentIndex = 0;
        private int nextIndex = 0;
        private int previousIndex = 0;

        private List<string> taskDescriptionList = new List<string>();
        private List<string> startEndDate = new List<string>();
        private List<string> userCommand = new List<string>();

        public CommandParser(string usercommand, FileHandler filehandler)
        {
            inputCommand = usercommand;
            fileHandler = filehandler;
            inputCommand = Regex.Replace(inputCommand, @"\s+", " ");
            userCommandArray = inputCommand.Split(' ');
            foreach (string str in userCommandArray)
            {
                str.Trim();
                userCommand.Add(str);
            }
            ParseCommand();
        }

        /// <summary>
        /// Parses the user command and determines the action to be done i.e Add, Modify, Delete etc.
        /// </summary>
        /// <returns>Returns a command object with details of the ToDo item</returns>
        public Command ParseCommand()
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
                        modifyKeywordFlag = 1;
                    }
                }
            }

            if (modifyKeywordFlag != 0)
            {
                if (nextIndex < userCommand.Count - 1)
                {
                    if (String.Equals(userCommand[nextIndex + 1], COMMAND_RANGE, StringComparison.CurrentCultureIgnoreCase))
                    {
                        taskDescription = ConvertToString(userCommand, taskDescriptionList, nextIndex + 2, userCommand.Count - 1);
                    }
                    else
                    {
                        taskDescription = ConvertToString(userCommand, taskDescriptionList, nextIndex + 1, userCommand.Count - 1);

                    }
                }

                else
                {
                    taskDescription = null;
                }

                Task taskToEdit = new Task(taskId, taskDescription, startDate, endDate, deadlineDate);
                EditCommand edit = new EditCommand(fileHandler, taskToEdit);
                return edit;
            }
            else
            {
                ParseDate();
                taskToAdd = new Task(0, taskDescription, startDate, endDate, deadlineDate);
                AddCommand add = new AddCommand(fileHandler, taskToAdd);
                return add;
            }

        }

        /// <summary>
        /// Parses the user command and determines the date and the taskdescription
        /// </summary>
        private void ParseDate()
        {
            foreach (string str in userCommand)
            {
                foreach (string keyword in COMMAND_DATE)
                {
                    if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                    {
                        nextIndex = currentIndex + 1;
                        previousIndex = nextIndex - 2;
                        dateKeywordFlag = 1;
                    }
                }

                if (dateKeywordFlag != 0)
                {
                    if (IsDateRange(userCommand[nextIndex]) == 2)
                    {
                        startEndDate.Sort((a, b) => a.CompareTo(b));
                        startDate = DateTime.Parse(startEndDate[0]);
                        endDate = DateTime.Parse(startEndDate[1]);
                        dateFlag = 1;
                    }
                    else if (IsValidDate(userCommand[nextIndex]))
                    {
                        deadlineDate = DateTime.Parse(userCommand[nextIndex]);
                        dateFlag = 1;
                    }
                    else
                    {
                        dateFlag = 0;
                    }
                    if (dateFlag == 1)
                    {
                        taskDescription = ConvertToString(userCommand, taskDescriptionList, 0, previousIndex);
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
        }

        /// <summary>
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
