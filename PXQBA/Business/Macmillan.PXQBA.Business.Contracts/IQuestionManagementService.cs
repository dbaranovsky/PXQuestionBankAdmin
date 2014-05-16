﻿using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Service to handle operations with questions
    /// </summary>
    public interface IQuestionManagementService
    {
        /// <summary>
        /// Retrieves questions list
        /// </summary>
        /// <returns></returns>
        PagedCollection<Question> GetQuestionList(Course course, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);

        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="question"></param>
        /// <returns>The updated object that was persisted</returns>
        Question CreateQuestion(Course course, QuestionType questionType, string bank, string chapter);

        /// <summary>
        /// Returns question by its ID
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Question GetQuestion(Course course, string questionId);

        /// <summary>
        /// Creates template for new question based on existing one. 
        /// The question is not persisted at this point. Use CreateQuestion to persist the question after modification
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Question DuplicateQuestion(Course course, string questionId);

        /// <summary>
        /// Returns list of question types available for specified course
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        IEnumerable<QuestionType> GetQuestionTypesForCourse();

        /// <summary>
        /// Update existing question metafields
        /// </summary>
        /// <param name="temporaryQuestion"></param>
        /// <returns></returns>
        Question UpdateQuestion(Course course, string sourceQuestionId, Question temporaryQuestion);
        
        bool UpdateQuestionField(Course course, string questionId, string fieldName, string fieldValue);

        Question CreateTemporaryQuestion(Course course, string questionId);

        /// <summary>
        /// Updates shared value of the question
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        bool UpdateSharedQuestionField(string questionId, string fieldName, string fieldValue);
    }
}