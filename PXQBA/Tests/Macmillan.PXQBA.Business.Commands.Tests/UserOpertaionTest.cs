using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class UserOpertaionTest
    {
        private IContext context;
        private IUserOperation userOperation; 
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;
        private IProductCourseOperation productCourseOperation;
        private IQuestionCommands questionCommands;
        private INoteCommands noteCommands;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IContext>();
            userOperation = new UserOperation(context);
            productCourseOperation = Substitute.For<IProductCourseOperation>();
            questionCommands = new QuestionCommands(context, productCourseOperation);
            noteCommands = Substitute.For<INoteCommands>();

            modelProfileService = new ModelProfileService(productCourseOperation, questionCommands, userOperation, noteCommands);
            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();
        }
        [TestMethod]
        public void GetUsers_UserNames_ListOfUsers()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetUsersBatch>(cmd =>
            {
                Assert.IsTrue(cmd.SearchParameters.Select(x=> x.Username).Contains("az4312"));
                cmd.ParseResponse(new DlapResponse(XDocument.Parse(usersResponse)));
            }));
            var users = userOperation.GetUsers(new List<string>()
                                              {
                                                  "az4312",
                                                  "czar"
                                              });
            Assert.IsTrue(users.Count() == 2);
            Assert.IsTrue(users.Select(x=> x.LastName).Contains("Art Deko"));
            Assert.IsTrue(users.Select(x => x.Id).Contains("czar"));
        }


        [TestMethod]
        public void GetUser_AgilixId_User()
        {
           
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetUsers>(cmd =>
                                                                                  {
                                                                                      Assert.IsTrue(cmd.SearchParameters.Id == "az4312");
                                                                                      cmd.ParseResponse(new DlapResponse(XDocument.Parse(userResponse)));
                                                                                      
                                                                                  }));
            var user = userOperation.GetUser("az4312");
            Assert.IsTrue(user.LastName == "Art Deko");
            Assert.IsTrue(user.Id == "az4312");

        }

        private const string usersResponse = @"<responses code=""OK""><responses>
                                                  <response code=""OK"">
                                                    <users>
                                                      <user userid=""az4312"" userguid=""c7121145-24ba-497a-9cd0-0e4cb1521f1d"" userreference=""mail@mai.l"" firstname=""firstname"" lastname=""Art Deko"" reference=""mail@mai.l"" domainid=""6650"" domainname=""BFWProducts"" userspace=""bfwproducts"" username=""11"" email=""mail@mai.l"" creationdate=""2012-10-03T18:36:33.967Z"" flags=""0"" />
                                                    </users>
                                                  </response>
                                               <response code=""OK"">
                                                    <users>
                                                      <user userid=""czar"" userguid=""4e8485f4-4a80-41b6-bd55-33714bb1aecb"" userreference=""mail@mai.l"" firstname=""firstname2"" lastname=""Art Deko"" reference=""mail@mai.l"" domainid=""110566"" domainname=""Tesd"" userspace=""onyx104805"" username=""11"" email=""mail@mai.l"" creationdate=""2013-05-14T15:06:07.243Z"" flags=""0"" />
                                                    </users>
                                                  </response>
                                               </responses></responses>";

        private const string userResponse = @"<users>
                                              <user userid=""az4312"" userguid=""16c39e56-fc69-45d8-a3ec-db523a7853f8"" userreference="""" firstname=""firstname"" lastname=""Art Deko"" reference="""" domainid=""66159"" domainname=""Baruch College CUNY (New York, NY)"" userspace=""dsf"" username=""6670125"" email=""lastname.lastname.instructor@macmillan.com"" creationdate=""2014-02-19T23:46:46.027Z"" flags=""0"">
                                                <data>
                                                  <pref_display_showscores>true</pref_display_showscores>
                                                </data>
                                              </user>
                                              <user userid=""191863"" userguid=""1a82cb41-040a-4351-9e92-04ec1887343d"" userreference="""" firstname=""firstname"" lastname=""Art Deko"" reference="""" domainid=""6650"" domainname=""BFWProducts"" userspace=""bfwproducts"" username=""6670125"" email=""lastname.lastname.instructor@macmillan.com"" creationdate=""2014-03-03T13:55:34.443Z"" flags=""0"" />
                                            </users>";
    }
}
