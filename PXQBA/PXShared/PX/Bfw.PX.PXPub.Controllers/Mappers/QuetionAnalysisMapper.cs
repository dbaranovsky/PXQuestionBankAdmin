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
    public static class QuetionAnalysisMapper
    {

        /// <summary>
        /// Convert to a Question Analysis.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static QuestionAnalysis ToQuestionAnalysis(this BizDC.QuestionAnalysis biz)
        {
            var model = new QuestionAnalysis
                            {
                                Attempts = biz.Attempts,
                                CorrectAnswerCount = biz.CorrectAnswerCount,
                                Correlation = biz.Correlation,
                                Enrollments = biz.Enrollments,
                                QuestionId = biz.QuestionId,
                                QuestionNumber = biz.QuestionNumber,
                                Score = biz.Score,
                                AverageTime = biz.AverageTime
                            };
            return model;
        }

    }
}
