// DisplayRubric
//
// This plugin is responsible for the client-side behaviors of displaying a rubric

(function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: 'DisplayRubric',
        dataKey: 'DisplayRubric',
        bindSuffix: '.DisplayRubric',
        dataAttrPrefix: 'data-dc-',
        readOnly: false,
        constant: {
            classNoBackground: 'no-background',
            eventRubricDataUpdated: 'rubric_data_updated',
            eventRubricDataUpdatedCallBack: 'rubric_data_updated_callback',
            eventRubricShow: 'show_rubric'
        },
        cache: {
        },
        defaults: {

        },
        sel: {
            submissionStatus: '.student-submission-status',
            divRubric: '.rubricView',
            rubricScore: '#divAssignmentViewer #divInstructorView .rubricrow .rubric-score-guides',
            divRubricBlock: '.rubric-score-guides',
            listener: ''
        },
        // private functions
        fn: {
            unbindRubric: function () {
                $(_static.sel.rubricScore).die();
            }
        }
    },
    // The public interface for interacting with this plugin.
    api = {
        init: function (options) {
            if (options) {
                if (options.readOnly && options.readOnly.toLowerCase() == 'true') {
                    _static.readOnly = true;
                } else {
                    _static.readOnly = false;
                }
            }

            if (_static.readOnly) {

                $(_static.sel.rubricScore).unbind();
                $(_static.sel.rubricScore).die();
                $(_static.sel.rubricScore).click(_static.fn.unbindRubric);

            } else {
            }

            $(_static.sel.divRubric).unbind(_static.constant.eventRubricShow);
            $(_static.sel.divRubric).bind(_static.constant.eventRubricShow, function (event, element) {
                if ($('#fne-window').is(':visible')) {
                    $('#fne-window ' + element).css('display', '');

                }
                $(element).css('display', 'block');

            });

            $(_static.sel.rubricScore).click(function (event) {
                if ($(!_static.readOnly && $(_static.sel.submissionStatus).length > 0)) {
                    var score = $(event.currentTarget);
                    if (score) {
                        $(_static.sel.submissionStatus).trigger(_static.constant.eventRubricDataUpdated, [score.attr(_static.dataAttrPrefix + 'id'), score.attr(_static.dataAttrPrefix + 'point')]);
                    }
                }
            });
            $(_static.sel.divRubricBlock).css('width', '20%');

        }
    };



    $.fn.DisplayRubric = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };

    window.DisplayRubric = {
        init: function (options) {
            api.init.apply(this, arguments);
        }
    };
} (jQuery))