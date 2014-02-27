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
    public class UpdateItems
    {
        private const string TestFilePath = "../../../TestScripts/test.xml";

        private const string TestUpdateFilePath1 = "../../../TestScripts/updateItems1.xml";
        private const string TestUpdateFilePath2 = "../../../TestScripts/updateItems2.xml";
        private const string TestUpdateFilePath3 = "../../../TestScripts/updateItems3.xml";
        private const string TestUpdateFilePath4 = "../../../TestScripts/updateItems4.xml";

        private const string DefaultTestEntityId = "63237";

        [TestMethod]
        public void UpdateTest1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing value */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestUpdateFilePath1), 
                                                String.Format("/id:{0}", DefaultTestEntityId),
                                            "/ti:universe9e", "/app:launchpad", "/e:dev"});

                    string expected = "Upload file not exists";

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <Update>
                            <key>/test</key>
                            <value>this is Thanda</value>
                        </Update>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = "<test>this is Thanda</test>";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }

        [TestMethod]
        public void UpdateTest2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update non-existing attribute */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestUpdateFilePath2),
                                                String.Format("/id:{0}", DefaultTestEntityId), 
                                            "/ti:universe9e", "/app:launchpad", "/e:dev"});

                    string expected = "Upload file not exists";

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <Update>
                            <key>/test[@res='blah']</key>
                            <value>ola</value>
                        </Update>
                        <Update>
                            <key>/test[@you='chicken']</key>
                        </Update>
                        <Update>
                            <key>/test</key>
                            <value>correct</value>
                        </Update>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = @"<test res=""blah"" you=""chicken"">correct</test>";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }

        [TestMethod]
        public void UpdateTest3()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update non-existing element */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestUpdateFilePath3),
                                                String.Format("/id:{0}", DefaultTestEntityId), 
                                            "/ti:universe9e", "/app:launchpad", "/e:dev"});

                    string expected = "Upload file not exists";

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <Update>
                            <key>/test[@res='blah']/hello</key>
                            <value>Thanda</value>
                        </Update>
                        <Update>
                            <key>/test/myfriend</key>
                            <value>Thanda Oo</value>
                        </Update>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = @"><hello><myfriend>Thanda</myfriend></hello>";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }

        [TestMethod]
        public void UpdateTest4()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing attribute */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestUpdateFilePath4),
                                                String.Format("/id:{0}", DefaultTestEntityId), 
                                            "/ti:universe9e", "/app:launchpad", "/e:dev"});

                    string expected = "Upload file not exists";

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <Update>
                            <key>/test[@pinky='me']/yetagain</key>
                            <value>I told you so</value>
                        </Update>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = @"><yetagain>I told you so</yetagain>";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }
    }
}
