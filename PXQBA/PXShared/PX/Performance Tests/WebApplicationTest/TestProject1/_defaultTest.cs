using WebApplicationTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for _defaultTest and is intended
    ///to contain all _defaultTest Unit Tests
    ///</summary>
    [TestClass()]
    public class _defaultTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
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
        ///A test for btnTestDS_Click
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType( "ASP.NET" )]
        [AspNetDevelopmentServerHost( "C:\\Users\\gchernyak\\documents\\visual studio 2010\\Projects\\WebApplicationTest\\WebApplicationTest", "/" )]
        [UrlToTest( "http://localhost:51472/" )]
        [DeploymentItem( "WebApplicationTest.dll" )]
        public void btnTestDS_ClickTest() {
            _default_Accessor target = new _default_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.btnTestDS_Click( sender, e );
            Assert.Inconclusive( "A method that does not return a value cannot be verified." );
        }

        /// <summary>
        ///A test for btnTestStream_Click
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType( "ASP.NET" )]
        [AspNetDevelopmentServerHost( "C:\\Users\\gchernyak\\documents\\visual studio 2010\\Projects\\WebApplicationTest\\WebApplicationTest", "/" )]
        [UrlToTest( "http://localhost:51472/" )]
        [DeploymentItem( "WebApplicationTest.dll" )]
        public void btnTestStream_ClickTest() {
            _default_Accessor target = new _default_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.btnTestStream_Click( sender, e );
            Assert.Inconclusive( "A method that does not return a value cannot be verified." );
        }
    }
}
