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
            //int taskId;
            int newTaskId;
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
                    int indexOfLastTask = listOfTasks.Count - 1;
                    string lastTaskIdString = listOfTasks[indexOfLastTask].Value; // Get the value of the last task
                    newTaskId = int.Parse(lastTaskIdString) + 1; // Increase value
                }
                else
                {
                    newTaskId = 1;
                }

                string newTaskIdString = newTaskId.ToString(); // Convert back to string

                XmlElement newTaskElement = taskListDoc.CreateElement("taskId"); // Create element named taskId
                newTaskElement.InnerText = newTaskIdString; // Specify text of new task element
                XmlNodeList childrenOfTask = rootElement.LastChild.ChildNodes;
                rootElement.LastChild.InsertBefore(newTaskElement, childrenOfTask[0]);
                

            }
            
        }

        internal Task GetTaskFromFile(int editedTaskId)
        {
        }

        internal void WriteEditedTaskToFile(Task editedTask)
        {

        }
    }
}
