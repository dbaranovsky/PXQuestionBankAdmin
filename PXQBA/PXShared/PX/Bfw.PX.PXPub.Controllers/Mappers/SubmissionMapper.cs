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
    public static class SubmissionMapper
    {
        /// <summary>
        /// Convert to a submission.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static Submission ToSubmission(this BizDC.Submission biz)
        {
            var model = new Submission();

            if (biz != null)
            {
                model.DateSubmitted = biz.SubmittedDate;
                model.Score = biz.Grade != null ? biz.Grade.Achieved : (double?) null;
                model.Body = biz.Body;
                model.Grade = biz.Grade.ToGrade();
                model.Version = biz.Version;
                model.Data = biz.Data;
                model.QuestionId = biz.ItemId;
                model.SubmissionStatus = biz.SubmissionStatus;

                if (biz.QuestionAttempts != null)
                {
                    model.QuestionAttempts = new Dictionary<string, IList<QuestionAttempt>>();

                    foreach (var qa in biz.QuestionAttempts)
                    {
                        var qaList =
                            qa.Value.Map(
                                q => new QuestionAttempt() { AttemptAnswer = q.AttemptAnswer, QuestionId = q.QuestionId, PartId = q.PartId, PointsPossible = q.PointsPossible, PointsComputed = q.PointsComputed, AttemptVersion = q.AttemptVersion })
                                .ToList();
                        model.QuestionId = qa.Key;
                        model.QuestionAttempts.Add(qa.Key, qaList);
                    }
                }

                if (biz.SubmissionAttempts != null)
                {
                    model.SubmissionAttempts = new Dictionary<string, SubmissionAttempt>();

                    foreach (var sa in biz.SubmissionAttempts)
                    {
                        var saList = new SubmissionAttempt()
                        {
                            LastSave = sa.Value.LastSave,
                            StartPage = sa.Value.StartPage,
                            PartId = sa.Value.PartId,
                            QuestionId = sa.Value.QuestionId,
                            SecondsSpent = sa.Value.SecondsSpent,
                            ToContinue = sa.Value.ToContinue
                        };
                        model.SubmissionAttempts.Add(sa.Key, saList);
                    }
                }
            }

            return model;
        }
    }
}
