using System.Collections.Generic;
using System.Xml.Linq;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// XML elements, attributes, etc used by the item schema.
    /// </summary>
    public static class ElStrings
    {
        /// <summary>
        /// Element that contains the parent item id.
        /// </summary>
        public static readonly XName parent = "parent";

        /// <summary>
        /// Element that contains the item type.
        /// </summary>
        public static readonly XName type = "type";


        /// <summary>
        /// Element that contains the item type.
        /// </summary>
        public static readonly XName flags = "flags";

        /// <summary>
        /// BFW custom element that contains the item's BFW specific type.
        /// </summary>
        public static readonly XName bfw_type = "bfw_type";

        /// <summary>
        /// Element that contains the item's title.
        /// </summary>
        public static readonly XName title = "title";

        /// <summary>
        /// Element that contains the item's subtitle.
        /// </summary>
        public static readonly XName subtitle = "subtitle";

        /// <summary>
        /// Element that contains the item's description.
        /// </summary>
        public static readonly XName description = "description";

        /// <summary>
        /// Element that contains the value of the gradable flag.
        /// </summary>
        public static readonly XName gradable = "gradable";

        /// <summary>
        /// Element that contains the value of the Allow Late Submission flag
        /// </summary>
        public static readonly XName allowLateSubmission = "allowlatesubmission";

        /// <summary>
        /// Element that contains the URL to the item's rubric.
        /// </summary>
        public static readonly XName rubric = "rubric";

        /// <summary>
        /// Element that contains the id of the folder item.
        /// </summary>
        public static readonly XName folder = "folder";

        /// <summary>
        /// Element that contains the URL to the item's resource.
        /// </summary>
        public static readonly XName href = "href";

        /// <summary>
        /// Element that contains the value of the item's toc visibility flag.
        /// </summary>
        public static readonly XName hiddenfromtoc = "hiddenfromtoc";

        /// <summary>
        /// Element that contains the value of the item's student visibility flag.
        /// </summary>
        public static readonly XName hiddenfromstudent = "hiddenfromstudent";

        /// <summary>
        /// Element that contains the value of the item's availability visibility date flag.
        /// </summary>
        public static readonly XName availabledate = "availabledate";

        /// <summary>
        /// Element that contains the item's due date.
        /// </summary>
        public static readonly XName duedate = "duedate";

        /// <summary>
        /// group ids for which item has been modified
        /// </summary>
        public static readonly XName AdjustedGroups = "AdjustedGroups";

        /// <summary>
        /// Element that contains the item's due date grace period.
        /// </summary>
        public static readonly XName duedategrace = "duedategrace";

        /// <summary>
        /// Element that contains the item is assigned or not.
        /// </summary>
        public static readonly XName meta_bfw_assigned = "meta-bfw_assigned";

        /// <summary>
        /// Attribute that contains a search shortcut
        /// </summary>
        public static readonly XName searchterm = "searchterm";

        /// <summary>
        /// Element that contains learning curve information inside questions
        /// </summary>
        public static readonly XName meta_lc_question_settings = "meta-lc-question-settings";

        /// <summary>
        /// Element that contains the primary questions information.
        /// </summary>
        public static readonly XName meta_lc_question_setting = "meta-lc-question-setting";

        public static readonly XName never_scramble_answers = "never_scramble_answers";

        public static readonly XName primary_question = "meta-primary-question";

        public static readonly XName difficulty_level = "difficulty_level";


        /// <summary>
        /// Element that contains the container names
        /// </summary>
        public static readonly XName containers = "meta-containers";
        public static readonly XName container = "meta-container";
        public static readonly XName toc = "toc";
        public static readonly XName metasubcontainerids = "meta-subcontainers";
        public static readonly XName metasubcontainerid = "meta-subcontainerid";
	    public static readonly XName dlaptype = "dlaptype";
        /// <summary>
        /// Element that contains the subcontainer id
        /// </summary>
        public static readonly XName subcontainerid = "meta-subcontainerid";
        /// <summary>
        /// Element that contains the item's maximum number of points.
        /// </summary>
        public static readonly XName maxpoints = "maxpoints";

        /// <summary>
        /// Element that contains the item's threshold.
        /// </summary>
        public static readonly XName threshold = "threshold";

        /// <summary>
        /// The weight of this item within its grading category.
        /// </summary>
        public static readonly XName weight = "weight";

        /// <summary>
        /// The number of lowest scores to drop from this category.
        /// </summary>
        public static readonly XName droplowest = "droplowest";

        /// <summary>
        /// Element that contains the item's dropbox settings.
        /// </summary>
        public static readonly XName dropbox = "dropbox";

        /// <summary>
        /// Element that contains the item's category.
        /// </summary>
        public static readonly XName category = "category";

        /// <summary>
        /// Element that contains the item's sco.
        /// </summary>
        public static readonly XName sco = "sco";

        /// <summary>
        /// Element that contains the item's sequence.
        /// </summary>
        public static readonly XName sequence = "sequence";

        /// <summary>
        /// Element that contains any item attachements.
        /// </summary>
        public static readonly XName attachments = "attachments";

        /// <summary>
        /// Element that contains the item's thumbnail.
        /// </summary>
        public static readonly XName thumbnail = "thumbnail";

        /// <summary>
        /// Element that contains the value of the course association flag.
        /// </summary>
        public static readonly XName associatedtocourse = "associatedtocourse";

        /// <summary>
        /// Element that contains the list of associated TOC items.
        /// </summary>
        public static readonly XName associatedTOCItems = "associatedTOCItems";

        /// <summary>
        /// Element that contains the custom exam type of this item.
        /// </summary>
        public static readonly XName customexamtype = "customexamtype";

        /// <summary>
        /// Element holding information about usage of the item on the proxy page
        /// </summary>
        public static readonly XName proxyConfig = "proxyConfig";

        /// <summary>
        /// The flag indicating usage of commenting 
        /// </summary>
        public static readonly XName allowComments = "allowComments";

        /// <summary>
        /// Element that contains the questions inside this item.
        /// </summary>
        public static readonly XName questions = "questions";

        // These aren't used here, but do need to be ignored when copying non-px settings from item to item

        /// <summary>
        /// Root element of the custom properties collection.
        /// </summary>
        public static readonly XName bfw_properties = "bfw_properties";

        /// <summary>
        /// Root element of the custom TOC collection.
        /// </summary>
        public static readonly XName bfw_tocs = "bfw_tocs";

        /// <summary>
        /// Element that represent which course type can edit that item
        /// </summary>
        public static readonly XName bfw_locked = "bfw_locked";

        /// <summary>
        /// Root element of the custom learning objectives collection.
        /// </summary>
        public static readonly XName learningObjectives = "learningobjectives";

        /// <summary>
        /// 
        /// </summary>
        public static readonly XName bfw_related_contents = "bfw_related_contents";

        /// <summary>
        /// Root element of the custom related templates collection.
        /// </summary>
        public static readonly XName relatedTemplates = "bfw_related_templates";

        /// <summary>
        /// itemid
        /// </summary>
        public static readonly XName ItemId = "itemid";

        /// <summary>
        /// Data
        /// </summary>
        public static readonly XName Data = "data";

        /// <summary>
        /// CreationBy
        /// </summary>
        public static readonly XName CreationBy = "creationby";

        /// <summary>
        /// Old Status
        /// </summary>
        public static readonly XName OldStatus = "oldstatus";

        /// <summary>
        /// New Status
        /// </summary>
        public static readonly XName NewStatus = "newstatus";

        /// <summary>
        /// IsMarkAsCompleteChecked
        /// </summary>
        public static readonly XName IsMarkAsCompleteChecked = "IsMarkAsCompleteChecked";

        /// <summary>
        /// DropBoxType
        /// </summary>
        public static readonly XName DropBoxType = "dropboxtype";

        /// <summary>
        /// Meta_Container
        /// </summary>
        public static readonly XName Meta_Container = "meta-container";

        /// <summary>
        /// Meta_SubcontainerId
        /// </summary>
        public static readonly XName Meta_SubcontainerId = "meta-subcontainerid";

        /// <summary>
        /// Guid
        /// </summary>
        public static readonly XName Guid = "guid";

        /// <summary>
        /// Bfw_Property
        /// </summary>
        public static readonly XName Bfw_Property = "bfw_property";

        /// <summary>
        /// Bfw_IncludeGbbScoreTrigger
        /// </summary>
        public static readonly XName Bfw_IncludeGbbScoreTrigger = "bfw_IncludeGbbScoreTrigger";

        /// <summary>
        /// GradeFlags
        /// </summary>
        public static readonly XName GradeFlags = "gradeflags";

        /// <summary>
        /// Passign Score
        /// </summary>
        public static readonly XName PassingScore = "masterythreshold";

        /// <summary>
        /// Setting
        /// </summary>
        public static readonly XName Setting = "setting";

        /// <summary>
        /// Question
        /// </summary>
        public static readonly XName Question = "Question";

        /// <summary>
        /// Entityid
        /// </summary>
        public static readonly XName Entityid = "entityid";

        /// <summary>
        /// Condition
        /// </summary>
        public static readonly XName Condition = "condition";

        /// <summary>
        /// Answer
        /// </summary>
        public static readonly XName Answer = "Answer";

        /// <summary>
        /// CorrectQuestion
        /// </summary>
        public static readonly XName CorrectQuestion = "CorrectQuestion";

        /// <summary>
        /// Rule
        /// </summary>
        public static readonly XName Rule = "rule";

        /// <summary>
        /// CorrectChoice
        /// </summary>
        public static readonly XName CorrectChoice = "CorrectChoice";

        /// <summary>
        /// Feedback
        /// </summary>
        public static readonly XName Feedback = "Feedback";

        /// <summary>
        /// Feedback_GROUP
        /// </summary>
        public static readonly XName Feedback_GROUP = "Feedback-GROUP";

        /// <summary>
        /// Achieved
        /// </summary>
        public static readonly XName Achieved = "achieved";

        /// <summary>
        /// Possible
        /// </summary>
        public static readonly XName Possible = "Possible";

        /// <summary>
        /// possible
        /// </summary>
        public static readonly XName possible = "possible";

        /// <summary>
        /// Letter
        /// </summary>
        public static readonly XName Letter = "letter";

        /// <summary>
        /// RawAchieved
        /// </summary>
        public static readonly XName RawAchieved = "rawachieved";

        /// <summary>
        /// RawPossible
        /// </summary>
        public static readonly XName RawPossible = "rawpossible";

        /// <summary>
        /// Attempts
        /// </summary>
        public static readonly XName Attempts = "attempts";

        /// <summary>
        /// ScoredDate
        /// </summary>
        public static readonly XName ScoredDate = "scoreddate";

        /// <summary>
        /// ScoredVersion
        /// </summary>
        public static readonly XName ScoredVersion = "scoredversion";

        /// <summary>
        /// SubmittedVersion
        /// </summary>
        public static readonly XName SubmittedVersion = "submittedversion";

        /// <summary>
        /// CategoryId
        /// </summary>
        public static readonly XName CategoryId = "categoryid";

        /// <summary>
        /// Status
        /// </summary>
        public static readonly XName Status = "status";

        /// <summary>
        /// SubmittedDate
        /// </summary>
        public static readonly XName SubmittedDate = "submitteddate";

        /// <summary>
        /// Categories
        /// </summary>
        public static readonly XName Categories = "categories";

        /// <summary>
        /// WeightedCategories
        /// </summary>
        public static readonly XName WeightedCategories = "weightedcategories";

        /// <summary>
        /// CategoryWeightTotal
        /// </summary>
        public static readonly XName CategoryWeightTotal = "categoryweighttotal";

        /// <summary>
        /// Total
        /// </summary>
        public static readonly XName Total = "total";

        /// <summary>
        /// TotalWithExtraCredit
        /// </summary>
        public static readonly XName TotalWithExtraCredit = "totalwithextracredit";

        /// <summary>
        /// Name
        /// </summary>
        public static readonly XName Name = "name";

        /// <summary>
        /// Id
        /// </summary>
        public static readonly XName Id = "id";

        /// <summary>
        /// GradeList
        /// </summary>
        public static readonly XName GradeList = "GradeList";

        /// <summary>
        /// ResponseVersion
        /// </summary>
        public static readonly XName ResponseVersion = "responseversion";

        /// <summary>
        /// Seconds
        /// </summary>
        public static readonly XName Seconds = "seconds";

        /// <summary>
        /// GroupId
        /// </summary>
        public static readonly XName GroupId = "groupid";

        /// <summary>
        /// OwnerId
        /// </summary>
        public static readonly XName OwnerId = "ownerid";

        /// <summary>
        /// Reference
        /// </summary>
        public static readonly XName Reference = "reference";

        /// <summary>
        /// DomainId
        /// </summary>
        public static readonly XName DomainId = "domainid";

        /// <summary>
        /// SetId
        /// </summary>
        public static readonly XName SetId = "setid";

        /// <summary>
        /// Group
        /// </summary>
        public static readonly XName Group = "group";

        /// <summary>
        /// Member
        /// </summary>
        public static readonly XName Member = "member";

        /// <summary>
        /// EnrollmentId
        /// </summary>
        public static readonly XName EnrollmentId = "enrollmentid";

        /// <summary>
        /// Message
        /// </summary>
        public static readonly XName Message = "message";

        /// <summary>
        /// Notes
        /// </summary>
        public static readonly XName Notes = "notes";
        /// <summary>
        /// Bfw_description
        /// </summary>
        public static readonly XName Bfw_description = "bfw_description";

        /// <summary>
        /// Version
        /// </summary>
        public static readonly XName Version = "version";

        /// <summary>
        /// Created
        /// </summary>
        public static readonly XName Created = "created";

        /// <summary>
        /// MessageId
        /// </summary>
        public static readonly XName MessageId = "messageid";

        /// <summary>
        /// ParentId
        /// </summary>
        public static readonly XName ParentId = "parentid";

        /// <summary>
        /// resourceentityid
        /// </summary>
        public static readonly XName ResourceEntityId = "resourceentityid";

        /// <summary>
        /// QuestionId
        /// </summary>
        public static readonly XName QuestionId = "questionid";

        /// <summary>
        /// Body
        /// </summary>
        public static readonly XName Body = "body";

        /// <summary>
        /// Interaction
        /// </summary>
        public static readonly XName Interaction = "interaction";
        
        /// <summary>
        /// width
        /// </summary>
        public static readonly XName InteractionWidth = "width";

        /// <summary>
        /// height
        /// </summary>
        public static readonly XName InteractionHeight = "height";

        /// <summary>
        /// significantfigures
        /// </summary>
        public static readonly XName InteractionSignificantFigures = "significantfigures";

        /// <summary>
        /// Question Text Type
        /// </summary>
        public static readonly XName TextType = "texttype";

        /// <summary>
        /// Value
        /// </summary>
        public static readonly XName Value = "value";

        /// <summary>
        /// CustomURL
        /// </summary>
        public static readonly XName CustomURL = "customurl";

        /// <summary>
        /// Choice
        /// </summary>
        public static readonly XName Choice = "choice";

        /// <summary>
        /// feedback
        /// </summary>
        public static readonly XName feedback = "feedback";

        /// <summary>
        /// Score
        /// </summary>
        public static readonly XName Score = "score";

        /// <summary>
        /// Groups
        /// </summary>
        public static readonly XName Groups = "groups";

        /// <summary>
        /// Meta
        /// </summary>
        public static readonly XName Meta = "meta";

        /// <summary>
        /// Meta
        /// </summary>
        public static readonly XName Partial = "partial";

        /// <summary>
        /// question
        /// </summary>
        public static readonly XName question = "question";

        /// <summary>
        /// Schema
        /// </summary>
        public static readonly XName Schema = "schema";

        /// <summary>
        /// answer
        /// </summary>
        public static readonly XName answer = "answer";

        /// <summary>
        /// Number
        /// </summary>
        public static readonly XName Number = "number";

        /// <summary>
        /// Enrollments
        /// </summary>
        public static readonly XName Enrollments = "enrollments";

        /// <summary>
        /// Enrollment
        /// </summary>
        public static readonly XName Enrollment = "enrollment";

        /// <summary>
        /// Correlation
        /// </summary>
        public static readonly XName Correlation = "correlation";

        /// <summary>
        /// Correct
        /// </summary>
        public static readonly XName Correct = "correct";

        /// <summary>
        /// Count
        /// </summary>
        public static readonly XName Count = "count";

        /// <summary>
        /// Path
        /// </summary>
        public static readonly XName Path = "path";

        /// <summary>
        /// CreationDate
        /// </summary>
        public static readonly XName CreationDate = "creationdate";

        /// <summary>
        /// ModifiedDate
        /// </summary>
        public static readonly XName ModifiedDate = "modifieddate";

        /// <summary>
        /// Resource
        /// </summary>
        public static readonly XName Resource = "resource";

        /// <summary>
        /// SignalId
        /// </summary>
        public static readonly XName SignalId = "signalid";

        /// <summary>
        /// Signal
        /// </summary>
        public static readonly XName Signal = "Signal";

        /// <summary>
        /// Submission
        /// </summary>
        public static readonly XName Submission = "submission";

        /// <summary>
        /// Submission Actions
        /// </summary>
        public static readonly XName Actions = "actions";

        /// <summary>
        /// Submission Actions Date
        /// </summary>
        public static readonly XName Date = "date";

        /// <summary>
        /// Submission Actions Location Attribute
        /// </summary>
        public static readonly XName Location = "location";


        /// <summary>
        /// Submission Action
        /// </summary>
        public static readonly XName Action = "action";

        /// <summary>
        /// AttemptQuestion
        /// </summary>
        public static readonly XName AttemptQuestion = "attemptquestion";

        /// <summary>
        /// PartId
        /// </summary>
        public static readonly XName PartId = "partid";

        /// <summary>
        /// Continue
        /// </summary>
        public static readonly XName Continue = "continue";

        /// <summary>
        /// LastSave
        /// </summary>
        public static readonly XName LastSave = "lastsave";

        /// <summary>
        /// Mask
        /// </summary>
        public static readonly XName Mask = "mask";

        /// <summary>
        /// PointsAssigned
        /// </summary>
        public static readonly XName PointsAssigned = "pointsassigned";

        /// <summary>
        /// PointsComputed
        /// </summary>
        public static readonly XName PointsComputed = "pointscomputed";

        /// <summary>
        /// PointsPossible
        /// </summary>
        public static readonly XName PointsPossible = "pointspossible";

        /// <summary>
        /// PointsAchieved
        /// </summary>
        public static readonly XName PointsAchieved = "pointsachieved";

        /// <summary>
        /// ForeignId
        /// </summary>
        public static readonly XName ForeignId = "foreignid";

        /// <summary>
        /// Response
        /// </summary>
        public static readonly XName Response = "response";

        /// <summary>
        /// TeacherResponse
        /// </summary>
        public static readonly XName TeacherResponse = "teacherresponse";

        /// <summary>
        /// Is Depreciated
        /// </summary>
        public static readonly XName IsDepreciated = "isdepreciated";
        /// <summary>
        /// Request
        /// </summary>
        public static readonly XName Request = "request";

        /// <summary>
        /// Cmd
        /// </summary>
        public static readonly XName Cmd = "cmd";

        /// <summary>
        /// GetUser
        /// </summary>
        public static readonly XName GetUser = "getuser";

        /// <summary>
        /// userid
        /// </summary>
        public static readonly XName UserId = "userid";

        /// <summary>
        /// Element that contains the rubric rules in item analysis report.
        /// </summary>
        public static readonly XName RubricRule = "rubricrule";

        /// <summary>
        /// GetUserList
        /// </summary>
        public static readonly XName GetUserList = "getuserlist";

        public static readonly XName Grades = "grades";
        public static readonly XName Max = "max";

        /// <summary>
        /// UserName
        /// </summary>
        public static readonly XName UserName = "username";
        public static readonly XName AttemptLimit = "attemptlimit";
        public static readonly XName GradeRule = "graderule";
        public static readonly XName SubmissionGradeAction = "submissiongradeaction";
        public static readonly XName CompletionTrigger = "completiontrigger";
        public static readonly XName TimeToComplete = "timetocomplete";
        public static readonly XName TimeLimit = "timelimit";
        public static readonly XName ExamFlags = "examflags";
        public static readonly XName QuestionsPerPage = "questionsperpage";
        public static readonly XName AllowViewHints = "allowviewhints";
        public static readonly XName PercentSubstractHint = "percentsubstracthint";
        public static readonly XName Percent = "percent";
        public static readonly XName ItemWeightTotal = "itemweighttotal";
        public static readonly XName ExamReviewRules = "examreviewrules";
        public static readonly XName GradeReleaseDate = "gradereleasedate";
        public static readonly XName AllowStudentEmailInstructors = "allowstudentsemailinstructors";
        public static readonly XName Item = "item";
        public static readonly XName Attachment = "attachment";
        public static readonly XName TocItem = "tocItem";
        public static readonly XName Objective = "objective";
        public static readonly XName Template = "template";
        public static readonly XName CategorySequence = "categorysequence";
        public static readonly XName AssignmentFolderId = "meta-xbook-assignment-id";

        public static readonly XName QuestionVersion = "version";
        public static readonly XName ExcerciseNo = "exercisenumber";
        public static readonly XName QuestionBank_UsedIn = "usedin";
        public static readonly XName eBookChapter = "ebooksection";
        public static readonly XName Difficulty = "difficulty";
        public static readonly XName CognitiveLevel = "cognitivelevel";
        public static readonly XName BloomsDomain = "bloomdomain";
        public static readonly XName Guidance = "guidance";
        public static readonly XName FreeResponseQuestion = "freeresponsequestion";

        public static readonly XName QuestionStatus = "questionstatus";
        public static readonly XName Defaults = "product-course-defaults";
        public static readonly XName ProductCourseSection = "product-course-id";


        //public static readonly XName group = "group";
        //public static readonly XName group = "group";
        /// <summary>
        /// Whitelist of elements that will be persisted whenever settings are copied.
        /// </summary>
        public static readonly IEnumerable<string> SettingsWhiteList = new List<string>()
            {
                "parent",
                "type",
                "bfw_type",
                "title",
                "description",
                "gradable",
                "rubric",
                "folder",
                "href",
                "hiddenfromtoc",
                "duedate",
                "maxpoints",
                "dropbox",
                "category",
                "sequence",
                "attachments",
                "thumbnail",
                "associatedtocourse",
                "associatedTOCItems",
                "customexamtype",
                "questions",
                "bfw_properties",
                "bfw_tocs",
                "meta-bfw_assigned",
                "bfw_related_templates",
                "bfw_related_contents"                
            };

       
    }
}
