using System.Collections.Generic;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.DataAccess.Data;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IModelProfileService
    {
        string SetLearningObjectives(IEnumerable<LearningObjective> learningObjectives);
        IEnumerable<LearningObjective> GetLOByGuid(string productCourseId, string learningObjectiveGuids);
        string GetQuestionCardLayout(Bfw.Agilix.DataContracts.Course src);

        List<CourseMetadataFieldDescriptor> GetCourseMetadataFieldDescriptors(
            Bfw.Agilix.DataContracts.Course src);

        string GetQuestionBankRepositoryCourse(Bfw.Agilix.DataContracts.Course src);
        QuestionMetadataSection GetQuestionDefaultValues(Bfw.Agilix.DataContracts.Question question);
        List<QuestionMetadataSection> GetProductCourseSections(Bfw.Agilix.DataContracts.Question question);
        List<QuestionMetadataSection> GetProductCourseSections(QuestionViewModel viewModel);
        QuestionMetadata GetQuestionMetadataForCourse(Question question, Course course = null);
        Dictionary<string, XElement> GetXmlMetadataElements(Question question);

        IEnumerable<ChapterViewModel> GetChaptersViewModel(Course course);
        IEnumerable<string> GetTitleNames(IEnumerable<string> titleIds);
        SharedQuestionDuplicateFromViewModel GetSourceQuestionSharedWith(QuestionMetadataSection section, Course course);
        QuestionMetadataSection GetDefaultSectionForViewModel(Question question);
    }
}