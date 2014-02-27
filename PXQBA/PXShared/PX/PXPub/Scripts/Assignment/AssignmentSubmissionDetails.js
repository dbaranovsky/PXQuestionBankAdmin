// AssignmentSubmissionDetails
//
// This plugin is responsible for the client-side behaviors of the assignment submission details in AssignmentSubmissionDetails view

(function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: 'AssignmentSubmissionDetails',
        dataKey: 'AssignmentSubmissionDetails',
        bindSuffix: '.AssignmentSubmissionDetails',
        dataAttrPrefix: 'data-asd-',
        constant: {

        },
        cache: {

        },
        defaults: {

        },
        sel: {
            assignmentId: '#submissionDetailsAssignmentId',
            controlUnsubmitSelected: '.submissionDetailsControl_unsubmit_selected',
            checkBox: '.submissionDetailsCheckBox'

        },
        // private functions
        fn: {
            onUnsubmitSelected: function () {
                PxPage.Loading();
                var selectedSubmission = $(_static.sel.checkBox + ' :checked');
                if (!selectedSubmission || selectedSubmission.length == 0)
                    return false;
                var enrollmentIds = [];
                for (var i = 0; i < selectedSubmission.length; i++) {
                    enrollmentIds.push($(selectedSubmission[i]).val());
                }
                var assignmentId = $(_static.sel.assignmentId).val();

                $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.Assignment_UnsubmitSumissions,
                    data: { assignmentId: assignmentId, enrollmentIds: enrollmentIds },

                    traditional: true,
                    success: function (data, textStatus, jqXHR) {
                        
                        PxPage.Loaded();

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('error ' + textStatus + ': ' + errorThrown);
                        PxPage.Loaded();
                    }
                });
            },

            initControl: function () {
                var unsubmitSelected = $(_static.sel.controlUnsubmitSelected);
                if (unsubmitSelected.length > 0) {
                    unsubmitSelected.unbind('click');
                    unsubmitSelected.click(_static.fn.onUnsubmitSelected);
                }

            }

        }
    },
    // The public interface for interacting with this plugin.
    api = {
        init: function (options) {
            _static.fn.initControl();

        }
    };



    $.fn.AssignmentSubmissionDetails = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };

    window.AssignmentSubmissionDetails = {
        init: function (options) {
            api.init.apply(this, arguments);
        }
    };
} (jQuery))