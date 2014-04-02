﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.DataAccess.Data;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class QuestionCommands : IQuestionCommands
    {
        private readonly IQBAUow qbaUow;

        public QuestionCommands(IQBAUow qbaUow, IModelProfileService modelProfileService)
        {
            this.qbaUow = qbaUow;
        }

        public void SaveQuestions(IList<Question> questions)
        {
            qbaUow.DbContext.Questions.AddRange(Mapper.Map<List<DataAccess.Data.Question>>(questions));
            qbaUow.Commit();
        }

        private static IQueryable<ProductCourse> BuildSorting(IQueryable<ProductCourse> query, SortCriterion sortCriterion)
        {
            if (sortCriterion != null && sortCriterion.SortType != SortType.None)
            {
                if (sortCriterion.ColumnName == MetadataFieldNames.Bank)
                {
                    return sortCriterion.IsAsc
                                ? query.OrderBy(x => x.Bank)
                                : query.OrderByDescending(x => x.Bank);
                }
                if (sortCriterion.ColumnName == MetadataFieldNames.Chapter)
                {
                    return sortCriterion.IsAsc
                                ? query.OrderBy(x => x.Chapter)
                                : query.OrderByDescending(x => x.Chapter);
                }
                if (sortCriterion.ColumnName == MetadataFieldNames.DlapTitle)
                {
                    return sortCriterion.IsAsc
                                ? query.OrderBy(x => x.Title)
                                : query.OrderByDescending(x => x.Title);
                }
                if (sortCriterion.ColumnName == MetadataFieldNames.DlapStatus)
                {
                    return sortCriterion.IsAsc
                                ? query.OrderBy(x => x.Question.Status)
                                : query.OrderByDescending(x => x.Question.Status);
                }
                if (sortCriterion.ColumnName == MetadataFieldNames.Sequence)
                {
                    return sortCriterion.IsAsc
                                ? query.OrderBy(x => x.Sequence)
                                : query.OrderByDescending(x => x.Sequence);
                }
            }
            return query.OrderBy(x => x.Id);
        }

        private static Question ConvertProductCourseQuestion(ProductCourse course)
        {
            return Mapper.Map<ProductCourse, Question>(course);
        }

        public PagedCollection<Question> GetQuestionList(string courseId, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            var questionsQuery = qbaUow.DbContext.ProductCourses.Where(q => q.ProductCourseDlapId == courseId);
            questionsQuery = BuildSorting(questionsQuery, sortCriterion);

            var result = new PagedCollection<Question>
                {
                    TotalItems = questionsQuery.Count(),
                    CollectionPage =
                        questionsQuery.Skip(startingRecordNumber).Take(recordCount).Select(ConvertProductCourseQuestion).ToList()
                };
            return result;
        }

        public bool UpdateQuestionField(string questionId, string fieldName, string value)
        {
            int id;
            if (int.TryParse(questionId, out id))
            {
                var question = qbaUow.DbContext.Questions.FirstOrDefault(q => q.Id == id);
                if (question != null)
                {
                    switch (fieldName)
                    {
                        case MetadataFieldNames.DlapStatus:
                            question.Status = ((int) EnumHelper.GetItemByDescription(typeof (QuestionStatus), value));
                            break;
                    }
                    qbaUow.Commit();
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}  
