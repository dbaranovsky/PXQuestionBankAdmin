//GradebookRoutes - handles routing for gradebook widget and related dialogs
var GradebookRoutes = function ($) {
    return {
        //gradebook to display assigned scores view
        assignedScores: function (args) {
            PxGradebook.CloseGradebookDialog();
            PxGradebook.ShowAssignedScores();
        },
        //displays the other scores view
        otherScores: function (args) {
            PxGradebook.ShowOtherScores();
        },
        //displays the item scores view
        itemScores: function (args) {
            if (args !== undefined && args.itemid !== undefined && args.itemType !== undefined && args.username !== undefined) {
                PxGradebook.ShowItemScores(args.itemid, args.itemType, args.username);
            }
        },
        //displays the assignment summary (from the tool tip) view
        assignmentSummary: function (args) {
            if (args !== undefined && args.assignmentid !== undefined) {
                PxGradebook.ShowAssignmentSummary(args.assignmentid);
            }
        },
        //displays the student detail ( click on a student name ) view
        studentDetail: function (args) {
            if (args !== undefined && args.studentUserId !== undefined && args.studentEnrollmentId !== undefined) {
                PxGradebook.ShowStudentDetail(args.studentUserId, args.studentEnrollmentId);
            }
        },
        viewAssignment: function (args) {
            $(PxPage.switchboard).trigger('viewassignment', [args.itemid]);
        }
    };
} (jQuery);