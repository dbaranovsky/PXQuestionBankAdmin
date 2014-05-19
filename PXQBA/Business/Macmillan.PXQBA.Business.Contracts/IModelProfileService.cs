using System.Collections.Generic;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.DataAccess.Data;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IModelProfileService
    {
        InteractionType CreateInteractionType(string questionType);

        string SetLearningObjectives(IEnumerable<LearningObjective> learningObjectives);
        IEnumerable<LearningObjective> GetLOByGuid(string productCourseId, string learningObjectiveGuids);
        string GetQuestionCardLayout(Bfw.Agilix.DataContracts.Course src);
        string GetHardCodedQuestionDuplicate();
        string GetQuizIdForQuestion(string id, string entityId);
        Question GetHardCodedSourceQuestion(int sharedFrom);

        IEnumerable<CourseMetadataFieldDescriptor> GetCourseMetadataFieldDescriptors(
            Bfw.Agilix.DataContracts.Course src);

        string GetQuestionBankRepositoryCourse(Bfw.Agilix.DataContracts.Course src);
        Dictionary<string, IEnumerable<string>> GetQuestionDefaultValues(Bfw.Agilix.DataContracts.Question question);
        IEnumerable<ProductCourseSection> GetProductCourseSections(Bfw.Agilix.DataContracts.Question question);
        QuestionMetadata GetQuestionMetadataForCourse(Question question, Course course = null);
        Dictionary<string, XElement> GetXmlMetadataElements(Question question);
    }
}