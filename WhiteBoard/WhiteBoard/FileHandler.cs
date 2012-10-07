using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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

            if (File.Exists(filePath))
            {
                taskListDoc.Load(filePath);
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

            }
        }

        internal static Task GetTaskFromFile(int editedTaskId)
        {
            Task T = new Task();
            return T;
        }

        internal static void WriteEditedTaskToFile(Task editedTask)
        {
           
        }
    }
}
