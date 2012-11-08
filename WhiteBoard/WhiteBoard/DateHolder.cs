﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    //@author U095159L
    class DateHolder
    {
        private string dateString;                      //String containing the date
        private int dateId;                             //The ID denoting whether the date is in day or date form

        public DateHolder(string datestring, int dateid)
        {
            dateString = datestring;
            dateId = dateid;
        }

        public DateTime ConvertToDateTime()
        {
            if (dateId == 1)                            //If date is in a day form
            {
                int i = 0;
                int dayid = -1;
                int days_difference;
                DayOfWeek today = DateTime.Now.DayOfWeek;
                var day = today;
                foreach (string str in Constants.DAYS_OF_WEEK)
                {
                    if (String.Equals(dateString, str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        dayid = i;
                    }
                    i++;
                }
                if (dayid == Constants.TODAY_ID)
                {
                    day = today;
                }
                else if (dayid == Constants.TOMORROW_ID)
                {
                    day = today + 1;
                }
                else
                {
                    day = (DayOfWeek)dayid;
                }
                var days = day - today;
                days_difference = (int)days;
                DateTime temp_date = DateTime.Now.AddDays(days_difference);
                if ((days_difference < 0 || days_difference == 0) && dayid != 7)
                {
                    temp_date = temp_date.AddDays(7);   //Denotes that the day belongs to the following week
                }
                return temp_date;
            }

            else
            {                                           //Otherwise parse the date in a DateTime format
                try
                {
                    var cultureinfo = new System.Globalization.CultureInfo("en-gb");
                    DateTime.Parse(dateString, cultureinfo, System.Globalization.DateTimeStyles.None);
                    return DateTime.Parse(dateString);
                }
                catch (FormatException e)
                {
                    throw e;
                }
            }
        }
    }
}
