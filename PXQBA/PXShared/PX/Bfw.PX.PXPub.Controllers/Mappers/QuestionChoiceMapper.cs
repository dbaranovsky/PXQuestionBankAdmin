using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class QuestionChoiceMapper
    {

        /// <summary>
        /// Convert to a Question Choice from a Biz QuestionChoice.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static QuestionChoice ToQuestionChoice(this BizDC.QuestionChoice biz)
        {
            return new QuestionChoice() { Id = biz.Id, Text = biz.Text, Feedback = biz.Feedback, Answer = biz.Answer };
        }

        /// <summary>
        /// Convert to a question choice from a Models QuestionChoice.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.QuestionChoice ToQuestionChoice(this QuestionChoice model)
        {
            return new BizDC.QuestionChoice() { Id = model.Id, Text = model.Text, Feedback = model.Feedback, Answer = model.Answer};
        }
    }
}
