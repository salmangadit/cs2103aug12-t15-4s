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
            int newTaskId;
            int indexOfLastTask;
            string lastTaskIdString;
            string newTaskIdString;
            int indexOfNewTask;
            List<Task> listOfTasksToAdd = new List<Task>();

            XmlSerializer objXmlSer = new XmlSerializer(typeof(List<Task>));

            StreamReader objStrRead = new StreamReader(filePath);
            if (objStrRead.Peek() >= 0) // if file isn't empty deserialize first
            {
                listOfTasksToAdd = (List<Task>)objXmlSer.Deserialize(objStrRead);
            }

            objStrRead.Close();
            listOfTasksToAdd.Add(taskToAdd);

            StreamWriter objStrWrt = new StreamWriter(filePath);
            objXmlSer.Serialize(objStrWrt, listOfTasksToAdd);
            objStrWrt.Close();

            // Clear list
            listOfTasksToAdd.Clear();

            XmlDocument taskListDoc = new XmlDocument();

            try
            {
                taskListDoc.Load(filePath);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex);
            }

            XmlElement rootElement = taskListDoc.DocumentElement; // Get reference to root node
            XmlNodeList listOfTasks = taskListDoc.GetElementsByTagName("taskId"); // Create a list of nodes whose name is taskId

            if (listOfTasks.Count > 0)
            {
                indexOfLastTask = listOfTasks.Count - 2;
                lastTaskIdString = listOfTasks[indexOfLastTask].Value; // Get the value of the last task
                newTaskId = int.Parse(lastTaskIdString) + 1; // Increase value
            }

            else
            {
                newTaskId = 1;
            }

            newTaskIdString = newTaskId.ToString(); // Convert back to string
            indexOfNewTask = listOfTasks.Count - 1;

            if (listOfTasks[indexOfNewTask].Value == "0")
            {
                listOfTasks[indexOfNewTask].RemoveAll();
                XmlText taskIdValue = taskListDoc.CreateTextNode(newTaskIdString);
                listOfTasks[indexOfNewTask].AppendChild(taskIdValue);
            }

            taskListDoc.Save(filePath);
            
        }

        internal Task GetTaskFromFile(int editedTaskId)
        {
            Task taskToBeEdited = new Task();
            Debug.Assert(editedTaskId > 0, "Task Id needs to be greater than 0");
            string editedTaskIdString = editedTaskId.ToString();

            XmlDocument taskListDoc = new XmlDocument();

            if (File.Exists(filePath))
            {
                taskListDoc.Load(filePath);
                XmlElement rootElement = taskListDoc.DocumentElement; // Get reference to root node
                XmlNodeList listOfTaskIds = taskListDoc.GetElementsByTagName("taskId"); // Create a list of nodes whose name is taskId

                foreach (XmlNode node in listOfTaskIds)
                {
                    if (node.Value == editedTaskIdString)
                    {
                        XmlNode parent_Task = node.ParentNode;
                        XmlNodeList listOfChildrenForTask = parent_Task.ChildNodes;

                        taskToBeEdited.Id = int.Parse(listOfChildrenForTask[0].Value);
                        taskToBeEdited.Description = listOfChildrenForTask[1].Value;
                        taskToBeEdited.StartTime = DateTime.Parse(listOfChildrenForTask[2].Value);
                        taskToBeEdited.EndTime = DateTime.Parse(listOfChildrenForTask[3].Value);
                        taskToBeEdited.Deadline = DateTime.Parse(listOfChildrenForTask[4].Value);
                        break;
                    }
                }
            }

            return taskToBeEdited;
        }

        internal void WriteEditedTaskToFile(Task editedTask)
        {
            string editedTaskIdString = editedTask.Id.ToString();
            XmlDocument taskListDoc = new XmlDocument();

            if (File.Exists(filePath))
            {
                taskListDoc.Load(filePath);
                XmlElement rootElement = taskListDoc.DocumentElement; // Get reference to root node
                XmlNodeList listOfTaskIds = taskListDoc.GetElementsByTagName("taskId"); // Create a list of nodes whose name is taskId

                foreach(XmlNode node in listOfTaskIds)
                {
                    if (node.Value == editedTaskIdString)
                    {
                        XmlNode parent_Task = node.ParentNode;
                        XmlNodeList listOfChildrenForTask = parent_Task.ChildNodes;

                        for(int i=1; i<listOfChildrenForTask.Count; i++) //Starts from index 1 since taskId cannot change
                        {
                            listOfChildrenForTask[i].RemoveAll();
                        }
 
                        XmlText taskDescriptionValue = taskListDoc.CreateTextNode(editedTask.Description);
                        listOfChildrenForTask[1].AppendChild(taskDescriptionValue);
                        XmlText taskStartTimeValue = taskListDoc.CreateTextNode(editedTask.StartTime.ToString());
                        listOfChildrenForTask[2].AppendChild(taskStartTimeValue);
                        XmlText taskEndTimeValue = taskListDoc.CreateTextNode(editedTask.EndTime.ToString());
                        listOfChildrenForTask[3].AppendChild(taskEndTimeValue);
                        XmlText taskDeadlineValue = taskListDoc.CreateTextNode(editedTask.Deadline.ToString());
                        listOfChildrenForTask[4].AppendChild(taskDeadlineValue);
                        break;

                    }
                }
                taskListDoc.Save(filePath);

            }
           
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
                foreach (Task taskToBeRemoved in listOfTasks)
                {
                    if (taskToBeRemoved.Id == deletedTaskId)
                    {
                        listOfTasks.Remove(taskToBeRemoved);
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
    }
}
