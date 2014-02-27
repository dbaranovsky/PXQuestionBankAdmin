using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class QuizSearchController : Controller
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the navigation actions.
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search actions.
        /// </summary>
        /// <value>
        /// The search actions.
        /// </value>
        protected BizSC.ISearchActions SearchActions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the question actions.
        /// </summary>
        /// <value>
        /// The question actions.
        /// </value>
        protected BizSC.IQuestionActions QuestionActions
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizSearchController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navigationActions">The navigation actions.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="searchActions">The search actions.</param>
        public QuizSearchController(BizSC.IBusinessContext context, BizSC.INavigationActions navigationActions, BizSC.IContentActions contentActions, BizSC.ISearchActions searchActions, BizSC.IQuestionActions questionActions)
        {
            Context = context;
            NavigationActions = navigationActions;
            ContentActions = contentActions;
            SearchActions = searchActions;
            QuestionActions = questionActions;
        }

        /// <summary>
        /// Searches the questions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public ActionResult SearchQuestions(Models.SearchQuery query, QuizBrowserMode mode = QuizBrowserMode.QuestionPicker)
        {
            var bizQuery = query.ToSearchQuery();
            var entityId = QuestionActions.GetQuestionRepositoryCourse(Context.EntityId);
            entityId = entityId.IsNullOrEmpty() ? Context.EntityId : entityId;

            int numFound = 0;
            var searchResults = SearchActions.DoQuestionSearch(bizQuery, entityId, out numFound);

           ViewData.Model = new QuizSearchResults()
            {
                Query = query,
                Quiz = new Quiz() {
                    Questions = (searchResults != null) ? searchResults.Map(q => q.ToQuestion()).ToList() : null, 
                    ShowReused = true,
                    CourseInfo = Context.Course.ToCourse(),
                    QuizPaging =
                         new Paging()
                         {
                             StartIndex = query.Start.ToString(),
                             LastIndex = query.Rows.ToString(),
                             TotalCount = numFound
                         }
                }
            };

            ViewData["mode"] = mode;
            
            return View("DisplayTemplates/QuizPartials/SearchQuiz");
        }
    }
}
