using WhiteBoard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace WhiteboardTest
{


    /// <summary>
    ///This is a test class for AutoCompletorTest and is intended
    ///to contain all AutoCompletorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AutoCompletorTest
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
        
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        /// <summary>
        ///A test for AddToSets
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void AddToSetsTest()
        {
            AutoCompletor_Accessor target = new AutoCompletor_Accessor(); // TODO: Initialize to an appropriate value

            Task task = new Task(1, "Hello    there Tester", DateTime.Now, DateTime.Now);

            List<string> expectedWordSet = new List<string>();
            expectedWordSet.Add("Hello");
            expectedWordSet.Add("there");
            expectedWordSet.Add("Tester");

            List<string> expectedLineSet = new List<string>();

            expectedLineSet.Add("Hello    there Tester");

            target.AddToSets(task);

            CollectionAssert.Equals(target.wordSet, expectedWordSet);
            CollectionAssert.Equals(target.lineSet, expectedLineSet);
        }

        /// <summary>
        ///A test for GenerateLineSet and GenerateWordSet
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void GenerateQuerySetTest()
        {
            AutoCompletor_Accessor target = new AutoCompletor_Accessor(); // TODO: Initialize to an appropriate value
            List<Task> tasks = new List<Task>(); // TODO: Initialize to an appropriate value

            Task task = new Task(1, "Hello there Tester", DateTime.Now, DateTime.Now);
            Task task2 = new Task(1, "How are you?", DateTime.Now, DateTime.Now);

            tasks.Add(task);
            tasks.Add(task2);

            List<string> expectedLineSet = new List<string>();
            List<string> expectedWordSet = new List<string>();

            expectedWordSet.Add("Hello");
            expectedWordSet.Add("there");
            expectedWordSet.Add("Tester");
            expectedWordSet.Add("How");
            expectedWordSet.Add("are");
            expectedWordSet.Add("you");
            expectedWordSet.Add("?");

            expectedLineSet.Add("Hello there Tester");
            expectedLineSet.Add("How are you?");

            target.GenerateQuerySet(tasks);

            CollectionAssert.Equals(target.lineSet, expectedLineSet);
            CollectionAssert.Equals(target.wordSet, expectedWordSet);
        }

        /// <summary>
        ///A test for Query
        ///</summary>
        [TestMethod()]
        public void QueryTest()
        {
            List<Task> tasks = new List<Task>(); // TODO: Initialize to an appropriate value

            Task task = new Task(1, "Hello there Tester", DateTime.Now, DateTime.Now);
            Task task2 = new Task(1, "Tester is there everywhere hello again!", DateTime.Now, DateTime.Now);
            Task task3 = new Task(1, "Testing is fun", DateTime.Now, DateTime.Now);

            tasks.Add(task);
            tasks.Add(task2);
            tasks.Add(task3);

            AutoCompletor target = new AutoCompletor(tasks); // TODO: Initialize to an appropriate value

            // test 1
            string[] queryCollectionSet1 = new string[] { "hel", "hello", "he" }; // TODO: Initialize to an appropriate value

            List<string> expectedSet1 = new List<string>(); // TODO: Initialize to an appropriate value

            expectedSet1.Add("hello");
            expectedSet1.Add("Hello there Tester");
            expectedSet1.Add("Tester is there everywhere hello again!");

            List<string> actual;

            foreach (string query in queryCollectionSet1)
            {
                actual = target.Query(query);
                CollectionAssert.AreEqual(expectedSet1, actual);
            }

            //test 2
            
            string[] queryCollectionSet2 = new string[] { "te", "test", "test" }; // TODO: Initialize to an appropriate value

            List<string> expectedSet2 = new List<string>(); // TODO: Initialize to an appropriate value

            expectedSet2.Add("tester");
            expectedSet2.Add("testing");
            expectedSet2.Add("Testing is fun");
            expectedSet2.Add("Hello there Tester");
            expectedSet2.Add("Tester is there everywhere hello again!");

            foreach (string query in queryCollectionSet2)
            {
                actual = target.Query(query);
                CollectionAssert.AreEqual(expectedSet2, actual);
            }

            //test 3

            string newQuery = "testi";

            List<string> expectedSet3 = new List<string>();

            expectedSet3.Add("testing");
            expectedSet3.Add("Testing is fun");

            actual = target.Query(newQuery);
            CollectionAssert.AreEqual(expectedSet3, actual);

        }

        /// <summary>
        ///A test for Query when search string is empty
        ///</summary>
        [TestMethod()]
        public void QueryTestEmptySearch()
        {
            List<Task> tasks = new List<Task>(); // TODO: Initialize to an appropriate value

            Task task = new Task(1, "Hello there Tester", DateTime.Now, DateTime.Now);
            Task task2 = new Task(1, "Tester is there everywhere hello again!", DateTime.Now, DateTime.Now);

            tasks.Add(task);
            tasks.Add(task2);

            AutoCompletor target = new AutoCompletor(tasks); // TODO: Initialize to an appropriate value

            string query = ""; // TODO: Initialize to an appropriate value
            List<string> expected = new List<string>(); // TODO: Initialize to an appropriate value

            List<string> actual;
            actual = target.Query(query);
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RemoveFromSets
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void RemoveFromSetsTest()
        {
            AutoCompletor_Accessor target = new AutoCompletor_Accessor(); // TODO: Initialize to an appropriate value
            Task task = new Task(1, "Hello there Tester", DateTime.Now, DateTime.Now); // TODO: Initialize to an appropriate value
            Task task2 = new Task(1, "Tester is prime", DateTime.Now, DateTime.Now);

            target.AddToSets(task);
            target.AddToSets(task2);

            target.RemoveFromSets(task2);

            List<string> expectedLineSet = new List<string>();
            List<string> expectedWordSet = new List<string>();

            expectedLineSet.Add("Hello there Tester");

            expectedWordSet.Add("Hello");
            expectedWordSet.Add("there");
            expectedWordSet.Add("Tester");

            CollectionAssert.Equals(target.lineSet, expectedLineSet);
            CollectionAssert.Equals(target.wordSet, expectedWordSet);
        }

        /// <summary>
        ///A test for SortByLength
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void SortByLengthTest()
        {
            IEnumerable<string> e = new string[] { "number", "word", "texting" }; // TODO: Initialize to an appropriate value
            IEnumerable<string> expected = new string[] { "word", "number", "texting" }; // TODO: Initialize to an appropriate value
            IEnumerable<string> actual;
            actual = AutoCompletor_Accessor.SortByLength(e);

            CollectionAssert.AreEqual(new List<string>(expected), new List<string>(actual));
        }

        /// <summary>
        ///A test for Update
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WhiteBoard.exe")]
        public void UpdateTest()
        {
            Task task = new Task(1, "Hello there Tester", DateTime.Now, DateTime.Now);
            AutoCompletor_Accessor target = new AutoCompletor_Accessor();

            target.Update(UpdateType_Accessor.Add, task, null);

            List<string> expectedWordSet = new List<string>();
            expectedWordSet.Add("Hello");
            expectedWordSet.Add("there");
            expectedWordSet.Add("Tester");

            List<string> expectedLineSet = new List<string>();

            expectedLineSet.Add("Hello there Tester");

            CollectionAssert.Equals(target.wordSet, expectedWordSet);
            CollectionAssert.Equals(target.lineSet, expectedLineSet);
        }
    }
}
