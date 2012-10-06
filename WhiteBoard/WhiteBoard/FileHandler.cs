using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
