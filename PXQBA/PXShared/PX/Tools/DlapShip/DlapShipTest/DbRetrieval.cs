using System;
using System.IO;
using DlapShip;
using DlapShip.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DlapShipTest
{
    [TestClass]
    public class DbRetrieval
    {
        private const string TestInvalidFilePath = "../../TestScripts/test.xml";
        private const string TestFilePath = "../../../TestScripts/test.xml";

        private const string DefaultTestEntityId = "63237";

        [TestMethod]
        public void RunMain()
        {
            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);

                /* empty arguments */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine))) {
                    Console.SetIn(sr);

                    Program.Main(new string[] { });

                    string expected = Resources.NoFileError;

                    Assert.IsTrue(sw.ToString().Contains(expected));
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* empty file path */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { "/ti:universe9e", "/app:faceplate", "/e:dev" });

                    string expected = Resources.NoFileError;

                    Assert.IsTrue(sw.ToString().Contains(expected));
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* empty title and application name */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestFilePath),
                                                "/ti:universe9e", "/e:dev" });

                    string expected = Resources.NoAppError;

                    Assert.IsTrue(sw.ToString().Contains(expected));
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* empty title and application name */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestFilePath),
                                                String.Format("/id:{0}", DefaultTestEntityId),
                                                "/app:faceplate", "/e:dev" });

                    Assert.IsTrue(sw.ToString().Contains(@"<response code=""OK"">"));
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* empty environment */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestFilePath), 
                                                "/ti:universe9e", "/app:faceplate" });

                    string expected = "DlapShip requires an environment";

                    Assert.IsTrue(sw.ToString().Contains(expected));
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* not executed - incorrect course title */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestFilePath),
                                                "/app:faceplate", "/ti:adfas", "/e:dev" });

                    Assert.IsTrue(Program.ListProductCourseIds.Count == 0);
                }
            }
        }
        
        [TestMethod]
        public void QueryTest()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* files test */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestInvalidFilePath), 
                                            "/ti:universe9e", "/app:faceplate", "/e:dev" });

                    string expected = String.Format("Error loading {0}: file does not exist", TestInvalidFilePath);

                    Assert.IsTrue(sw.ToString().Contains(expected));
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* db query retrieval test */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestFilePath), 
                                            "/ti:universe9e", "/app:faceplate", "/e:dev"});

                    Assert.IsTrue(sw.ToString().Contains(@"<response code=""OK"">"));
                }
            }
        }
    }
}
