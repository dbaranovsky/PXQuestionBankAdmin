using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.DataAccess.Data;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Macmillan.PXQBA.Web.ViewModels.User;
using Macmillan.PXQBA.Web.ViewModels.Versions;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IModelProfileService
    {
        QuestionMetadataSection GetQuestionDefaultValues(Bfw.Agilix.DataContracts.Question question);
        List<QuestionMetadataSection> GetProductCourseSections(Bfw.Agilix.DataContracts.Question question);
        List<QuestionMetadataSection> GetProductCourseSections(QuestionViewModel viewModel);
        QuestionMetadata GetQuestionMetadataForCourse(Question question, Course course = null);
        Dictionary<string, XElement> GetXmlMetadataElements(Question question);

        IEnumerable<ChapterViewModel> GetChaptersViewModel(Course course);
        IEnumerable<string> GetTitleNames(IEnumerable<string> titleIds);
        SharedQuestionDuplicateFromViewModel GetSourceQuestionSharedFrom(string questionIdDuplicateFrom, Course course);
        QuestionMetadataSection GetDefaultSectionForViewModel(Question question);
        string GetModifierName(string modifiedByUserId);
        Question GetDuplicateFromQuestion(string repositoryCourseId, string duplicateFrom);
        string GetDuplicateFromShared(Bfw.Agilix.DataContracts.Question question);
        string GetDuplicateFrom(Bfw.Agilix.DataContracts.Question question);
        string GetDraftFrom(Bfw.Agilix.DataContracts.Question question);
        string GetRestoredFromVersion(Bfw.Agilix.DataContracts.Question question);
        bool GetPublishedFromDraft(Bfw.Agilix.DataContracts.Question question);
        string GetModifiedBy(Bfw.Agilix.DataContracts.Question question);
        int GetNumericVersion(string questionVersion);
        string GetCourseBanks(Course course);
        string GetCourseChapters(Course course);
        IEnumerable<AvailableChoiceItem> GetMetadataFieldValues(CourseMetadataFieldDescriptor field);
        IEnumerable<CourseMetadataFieldDescriptor> GetCourseFieldDescriptors(MetadataConfigViewModel metadataConfigViewModel);
        MetadataFieldType GetMetadataFieldType(string type);
        string MetadataFieldTypeToString(MetadataFieldType type);
        IEnumerable<CourseMetaFieldValue> GetFieldValues(IEnumerable<AvailableChoiceItem> valuesOptions);
        Question GetQuestionVersion(string entityId, string id, string version);
        IEnumerable<Capability> GetActiveRoleCapabilities(RoleViewModel src);
        IEnumerable<CapabilityGroupViewModel> GetCapabilityGroups(IList<Capability> capabilities);

        IEnumerable<CourseMetadataFieldDescriptor> MapFieldsWithItemLinks(List<QuestionCardData> questionCardData, XElement courseData);
        Question GetQuestionFromParsedQuestion(ParsedQuestion parsedQuestion, Course course);
        Resource GetResourceFromRestoredResource(ParsedResource parsedResource, string courseId);
    }
}