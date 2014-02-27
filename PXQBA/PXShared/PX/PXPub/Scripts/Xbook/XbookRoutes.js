//XbookRoutes - handles routing for xbookwidget hash url's
var XbookRoutes = function ($) {
    return {
        //Navigates the TOC to the itemid
        displayContentItem: function (args) {
            if (args.displayAssign) {
                PxXbookWidget.toc_displayAddAssignment();
            }
            if (args && args.itemid) {
                PxXbookWidget.toc_navigateTo(args.itemid, args.assignmentid);
            }
        },
        //Displays the px add content dialog
        displayAddContentDialog: function (args) {
            var itemid = args.itemid || '';
            PxXbookWidget.displayAddContentDialog(itemid);
        },
        //Opens up the edit course links dialog
        courseLinks: function () {
            PxXbookWidget.editCourseLinks();
        },
        //Opens a assignment folder on the assignment list tab
        displayAssignment: function (args) {
            var assignmentid = args.assignmentid || '';
            var reload = args.reload || false;
            if (reload) {
                PxXbookWidget.assignmentList_reload();
            }
            PxXbookWidget.assignmentList_openFolder(assignmentid);
        }
    };
} (jQuery);