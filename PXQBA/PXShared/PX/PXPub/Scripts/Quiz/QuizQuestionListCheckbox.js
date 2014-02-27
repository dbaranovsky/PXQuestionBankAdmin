//jQuery plugin for Quiz question list checkbox functionality

(function ($) {
    var _static = {
        pluginName: "questionlistcheckbox",
        dataKey: "questionlistcheckbox",
        bindSuffix: ".questionlistcheckbox",
        dataAttrPrefix: "data-qlc-",
        //selectors for commonly accessed elements
        sel: {

            //selection checkbox functionality
            checkAllBox: ".selected-questions .questions-actions input[name=selectall]",
            checkAvailableAllOption: ".available-questions #select-gearbox .select-all",
            checkAvailableNoneOption: ".available-questions #select-gearbox .select-none",
            checkCurrentAllOption: ".quiz-overview #select-gearbox-current .select-all",
            checkCurrentNoneOption: ".quiz-overview #select-gearbox-current .select-none",

            checkSelectCurrentQuestion: ".selected-questions input[type=checkbox]",
            checkSelectAvailableQuestion: ".available-questions input[type=checkbox]",
            checkSelectOverviewQuestion: ".quiz-overview input[type=checkbox]",

        },
        //private functions
        fn: {
            SetMenuStatusOnCurrentQuestion: function (target) {
                if ($(target).prop('checked')) {
                    PxQuiz.SetAllChecks(target, true);
                    $('.question-bank-header-right .questions-actions .remove-btn-wrapper').show();
                    PxQuiz.cfind(target, "ul.questions:visible li.question").addClass("selected-question-in-quiz-editor");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .preview-current-question").addClass("displayquestionmenu");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .edit-current-question").addClass("displayquestionmenu");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .move-current-question").addClass("displayquestionmenu");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .delete-current-question").addClass("displayquestionmenu");
                }
                else {
                    PxQuiz.SetAllChecks(target, false);
                    $('.question-bank-header-right .questions-actions .remove-btn-wrapper').hide();
                    PxQuiz.cfind(target, "ul.questions:visible li.question").removeClass("selected-question-in-quiz-editor");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .preview-current-question").removeClass("displayquestionmenu");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .edit-current-question").removeClass("displayquestionmenu");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .move-current-question").removeClass("displayquestionmenu");
                    PxQuiz.cfind(target, "ul.questions:visible li.question .delete-current-question").removeClass("displayquestionmenu");

                }
            },
            SetMenuStatusOnAvailableQuestion: function (target) {
                PxQuiz.cfind(target, "ul.questions:visible li.question .preview-available-question").addClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .add-to-pool-available-question").addClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question").addClass("selected-question-in-quiz-editor");

                $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper, .available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').show();
            },
            SetMenuStatusOffAvailableQuestion: function (target) {
                PxQuiz.cfind(target, "ul.questions:visible li.question .preview-available-question").removeClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .add-to-pool-available-question").removeClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question").removeClass("selected-question-in-quiz-editor");

                $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper, .available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').hide();
            },
            SetMenuStatusOnCurrentOverviewQuestion: function (target) {
                $('.quiz-overview .various-actions .remove, .quiz-overview .various-actions .edit-settings').show();
                PxQuiz.cfind(target, "ul.questions:visible li.question").addClass("selected-question-in-quiz-editor");
                PxQuiz.cfind(target, "ul.questions:visible li.question .preview-current-question").addClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .edit-current-question").addClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .move-current-question").addClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .delete-current-question").addClass("displayquestionmenu");
            },
            SetMenuStatusOffCurrentOverviewQuestion: function (target) {
                $('.quiz-overview .various-actions .remove, .quiz-overview .various-actions .edit-settings').hide();
                PxQuiz.cfind(target, "ul.questions:visible li.question").removeClass("selected-question-in-quiz-editor");
                PxQuiz.cfind(target, "ul.questions:visible li.question .preview-current-question").removeClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .edit-current-question").removeClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .move-current-question").removeClass("displayquestionmenu");
                PxQuiz.cfind(target, "ul.questions:visible li.question .delete-current-question").removeClass("displayquestionmenu");
            },
            SetAvailableQuestionMenuStatus: function (event) {
                if (!$(event.target).closest(".question").hasClass("selected-question-in-quiz-editor")) {
                    $(event.target).closest(".question").find(".preview-available-question, .add-to-pool-available-question").addClass("displayquestionmenu");

                    $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper, .available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').show();
                    $(event.target).closest(".question").addClass('selected-question-in-quiz-editor');
                }
                else {
                    $(event.target).closest(".question").find(".preview-available-question, .add-to-pool-available-question").removeClass("displayquestionmenu");
                    $(event.target).closest(".question").removeClass('selected-question-in-quiz-editor');

                    var target = event.target;
                    var checked = PxQuiz.cfind(target, 'input[name=selected]:checked');
                    if (checked != null && checked.length > 0) {
                        $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper, .available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').show();
                    }
                    else {
                        $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper, .available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').hide();
                    }
                }
            },
            SetCurrentQuestionMenuStatus: function (event) {
                $('#fne-window').removeClass('require-confirm');
                var target = event.target;
                var checked = PxQuiz.cfind(target, 'input[name=selected]:checked');
                if (checked != null && checked.length > 0) {
                    $('.quiz-overview .various-actions .remove, .quiz-overview .various-actions .edit-settings').show();
                }
                else {
                    $('.quiz-overview .various-actions .remove, .quiz-overview .various-actions .edit-settings').hide();
                }
                
                if (!$(event.target).closest(".question").hasClass("selected-question-in-quiz-editor")) {

                    $(event.target).closest(".question").addClass("selected-question-in-quiz-editor");
                    $(event.target).closest(".question").find(".preview-current-question, .edit-current-question, .move-current-question, .delete-current-question").addClass("displayquestionmenu");
                }
                else if ($(event.target).closest(".question").hasClass("selected-question-in-quiz-editor") &&
                         $(event.target).closest(".question").find(".question-text").hasClass("collapsed")) {
                    $(event.target).closest(".question").removeClass("selected-question-in-quiz-editor");
                    $(event.target).closest(".question").find(".preview-current-question, .edit-current-question, .move-current-question, .delete-current-question").removeClass("displayquestionmenu");
                }
                else if ($(event.target).closest(".question").hasClass("selected-question-in-quiz-editor") &&
                         !$(event.target).closest(".question").find(".question-text").hasClass("collapsed")) {
                    $(event.target).closest(".question").removeClass("selected-question-in-quiz-editor");
                }
            },
        }//end private functions

    };

    var api = {
        init: function (options) {
            //Checkbox functionality
            $(document).off('change', _static.sel.checkAllBox).on('change', _static.sel.checkAllBox, function (event) { _static.fn.SetMenuStatusOnCurrentQuestion(event.target); });
            $(document).off('click', _static.sel.checkAvailableAllOption).on('click', _static.sel.checkAvailableAllOption, function (event) { _static.fn.SetMenuStatusOnAvailableQuestion(event.target); });
            $(document).off('click', _static.sel.checkAvailableNoneOption).on('click', _static.sel.checkAvailableNoneOption, function (event) { _static.fn.SetMenuStatusOffAvailableQuestion(event.target); });
            $(document).off('click', _static.sel.checkCurrentAllOption).on('click', _static.sel.checkCurrentAllOption, function (event) { _static.fn.SetMenuStatusOnCurrentOverviewQuestion(event.target); });
            $(document).off('click', _static.sel.checkCurrentNoneOption).on('click', _static.sel.checkCurrentNoneOption, function (event) { _static.fn.SetMenuStatusOffCurrentOverviewQuestion(event.target); });
            $(document).off('change', _static.sel.checkSelectAvailableQuestion).on('change', _static.sel.checkSelectAvailableQuestion, function (event) { _static.fn.SetAvailableQuestionMenuStatus(event); });
            $(document).off('click', _static.sel.checkSelectOverviewQuestion).on('click', _static.sel.checkSelectOverviewQuestion, function (event) { _static.fn.SetCurrentQuestionMenuStatus(event); });
        },
    };
    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.questionlistcheckbox = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

}(jQuery));