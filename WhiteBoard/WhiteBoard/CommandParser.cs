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

        protected string inputCommand;
        public string taskDescription = null;
        public DateTime? deadlineDate = null;
        public DateTime? startDate = null;
        public DateTime? endDate = null;
        protected string[] userCommandArray;
        private FileHandler fileHandler;
        private Task taskToAdd;

        protected int keywordFlag = 0;
        protected int dateFlag = 0;
        protected int currentIndex = 0;
        protected int nextIndex = 0;
        protected int previousIndex = 0;

        private List<string> taskDescriptionList = new List<string>();
        private List<string> startEndDate = new List<string>();
        protected List<string> userCommand = new List<string>();

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
        /// Parses the user command and determines the date and the taskdescription
        /// </summary>
        /// <returns>Returns a command object with details of the ToDo item</returns>
        public Command ParseCommand()
        {
            foreach (string str in userCommand)
            {
                foreach (string keyword in COMMAND_DATE)
                {
                    if (String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase) && currentIndex < userCommand.Count() - 1)
                    {
                        nextIndex = currentIndex + 1;
                        previousIndex = nextIndex - 2;
                        keywordFlag = 1;
                    }
                }

                if (keywordFlag != 0)
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
                        taskDescription = ConvertToString(userCommand, taskDescriptionList, previousIndex);
                    }
                    keywordFlag = 0;
                }
                currentIndex++;
            }

            if (dateFlag == 0)
            {
                taskDescription = inputCommand;
                deadlineDate = startDate = endDate = null;
            }

                taskToAdd = new Task(0, taskDescription, startDate, endDate, deadlineDate);
                AddCommand add = new AddCommand(fileHandler, taskToAdd);
                return add;
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
        private string ConvertToString(List<string> list, List<string> templist, int index)
        {
            string tempstring;
            templist.Clear();
            for (int i = 0; i <= index; ++i)
            {
                templist.Add(list[i]);
            }
            tempstring = String.Join(" ", templist.ToArray());
            return tempstring;
        }
    }
}
