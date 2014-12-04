using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Lib;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    /// <summary>
    /// Summary description for UnitTest2
    /// </summary>
    [TestClass]
    public class UnitTest2
    {
        public UnitTest2()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void GRSTest1()
        {
            string upc = "00018771897620";
            var response = GrsHelper.GetReleaseRights(upc, "UPC");
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/" + upc + ".xml");
            using (var sw = new StreamWriter(outputPath))
            {
                string xml = SerializationHelper.XmlSerialize(response, true);
                sw.WriteLine(xml);
                sw.Flush();
                sw.Close();
            }

            Assert.AreEqual(1, response.Length);
        }

        [TestMethod]
        public void GRSTest2()
        {
            var response = GrsHelper.GetReleasesSearch();
            Assert.AreEqual(1, response.Length);
        }
    }
}
