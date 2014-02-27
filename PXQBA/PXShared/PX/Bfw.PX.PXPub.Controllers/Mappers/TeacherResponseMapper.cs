using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class TeacherResponseMapper
    {
        /// <summary>
        /// Converts to a Teacher Response from an Assingment Model
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        public static TeacherResponse ToTeacherResponse(this Assignment model, string behavior)
        {
            var biz = new TeacherResponse
            {
                TeacherResponseType = TeacherResponseType.None,
                Mask = GradeStatus.ShowScore,
                Status = GradeStatus.ShowScore,
                PointsPossible = model.PossibleScore,
                PointsAssigned = model.AssignedScore,
                ScoredVersion = model.Submission.Version
            };

            switch (behavior.ToLowerInvariant())
            {
                case "save":
                    biz.ScoredVersion = 2;
                    break;
                case "unsubmit":
                    biz.Mask = GradeStatus.AllowResubmission;
                    biz.Status = GradeStatus.AllowResubmission;
                    break;
            }            

            return biz;
        }
    }
}
