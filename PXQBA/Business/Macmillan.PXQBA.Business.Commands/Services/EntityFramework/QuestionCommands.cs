using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.DataAccess.Data;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class QuestionCommands : IQuestionCommands
    {
        private readonly IQBAUow qbaUow;
        private readonly IModelProfileService modelProfileService;

        public QuestionCommands(IQBAUow qbaUow, IModelProfileService modelProfileService)
        {
            this.qbaUow = qbaUow;
            this.modelProfileService = modelProfileService;
        }

        public void SaveQuestions(IList<Question> questions)
        {
            qbaUow.DbContext.Questions.AddRange(Mapper.Map<List<DataAccess.Data.Question>>(questions));
            qbaUow.Commit();
        }

        public QuestionList GetQuestionList(string query, int page, int questionPerPage)
        {
            // TODO: needs to parse query and take productCourseId (titleId) and other parameters from there
            // After Product course is selected
            var questionsTotal = qbaUow.DbContext.Questions.Where(q => q.ProductCourses.Any(p => p.ProductCourseDlapId == Constants.ProductCourseId)).OrderBy(q => q.Id);

             var questionPage = questionsTotal.Skip((page - 1)*questionPerPage)
                    .Take(questionPerPage)
                    .Select(Mapper.Map<QuestionMetadata>);

            return new QuestionList
            {
                Questions = questionPage.ToList(),
                AllQuestionsAmount = questionsTotal.Count()
            };
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
