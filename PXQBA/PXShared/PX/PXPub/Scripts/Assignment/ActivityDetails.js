// ActivityDetails
//
// This plugin is responsible for the client-side behaviors of activity details
(function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: 'ActivityDetails',
        dataKey: 'ActivityDetails',
        bindSuffix: '.ActivityDetails',
        dataAttrPrefix: 'data-ad-',
        readOnly: false,
        constant: {
        },
        cache: {
        },
        defaults: {

        },
        sel: {
            divTitle: '.activity-details-title',
            divContent: '.activity-details-content',
            divRubric: '.rubricView',
            divRubricColumn: '.tblAssignmentView .rubric-score-guides'
        },
        // private functions
        fn: {
            collapseContent: function () {

            },
            expandContent: function () {

            },
            onClickTitle: function (event) {

                if ($(_static.sel.divContent).is(':visible')) {
                    $(_static.sel.divTitle).html('+ Activity Details');
                } else {
                    $(_static.sel.divTitle).html('- Activity Details');


                }
                $(_static.sel.divContent).slideToggle(400);

            }

        }
    },
    // The public interface for interacting with this plugin.
    api = {
        init: function (options) {
            //setup styles
            $(_static.sel.divRubric).css('width','auto');
            $(_static.sel.divRubric).css('float','none');
            $(_static.sel.divRubric).css('border-right','none');
            $(_static.sel.divRubric).css('padding','0px');
            $(_static.sel.divRubricColumn).css('width', '20%');

            $(_static.sel.divContent).hide();

            $(_static.sel.divTitle).unbind('click');

            $(_static.sel.divTitle).click(function (event) {
                _static.fn.onClickTitle();
            });


        }
    };



    $.fn.ActivityDetails = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };

    window.ActivityDetails = {
        init: function (options) {
            api.init.apply(this, arguments);
        }
    };
} (jQuery))