using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace WhiteBoard
{
    //@author U096089W
    /// <summary>
    /// Task class to store information about the users tasks
    /// </summary>
    public class Task
    {
        #region Private Fields
        private int taskId;
        private string taskDescription;

        private DateTime? taskStartTime;
        private DateTime? taskEndTime;
        private bool archive;
        #endregion

        #region Constructors
        public Task()
        {
        }

        public Task(int taskId = 0, string taskDescription = null, DateTime? taskStartTime = null, DateTime? taskEndTime = null, bool archive = false)
        {
            this.taskId = taskId;
            this.taskDescription = taskDescription;
            this.taskStartTime = taskStartTime;
            this.taskEndTime = taskEndTime;
            this.archive = archive;
        }
        #endregion

        #region Public Properties
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

        public bool DueToday
        {
            get
            {
                if (taskEndTime == null && taskStartTime == null)
                    return false;
                else if (taskEndTime != null)
                {
                    if ((DateTime.Now <= taskEndTime) && (DateTime.Now >= taskStartTime) )
                        return true;
                    else
                        return false;
                }
                else if (taskEndTime == null && taskStartTime != null)
                {
                    if (DateTime.Now == taskStartTime)
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }
        #endregion

        #region Private Class Helper Methods
        private Brush OverdueColor()
        {
            // Create a diagonal linear gradient with four stops.   
            LinearGradientBrush myLinearGradientBrush =
                new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(Constants.TASK_OVERDUE_START_X, Constants.TASK_OVERDUE_START_Y);
            myLinearGradientBrush.EndPoint = new Point(Constants.TASK_OVERDUE_END_X, Constants.TASK_OVERDUE_END_Y);
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.White, Constants.TASK_OVERDUE_GRADIENT_WHITE));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.LightGray, Constants.TASK_OVERDUE_GRADIENT_LIGHT_GRAY));

            return myLinearGradientBrush;
        }

        private Brush NormalColor()
        {
            // Create a diagonal linear gradient with four stops.   
            LinearGradientBrush myLinearGradientBrush =
                new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(Constants.TASK_NORMAL_START_X, Constants.TASK_NORMAL_START_Y);
            myLinearGradientBrush.EndPoint = new Point(Constants.TASK_NORMAL_END_X, Constants.TASK_NORMAL_END_Y);
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.White, Constants.TASK_NORMAL_GRADIENT_WHITE));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.LightGray, Constants.TASK_NORMAL_GRADIENT_LIGHT_GRAY));

            return myLinearGradientBrush;
        }
        #endregion
    }
}
