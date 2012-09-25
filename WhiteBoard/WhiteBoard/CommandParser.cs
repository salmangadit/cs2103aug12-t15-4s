using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;

namespace WhiteBoard
{
    class CommandParser
    {
        private string[] COMMAND_COMPARE = { "BY", "ON", "BEFORE", "AT" };

        private string inputCommand;
        private string taskDescription;
        private DateTime dateTime;
        private string[] userCommandArray;
        private FileHandler fileHandler;
        private ToDo taskToAdd = new ToDo();

        private List<string> taskDescriptionList = new List<string>();
        private List<string> userCommand = new List<string>();

        public CommandParser( string usercommand, FileHandler filehandler)
        {
            inputCommand = usercommand;
            fileHandler = filehandler;
            userCommandArray = inputCommand.Split(' ');
            foreach (string str in userCommandArray)
            {
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
            int keywordflag = 0;
            int index = 0;
            int next = 0;
            int previous = 0;
            
            foreach (string str in userCommand)
            {
                foreach (string keyword in COMMAND_COMPARE)
                {
                    if(String.Equals(keyword, str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        next = index + 1;
                        previous = next - 2;
                        keywordflag = 1;
                    }
                }

                if(keywordflag!=0)
                {
                    if (CheckDate(userCommand[next].ToString()))
                    {
                        dateTime = DateTime.Parse(userCommand[next].ToString());
                        taskDescription = ConvertToString(userCommand, taskDescriptionList, previous);
                        keywordflag = 0;
                    }
                }
                index++;
            }

            taskToAdd.Deadline = dateTime;
            taskToAdd.TaskDesciption = taskDescription;
            AddCommand addcommand = new AddCommand(fileHandler, taskToAdd);
            return addcommand;
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
            tempstring = String.Join(" ",templist.ToArray());
            return tempstring;
        }

        /// <summary>
        /// Checks if the date is a valid date
        /// </summary>
        /// <param name="datestring">A string containing the date information</param>
        /// <returns>Returns true or false depending on whether the date is valid</returns>
        private bool CheckDate(string datestring)
        {
            string[] dateformat = {"dd/mm/yyyy","dd-mm-yyyy"};
            DateTime datetime;
            bool isdate = false;
            foreach (string format in dateformat)
            {
                if (!isdate)
                {
                    isdate = DateTime.TryParseExact(datestring, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime);
                }
            }
            return isdate;
        }
    }
}
