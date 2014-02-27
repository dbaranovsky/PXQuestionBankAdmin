using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bdc = Bfw.PX.Biz.DataContracts;
using Bfw.Common.Database;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.Services.Mappers;
using BizQuestion = Bfw.PX.Biz.DataContracts.Question;
using System.Text;
namespace Bfw.PX.Biz.Direct.Services
{
    public class QuestionAdminActions : IQuestionAdminActions
    {
        /// <summary>
        /// The IBusinessContext implementation to use.
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// The ICourseActions.
        /// </summary>
        protected ICourseActions CourseActions { get; set; }

        /// <summary>
        /// The IEnrollmentActions.
        /// </summary>
        protected IEnrollmentActions EnrollmentActions { get; set; }
        
        /// <summary>
        /// The IUserActions.
        /// </summary>
        protected IUserActions UserActions { get; set; }

        /// <summary>
        /// The item search service.
        /// </summary>
        protected IItemQueryActions ItemQueryActions { get; set; }
        /// <summary>
        /// The IContentActions
        /// </summary>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// The IQuestionActions
        /// </summary>
        protected IQuestionActions QuestionActions { get; set; }

        public QuestionAdminActions(IBusinessContext context, ISessionManager sessionManager, ICourseActions courseActions, IContentActions contentActions, IUserActions userActions, IEnrollmentActions enrollmentActions, IQuestionActions questionActions, IItemQueryActions itemQueryActions)
		{
			Context = context;
            SessionManager = sessionManager;
            CourseActions = courseActions;
            ContentActions = contentActions;
            UserActions = userActions;
            EnrollmentActions = enrollmentActions;
            QuestionActions = questionActions;
            ItemQueryActions = itemQueryActions;
		}

