using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Schema;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

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
using Bfw.Agilix.ServiceContracts;
using Bfw.Agilix.Services;

using Mono.Options;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services;
using AgxSC = Bfw.Agilix.ServiceContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace ReportData
{
    class Program
    {
        private static ISession Session { get; set; }

        private static string sCourseID = string.Empty;
        private static string sDomainID = string.Empty;
        private static string sStartDate = string.Empty;
        private static string sEndDate = string.Empty;

        private static string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["Bfw.PX.Comments.Data.Properties.Settings.PX_CommentsConnectionString"].ToString();

        static void Main(string[] args)
        {
            try
            {
                var iCount = args.Count();
                switch (iCount)
                { 
                    case 1:
                        sDomainID = args[0].ToString() ;
                        sCourseID = string.Empty ;
                        break;
                    case 2:
                        //string sArg = args[1].ToString();
                        //int icourseid;
                        //bool res = int.TryParse(sArg, out icourseid);
                        //if (res == false)
                        //{
                        // date
                        //}
                        sDomainID = args[0].ToString();
                        sCourseID = args[1].ToString();
                        break;
                    case 3:
                        sDomainID = args[0].ToString();
                        sStartDate = args[1].ToString();
                        sEndDate = args[2].ToString();
                        break;
                    default:
                        break;

                }

                ConfigureServiceLocator();
                var options = new CommandOptions();
                var os = RegisterOptions(options);
                var sm = EstablishConnection();
                Session = sm.CurrentSession;
                var toProcess = new Dictionary<string, ContentItem>();
                var processed = new List<Item>();

                os.Parse(args);
                //* Main Submissions
                Console.WriteLine("Processing File Submission");
                FileSubmission(sm);
                Console.WriteLine("Processing Quiz Submission");
                QuizSubmission(sm);
                Console.WriteLine("Processing Content Submissions..");
                ContentSubmission(sm);
                //*
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }


        private static ISessionManager EstablishConnection()
        {
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, null);
            return sm;
        }

        private static OptionSet RegisterOptions(CommandOptions options)
        {
            var os = new OptionSet();
            os.Add("s:", f => options.LastsignalId = f);
            os.Add("t:", t => options.Type = BizEntityExtensions.PropertyTypeFromString(t));
            os.Add("a:", a => options.Action = a);
            return os;
        }

        private static void GetGradeList(string enrollmentId, string itemId, ISessionManager manager)
        {
            try
            {
                IGradeListService gradelistService = new GradeListService(manager);
                GradeListSearch gs1 = new GradeListSearch();
                gs1.EnrollmentId = enrollmentId;
                gs1.ItemId = itemId;
                XDocument gradelist = gradelistService.GetGradeList(gs1);
                //<grade status="1" responseversion="1" attempts="1" seconds="288" submittedversion="1" submitteddate="2011-05-06T17:42:30.703Z" />
                string sStatus = string.Empty;
                string sSeconds = string.Empty;
                foreach (var detail in gradelist.Descendants("grade"))
                {
                    if (detail.Attribute("status") != null)
                    {
                        sStatus = detail.Attribute("status").Value;
                    }
                    if (detail.Attribute("seconds") != null)
                    {
                        sSeconds = detail.Attribute("seconds").Value;
                    }
                }

                SqlConnection connection = new SqlConnection(strCon);
                string strInsert = "Insert into rpt_ContentSession(ContentSubmissionID,StatusID,Duration) values ('" + itemId + "','" + sStatus + "','" + sSeconds + "')";
                SqlCommand cmdinsert = new SqlCommand(strInsert, connection);
                connection.Open();
                cmdinsert.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private static void SignalList(string lastsignalid, string PackageType, ISessionManager manager)
        {
            try
            {
            ISignalService signalService = new SignalService(manager);
            //SignalActions signalActions = new SignalActions(signalService) ;
            //var signals = signalActions.GetSignalList(lastsignalid, domainId, "1.1");

            SignalSearch ss1 = new SignalSearch();
            ss1.LastSignalId = lastsignalid;
            ss1.SignalType = "1.1";
            XDocument signals = signalService.GetSignlaList(ss1);

            DataSet reportData = new DataSet();
            reportData.ReadXml(signals.CreateReader());                 
            if (reportData.Tables.Count > 0)
            {
                SqlConnection connection = new SqlConnection(strCon);
                SqlBulkCopy sbc = new SqlBulkCopy(connection);
                sbc.DestinationTableName = "SignalList";
                sbc.ColumnMappings.Add("signalid", "signalid");
                sbc.ColumnMappings.Add("domainid", "domainid");
                sbc.ColumnMappings.Add("type", "type");
                sbc.ColumnMappings.Add("entityid", "entityid");
                sbc.ColumnMappings.Add("creationdate", "creationdate");
                sbc.ColumnMappings.Add("creationby", "creationby");
                connection.Open();
                sbc.WriteToServer(reportData.Tables[0]);
                connection.Close();
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void GetStudentSubmission(string Enrollmentid, string itemId, string PackageType, string Filepath, ISessionManager manager)
        {
            try
            {
                IGradeService gradeService = new GradeService(manager);
                SubmissionSearch gs1 = new SubmissionSearch();
                gs1.EnrollmentId = Enrollmentid;
                gs1.ItemId = itemId;
                gs1.PackageType = PackageType;
                gs1.FilePath = Filepath;
                var submissionxml = gradeService.GetStudentSubmissionXML(gs1);
                var filteredQuestions = new XDocument(new XElement("questions"));
                if (submissionxml != null)
                {
                    foreach (var submission in submissionxml.Element("submission").Descendants("submission"))
                    {
                        filteredQuestions.Element("questions").Add(new XElement("question",
                                                    new XAttribute("answer", submission.Element("answer") == null ? "0" : submission.Element("answer").Value),
                                                    new XAttribute("enrollmentid", Enrollmentid),
                                                    new XAttribute("quizid", itemId),
                                                    new XAttribute("questionid", submission.Element("attemptquestion").Attribute("id").Value))
                                                    );
                    }
                    DataSet reportData = new DataSet();
                    reportData.ReadXml(filteredQuestions.CreateReader()); 
                    if (reportData.Tables.Count > 0)
                    {
                        SqlConnection connection = new SqlConnection(strCon);
                        SqlBulkCopy sbc = new SqlBulkCopy(connection);
                        sbc.DestinationTableName = "rpt_QuizSubmission";
                        sbc.ColumnMappings.Add("quizid", "QuizID");
                        sbc.ColumnMappings.Add("questionid", "QuestionID");
                        sbc.ColumnMappings.Add("enrollmentid", "EnrollmentID");
                        sbc.ColumnMappings.Add("answer", "StudentAnswer");
                        connection.Open();
                        sbc.WriteToServer(reportData.Tables[0]);
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void GetStudentSubmissionHistory(string Enrollmentid, string itemId, ISessionManager manager)
        {
            try
            {
                IGradeService gradeService = new GradeService(manager);
                SubmissionSearch gs1 = new SubmissionSearch();
                gs1.EnrollmentId = Enrollmentid;
                gs1.ItemId = itemId;
                List<Bfw.Agilix.DataContracts.Submission> submissions = gradeService.GetSubmissions(gs1).ToList();

                XDocument xdoc = new XDocument(new XElement("submissions"));

                foreach (var submission in submissions)
                {
                    xdoc.Element("submissions").Add(new XElement("submission", new XAttribute("itemid", itemId), new XAttribute("submitteddate", submission.SubmittedDate), new XAttribute("enrollmentid", Enrollmentid)));
                    //xdoc.Element("submissions").Add(new XElement("submission", new XAttribute("itemid", submission.ItemId), new XAttribute("enrollmentid", submission.EnrollmentId)));
                }

                // Insertng values into database
                // File Submission table
                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader()); 
                if (reportData.Tables.Count > 0)
                {
                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_FileSubmission";
                    sbc.ColumnMappings.Add("itemid", "FileSubmissionID");
                    sbc.ColumnMappings.Add("submitteddate", "FileSubmissionDate");
                    sbc.ColumnMappings.Add("itemid", "AssignmentID");
                    sbc.ColumnMappings.Add("enrollmentid", "EnrollmentID");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void GetQuestionList(string EntityId, IEnumerable<string> QuestionId, ISessionManager manager)
        {
            try
            {
                IQuestionService questionService = new QuestionService(manager);
                QuestionSearch gs1 = new QuestionSearch();
                gs1.EntityId = EntityId;
                gs1.QuestionIds = QuestionId;
                List<Bfw.Agilix.DataContracts.Question> questions = questionService.GetQuestions(gs1).ToList();
                XDocument questiondoc = new XDocument(new XElement("questions"));
                XDocument answerdoc = new XDocument(new XElement("answers"));
                foreach (var question in questions)
                {
                    questiondoc.Element("questions").Add(new XElement("question",
                        new XAttribute("id", question.Id),
                        new XAttribute("questiontext", question.Body),
                        new XAttribute("questiontype", question.InteractionType)));
                    if (question.Choices.Count > 0)
                    {
                        for (int i = 0; i < question.Choices.Count; i++)
                        {
                            answerdoc.Element("answers").Add(new XElement("answer",
                            new XAttribute("questionid", question.Id),
                            new XAttribute("correctanswer", question.Answer),
                            new XAttribute("answertext", question.Choices[i].Text)
                            ));
                        }
                    }
                    else
                    {
                            answerdoc.Element("answers").Add(new XElement("answer",
                            new XAttribute("questionid", question.Id),
                            new XAttribute("correctanswer", question.Answer == null ? "0" : question.Answer),
                            new XAttribute("answertext", "No Answer")
                            ));
                    }

                }

                // Insertng values into database
                // questionmaster table
                DataSet reportData = new DataSet();
                reportData.ReadXml(questiondoc.CreateReader()); 
                if (reportData.Tables.Count > 0)
                {
                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_QuestionMaster";
                    sbc.ColumnMappings.Add("id", "QuestionID");
                    sbc.ColumnMappings.Add("questiontext", "QuestionText");
                    sbc.ColumnMappings.Add("questiontype", "QuestionType");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
                // student table
                DataSet studentData = new DataSet();
                studentData.ReadXml(answerdoc.CreateReader()); 
                if (studentData.Tables.Count > 0)
                {
                    SqlConnection connection1 = new SqlConnection(strCon);
                    SqlBulkCopy sbc1 = new SqlBulkCopy(connection1);
                    sbc1.DestinationTableName = "rpt_AnswerDetail";
                    sbc1.ColumnMappings.Add("questionid", "Questionid");
                    sbc1.ColumnMappings.Add("answertext", "AnswerText");
                    sbc1.ColumnMappings.Add("correctanswer", "CorrectAnswer");
                    connection1.Open();
                    sbc1.WriteToServer(studentData.Tables[0]);
                    connection1.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


        private static void GetEntityEnrollmentList(string courseId, string sectionId, string userId, ISessionManager manager)
        {
            try
            {
                IEnrollmentService enrollmentService = new EnrollmentService(manager);
                EntitySearch es1 = new EntitySearch();
                es1.CourseId = courseId;
                es1.SectionId = sectionId;
                es1.UserId = userId;

                List<Bfw.Agilix.DataContracts.Enrollment> enrollments = enrollmentService.GetEntityEnrollments(es1).ToList();

                XDocument xdoc = new XDocument(new XElement("enrollments"));
                XDocument studentdoc = new XDocument(new XElement("students"));
                foreach (var enrollment in enrollments)
                {
                    xdoc.Element("enrollments").Add(new XElement("enrollment", new XAttribute("id", enrollment.Id), new XAttribute("studentid", enrollment.User.Id), new XAttribute("courseid", enrollment.Course.Id)));
                    studentdoc.Element("students").Add(new XElement("student", new XAttribute("studentid", enrollment.User.Id), new XAttribute("studentname", enrollment.User.FirstName + ' ' + enrollment.User.LastName)));
                }

                // Insertng values into database
                // enrollment table
                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader());  
                if (reportData.Tables.Count >0 )
                {
                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_Enrollment";
                    sbc.ColumnMappings.Add("id", "EnrollmentID");
                    sbc.ColumnMappings.Add("studentid", "StudentID");
                    sbc.ColumnMappings.Add("courseid", "CourseID");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
                // student table
                
                DataSet studentData = new DataSet();
                studentData.ReadXml(studentdoc.CreateReader());
                if (studentData.Tables.Count >0)
                {
                    SqlConnection connection1 = new SqlConnection(strCon);
                    SqlBulkCopy sbc1 = new SqlBulkCopy(connection1);
                    sbc1.DestinationTableName = "rpt_StudentMaster";
                    sbc1.ColumnMappings.Add("studentid", "StudentID");
                    sbc1.ColumnMappings.Add("studentname", "StudentName");
                    connection1.Open();
                        sbc1.WriteToServer(studentData.Tables[0]);
                    connection1.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void GetAssignmentList(string entityId, string query, ISessionManager manager)
        {
            try
            {
                IItemService itemService = new ItemService(manager);
                ItemSearch is1 = new ItemSearch();
                is1.EntityId = entityId;
                is1.Query = query;
                List<Bfw.Agilix.DataContracts.Item> items = itemService.SearchItems(is1).ToList();

                XDocument xdoc = new XDocument(new XElement("items"));

                foreach (var item in items)
                {
                    xdoc.Element("items").Add(new XElement("item", 
                                              new XAttribute("id", item.Id), 
                                              new XAttribute("name", item.Title == null ? "No Title" : item.Title),
                                              new XAttribute("entityid", item.EntityId)));
                }
                // Insertng values into database

                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader()); 
                if (reportData.Tables.Count > 0)
                {

                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_AssignmentMaster";
                    sbc.ColumnMappings.Add("id", "AssignmentID");
                    sbc.ColumnMappings.Add("name", "AssignmentName");
                    sbc.ColumnMappings.Add("entityid", "CourseID");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void GetAssessmentList(string entityId, string query, ISessionManager manager)
        {
            try
            {
                IItemService itemService = new ItemService(manager);
                ItemSearch is1 = new ItemSearch();
                is1.EntityId = entityId;
                is1.Query = query;
                List<Bfw.Agilix.DataContracts.Item> items = itemService.SearchItems(is1).ToList();

                XDocument xdoc = new XDocument(new XElement("items"));

                foreach (var item in items)
                {
                    XDocument questions = new XDocument(item.Data);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(questions.CreateReader()); 
                    XmlNodeList elemList = doc.GetElementsByTagName("question");
                    List<string> questionList = new List<string>();
                    if (elemList.Count > 0)
                    {
                        for (int i = 0; i < elemList.Count; i++)
                        {
                            if (questionList == null)
                            {
                                questionList.Add(elemList[i].Attributes["id"].Value);
                            }
                            else
                            {
                                questionList.Add(questionList + "|" + elemList[i].Attributes["id"].Value);
                            }
                        }
                    }
                    if (questionList.Count > 0)
                    {
                        GetQuestionList(entityId, questionList, manager);
                    }
                    //quiz document
                    xdoc.Element("items").Add(new XElement("item",
                        new XAttribute("id", item.Id),
                        new XAttribute("name", item.Title),
                        new XAttribute("courseid", item.EntityId)
                        ));
                }
                // Insertng values into quiz table

                DataSet reportData1 = new DataSet();
                reportData1.ReadXml(xdoc.CreateReader());  
                if (reportData1.Tables.Count > 0)
                {
                    SqlConnection connection1 = new SqlConnection(strCon);
                    SqlBulkCopy sbc1 = new SqlBulkCopy(connection1);
                    sbc1.DestinationTableName = "rpt_QuizMaster";
                    sbc1.ColumnMappings.Add("id", "QuizID");
                    sbc1.ColumnMappings.Add("name", "QuizTitle");
                    sbc1.ColumnMappings.Add("courseid", "CourseID");
                    connection1.Open();
                    sbc1.WriteToServer(reportData1.Tables[0]);
                    connection1.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        //private static void GetGroupList(string courseId, int setId, ISessionManager manager)
        //{
        //    try
        //    {
        //        IGroupService groupService = new GroupService(manager);
        //        Bfw.Agilix.DataContracts.Group g1 = new Bfw.Agilix.DataContracts.Group();
        //        List<Bfw.Agilix.DataContracts.Group> grouplist = groupService.GetGroupList(courseId ,setId).ToList() ;

        //        XDocument xdoc = new XDocument(new XElement("grouplist"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        private static void GetWikiList(string entityId, string query, ISessionManager manager)
        {
            try
            {
                IItemService itemService = new ItemService(manager);
                ItemSearch is1 = new ItemSearch();
                is1.EntityId = entityId;
                is1.Query = query;
                List<Bfw.Agilix.DataContracts.Item> items = itemService.SearchItems(is1).ToList();

                XDocument xdoc = new XDocument(new XElement("items"));

                foreach (var item in items)
                {
                    xdoc.Element("items").Add(new XElement("item", new XAttribute("itemid", item.Id),
                        new XAttribute("creationdate", item.Created),
                        new XAttribute("topic", item.Title),
                        new XAttribute("typeid", "4"),
                        new XAttribute("enrollmentid", item.CreationBy),
                        new XAttribute("entityid", item.EntityId)));
                }

                // Insertng values into database
                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader());  
                if (reportData.Tables.Count > 0)
                {
                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_ContentSubmission";
                    sbc.ColumnMappings.Add("itemid", "ContentSubmissionID");
                    sbc.ColumnMappings.Add("creationdate", "ContentSubmissionDate");
                    sbc.ColumnMappings.Add("topic", "Topic");
                    sbc.ColumnMappings.Add("entityid", "CourseID");
                    sbc.ColumnMappings.Add("enrollmentid", "EnrollmentID");
                    sbc.ColumnMappings.Add("typeid", "ContentTypeID");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void GetQuizSubmissionList(string entityId, string query, ISessionManager manager)
        {
            try
            {
                IItemService itemService = new ItemService(manager);
                ItemSearch is1 = new ItemSearch();
                is1.EntityId = entityId;
                is1.Query = query;
                List<Bfw.Agilix.DataContracts.Item> items = itemService.SearchItems(is1).ToList();

                XDocument xdoc = new XDocument(new XElement("items"));

                foreach (var item in items)
                {
                    xdoc.Element("items").Add(new XElement("item", new XAttribute("itemid", item.Id),
                        new XAttribute("creationdate", item.Created),
                        new XAttribute("topic", item.Title),
                        new XAttribute("typeid", "1"),
                        new XAttribute("enrollmentid", item.CreationBy),
                        new XAttribute("entityid", item.EntityId)));
                }

                // Insertng values into database

                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader());  
                if (reportData.Tables.Count > 0)
                {
                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_ContentSubmission";
                    sbc.ColumnMappings.Add("itemid", "ContentSubmissionID");
                    sbc.ColumnMappings.Add("creationdate", "ContentSubmissionDate");
                    sbc.ColumnMappings.Add("topic", "Topic");
                    sbc.ColumnMappings.Add("enrollmentid", "EnrollmentID");
                    sbc.ColumnMappings.Add("entityid", "CourseID");
                    sbc.ColumnMappings.Add("typeid", "ContentTypeID");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void GetHomeworkList(string entityId, string query, ISessionManager manager)
        {
            try
            {
                IItemService itemService = new ItemService(manager);
                ItemSearch is1 = new ItemSearch();
                is1.EntityId = entityId;
                is1.Query = query;
                List<Bfw.Agilix.DataContracts.Item> items = itemService.SearchItems(is1).ToList();

                XDocument xdoc = new XDocument(new XElement("items"));

                foreach (var item in items)
                {
                    xdoc.Element("items").Add(new XElement("item", new XAttribute("itemid", item.Id),
                        new XAttribute("creationdate", item.Created),
                        new XAttribute("topic", item.Title),
                        new XAttribute("typeid", "2"),
                        new XAttribute("enrollmentid", item.CreationBy),
                        new XAttribute("entityid", item.EntityId)));
                }
                // Insertng values into database

                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader()); 
                if (reportData.Tables.Count > 0)
                {

                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_ContentSubmission";
                    sbc.ColumnMappings.Add("itemid", "ContentSubmissionID");
                    sbc.ColumnMappings.Add("creationdate", "ContentSubmissionDate");
                    sbc.ColumnMappings.Add("topic", "Topic");
                    sbc.ColumnMappings.Add("entityid", "CourseID");
                    sbc.ColumnMappings.Add("enrollmentid", "EnrollmentID");
                    sbc.ColumnMappings.Add("typeid", "ContentTypeID");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void GetDiscussionList(string entityId, string query, ISessionManager manager)
        {
            try
            {
                IItemService itemService = new ItemService(manager);
                ItemSearch is1 = new ItemSearch();
                is1.EntityId = entityId;
                is1.Query = query;
                List<Bfw.Agilix.DataContracts.Item> items = itemService.SearchItems(is1).ToList();

                XDocument xdoc = new XDocument(new XElement("items"));

                foreach (var item in items)
                {
                    xdoc.Element("items").Add(new XElement("item", new XAttribute("itemid", item.Id),
                        new XAttribute("creationdate", item.Created),
                        new XAttribute("topic", item.Title),
                        new XAttribute("typeid", "7"),
                        new XAttribute("enrollmentid", item.CreationBy),
                        new XAttribute("entityid", item.EntityId)));
                }

                // Insertng values into database

                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader());  
                if (reportData.Tables.Count > 0)
                {

                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_ContentSubmission";
                    sbc.ColumnMappings.Add("itemid", "ContentSubmissionID");
                    sbc.ColumnMappings.Add("creationdate", "ContentSubmissionDate");
                    sbc.ColumnMappings.Add("topic", "Topic");
                    sbc.ColumnMappings.Add("entityid", "CourseID");
                    sbc.ColumnMappings.Add("enrollmentid", "EnrollmentID");
                    sbc.ColumnMappings.Add("typeid", "ContentTypeID");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


        private static void GetCourseList(string domainId, ISessionManager manager)
        {
            try
            {
                ICourseService courseService = new CourseService(manager);
                CourseSearch cs1 = new CourseSearch();
                cs1.DomainId = domainId;
                List<Bfw.Agilix.DataContracts.Course> courses = courseService.GetCourses(cs1).ToList();

                XDocument xdoc = new XDocument(new XElement("courses"));

                foreach (var course in courses)
                {
                    xdoc.Element("courses").Add(new XElement("course",
                        new XAttribute("courseid", course.Id),
                        new XAttribute("coursename", course.Title),
                        new XAttribute("creationdate", course.CreationDate )));

                }
                // Insertng values into database table rpt_cousemaster;
                DataSet reportData = new DataSet();
                reportData.ReadXml(xdoc.CreateReader());  
                if (reportData.Tables.Count > 0)
                {

                    SqlConnection connection = new SqlConnection(strCon);
                    SqlBulkCopy sbc = new SqlBulkCopy(connection);
                    sbc.DestinationTableName = "rpt_CourseMaster";
                    sbc.ColumnMappings.Add("courseid", "CourseID");
                    sbc.ColumnMappings.Add("coursename", "CourseName");
                    sbc.ColumnMappings.Add("creationdate", "CreationDate");
                    connection.Open();
                    sbc.WriteToServer(reportData.Tables[0]);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ContentSessonSubmission(ISessionManager manager)
        {
            try
            {
                string sEnrollmentID;
                string sItemID;

                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand command = new SqlCommand("select en.EnrollmentID , cs.ContentSubmissionID from rpt_ContentSubmission cs , rpt_Enrollment en where cs.EnrollmentID = en.StudentID ", connection);
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                connection.Open();
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                // Process each result
                int i;
                for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    sEnrollmentID = ds.Tables[0].Rows[i]["EnrollmentID"].ToString();
                    sItemID = ds.Tables[0].Rows[i]["ContentSubmissionID"].ToString();
                    GetGradeList("5222", sItemID, manager);
                }
                // Close the connection
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void FileSubmission(ISessionManager manager)
        {
            try
            {
                // to get the item
                //delete records from the tables used for FileSubmission
                DeleteFileSubmissionTables(); 
                // to get the course

                GetCourseList(sDomainID, manager);

                string tmpCourseID;

                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand command = new SqlCommand() ;
                if (sCourseID != string.Empty)
                {
                    command = new SqlCommand("select * from rpt_CourseMaster where CourseID in (" + sCourseID + ") ", connection);
                }
                else
                    if (sStartDate != string.Empty)
                    {
                        command = new SqlCommand("select * from rpt_CourseMaster where CONVERT(date,creationdate,111) >='" + sStartDate + "' and CONVERT(date,creationdate,111) <='" + sEndDate + "'", connection);
                    }
                    else
                    {
                        command = new SqlCommand("select * from rpt_CourseMaster ", connection);
                    }

                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                connection.Open();
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                // Process each result
                int i;
                for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    tmpCourseID = ds.Tables[0].Rows[i]["CourseID"].ToString();
                    GetAssignmentList(tmpCourseID, "/type='Assignment'", manager);
                    GetEntityEnrollmentList(tmpCourseID, null, null, manager);
                }
                // Close the connection
                connection.Close();

                string sItemID;
                string sEnrollmentID;
                string sAssignmentName;
                string sCourse;
                SqlConnection connection1 = new SqlConnection(strCon);
                SqlCommand command1 = new SqlCommand("Select enrollmentid, assignmentid, AssignmentName, am.CourseID  from rpt_Enrollment en, rpt_AssignmentMaster am where en.CourseID = am.CourseID", connection1);
                SqlDataAdapter adapter1 = new SqlDataAdapter();
                DataSet ds1 = new DataSet();
                connection1.Open();
                adapter1.SelectCommand = command1;
                adapter1.Fill(ds1);
                // Process each result
                int j;
                for (j = 0; j <= ds1.Tables[0].Rows.Count - 1; j++)
                {
                    sEnrollmentID = ds1.Tables[0].Rows[j]["enrollmentid"].ToString();
                    sItemID = ds1.Tables[0].Rows[j]["assignmentid"].ToString();
                    sAssignmentName = ds1.Tables[0].Rows[j]["assignmentname"].ToString();
                    sCourse = ds1.Tables[0].Rows[j]["courseid"].ToString();
                    GetStudentSubmissionHistory(sEnrollmentID, sItemID, manager);
                    //SqlCommand cmd = new SqlCommand();
                    //string strCmd;
                    //sAssignmentName = sAssignmentName.Replace("'", "''");
                    //strCmd = "Update rpt_FileSubmission set FileName = '" + sAssignmentName + "', courseid=" + sCourse + " where assignmentid='" + sItemID + "' and enrollmentid='" + sEnrollmentID + "'";
                    //    SqlCommand updatecmd = new SqlCommand(strCmd, connection1);
                    //    updatecmd.ExecuteNonQuery();

                    //using parameters
                    string updateSql ="Update rpt_FileSubmission" + " set FileName = @FileName, courseid=@CourseID" + " Where assignmentid=@AssignmentID and enrollmentid=@EnrollmentID";
                    //string updateSql1 = "UPDATE Employees " + "SET LastName = @LastName " + "WHERE FirstName = @FirstName"; 
                    SqlCommand UpdateCmd = new SqlCommand(updateSql, connection1);         
                    // Map Parameters  
                    UpdateCmd.Parameters.Add("@FileName", SqlDbType.NVarChar, 500, "FileName"); 
                    UpdateCmd.Parameters.Add("@CourseID", SqlDbType.Int,10, "CourseID"); 
                    UpdateCmd.Parameters.Add("@AssignmentID", SqlDbType.NVarChar, 50, "assignmentid"); 
                    UpdateCmd.Parameters.Add("@EnrollmentID", SqlDbType.Int , 10, "enrollmentid"); 
                    UpdateCmd.Parameters["@FileName"].Value = sAssignmentName ;
                    UpdateCmd.Parameters["@CourseID"].Value = sCourse ;
                    UpdateCmd.Parameters["@AssignmentID"].Value = sItemID ;
                    UpdateCmd.Parameters["@EnrollmentID"].Value = sEnrollmentID ;
                    UpdateCmd.ExecuteNonQuery(); 
                    //

                }
                connection1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void QuizSubmission(ISessionManager manager)
        {
            try
            {
                DeleteQuizSubmissionTables();
                string tmpCourseID;

                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand command = new SqlCommand();
                if (sCourseID != string.Empty)
                {
                    command = new SqlCommand("select * from rpt_CourseMaster where CourseID in (" + sCourseID + ") ", connection);
                }
                else
                    if (sStartDate != string.Empty)
                    {
                        command = new SqlCommand("select * from rpt_CourseMaster where CONVERT(date,creationdate,111) >='" + sStartDate + "' and CONVERT(date,creationdate,111) <='" + sEndDate + "'", connection);
                    }
                    else
                    {
                        command = new SqlCommand("select * from rpt_CourseMaster ", connection);
                    }
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                connection.Open();
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                // Process each result
                int i;
                for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    tmpCourseID = ds.Tables[0].Rows[i]["CourseID"].ToString();
                    //GetAssessmentList("6090", "/type='Assessment'", manager);
                    GetAssessmentList(tmpCourseID, "/type='Assessment'", manager);
                    QuizSubmissionbyStudent(tmpCourseID, manager);
                }
                // Close the connection
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void QuizSubmissionbyStudent(string sCourseID, ISessionManager manager)
        {
            try
            {
                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand command = new SqlCommand("select QuizID,EnrollmentID from rpt_QuizMaster qm, rpt_Enrollment en where qm.CourseID = en.CourseID and qm.CourseID=" + sCourseID + "", connection);
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                connection.Open();
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                // Process each result
                int i;
                string sEnrollmentID;
                string sQuizID;
                for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    sEnrollmentID = ds.Tables[0].Rows[i]["EnrollmentID"].ToString();
                    sQuizID = ds.Tables[0].Rows[i]["QuizID"].ToString();
                    GetStudentSubmission(sEnrollmentID, sQuizID, "file", "meta.xml", manager);
                }
                // Close the connection
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ContentSubmission(ISessionManager manager)
        {
            try
            {
                DeleteContentSubmissionTables();
                string tmpCourseID;

                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand command = new SqlCommand();
                if (sCourseID != string.Empty)
                {
                    command = new SqlCommand("select * from rpt_CourseMaster where CourseID in (" + sCourseID + ") ", connection);
                }
                else
                    if (sStartDate != string.Empty)
                    {
                        command = new SqlCommand("select * from rpt_CourseMaster where CONVERT(date,creationdate,111) >='" + sStartDate + "' and CONVERT(date,creationdate,111) <='" + sEndDate + "'", connection);
                    }
                    else
                    {
                        command = new SqlCommand("select * from rpt_CourseMaster ", connection);
                    }
                //SqlCommand command = new SqlCommand("select * from rpt_CourseMaster order by CourseID", connection);
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                connection.Open();
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                // Process each result
                int i;
                for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    tmpCourseID = ds.Tables[0].Rows[i]["CourseID"].ToString();
                    GetWikiList(tmpCourseID, "/type='Wiki'", manager);
                    GetDiscussionList(tmpCourseID, "/type='Discussion'", manager);
                    GetHomeworkList(tmpCourseID, "/type='Homework'", manager);
                    GetQuizSubmissionList(tmpCourseID, "/type='Assessment'", manager);
                }
                SqlCommand deletetmp = new SqlCommand("Delete from rpt_ContentSubmission where LEFT(contentsubmissionid,3)='TMP'", connection);
                deletetmp.ExecuteNonQuery();
                // Close the connection
                connection.Close();
                // call content session submission 
                ContentSessonSubmission(manager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static void GetEnrollmentActivityList(string enrollmentid, ISessionManager manager)
        {
            IEnrollmentActivity enrollmentService = new EnrollmentActivityService(manager);

            EnrollmentActivitySearch es1 = new EnrollmentActivitySearch();
            es1.EnrollmentId = enrollmentid;
            XDocument enrollmentactivity = enrollmentService.GetEnrollmentActivity(es1);

        }


        private static void DeleteFileSubmissionTables()
        {
            try
            {
                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand deletetable = new SqlCommand(" Delete from rpt_CourseMaster; Delete from rpt_AssignmentMaster ; Delete from rpt_StudentMaster ; Delete from rpt_Enrollment ; Delete from rpt_FileSubmission ", connection);
                connection.Open();
                deletetable.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void DeleteContentSubmissionTables()
        {
            try
            {
                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand deletetable = new SqlCommand("Delete from rpt_ContentSubmission where ContentTypeID <>1; Delete from rpt_ContentSession; ", connection);
                connection.Open();
                deletetable.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void DeleteQuizSubmissionTables()
        {
            try
            {
                SqlConnection connection = new SqlConnection(strCon);
                SqlCommand deletetable = new SqlCommand("Delete from rpt_QuizMaster; Delete from rpt_QuestionMaster;  Delete from rpt_AnswerDetail; Delete from rpt_QuizSubmission; Delete from rpt_ContentSubmission where ContentTypeID =1;", connection);
                connection.Open();
                deletetable.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }

    public class CommandOptions
    {
        public string LastsignalId { get; set; }

        /// <summary>
        /// Id of the course the folder exists in
        /// </summary>

        public PropertyType Type { get; set; }

        public string Action { get; set; }

    }
}
