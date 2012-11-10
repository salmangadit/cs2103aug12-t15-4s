using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhiteBoard;

namespace WhiteboardTest
{
    /// <summary>
    /// Summary description for FileHandlerTest
    /// </summary>
    [TestClass]
    public class FileHandlerTest
    {
        public FileHandlerTest()
        {
            FileHandler.Instance.FileUpdateEvent += new FileUpdate(Update);
        }

        private void Update(UpdateType update, Task task, Task uneditedTask)
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [DeploymentItem("WhiteBoard.exe")]
        public void AddTaskToFileTest()
        {
            Task task1 = new Task(0, "Hello World", new DateTime(2012, 11, 11));
            FileHandler filehandler = FileHandler.UnitTestInstance;
            filehandler.AddTaskToFile(task1);
            Task task2 = filehandler.GetTaskFromFile(1);
            Assert.AreEqual(task1.Id, task2.Id);
            Assert.AreEqual(task1.Description, task2.Description);
            Assert.AreEqual(task1.StartTime, task2.StartTime);
        }

        /// <summary>
        ///A test for AddTaskToFile
        ///</summary>
        [TestMethod()]
        public void AddTaskToFileTest1()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012,12,5));
            int taskId = 123; 
            bool expected = true; 
            bool actual;
            filehandler.AddTaskToFile(taskToAdd);
            actual = filehandler.AddTaskToFile(taskToAdd, taskId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetTaskFromFile
        ///</summary>
        [TestMethod()]
        public void GetTaskFromFileTest()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012, 12, 5));
            Task taskToAdd2 = new Task(0, "Go shopping with mom", new DateTime(2012, 12, 16, 10, 30, 0), new DateTime(2012, 12, 16, 2, 0, 0));
            Task taskToAdd3 = new Task(0, "Floating task");
            filehandler.AddTaskToFile(taskToAdd);
            filehandler.AddTaskToFile(taskToAdd2);
            filehandler.AddTaskToFile(taskToAdd3);
            int editedTaskId = 2;
            Task expected = taskToAdd2; 
            Task actual;
            actual = filehandler.GetTaskFromFile(editedTaskId);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.StartTime, actual.StartTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);
        }

        /// <summary>
        ///A test for WriteEditedTaskToFile
        ///</summary>
        [TestMethod()]
        public void WriteEditedTaskToFileTest()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012, 12, 5));
            Task taskToAdd2 = new Task(0, "Go shopping with mom", new DateTime(2012, 12, 16, 10, 30, 0), new DateTime(2012, 12, 16, 2, 0, 0));
            Task taskToAdd3 = new Task(0, "Floating task");
            filehandler.AddTaskToFile(taskToAdd);
            filehandler.AddTaskToFile(taskToAdd2);
            filehandler.AddTaskToFile(taskToAdd3);
            Task editedTask = taskToAdd3;
            editedTask.Description = "Now edited";
            filehandler.WriteEditedTaskToFile(editedTask);
            Task actual= filehandler.GetTaskFromFile(editedTask.Id);
            Assert.AreEqual(editedTask.Id, actual.Id);
            Assert.AreEqual(editedTask.Description, actual.Description);
            Assert.AreEqual(editedTask.StartTime, actual.StartTime);
            Assert.AreEqual(editedTask.EndTime, actual.EndTime);
            
        }

        /// <summary>
        ///A test for DeleteTaskFromFile
        ///</summary>
        [TestMethod()]
        public void DeleteTaskFromFileTest()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012, 12, 5));
            Task taskToAdd2 = new Task(0, "Go shopping with mom", new DateTime(2012, 12, 16, 10, 30, 0), new DateTime(2012, 12, 16, 2, 0, 0));
            Task taskToAdd3 = new Task(0, "Floating task");
            filehandler.AddTaskToFile(taskToAdd);
            filehandler.AddTaskToFile(taskToAdd2);
            filehandler.AddTaskToFile(taskToAdd3);
            int deletedTaskId = 3; 
            bool expected = true; 
            bool actual;
            actual = filehandler.DeleteTaskFromFile(deletedTaskId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ArchiveTaskInFile
        ///</summary>
        [TestMethod()]
        public void ArchiveTaskInFileTest()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012, 12, 5));
            Task taskToAdd2 = new Task(0, "Go shopping with mom", new DateTime(2012, 12, 16, 10, 30, 0), new DateTime(2012, 12, 16, 2, 0, 0));
            Task taskToAdd3 = new Task(0, "Floating task");
            filehandler.AddTaskToFile(taskToAdd);
            filehandler.AddTaskToFile(taskToAdd2);
            filehandler.AddTaskToFile(taskToAdd3);
            int archivedTaskId = 2;          
            filehandler.ArchiveTaskInFile(archivedTaskId);
            Task actual = filehandler.GetTaskFromFile(archivedTaskId);
            Assert.AreEqual(actual.Archive, true);
        }

        /// <summary>
        ///A test for UnarchiveTaskInFile
        ///</summary>
        [TestMethod()]
        public void UnarchiveTaskInFileTest()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012, 12, 5));
            Task taskToAdd2 = new Task(0, "Go shopping with mom", new DateTime(2012, 12, 16, 10, 30, 0), new DateTime(2012, 12, 16, 2, 0, 0));
            Task taskToAdd3 = new Task(0, "Floating task");
            filehandler.AddTaskToFile(taskToAdd);
            filehandler.AddTaskToFile(taskToAdd2);
            filehandler.AddTaskToFile(taskToAdd3);
            filehandler.ArchiveTaskInFile(1);
            int unarchivedTaskId = 1; 
            filehandler.UnarchiveTaskInFile(unarchivedTaskId);
            Task actual = filehandler.GetTaskFromFile(unarchivedTaskId);
            Assert.AreEqual(actual.Archive, false);
        }

        /// <summary>
        ///A test for ViewArchive
        ///</summary>
        [TestMethod()]
        public void ViewArchiveTest()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012, 12, 5));
            Task taskToAdd2 = new Task(0, "Go shopping with mom", new DateTime(2012, 12, 16, 10, 30, 0), new DateTime(2012, 12, 16, 2, 0, 0));
            Task taskToAdd3 = new Task(0, "Floating task");
            filehandler.AddTaskToFile(taskToAdd);
            filehandler.AddTaskToFile(taskToAdd2);
            filehandler.AddTaskToFile(taskToAdd3);
            filehandler.ArchiveTaskInFile(1);
            filehandler.ArchiveTaskInFile(2);
            List<Task> expected = new List<Task>();
            expected.Add(taskToAdd);
            expected.Add(taskToAdd2);
            List<Task> actual;
            actual = filehandler.ViewArchive();
            Assert.AreEqual(true, actual[0].Archive);
            Assert.AreEqual(true, actual[1].Archive);
        }

        /// <summary>
        ///A test for ViewTasks
        ///</summary>
        [TestMethod()]
        public void ViewTasksTest()
        {
            FileHandler filehandler = FileHandler.UnitTestInstance;
            Task taskToAdd = new Task(0, "Hey there", new DateTime(2012, 12, 5));
            Task taskToAdd2 = new Task(0, "Go shopping with mom", new DateTime(2012, 12, 16, 10, 30, 0), new DateTime(2012, 12, 16, 2, 0, 0));
            Task taskToAdd3 = new Task(0, "Floating task");
            Task taskToAdd4 = new Task(0, "Visit Annie", new DateTime(2012, 12, 16, 2, 2, 0));
            filehandler.AddTaskToFile(taskToAdd);
            filehandler.AddTaskToFile(taskToAdd2);
            filehandler.AddTaskToFile(taskToAdd3);
            filehandler.AddTaskToFile(taskToAdd4);
            DateTime start = new DateTime(2012, 12, 5);
            DateTime end = new DateTime(2012, 12, 16, 2, 1, 0);
            Nullable<DateTime> startDate = new Nullable<DateTime>(start); 
            Nullable<DateTime> endDate = new Nullable<DateTime>(end); 
            List<Task> actual;
            actual = filehandler.ViewTasks(startDate, endDate);
            Assert.AreEqual(1, actual[0].Id);
            Assert.AreEqual(2, actual[1].Id);
        }
    }
}