    	/// <summary>
    	/// Adds a note to the system.
    	/// </summary>
    	/// <param name="questionNote"> </param>
    	/// <returns></returns>
    	public void AddQuestionNote(QuestionNote questionNote)
        {
            var db = new DatabaseManager("PXData");
            using (Context.Tracer.DoTrace("QuestionAdminActions.AddQuestionNote()"))
            {
                try
                {
                    db.StartSession();
                    db.ExecuteNonQuery("AddQuestionNote @0, @1, @2, @3, @4, @5, @6", questionNote.QuestionId, questionNote.CourseId, questionNote.UserId, questionNote.FirstName, questionNote.LastName, questionNote.Text, questionNote.AttachPath);                    
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

    	/// <summary>
    	/// Gets question notes for specified question.
    	/// </summary>
    	/// <param name="questionId"> </param>
    	/// <returns></returns>
    	public IEnumerable<QuestionNote> GetQuestionNotes(string questionId)
        {
            using (Context.Tracer.DoTrace("QuestionAdminActions.GetQuestionNotes(questionId={0})", questionId))
            {
                var QuestionNotes = new List<Bdc.QuestionNote>();
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    var records = db.Query("GetQuestionNotes @0", questionId);

                    if (!records.IsNullOrEmpty())
                    {
                        QuestionNotes = records.Map(r => new Bdc.QuestionNote()
                        {
                            Id = r.String("Id"),
                            CourseId = r.String("CourseId"),
                            QuestionId = r.String("QuestionId"),
                            Text = r.String("Text"),
                            AttachPath = r.String("AttachPath"),
                            UserId = r.String("UserId"),
                            FirstName = r.String("FirstName"),
                            LastName = r.String("LastName"),
                            Created = r.DateTime("Created")
                        }).ToList();
                    }
                }
                finally
                {
                    db.EndSession();
                }

                return QuestionNotes;
            }
        }


    	/// <summary>
    	/// Adds a question log to the system.
    	/// </summary>
    	/// <param name="questionLog"> </param>
    	/// <returns></returns>
    	public void AddQuestionLog(QuestionLog questionLog)
        {
            var db = new DatabaseManager("PXData");
            using (Context.Tracer.DoTrace("QuestionAdminActions.AddQuestionLog()"))
            {
                try
                {
                    db.StartSession();
                    db.ExecuteNonQuery("AddQuestionLog @0, @1, @2, @3, @4, @5, @6, @7", questionLog.QuestionId, questionLog.CourseId, questionLog.UserId, questionLog.FirstName,
                        questionLog.LastName, questionLog.EventType,questionLog.Version,questionLog.ChangesMadeXML);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        /// <summary>
        /// Gets question logs for specified question.
        /// </summary>
		/// <param name="questionId">questionId</param>
        /// <returns></returns>
        public IEnumerable<QuestionLog> GetQuestionLogs(string questionId)
        {
            using (Context.Tracer.DoTrace("QuestionAdminActions.GetQuestionLogs(questionId={0})", questionId))
            {
                var QuestionLogs = new List<Bdc.QuestionLog>();
                var db = new DatabaseManager("PXData");
                try
                {
                    db.StartSession();
                    var records = db.Query("GetQuestionLogs @0", questionId);

                    if (!records.IsNullOrEmpty())
                    {
                        QuestionLogs = records.Map(r => new Bdc.QuestionLog()
                        {
                            Id = r.String("Id"),
                            CourseId = r.String("CourseId"),
                            QuestionId = r.String("QuestionId"),
                            EventType = (QuestionLogEventType)int.Parse(r.String("EventType")),
                            UserId = r.String("UserId"),
                            FirstName = r.String("FirstName"),
                            LastName = r.String("LastName"),
                            Created = r.DateTime("Created"),
                            Version = string.IsNullOrEmpty(r.String("version")) == true ? "" : r.String("version"),
                            ChangesMadeXML = r.String("Changes")
                        }).ToList();
                    }
                }
                finally
                {
                    db.EndSession();
                }
                
                return QuestionLogs;  
            }
              
        }

        /// <summary>
        /// get all the history versions of a given question
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="questionId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public List<BizQuestion> GetQuestionAllVersions(string entityId,string questionId,bool version)
        {
            using (Context.Tracer.DoTrace("QuestionAdminActions.GetQuestionLogs(questionId={0})", questionId))
            {
                var QuestionHistory = QuestionActions.GetQuestionsList(entityId, questionId, version);
                return QuestionHistory.OrderByDescending(i => i.ModifiedDate.Ticks).ToList();
            }
        }
        /// <summary>
        /// return xml that contains the Diff in Question versions
        /// </summary>
        /// <returns></returns>
        public string FindChangesInQuestions(BizQuestion _old, BizQuestion _new)
        {
            StringBuilder xml = new StringBuilder();
            var _changes = "<changes>";
            var _changes_Close = "</changes>";

            if (_old == null || _new == null) return "";
            else
            {
                xml.Append(_changes);

                xml.Append(CreateStringTag("Title", _old.Title, _new.Title));
                xml.Append(CreateStringTag("Excercise No", _old.ExcerciseNo, _new.ExcerciseNo));
                xml.Append(CreateStringTag("Question Bank", _old.QuestionBankText, _new.QuestionBankText));
                xml.Append(CreateStringTag("eBook Chapter", _old.eBookChapter, _new.eBookChapter));
                xml.Append(CreateStringTag("Difficulty", _old.Difficulty, _new.Difficulty));
                xml.Append(CreateStringTag("Cognitive Level", _old.CongnitiveLevel, _new.CongnitiveLevel));
                xml.Append(CreateStringTag("Blooms Domain", _old.BloomsDomain, _new.BloomsDomain));
                xml.Append(CreateStringTag("Guidance", _old.Guidance, _new.Guidance));
                xml.Append(CreateStringTag("Free Response Question", _old.FreeResponseQuestion, _new.FreeResponseQuestion));
                xml.Append(CreateStringTag("Assigned Chapter", _old.AssignedChapter, _new.AssignedChapter));
                xml.Append(CreateStringTag("General FeedBack", _old.GeneralFeedback, _new.GeneralFeedback));
                xml.Append(CreateStringTag("Points", _old.Points.ToString(), _new.Points.ToString()));
                xml.Append(CreateStringTag("Body", _old.Body, _new.Body));
                xml.Append(CreateStringTag("Interaction Data", _old.InteractionData, _new.InteractionData));
               xml.Append(CreateStringTag("Question Status", _old.QuestionStatus, _new.QuestionStatus,true));  //to ignore Field with zero send true
                //***********************************************************************
                //                  comparing collections                              //
                //***********************************************************************
                if ((_old.Choices != null && _new.Choices != null) && (_old.Choices.Count() != _new.Choices.Count()))
                        xml.Append(CreateStringTag("Choices", _old.Choices.Count().ToString(), _new.Choices.Count().ToString()));

                if ((_old.SelectedSuggestedUse != null && _new.SelectedSuggestedUse != null) && (_old.SelectedSuggestedUse.Count() != _new.SelectedSuggestedUse.Count()))
                {
                    var oldlist = string.Join(",", _old.SelectedSuggestedUse.Select(m => m.Value).ToArray());
                    oldlist = string.IsNullOrEmpty(oldlist) == true ? "" : oldlist;
                    var newlist = string.Join(",", _new.SelectedSuggestedUse.Select(m => m.Value).ToArray());
                    newlist = string.IsNullOrEmpty(newlist) == true ? "" : newlist;
                    xml.Append(CreateStringTag("Suggested Use", oldlist,newlist));
                }

                if ((_old.SelectedLearningObjectives != null && _new.SelectedLearningObjectives != null) && (_old.SelectedLearningObjectives.Count() != _new.SelectedLearningObjectives.Count()))
                {
                    var oldlist = string.Join("||", _old.SelectedLearningObjectives.Select(m=>m.Value).ToArray());
                    oldlist = string.IsNullOrEmpty(oldlist) == true ? "" : oldlist;
                    var newlist = string.Join("||", _new.SelectedLearningObjectives.Select(m => m.Value).ToArray());
                    newlist = string.IsNullOrEmpty(newlist) == true ? "" : newlist;
                    xml.Append(CreateStringTag("Learning Objectives", oldlist,newlist));
                }

                xml.Append(_changes_Close);
            }
            return xml.ToString();
        }


        private string CreateStringTag(string fieldName,string _old,string _newOne,bool ignoreField=false)
        {
            var oldstring = string.IsNullOrEmpty(_old) == true ? "" : _old.Trim();
            var newstring = string.IsNullOrEmpty(_newOne) == true ? "" : _newOne.Trim();
            
            if (newstring.Trim() == "0" && ignoreField==false)
                newstring = string.Empty;

            if (oldstring != newstring)
                return "<change><field>" + fieldName + "</field><orig>" + oldstring + "</orig><new>" + newstring + "</new></change>";
            else return "";
        }

    	/// <summary>
    	/// GetSelectedQuizzes
    	/// </summary>
    	/// <param name="entityId"> </param>
    	/// <param name="quizIdsList"> </param>
    	/// <returns></returns>
        public List<BizQuestion> GetQuestionsForSelectedQuizzes(string entityId, List<ContentItem> quizIdsList)
		{
		    if (quizIdsList==null | entityId==null) return null;

			List<BizQuestion> questionsToReturn = null;

			using (Context.Tracer.DoTrace("QuestionAdminActions.GetQuestionsForSelectedQuizzes()"))
			{

				foreach (var Q in quizIdsList)
				{
                    var quizId = Q.Id;
					var questionsResultSet = QuestionActions.GetQuestions(entityId, quizId, null, null);

					var questions = questionsResultSet.Questions.Distinct();

					foreach (var question in questions)
					{
                        question.AssignedQuizes = new List<ContentItem> { new ContentItem() { Id = quizId, Type = "Assessment", CourseId = Context.CourseId} };
                        question.AssignedChapter = Q.ParentId;
					}

					if (questionsToReturn == null) questionsToReturn = new List<BizQuestion>();
					questionsToReturn.AddRange(questions);
				}				
			}

			return questionsToReturn;
		}

    	/// <summary>
    	/// Get QuestionIDs List from the list of Quizzes
    	/// </summary>
    	/// <param name="quizList"> </param>
    	/// <returns></returns>
    	public List<string> GetQuestionIDsList(List<ContentItem> quizList)
		{
			var questionIdsList = quizList.Select(q => q.QuizQuestions );

			return (List<string>) questionIdsList;
		}




    	/// <summary>
		/// GetQuestionsForSelectedChapters
    	/// </summary>
    	/// <param name="selectedStatus"> </param>
    	/// <param name="selectedChapters"></param>
    	/// <param name="keyword"> </param>
    	/// <param name="selectedFormats"> </param>
    	/// <param name="selectedQuizzes"> </param>
    	/// <returns></returns>
    	public List<ContentItem> GetSearchResultQuestions(string keyword, string selectedFormats, string selectedStatus, string selectedChapters, string selectedQuizzes)
		{
			//cmd=getquestionlist&entityid=65131&questionid=34720FC1F33BC77A50772BB37DD00014|34720FC1F33BC77A50772BB37DD00022&query=/interaction@type='choice'&count=25

    		selectedFormats = selectedFormats.Replace("FormatSelectedValues=", "/interaction@type='").Replace("&", "' OR ");
			selectedChapters = selectedChapters.Replace("ChapterSelectedValues=", "/parent='").Replace("&", "' OR ");
			selectedQuizzes = selectedQuizzes.Replace("QuizSelectedValues=", "/parent='").Replace("&", "' OR ");
			selectedStatus = selectedStatus.Replace("StatusSelectedValues=", "/parent='").Replace("&", "' OR ");

			List<Bdc.ContentItem> itemsToReturn = null;

			using (Context.Tracer.DoTrace("QuestionAdminActions.GetCourseQuizzesForSelectedChapters()"))
			{

			    
				if (!String.IsNullOrEmpty(selectedQuizzes))
				{
					

				}


				ItemSearch itemsSearchQuery = new ItemSearch()
				{
					EntityId = Context.CourseId,
					Query = "/bfw_subtype='QUIZ' AND (" + selectedChapters + "')"
				};

				var items = ContentActions.FindContentItems(itemsSearchQuery);

				if (items != null) itemsToReturn = items.OrderBy(itm => itm.Title).Distinct().ToList();
			}

			return itemsToReturn;
		}

        /// <summary>
        /// Enrolls current RA user to the Product course
        /// </summary>
        /// <param name="instructorPermissionFlags">Rights flag for Instructor</param>
        /// <returns>Flag</returns>
        public bool CreateAdminUserEnrollment(string instructorPermissionFlags)
        {            
            String domainId;            
            bool result = false;

            Bdc.Course course = CourseActions.GetCourseByCourseId(Context.CourseId);
            domainId = course.Domain.Id;

            Bdc.UserInfo userInfo;

            if (string.IsNullOrEmpty(Context.CurrentUser.Id) && !string.IsNullOrEmpty(Context.CurrentUser.Username) && !string.IsNullOrEmpty(Context.CurrentUser.Email))
            {
                IEnumerable<Bdc.Domain> userDomains = Context.GetRaUserDomains();
                if ((userDomains.Where(d => d.Id == domainId)).Count() == 0)
                {
                    var user = Context.GetNewUserData();
                    user.Password = ConfigurationManager.AppSettings["BrainhoneyDefaultPassword"];
                    user.PasswordQuestion = "more";
                    user.PasswordAnswer = "please";
                    //add the user to the domain
                    userInfo = UserActions.CreateUser(user.Username, user.Password, user.PasswordQuestion, user.PasswordAnswer,
                        user.FirstName, user.LastName, user.Email, domainId, course.Domain.Name, user.ReferenceId);

                }
                else
                {
                    var cmd = new GetUsers() { SearchParameters = new Bfw.Agilix.DataContracts.UserSearch() { ExternalId = Context.CurrentUser.Username, DomainId = domainId } };
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                    userInfo = cmd.Users.Map(u => u.ToUserInfo()).ToList().First();                    
                }

                if (userInfo != null)
                {
                    Context.CurrentUser = userInfo;
                }                
            }

            if (!string.IsNullOrEmpty(Context.CurrentUser.Id) && !string.IsNullOrEmpty(Context.CourseId))
            {
                Bdc.Enrollment userEnrollment = EnrollmentActions.GetEnrollment(Context.CurrentUser.Id, course.Id);
                if (userEnrollment != null && !string.IsNullOrEmpty(userEnrollment.Id))
                {
                    Context.CurrentUser.EnrollmentIdForCurrentCourse = userEnrollment.Id;
                    Context.EnrollmentId = userEnrollment.Id;
                    result = true;
                }
                else
                {
                    //enroll part
                    List<Bdc.Enrollment> enroll = new List<Bdc.Enrollment>();
                    enroll = EnrollmentActions.CreateEnrollments(course.Domain.Id, Context.CurrentUser.Id, course.Id, instructorPermissionFlags, "1", DateTime.Now, DateTime.Now.AddYears(1), string.Empty, string.Empty);

                    if (enroll.Count() > 0)
                    {
                        Context.CurrentUser.EnrollmentIdForCurrentCourse = enroll.First().Id;
                        Context.EnrollmentId = enroll.First().Id;
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Enrolls Anonymous User to the Discipline course       
        /// </summary>
        /// <param name="instructorPermissionFlags">Rights flag for Instructor</param>
        /// <returns>Enrollment Id</returns>
        public string GetAnonymousUserEnrollmentId(string instructorPermissionFlags, string DisciplineCourseId)
        {
            string enrollmentId = string.Empty;
            String domainId;
            Bdc.Course course = CourseActions.GetCourseByCourseId(DisciplineCourseId);
            domainId = course.Domain.Id;
            //Bdc.UserInfo anonymousUserInfo;
            //string anonymousUserId = string.Empty;

            //if (!string.IsNullOrEmpty(Context.CourseId) && !string.IsNullOrEmpty(domainId))
            //{
            //    var cmd = new GetUsers() { SearchParameters = new Bfw.Agilix.DataContracts.UserSearch() { Username = "anonymous", DomainId = domainId } };
            //    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            //    anonymousUserInfo = cmd.Users.Map(u => u.ToUserInfo()).ToList().First();
            //    if (anonymousUserInfo != null)
            //        anonymousUserId = anonymousUserInfo.Id;
            //}
            //if (!string.IsNullOrEmpty(anonymousUserId) && !string.IsNullOrEmpty(DisciplineCourseId))
            //{
            //    Bdc.Enrollment userEnrollment = EnrollmentActions.GetEnrollment(anonymousUserId, DisciplineCourseId);
            //    if (userEnrollment != null && !string.IsNullOrEmpty(userEnrollment.Id))
            //        enrollmentId = userEnrollment.Id;
            //    else
            //    {
            //        //enroll part
            //        List<Bdc.Enrollment> enroll = new List<Bdc.Enrollment>();
            //        enroll = EnrollmentActions.CreateEnrollments(domainId, anonymousUserId, DisciplineCourseId, instructorPermissionFlags, "1", DateTime.Now, DateTime.Now.AddYears(3), string.Empty, string.Empty);

            //        if (enroll.Count() > 0)
            //            enrollmentId = enroll.First().Id;
            //    }
            //}
            var currentUser = Context.CurrentUser.Id;
            if (!string.IsNullOrEmpty(currentUser) && !string.IsNullOrEmpty(DisciplineCourseId))
            {
                Bdc.Enrollment userEnrollment = EnrollmentActions.GetEnrollment(currentUser, DisciplineCourseId);
                if (userEnrollment != null && !string.IsNullOrEmpty(userEnrollment.Id))
                    enrollmentId = userEnrollment.Id;
                else
                {
                    //enroll part
                    List<Bdc.Enrollment> enroll = new List<Bdc.Enrollment>();
                    enroll = EnrollmentActions.CreateEnrollments(domainId, currentUser, DisciplineCourseId, instructorPermissionFlags, "1", DateTime.Now, DateTime.Now.AddYears(3), string.Empty, string.Empty);

                    if (enroll.Count() > 0)
                        enrollmentId = enroll.First().Id;
                }
            }
            return enrollmentId;
        }
    }
}
