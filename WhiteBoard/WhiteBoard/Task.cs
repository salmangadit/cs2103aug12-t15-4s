using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace WhiteBoard
{
    public class Task
    {
        int taskId = 0;
        string taskDescription;

        DateTime? taskStartTime;
        DateTime? taskEndTime;
        bool archive;

        public Task()
        {
        }

        public Task(int taskId = 0, string taskDescription = null, DateTime? taskStartTime = null, DateTime? taskEndTime = null, bool archive=false)
        {
            this.taskId = taskId;
            this.taskDescription = taskDescription;
            this.taskStartTime = taskStartTime;
            this.taskEndTime = taskEndTime;
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
                    //return "Task Description is not set";
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
                    //throw new ArgumentNullException("Start time is not set");
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
                    //throw new ArgumentNullException("Start time is not set");
                }

                return taskEndTime;
            }

            set
            {
                this.taskEndTime = value;
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

        public bool Floating
        {
            get
            {
                if (taskEndTime == null)
                    return false;
                else
                    return true;
            }
        }

        public Brush Background
        {
            get
            {
                if (taskEndTime == null && taskStartTime == null)
                    return NormalColor();
                else if (taskEndTime != null)
                {
                    if (DateTime.Now > taskEndTime)
                        return OverdueColor();
                    else
                        return NormalColor();
                }
                else if (taskEndTime == null && taskStartTime != null)
                {
                    if (DateTime.Now > taskStartTime)
                        return OverdueColor();
                    else
                        return NormalColor();
                }

                return NormalColor();
            }
        }

        private Brush OverdueColor()
        {
            // Create a diagonal linear gradient with four stops.   
            LinearGradientBrush myLinearGradientBrush =
                new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 0);
            myLinearGradientBrush.EndPoint = new Point(0, 1);
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.White, 0.0));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.LightGray, 1));

            return myLinearGradientBrush;
        }

        private Brush NormalColor()
        {
            // Create a diagonal linear gradient with four stops.   
            LinearGradientBrush myLinearGradientBrush =
                new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 0);
            myLinearGradientBrush.EndPoint = new Point(0, 2);
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.White, 0.0));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.LightGray, 2));

            return myLinearGradientBrush;
        }

        public bool Overdue
        {
            get
            {
                if (taskEndTime == null && taskStartTime == null)
                    return false;
                else if (taskEndTime != null)
                {
                    if (DateTime.Now > taskEndTime)
                        return true;
                    else
                        return false;
                }
                else if (taskEndTime == null && taskStartTime != null)
                {
                    if (DateTime.Now > taskStartTime)
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }
    }
}
