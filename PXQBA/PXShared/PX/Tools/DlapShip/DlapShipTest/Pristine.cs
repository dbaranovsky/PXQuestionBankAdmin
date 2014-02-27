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
    public class PristineTest
    {
        /// <summary>
        /// The test batch file path
        /// </summary>
        private const string TestBatchFilePath = @"../../../../../DLAPScripts/DlapShipScripts/Tests/FullXBook4DlapShip.xml";
        private const string TestUpdateFilePath = "../../../TestScripts/dashboardEditLink.xml";

        private const string DefaultTestEntityId = "73427";

        [TestMethod]
        public void XBook()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing value */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestBatchFilePath), 
                                                String.Format("/u:{0}", TestUpdateFilePath), 
                                                String.Format("/id:{0}", DefaultTestEntityId),
                                                "/app:xbook", "/e:pr"});
                    
                    Assert.IsTrue(sw.ToString().Contains(@"<response code=""OK"">"));
                    
                    Assert.IsTrue(Program.ResultData.Count > 0);

                    string expectedNode = @"<bfw_property name=""allow_edit_course"" type=""String"">true</bfw_property>";

                    foreach (XDocument document in Program.ResultData)
                    {
                        Assert.IsTrue(document.GetXmlDocument().InnerXml.Contains(expectedNode));
                    }
                }
            }
        }

        [TestMethod]
        public void LaunchpadBatchRemoval()
        {
            var pathToUpdate = @"C:\Development\PX\Trunk\PX\DLAPScripts\CourseScripts\faceplate_PROD.xml";

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing value */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", pathToUpdate),
                                                "/app:launchpad", "/e:pr"});

                    Assert.IsTrue(sw.ToString().Contains(@"<response code=""OK"">"));
                }
            }
        }
    }
}
