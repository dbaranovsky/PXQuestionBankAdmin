using System;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.DataAccess.Data;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class QuestionCommands : IQuestionCommands
    {
        private readonly QBADummyModelContainer dbContext;

        public QuestionCommands(QBADummyModelContainer dbContext, IModelProfileService modelProfileService)
        {
            this.dbContext = dbContext;
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
                if (sortCriterion.ColumnName == MetadataFieldNames.DlapType)
                {
                    return sortCriterion.IsAsc
                                ? query.OrderBy(x => x.Question.Type)
                                : query.OrderByDescending(x => x.Question.Type);
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
            var questionsQuery = dbContext.ProductCourses.Where(q => q.ProductCourseDlapId == courseId);
            questionsQuery = BuildSorting(questionsQuery, sortCriterion);

            var result = new PagedCollection<Question>
                {
                    TotalItems = questionsQuery.Count(),
                    CollectionPage =
                        questionsQuery.Skip(startingRecordNumber).Take(recordCount).Select(ConvertProductCourseQuestion).ToList()
                };
            return result;
        }

        public Question CreateQuestion(string courseId, Question question)
        {
            DataAccess.Data.Question questionEntity = Mapper.Map<Question, DataAccess.Data.Question>(question);
            dbContext.Questions.AddObject(questionEntity);
            dbContext.SaveChanges();

            ProductCourse courceQuestion = Mapper.Map<Question, ProductCourse>(question);
            courceQuestion.ProductCourseDlapId = courseId;
            courceQuestion.QuestionId = questionEntity.Id;

            dbContext.ProductCourses.AddObject(courceQuestion);

            dbContext.SaveChanges();

            return question;
        }

        public Question GetQuestion(string questionId)
        {
            var entity = dbContext.ProductCourses.FirstOrDefault(q => q.Question.DlapId == questionId);
            return ConvertProductCourseQuestion(entity);
        }

        public void UpdateQuestionSequence(string courseId, string questionId, int newSequenceValue)
        {
            var entity = dbContext.ProductCourses.FirstOrDefault(q => q.Question.DlapId == questionId && q.ProductCourseDlapId == courseId);
            var sameBankQuestions =
                dbContext.ProductCourses.Where(q => q.Bank == entity.Bank && q.ProductCourseDlapId == courseId && q.Sequence >= newSequenceValue);
            foreach (var sameBankQuestion in sameBankQuestions)
            {
                ++sameBankQuestion.Sequence;
            }
            entity.Sequence = newSequenceValue;
            dbContext.SaveChanges();
        }

        public bool UpdateQuestionField(string questionId, string fieldName, string value)
        {
            var question = dbContext.Questions.FirstOrDefault(q => q.DlapId == questionId);
            if (question != null)
            {
                switch (fieldName)
                {
                    case MetadataFieldNames.DlapStatus:
                        question.Status = (int)((QuestionStatus)Enum.Parse(typeof(QuestionStatus), value, true));
                        break;
                }
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}  
