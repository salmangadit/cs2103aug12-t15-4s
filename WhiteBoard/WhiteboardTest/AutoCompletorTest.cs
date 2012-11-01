using WhiteBoard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;

namespace WhiteboardTest
{


    /// <summary>
    ///This is a test class for AutoCompletorTest and is intended
    ///to contain all AutoCompletorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AutoCompletorTest
    {

        static List<Task> tasks;
        static AutoCompletor_Accessor autoComplete;
        private TestContext testContextInstance;
        static Task task = new Task(1, "Hello there Tester", DateTime.Now, DateTime.Now);
        static Task task2 = new Task(1, "Tester is there everywhere hello again!", DateTime.Now, DateTime.Now);
        static Task task3 = new Task(1, "Testing is fun", DateTime.Now, DateTime.Now);

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

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            tasks = new List<Task>();

            tasks.Add(task);
            tasks.Add(task2);
            tasks.Add(task3);

            autoComplete = new AutoCompletor_Accessor(tasks);
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            tasks.Clear();

            tasks.Add(task);
            tasks.Add(task2);
            tasks.Add(task3);

            autoComplete = new AutoCompletor_Accessor(tasks);
        }

        /// <summary>
        ///A test for Query
        ///</summary>
        [TestMethod()]
        public void QueryTest()
        {
            List<string> expectedSet = new List<string>(); // TODO: Initialize to an appropriate value

            expectedSet.Add("hello");
            expectedSet.Add("Hello there Tester");
            expectedSet.Add("Tester is there everywhere hello again!");

            TestQuery("he", expectedSet);
            TestQuery("hel", expectedSet);
            TestQuery("hello", expectedSet);

            expectedSet.Clear();

            expectedSet.Add("tester");
            expectedSet.Add("testing");
            expectedSet.Add("Testing is fun");
            expectedSet.Add("Hello there Tester");
            expectedSet.Add("Tester is there everywhere hello again!");

            TestQuery("te", expectedSet);
            TestQuery("tes", expectedSet);
            TestQuery("test", expectedSet);

            expectedSet.Clear();

            expectedSet.Add("testing");
            expectedSet.Add("Testing is fun");

            TestQuery("testi", expectedSet);
            TestQuery("testing", expectedSet);
        }

        /// <summary>
        ///A test for Query when search string is empty
        ///</summary>
        [TestMethod()]
        public void EmptyQuerySearchTest()
        {
            string query = String.Empty;
            List<string> expected = new List<string>();

            TestQuery(query, expected);
        }

        /// <summary>
        /// Queries and asserts 
        ///</summary>
        private void TestQuery(string queryString, List<string> expected)
        {
            List<string> actual = autoComplete.Query(queryString);
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test the update event on add
        ///</summary>
        [TestMethod()]
        public void AddUpdateTest()
        {
            Task newTask = new Task(4, "Update Add", DateTime.Now, DateTime.Now);
            autoComplete.Update(UpdateType_Accessor.Add, newTask, null);

            List<string> expectedSet = new List<string>();

            expectedSet.Add("add");
            expectedSet.Add("Update Add");

            TestQuery("add", expectedSet);
        }

        /// <summary>
        ///Test the update event on archive
        ///</summary>
        [TestMethod()]
        public void ArchiveUpdateTest()
        {
            autoComplete.Update(UpdateType_Accessor.Archive, tasks[0], null);

            List<string> expectedSet = new List<string>();
            
            expectedSet.Add("hello");
            expectedSet.Add("Tester is there everywhere hello again!");

            TestQuery("he", expectedSet);
            TestQuery("hel", expectedSet);
            TestQuery("hello", expectedSet);

        }



    }
}
