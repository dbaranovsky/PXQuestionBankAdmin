using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IModelProfileService
    {
        InteractionType CreateInteractionType(string questionType);

        Dictionary<string, string> CreateQuestionMetadata(Question question);
        string SetLearningObjectives(IEnumerable<LearningObjective> learningObjectives);
        IEnumerable<LearningObjective> GetLOByGuid(string productCourseId, string learningObjectiveGuids);
        string GetQuestionCardLayout(Bfw.Agilix.DataContracts.Course src);
        IEnumerable<Chapter> GetHardCodedQuestionChapters();
        string GetHardCodedSharedFrom(int questionId);
        IEnumerable<string> GetHardCodedSharedTo(int questionId);
        string GetHardCodedQuestionDuplicate();
        string GetQuizIdForQuestion(string id, string entityId);
    }
}