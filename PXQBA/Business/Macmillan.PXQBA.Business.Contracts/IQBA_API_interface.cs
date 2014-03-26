
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Interface only lists the methods that need to be moved to corresponding services, the signature should be changed in appropriate way
    /// </summary>
    public interface IQBA_API_interface
    {
        ///IMPORTANT: all the methods should be moved to corresponding services and signature should be changed in appropriate way

        List<Question> GetQuestions(string titleId);

        string GetQuestionPreview(string questionId);

        List<Question> GetOrderedQuestionList(string titleId, string orderBy);

        void SaveQuestion(Question question);

        bool IsQuestionLive(string questionId); // Maybe can be checked via metadata field

        bool IsMetadataFieldEditable(string questionId, string fieldName); // Maybe can be checked via field attribute

        void CopyQuestion(string questionId);

        void GetChapterList(string titleId);

        void GetBankList(string titleId);

        Question CreateQuestion();

        string GetQuestionStatus(string questionId); // Maybe can be checked via metadata field

        List<string> GetQuestionStatusList();

        void GetQuestionNotes(string questionId);

        void SaveNote(string questionId);

        bool IsNoteFlagged(string questionId, string noteId); // Maybe can be checked via metadata field

        bool SetFlag(string noteId, bool flag); //Maybe can be done in metadata field

        void DeleteNote(string questionId, string noteId);

        bool IsQuestionFlagged(string questionId);

        Question GetQuestionMetadata(string questionId);

        void QuestionBulkUpdate(List<Question> questions);

        List<string> GetFilterableMetadataFields(string titleId);

        List<string> GetPossibleMetadataFieldValues(string titleId, string fieldName);

        List<Question> GetQuestions(Dictionary<string, string> filter);

        void GetTitleList(string userId);

    }
}