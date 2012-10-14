using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace WhiteBoard
{
    class FileHandler
    {
        string filePath;

        public FileHandler()
        {
            string fileName = "TasksList.xml";

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

        internal void AddTaskToFile(Task taskToAdd)
        {
            List<Task> listOfAllTasks = new List<Task>();
            int lastTaskIndex;
            int newTaskId = 1;

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // if file isn't empty deserialize first
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                lastTaskIndex = listOfAllTasks.Count - 1;
                newTaskId = listOfAllTasks[lastTaskIndex].Id + 1;
            }
            objStrRead.Close();

            taskToAdd.Id = newTaskId;
            listOfAllTasks.Add(taskToAdd);
            StreamWriter objStrWrt = new StreamWriter(filePath);
            try
            {
                objXmlSer.Serialize(objStrWrt, listOfAllTasks);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Unable to serialize null values");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("There was an error generating the XML document");
            }
            objStrWrt.Close();  
        }

        internal Task GetTaskFromFile(int editedTaskId)
        {
            Task taskToBeEdited = new Task();
            List<Task> listOfAllTasks = new List<Task>();
            Debug.Assert(editedTaskId > 0, "Task Id needs to be greater than 0");

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // if file isn't empty deserialize first
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach(Task t in listOfAllTasks)
                {
                    if(t.Id == editedTaskId)
                    {
                        taskToBeEdited = t;
                        break;
                    }
                }
            }
            objStrRead.Close();
            return taskToBeEdited;
        }

        internal bool WriteEditedTaskToFile(Task editedTask)
        {
            bool edited = false;
            List<Task> listOfAllTasks = new List<Task>();
            int indexOfEditedTask;

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // if file isn't empty deserialize first
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
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                objXmlSer.Serialize(objStrWrt, listOfAllTasks);
                objStrWrt.Close();
            }
            return edited;
        }

        internal bool DeleteTaskFromFile(int deletedTaskId)
        {
            bool deleted = false;
            List<Task> listOfTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // If file is not empty
            {
                listOfTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfTasks)
                {
                    if (t.Id == deletedTaskId)
                    {
                        listOfTasks.Remove(t);
                        deleted = true;
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                objXmlSer.Serialize(objStrWrt, listOfTasks);
                objStrWrt.Close();
            }
            // If file is already empty it will return false. Is that ok?
            return deleted;
        }

        internal bool ArchiveTaskInFile(int archivedTaskId)
        {
            bool archived = false;
            List<Task> listOfTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // If file is not empty
            {
                listOfTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach(Task t in listOfTasks)
                {
                    if(t.Id == archivedTaskId)
                    {
                        t.Archive = true;
                        archived = true;
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                objXmlSer.Serialize(objStrWrt, listOfTasks);
                objStrWrt.Close();
            }

            return archived;
        }

        internal bool UnarchiveTaskInFile(int unarchivedTaskId)
        {
            bool unarchived = false;
            List<Task> listOfTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // If file is not empty
            {
                listOfTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfTasks)
                {
                    if (t.Id == unarchivedTaskId)
                    {
                        t.Archive = false;
                        unarchived = true;
                        break;
                    }
                }
                objStrRead.Close();
                StreamWriter objStrWrt = new StreamWriter(filePath);
                objXmlSer.Serialize(objStrWrt, listOfTasks);
                objStrWrt.Close();
            }

            return unarchived;
        }

        internal List<Task> ViewAll()
        {
            List<Task> listOfAllTasks = new List<Task>();
            List<Task> listOfNonArchivedTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // If file is not empty
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if (t.Archive == false)
                    {
                        listOfNonArchivedTasks.Add(t);
                    }
                }
                objStrRead.Close();
            }

            return listOfNonArchivedTasks; // will return empty list if file is empty or no non-archived tasks
        }

        internal List<Task> ViewArchive()
        {
            List<Task> listOfAllTasks = new List<Task>();
            List<Task> listOfArchivedTasks = new List<Task>();
            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // If file is not empty
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

            return listOfArchivedTasks; // Will return empty list if file is empty or if file contains no archived tasks
        }

        internal List<Task> ViewTasks(DateTime? startDate, DateTime? endDate)
        {
            List<Task> listOfAllTasks = new List<Task>();
            List<Task> listOfTasksWithinRange = new List<Task>();

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // If file is not empty
            {
                listOfAllTasks = (List<Task>)objXmlSer.Deserialize(objStrRead);
                foreach (Task t in listOfAllTasks)
                {
                    if (!t.Archive)
                    {
                        if((t.StartTime.HasValue) && (t.StartTime >= startDate) && (t.StartTime <= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }

                        else if ((t.EndTime.HasValue) && (t.EndTime >= startDate) && (t.EndTime <= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }

                        else if ((t.Deadline.HasValue) && (t.Deadline >= startDate) && (t.Deadline <= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }

                        else if ((t.StartTime.HasValue) && (t.StartTime <= startDate) && (t.EndTime.HasValue) && (t.EndTime >= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }

                        else if ((t.StartTime.HasValue) && (t.StartTime <= startDate) && (t.Deadline.HasValue) && (t.Deadline >= endDate))
                        {
                            listOfTasksWithinRange.Add(t);
                        }
                    }  
                }
            }
            objStrRead.Close();
            return listOfTasksWithinRange;
        }
    }
}
