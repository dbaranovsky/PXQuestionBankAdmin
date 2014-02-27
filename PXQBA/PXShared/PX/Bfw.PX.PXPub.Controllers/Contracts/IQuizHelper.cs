using System.Collections.Generic;

using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Contracts
{
    public interface IQuizHelper
    {
        /// <summary>
        /// The default points for a quiz if none are set on the backend
        /// </summary>
        int DefaultHtmlQuizPoints { get; }

        /// <summary>
        /// Copies all questions to the current entity, along with all bank questions
        /// Saves question order, points, bank use
        /// Saves all question banks
        /// </summary>
        /// <param name="quizQuestions"></param>
        /// <param name="contentActions"></param>
        /// <param name="questionActions"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        List<Biz.DataContracts.Question> UpdateQuizFromQuizQuestions(QuizQuestions quizQuestions,
            BizSC.IContentActions contentActions, BizSC.IQuestionActions questionActions, BizSC.IBusinessContext context);

        /// <summary>
        /// Copies all questions to the current entity, along with all bank questions
        /// Saves question order, points, bank use
        /// Saves all question banks
        /// </summary>
        /// <param name="item"></param>
        /// <param name="contentActions"></param>
        /// <param name="questionActions"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        List<Biz.DataContracts.Question> UpdateQuizFromQuizItem(Quiz item, BizSC.IContentActions contentActions, BizSC.IQuestionActions questionActions, BizSC.IBusinessContext context);

        /// <summary>
        /// Sets all necessary fields to make the item show up in the gradebook
        /// </summary>
        /// <param name="itemId">Id of the item to make gradable</param>
        /// <param name="contentActions"></param>
        /// <param name="context"></param>
        /// <returns>True if the item was made gradable, false otherwise</returns>
        bool MakeQuizGradable(string itemId, BizSC.IContentActions contentActions, BizSC.IBusinessContext context);
    }
}