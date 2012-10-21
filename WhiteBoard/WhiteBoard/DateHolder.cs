using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    class DateHolder
    {
        private string[] DAYS_OF_WEEK = { "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "TODAY", "TOMORROW" };
        private string dateString;
        private int dateId;

        public DateHolder(string datestring, int dateid)
        {
            dateString = datestring;
            dateId = dateid;
        }

        public DateTime DateParse()
        {
            if (dateId == 1)
            {
                int i = 0;
                int dayid = -1;
                foreach (string str in DAYS_OF_WEEK)
                {
                    if (String.Equals(dateString, str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        dayid = i;
                    }
                    i++;
                }
                DayOfWeek today = DateTime.Now.DayOfWeek;
                var day = today;
                if (dayid == 7)
                {
                    day = today;
                }
                else if (dayid == 8)
                {
                    day = today + 1;
                }
                else
                {
                    day = (DayOfWeek)dayid;
                }
                var days = day - today;
                int days_diff = (int)days;
                DateTime temp_date = DateTime.Now.AddDays(days_diff);
                if ((days_diff < 0 || days_diff == 0) && dayid != 7)
                {
                    temp_date = temp_date.AddDays(7);
                }
                return temp_date;
            }

            else
            {
                return DateTime.Parse(dateString);
            }
        }
    }
}
