using System;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Tests
{
    [TestClass]
    public class ContextTests
    {
        private ISessionManager sessionManager;
        private ILogger logger;
        private ITraceManager tracer;
        private ICacheProvider cacheProvider;
        private IRAServices raServices;
        private Context context;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;

        [TestInitialize]

        public void TestInitialize()
        {
            modelProfileService = Substitute.For<IModelProfileService>();
            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();
            
            sessionManager = Substitute.For<ISessionManager>();
            logger = Substitute.For<ILogger>();
            tracer = Substitute.For<ITraceManager>();
            cacheProvider = Substitute.For<ICacheProvider>();
            raServices = Substitute.For<IRAServices>();

            var adminConnection = Substitute.ForPartsOf<DlapConnection>(new []{@"http://testdlap.com/"});
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string) x.Parameters["cmd"] == "getuserlist")).Returns(new DlapResponse(){ResponseXml = XDocument.Parse(userXmlResponseWihoutDummyCourseEnrollment)});
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "getuserenrollmentlist2")).Returns(new DlapResponse() { ResponseXml = XDocument.Parse(enrollmentWithoutDummyCourseXmlResponse) });
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "getuserlist" && (string)x.Parameters["userid"] == "434")).Returns(new DlapResponse() { ResponseXml = XDocument.Parse(userXmlResponseWihDummyCourseEnrollment) });
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "getuserenrollmentlist2" && (string)x.Parameters["userid"] == "434")).Returns(new DlapResponse() { ResponseXml = XDocument.Parse(enrollmentWithDummyCourseXmlResponse) });
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "createenrollments")).Returns(new DlapResponse(){Code =  DlapResponseCode.OK});

            context = new Context(sessionManager, logger, tracer, cacheProvider, raServices, adminConnection);

        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetSiteInfo_SiteUrl_NullResponseFromRA()
        {
            context.GetSiteInfo("");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetSiteInfo_SiteUrl_InvelidErrorStats()
        {

            raServices.GetSiteListByBaseUrl("url").ReturnsForAnyArgs(new RASiteInfo() { Error = new Error() { Code = "-1", Message = "test" } });
            context.GetSiteInfo("url");
        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetSiteInfo_SiteUrl_NullErrorStatusFromRA()
        {

            raServices.GetSiteListByBaseUrl("2.domain.com").ReturnsForAnyArgs(new RASiteInfo() { Error = null });
            context.GetSiteInfo("2.domain.com");
        } 
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetSiteInfo_SiteUrl_SitesInfoIsNull()
        {

            var context = new Context(sessionManager, logger, tracer, cacheProvider, raServices);
            raServices.GetSiteListByBaseUrl("lcl.domain.com").ReturnsForAnyArgs(new RASiteInfo() { Error = new Error() { Code = "1" } , Sites = new SiteInfoAndType[0]});
            context.GetSiteInfo("lcl.domain.com");
        }



        [TestMethod]
        public void GetSiteInfo_CorrectSiteUrl_CorrectSiteInfo()
        {

            raServices.GetSiteListByBaseUrl(@"http://dev.domain.com").Returns(new RASiteInfo()
            {
                Error = new Error() { Code = "1" },
                Sites = new[]
                        {
                            new SiteInfoAndType
                            {
                                AgilixCourseId = "13",
                                BaseUrl = "base",
                                SiteId = "3556"
                            }
                        }
            });
           var siteInfo =  context.GetSiteInfo("lcl.domain.com");
            Assert.IsTrue(siteInfo.AgilixSiteID == "13");
            Assert.IsTrue(siteInfo.SiteID == "3556");
            Assert.IsTrue(siteInfo.BaseURL == "base");
        }


        [TestMethod]
        public void Initialize_UserIdWithoutDummyCourseEnrollment_CorrectCurrentUser()
        {
            context.Initialize("1232");
            Assert.IsTrue(context.CurrentUser.Id == "1232");
            Assert.IsTrue(context.CurrentUser.DomainName == "onyx102584");
            Assert.IsTrue(context.CurrentUser.LastName == "Name1");
            Assert.IsTrue(context.CurrentUser.Username == "2345123");
            Assert.IsTrue(context.CurrentUser.Email == "test.test.instructor@macmillan.com");
            
        }


        [TestMethod]
        public void Initialize_UserIdWithoutAnyEnrollment_CorrectCurrentUser()
        {
            int counter = 0;
            var adminConnection = Substitute.ForPartsOf<DlapConnection>(new[] { @"http://testdlap.com/" });
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "getuserlist")).Returns(new DlapResponse() { ResponseXml = XDocument.Parse(userXmlResponseWihoutDummyCourseEnrollment) });
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "getuserenrollmentlist2")).Returns(new DlapResponse() { ResponseXml = XDocument.Parse("<enrollments></enrollments>") });
            adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "createenrollments")).Returns(new DlapResponse() { Code = DlapResponseCode.OK }).AndDoes(x => counter++);
            var context = new Context(sessionManager, logger, tracer, cacheProvider, raServices, adminConnection);
            
            
            context.Initialize("1232");
            Assert.IsTrue(context.CurrentUser.Id == "1232");
            Assert.IsTrue(context.CurrentUser.DomainName == "onyx102584");
            Assert.IsTrue(context.CurrentUser.LastName == "Name1");
            Assert.IsTrue(context.CurrentUser.Username == "2345123");
            Assert.IsTrue(context.CurrentUser.Email == "test.test.instructor@macmillan.com");
            Assert.IsTrue(counter == 1);

        }
         [TestMethod]
        public void Initialize_UserId_NewSessionStarted()
        {
            sessionManager.ResumeSession(null, null, null).ReturnsForAnyArgs((ISession) null);
            context.Initialize("434");
        }

         [TestMethod]
         public void Initialize_UserIdWithCorruptedEnrollment_ErrorCatched()
         {
             var adminConnection = Substitute.ForPartsOf<DlapConnection>(new[] { @"http://testdlap.com/" });
             adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "getuserlist")).Returns(new DlapResponse() { ResponseXml = XDocument.Parse(userXmlResponseWihoutDummyCourseEnrollment) });
             adminConnection.Send(Arg.Is<DlapRequest>(x => (string)x.Parameters["cmd"] == "getuserenrollmentlist2")).Returns(new DlapResponse() { ResponseXml = XDocument.Parse(enrollmentWithoutDummyCourseXmlResponse) });
             adminConnection.Send(Arg.Do<DlapRequest>(x =>
                                                      {
                                                          if ((string) x.Parameters["cmd"] == "createenrollments")
                                                          {
                                                              throw new WebException();
                                                          }
                                                          
                                                      }));

             var context = new Context(sessionManager, logger, tracer, cacheProvider, raServices, adminConnection);
             int counter = 0;
             logger.Log(Arg.Any<Exception>(), Arg.Do<LogSeverity>(x=>
                                                                  {
                                                                      if (x == LogSeverity.Error)
                                                                      {
                                                                          counter++;
                                                                      }
                                                                  }));
             sessionManager.ResumeSession(null, null, null).ReturnsForAnyArgs((ISession)null);
             context.Initialize("1232");
            Assert.IsTrue(counter == 1);
         }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Initialize_IncorrectUserId_DlapException()
        {
            var context = new Context(sessionManager, logger, tracer, cacheProvider, raServices);
            context.Initialize("32425456");
        }


        private const string userXmlResponseWihoutDummyCourseEnrollment = @"<users>
  <user userid=""1232"" userguid=""4343fdgfdgdfd"" userreference="""" firstname=""Name0"" lastname=""Name1"" reference="""" domainid=""1111"" domainname=""test domain"" userspace=""onyx102584"" username=""2345123"" email=""test.test.instructor@macmillan.com"" creationdate=""2014-02-19T23:46:46.027Z"" flags=""0"">
    <data>
      <pref_gradebook_185943-viewflags>10808740100</pref_gradebook_185943-viewflags>
      <pref_gradebook_185943-gradeview>9</pref_gradebook_185943-gradeview>
      <pref_gradebook_189337-viewflags>10808740100</pref_gradebook_189337-viewflags>
      <pref_gradebook_189337-gradeview>9</pref_gradebook_189337-gradeview>
      <pref_gradebook_197238-viewflags>10808740100</pref_gradebook_197238-viewflags>
      <pref_gradebook_197238-gradeview>9</pref_gradebook_197238-gradeview>
      <pref_gradebook_263510-viewflags>10808740100</pref_gradebook_263510-viewflags>
      <pref_gradebook_263510-gradeview>9</pref_gradebook_263510-gradeview>
      <pref_display_showexcused>false</pref_display_showexcused>
      <pref_display_showscores>true</pref_display_showscores>
    </data>
  </user>
  <user userid=""1232"" userguid=""4343fdgfdgdfd"" userreference="""" firstname=""Name0"" lastname=""Name1"" reference="""" domainid=""1111"" domainname=""test domain"" userspace=""onyx102584"" username=""6670125"" email=""test.test.instructor@macmillan.com"" creationdate=""2014-03-03T13:55:34.443Z"" flags=""0"" />
