using System;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Services
{
    public class QbaApiStub : IQBA_API_interface
    {
        public List<Question> GetQuestions(string titleId)
        {
            throw new NotImplementedException();
        }

        public string GetQuestionPreview(string questionId)
        {
            throw new NotImplementedException();
        }

        public List<Question> GetOrderedQuestionList(string titleId, string orderBy)
        {
            throw new NotImplementedException();
        }

        public void SaveQuestion(Question question)
        {
            throw new NotImplementedException();
        }

        public bool IsQuestionLive(string questionId)
        {
            throw new NotImplementedException();
        }

        public bool IsMetadataFieldEditable(string questionId, string fieldName)
        {
            throw new NotImplementedException();
        }

        public void CopyQuestion(string questionId)
        {
            throw new NotImplementedException();
        }

        public void GetChapterList(string titleId)
        {
            throw new NotImplementedException();
        }

        public void GetBankList(string titleId)
        {
            throw new NotImplementedException();
        }

        public Question CreateQuestion()
        {
            throw new NotImplementedException();
        }

        public string GetQuestionStatus(string questionId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetQuestionStatusList()
        {
            throw new NotImplementedException();
        }

        public void GetQuestionNotes(string questionId)
        {
            throw new NotImplementedException();
        }

        public void SaveNote(string questionId)
        {
            throw new NotImplementedException();
        }

        public bool IsNoteFlagged(string questionId, string noteId)
        {
            throw new NotImplementedException();
        }

        public bool SetFlag(string noteId, bool flag)
        {
            throw new NotImplementedException();
        }

        public void DeleteNote(string questionId, string noteId)
        {
            throw new NotImplementedException();
        }

        public bool IsQuestionFlagged(string questionId)
        {
            throw new NotImplementedException();
        }

        public Question GetQuestionMetadata(string questionId)
        {
            throw new NotImplementedException();
        }

        public void QuestionBulkUpdate(List<Question> questions)
        {
            throw new NotImplementedException();
        }

        public List<string> GetFilterableMetadataFields(string titleId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetPossibleMetadataFieldValues(string titleId, string fieldName)
        {
            throw new NotImplementedException();
        }

        public List<Question> GetQuestions(Dictionary<string, string> filter)
        {
            throw new NotImplementedException();
        }

        public void GetTitleList(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
