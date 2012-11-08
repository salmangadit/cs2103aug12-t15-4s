using WhiteBoard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace WhiteboardTest
{
    //@author U094776M
    
    /// <summary>
    ///This is a test class for CommandParserTest and is intended
    ///to contain all CommandParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommandParserTest
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
        ///A test for SplitString
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void SplitStringTest()
        {
            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            CommandParser_Accessor target = new CommandParser_Accessor(); // TODO: Initialize to an appropriate value

            //test case 1
            string usercommand = "  hello    there     Jim"; // TODO: Initialize to an appropriate value
            List<string> expected = new List<string>();
            expected.Add("hello");
            expected.Add("there");
            expected.Add("Jim");
            target.SplitString(usercommand);
            List<string> userCommandList = target.ReturnUserCommandListForTesting();
            CollectionAssert.AreEqual(expected, userCommandList);
            userCommandList.Clear();

            //test case 2

            usercommand = "  hello    there     Jim   76567 "; // TODO: Initialize to an appropriate value
            expected = new List<string>();
            expected.Add("hello");
            expected.Add("there");
            expected.Add("Jim");
            expected.Add("76567");
            target.SplitString(usercommand);
            userCommandList = target.ReturnUserCommandListForTesting();
            CollectionAssert.AreEqual(expected, userCommandList);
        }

        /// <summary>
        ///A test for IsValidTaskId
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void IsValidTaskIdTest()
        {
            CommandParser_Accessor target = new CommandParser_Accessor(); // TODO: Initialize to an appropriate value

            //Test case 1
            string str = "T00024"; // TODO: Initialize to an appropriate value
            int expected = 24; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.IsValidTaskId(str);
            Assert.AreEqual(expected, actual);
            
            // Test case 2
        }

        /// <summary>
        ///A test for IsTime
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void IsTimeTest()
        {
            CommandParser_Accessor target = new CommandParser_Accessor(); // TODO: Initialize to an appropriate value

            //Test case 1
            string time = "8pm"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsTime(time);
            Assert.AreEqual(expected, actual);
           
            // Test case 2
        }

        /// <summary>
        ///A test for ParseTime
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void ParseTimeTest()
        {
            CommandParser_Accessor target = new CommandParser_Accessor(); // TODO: Initialize to an appropriate value

            // Test case 1
            string time = "10:30pm"; // TODO: Initialize to an appropriate value
            TimeSpan expected = new TimeSpan(22,30,0); // TODO: Initialize to an appropriate value
            TimeSpan actual;
            actual = target.ParseTime(time);
            Assert.AreEqual(expected, actual);
           

            // Test case 2
        }

        /// <summary>
        ///A test for IsDate
        ///</summary>
        [TestMethod()]
        public void IsDateTest()
        {
            CommandParser target = new CommandParser(); // TODO: Initialize to an appropriate value
            string datestring = "20.11.2012"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsDate(datestring);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseForDates
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void ParseForDatesTest()
        {
            CommandParser_Accessor target = new CommandParser_Accessor(); // TODO: Initialize to an appropriate value
            List<string> commandlist = new List<string>(); ; // TODO: Initialize to an appropriate value
            commandlist.Add("Go");
            commandlist.Add("shopping");
            commandlist.Add("on");
            commandlist.Add("10.12.12");
            int index = 1; // TODO: Initialize to an appropriate value
            bool modify = false; // TODO: Initialize to an appropriate value
            int expected = 1; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ParseForDates(commandlist, index, modify);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
