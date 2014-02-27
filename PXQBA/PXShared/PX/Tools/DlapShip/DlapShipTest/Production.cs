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
    public class ProductionTest
    {
        private const string TestUpdateFilePath = "../../../TestScripts/dashboardEditLink.xml";

        private const string DefaultTestEntityId = "11134";

        [TestMethod]
        public void XBook()
        {
            var TestBatchFilePath = @"../../../../../DLAPScripts/DlapShipScripts/Tests/FullXBook4DlapShip.xml";

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing value */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestBatchFilePath), 
                                                String.Format("/u:{0}", TestUpdateFilePath), 
                                                "/app:xbook", "/e:prod"});
                    
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
        public void Launchpad()
        {
            var TestBatchFilePath = @"../../../../../DLAPScripts/PRODUCTS/Launchpad/_GLOBAL/ALL/dashboard_course_update.xml";

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                /* update existing value */
                using (StringReader sr = new StringReader(String.Format("DlapShip {0}", Environment.NewLine)))
                {
                    Console.SetIn(sr);

                    Program.Main(new string[] { String.Format("/f:{0}", TestBatchFilePath), 
                                                String.Format("/u:{0}", TestUpdateFilePath), 
                                                String.Format("/id:{0}", 11159),
                                                "/app:launchpad", "/e:prod"});

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
                                                "/app:launchpad", "/e:prod"});

                    Assert.IsTrue(sw.ToString().Contains(@"<response code=""OK"">"));
                }
            }
        }
    }
}
