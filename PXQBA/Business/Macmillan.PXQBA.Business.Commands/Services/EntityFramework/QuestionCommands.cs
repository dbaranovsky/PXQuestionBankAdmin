using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.DataAccess.Data;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Commands.Services.SQL
{
    public class QuestionCommands : IQuestionCommands
    {
        private readonly IQBAUow qbaUow;

        public QuestionCommands(IQBAUow qbaUow)
        {
            this.qbaUow = qbaUow;
        }

        public void SaveQuestions(IList<Question> questions)
        {
            qbaUow.DbContext.Questions.AddRange(Mapper.Map<List<DataAccess.Data.Question>>(questions));
            qbaUow.Commit();
        }

        public QuestionList GetQuestionList(string query, int page, int questionPerPage)
        {
            var questions = qbaUow.DbContext.Questions.OrderBy(q => q.Id).Skip((page-1)*questionPerPage).Take(questionPerPage).Select(Mapper.Map<Question>);

            return new QuestionList
            {
                Questions = questions.ToList(),
                AllQuestionsAmount = qbaUow.DbContext.Questions.Count()
            };
        }
    }
}  
