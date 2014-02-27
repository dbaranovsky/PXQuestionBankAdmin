using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DlapShip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DlapShipTest
{
    [TestClass]
    public class DeleteItems
    {
        private const string TestDeleteFilePath1 = "../../../TestScripts/deleteItems1.xml";
        private const string TestDeleteFilePath2 = "../../../TestScripts/deleteItems2.xml";
        private const string TestDeleteFilePath3 = "../../../TestScripts/deleteItems3.xml";

        private const string DefaultTestEntityId = "63237";

        [TestMethod]
        public void DeleteTest1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* delete existing element */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestDeleteFilePath1),
                                                String.Format("/id:{0}", DefaultTestEntityId), 
                                            "/ti:universe9e", "/app:launchpad", "/e:dev"});

                    string expected = String.Format("Upload file not exists{0}", Environment.NewLine);

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                       <Delete>
                            /test[@res='blah']/hello
                        </Delete>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = @"<hello />";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }

        [TestMethod]
        public void DeleteTest2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* delete existing element with attribute */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestDeleteFilePath2),
                                                String.Format("/id:{0}", DefaultTestEntityId), 
                                            "/ti:universe9e", "/app:launchpad", "/e:dev"});

                    string expected = String.Format("Upload file not exists{0}", Environment.NewLine);

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <Delete>
                            /test[@res='chicken']
                        </Delete>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = @"<test";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }
        
        [TestMethod]
        public void DeleteTest3()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* delete existing element */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestDeleteFilePath3),
                                                String.Format("/id:{0}", DefaultTestEntityId), 
                                            "/ti:universe9e", "/app:launchpad", "/e:dev"});

                    string expected = String.Format("Upload file not exists{0}", Environment.NewLine);

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <Delete>
                            /test
                        </Delete>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = @"<test></test>";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }
    }
}