</users>";

        private const string enrollmentWithoutDummyCourseXmlResponse = @"<enrollments>
  <enrollment id=""197240"" userid=""1232"" entityid=""197238"" domainid=""66159"" reference="""" guid=""ea0855fb-c139-4e76-8793-3360d4e3d5ec"" flags=""4950738731008"" status=""1"" achieved=""0"" possible=""0"" failing=""false"" startdate=""2014-03-11T15:46:21.487Z"" enddate=""9999-12-31T23:59:59.997Z"">
    <data>
      <status>
        <responsiveness signal=""Green"" />
        <performance signal=""Green"" />
        <pace signal=""Green"" />
      </status>
    </data>
    <entity id=""197238"" entitytype=""C"" title=""Moore/Notz/Fligner, The Basic Practice of Statistics, 6e"" reference="""" guid=""35ca7f49-a98c-4ab1-8d21-ce5dda0ccd57"" domainid=""66159"" schema=""2"" protection=""0"" type=""Range"" startdate=""1753-01-01T00:00:00Z"" enddate=""9999-12-31T00:00:00Z"" days=""365"" term="""" baseid=""0"" />
    <domain id=""66159"" name=""Baruch College CUNY (New York, NY)"" />
  </enrollment>
</enrollments>";


        private const string userXmlResponseWihDummyCourseEnrollment = @"<users>
  <user userid=""434"" userguid=""4343fdgfdgdfd"" userreference="""" firstname=""Name0"" lastname=""Name1"" reference="""" domainid=""1111"" domainname=""test domain"" userspace=""onyx102584"" username=""6670125"" email=""test.test.instructor@macmillan.com"" creationdate=""2014-02-19T23:46:46.027Z"" flags=""0"">
    <data>
      <pref_gradebook_185943-viewflags>10808740100</pref_gradebook_185943-viewflags>
      <pref_gradebook_185943-gradeview>9</pref_gradebook_185943-gradeview>
      <pref_gradebook_189337-viewflags>10808740100</pref_gradebook_189337-viewflags>
      <pref_gradebook_189337-gradeview>9</pref_gradebook_189337-gradeview>
      <pref_gradebook_197238-viewflags>10808740100</pref_gradebook_197238-viewflags>
      <pref_gradebook_197238-gradeview>9</pref_gradebook_197238-gradeview>
      <pref_gradebook_263510-viewflags>10808740100</pref_gradebook_263510-viewflags>
      <pref_gradebook_263510-gradeview>9</pref_gradebook_263510-gradeview>
      <pref_display_showexcused>false</pref_display_showexcused>
      <pref_display_showscores>true</pref_display_showscores>
    </data>
  </user>
  <user userid=""434"" userguid=""4343fdgfdgdfd"" userreference="""" firstname=""Name0"" lastname=""Name1"" reference="""" domainid=""1111"" domainname=""test domain"" userspace=""onyx102584"" username=""6670125"" email=""test.test.instructor@macmillan.com"" creationdate=""2014-03-03T13:55:34.443Z"" flags=""0"" />
</users>";

        private const string enrollmentWithDummyCourseXmlResponse = @"<enrollments>
  <enrollment id=""197240"" userid=""434"" entityid=""197238"" domainid=""66159"" reference="""" guid=""ea0855fb-c139-4e76-8793-3360d4e3d5ec"" flags=""4950738731008"" status=""1"" achieved=""0"" possible=""0"" failing=""false"" startdate=""2014-03-11T15:46:21.487Z"" enddate=""9999-12-31T23:59:59.997Z"">
    <data>
      <status>
        <responsiveness signal=""Green"" />
        <performance signal=""Green"" />
        <pace signal=""Green"" />
      </status>
    </data>
    <entity id=""197238"" entitytype=""C"" title=""Moore/Notz/Fligner, The Basic Practice of Statistics, 6e"" reference="""" guid=""35ca7f49-a98c-4ab1-8d21-ce5dda0ccd57"" domainid=""66159"" schema=""2"" protection=""0"" type=""Range"" startdate=""1753-01-01T00:00:00Z"" enddate=""9999-12-31T00:00:00Z"" days=""365"" term="""" baseid=""0"" />
    <domain id=""66159"" name=""Baruch College CUNY (New York, NY)"" />
  </enrollment>
  <enrollment id=""382512"" userid=""185781"" entityid=""200117"" domainid=""66159"" reference="""" guid=""273e84b4-e13f-4442-b5a7-b3f43b1b7c2f"" flags=""4398046904320"" status=""1"" achieved=""0"" possible=""0"" failing=""false"" startdate=""2014-09-09T11:48:31.053Z"" enddate=""3014-09-09T11:48:31.053Z"">
    <data />
    <entity id=""200117"" entitytype=""C"" title=""QBA dummy course"" reference="""" guid=""4f9c3d27-4455-4696-8055-78f053782981"" domainid=""1"" schema=""2"" protection=""0"" type=""Range"" startdate=""2014-07-22T04:00:00Z"" enddate=""2025-07-22T04:00:00Z"" days=""365"" term="""" baseid=""0"" />
    <domain id=""66159"" name=""Baruch College CUNY (New York, NY)"" />
  </enrollment>
</enrollments>";
    }
}
