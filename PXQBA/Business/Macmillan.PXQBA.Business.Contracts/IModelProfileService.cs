using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.DataAccess.Data;
using Course = Bfw.Agilix.DataContracts.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IModelProfileService
    {
        InteractionType CreateInteractionType(string questionType);

        Dictionary<string, string> CreateQuestionMetadata(Question question);
        string SetLearningObjectives(IEnumerable<LearningObjective> learningObjectives);
        IEnumerable<LearningObjective> GetLOByGuid(string productCourseId, string learningObjectiveGuids);
        string GetQuestionCardLayout(Bfw.Agilix.DataContracts.Course src);
        IEnumerable<ProductCourseSection> GetHardCodedSharedProductCourses(ProductCourse productCourse);
        string GetHardCodedQuestionDuplicate();
        string GetQuizIdForQuestion(string id, string entityId);
        Question GetHardCodedSourceQuestion(int sharedFrom);

        IEnumerable<CourseMetadataFieldDescriptor> GetCourseMetadataFieldDescriptors(
            Bfw.Agilix.DataContracts.Course src);
    }
}