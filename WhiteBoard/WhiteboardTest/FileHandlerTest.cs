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
    }
}
