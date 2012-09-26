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

        internal void WriteToFile(ToDo taskToAdd)
        {
            XmlSerializer objXmlSer = new XmlSerializer(typeof(ToDo));
            StreamWriter objStrWrt = new StreamWriter(filePath);
            objXmlSer.Serialize(objStrWrt, taskToAdd);
            objStrWrt.Close();
        }
    }
}
