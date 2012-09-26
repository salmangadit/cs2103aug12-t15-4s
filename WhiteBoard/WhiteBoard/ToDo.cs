using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WhiteBoard
{
    class ToDo
    {
        string taskDescription;

        DateTime taskStartTime;
        DateTime taskEndTime;

        DateTime taskDeadline;

        public string TaskDescription
        {
            get
            {
                if (taskDescription == null)
                {
                    return "Task Description is not set";
                    //throw new ArgumentNullException("Task Description is not set");
                }

                return taskDescription;
            }
            set
            {
                this.taskDescription = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                if (taskStartTime == null)
                {
                    throw new ArgumentNullException("Start time is not set");
                }

                return taskStartTime;
            }
            set
            {
                this.taskStartTime = value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                if (taskEndTime == null)
                {
                    throw new ArgumentNullException("Start time is not set");
                }

                return taskEndTime;
            }
            set
            {
                this.taskEndTime = value;
            }
        }

        public DateTime Deadline
        {
            get
            {
                if (taskDeadline == null)
                {
                    throw new ArgumentNullException("Start time is not set");
                }

                return taskDeadline;
            }
            set
            {
                this.taskDeadline = value;
            }
        }

    }
}
