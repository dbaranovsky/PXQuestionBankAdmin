using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.Sql;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.Common.Collections;
using Bfw.Common.Patterns.Unity;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Services.Mappers;

using Bfw.PX.Biz.Direct.Services;

using Dapper;

using Mono.Options;

namespace Metadata
{
    class Program
    {
        private static ISession Session { get; set; }

        static void Main(string[] args)
        {
            ConfigureServiceLocator();

            var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();
            if (tracer != null)
                tracer.StartTracing();

            var options = new CommandOptions();
            var os = RegisterOptions(options);
            var sm = EstablishConnection();
            Session = sm.CurrentSession;
            var toProcess = new Dictionary<string, ContentItem>();
            var processed = new List<Item>();
            var promptBeforeClose = false;

            os.Parse(args);

            /* invocation entails passing params to cli as follows:
             *      /c:9 /f:PX_FLASHCARDS /v:ebook /a:search
             *      
             *      where: 
             *      /c = courseid
             *      /f = folderid
             *      /v = the catagories you wish to assign comma separated
             *      
             * Currently, category options are:
             *  - ebook
             *  - other 
             *  - exclude
             * 
             *      /a = the action to perform. By specifying "search" the command will apply
             *          the search categories. Leaving this out will result in the program processing
             *          metadata as it was originally intended to do.
             */

            if (options.Action.ToLowerInvariant() == "search")
            {
                ItemsToCategorize(options.CourseId, options.FolderId, options.Value, processed);
                // following chunks the records to process in case there are too many do to at once
                if (processed.Count > 100)
                {
                    int skip = 0;
                    List<Item> records = new List<Item>();
                    do
                    {
                        Console.WriteLine("Putting group: " + skip);
                        records = processed.Skip(skip).Take(100).ToList();
                        if (records.Count > 0) StoreItems(options.CourseId, records);
                        skip += 100;
                    } while (records.Count >= 100);
                }
                else StoreItems(options.CourseId, processed);
            }
            else if (options.Action.ToLowerInvariant() == "enrollmentstatus")
            {
                if (string.IsNullOrEmpty(options.Environment))
                {
                    Console.WriteLine("enrollmentstatus command requires an enviornment (ex. /e:DEV)");
                }
                else if (string.IsNullOrEmpty(options.UserId))
                {
                    Console.WriteLine("enrollmentstatus command requires a user id (ex. /u:123)");
                }
                else if (string.IsNullOrEmpty(options.Status))
                {
                    Console.WriteLine("enrollmentstatus command requires a status (ex. /s:10)");
                }
                else
                {
                    EnrollmentStatus(options);
                }
            }
            else if (options.Action.ToLowerInvariant() == "generatetestdata")
            {
                if (string.IsNullOrEmpty(options.Environment))
                {
                    Console.WriteLine("generatetestdata command requires an enviornment (ex. /e:DEV)");
                }
                else if (string.IsNullOrEmpty(options.DataFilePath))
                {
                    Console.WriteLine("generatetestdata command requires a datafile path (ex. /d:\"C:\file.txt\")");
                }
                else if (!File.Exists(options.DataFilePath))
                {
                    Console.WriteLine("The given file does not exist: {0}", options.DataFilePath);
                }
                else if (options.Count <= 0)
                {
                    Console.WriteLine("generatetestdata command requires a Count parameter greater than zero (ex. /count:10)");
                }
                else if (string.IsNullOrEmpty(options.ProductCourseId))
                {
                    Console.WriteLine("generatetestdata command requires a product course id (ex. /p:1234)");
                }
                else if (string.IsNullOrEmpty(options.DomainId))
                {
                    Console.WriteLine("generatetestdata command requires a domain id (ex. /m:1234)");
                }
                else if (string.IsNullOrEmpty(options.OutputFolder))
                {
                    Console.WriteLine("generatetestdata command requires an output_folder");
                }
                else if (string.IsNullOrEmpty(options.Title))
                {
                    Console.WriteLine("generatetestdata command requires a title");
                }
                else if (string.IsNullOrEmpty(options.URL))
                {
                    Console.WriteLine("generatetestdata command requires a url");
                }
                else if (string.IsNullOrEmpty(options.LoginURL))
                {
                    Console.WriteLine("generatetestdata command requires a login_url");
                }
                else
                {
                    GenerateTestData(options);
                }
                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "purgeenrollments")
            {
                if (string.IsNullOrEmpty(options.Environment))
                {
                    Console.WriteLine("purgeenrollments command requires an enviornment (ex. /e:DEV)");
                }
                else if (string.IsNullOrEmpty(options.DataFilePath) && string.IsNullOrEmpty(options.UserId))
                {
                    Console.WriteLine("purgeenrollments command requires a user id (ex. /u:123) or datafile path (ex. /d:\"C:\file.txt\")");
                }
                else if (!string.IsNullOrEmpty(options.DataFilePath) && !File.Exists(options.DataFilePath))
                {
                    Console.WriteLine("The given file does not exist: {0}", options.DataFilePath);
                }
                else
                {
                    PurgeEnrollments(options);
                }
                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "updatepasswords")
            {
                if (string.IsNullOrEmpty(options.Environment))
                {
                    Console.WriteLine("updatepasswords requires environment");
                }
                else if (string.IsNullOrEmpty(options.ParentDomain))
                {
                    Console.WriteLine("updatepasswords requires parent domain");
                }
                else if (string.IsNullOrEmpty(options.Password))
                {
                    Console.WriteLine("updatepasswords requires password");
                }
                else
                {
                    SetUserPasswords(options.Environment, options.ParentDomain, options.Password);
                    promptBeforeClose = true;
                }
            }
            else if (options.Action.ToLowerInvariant() == "updatereferencebyusername")
            {
                if (string.IsNullOrEmpty(options.Environment))
                {
                    Console.WriteLine("updatereferencebyusername requires environment");
                }
                else if (string.IsNullOrEmpty(options.Value))
                {
                    Console.WriteLine("updatereferencebyusername requires username search passed as value");
                }
                else
                {
                    UpdateReferenceIdByUserName(options.Environment, options.Value);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "publishcourse")
            {
                if (string.IsNullOrEmpty(options.SourceEnvironment))
                {
                    Console.WriteLine("publishcourse requires source_env");
                }
                else if (string.IsNullOrEmpty(options.SourceCourse))
                {
                    Console.WriteLine("publishcourse requires source_course");
                }
                else if (string.IsNullOrEmpty(options.DestinationEnvironment))
                {
                    Console.WriteLine("publishcourse requires dest_env");
                }
                else if (string.IsNullOrEmpty(options.DestinationCourse))
                {
                    Console.WriteLine("publishcourse requires dest_course");
                }
                else
                {
                    PublishCourse(options.SourceEnvironment, options.SourceCourse, options.DestinationEnvironment, options.DestinationCourse, string.Empty);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "movecoursedata")
            {
                if (string.IsNullOrEmpty(options.SourceCourse))
                {
                    Console.WriteLine("movecoursedata requires source_course");
                }
                else if (string.IsNullOrEmpty(options.DestinationCourse))
                {
                    Console.WriteLine("movecoursedata request dest_course");
                }
                else if (string.IsNullOrEmpty(options.Environment))
                {
                    Console.WriteLine("movecoursedata requires environment");
                }
                else
                {
                    var enrollments = MoveEnrollments(options.Environment, options.SourceCourse, options.DestinationCourse);
                    MoveStudentSubmissions(options.Environment, options.SourceCourse, options.DestinationCourse, enrollments);
                    MoveDiscussionBoards(options.Environment, options.SourceCourse, options.DestinationCourse);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "purgecourses")
            {
                if (string.IsNullOrEmpty(options.BeforeDate))
                {
                    Console.WriteLine("purgecourses command requires a before date be provided");
                }
                else
                {
                    DateTime before = DateTime.MinValue;

                    if (!DateTime.TryParse(options.BeforeDate, out before))
                    {
                        Console.WriteLine("date entered is not a valid date");
                    }
                    else if (string.IsNullOrEmpty(options.DomainId))
                    {
                        Console.WriteLine("purgecourses command requires a domain id");
                    }
                    else
                    {
                        string error = string.Empty;
                        PurgeCourses(before, options.DomainId, false, out error);

                        if (!string.IsNullOrEmpty(error))
                        {
                            Console.WriteLine(error);
                        }
                    }
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "updatedomainresources")
            {
                if (string.IsNullOrEmpty(options.Environment))
                {
                    Console.WriteLine("environment is required for updatedomainresources");
                }
                else if (string.IsNullOrEmpty(options.DomainId))
                {
                    Console.WriteLine("domain id is required for updatedomainresources");
                }
                else if (string.IsNullOrEmpty(options.ParentDomain))
                {
                    Console.WriteLine("parent domain is required for updatedomainresources");
                }
                else
                {
                    UpdateDomainResources(options.Environment, options.DomainId, options.ParentDomain);
                    UpdateDomainCustomizations(options.Environment, options.DomainId, options.ParentDomain);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "activate")
            {
                Activate(options);
            }
            else if (options.Action.ToLowerInvariant() == "importusers")
            {
                if (string.IsNullOrEmpty(options.DataFilePath))
                {
                    Console.WriteLine("importusers command requires an input datafile (csv file with fields [lastname:firstname:email:referenceid] no header row)");
                }
                else if (string.IsNullOrEmpty(options.DomainId))
                {
                    Console.WriteLine("importusers command requires a domain id");
                }
                else if (!File.Exists(options.DataFilePath))
                {
                    Console.WriteLine("specified datafile does not exist");
                }
                else
                {
                    ImportUsers(options);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "createprogram")
            {
                if (string.IsNullOrEmpty(options.UserId))
                {
                    Console.WriteLine("createprogram requires a userid to use as the program manager");
                }
                else if (string.IsNullOrEmpty(options.DomainId))
                {
                    Console.WriteLine("createprogram requires the domainid of the program");
                }
                else
                {
                    CreateProgram(options.UserId, options.DomainId);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "addtoprogram")
            {
                if (string.IsNullOrEmpty(options.DomainId))
                {
                    Console.WriteLine("addtoprogram requires domainid");
                }
                else if (string.IsNullOrEmpty(options.DataFilePath) || !File.Exists(options.DataFilePath))
                {
                    Console.WriteLine("addtoprogram requires a valid datafile");
                }
                else
                {
                    AddUsersToProgram(options.DataFilePath, options.DomainId);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "deleteusers")
            {
                if (string.IsNullOrEmpty(options.DomainId))
                {
                    Console.WriteLine("deleteusers requires a domainid");
                }
                else if (string.IsNullOrEmpty(options.DataFilePath) || !File.Exists(options.DataFilePath))
                {
                    Console.WriteLine("deleteusers requires a valid datafile");
                }
                else
                {
                    DeleteUsers(options.DataFilePath, options.DomainId);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "fixuserinfo")
            {
                if (string.IsNullOrEmpty(options.DomainId))
                {
                    Console.WriteLine("fixuserinfo requires a domainid");
                }
                else
                {
                    FixUserInfo(options.DomainId, options.ForceUpdate);
                }

                promptBeforeClose = true;
            }
            else if (options.Action.ToLowerInvariant() == "executedlapscript")
            {
                if (string.IsNullOrEmpty(options.DataFilePath))
                {
                    Console.WriteLine("executedlapscript requires a data file path");
                }
                else if (!File.Exists(options.DataFilePath))
                {
                    Console.WriteLine("specified datafile does not exist");
                }
                else if (string.IsNullOrWhiteSpace(options.CourseId) || string.IsNullOrWhiteSpace(options.DomainId) || string.IsNullOrWhiteSpace(options.Title))
                {
                    Console.WriteLine("executedlapscript requires a courseid, domainid, title");
                }
                else
                {
                    ExecuteDlapScript(options.DataFilePath, options.CourseId, options.DomainId, options.Title, options.Environment);
                }
                promptBeforeClose = true;
            }
            else
            {
                ItemsToProcess(options.CourseId, options.FolderId, toProcess);
                processed = ProcessItems(options, toProcess);
                StoreItems(options.CourseId, processed);
            }

            if (promptBeforeClose)
            {
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
            }
        }

        private static List<EnrollmentMap> MoveEnrollments(string environment, string source, string destination)
        {
            var map = new List<EnrollmentMap>();

            var getEnrollments = new GetEntityEnrollmentList
            {
                SearchParameters = new EntitySearch
                {
                    AllStatus = true,
                    CourseId = source
                }
            };

            try
            {
                Session.ExecuteAsAdmin(environment, getEnrollments);
                string sourceEnrollmentId = string.Empty;
                foreach (var enrollment in getEnrollments.Enrollments)
                {
                    var addEnrollment = new CreateEnrollment();

                    sourceEnrollmentId = enrollment.Id;
                    enrollment.Course.Id = destination;
                    enrollment.CourseId = destination;
                    addEnrollment.Add(enrollment);

                    try
                    {
                        Session.ExecuteAsAdmin(environment, addEnrollment);
                        map.Add(new EnrollmentMap { Source = sourceEnrollmentId, Destination = addEnrollment.Enrollments.First().Id });
                        Console.WriteLine("copied {0} enrollments into {1}", getEnrollments.Enrollments.Count, destination);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not copy enrollments into destination");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not get enrollments from source");
            }

            return map;
        }

        public static void MoveStudentSubmissions(string environment, string source, string destination, List<EnrollmentMap> enrollments)
        {
            try
            {
                foreach (var enrollment in enrollments)
                {
                    var getGrades = new GetGrades
                    {
                        SearchParameters = new GradeSearch
                        {
                            EnrollmentId = enrollment.Source
                        }
                    };

                    try
                    {
                        Session.ExecuteAsAdmin(environment, getGrades);

                        foreach (var grade in getGrades.Enrollments.First(e => e.Id == enrollment.Source).Grades)
                        {
                            var getSubmission = new GetStudentSubmission
                            {
                                SearchParameters = new SubmissionSearch
                                {
                                    EnrollmentId = enrollment.Source,
                                    ItemId = grade.Item.Id
                                }
                            };

                            try
                            {
                                Session.ExecuteAsAdmin(environment, getSubmission);

                                getSubmission.Submission.EnrollmentId = enrollment.Destination;
                                getSubmission.Submission.ItemId = grade.Item.Id;
                                var putSubmission = new PutStudentSubmission
                                {                
                                    Submission = getSubmission.Submission
                                };

                                Session.ExecuteAsAdmin(environment, putSubmission);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Could not get submission for item {0} with id {1}", grade.Item.Title, grade.Item.Id);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get grades for enrollment {0}", enrollment.Source);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not get enrollments from source");
            }
        }

        public static void MoveDiscussionBoards(string environment, string source, string destination)
        {
            var getBoards = new GetItems
            {
                SearchParameters = new ItemSearch
                {
                    EntityId = source,
                    Type = DlapItemType.Discussion
                }
            };

            try
            {
                Session.ExecuteAsAdmin(environment, getBoards);

                foreach (var board in getBoards.Items)
                {
                    var getMessageList = new DlapRequest
                    {
                        Type = DlapRequestType.Get,
                        Parameters = new Dictionary<string, object>()
                    };

                    getMessageList.Parameters["cmd"] = "getmessagelist";
                    getMessageList.Parameters["entityid"] = source;
                    getMessageList.Parameters["itemid"] = board.Id;

                    try
                    {
                        var messageList = Session.Send(getMessageList, asAdmin: true, environment: environment);
                        Console.WriteLine(messageList.ResponseXml);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("could not get message list for board: {0}", board.Title);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("could not list discussion board items");
            }
        }

        private static void ExecuteDlapScript(string filename, string courseid, string domainid, string title, string environmnt)
        {
            var text = File.ReadAllText(filename);
            text = text.Replace("-courseid-", courseid)
                .Replace("-entityid-", courseid)
                .Replace("-title-", title)
                .Replace("-domainid-", domainid);

            //set up request
            XDocument doc = XDocument.Parse(text);

            DlapRequest request = new DlapRequest()
                {
                    Type = DlapRequestType.Post,
                    ContentType = "text/xml",
                    SuppressWrapper =  true,
                    Mode = DlapRequestMode.Batch 
                };
            var batchElem = doc.Element("batch");
            if (batchElem != null)
            {
                request.AppendData(batchElem);
            }
            else
            {
                var requestElem = doc.Element("request");
                if (requestElem != null)
                {
                    request.AppendData(requestElem);
                    request.SuppressWrapper = false;
                }
            }

            //execute script try
            string error = string.Empty;
            DlapResponse response;
            try
            {
                response = Session.Send(request, true, environmnt);

                if (response.Code != DlapResponseCode.OK)
                {
                    error = response.Message;
                }
                else
                {
                    Console.WriteLine(response.ResponseXml.ToString());    
                }
                
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            Console.WriteLine(error);
        }

        private static void FixUserInfo(string domainId, bool forceUpdate = false)
        {
            var getUserList = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    DomainId = domainId
                }
            };

            try
            {
                Session.ExecuteAsAdmin(getUserList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not get user list for domain {0}, {1}", domainId, ex.Message);
                return;
            }

            if (getUserList.Users.IsNullOrEmpty())
            {
                Console.WriteLine("No users found in domain {0}", domainId);
            }
            else
            {
                var client = new RAServices();

                foreach (var user in getUserList.Users)
                {
                    var response = client.GetUserProfile(new string[] { user.Reference });

                    if (response.Error.Code.Equals("0") && !UserInfoMatchesRA(user, response.UserProfile.First()))
                    {
                        var profile = response.UserProfile.First();
                        Console.WriteLine("{0}, {1}, {2} | {3}, {4}, {5}",
                            profile.FirstName,
                            profile.LastName,
                            profile.Email,
                            user.FirstName, user.LastName, user.Email);

                        user.FirstName = profile.FirstName;
                        user.LastName = profile.LastName;
                        user.Email = profile.Email;
                        user.Credentials.Username = profile.Email;

                        var updateUser = new UpdateUsers();
                        updateUser.Add(user);

                        if (forceUpdate)
                        {
                            Session.ExecuteAsAdmin(updateUser);
                        }
                    }
                }
            }
        }

        private static bool UserInfoMatchesRA(AgilixUser user, UserProfile raProfile)
        {
            var matches = true;

            if (user.FirstName != raProfile.FirstName || user.FirstName.ToLowerInvariant() == "fname")
            {
                matches = false;
            }

            if (user.LastName != raProfile.LastName || user.LastName.ToLowerInvariant() == "lname") {
                matches = false;
            }

            if (user.Credentials.Username != raProfile.Email)
            {
                matches = false;
            }

            return matches;
        }

        private static void UpdateReferenceIdByUserName(string environment, string userName)
        {
            var getUserList = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    Username = userName
                }
            };

            try
            {
                Session.ExecuteAsAdmin(environment, getUserList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not get user list for username {0}, {1}", userName, ex.Message);
                return;
            }

            if (getUserList.Users.IsNullOrEmpty())
            {
                Console.WriteLine("No users found with username {0}", userName);
            }
            else
            {
                var cache = new Dictionary<string, dynamic>();
                var notFound = new List<string>();
                var found = new List<string>();
                dynamic raUser = null;
                foreach (var agxUser in getUserList.Users)
                {
                    if (!cache.ContainsKey(agxUser.Credentials.Username))
                    {
                        cache[agxUser.Credentials.Username] = GetRaUser(environment, agxUser.Credentials.Username);
                    }

                    raUser = cache[agxUser.Credentials.Username];

                    if (raUser != null)
                    {
                        agxUser.FirstName = raUser.firstName;
                        agxUser.LastName = raUser.lastName;
                        agxUser.Email = raUser.userEmail;
                        agxUser.Credentials.Username = raUser.userEmail;
                        agxUser.Reference = raUser.userID.ToString();

                        var updateUser = new UpdateUsers();
                        updateUser.Add(agxUser);

                        Session.ExecuteAsAdmin(environment, updateUser);

                        found.Add(agxUser.Credentials.Username);
                    }
                    else
                    {
                        notFound.Add(agxUser.Credentials.Username);
                    }
                }

                if (found.Count > 0)
                {
                    Console.WriteLine("the following users were fixed in DLAP:");

                    found.Distinct((a, b) => a.ToLowerInvariant() == b.ToLowerInvariant()).ToList().ForEach(x => Console.WriteLine(x));
                }

                if (notFound.Count > 0)
                {
                    Console.WriteLine("the following users were not found in RA");

                    notFound.Distinct((a, b) => a.ToLowerInvariant() == b.ToLowerInvariant()).ToList().ForEach(x => Console.WriteLine(x));
                }
            }
        }

        private static void ApplyRAPackage(string environment, string userId, int packageId)
        {
            var connectionString = string.Format("{0}RA", environment);
            using (var conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
            {
                conn.Open();

                conn.Execute("usp_AddUserPackage",
                                          new { userid = userId, packageid = packageId, expiration = DateTime.Now.AddYears(20) },
                                          commandType: CommandType.StoredProcedure, commandTimeout: 60 * 60);
            }
        }

        private static int FindRAPackage(string environment, string name)
        {
            int id = -1;
            var connectionString = string.Format("{0}RA", environment);
            using (var conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
            {
                conn.Open();

                var p = new DynamicParameters();
                p.Add("@szBaseURL", "");
                p.Add("@szSiteISBN", "");
                p.Add("@szPackageDescription", name);
                p.Add("@szPackageType", "");
                p.Add("@iLevelOfAccess", "");
                p.Add("@iLinesPerPage", 10);
                p.Add("@iPageNo", 1);
                p.Add("@szExceludePackageList", "");
                p.Add("@iSetRowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@iRetCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@szMsgTier1", dbType: DbType.String, direction: ParameterDirection.Output, size: 2048);
                p.Add("@szMsgTier2", dbType: DbType.String, direction: ParameterDirection.Output, size: 1024);
                p.Add("@szMsgTier3", dbType: DbType.String, direction: ParameterDirection.Output, size: 1024);

                var packageRows = conn.Query("svc.uspSearchPackages",
                                                               p
                                                               , commandType: CommandType.StoredProcedure,
                                                               commandTimeout: 90).ToList();

                var package = packageRows.FirstOrDefault();

                if (package != null)
                {
                    id = package.packageID;
                }
            }

            return id;
        }

        private static string RegisterRAUser(string environment, string userName, string firstName, string lastName)
        {
            var id = string.Empty;

            var connectionString = string.Format("{0}RA", environment);
            using (var conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
            {
                conn.Open();
                var p = new DynamicParameters();
                p.Add("@szUserEmail", userName, dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@szFirstName", firstName, dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@szLastName", lastName, dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@szPassword", "123456", dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@szPasswordhint", "first six", dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@bOptInEmail", false, dbType: DbType.Boolean, direction: ParameterDirection.Input);
                p.Add("@szInstructorEmail", "", dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@szMailPreferences", "", dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@szBaseUrl", "", dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@szRemoteIPAddr", "127.0.0.1", dbType: DbType.String, direction: ParameterDirection.Input);
                p.Add("@iUserID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@iRetCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@szMsgTier1", dbType: DbType.String, direction: ParameterDirection.Output, size: 2048);
                p.Add("@szMsgTier2", dbType: DbType.String, direction: ParameterDirection.Output, size: 1024);
                p.Add("@szMsgTier3", dbType: DbType.String, direction: ParameterDirection.Output, size: 1024);

                conn.Execute("svc.uspRegisterUser", p, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 90);

                var retCode = p.Get<int>("@iRetCode");
                var userID = p.Get<int>("@iUserID");
                if (retCode != 0 || userID <= 0)
                {
                    var msg = p.Get<string>("@szMsgTier2");
                    Console.WriteLine("error registering {0} -> {1}", userName, msg);
                }

                id = userID.ToString();
            }

            return id;
        }

        private static dynamic GetRaUser(string environment, string username)
        {
            dynamic user = null;
            var connectionString = string.Format("{0}RA", environment);
            using (var conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
            {
                conn.Open();

                var list = conn.Query("ShowRAUserInformation",
                    new
                    {
                        UserEmail = username,
                        FirstName = string.Empty,
                        LastName = string.Empty,
                        PageIndex = 1,
                        PageSize = 10,
                        CountRequired = 0,
                        exactMatch = "1"
                    }
                , commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 90).ToList();

                user = list.FirstOrDefault();
            }

            return user;
        }

        private static void DeleteUsers(string file, string domainId)
        {
            var records = File.ReadAllLines(file);
            var total = records.Length;
            var count = 0;
            var chunk = 25;
            var getDomain = new GetDomain { DomainId = domainId };

            try
            {
                Session.ExecuteAsAdmin(getDomain);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not load domain with id {0}: {1}", domainId, ex.Message);
                return;
            }

            if (getDomain.Domain == null)
            {
                Console.WriteLine("Could not load domain with id {0}", domainId);
                return;
            }

            Console.WriteLine("attempting to delete {0} users from domain {1}", total, domainId);

            foreach (var record in records)
            {
                var fields = record.Split(',');
                var error = string.Empty;

                //note that field zero isn't parameter1!
                DeleteUser(fields[3], domainId, out error);

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("error processing record: [{0},{1},{2},{3}] with message: {4}", fields[0], fields[1], fields[2], fields[3], error);
                }
                else
                {
                    ++count;
                }

                if ((count % chunk) == 0)
                {
                    Console.WriteLine("{0} of {1} deleted", count, total);
                }
            }

            Console.WriteLine("{0} of {1} deleted with {2} errors", count, total, total - count);
        }

        private static void DeleteUser(string referenceId, string domainId, out string error)
        {
            error = string.Empty;

            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    DomainId = domainId,
                    ExternalId = referenceId
                }
            };

            try
            {
                Session.ExecuteAsAdmin(getUser);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (getUser.Users.IsNullOrEmpty())
            {
                error = string.Format("no user with id {0}/{1} found", referenceId, domainId);
            }
            else
            {
                var deleteUser = new DlapRequest
                {
                    Type = DlapRequestType.Post,
                    Parameters = new Dictionary<string, object> {
                        { "cmd", "deleteusers" }
                    },
                    ContentType = "text/xml"
                };

                deleteUser.AppendData(XElement.Parse(string.Format("<requests><user userid=\"{0}\" /></requests>", getUser.Users.First().Id)));

                DlapResponse response;

                try
                {
                    response = Session.Send(deleteUser);

                    if (response.Code != DlapResponseCode.OK)
                    {
                        error = response.Message;
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
        }

        private static void AddUsersToProgram(string file, string domainId)
        {
            var db = new Bfw.Common.Database.DatabaseManager("PXData");
            var records = File.ReadAllLines(file).Filter(r => r != ",,,").ToArray();
            var total = records.Length;
            var count = 0;

            try
            {
                db.StartSession();
                foreach (var record in records)
                {
                    var fields = record.Split(',');

                    if (AddToProgram(db, string.Empty, fields[3], domainId))
                    {
                        ++count;
                    }

                    if (count % 25 == 0)
                    {
                        Console.WriteLine("{0} of {1} records processed", count, total);
                    }
                }

                Console.WriteLine("{0} of {1} records processed with {2} errors", count, total, total - count);
            }
            finally
            {
                db.EndSession();
            }
        }

        private static bool AddToProgram(Bfw.Common.Database.DatabaseManager db, string userId, string referenceId, string domainId)
        {
            var programId = GetProgramForDomain(db, domainId);
            var userAddedOk = false;

            if (programId < 0)
            {
                Console.WriteLine("no program with id {0} found", programId);
                return userAddedOk;
            }

            GetUsers getUser = null;
            string userIdString = userId;

            if (!string.IsNullOrEmpty(userId))
            {
                getUser = new GetUsers
                {
                    SearchParameters = new UserSearch
                    {
                        Id = userId
                    }
                };
            }
            else
            {
                userIdString = string.Format("{0}/{1}", domainId, referenceId);
                getUser = new GetUsers
                {
                    SearchParameters = new UserSearch
                    {
                        DomainId = domainId,
                        ExternalId = referenceId
                    }
                };
            }

            Session.ExecuteAsAdmin(getUser);

            if (getUser.Users.Count == 0)
            {
                Console.WriteLine("no user with id {0}", userIdString);
                return userAddedOk;
            }
            else
            {
                userId = getUser.Users.First().Id;
                referenceId = getUser.Users.First().Reference;
            }            

            try
            {
                db.ExecuteNonQuery("InsertUserProgram @0, @1, @2, @3, @4", programId, null, userId, referenceId, domainId);
                userAddedOk = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error insterting user with id {0}: {1}", userIdString, ex.Message);
            }

            return userAddedOk;
        }

        private static int GetProgramForDomain(Bfw.Common.Database.DatabaseManager db, string domainId)
        {
            int programId = -1;
            try
            {
                var records = db.Query("select Program_id from Programs where Program_manager_domain_id = @0", int.Parse(domainId));
                var program = records.FirstOrDefault();

                if (program != null)
                {
                    programId = program.Int32("Program_id");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error getting program for domain with id {0}: {1}", domainId, ex.Message);
            }

            return programId;
        }

        private static void CreateProgram(string userId, string domainId)
        {
            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    Id = userId
                }
            };

            Session.ExecuteAsAdmin(getUser);

            if (getUser.Users.IsNullOrEmpty())
            {
                Console.WriteLine("no user with id: {0} found. can not create program", userId);
                return;
            }

            var refId = getUser.Users.First().Reference;
            var db = new Bfw.Common.Database.DatabaseManager("PXData");
            try
            {
                db.StartSession();
                db.ExecuteNonQuery("InsertProgram @0, @1, @2, @3", null, userId, refId, domainId);
            }
            finally
            {
                db.EndSession();
            }
        }
        
        private static void ImportUsers(CommandOptions options)
        {
            var records = File.ReadAllLines(options.DataFilePath).Filter(r => r != ",,,").ToArray();
            var total = records.Length;
            var count = 0;
            var chunk = 25;
            var getDomain = new GetDomain { DomainId = options.DomainId };

            try
            {
                Session.ExecuteAsAdmin(getDomain);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not load domain with id {0}: {1}", options.DomainId, ex.Message);
                return;
            }

            if (getDomain.Domain == null)
            {
                Console.WriteLine("Could not load domain with id {0}", options.DomainId);
                return;
            }

            Console.WriteLine("attempting to import {0} users to domain {1}", total, options.DomainId);

            foreach (var record in records)
            {
                var fields = record.Split(',');
                var error = string.Empty;                

                //note that field zero isn't parameter1!
                ImportUser(fields[1], fields[0], fields[2], fields[3], options.DomainId, getDomain.Domain.Userspace, out error);

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("error processing record: [{0},{1},{2},{3}] with message: {4}", fields[0], fields[1], fields[2], fields[3], error);
                }
                else
                {
                    ++count;
                }

                if ((count % chunk) == 0)
                {
                    Console.WriteLine("{0} of {1} imported", count, total);
                }
            }

            Console.WriteLine("{0} of {1} imported with {2} errors", count, total, total - count);
        }

        private static void ImportUser(string firstName, string lastName, string eMail, string referenceId, string domainId, string domainName, out string error)
        {
            error = string.Empty;

            try
            {
                var getUser = new GetUsers
                {
                    SearchParameters = new UserSearch
                    {
                        DomainId = domainId,
                        ExternalId = referenceId
                    }
                };

                Session.ExecuteAsAdmin(getUser);

                if (getUser.Users.IsNullOrEmpty())
                {
                    getUser = new GetUsers
                    {
                        SearchParameters = new UserSearch
                        {
                            DomainId = domainId,
                            Username = eMail
                        }
                    };

                    Session.ExecuteAsAdmin(getUser);
                }

                if (getUser.Users.IsNullOrEmpty() && !string.IsNullOrEmpty(eMail))
                {
                    var credentials = new Credentials() { Username = eMail, Password = "123456", PasswordQuestion = "first six", PasswordAnswer = "integers" };
                    var domain = new Bfw.Agilix.DataContracts.Domain() { Id = domainId, Name = domainName };

                    var cmd = new CreateUsers();

                    cmd.Add(new AgilixUser()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = eMail,
                        Credentials = credentials,
                        Domain = domain,
                        Reference = referenceId
                    });

                    Session.ExecuteAsAdmin(cmd);
                }
                else if (!string.IsNullOrEmpty(eMail))
                {
                    var user = getUser.Users.First();
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    user.Email = eMail;
                    user.Reference = referenceId;

                    var cmd = new UpdateUsers();
                    cmd.Add(user);

                    Session.ExecuteAsAdmin(cmd);
                }                    
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private static void PurgeEnrollments(CommandOptions options)
        {
            var userIds = new List<string>();

            if (!string.IsNullOrEmpty(options.DataFilePath))
            {
                Console.WriteLine("Loading ids from data file: {0}", options.DataFilePath);
                userIds = ExternalIdsFromFile(options.DataFilePath);
            }
            else
            {
                Console.WriteLine("Using id: {0}", options.UserId);
                userIds.Add(options.UserId);
            }

            if (options.UseExternalId)
            {
                Console.WriteLine("Converting externalid to userids");
                userIds = UserIdsFromExternalIds(options.Environment, userIds);
            }

            var total = userIds.Count;
            var step = 50;
            var current = 0;

            Console.WriteLine("Purging courses for {0} users", total);
            foreach (string userId in userIds)
            {
                PurgeUserEnrollments(options.Environment, userId);

                ++current;

                if (current % step == 0)
                {
                    Console.WriteLine("Purged courses for {0} out of {1} users", current, total);
                }
            }

            Console.WriteLine("Completed purge of courses for {0} users", total);
        }

        private static void PurgeUserEnrollments(string environment, string userId)
        {
            var cmd = new GetUserEnrollmentList
            {
                SearchParameters = new EntitySearch
                {
                    UserId = userId
                },
                AllStatus = true
            };

            Session.ExecuteAsAdmin(environment, cmd);

            if (cmd.Enrollments.IsNullOrEmpty())
            {
                Console.WriteLine("No Enrollments were found for user with id: {0}", userId);
            }
            else
            {
                Console.WriteLine("Deleting {0} enrollments for user with id: {1}", cmd.Enrollments.Count, userId);

                var delete = new DeleteEnrollments
                {
                    Enrollments = cmd.Enrollments
                };

                try
                {
                    Session.ExecuteAsAdmin(environment, delete);
                    Console.WriteLine("Successfully deleted all enrollments for user with id: {0}", userId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while deleting enrollments for user with id: {0}, {1}", userId, ex.Message);
                }
            }
        }

        private static List<string> UserIdsFromExternalIds(string environment, List<string> externalIds, string domainId = "")
        {
            var userIds = new List<string>();

            foreach (var externalId in externalIds)
            {                
                userIds.AddRange(UserIdFromExternalId(environment, externalId, domainId));
            }

            return userIds;
        }

        private static List<string> UserIdFromExternalId(string environment, string externalId, string domainId = "")
        {
            var users = UserFromExternalId(environment, externalId, domainId);
            var userIds = users.Map(u => u.Id);

            return userIds.ToList();
        }

        private static List<Bfw.PX.Biz.DataContracts.UserInfo> UserFromExternalId(string environment, string externalId, string domainId = "")
        {
            var cmd = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    ExternalId = externalId
                }
            };

            Session.ExecuteAsAdmin(environment, cmd);

            var users = cmd.Users;

            if (!string.IsNullOrEmpty(domainId))
            {
                users.RemoveAll(u => u.Domain.Id != domainId);
            }

            return users.Map(u => u.ToUserInfo()).ToList();
        }

        private static List<string> ExternalIdsFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            return lines.ToList();
        }

        private static TestDataset ParseTestDatasetFile(string environment, string filePath, string domainId, string productCourseId)
        {
            var dataset = new TestDataset
            {
                Students = new List<Student>(),
                Instructors = new List<Instructor>()
            };

            var lines = File.ReadAllLines(filePath);
            var EXTERNALID = 0;
            var TYPE = 1;
            var STUDENT = "S";
            var INSTRUCTOR = "T";
            var lineNumber = 1;
            var total = lines.Length;
            var step = 100;

            Console.WriteLine("attempting to parse {0} line data file", total);
            foreach (var line in lines)
            {
                var fields = line.Split(',');
                if (fields[TYPE] == STUDENT)
                {
                    dataset.Students.Add(new Student
                    {
                        ExternalId = fields[EXTERNALID],
                        User = UserFromExternalId(environment, fields[EXTERNALID], domainId).First()
                    });
                }
                else if (fields[TYPE] == INSTRUCTOR)
                {
                    dataset.Instructors.Add(new Instructor
                    {
                        ExternalId = fields[EXTERNALID],
                        User = UserFromExternalId(environment, fields[EXTERNALID], domainId).First(),
                        Courses = new List<Bfw.PX.Biz.DataContracts.Course>()
                    });
                }
                else
                {
                    throw new Exception(string.Format("Bad input file format. Unrecognized User Type on line {0}", lineNumber));
                }

                if (lineNumber % step == 0)
                {
                    Console.WriteLine("done parsing {0} of {1} lines", lineNumber, total);
                }

                ++lineNumber;
            }

            var currentStudent = 0;
            var instructorStudentMax = 25;
            foreach (var instructor in dataset.Instructors)
            {
                for (var i = 0; (i < instructorStudentMax) && (currentStudent < dataset.Students.Count); ++i,++currentStudent)
                {
                    dataset.Students[currentStudent].InstructorExternalId = instructor.ExternalId;
                }
            }

            var getDomain = new GetDomain
            {
                DomainId = domainId
            };

            var getCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = productCourseId
                }
            };

            Session.ExecuteAsAdmin(environment, getDomain);
            Session.ExecuteAsAdmin(environment, getCourse);

            dataset.Domain = getDomain.Domain.ToDomain();
            dataset.Product = getCourse.Courses.First().ToCourse();

            return dataset;
        }

        private static void GenerateTestData(CommandOptions options)
        {
            var userIds = new List<string>();
                        
            Console.WriteLine("Loading ids from data file: {0}", options.DataFilePath);
            var dataset = ParseTestDatasetFile(options.Environment, options.DataFilePath, options.DomainId, options.ProductCourseId);
            
            CreateCoursesForInstructors(options.Environment, dataset, options.Count);
            EnrollStudentsInCoursesForInstructors(options.Environment, dataset);
            DumpTestDataToLoadRunnerFiles(options, dataset);
        }

        private static void DumpTestDataToLoadRunnerFiles(CommandOptions options, TestDataset dataset)
        {
            string instructorFile = string.Format("{0}\\Instructors.dat", options.OutputFolder);
            string studentFile = string.Format("{0}\\Students.dat", options.OutputFolder);
            string loginFile = string.Format("{0}\\LoginURL.dat", options.OutputFolder);

            DumpInstructorDataToLoadRunnerFile(instructorFile, options.Title, options.URL, options.Password, dataset);
            DumpStudentDataToLoadRunnerFile(studentFile, options.Title, options.URL, options.Password, dataset);
            DumpLoginDataToLoadRunnerFile(loginFile, options.LoginURL);
        }

        private static void DumpInstructorDataToLoadRunnerFile(string filePath, string title, string url, string password, TestDataset dataset)
        {
            Console.WriteLine("Generating Instructor LoadRunner File");
            var data = new StringBuilder();

            data.AppendLine("Username,Password,title,URL,mCourse");
            foreach (var instructor in dataset.Instructors)
            {
                foreach (var course in instructor.Courses)
                {
                    data.AppendFormat("{0},{1},{2},{3},{4}", instructor.User.Username, password, title, url, course.Id);
                    data.AppendLine();
                }
            }

            File.WriteAllText(filePath, data.ToString());
        }

        private static void DumpStudentDataToLoadRunnerFile(string filePath, string title, string url, string password, TestDataset dataset)
        {
            Console.WriteLine("Generating Student LoadRunner File");
            var data = new StringBuilder();

            data.AppendLine("sUsername,sPassword,title,sURL,mCourseID");
            foreach (var student in dataset.Students)
            {
                var instructor = dataset.Instructors.Find(i => i.ExternalId == student.InstructorExternalId);
                foreach (var course in instructor.Courses)
                {
                    data.AppendFormat("{0},{1},{2},{3},{4}", student.User.Username, password, title, url, course.Id);
                    data.AppendLine();
                }
            }

            File.WriteAllText(filePath, data.ToString());
        }

        private static void DumpLoginDataToLoadRunnerFile(string filePath, string loginUrl)
        {
            Console.WriteLine("Generating Login LoadRunner File");
            var sb = new StringBuilder();
            sb.AppendLine("ActivationURL,URLActivation");
            sb.AppendLine(string.Format("{0},{1}", loginUrl, loginUrl));

            File.WriteAllText(filePath, sb.ToString());
        }

        private static void CreateCoursesForInstructors(string environment, TestDataset dataset, int count)
        {
            var sm = new EnvironmentSessionManager(environment, new Bfw.Common.Logging.NullLogger(), new Bfw.Common.Logging.NullTraceManager());
            var domain = dataset.Domain;
            var product = dataset.Product;
            var totalInstructors = dataset.Instructors.Count;
            var currentInstructors = 0;
            var step = 10;

            Console.WriteLine("attempting to create courses for {0} instructors", totalInstructors);
            foreach (var instructor in dataset.Instructors)
            {  
                var user = instructor.User;

                Console.WriteLine("attempting to create {0} courses for instructor {1}", count, user.Id);
                for (int i = 0; i < count; ++i)
                {
                    var course = CreateCourseForInstructor(sm, user, product, domain);

                    if (course != null)
                    {
                        instructor.Courses.Add(course);
                    }
                }

                ++currentInstructors;

                if (currentInstructors % step == 0)
                {
                    Console.WriteLine("created courses for {0} out of {1} instructors", currentInstructors, totalInstructors);
                }
            }

            Console.WriteLine("created courses for {0} instructors", totalInstructors);
        }

        private static Bfw.PX.Biz.DataContracts.Course CreateCourseForInstructor(ISessionManager sessionManager, UserInfo user, Bfw.PX.Biz.DataContracts.Course product, Bfw.PX.Biz.DataContracts.Domain domain)
        {
            var context = SetupContextForInstructor(sessionManager, product, user, domain);
            var noteActions = new NoteActions(context, sessionManager);
            var db = new Bfw.Common.Database.DatabaseManager();
            var contentActions = new ContentActions(context, sessionManager, null, db);
            var domainActions = new DomainActions(context, sessionManager);
            var courseActions = new CourseActions(context, sessionManager, noteActions, contentActions, domainActions);

            string productId = context.Product.Id;
            string instructorName = user.FirstName + " " + user.LastName;
            string academicTerm = "2349a155-7a5c-4788-880f-f0b6fcb1ff8f2";
            string timezone = "Eastern Standard Time";
            string parentCourseId = context.Course.Id;
            string courseSubType = context.Course.SubType;
            bool retry = true;
            var input = string.Empty;

            Bfw.PX.Biz.DataContracts.Course course = null;
            Bfw.PX.Biz.DataContracts.Course parent = null;
            while (retry)
            {
                try
                {
                    parent = courseActions.GetCourseByCourseId(parentCourseId);
                    course = courseActions.CreateDerivedCourse(parent, domain.Id, string.Empty);

                    retry = false;
                }
                catch
                {
                    Console.Write("error creating course, retry? (y/n): ");
                    input = Console.ReadLine();

                    if (input.ToLowerInvariant() == "y")
                    {
                        retry = true;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            retry = true;
            input = string.Empty;

            course.Title = string.Format("{0} {1}", parent.Title, course.Id);
            course.AcademicTerm = academicTerm;
            course.CourseTimeZone = timezone;
            course.InstructorName = instructorName;
            course.CourseSubType = "regular";
            course.CourseType = "FACEPLATE";
            course.CourseHomePage = "PX_HOME_FACEPLATE";
            course.CourseOwner = context.CurrentUser.Id;
            course.DashboardCourseId = productId;
            course.ParentId = parentCourseId;
            course.DerivedCourseId = parentCourseId;

            while (retry)
            {
                try
                {
                    course = courseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { course }).First();
                    retry = false;
                }
                catch
                {
                    Console.Write("error updating course, retry? (y/n): ");
                    input = Console.ReadLine();

                    if (input.ToLowerInvariant() == "y")
                    {
                        retry = true;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            retry = true;
            input = string.Empty;

            while (retry)
            {
                try
                {
                    courseActions.ActivateCourse(course);
                    retry = false;
                }
                catch
                {
                    Console.Write("error activating course, retry? (y/n): ");
                    input = Console.ReadLine();

                    if (input.ToLowerInvariant() == "y")
                    {
                        retry = true;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
            return course;
        }

        private static void EnrollStudentsInCoursesForInstructors(string environment, TestDataset dataset)
        {
            var sm = new EnvironmentSessionManager(environment, new Bfw.Common.Logging.NullLogger(), new Bfw.Common.Logging.NullTraceManager());
            var total = dataset.Students.Count;
            var current = 0;
            var step = 100;

            Console.WriteLine("Enrolling {0} students", total);
            foreach (var student in dataset.Students)
            {
                var instructor = dataset.Instructors.Find(i => i.ExternalId == student.InstructorExternalId);

                foreach (var course in instructor.Courses)
                {
                    EnrollStudentInCourse(sm, student.User, dataset.Product, course, dataset.Domain);
                }

                Console.WriteLine("Enrolled student {0} into {1} courses for instructor {2}", student.User.ReferenceId, instructor.Courses.Count, instructor.User.ReferenceId);

                ++current;

                if (current % step == 0)
                {
                    Console.WriteLine("Enrolled {0} out of {1} students into courses", current, total);
                }
            }

            Console.WriteLine("Completed enrolling {0} students into courses", total);
        }

        private static void EnrollStudentInCourse(ISessionManager sessionManager, UserInfo user, Bfw.PX.Biz.DataContracts.Course product, Bfw.PX.Biz.DataContracts.Course course, Bfw.PX.Biz.DataContracts.Domain domain)
        {
            var context = SetupContextForStudent(sessionManager, product, course, user, domain);
            var noteActions = new NoteActions(context, sessionManager);
            var db = new Bfw.Common.Database.DatabaseManager();
            var contentActions = new ContentActions(context, sessionManager, null, db);
            var domainActions = new DomainActions(context, sessionManager);            
            var userActions = new UserActions(context, sessionManager, contentActions, domainActions);
            var enrollmentActions = new EnrollmentActions(context, sessionManager, noteActions, userActions, contentActions);

            var retry = true;
            var input = string.Empty;

            while (retry)
            {
                try
                {
                    enrollmentActions.CreateEnrollments(domain.Id, user.Id, course.Id, ConfigurationManager.AppSettings["StudentPermissionFlags"], "1", DateTime.Now, DateTime.Now.AddMonths(6), string.Empty, string.Empty, true);
                    retry = false;
                }
                catch
                {
                    Console.Write("error enrolling in course, retry? (y/n): ");
                    input = Console.ReadLine();

                    if (input.ToLowerInvariant() == "y")
                    {
                        retry = true;
                    }
                    else
                    {
                        retry = false;
                    }
                }
            }
        }

        private static Bfw.PX.Biz.ServiceContracts.IBusinessContext SetupContextForInstructor(ISessionManager sessionManager, Bfw.PX.Biz.DataContracts.Course product, UserInfo user, Bfw.PX.Biz.DataContracts.Domain domain)
        {
            var context = new ConsoleBusinessContext();

            context.AccessLevel = Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
            context.AccessType = Bfw.PX.Biz.ServiceContracts.AccessType.Adopter;
            context.CanCreateCourse = true;
            context.Course = product;
            context.CourseId = product.Id;
            context.CourseIsProductCourse = true;
            context.CurrentUser = user;
            context.DashboardCourseId = product.Id;
            context.Domain = domain;
            context.EntityId = product.Id;
            context.ImpersonateStudent = false;
            context.IsAnonymous = false;
            context.IsCourseReadOnly = true;
            context.IsPublicView = false;
            context.IsSharedCourse = false;
            context.Product = product;
            context.ProductCourseId = product.Id;

            sessionManager.CurrentSession = sessionManager.StartNewSession(string.Format("{0}/{1}", domain.Userspace, user.Username), "fakepassword", false, user.Id);
            
            return context;
        }

        private static Bfw.PX.Biz.ServiceContracts.IBusinessContext SetupContextForStudent(ISessionManager sessionManager, Bfw.PX.Biz.DataContracts.Course product, Bfw.PX.Biz.DataContracts.Course course, UserInfo user, Bfw.PX.Biz.DataContracts.Domain domain)
        {
            var context = new ConsoleBusinessContext();

            context.AccessLevel = Bfw.PX.Biz.ServiceContracts.AccessLevel.Student;
            context.AccessType = Bfw.PX.Biz.ServiceContracts.AccessType.Adopter;
            context.CanCreateCourse = false;
            context.Course = course;
            context.CourseId = course.Id;
            context.CourseIsProductCourse = false;
            context.CurrentUser = user;
            context.DashboardCourseId = product.Id;
            context.Domain = domain;
            context.EntityId = course.Id;
            context.ImpersonateStudent = false;
            context.IsAnonymous = false;
            context.IsCourseReadOnly = true;
            context.IsPublicView = false;
            context.IsSharedCourse = false;
            context.Product = product;
            context.ProductCourseId = product.Id;

            sessionManager.CurrentSession = sessionManager.StartNewSession(string.Format("{0}/{1}", domain.Userspace, user.Username), "fakepassword", false, user.Id);

            return context;
        }

        private static void EnrollmentStatus(CommandOptions options)
        {
            var cmd = new GetUserEnrollmentList
            {
                SearchParameters = new EntitySearch
                {
                    UserId = options.UserId                    
                },
                AllStatus = true
            };

            Session.ExecuteAsAdmin(options.Environment, cmd);

            if (cmd.Enrollments.IsNullOrEmpty())
            {
                Console.WriteLine("No Enrollments were found for user with id: {0}", options.UserId);
            }
            else
            {
                Console.WriteLine("Setting Status for {0} enrollments for user with id: {1}", cmd.Enrollments.Count, options.UserId);

                cmd.Enrollments.ForEach(e => e.Status = options.Status);

                var update = new UpdateEnrollments
                {
                    Enrollments = cmd.Enrollments
                };

                try
                {
                    Session.ExecuteAsAdmin(options.Environment, update);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while setting status for enrollments: {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Adds or sets the value of an existing metadatavalue on a content item
        /// </summary>
        /// <param name="options"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private static List<Item> ProcessItems(CommandOptions options, IDictionary<string, ContentItem> items)
        {
            var result = new List<Item>();
            foreach (var item in items.Values){
                if (item.Metadata.ContainsKey(options.Key)){
                    item.Metadata[options.Key].Value = options.Value;
                }else{
                    item.Metadata[options.Key] = new MetadataValue() { Value = options.Value, Type = options.Type };
                }
                result.Add(item.ToItem());
            }
            return result;
        }

        /// <summary>
        /// Recursively loads all items under parentId and puts them into the items dictionary
        /// </summary>
        /// <param name="parentId">item to get children of</param>
        /// <param name="items">dictionary of items indexed by id</param>
        private static void ItemsToProcess(string entityId, string parentId, IDictionary<string, ContentItem> items)
        {
            var cmd = ListChildren(entityId, parentId);
            Session.Execute(cmd);
            foreach (var item in cmd.Items)
            {
                if (!items.ContainsKey(item.Id))
                    items[item.Id] = item.ToContentItem();
                ItemsToProcess(entityId, item.Id, items);
            }
        }


        /// <summary>
        /// Recursively loads all items under parentId and puts them into the items dictionary
        /// </summary>
        /// <param name="parentId">item to get children of</param>
        /// <param name="items">dictionary of items indexed by id</param>
        private static void ItemsToCategorize(string entityId, string parentId, string strSearchCats, List<Item> items)
        {
            var cmd = ListChildren(entityId, parentId);
            Session.Execute(cmd);
            foreach (var item in cmd.Items){
                Console.WriteLine("Processing: " + item.Id); 
                bool hasSearchProperty = item.Data.Elements("meta-bfw_searchcategory").Any();
                if (!hasSearchProperty)
                    item.Data.Add(new XElement("meta-bfw_searchcategory", strSearchCats));
                else
                    item.Data.Element("meta-bfw_searchcategory").Value = strSearchCats;
                items.Add(item);
                ItemsToCategorize(entityId, item.Id, strSearchCats, items);
            }
        }

        private static void StoreItems(string entityId, List<Item> items)
        {
            if (items.Count == 0){
                Console.WriteLine("No items to process for : " + entityId);
                return;
            }

            var cmd = new PutItems();
            cmd.Add(items);
            Session.Execute(cmd);
            if (!cmd.Failures.IsNullOrEmpty())
            {
                foreach (var failure in cmd.Failures)
                {
                    Console.WriteLine("Failed putting item {0} with reason {1}", failure.ItemId, failure.Reason);
                }
            }
        }

        private static void StoreItems(string entityId, List<Item> items, String environment)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("No items to process for : " + entityId);
                return;
            }

            var cmd = new PutItems();
            cmd.Add(items);
            Session.ExecuteAsAdmin(environment, cmd);
            if (!cmd.Failures.IsNullOrEmpty())
            {
                foreach (var failure in cmd.Failures)
                {
                    Console.WriteLine("Failed putting item {0} with reason {1}", failure.ItemId, failure.Reason);
                }
            }
        }

        private static GetItems ListChildren(string entityId, string parentId)
        {
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = entityId,
                    Query = "/parent='" + parentId + "'",
                    Depth = 1
                }
            };
            return query;
        }

        private static GetItems ListChildren(string entityId, string parentId, int depth)
        {
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = entityId,
                    Query = "/parent='" + parentId + "'",
                    Depth = depth
                }
            };
            return query;
        }

        private static GetItems ListChildren(string entityId, string parentId, int depth, string categoryId)
        {
            var itemSearch = ItemQueryHelper.BuildListChildrenQuery(entityId, parentId, depth, categoryId, "");

            var query = new GetItems()
            {
                SearchParameters = itemSearch
            };
            return query;
        }

        private static void Activate(CommandOptions options)
        {
            if (string.IsNullOrEmpty(options.DataFilePath))
            {
                Console.WriteLine("You must specify the path to a data file containing one course id per line");
                return;
            }

            if (!File.Exists(options.DataFilePath))
            {
                Console.WriteLine("Data file specified does not exist");
                return;
            }

            var lines = File.ReadAllLines(options.DataFilePath);
            var trimed = lines.Map(s => s.Trim());

            ActivateCourses(trimed);
        }

        private static void ActivateCourses(IEnumerable<string> courseIds)
        {
            string date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

            foreach (var id in courseIds)
            {
                var getCourse = new GetCourse
                {
                    SearchParameters = new CourseSearch
                    {
                        CourseId = id
                    }
                };

                Session.Execute(getCourse);

                var course = getCourse.Courses.First().ToCourse();
                course.ActivatedDate = date;

                var updateCourse = new UpdateCourses();
                updateCourse.Add(course.ToCourse());

                Session.Execute(updateCourse);
            }
        }

        private static void PurgeCourses(DateTime before, string domainId, bool recursive, out string error)
        {
            int totalPurged = 0;
            int purgeBatchSize = 0;
            var purgedIds = new Dictionary<string, string>();
            error = string.Empty;

            Console.WriteLine("attempting to purge courses created before {0:MM/dd/yyyy}", before);

            do
            {
                var purgeList = CoursesToPurge(before, domainId);

                if (!ContinuePurge(purgeList, purgedIds))
                {
                    Console.WriteLine("cancelling purge because course was found in after already attempting to purge it.");
                    break;
                }

                purgeBatchSize = purgeList.Count();

                if (purgeBatchSize > 0)
                {
                    Console.WriteLine("purging {0} courses", purgeBatchSize);

                    totalPurged += purgeBatchSize;
                    //DeleteCourses(purgeList);
                }

            } while (purgeBatchSize > 0);

            Console.WriteLine("purge complete. purged {0} courses", totalPurged);
        }

        private static bool ContinuePurge(IEnumerable<string> batch, IDictionary<string, string> history)
        {
            var ok = true;

            foreach (var id in batch)
            {
                if (history.ContainsKey(id))
                {
                    Console.WriteLine("detected course with id {0} after purged", id);
                    ok = false;
                    break;
                }
            }

            return ok;
        }

        private static IEnumerable<string> CoursesToPurge(DateTime before, string domainId)
        {
            IEnumerable<string> purgeList = new List<string>();
            var cmd = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    DomainId = domainId,
                    Query = string.Format("/creationdate < {0:yyy-MM-ddT00:00:00Z}", before)
                }
            };

            Session.ExecuteAsAdmin(cmd);

            if (!cmd.Courses.IsNullOrEmpty())
            {
                XElement data;
                XElement courseType;
                XElement productId;
                var courseList = new List<Bfw.Agilix.DataContracts.Course>();
                GetCourse getCourse;

                foreach (var c in cmd.Courses)
                {
                    getCourse = new GetCourse
                    {
                        SearchParameters = new CourseSearch
                        {
                            CourseId = c.Id
                        }
                    };

                    Session.ExecuteAsAdmin(getCourse);

                    data = getCourse.Courses.First().Data;
                    courseType = data.Element("bfw_course_type");
                    productId = data.Element("meta-product-course-id");

                    //we only want to purge NON-DASHBOARD, NON-PRODUCT, NON-DISCIPLINE courses.
                    if ((courseType.Value.ToLowerInvariant().Contains("dashboard") == false) && productId != null)
                    {
                        courseList.Add(c);
                    }
                }

                purgeList = courseList.Map(c => c.Id);
            }

            return purgeList;
        }

        private static void DeleteCourses(IEnumerable<string> courseIds)
        {
            var cmd = new DeleteCourses();

            cmd.Add(courseIds.Map(c => new Bfw.Agilix.DataContracts.Course { Id = c }));

            Session.ExecuteAsAdmin(cmd);
        }

        /// <summary>
        /// Sets the password for all users in the target domain, or if recurive is true, the domain plus all
        /// child domains. If password is provided, then all users are given that password. If the password is
        /// NOT provided, then a password is calculated for the user.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="domainId"></param>
        /// <param name="recursive"></param>
        /// <param name="password"></param>
        public static void SetUserPasswords(string environment, string parentDomainId, string password)
        {
            var getParentDomain = new GetDomain
            {
                DomainId = parentDomainId
            };

            Session.ExecuteAsAdmin(environment, getParentDomain);

            if (getParentDomain.Domain == null)
            {
                Console.WriteLine("parent domain with id {0} does not exist", parentDomainId);
                return;
            }

            var getTargetDomains = new GetDomainList
            {
                RunAsync = true,
                SearchParameters = new Bfw.Agilix.DataContracts.Domain
                {
                    Id = parentDomainId
                }
            };

            Session.ExecuteAsAdmin(environment, getTargetDomains);

            if (getTargetDomains.Domains.IsNullOrEmpty())
            {
                Console.WriteLine("parent domain with id {0} does not have any children", parentDomainId);
                return;
            }

            var domainCount = getTargetDomains.Domains.Count();
            var domainsProcessed = 0;
            var userCount = 0;
            var usersProcessed = 0;

            Console.WriteLine("attempting to update passwords in {0} domains", domainCount);

            foreach (var domain in getTargetDomains.Domains)
            {
                var getUserList = new GetUsers
                {
                    RunAsync = true,
                    SearchParameters = new UserSearch
                    {
                        DomainId = domain.Id
                    }
                };

                Session.ExecuteAsAdmin(environment, getUserList);

                userCount = getUserList.Users.Count;
                usersProcessed = 0;
                Console.WriteLine("updating passwords for {0} users in domain {1}", userCount, domain.Id);
                foreach (var user in getUserList.Users)
                {
                    var updatePassword = new UpdatePassword
                    {
                        RunAsync = true,
                        UserId = user.Id,
                        Password = password
                    };

                    Session.ExecuteAsAdmin(environment, updatePassword);

                    if (!updatePassword.Success)
                    {
                        Console.WriteLine("error updating password for user: {0}", user.Id);
                    }

                    ++usersProcessed;

                    if (usersProcessed % 10 == 0)
                    {
                        Console.WriteLine("processed {0} users out of {1} for domain {2}", usersProcessed, userCount, domain.Id);
                    }
                }

                ++domainsProcessed;

                if (domainsProcessed % 10 == 0)
                {
                    Console.WriteLine("processed {0} domains out of {1}", domainsProcessed, domainCount);
                }
            }
        }

        /// <summary>
        /// Updates all resources in all children of parentDomainId with resources from sourceDomainId.
        /// </summary>
        /// <param name="sourceDomainId"></param>
        /// <param name="parentDomainId"></param>
        private static void UpdateDomainResources(string environment, string sourceDomainId, string parentDomainId)
        {
            var getSourceDomain = new GetDomain
            {
                DomainId = sourceDomainId
            };

            var getParentDomain = new GetDomain
            {
                DomainId = parentDomainId
            };

            Session.ExecuteAsAdmin(environment, getSourceDomain);

            if (getSourceDomain.Domain == null)
            {
                Console.WriteLine("source domain with id {0} does not exist", sourceDomainId);
                return;
            }

            Session.ExecuteAsAdmin(environment, getParentDomain);

            if (getParentDomain.Domain == null)
            {
                Console.WriteLine("parent domain with id {0} does not exist", parentDomainId);
                return;
            }

            var getTargetDomains = new GetDomainList
            {
                SearchParameters = new Bfw.Agilix.DataContracts.Domain
                {
                    Id = parentDomainId
                }
            };

            Session.ExecuteAsAdmin(environment, getTargetDomains);

            if (getTargetDomains.Domains.IsNullOrEmpty())
            {
                Console.WriteLine("parent domain with id {0} does not have any children", parentDomainId);
                return;
            }

            var count = getTargetDomains.Domains.Count();
            var item = 0;
            var errors = 0;

            Console.WriteLine("attempting to copy resources to {0} domains", count);
            foreach (var domain in getTargetDomains.Domains)
            {
                try
                {
                    ++item;
                    var copyResources = new CopyResources
                    {
                        SourceEntityId = sourceDomainId,
                        DestEntityId = domain.Id
                    };

                    Session.ExecuteAsAdmin(environment, copyResources);
                }
                catch
                {
                    Console.WriteLine("error copying resources for domain with id {0}", domain.Id);
                    ++errors;
                }

                if (item % 10 == 0)
                {
                    Console.WriteLine("Processing domain {0} of {1}", item, count);
                }
            }

            Console.WriteLine("Completed processing {0} of {1} domains with {2} errors", item, count, errors);
        }

        /// <summary>
        /// Updates all resources in all children of parentDomainId with resources from sourceDomainId.
        /// </summary>
        /// <param name="sourceDomainId"></param>
        /// <param name="parentDomainId"></param>
        private static void UpdateDomainCustomizations(string environment, string sourceDomainId, string parentDomainId)
        {
            var getSourceDomain = new GetDomain
            {
                DomainId = sourceDomainId
            };

            var getParentDomain = new GetDomain
            {
                DomainId = parentDomainId
            };

            Session.ExecuteAsAdmin(environment, getSourceDomain);

            if (getSourceDomain.Domain == null)
            {
                Console.WriteLine("source domain with id {0} does not exist", sourceDomainId);
                return;
            }

            Session.ExecuteAsAdmin(environment, getParentDomain);

            if (getParentDomain.Domain == null)
            {
                Console.WriteLine("parent domain with id {0} does not exist", parentDomainId);
                return;
            }

            var getTargetDomains = new GetDomainList
            {
                SearchParameters = new Bfw.Agilix.DataContracts.Domain
                {
                    Id = parentDomainId
                }
            };

            Session.ExecuteAsAdmin(environment, getTargetDomains);

            if (getTargetDomains.Domains.IsNullOrEmpty())
            {
                Console.WriteLine("parent domain with id {0} does not have any children", parentDomainId);
                return;
            }

            var count = getTargetDomains.Domains.Count();
            var item = 0;
            var errors = 0;

            Console.WriteLine("attempting to copy resources to {0} domains", count);

            var cmdUpdateDomains = new UpdateDomains();

            foreach (var domain in getTargetDomains.Domains)
            {
                ++item;

                domain.Data = getSourceDomain.Domain.Data;

                cmdUpdateDomains.Add(domain);            

                if (item % 10 == 0)
                {
                    Console.WriteLine("Processing domain {0} of {1}", item, count);
                }
            }

            try
            {
                Session.ExecuteAsAdmin(environment, cmdUpdateDomains);
                for (int i = 0; i < cmdUpdateDomains.Responses.Count; i++)
                {
                    if (cmdUpdateDomains.Responses[i].Code != DlapResponseCode.OK)
                    {
                        Console.WriteLine("Error updating domain customizations for domain: " + cmdUpdateDomains.Domains[i].Name);
                        ++errors;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error updating domain customizations.");
                
            }

            Console.WriteLine("Completed processing {0} of {1} domains with {2} errors", item, count, errors);
        }

        private static void PublishCourse(string fromEnvironment, string fromCourseId, string toEnvironment, string toCourseId, string toDisciplineId)
        {
            if (!PublishItems(fromEnvironment, fromCourseId, toEnvironment, toCourseId))
            {
                Console.WriteLine("could not publish items from course {0} to course {1}", fromCourseId, toCourseId);
                return;
            }

            if (!PublishQuestions(fromEnvironment, fromCourseId, toEnvironment, toCourseId))
            {
                Console.WriteLine("could not publish questions from course {0} to course {1}", fromCourseId, toCourseId);
                return;
            }

            if (!PublishResources(fromEnvironment, fromCourseId, toEnvironment, toCourseId))
            {
                Console.WriteLine("could not publish resources from course {0} to course {1}", fromCourseId, toCourseId);
            }
        }

        private static bool PublishItems(string fromEnvironment, string fromCourseId, string toEnvironment, string toCourseId)
        {
            var publishItemsOK = true;

            var itemsInSourceCourse = GetAllItemsInCourse(fromEnvironment, fromCourseId);
            var itemsInTargetCourse = GetAllItemsInCourse(toEnvironment, toCourseId);

            if (itemsInSourceCourse.IsNullOrEmpty() || itemsInTargetCourse.IsNullOrEmpty())
            {
                publishItemsOK = false;
            }
            else
            {
                var itemsToDelete = (from item in itemsInTargetCourse
                                    where !itemsInSourceCourse.Exists(i => i.Id.ToLowerInvariant() == item.Id.ToLowerInvariant())
                                    select item).ToList();

                var itemsToAddOrUpdate = (from item in itemsInSourceCourse
                                          where !itemsToDelete.Exists(i => i.Id.ToLowerInvariant() == item.Id.ToLowerInvariant())
                                          select item).ToList();

                if (!itemsToDelete.IsNullOrEmpty())
                {
                    itemsToDelete.ForEach(item => item.EntityId = toCourseId);
                    var deleteItems = new DeleteItems
                    {
                        Items = itemsToDelete,
                    };

                    try
                    {
                        Session.ExecuteAsAdmin(toEnvironment, deleteItems);
                        Console.WriteLine("deleted {0} items from target course", itemsToDelete.Count);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("could not delete items from target course {0}: {1}", toCourseId, ex.Message);
                        publishItemsOK = false;
                    }
                }
                else
                {
                    Console.WriteLine("no items to delete from target course");
                }

                if (publishItemsOK && !itemsToAddOrUpdate.IsNullOrEmpty())
                {
                    var putItems = new PutItems();
                    itemsToAddOrUpdate.ForEach(item => item.EntityId = toCourseId);
                    putItems.Add(itemsToAddOrUpdate);
                    putItems.RunAsync = true;

                    try
                    {
                        Console.WriteLine("attempting to add/update {0} items", itemsToAddOrUpdate.Count);
                        Session.ExecuteAsAdmin(toEnvironment, putItems);
                        Console.WriteLine("items add/updated");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("could not add or update items in target course {0}: {1}", toCourseId, ex.Message);
                        publishItemsOK = false;
                    }
                }
                
            }

            return publishItemsOK;            
        }

        private static bool PublishQuestions(string fromEnvironment, string fromCourseId, string toEnvironment, string toCourseId)
        {
            var publishQuestionsOK = true;

            var questionsInSourceCourse = GetAllQuestionsInCourse(fromEnvironment, fromCourseId);
            var questionsInTargetCourse = GetAllQuestionsInCourse(toEnvironment, toCourseId);


            var questionsToDelete = (from item in questionsInTargetCourse
                                     where !questionsInSourceCourse.Exists(i => i.Id.ToLowerInvariant() == item.Id.ToLowerInvariant())
                                     select item).ToList();

            var questionsToAddOrUpdate = (from item in questionsInSourceCourse
                                          where !questionsToDelete.Exists(i => i.Id.ToLowerInvariant() == item.Id.ToLowerInvariant())
                                          select item).ToList();

            if (!questionsToDelete.IsNullOrEmpty())
            {
                questionsToDelete.ForEach(question => question.EntityId = toCourseId);
                var deleteQuestions = new DeleteQuestions
                {
                    Questions = questionsToDelete.Map(q => q.ToEntity()).ToList()
                };

                try
                {
                    Session.ExecuteAsAdmin(toEnvironment, deleteQuestions);
                    Console.WriteLine("deleted {0} questions from target course", questionsToDelete.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("could not delete questions from target course {0}: {1}", toCourseId, ex.Message);
                    publishQuestionsOK = false;
                }
            }
            else
            {
                Console.WriteLine("no questions to delete from target course");
            }

            if (publishQuestionsOK && !questionsToAddOrUpdate.IsNullOrEmpty())
            {
                questionsToAddOrUpdate.ForEach(question => question.EntityId = toCourseId);

                try
                {
                    Console.WriteLine("attempting to add/update {0} questions", questionsToAddOrUpdate.Count);

                    foreach (var question in questionsToAddOrUpdate)
                    {
                        var putQuestion = new PutQuestions();
                        putQuestion.Add(question);

                        Session.ExecuteAsAdmin(toEnvironment, putQuestion);
                    }

                    Console.WriteLine("questions add/updated");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("could not add or update questions in target course {0}: {1}", toCourseId, ex.Message);
                    publishQuestionsOK = false;
                }
            }

            return publishQuestionsOK;
        }

        private static bool PublishResources(string fromEnvironment, string fromCourseId, string toEnvironment, string toCourseId)
        {
            var publishResourcesOK = true;

            var resourcesInSourceCourse = GetAllResourcesInCourse(fromEnvironment, fromCourseId);
            var resourcesInTargetCourse = GetAllResourcesInCourse(toEnvironment, toCourseId);


            var resourcesToDelete = (from item in resourcesInTargetCourse
                                     where !resourcesInSourceCourse.Exists(i => i.Url.ToLowerInvariant() == item.Url.ToLowerInvariant())
                                     select item).ToList();

            var resourcesToAddOrUpdate = (from item in resourcesInSourceCourse
                                          where !resourcesToDelete.Exists(i => i.Url.ToLowerInvariant() == item.Url.ToLowerInvariant())
                                          select item).ToList();

            if (!resourcesToDelete.IsNullOrEmpty())
            {
                resourcesToDelete.ForEach(resource => resource.EntityId = toCourseId);
                var deleteResources = new DeleteResources
                {
                    ResourcesToDelete = resourcesToDelete
                };

                try
                {
                    Session.ExecuteAsAdmin(toEnvironment, deleteResources);
                    Console.WriteLine("deleted {0} resources from target course", resourcesToDelete.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("could not delete resources from target course {0}: {1}", toCourseId, ex.Message);
                    publishResourcesOK = false;
                }
            }
            else
            {
                Console.WriteLine("no resources to delete from target course");
            }

            if (publishResourcesOK && !resourcesToAddOrUpdate.IsNullOrEmpty())
            {
                resourcesToAddOrUpdate.ForEach(resource => resource.EntityId = toCourseId);

                string currentPath = string.Empty;

                int totalResources = resourcesToAddOrUpdate.Count;
                int published = 0;

                Console.WriteLine("attempting to add/update {0} resources", totalResources);

                foreach (var resource in resourcesToAddOrUpdate)
                {
                    bool retry = true;

                    while (retry)
                    {
                        try
                        {
                            var getResource = new GetResource
                            {
                                EntityId = fromCourseId,
                                ResourcePath = resource.Url
                            };

                            currentPath = resource.Url;
                            Session.ExecuteAsAdmin(fromEnvironment, getResource);

                            getResource.Resource.GetStream().CopyTo(resource.GetStream());

                            var putResource = new PutResource
                            {
                                Resource = resource
                            };

                            Session.ExecuteAsAdmin(toEnvironment, putResource);

                            ++published;                            
                            if ((published % 10 == 0) || published >= totalResources)
                            {
                                Console.WriteLine("published {0} of {1} resources to course {2}", published, totalResources, toCourseId);
                            }
                            retry = false;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("error while publishing resource {0}, retry (y/n)?", currentPath);
                            var doRetry = Console.ReadLine();

                            if (!string.IsNullOrEmpty(doRetry) && doRetry.ToLowerInvariant()[0] == 'y')
                            {
                                retry = true;
                            }
                            else
                            {
                                retry = false;
                            }
                        }
                    }
                }

                Console.WriteLine("all resources add/updated");
            }

            return publishResourcesOK;
        }

        private static List<Item> GetAllItemsInCourse(string environment, string courseId)
        {
            var getAllItems = new GetItems
            {
                SearchParameters = new ItemSearch
                {
                    EntityId = courseId
                }
            };

            try
            {
                Session.ExecuteAsAdmin(environment, getAllItems);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting all items in course {0}: {1}", courseId, ex.Message);
            }

            return getAllItems.Items;
        }

        private static List<Bfw.Agilix.DataContracts.Question> GetAllQuestionsInCourse(string environment, string courseId)
        {
            var getQuestionList = new GetQuestions
            {
                SearchParameters = new QuestionSearch
                {
                    EntityId = courseId
                }
            };

            try
            {
                Session.ExecuteAsAdmin(environment, getQuestionList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting all questions in course {0}: {1}", courseId, ex.Message);
            }

            return getQuestionList.Questions.ToList();
        }

        private static List<Bfw.Agilix.DataContracts.Resource> GetAllResourcesInCourse(string environment, string courseId)
        {
            var getResourceList = new GetResourceList
            {
                EntityId = courseId
            };

            try
            {
                Session.ExecuteAsAdmin(environment, getResourceList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting all resources in course {0}: {1}", courseId, ex.Message);
            }

            return getResourceList.Resources;
        }

        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static OptionSet RegisterOptions(CommandOptions options)
        {
            var os = new OptionSet();
            os.Add("k:", k => options.Key = k);
            os.Add("v:", v => options.Value = v);
            os.Add("f:", f => options.FolderId = f);
            os.Add("c:", c => options.CourseId = c);
            os.Add("p:", p => options.ProductCourseId = p);
            os.Add("t:", t => options.Type = BizEntityExtensions.PropertyTypeFromString(t));
            os.Add("u:", u => options.UserId = u);
            os.Add("s:", s => options.Status = s);
            os.Add("a:", a => options.Action = a);
            os.Add("d:", d => options.DataFilePath = d);
            os.Add("m:", m => options.DomainId = m);
            os.Add("r:", r => options.ReferenceId = r);
            os.Add("z:", z => options.ForceUpdate = bool.Parse(z));
            os.Add("ti:", ti => options.Title = ti);
            os.Add("e:", e => options.Environment = e);
            os.Add("pm:", pm => options.ParentDomain = pm);
            os.Add("before:", before => options.BeforeDate = before);
            os.Add("source_course:", source_course => options.SourceCourse = source_course);
            os.Add("dest_course:", dest_course => options.DestinationCourse = dest_course);
            os.Add("source_env:", source_env => options.SourceEnvironment = source_env);
            os.Add("dest_env:", dest_env => options.DestinationEnvironment = dest_env);
            os.Add("password:", password => options.Password = password);
            os.Add("use_externalid:", extid => options.UseExternalId = Boolean.Parse(extid));
            os.Add("count:", count => options.Count = int.Parse(count));
            os.Add("url:", url => options.URL = url);
            os.Add("output_folder:", output => options.OutputFolder = output);
            os.Add("login_url:", login => options.LoginURL = login);
            return os;
        }

        private static ISessionManager EstablishConnection()
        {
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, false, username);
            return sm;
        }        
    }

    public class CommandOptions
    {
        /// <summary>
        /// Metadata key to add or overwrite
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Value of the Metadata key to use
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Id of the folder whose items should be marked
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// Id of the course the folder exists in
        /// </summary>
        public string CourseId { get; set; }

        public PropertyType Type { get; set; }

        public string Action { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; }

        public string DataFilePath { get; set; }

        /// <summary>
        /// Id of the course to copy items to
        /// </summary>
        public string ProductCourseId { get; set; }

        public string DomainId { get; set; }
        
        public string Title { get; set; }

        public string Environment { get; set; }

        public string ReferenceId { get; set; }

        public bool ForceUpdate { get; set; }

        public string ParentDomain { get; set; }

        public string BeforeDate { get; set; }

        public string SourceCourse { get; set; }
        public string DestinationCourse { get; set; }

        public string SourceEnvironment { get; set; }
        public string DestinationEnvironment { get; set; }

        public string Password { get; set; }

        public bool UseExternalId { get; set; }

        public int Count { get; set; }

        public string URL { get; set; }

        public string OutputFolder { get; set; }

        public string LoginURL { get; set; }
    }

    public class EnrollmentMap
    {
        public string Source { get; set; }
        public string Destination { get; set; }
    }

    public class Student
    {
        public Bfw.PX.Biz.DataContracts.UserInfo User { get; set; }
        public string ExternalId { get; set; }        
        public string InstructorExternalId { get; set; }
    }

    public class Instructor
    {
        public Bfw.PX.Biz.DataContracts.UserInfo User { get; set; }
        public string ExternalId { get; set; }
        public List<Bfw.PX.Biz.DataContracts.Course> Courses { get; set; }
    }

    public class TestDataset
    {
        public List<Instructor> Instructors { get; set; }
        public List<Student> Students { get; set; }
        public Bfw.PX.Biz.DataContracts.Course Product { get; set; }
        public Bfw.PX.Biz.DataContracts.Domain Domain { get; set; }
    }
}
