using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using DlapShip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DlapShipTest
{
    [TestClass]
    public class UpdateDlap
    {
        [TestMethod]
        public void UpdateDlapTest1()
        {
            string TestUpdateFilePath1 = "../../../TestScripts/updateItems1.xml";

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing value */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/u:{0}", TestUpdateFilePath1), 
                                            "/ti:universe9e", "/app:faceplate", "/e:dev"});

                    string expected = "Upload file not exists";

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <Update>
                            <key>/test</key>
                            <value>this is Thanda</value>
                        </Update>
                     */

                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = "this is Thanda";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }

                    expected = "Successful update to DLAP";

                    var totalExpectedCount = new Regex(expected).Matches(sw.ToString()).Count;

                    Assert.IsTrue(totalExpectedCount == Program.ItemsCount);
                }
            }
        }

        [TestMethod]
        public void UpdateBatchTest()
        {
            string TestUpdateFilePath1 = "../../../TestScripts/test.xml";

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing value */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestUpdateFilePath1), 
                                            "/ti:universe9e", "/app:faceplate", "/e:dev"});

                    string expected = "UpdateItems file not exists";

                    Assert.IsTrue(sw.ToString().Contains(expected));

                    /*
                        <batch>
                            <request cmd="updatecourses">
                                <requests>
                                    <course courseid="-courseid-" reference="">
                     */

                    Assert.IsTrue(sw.ToString().Contains(@"<response code=""OK"">"));
                }
            }
        }
    }
}
