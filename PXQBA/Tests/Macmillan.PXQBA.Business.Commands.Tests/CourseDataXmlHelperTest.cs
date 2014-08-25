using System;
using System.Linq;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class CourseDataXmlHelperTest
    {
        private const string xmlDataWithItemLinks = @"<coursedata>
                                                        <itemlinksfields>
                                                             <field1 friendlyname=""friendlyname"">
                                                                   <item>
                                                                            <id>1</id>
                                                                            <value>value1</value>
                                                                    </item>   
                                                             </field1>
                                                             <field2>
                                                                   
                                                             </field2> 

                                                                 <field3>
                                                                    <item>
                                                                            <id>1</id>
                                                                   
                                                                    </item>  
                                                                       <item>
                                                                          
                                                                            <value>value2</value>
                                                                    </item>   
                                                             </field3> 
                                                    </itemlinksfields>
                                                    </coursedata>";
        
        [TestMethod]
        public void GetItemLinksDescriptors_XmlDataWithItemLinks_ItemLinkFields()
        {
            var itemLinks = CourseDataXmlHelper.GetItemLinksDescriptors(XElement.Parse(xmlDataWithItemLinks));
            Assert.IsTrue(itemLinks.Count() == 3);
            Assert.IsTrue(itemLinks.First().FriendlyName == "friendlyname");
            Assert.IsTrue(itemLinks.Last().CourseMetadataFieldValues.Count() == 2);

        }

        [TestMethod]
        public void GetItemLinksDescriptors_Null_EmptyList()
        {
            Assert.IsFalse(CourseDataXmlHelper.GetItemLinksDescriptors(null).Any());
        }


        [TestMethod]
        public void GetItemLinksDescriptors_EmptyXml_EmptyList()
        {
            Assert.IsFalse(CourseDataXmlHelper.GetItemLinksDescriptors(XElement.Parse("<coursedata></coursedata>")).Any());
        }


    }
}
