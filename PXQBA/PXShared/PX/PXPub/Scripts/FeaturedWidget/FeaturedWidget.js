//Script file for controlling actions associated with the Featured Content Widget
var PxFeaturedWidget = function ($) {
    var _static = {
        sel: {
            widgetHeaderText: '.widgetHeaderText', //widget header will contain the featured widget title
            contentType: 'contenttype',
            widgetItem: '.widgetItem',
            featuredWidget: '.featuredwidget'
        },

        fn: {
            init: function () {
                $(_static.sel.featuredWidget). //wrapper for the featured content in the Widget
                closest(_static.sel.widgetItem). //wrapper for the entire Widget
                find(_static.sel.widgetHeaderText). //wrapper for the title of the Widget
                bind('click', _static.fn.featuredTitleClicked);
            },

            //Trigger the "FeaturedWidgetClicked" event when the header of the Featured Widget is clicked.
            // Also pass the content type (LearningCurve, VideoPlayer etc.) while triggering the event
            featuredTitleClicked: function () {
                var featuredWidget = $(this).closest(_static.sel.widgetItem).find(_static.sel.featuredWidget);
                var contentType = $(featuredWidget).attr(_static.sel.contentType);
                $(PxPage.switchboard).trigger("FeaturedWidgetClicked", [contentType]);
            }
        }
    };

    return {

        Init: function () {
            _static.fn.init();
        }
    };
} (jQuery);