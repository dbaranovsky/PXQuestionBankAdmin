using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Macmillan.PXQBA.Web.ViewModels.User;
using Macmillan.PXQBA.Web.ViewModels.Versions;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Represents service that converts objects and their properties while mapping
    /// </summary>
    public interface IModelProfileService
    {
        /// <summary>
        /// Parses default section of the question
        /// </summary>
        /// <param name="question">Agilix question</param>
        /// <returns>Default section</returns>
        QuestionMetadataSection GetQuestionDefaultValues(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Parses list of product course sections of the question
        /// </summary>
        /// <param name="question">Agilix question</param>
        /// <returns>List of product course sections</returns>
        List<QuestionMetadataSection> GetProductCourseSections(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Gets product course sections from view model that came from the UI
        /// </summary>
        /// <param name="viewModel">Question view model</param>
        /// <returns>List of product course sections</returns>
        List<QuestionMetadataSection> GetProductCourseSections(QuestionViewModel viewModel);

        /// <summary>
        /// Gets question metadata from question object
        /// </summary>
        /// <param name="question">Question</param>
        /// <param name="course">Course question belongs to</param>
        /// <returns>Question metadata</returns>
        QuestionMetadata GetQuestionMetadataForCourse(Question question, Course course = null);

        /// <summary>
        /// Converts question metadata into xml elements
        /// </summary>
        /// <param name="question">Question object</param>
        /// <returns>Dictionary of metadata elements</returns>
        Dictionary<string, XElement> GetXmlMetadataElements(Question question);

        /// <summary>
        /// Gets chapters view model list from course
        /// </summary>
        /// <param name="course">Course</param>
        /// <returns>List of chapters</returns>
        IEnumerable<ChapterViewModel> GetChaptersViewModel(Course course);

        /// <summary>
        /// Gets product course names by product course ids
        /// </summary>
        /// <param name="titleIds">Product course id list</param>
        /// <returns>Product courses names list</returns>
        IEnumerable<string> GetTitleNames(IEnumerable<string> titleIds);

        /// <summary>
        /// Loads information about source question that current question was shared from
        /// </summary>
        /// <param name="questionIdDuplicateFrom">Id of the question shared from</param>
        /// <param name="course">Current course</param>
        /// <returns>Shared Question from</returns>
        SharedQuestionDuplicateFromViewModel GetSourceQuestionSharedFrom(string questionIdDuplicateFrom, Course course);

        /// <summary>
        /// Gets default section from view model
        /// </summary>
        /// <param name="question">Question</param>
        /// <returns>Default metadata section</returns>
        QuestionMetadataSection GetDefaultSectionForViewModel(Question question);

        /// <summary>
        /// Gets the name of the question modifier
        /// </summary>
        /// <param name="modifiedByUserId">Modifier user id</param>
        /// <returns>Modifier name</returns>
        string GetModifierName(string modifiedByUserId);

        /// <summary>
        /// Loads question that current question is duplicated from
        /// </summary>
        /// <param name="repositoryCourseId">Repository course id</param>
        /// <param name="duplicateFrom">Question id duplicate from</param>
        /// <returns>Question duplicate from</returns>
        Question GetDuplicateFromQuestion(string repositoryCourseId, string duplicateFrom);
        
        /// <summary>
        /// Gets id of the shared question that current question was duplicated from
        /// </summary>
        /// <param name="question">Current question</param>
        /// <returns>Question id</returns>
        string GetDuplicateFromShared(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Gets id of the question that current question was duplicated from
        /// </summary>
        /// <param name="question">Current question</param>
        /// <returns>Question id</returns>
        string GetDuplicateFrom(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Gets question id current question was created as draft from
        /// </summary>
        /// <param name="question">Current question which is draft</param>
        /// <returns>Question id</returns>
        string GetDraftFrom(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Gets version number current version was restored from
        /// </summary>
        /// <param name="question">Version of the question</param>
        /// <returns>Version number</returns>
        string GetRestoredFromVersion(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Indicates if particular version of the question was published from draft
        /// </summary>
        /// <param name="question">Question version</param>
        /// <returns>If is published from draft</returns>
        bool GetPublishedFromDraft(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Gets modifier name
        /// </summary>
        /// <param name="question">Question</param>
        /// <returns>modifier name</returns>
        string GetModifiedBy(Bfw.Agilix.DataContracts.Question question);

        /// <summary>
        /// Gets numeric version from string
        /// </summary>
        /// <param name="questionVersion">String version</param>
        /// <returns>Numeric version</returns>
        int GetNumericVersion(string questionVersion);

        /// <summary>
        /// Gets course banks separated by new line
        /// </summary>
        /// <param name="course">Course</param>
        /// <returns>Banks</returns>
        string GetCourseBanks(Course course);

        /// <summary>
        /// Gets course chapters separated by new line
        /// </summary>
        /// <param name="course">Course</param>
        /// <returns>Chapters</returns>
        string GetCourseChapters(Course course);

        /// <summary>
        /// Gets available values for course field
        /// </summary>
        /// <param name="field">Course field</param>
        /// <returns>Values</returns>
        IEnumerable<AvailableChoiceItem> GetMetadataFieldValues(CourseMetadataFieldDescriptor field);

        /// <summary>
        /// Gets list of course fields configured for questions
        /// </summary>
        /// <param name="metadataConfigViewModel">Course configuration</param>
        /// <returns>List of field descriptors</returns>
        IEnumerable<CourseMetadataFieldDescriptor> GetCourseFieldDescriptors(MetadataConfigViewModel metadataConfigViewModel);

        /// <summary>
        /// Gets the type from string
        /// </summary>
        /// <param name="type">Strign type</param>
        /// <returns>Type</returns>
        MetadataFieldType GetMetadataFieldType(string type);

        /// <summary>
        /// Converts field type to string
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>String type</returns>
        string MetadataFieldTypeToString(MetadataFieldType type);

        /// <summary>
        /// Loads a particular version of the question
        /// </summary>
        /// <param name="entityId">Repository course id</param>
        /// <param name="id">Question id</param>
        /// <param name="version">Version to load</param>
        /// <returns>Question version</returns>
        Question GetQuestionVersion(string entityId, string id, string version);

        /// <summary>
        /// Loads checked capabilities for role
        /// </summary>
        /// <param name="src">Role</param>
        /// <returns>List of checked capabilities</returns>
        IEnumerable<Capability> GetActiveRoleCapabilities(RoleViewModel src);

        /// <summary>
        /// Logically groups capabilities by features they belong to
        /// </summary>
        /// <param name="capabilities">Capabilities list</param>
        /// <returns>Grouped capabilities</returns>
        IEnumerable<CapabilityGroupViewModel> GetCapabilityGroups(IList<Capability> capabilities);

        /// <summary>
        /// Maps item links xml elements to field descriptors
        /// </summary>
        /// <param name="questionCardData">List of item links elements</param>
        /// <param name="courseData">Course data</param>
        /// <returns>Field descriptors for item links</returns>
        IEnumerable<CourseMetadataFieldDescriptor> MapFieldsWithItemLinks(List<QuestionCardData> questionCardData, XElement courseData);

        /// <summary>
        /// Converts parsed question to question object
        /// </summary>
        /// <param name="parsedQuestion">Parsed question</param>
        /// <param name="course">Course question belongs to</param>
        /// <returns>Question</returns>
        Question GetQuestionFromParsedQuestion(ParsedQuestion parsedQuestion, Course course);

        /// <summary>
        /// Converts parsed resource
        /// </summary>
        /// <param name="parsedResource">Parsed resource</param>
        /// <param name="courseId">Course id</param>
        /// <returns>Resource</returns>
        Resource GetResourceFromParsedResource(ParsedResource parsedResource, string courseId);
    }
}