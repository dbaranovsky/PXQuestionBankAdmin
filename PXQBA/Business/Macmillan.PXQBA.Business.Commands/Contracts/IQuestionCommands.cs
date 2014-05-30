﻿using System.Collections.Generic;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IQuestionCommands
    {
        PagedCollection<Question> GetQuestionList(string questionRepositoryCourseid, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);
        Question CreateQuestion(string productCourseId, Question question);
        Question GetQuestion(string repositoryCourseId, string questionId);

        Dictionary<string, int> GetQuestionCountByChapters(string questionRepositoryCourseId, string currentCourseId);
        Question UpdateQuestion(Question question);
        Question UpdateQuestionInTempQuiz(Question question);
        bool UpdateQuestionField(string productCourseId, string repositoryCourseId, string questionId, string fieldName, string value);

        bool BulklUpdateQuestionField(string productCourseId, string repositoryCourseId, string[] questionId, string fieldName, string value);
        bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, IEnumerable<string> fieldValues);

        string GetQuizIdForQuestion(string questionId, string entityId);

        bool RemoveFromTitle(string[] questionsId, string questionRepositoryCourseId, string currentCourseId);

        IEnumerable<Question> GetQuestions(string repositoryCourseId, string[] questionsId);

        bool UpdateQuestions(IEnumerable<Question> questions, string repositoryCourseId);

        void ExecuteSolrUpdateTask();
    }
}
