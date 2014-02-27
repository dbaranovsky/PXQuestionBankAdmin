// -----------------------------------------------------------------------
// <copyright file="LearningCurveQuestionSettingsMapper.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class LearningCurveQuestionSettingsMapper
    {
        /// <summary>
        /// Convert to a Question Choice from a Biz QuestionChoice.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static LearningCurveQuestionSettings ToLearningCurveQuestionSettings(this BizDC.LearningCurveQuestionSettings biz)
        {
            return new LearningCurveQuestionSettings()
            {
                QuizId = biz.Id,
                DifficultyLevel = biz.DifficultyLevel,
                NeverScrambleAnswers = biz.NeverScrambleAnswers,
                PrimaryQuestion = biz.PrimaryQuestion
            };
        }

        /// <summary>
        /// Convert to a question choice from a Models QuestionChoice.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.LearningCurveQuestionSettings ToLearningCurveQuestionSettings(this LearningCurveQuestionSettings model)
        {
            return new BizDC.LearningCurveQuestionSettings()
            {
                Id = model.QuizId,
                DifficultyLevel = model.DifficultyLevel,
                NeverScrambleAnswers = model.NeverScrambleAnswers,
                PrimaryQuestion = model.PrimaryQuestion
            };
        }
    }
}
