using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WhiteBoard
{
    public class Task
    {
        int taskId = 0;
        string taskDescription;

        DateTime? taskStartTime;
        DateTime? taskEndTime;
        DateTime? taskDeadline;
        bool archive;

        public Task()
        {
        }

        public Task(int taskId = 0, string taskDescription = null, DateTime? taskStartTime = null, DateTime? taskEndTime = null, DateTime? taskDeadline = null, bool archive=false)
        {
            this.taskId = taskId;
            this.taskDescription = taskDescription;
            this.taskStartTime = taskStartTime;
            this.taskEndTime = taskEndTime;
            this.taskDeadline = taskDeadline;
            this.archive = archive;
        }

        public int Id
        {
            get
            {
                if (taskId == 0)
                {
                    throw new ArgumentNullException("Task ID is not set");
                }

                return taskId;
            }

            set
            {
                this.taskId = value;
            }

        }

        public string Description
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

        public DateTime? StartTime
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

        public DateTime? EndTime
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

        public DateTime? Deadline
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

        public bool Archive
        {
            get
            {
                return archive;
            }

            set
            {
                this.archive = value;
            }
        }

    }
}
