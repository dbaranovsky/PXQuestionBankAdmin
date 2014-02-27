//XbookRoutes - handles routing for xbookwidget hash url's
var XbookMediaRoutes = function ($) {
    return {
        //Navigates the TOC to the itemid
        toc_navigateTo: function (args) {
            if (args != undefined && args.itemid !== undefined) {
                PxXbookWidget.toc_navigateTo(args.itemid);
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
        }
    };
} (jQuery);