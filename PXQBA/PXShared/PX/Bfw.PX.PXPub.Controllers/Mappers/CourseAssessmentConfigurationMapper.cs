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
    public static class CourseAssessmentConfigurationMapper
    {
        /// <summary>
        /// Converts to a Grade Book Weight Catgory from a Biz Grade Book Weight Category.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static CourseAssessmentConfiguration ToModel(this BizDC.CourseAssessmentConfiguration biz)
        {
            var model = new CourseAssessmentConfiguration();
            
            model.Rubrics.ShowEditOnLeft = biz.Rubrics.ShowEditOnLeft;
            model.Rubrics.ShowDeleteOnLeft = biz.Rubrics.ShowDeleteOnLeft;
            model.Rubrics.ShowEditOnRight = biz.Rubrics.ShowEditOnRight;
            model.Rubrics.ShowLeftColumn = biz.Rubrics.ShowLeftColumn;
            model.Rubrics.ShowRightColumn = biz.Rubrics.ShowRightColumn;
            model.Rubrics.ShowPreviewOnLeft = biz.Rubrics.ShowPreviewOnLeft;
            model.Rubrics.ShowPreviewOnRight = biz.Rubrics.ShowPreviewOnRight;
            model.Rubrics.ShowViewAlignments = biz.Rubrics.ShowViewAlignments;
            model.Rubrics.ShowRubricAlignments = biz.Rubrics.ShowRubricAlignments;
            model.Rubrics.ShowAssignmentAlignments = biz.Rubrics.ShowAssignmentAlignments;
            model.Rubrics.ShowInAssessment = biz.Rubrics.ShowInAssessment;

            model.Objectives.ShowLeftColumn = biz.Objectives.ShowLeftColumn;
            model.Objectives.ShowRightColumn = biz.Objectives.ShowRightColumn;
            model.Objectives.ShowEditOnLeft = biz.Objectives.ShowEditOnLeft;
            model.Objectives.ShowEditOnRight = biz.Objectives.ShowEditOnRight;
            model.Objectives.ShowViewAlignments = biz.Objectives.ShowViewAlignments;
            model.Objectives.ShowObjectiveAlignments = biz.Objectives.ShowObjectiveAlignments;
            model.Objectives.ShowAssignmentAlignments = biz.Objectives.ShowAssignmentAlignments;
            model.Objectives.ShowInAssessment = biz.Objectives.ShowInAssessment;
            
            model.Reports.ShowInAssessment = biz.Reports.ShowInAssessment;

            return model;
        }
    }
}