using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Web.Mvc;
using Bfw.PX.PXPub.Components;
using System.Web.UI;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class QuestionImporterController : Controller
    {
        internal struct ParseResults
        {
            internal List<string> Errors;
            internal List<RespondusQuestion> Questions;
        }

        /// <summary>
        /// Contains business layer context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IQuestionActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IQuestionActions QuestionActions { get; set; }

        /// <summary>
        /// Access to an IQuestionImporterActions implementation.
        /// </summary>
        /// <value>
        /// The question importer actions.
        /// </value>
        protected BizSC.IQuestionImporterActions QuestionImporterActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionImporterController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="questionActions">The Question actions.</param>
        /// <param name="questionImporterActions">The questionImporterActions.</param>
        public QuestionImporterController(BizSC.IBusinessContext context, BizSC.IQuestionActions questionActions, BizSC.IQuestionImporterActions questionImporterActions)
        {
            Context = context;
            QuestionActions = questionActions;
            QuestionImporterActions = questionImporterActions;
        }

        /// <summary>
        /// Returns the importer dialog 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ImporterDialog()
        {
            return PartialView("~/Views/Shared/DisplayTemplates/QuizPartials/ImporterDialog.ascx");
        }

        /// <summary>
        /// Parses out and validates string content into respondus questions
        /// </summary>
        /// <param name="data"></param>
        /// <returns>set of validation errors</returns>
        [OutputCache(Duration = 1, VaryByParam = "none", Location = OutputCacheLocation.Any)]
        [ValidateInput(false)]
        public JsonResult ValidateRespondusQuestions(string data)
        {
            // validate against syntax rules            
            var parseResults = ParseRespondusQuestions(data);

            var questions = parseResults.Questions;
            var errors = parseResults.Errors;

            // validate against DLAP            
            if (errors.Count == 0)
            {
                var questionsWithErrors = QuestionImporterActions.ValidateThruDLAP(Context.EntityId, questions, QuestionActions);

                errors = (from q in questionsWithErrors
                          select String.Format("|{0}|{1}", q.ValidationError, q.Text)).ToList();
            }
            
            return new JsonDataContractResult(errors.ToArray());
        }

        /// <summary>
        /// Adds questions to the course and to the current quiz
        /// </summary>
        /// <param name="data"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult ImportRespondusQuestions(string data, string quizId)
        {
            // validate against syntax rules            
            var parseResults = ParseRespondusQuestions(data);

            var questions = parseResults.Questions;
            var errors = parseResults.Errors;

            if (errors.Count == 0)
            {
                questions = QuestionImporterActions.Import(Context.EntityId, questions, quizId, QuestionActions);

                if (questions.Count(o => o.ValidationError != null) > 0)
                {
                    errors = (from q in questions
                              where q.ValidationError != null
                              select String.Format("|{0}|{1}", q.ValidationError, q.Text)).ToList();

                    return new JsonDataContractResult(errors.ToArray());
                }
            }
            else
            {
                return new JsonDataContractResult(errors.ToArray());
            }

            return new JsonDataContractResult("SUCCESS");
        }

        /// <summary>
        /// Validates and parses out questions 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ParseResults ParseRespondusQuestions(string data)
        {
            var questions = QuestionImporterActions.Parse(data);
            var questionsWithErrors = questions.Where(o => o.ValidationError != null);

            var errors = (from q in questionsWithErrors
                          select String.Format("{0}|{1}|{2}", 
                            q.ValidationError.Split(new string[] { "|" }, StringSplitOptions.None)[0], 
                            q.ValidationError.Split(new string[] { "|" }, StringSplitOptions.None)[1], 
                            q.Text))
                            .ToList();

            var answers = questions.SelectMany(o => o.Choices);
            var answersWithErrors = answers.Where(o => o.ValidationError != null);

            errors.AddRange((from a in answersWithErrors
                          select String.Format("{0}|{1}|{2}", 
                            a.ValidationError.Split(new string[] { "|" }, StringSplitOptions.None)[0],
                            a.ValidationError.Split(new string[] { "|" }, StringSplitOptions.None)[1], 
                            a.Text))
                            .ToList());
            
            return new ParseResults() { Errors = errors, Questions = questions };
        }
    }
}
