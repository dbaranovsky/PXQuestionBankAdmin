using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.PX.Biz.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Models;
using NSubstitute;
using Microsoft.Practices.ServiceLocation;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Models.Test
{
    [TestClass]
    public class ExtensionMethodsTest
    {
        private IBusinessContext context;
        private IServiceLocator serviceLocator;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();

            serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IBusinessContext>().Returns(context);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        [TestMethod]
        public void RestrictedDate_Should_Return_GMT_If_Date_Saved_Without_Kind()
        {
            ContentItem item = new ContentItem();
            var visibility = new XElement("bfw_visibility");
            var date = new XElement("date");
            var endDate = new XAttribute("endate", "1/8/2023 05:30 PM");
            date.Add(endDate);
            visibility.Add(date);
            item.Visibility = visibility;

            string restrictedDate = item.RestrictedDate();

            Assert.AreEqual(new DateTime(2023, 1, 8, 17, 30, 0, DateTimeKind.Utc), DateTime.Parse(restrictedDate).ToUniversalTime());
        }

        [TestMethod]
        public void RestrictedDate_Should_Return_GMT_If_Date_Saved_As_GMT()
        {
            ContentItem item = new ContentItem();
            var visibility = new XElement("bfw_visibility");
            var date = new XElement("date");
            var endDate = new XAttribute("endate", "2023-01-08T17:30:00Z");
            date.Add(endDate);
            visibility.Add(date);
            item.Visibility = visibility;

            string restrictedDate = item.RestrictedDate();

            Assert.AreEqual(new DateTime(2023, 1, 8, 17, 30, 0, DateTimeKind.Utc), DateTime.Parse(restrictedDate).ToUniversalTime());
        }

        [TestMethod]
        public void ApplyRestrictedAccess_Should_Return_False_For_Instructor()
        {
            ContentItem item = new ContentItem();
            item.UserAccess = AccessLevel.Instructor;

            var result = item.ApplyRestrictedAccess();

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ApplyRestrictedAccess_WhenInstructorViewInTOC_ShouldReturnTrue()
        {
            context.Course = new Biz.DataContracts.Course()
            {
                Id = "1"
            };
            ContentItem item = new ContentItem();
            item.UserAccess = AccessLevel.Instructor;
            var visibility = new XElement("bfw_visibility");
            visibility.Add(new XElement("restriction"));
            var date = new XElement("date");
            var endDate = new XAttribute("endate", DateTime.Now.AddMinutes(30).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
            date.Add(endDate);
            visibility.Add(date);
            item.Visibility = visibility;

            var result = item.ApplyRestrictedAccess(true);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ApplyRestrictedAccess_Should_Return_False_If_Visibility_Not_Set()
        {
            ContentItem item = new ContentItem();
            item.UserAccess = AccessLevel.Student;

            var result = item.ApplyRestrictedAccess();

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ApplyRestrictedAccess_Should_Return_True_If_Date_Saved_As_GMT()
        {
            context.Course = new Biz.DataContracts.Course()
            {
                Id = "1"
            };
            ContentItem item = new ContentItem();
            item.UserAccess = AccessLevel.Student;
            var visibility = new XElement("bfw_visibility");
            visibility.Add(new XElement("restriction"));
            var date = new XElement("date");
            var endDate = new XAttribute("endate", DateTime.Now.AddMinutes(30).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
            date.Add(endDate);
            visibility.Add(date);
            item.Visibility = visibility;

            var result = item.ApplyRestrictedAccess();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ApplyRestrictedAccess_Should_Return_False_If_Date_Saved_As_GMT()
        {
            context.Course = new Biz.DataContracts.Course()
            {
                Id = "1"
            };
            ContentItem item = new ContentItem();
            item.UserAccess = AccessLevel.Student;
            var visibility = new XElement("bfw_visibility");
            visibility.Add(new XElement("restriction"));
            var date = new XElement("date");
            var endDate = new XAttribute("endate", DateTime.Now.AddMinutes(-30).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
            date.Add(endDate);
            visibility.Add(date);
            item.Visibility = visibility;

            var result = item.ApplyRestrictedAccess();

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ApplyRestrictedAccess_Should_Return_True_If_Date_Saved_Without_Kind()
        {
            context.Course = new Biz.DataContracts.Course()
            {
                Id = "1"
            };
            ContentItem item = new ContentItem();
            item.UserAccess = AccessLevel.Student;
            var visibility = new XElement("bfw_visibility");
            visibility.Add(new XElement("restriction"));
            var date = new XElement("date");
            var endDate = new XAttribute("endate", DateTime.Now.AddMinutes(30).ToUniversalTime().ToString("MM/dd/yyyy HH:mm tt"));
            date.Add(endDate);
            visibility.Add(date);
            item.Visibility = visibility;

            var result = item.ApplyRestrictedAccess();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ApplyRestrictedAccess_Should_Return_False_If_Date_Saved_Without_Kind()
        {
            context.Course = new Biz.DataContracts.Course()
            {
                Id = "1"
            };
            ContentItem item = new ContentItem();
            item.UserAccess = AccessLevel.Student;
            var visibility = new XElement("bfw_visibility");
            visibility.Add(new XElement("restriction"));
            var date = new XElement("date");
            var endDate = new XAttribute("endate", DateTime.Now.AddMinutes(-30).ToUniversalTime().ToString("MM/dd/yyyy HH:mm tt"));
            date.Add(endDate);
            visibility.Add(date);
            item.Visibility = visibility;

            var result = item.ApplyRestrictedAccess();

            Assert.AreEqual(false, result);
        }

        #region Removable Setting

        [TestMethod]
        public void IsRemovable_Should_Return_True_If_Switch_is_True()
        {
            const string strXml = @"<items>
                              <item id=""TestId"">
                                <data>
                                  <parent>PX_MULTIPART_LESSONS</parent>
                                  <sequence>a</sequence>
                                  <title>Unit</title>
                                  <description>blabla</description>
                                  <bfw_type>TestType</bfw_type>
                                  <bfw_properties>
                                    <bfw_property name=""testName"" type=""String""></bfw_property>        
                                  </bfw_properties>
                                  <bfw_tocs>
                                    <bfw_toc_contents parentid=""PX_MULTIPART_LESSONS"" sequence=""a"" />
                                    <my_materials type=""bfw_toc"" parentid=""my_materials_167563"" sequence=""a"">my_materials</my_materials>
                                    <syllabusfilter type=""bfw_toc"" parentid=""PX_MULTIPART_LESSONS"" sequence=""_m"">syllabusfilter</syllabusfilter>
                                  </bfw_tocs>
                                  <meta-containers>
                                    <meta-container toc=""syllabusfilter"" dlaptype=""exact"">Launchpad</meta-container>
                                  </meta-containers>
                                  <meta-subcontainers>
                                    <meta-subcontainerid toc=""syllabusfilter"" dlaptype=""exact"">PX_MULTIPART_LESSONS</meta-subcontainerid>
                                  </meta-subcontainers>
                                </data>
                              </item>
                            </items>";

            var item = new Biz.DataContracts.ContentItem()
            {
                ItemDataXml = XElement.Parse(strXml)
            };

            var removableSetting = new RemovableSetting()
            {
                Switch = true
            };

            var isRemovable = item.IsRemovable(removableSetting);

            Assert.IsTrue(isRemovable);
        }

        [TestMethod]
        public void IsRemovable_Should_Return_True_If_Query_In_Filters_Matches_ItemDataXml()
        {
            const string strXml = @"<items>
                              <item id=""TestId"">
                                <data>
                                  <parent>PX_MULTIPART_LESSONS</parent>
                                  <sequence>a</sequence>
                                  <title>Unit</title>
                                  <description>blabla</description>
                                  <bfw_type>TestType</bfw_type>
                                  <bfw_properties>
                                    <bfw_property name=""testName"" type=""String""></bfw_property>        
                                  </bfw_properties>
                                  <bfw_tocs>
                                    <bfw_toc_contents parentid=""PX_MULTIPART_LESSONS"" sequence=""a"" />
                                    <my_materials type=""bfw_toc"" parentid=""my_materials_167563"" sequence=""a"">my_materials</my_materials>
                                    <syllabusfilter type=""bfw_toc"" parentid=""PX_MULTIPART_LESSONS"" sequence=""_m"">syllabusfilter</syllabusfilter>
                                  </bfw_tocs>
                                  <meta-containers>
                                    <meta-container toc=""syllabusfilter"" dlaptype=""exact"">Launchpad</meta-container>
                                  </meta-containers>
                                  <meta-subcontainers>
                                    <meta-subcontainerid toc=""syllabusfilter"" dlaptype=""exact"">PX_MULTIPART_LESSONS</meta-subcontainerid>
                                  </meta-subcontainers>
                                </data>
                              </item>
                            </items>";

            var item = new Biz.DataContracts.ContentItem()
            {
                ItemDataXml = XElement.Parse(strXml)
            };

            var removableSetting = new RemovableSetting()
            {
                Switch = true,
                XPathQueryFilter = "//bfw_tocs[my_materials='my_materials']"
            };

            var isRemovable = item.IsRemovable(removableSetting);

            Assert.IsTrue(isRemovable);
        }

        [TestMethod]
        public void IsRemovable_Should_Return_True_If_All_Queries_In_Filters_Matches_ItemDataXml()
        {
            const string strXml = @"<items>
                              <item id=""TestId"">
                                <data>
                                  <parent>PX_MULTIPART_LESSONS</parent>
                                  <sequence>a</sequence>
                                  <title>Unit</title>
                                  <description>blabla</description>
                                  <bfw_type>TestType</bfw_type>
                                  <bfw_properties>
                                    <bfw_property name=""testName"" type=""String""></bfw_property>        
                                  </bfw_properties>
                                  <bfw_tocs>
                                    <bfw_toc_contents parentid=""PX_MULTIPART_LESSONS"" sequence=""a"" />
                                    <my_materials type=""bfw_toc"" parentid=""my_materials_167563"" sequence=""a"">my_materials</my_materials>
                                    <syllabusfilter type=""bfw_toc"" parentid=""PX_MULTIPART_LESSONS"" sequence=""_m"">syllabusfilter</syllabusfilter>
                                  </bfw_tocs>
                                  <meta-containers>
                                    <meta-container toc=""syllabusfilter"" dlaptype=""exact"">Launchpad</meta-container>
                                  </meta-containers>
                                  <meta-subcontainers>
                                    <meta-subcontainerid toc=""syllabusfilter"" dlaptype=""exact"">PX_MULTIPART_LESSONS</meta-subcontainerid>
                                  </meta-subcontainers>
                                </data>
                              </item>
                            </items>";

            var item = new Biz.DataContracts.ContentItem()
            {
                ItemDataXml = XElement.Parse(strXml)
            };

            var removableSetting = new RemovableSetting()
            {
                Switch = true,
                XPathQueryFilter = "//bfw_tocs[my_materials='my_materials']/ancestor::data[bfw_type='TestType']"
            };

            var isRemovable = item.IsRemovable(removableSetting);

            Assert.IsTrue(isRemovable);
        }

        [TestMethod]
        public void IsRemovable_Should_Return_True_By_DEFAULT_If_NO_Filters()
        {
            const string strXml = @"<items>
                              <item id=""TestId"">
                                <data>
                                  <parent>PX_MULTIPART_LESSONS</parent>
                                  <sequence>a</sequence>
                                  <title>Unit</title>
                                  <description>blabla</description>
                                  <bfw_type>TestType</bfw_type>
                                  <bfw_properties>
                                    <bfw_property name=""testName"" type=""String""></bfw_property>        
                                  </bfw_properties>
                                  <bfw_tocs>
                                    <bfw_toc_contents parentid=""PX_MULTIPART_LESSONS"" sequence=""a"" />
                                    <my_materials type=""bfw_toc"" parentid=""my_materials_167563"" sequence=""a"">my_materials</my_materials>
                                    <syllabusfilter type=""bfw_toc"" parentid=""PX_MULTIPART_LESSONS"" sequence=""_m"">syllabusfilter</syllabusfilter>
                                  </bfw_tocs>
                                  <meta-containers>
                                    <meta-container toc=""syllabusfilter"" dlaptype=""exact"">Launchpad</meta-container>
                                  </meta-containers>
                                  <meta-subcontainers>
                                    <meta-subcontainerid toc=""syllabusfilter"" dlaptype=""exact"">PX_MULTIPART_LESSONS</meta-subcontainerid>
                                  </meta-subcontainers>
                                </data>
                              </item>
                            </items>";

            var item = new Biz.DataContracts.ContentItem()
            {
                ItemDataXml = XElement.Parse(strXml)
            };

            var removableSetting = new RemovableSetting()
            {
                Switch = true
            };

            var isRemovable = item.IsRemovable(removableSetting);

            Assert.IsTrue(isRemovable);
        }

        [TestMethod]
        public void IsRemovable_Should_Return_False_If_Switch_is_False()
        {
            const string strXml = @"<items>
                              <item id=""TestId"">
                                <data>
                                  <parent>PX_MULTIPART_LESSONS</parent>
                                  <sequence>a</sequence>
                                  <title>Unit</title>
                                  <description>blabla</description>
                                  <bfw_type>TestType</bfw_type>
                                  <bfw_properties>
                                    <bfw_property name=""testName"" type=""String""></bfw_property>        
                                  </bfw_properties>
                                  <bfw_tocs>
                                    <bfw_toc_contents parentid=""PX_MULTIPART_LESSONS"" sequence=""a"" />
                                    <my_materials type=""bfw_toc"" parentid=""my_materials_167563"" sequence=""a"">my_materials</my_materials>
                                    <syllabusfilter type=""bfw_toc"" parentid=""PX_MULTIPART_LESSONS"" sequence=""_m"">syllabusfilter</syllabusfilter>
                                  </bfw_tocs>
                                  <meta-containers>
                                    <meta-container toc=""syllabusfilter"" dlaptype=""exact"">Launchpad</meta-container>
                                  </meta-containers>
                                  <meta-subcontainers>
                                    <meta-subcontainerid toc=""syllabusfilter"" dlaptype=""exact"">PX_MULTIPART_LESSONS</meta-subcontainerid>
                                  </meta-subcontainers>
                                </data>
                              </item>
                            </items>";

            var item = new Biz.DataContracts.ContentItem()
            {
                ItemDataXml = XElement.Parse(strXml)
            };

            var removableSetting = new RemovableSetting()
            {
                Switch = false,
                XPathQueryFilter = "//bfw_tocs[my_materials='my_materials']"
            };

            var isRemovable = item.IsRemovable(removableSetting);

            Assert.IsFalse(isRemovable);
        }

        [TestMethod]
        public void IsRemovable_Should_Return_False_If_Query_In_Filters_DOES_NOT_Match_ItemDataXml()
        {
            const string strXml = @"<items>
                              <item id=""TestId"">
                                <data>
                                  <parent>PX_MULTIPART_LESSONS</parent>
                                  <sequence>a</sequence>
                                  <title>Unit</title>
                                  <description>blabla</description>
                                  <bfw_type>TestType</bfw_type>
                                  <bfw_properties>
                                    <bfw_property name=""testName"" type=""String""></bfw_property>        
                                  </bfw_properties>
                                  <bfw_tocs>
                                    <bfw_toc_contents parentid=""PX_MULTIPART_LESSONS"" sequence=""a"" />
                                    <syllabusfilter type=""bfw_toc"" parentid=""PX_MULTIPART_LESSONS"" sequence=""_m"">syllabusfilter</syllabusfilter>
                                  </bfw_tocs>
                                  <meta-containers>
                                    <meta-container toc=""syllabusfilter"" dlaptype=""exact"">Launchpad</meta-container>
                                  </meta-containers>
                                  <meta-subcontainers>
                                    <meta-subcontainerid toc=""syllabusfilter"" dlaptype=""exact"">PX_MULTIPART_LESSONS</meta-subcontainerid>
                                  </meta-subcontainers>
                                </data>
                              </item>
                            </items>";

            var item = new Biz.DataContracts.ContentItem()
            {
                ItemDataXml = XElement.Parse(strXml)
            };

            var removableSetting = new RemovableSetting()
            {
                Switch = true,
                XPathQueryFilter = "//bfw_tocs[my_materials='my_materials']"
            };

            var isRemovable = item.IsRemovable(removableSetting);

            Assert.IsFalse(isRemovable);
        }

        [TestMethod]
        public void IsRemovable_Should_Return_False_If_One_of_Many_Queries_In_Filters_DOES_NOT_Match_ItemDataXml()
        {
            const string strXml = @"<items>
                              <item id=""TestId"">
                                <data>
                                  <parent>PX_MULTIPART_LESSONS</parent>
                                  <sequence>a</sequence>
                                  <title>Unit</title>
                                  <description>blabla</description>
                                  <bfw_type>TestType</bfw_type>
                                  <bfw_properties>
                                    <bfw_property name=""testName"" type=""String""></bfw_property>        
                                  </bfw_properties>
                                  <bfw_tocs>
                                    <bfw_toc_contents parentid=""PX_MULTIPART_LESSONS"" sequence=""a"" />
                                    <syllabusfilter type=""bfw_toc"" parentid=""PX_MULTIPART_LESSONS"" sequence=""_m"">syllabusfilter</syllabusfilter>
                                  </bfw_tocs>
                                  <meta-containers>
                                    <meta-container toc=""syllabusfilter"" dlaptype=""exact"">Launchpad</meta-container>
                                  </meta-containers>
                                  <meta-subcontainers>
                                    <meta-subcontainerid toc=""syllabusfilter"" dlaptype=""exact"">PX_MULTIPART_LESSONS</meta-subcontainerid>
                                  </meta-subcontainers>
                                </data>
                              </item>
                            </items>";

            var item = new Biz.DataContracts.ContentItem()
            {
                ItemDataXml = XElement.Parse(strXml)
            };

            var removableSetting = new RemovableSetting()
            {
                Switch = true,
                XPathQueryFilter = "//bfw_tocs[my_materials='my_materials']/ancestor::data[bfw_type='PxUnit']"
            };

            var isRemovable = item.IsRemovable(removableSetting);

            Assert.IsFalse(isRemovable);
        }

        #endregion
    }
}