using WhiteBoard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace WhiteboardTest
{

    //@author U095146E
    /// <summary>
    ///This is a test class for ControllerTest and is intended
    ///to contain all ControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ControllerTest
    {
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Getting View Command Object
        ///</summary>
        [TestMethod()]
        public void GetViewCommandObjectTest()
        {
            FacadeLayer target = new FacadeLayer(); // TODO: Initialize to an appropriate value
            string userString = "view all"; // TODO: Initialize to an appropriate value
            List<Task> screenState = new List<Task>(); // TODO: Initialize to an appropriate value
            FileHandler fileHandler = FileHandler.Instance;
            Task view = new Task();
            Command expected = new ViewCommand(fileHandler, view, screenState); // TODO: Initialize to an appropriate value
            Command actual;
            actual = target.GetCommandObject(userString, screenState);
            Assert.AreEqual(expected.CommandType, actual.CommandType);
        }

        /// <summary>
        ///A test for Getting Add Command Object
        ///</summary>
        [TestMethod()]
        public void GetAddCommandObjectTest()
        {
            FacadeLayer target = new FacadeLayer(); // TODO: Initialize to an appropriate value
            string userString = "test"; // TODO: Initialize to an appropriate value
            List<Task> screenState = new List<Task>(); // TODO: Initialize to an appropriate value
            FileHandler fileHandler = FileHandler.Instance;
            Task add = new Task(0,"test");
            Command expected = new AddCommand(fileHandler, add, screenState); // TODO: Initialize to an appropriate value
            Command actual;
            actual = target.GetCommandObject(userString, screenState);
            Assert.AreEqual(expected.CommandType, actual.CommandType);
        }

        /// <summary>
        ///A test for Getting Edit Command Object
        ///</summary>
        [TestMethod()]
        public void GetEditCommandObjectTest()
        {
            FacadeLayer target = new FacadeLayer(); // TODO: Initialize to an appropriate value
            string userString = "modify T4 xxx"; // TODO: Initialize to an appropriate value
            List<Task> screenState = new List<Task>(); // TODO: Initialize to an appropriate value
            FileHandler fileHandler = FileHandler.Instance;
            Task edit = new Task(4,"xxx");
            Command expected = new EditCommand(fileHandler, edit, screenState); // TODO: Initialize to an appropriate value
            Command actual;
            actual = target.GetCommandObject(userString, screenState);
            Assert.AreEqual(expected.CommandType, actual.CommandType);
        }

        /// <summary>
        ///A test for GetAllTasks
        ///</summary>
        [TestMethod()]
        public void GetAllTasksTest()
        {
            FacadeLayer target = new FacadeLayer(); // TODO: Initialize to an appropriate value
            List<Task> screenState = new List<Task>(); // TODO: Initialize to an appropriate value
            Task view = new Task();
            FileHandler fileHandler = FileHandler.Instance;
            Command expected = new ViewCommand(fileHandler, view, screenState); // TODO: Initialize to an appropriate value
            Command actual;
            actual = target.GetAllTasks(screenState);
            Assert.AreEqual(expected.CommandType, actual.CommandType);
        }
    }
}
