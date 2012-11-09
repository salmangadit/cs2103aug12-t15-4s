using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using log4net;

namespace WhiteBoard
{
    //@author U094776M
    delegate void FileUpdate(UpdateType update, Task task, Task uneditedTask);

    class FileHandler
    {
        #region Private Fields

        protected static readonly ILog log = LogManager.GetLogger(typeof(FileHandler));
        private static FileHandler instance;
        private event FileUpdate updateEvent;
        string filePath;

        #endregion

        #region Constructors

        private FileHandler()
        {
            string fileName = Constants.FILENAME;

            // set file path, we use the current Directory for the user and specified file name
            filePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + fileName;

            // check if file exists and create one if need be
            FileStream fs = null;
            if (!File.Exists(filePath))
            {
                using (fs = File.Create(filePath))
                {

                }
            }
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Implementing FileHandler as singleton
        /// </summary>
        public static FileHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileHandler();
                }

                return instance;
            }
        }

        #endregion

        #region Public Class Methods

        /// <summary>
        /// Add task to xml file by serializing the task object to the xml file
        /// </summary>
        /// <param name="taskToAdd"> Task object containing task id, description, start time, end time and bool archive.</param>
        /// <returns> True if task has been successfully added </returns>
        internal bool AddTaskToFile(Task taskToAdd)
        {
            log.Debug("Task going to be added");
            bool taskAdded = false;
            List<Task> listOfAllTasks = new List<Task>();
            int lastTaskIndex;
            int newTaskId = 1;

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);

            // if file isn't empty deserialize first
            if (objStrRead.Peek() >= 0)
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                lastTaskIndex = listOfAllTasks.Count - 1;
                if (lastTaskIndex >= 0)
                {
                    newTaskId = listOfAllTasks[lastTaskIndex].Id + 1;
                }
            }
            objStrRead.Close();

            taskToAdd.Id = newTaskId;
            listOfAllTasks.Add(taskToAdd);
            taskAdded = true;
            StreamWriter objStrWrt = new StreamWriter(filePath);
            try
            {
                objXmlSer.Serialize(objStrWrt, listOfAllTasks);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine(Constants.UNABLE_TO_SERIALIZE);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine(Constants.ERROR_GENERATING_DOC);
            }
            objStrWrt.Close();
            Notify(UpdateType.Add, taskToAdd);
            log.Debug("Task added");
            return taskAdded;
        }

        /// <summary>
        /// Add a task to the file by serializing the task object to the xml file. This method is called when the user has to undo a delete command.
        /// </summary>
        /// <param name="taskToAdd"> Task object containing task id, description, start time, end time and bool archive. </param>
        /// <param name="taskId"> Task Id of the task that has to be re-added</param>
        /// <returns> True if task has been successfully added</returns>

        internal bool AddTaskToFile(Task taskToAdd, int taskId)
        {
            List<Task> listOfAllTasks = new List<Task>();
            bool taskAdded = false;
            Debug.Assert(taskId > 0, "Task Id should be a positive number!");
            taskToAdd.Id = taskId;
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));
            try
            {
                StreamReader objStrRead = new StreamReader(filePath);
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                objStrRead.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }

            // If no tasks in file
            if (listOfAllTasks.Count == 0)
            {
                listOfAllTasks.Add(taskToAdd);
                taskAdded = true;
            }
            // If there are tasks in file
            else if (listOfAllTasks.Count > 0)
            {
                int indexOfTaskToAdd;
                int lastTaskIndex = listOfAllTasks.Count - 1;
                // If inserting at the top of the list
                if (taskId < listOfAllTasks[0].Id)
                {
                    indexOfTaskToAdd = 0;
                    listOfAllTasks.Insert(indexOfTaskToAdd, taskToAdd);
                    taskAdded = true;
                }

                for (int index = 0; index < (listOfAllTasks.Count - 1); index++)
                {
                    // If inserting at the middle of the list
                    if ((taskId > listOfAllTasks[index].Id) && (taskId < listOfAllTasks[index + 1].Id))
                    {
                        indexOfTaskToAdd = index + 1;
                        listOfAllTasks.Insert(indexOfTaskToAdd, taskToAdd);
                        taskAdded = true;
                        break;
                    }
                }

                // If inserting at the end of the list
                if (taskId > listOfAllTasks[lastTaskIndex].Id)
                {
                    listOfAllTasks.Add(taskToAdd);
                    taskAdded = true;
                }
            }
            StreamWriter objStrWrt = new StreamWriter(filePath);
            try
            {
                objXmlSer.Serialize(objStrWrt, listOfAllTasks);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine(Constants.UNABLE_TO_SERIALIZE);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine(Constants.ERROR_GENERATING_DOC);
            }

            objStrWrt.Close();
            Notify(UpdateType.Add, taskToAdd);
            return taskAdded;
        }

        /// <summary>
        /// Retrieve a task object from file for editing it by deserializing the entire xml file into a list of task objects and then matching the task id of the required task.
        /// </summary>
        /// <param name="editedTaskId"> Task id of the task that has to be retrieved for editing</param>
        /// <returns> Returns the task object to be edited</returns>

        internal Task GetTaskFromFile(int editedTaskId)
        {
            log.Debug("Going to get task from file for editing");
            Task taskToBeEdited = new Task();
            List<Task> listOfAllTasks = new List<Task>();
            Debug.Assert(editedTaskId > 0, "Task Id should be a positive number!");

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            // if file isn't empty deserialize first
            if (objStrRead.Peek() >= 0)
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if (t.Id == editedTaskId)
                    {
                        taskToBeEdited = t;
                        break;
                    }
                }
            }
            objStrRead.Close();
            return taskToBeEdited;
        }

        /// <summary>
        /// Write back the edited task to the file by serializing the edited task object.
        /// </summary>
        /// <param name="editedTask"> Edited task object</param>
        /// <returns> True if edited task has been successfully written to the xml file </returns>

        internal bool WriteEditedTaskToFile(Task editedTask)
        {
            bool edited = false;
            List<Task> listOfAllTasks = new List<Task>();
            int indexOfEditedTask;

            Task uneditedTask = null;

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            // if file isn't empty deserialize first
            if (objStrRead.Peek() >= 0)
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if (t.Id == editedTask.Id)
                    {
                        indexOfEditedTask = listOfAllTasks.IndexOf(t);
                        listOfAllTasks.Remove(t);
                        listOfAllTasks.Insert(indexOfEditedTask, editedTask);
                        edited = true;
                        uneditedTask = t;
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                try
                {
                    objXmlSer.Serialize(objStrWrt, listOfAllTasks);
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine(Constants.UNABLE_TO_SERIALIZE);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine(Constants.ERROR_GENERATING_DOC);
                }
                objStrWrt.Close();
            }

            if (edited)
            {
                Notify(UpdateType.Edit, editedTask, uneditedTask);
            }
            return edited;
        }

        /// <summary>
        /// Delete a task from the xml file by deserializing the entire file into a list of task objects and then removing it from the list. The list of objects is then serialized into the xml file.
        /// </summary>
        /// <param name="deletedTaskId"> Task Id of the task object to be deleted </param>
        /// <returns></returns>

        internal bool DeleteTaskFromFile(int deletedTaskId)
        {
            log.Debug("Task going to be deleted");
            bool deleted = false;
            List<Task> listOfTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));
            Debug.Assert(deletedTaskId > 0, "Deleted Task Id should be a positive number!");

            Task deletedTask = null;

            StreamReader objStrRead = new StreamReader(filePath);
            // If file is not empty
            if (objStrRead.Peek() >= 0)
            {
                listOfTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfTasks)
                {
                    if (t.Id == deletedTaskId)
                    {
                        listOfTasks.Remove(t);
                        deleted = true;
                        deletedTask = t;
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                objXmlSer.Serialize(objStrWrt, listOfTasks);
                objStrWrt.Close();
            }
            else
            {
                throw new SystemException(Constants.ERROR_FILE_EMPTY);
            }

            if (deleted)
            {
                Notify(UpdateType.Delete, deletedTask);
                log.Debug("Task deleted");
            }

            return deleted;
        }

        /// <summary>
        /// Archive a task in file by setting the archive flag to true.
        /// </summary>
        /// <param name="archivedTaskId"> Task Id of the task object to be archived</param>
        /// <returns> True if successfully archived</returns>

        internal bool ArchiveTaskInFile(int archivedTaskId)
        {
            log.Debug("Task going to be archived");
            bool archived = false;
            List<Task> listOfTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));
            Debug.Assert(archivedTaskId > 0, "Archived Task Id should be a positive number");

            Task archivedTask = null;

            StreamReader objStrRead = new StreamReader(filePath);

            // If file is not empty
            if (objStrRead.Peek() >= 0)
            {
                listOfTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfTasks)
                {
                    if (t.Id == archivedTaskId)
                    {
                        t.Archive = true;
                        archived = true;
                        archivedTask = t;
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                objXmlSer.Serialize(objStrWrt, listOfTasks);
                objStrWrt.Close();
            }
            else
            {
                throw new SystemException(Constants.ERROR_FILE_EMPTY);
            }

            if (archived)
            {
                Notify(UpdateType.Archive, archivedTask);
                log.Debug("Task archived");
            }

            return archived;
        }

        /// <summary>
        /// Unarchive a task in the xml file by setting the archive flag of the task to false
        /// </summary>
        /// <param name="unarchivedTaskId"> Task Id of the task to be unarchived</param>
        /// <returns> True if task has been unarchived successfully</returns>

        internal bool UnarchiveTaskInFile(int unarchivedTaskId)
        {
            bool unarchived = false;
            List<Task> listOfTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));
            Debug.Assert(unarchivedTaskId > 0, "Unarchived Task Id should be a positive number!");

            Task unarchivedTask = null;

            StreamReader objStrRead = new StreamReader(filePath);
            // If file is not empty
            if (objStrRead.Peek() >= 0)
            {
                listOfTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfTasks)
                {
                    if (t.Id == unarchivedTaskId)
                    {
                        t.Archive = false;
                        unarchived = true;
                        unarchivedTask = t;
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                objXmlSer.Serialize(objStrWrt, listOfTasks);
                objStrWrt.Close();
            }
            else
            {
                throw new SystemException(Constants.ERROR_FILE_EMPTY);
            }

            if (unarchived)
            {
                Notify(UpdateType.Unarchive, unarchivedTask);
            }

            return unarchived;
        }

        /// <summary>
        /// Deserializes the entire xml file into a list of task objects. Puts the unarchived tasks into a separate list.
        /// </summary>
        /// <returns> A list of unarchived task objects sorted by date </returns>

        internal List<Task> ViewAll()
        {
            List<Task> listOfAllTasks = new List<Task>();
            List<Task> listOfNonArchivedTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            // If file is not empty
            if (objStrRead.Peek() >= 0)
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if (t.Archive == false)
                    {
                        listOfNonArchivedTasks.Add(t);
                    }
                }             
            }
            objStrRead.Close();
            listOfNonArchivedTasks = listOfNonArchivedTasks.OrderBy(x => x.StartTime).ToList();
            return listOfNonArchivedTasks;
        }

        /// <summary>
        /// Deserializes the entire xml file into a list of task objects. Puts the archived tasks into a separate list.
        /// </summary>
        /// <returns> A list of archived task objects sorted by date</returns>

        internal List<Task> ViewArchive()
        {
            List<Task> listOfAllTasks = new List<Task>();
            List<Task> listOfArchivedTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);

            // If file is not empty
            if (objStrRead.Peek() >= 0)
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if (t.Archive == true)
                    {
                        listOfArchivedTasks.Add(t);
                    }
                }
                objStrRead.Close();
            }

            listOfArchivedTasks = listOfArchivedTasks.OrderBy(x => x.StartTime).ToList();
            return listOfArchivedTasks;
        }

        /// <summary>
        /// Deserializes the entire xml file into a list of task objects. Puts the tasks for the date in a separate list.
        /// </summary>
        /// <param name="date"> The date for which tasks on it need to be viewed by the user</param>
        /// <returns> List of task objects for that day sorted by date</returns>

        internal List<Task> ViewTasks(DateTime? date)
        {
            List<Task> listOfAllTasks = new List<Task>();
            List<Task> listOfTasksForTheDay = new List<Task>();

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            // If file is not empty
            if (objStrRead.Peek() >= 0)
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if ((!t.Archive) && (t.StartTime != null) && (t.EndTime != null))
                    {
                        if ((t.StartTime.Value.Date <= date.Value.Date) && (t.EndTime.Value.Date >= date.Value.Date))
                        {
                            listOfTasksForTheDay.Add(t);
                        }
                    }

                    else if ((!t.Archive) && (t.StartTime != null))
                    {
                        if (t.StartTime.Value.Date == date.Value.Date)
                        {
                            listOfTasksForTheDay.Add(t);
                        }
                    }

                    else if ((!t.Archive) && (t.EndTime != null))
                    {
                        if (t.EndTime.Value.Date == date.Value.Date)
                        {
                            listOfTasksForTheDay.Add(t);
                        }
                    }
                }
            }
            objStrRead.Close();
            listOfTasksForTheDay = listOfTasksForTheDay.OrderBy(x => x.StartTime).ToList();
            return listOfTasksForTheDay;
        }

        /// <summary>
        /// Deserializes the entire xml file into a list of task objects. Puts the tasks within the date range in a separate list.
        /// </summary>
        /// <param name="startDate"> start date of the range</param>
        /// <param name="endDate">end date of the range</param>
        /// <returns> A list of tasks objects that fall within the range sorted by date</returns>

        internal List<Task> ViewTasks(DateTime? startDate, DateTime? endDate)
        {
            List<Task> listOfAllTasks = new List<Task>();
            List<Task> listOfTasksWithinRange = new List<Task>();

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            // If file is not empty
            if (objStrRead.Peek() >= 0)
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if (!t.Archive)
                    {
                        if ((t.StartTime.HasValue) && (t.StartTime >= startDate) && (t.StartTime <= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }

                        else if ((t.EndTime.HasValue) && (t.EndTime >= startDate) && (t.EndTime <= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }
                        else if ((t.StartTime.HasValue) && (t.StartTime <= startDate) && (t.EndTime.HasValue) && (t.EndTime >= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }
                    }
                }
            }
            objStrRead.Close();
            listOfTasksWithinRange = listOfTasksWithinRange.OrderBy(x => x.StartTime).ToList();
            return listOfTasksWithinRange;
        }

        #endregion

        #region Event Handlers

        public event FileUpdate FileUpdateEvent
        {
            add
            {
                updateEvent += value;
            }
            remove
            {
                updateEvent -= value;
            }
        }

        #endregion

        #region Private Class Helper Methods

        private void Notify(UpdateType update, Task task, Task uneditedTask = null)
        {
            if (update == UpdateType.Edit && (task == null || uneditedTask == null))
            {
                throw new ArgumentNullException("Task not set for Edit");
            }

            if (task == null)
            {
                throw new ArgumentNullException("Task not set");
            }

            updateEvent(update, task, uneditedTask);
        }

        #endregion

    }
}
