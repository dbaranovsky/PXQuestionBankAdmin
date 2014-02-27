using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Defines the course assessment configuration.
    /// </summary>
    public class CourseAssessmentConfiguration
    {
        /// <summary>
        /// Configuration for rubric component.
        /// </summary>
        public RubricCourseConfiguration Rubrics { get; set; }


        /// <summary>
        /// Configuration for learning objectives component.
        /// </summary>
        public ObjectivesCourseConfiguration Objectives { get; set; }

        /// <summary>
        /// Configuration for report component.
        /// </summary>
        public ReportCourseConfiguration Reports { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseAssessmentConfiguration"/> class.
        /// </summary>
        public CourseAssessmentConfiguration()
        {
            Rubrics = new RubricCourseConfiguration();
            Objectives = new ObjectivesCourseConfiguration();
            Reports = new ReportCourseConfiguration();
        }
    }

    [Serializable]
    /// <summary>
    /// Representation of a course rubric component configuration in the system.
    /// </summary>
    public class RubricCourseConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show preview link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show preview link.
        /// </value>
        public Boolean ShowPreviewOnLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show preview link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show preview link.
        /// </value>
        public Boolean ShowPreviewOnRight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show edit link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show preview link.
        /// </value>
        public Boolean ShowEditOnLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show delete link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show preview link.
        /// </value>
        public Boolean ShowDeleteOnLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show edit link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show preview link.
        /// </value>
        public Boolean ShowEditOnRight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show alignments screen.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show alignments screen.
        /// </value>
        public Boolean ShowViewAlignments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show eportfolio alignments option.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show eportfolio alignments option.
        /// </value>
        public Boolean ShowRubricAlignments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show assignment alignments option.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show assignment alignments option.
        /// </value>
        public Boolean ShowAssignmentAlignments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the course rubrics screen.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show course rubrics screen.
        /// </value>
        public Boolean ShowLeftColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show in assessment.
        /// </summary>
        public Boolean ShowInAssessment { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RubricCourseConfiguration"/> class.
        /// </summary>
        public RubricCourseConfiguration()
        {
            
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the course rubrics screen.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show course rubrics screen.
        /// </value>
        public bool ShowRightColumn { get; set; }
    }

    [Serializable]
    /// <summary>
    /// Representation of a course learning objectives component configuration in the system.
    /// </summary>
    public class ObjectivesCourseConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show edit link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show preview link.
        /// </value>
        public Boolean ShowEditOnLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show edit link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show preview link.
        /// </value>
        public Boolean ShowEditOnRight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the course rubrics screen.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show course rubrics screen.
        /// </value>
        public Boolean ShowLeftColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show alignments screen.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show alignments screen.
        /// </value>
        public Boolean ShowViewAlignments { get; set; }        

        /// <summary>
        /// Gets or sets a value indicating whether to show objective alignments option.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show objective alignments option.
        /// </value>
        public Boolean ShowObjectiveAlignments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show assignment alignments option.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show assignment alignments option.
        /// </value>
        public Boolean ShowAssignmentAlignments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show in assessment.
        /// </summary>
        public Boolean ShowInAssessment { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectivesCourseConfiguration"/> class.
        /// </summary>
        public ObjectivesCourseConfiguration()
        {

        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the course learning objectives screen.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if course should show courses learning objectives screen.
        /// </value>
        public Boolean ShowRightColumn { get; set; }
    }

    [Serializable]
    /// <summary>
    /// Representation of a course report component configuration in the assessment tab.
    /// </summary>
    public class ReportCourseConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show in assessment.
        /// </summary>
        public Boolean ShowInAssessment { get; set; }
    }
}