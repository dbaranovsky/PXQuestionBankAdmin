using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Models
{
    public class TreeWidgetSettings
    {
        /// <summary>
        /// Seems similar to IsReadOnly
        /// </summary>
        public bool AllowEditing { get; set; }

        /// <summary>
        /// Can filter the items being displayed in the Unit
        /// </summary>
        public string ShowOnlyFilter { get; set; }

        /// <summary>
        /// Get/Set ability to drag drop within the unit
        /// </summary>
        public bool AllowDragDrop { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowCollapsedUnassigned { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CollapseUnassigned { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SplitAssigned { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SortByDueDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowPastDue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PastDueCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowDueLater { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DueLaterDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DueLaterCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool GreyoutPastDue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Shows/Hides the student item status.  This includes the hover/gearbox/assign functionality for instructor 
        /// (not sure if this is on purpose), 
        /// </summary>
        public bool ShowStudentDateData { get; set; }

        /// <summary>
        /// Get/Set the ability to open the content when it is clicked
        /// </summary>
        public bool OpenContentOnClick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show assignment unit workflow].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show assignment unit workflow]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAssignmentUnitWorkflow { get; set; }

        /// <summary>
        /// The template is of PX Unit for creating assignment units
        /// </summary>
        public string UnitTemplateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a caret(or +/- icon) to be added.
        /// </summary>
        /// <value>
        /// <c>true</c> if a caret(or +/- icon) is to be added; otherwise, <c>false</c>.
        /// </value>
        public bool ShowExpandIconAtAllLevels { get; set; }

        /// <summary>
        /// Get/Set the ability to show the description of the subcontainer
        /// </summary>
        public bool ShowDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowWidgetTitles { get; set; }

        /// <summary>
        /// Get/Set the ability to have a browse more resources item at the bottom of every subcontainer
        /// </summary>
        public bool ShowBrowseMoreResources { get; set; }

        /// <summary>
        /// Get/Set the ability to only have Learning Curve content items open up in FNE mode when clicked
        /// </summary>
        public bool FneOnlyLearningCurve { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool QuizBrowser { get; set; }

        /// <summary>
        /// ID of the tree widget this settings belongs to
        /// </summary>
        public string WidgetId { get; set; }

        /// <summary>
        /// ID of the course
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// The application that is using the tree widget
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// If the course is a sandbox course
        /// </summary>
        public bool IsSandboxCourse { get; set; }

        /// <summary>
        /// If set to true, items in this TOC will be loaded from the product course rather than the derivative
        /// This allows us to have a fresh copy of the TOC w/o any instructor modifications
        /// </summary>
        public bool UseProductCourse { get; set; } 

        /// <summary>
        /// Instructor vs Student
        /// </summary>
        public AccessLevel UserAccess { get; set; }

        /// <summary>
        /// Scrolls viewport to folder on open
        /// </summary>
        public bool ScrollOnOpen { get; set; }

        /// <summary>
        /// Close all open nodes when another is opened
        /// </summary>
        public bool CloseAllOnOpen { get; set; }

		/// <summary>
        /// TOC for this widget
        /// </summary>
        public string TOC { get; set; }

        /// <summary>
        /// The TOC that assignments will get added to, it different from active toc
        /// </summary>
        public string AssignmentTOC { get; set; }

        /// <summary>
        /// Container that the item belongs to.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Subcontainer that the item belongs to
        /// </summary>
        public string SubContainer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an item with specific toc needs to be removed on un-assignment.
        /// </summary>
        /// <value>
        /// <c>true</c> if [remove configuration unassign]; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool RemoveOnUnassign { get; set; }

        /// <summary>
        /// Gets or sets the removable setting where only specified item in this setting can be removed.
        /// The below is an example of DLAP schema
        /// 	<bfw_property name="bfw_removable_switch" type="Boolean">true</bfw_property>
		///	    <bfw_property name="bfw_removable_xpath_query" type="String">//bfw_tocs[my_materials='my_materials']</bfw_property>
		///	    <bfw_property name="bfw_remove_from_toc" type="String">syllabusfilter,assignmentfilter</bfw_property>
        /// </summary>
        /// <value>
        /// The removable setting.
        /// </value>
        public RemovableSetting RemovableSetting { get; set; }

        public TreeWidgetSettings()
        {
            //These default settings currently line up with how launchpad works
            AllowEditing = true;
            AllowDragDrop = true;
            ShowExpandIconAtAllLevels = false;
            ShowStudentDateData = true;
            ShowDescription = true;
            ShowBrowseMoreResources = true;
            OpenContentOnClick = false;
            ScrollOnOpen = true;
            CloseAllOnOpen = true;
            TOC = string.Empty;
            AssignmentTOC = string.Empty;
        }
    }
}
