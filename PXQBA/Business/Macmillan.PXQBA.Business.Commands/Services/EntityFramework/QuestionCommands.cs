using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public QuestionCommands(QBADummyModelContainer dbContext)
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

        public PagedCollection<Question> GetQuestionList(string courseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            var questionsQuery = dbContext.ProductCourses.Where(q => q.ProductCourseDlapId == courseId);
            questionsQuery = BuildSorting(questionsQuery, sortCriterion);
            questionsQuery = BuildFiltering(questionsQuery, filter);

            var result = new PagedCollection<Question>
                {
                    TotalItems = questionsQuery.Count(),
                    CollectionPage =
                        questionsQuery.Skip(startingRecordNumber).Take(recordCount).Select(ConvertProductCourseQuestion).ToList()
                };
            return result;
        }

        private IQueryable<ProductCourse> BuildFiltering(IQueryable<ProductCourse> query, IEnumerable<FilterFieldDescriptor> filter)
        {
            if (filter.Any())
            {
                foreach (var filterFieldDescriptor in filter.Where(f => f.Values.Any()))
                {
                    if (filterFieldDescriptor.Field == MetadataFieldNames.Bank)
                    {
                        query = query.Where(x => filterFieldDescriptor.Values.Select(v => v.ToUpper()).Contains(x.Bank.ToUpper()));
                    }
                    if (filterFieldDescriptor.Field == MetadataFieldNames.Chapter)
                    {
                        query = query.Where(x => filterFieldDescriptor.Values.Select(v => v.ToUpper()).Contains(x.Chapter.ToUpper()));
                    }
                    if (filterFieldDescriptor.Field == MetadataFieldNames.Difficulty)
                    {
                        query = query.Where(x => filterFieldDescriptor.Values.Select(v => v.ToUpper()).Contains(x.Difficulty.ToUpper()));
                    }
                    if (filterFieldDescriptor.Field == MetadataFieldNames.LearningObjectives)
                    {
                        query = query.Where(p => filterFieldDescriptor.Values.Any(v => p.LearningObjectives.ToUpper().Contains(v.ToUpper())));
                    }
                }
            }
            return query;
        }

        private bool GeneratePredicateForLearningObjective(ProductCourse course, IEnumerable<string> values)
        {
            var result = false;
            foreach (var value in values)
            {
                result = result || course.LearningObjectives.ToUpper().Contains(value.ToUpper());
            }
            return result;
        }

        public Question CreateQuestion(string courseId, Question question)
        {
            question.Id = Guid.NewGuid().ToString();
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

        public Question UpdateQuestion(Question question)
        {
            var existingQuestion = dbContext.ProductCourses.FirstOrDefault(q => q.Question.DlapId == question.Id);
            if (existingQuestion == null)
            {
                throw new ArgumentException("Provided question does not exist");
            }

            Mapper.Map(question, existingQuestion);
            dbContext.SaveChanges();

            return question;
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

        public bool UpdateSharedQuestionField(string questionId, string fieldName, string fieldValue)
        {
            //TODO: implement in real data
            return true;
        }
    }
}  
