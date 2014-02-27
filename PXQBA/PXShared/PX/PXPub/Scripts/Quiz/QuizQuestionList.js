//jQuery plugin for Quiz Preview functionality

(function ($) {
    var _static = {
        pluginName: "questionlist",
        dataKey: "questionlist",
        bindSuffix: "questionlist",
        dataAttrPrefix: "data-ql-",
        //static variables:
        isInitialized: false,
        //plugin defaults
        defaults: {
            MouseDown: false

        },
        //css settings
        css: {

        },
        //selectors for commonly accessed elements
        sel: {

            //CONTAINERS for different question list modes:
            availableQuestions: ".available-questions", //list of question available to be added (quiz editor)
            selectedQuestions: ".selected-questions", //list of questions in current quiz (quiz editor)
            quizOverview: ".quiz-overview", //list of questions in quiz (quiz preview)

            //CONTAINER ELEMENTS
            questionlistContainer: ".questionlist",
            questionlist: ".question-list ul.questions", //actual UL of the questions
            questionDisplay: ".question-display", //container when questions are being previewed

            //BUTTONS
            btnEditQuestion: ".selected-questions .edit-current-question",
            btnOverviewEdit: ".quiz-overview .edit-current-question",
            btnDeleteQuestion: ".delete-current-question",
            btnDeleteSelectedQuestions: ".remove-question, .various-actions .remove",
            btnEditSelectedQuestionSettings: ".various-actions .edit-settings",
            btnExpandAll: ".expand-all-available-question, .various-actions .expandAll",
            btnCollapseAll: ".collapse-all-available-question, .various-actions .collapseAll",
            btnPrintQuiz: ".printQuiz",

            //hover actions
            btnExpandAvailQuestion: ".expand-available-question",
            btnExpandQuestion: ".expand-current-question",
            //question pool actions
            btnShowQuestionPool: ".show-current-questions",
            btnHideQuestionPool: ".hide-current-questions",
            //question preview
            btnShowPreview: ".preview-current-question",
            btnShowPreviewAvail: ".preview-available-question",

            //settings
            btnChangePoints: "ul.questions li.question .total-points .point-label",

            //selectors for selecting a question
            selectQuestion: ".table-layout .question a.expand-link, .available-questions .question:not(.in-used-question) a.expand-link, .selected-questions .question a.expand-link",
            selectQuestionCheckbox: ".question input[name=selected]",

            addToNewAssessment: '.add-to-new-assessment',
            addToNewAssessmentDlg: '.add-to-new-assessment-dialog'
        },
        //hooks for the FNE window
        fneHooks: {
            FneInit: function () {
                PxQuiz.UpdateAddedQuestions();
                _static.fn.NumberQuestions($(".selected-questions .question-display"));
                _static.fn.SetupSorting($(".selected-questions .question-display"));

            }
        },
        //private functions
        fn:
        {
            SetupSorting: function (target) {
                var questionLists = $(target).find('.questions');
                $.each(questionLists, function () {
                    if ($(this).hasClass('ui-sortable')) {
                        $(this).sortable("refresh");
                    } else {
                        var singleQuestionText = "Drag this question to where you want to position it in the quiz";
                        var helper = $("<div class='quiz-dragging-helper'><span>" + singleQuestionText + "</span></div>");
                        $(this).sortable({
                            cancel: ".selected-question-info, .question-table-header",
                            handle: '.drag-indicator',
                            revert: true,
                            scroll: false,
                            placeholder: "ui-state-highlight",
                            distance: 10,
                            connectWith: ".questions",
                            appendTo: document.body,
                            zIndex: 3000,
                            opacity: 0.9,
                            cursor: "pointer",
                            cursorAt: { left: -15, top: -15 },
                            helper: function (event, ui) {
                                // Make sure the question, quiz being dragged becomes selected
                                return helper;
                            },
                            remove: function (event, ui) {
                                //_static.fn.DoSortUpdate(event, ui);
                            },
                            stop: function (event, ui) {
                                $('.qtip-defaults').hide();
                            },
                            start: function (event, ui) {
                                $('.qtip-defaults').hide();
                                $('.ui-state-highlight').append('<div class="drag-label">   Move Here</div>');
                                $('.ui-state-highlight').append('<div class="icon"></div>');
                            }
                        }).disableSelection();

                        $(this).sortable().unbind("sortupdate").bind("sortupdate", function (event, ui) {
                            //dont trigger update when the item is origionally from elsewhere (recieve)
                            //questions pools will be save automatically
                            if (ui.sender != null && ui.sender[0] !== ui.item.parent()[0]) {
                                return true;
                            }
                            _static.fn.DoSortUpdate(event, ui);
                        });
                    }
                });
                
               
            },
            CloseQuestionCard: function () {
                if ($('.questionCard')) {
                    $('.questionCard .ui-dialog-titlebar-close').trigger('click');
                }
            },
            RefreshSorting: function(target) {
                var questionLists = $(target).find('.questions.ui-sortable');
                $.each(questionLists, function() {
                    $(this).sortable("refresh");
                });
            },
            ClearAllQuestions: function (target) {
                PxQuiz.SetAllChecks(target, false);
                return false;
            },
            
            DoSortUpdate: function(event, ui) {
                $('.qtip-defaults').hide();
                PxQuiz.updatedTimes++;
                if (PxQuiz.updatedTimes == 1) {
                    if ($(ui.item).find('input.questions-points:visible').length) {
                        $(ui.item).find('input.questions-points').focusout();
                    }
                    var currentTarget = $(ui.item).closest('ul.questions');
                    _static.fn.UpdateQuestionPoint(event.target, currentTarget, $(ui.item));
                    _static.fn.ProcessMetaDataUpdate(null, currentTarget,$(ui.item),function () {
                        PxPage.Loading($(currentTarget));
                        if (!(ui.item.hasClass('available') || (event.target == currentTarget[0]))) {
                            if ($.contains(currentTarget[0], event.target)) {
                                //means we have dragged something from the question bank to the quiz.
                                //so we should load question bank first and then load quiz.
                                currentTarget = event.target;
                            }
                        }
                        PxQuiz.updatedTimes = 0;
                        _static.fn.NumberQuestions(currentTarget);
                        _static.fn.SaveQuestionList(currentTarget);
                        PxQuiz.UpdateAddedQuestions();
                        PxQuiz.UpdateSelectedQuestionsMenu('.selected-questions .question-list'); //IE7 fix.
                        if ($('#fne-content').is(':visible')) {
                            _static.fn.ClearAllQuestions($('.available-questions'));
                        }
                        $(PxPage.switchboard).one("questionlistupdated", function () {
                            $(ui.item).addClass("fade-effect");
                            PxPage.Fade();
                        });
                    });
                }
            },
            CollapseQuestion: function () {
                return "collapsed";
            },
            ExpandQuestion: function (event) {
                var question = $(event.target).closest(".question");
                $('.qtip-defaults').hide();
                _static.fn.ExpandQuestionAction(question);
                return false;
            },
            ExpandQuestionAction: function (question) {
                if (question == null || question.length == 0) {
                    return;
                } else {
                    question = $(question);
                }
                var description = $(question).find(".description:first");
                description.find(".question-text").toggleClass("collapsed");
                if (question.find("input[name=selected]").prop('checked')) {
                    question.removeClass("selected-question-in-quiz-editor");
                }

                var questionElementSelector = '.preview-available-question, .add-to-pool-available-question' +
                        '.preview-current-question, .edit-current-question, .move-current-question, .delete-current-question';

                if (description.find(".question-text").hasClass("collapsed")) {
                    description.find(".expand-available-question, .expand-current-question").text("Expand");

                    question.find(questionElementSelector).removeClass("displayquestionmenu");

                }
                else {
                    description.find(".expand-available-question, .expand-current-question").text("Collapse");

                    question.find(questionElementSelector).addClass("displayquestionmenu");

                }

                var quType = question.find(".question-type").val();
                if (quType == "CUSTOM") {
                    var quCustomUrl = question.find(".question-custom-url").val();
                    var quId = question.find(".question-id").val();
                    var quEntityId = question.find(".question-entity-id").val();
                    _static.fn.IframeQuestionPreview(quId, quCustomUrl, "", quEntityId);
                }

            },
            BindFneHooks: function () {
                PxPage.FneInitHooks['quiz-editor'] = _static.fneHooks.FneInit;
            },
            CommonPostProcessing: function () {
                PxQuiz.FneResize();
                PxQuiz.UpdateAddedQuestions();
                var currentTarget = $(".selected-questions .question-display");
                _static.fn.NumberQuestions(currentTarget);
                _static.fn.RefreshSorting(currentTarget);
                //PxQuiz.BindQtipForPreviewQuestion();
            },
            ViewQuestions: function (event, callback) {
                var eventTarget = $(event.target);
                //If the parent div is already loading, don't create another spinning icon.
                var isLoading = $('.selected-questions > .blockUI').length !== 0;
                if(!isLoading)
                    PxPage.Loading(eventTarget.closest('li.question.bank'));
                eventTarget.closest('li.question.bank').addClass('bank-expanded');
                eventTarget.closest(".description").find(".question-text:first").removeClass(_static.fn.CollapseQuestion);
                var quType = $(event.target).closest(".question").find(".question-type").val();
                var quCustomUrl = $(event.target).closest(".question").find(".question-custom-url").val();
                var quId = $(event.target).closest(".question").find(".question-id").val();
                var quEntityId = $(event.target).closest(".question").find(".question-entity-id").val();
                if (quType == "CUSTOM") {
                    _static.fn.IFrameQuestionPreview(quId, quCustomUrl, "", quEntityId);
                }

                var question = $(event.target).closest('li.question');
                var quizId = question.find('input.question-id').val();
                var questionPool = $(event.target).closest('li.question.bank').find('.question-pool');

                var sortableTarget = $(event.target).closest('li.question.bank');

                var url = PxPage.Routes.get_mainpoollist;
                if ($('#fne-content').is(':visible')) {
                    url = PxPage.Routes.get_poolquestionlist;
                }

                var isQuestionOverview = false;
                var mainQuizId = "";
                if ($('.quiz-overview .question-display').is(':visible')) {
                    isQuestionOverview = true;
                    mainQuizId = $("#content-item-id").text();
                }

                questionPool.load(
                    url,
                    { quizId: quizId, isQuestionOverview: isQuestionOverview, mainQuizId: mainQuizId },
                    function () {
                        _static.fn.NumberQuestions(questionPool);
                        _static.fn.SetupSorting(sortableTarget);
                        _static.fn.OpenEditQuestionSettingsDialog();
                        _static.fn.OpenEditQuestionSettingsDialog2();
                        _static.fn.ModifyInUsedQuestions($('div.select-menu'));
                        PxPage.Loaded($(event.target).closest('li.question.bank'));
                        $(PxPage.switchboard).trigger("questionpool-loaded", [{ quizId: quizId, isQuestionOverview: isQuestionOverview, mainQuizId: mainQuizId}]);
                    }
                );
                $(event.target).closest('li.question.bank').find('.show-current-questions').hide();
                $(event.target).closest('li.question.bank').find('.hide-current-questions').show();
                $('.question-list').questionListGearbox('updateQuestionsMenu', event);
                
                if (event != null) {
                    event.preventDefault();
                }
            },
            HideQuestions: function (event) {
                $(event.target).closest('li.question.bank').removeClass('bank-expanded');
                $(event.target).closest(".description").find(".question-text").addClass(_static.fn.CollapseQuestion);
                $(event.target).closest('li.question.bank').find('.question-pool').empty();
                $(event.target).closest('li.question.bank').find('.show-current-questions').show();
                $(event.target).closest('li.question.bank').find('.hide-current-questions').hide();
                $('.question-list').questionListGearbox('updateQuestionsMenu', event);
                if (event) {
                    event.preventDefault();
                }
            },

            OpenQuestionDialog: function (event) {
                var target = event.target;
                var question = $(target).closest('li.question');

                question.addClass('question-being-previewed');

                var questionIsInQuiz = $(question).hasClass("in-used-question") || $(question).parents('.selected-questions').length > 0 || $(question).parents('.quiz-overview').length > 0; //$(question).hasClass("available").length == 0; //if question is available, that means its not being opened in the context of a quiz
                var fromQuizEditor = $(question).closest(".quiz-editor").length > 0; //question opened from quiz editor
                var quType = $(event.target).closest(".question").find(".question-type").val();

                var buttons = {};
                if (quType == "CUSTOM") {
                    buttons["RegenerateVariables"] = {
                        text: "Regenerate Variables",
                        click: function () {
                            _static.fn.IframeQuestionPreview(quId, quCustomUrl, ".question-dialog-text", quEntityId);
                        },
                        "questionId": $(question).find(".question-id").val()
                    };
                }
                if (fromQuizEditor) {
                    //BUTTON ADD
                    buttons["Add"] = {
                        html: questionIsInQuiz ? "&#10004; Added to this assessment" : "Add to this assessment &#x25BC;",
                        click: function () {
                            if (questionIsInQuiz) {
                                _static.fn.RemoveCurrentQuestion(event);
                                $(this).dialog('close').dialog("destroy");
                            } else {
                                //open add pool menu

                            }

                        },
                        "class": questionIsInQuiz ? "remove-question-hover" : "add-to-pool-preview-quiestion-btn-wrapper"
                    };
                    $(document).off('mouseenter', ".remove-question-hover").on('mouseenter', ".remove-question-hover", function () {
                        $(this).find(".ui-button-text").css('background-color', 'red');
                        $(this).find(".ui-button-text").html("X Remove from assessment");
                    });
                    $(document).off('mouseleave', ".remove-question-hover").on('mouseleave', ".remove-question-hover", function () {
                        $(this).find(".ui-button-text").html(questionIsInQuiz ? "&#10004; Added to this assessment" : "Add to this assessment &#x25BC;");
                    });

                }

                if (questionIsInQuiz) {
                    buttons["Edit"] = {
                        text: "Edit",
                        click: function () {
                            $(this).dialog("destroy");
                            question.find('.edit-current-question').click();
                        }
                    };
                }

                buttons["InUse"] = {
                    html: fromQuizEditor ?
                        ($(question).hasClass("reused") ? "&#10004 Used Elsewhere &#x25BC;" : "Use elsewhere &#x25BC;") :
                        ($(question).hasClass("reused") || questionIsInQuiz ? "&#10004 In Use &#x25BC;" : "Use &#x25BC;"),
                    click: function () {
                        _static.fn.UsedElseWhere($(question).find('#question-tool-tip').html());
                    },
                    "class": "buttonuse usedeverywhere" + $(question).hasClass("reused") ? "buttonuse reused" : "",
                    "questionId": $(question).find(".question-id").val()
                };

                $('.qtip-defaults').hide();

                //open the editor if it is on the LHS else load the dialog.

                var dialog = $(question).closest('.question-list').find('.question-dialog-text').first().clone().dialog({
                    width: 900,
                    height: 'auto',
                    minWidth: 900,
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    zIndex: 1005,
                    buttons: buttons,
                    position: { my: "bottom", at: "center", of: window },

                    open: function (event, ui) {
                        $('.ui-dialog-buttonpane').addClass('previewbtns');
                        
                        $('.previewbtns').append('<div class="add-menu"><div class="gearbox"></div></div>');


                        $('.question-list').questionListGearbox('initQuestionGearbox');

                        $('.previewbtns .gearbox').hide();
                        
                        $('.add-to-pool-preview-quiestion-btn-wrapper').unbind('click').bind('click', function () {
                            $('.previewbtns .gearbox').click();
                            $('.previewbtns #add-gearbox').offset({
                                top: $('.add-to-pool-preview-quiestion-btn-wrapper').offset().top + 47,
                                left: $('.add-to-pool-preview-quiestion-btn-wrapper').offset().left
                            });
                        });

                        $(this).parent().addClass('questionCard');
                    },
                    beforeClose: function () {
                        question.removeClass('question-being-previewed');
                    },
                    close: function () {


                        if ($("#fne-window").is(":visible")) { 
                            $('html').addClass('fne-scrollbars');  //return back to normal FNE view
                        }
                        $(this).remove();
                    }
                });                
            
                var html = '';

                var courseCardTemplate = $("#question-card-template").text();
                if (courseCardTemplate != null && courseCardTemplate != undefined && courseCardTemplate != "") {
                    var template = Handlebars.compile(courseCardTemplate);
                    var metadata = $.trim($(event.target).closest(".description").find(".question-text .question-metadata").html());
                    if (metadata != null && metadata != "null" && metadata.length > 0) {
                        //PX-4949 adding support for Learning objectives and suggested Use
                       try {
                            var objmeta = jQuery.parseJSON(metadata);
                            objmeta.suggesteduse = jQuery.parseJSON($(event.target).closest(".description").find(".metadata-suggestedUse").html());
                            objmeta.learningobjectives = jQuery.parseJSON($(event.target).closest(".description").find(".metadata-learning-objectives").html());

                            html = template(objmeta).replace(/\|/g, ',');
                            html = html.replace(/\[\[/g, '<a class="searchTerm">').replace(/\]\]/g, '</a>');
                        } catch (ex) {
                            
                        }

                        var searchTerm = '';
                        $(event.target).closest(".description").find(".question-text .questioncard-container .content").each(function () {
                            var meta = $(this).parent('div').attr('class');

                            $.each(PxPage.QuestionFilter.FilterMetadata, function () {
                                if (this.Name == meta) {
                                    searchTerm = this.Searchterm;
                                }
                            });

                            if (searchTerm != undefined && searchTerm.length > 0) {
                                var text = $(this).text();
                                $(this).html("<a href='javascript:' class='searchTerm' onclick='$.fn.questionlist(\"searchQuestionMetaData\", \"" + searchTerm + text + "\");'>" + text + "</a>");
                            }
                        });

                        $(".ui-dialog-titlebar.ui-widget-header").addClass("questioncardTitle");
                    }
                }

                $(".ui-dialog").find('.question-dialog-text').html($(event.target).closest(".description").find(".question-text").html());
                $(".ui-dialog").find(".questioncard-container").html(html).show();

                /*var*/quType = $(event.target).closest(".question").find(".question-type").val();
                var quCustomUrl = $(event.target).closest(".question").find(".question-custom-url").val();
                var quEntityId = $(event.target).closest(".question").find(".question-entity-id").val();
                var quId = $(event.target).closest(".question").find(".question-id").val();
                var quizId = $($(".content-item-id")[0]).text(); //FIX of PLATX-5826.

                /*start transformation of question card layout*/
                var preview = $(".ui-dialog.questionCard");
                //preview.css('top', $(window).height() / 4);

                var btnPane = preview.find('.previewbtns');

                preview.find('.question-container').after("<div class='question-clear' style='clear: both' />");
                preview.find('.question-clear').after(btnPane);
                preview.find('.ui-dialog-buttonset').css('float', 'left');
                preview.find('.previewbtns').css('background-color', '#F8F9F9');

                preview.find('.questioncardrow').each(function () {
                    if ($.trim($(this).text()) == '') {
                        $(this).remove();
                    }
                });

                $('.fne-scrollbars').removeClass('fne-scrollbars');//when inside the fne-window, enable scrolling so that we can see the content
                /*end transformation of question card layout*/

                if (quType == "CUSTOM") {
                    
                    // _static.fn.InlineQuestionPreview(quId, quCustomUrl, ".question-dialog-text", quEntityId);
                    _static.fn.IframeQuestionPreview(quId, quCustomUrl, ".question-dialog-text", quEntityId);

                    //$('.question-dialog-text #custom_preview_' + quId).height(500);
                    var wHeight = $(window).height();
                    var h = wHeight * 0.6;
                    h = h < 500 ? 500 : h;
                    var t = wHeight * 0.1;

                    $('.question-dialog-text #custom_preview_' + quId).css("height", h + "px");
                    preview.css("top", t + "px");
                }

                return false;
            },
            
            EditCurrentQuestion: function (event) {
                //hide the related-content for pool if exists.
                PxPage.Loading('fne-window');
                var editorPoolRelatedContent = $("#related-content-editor-pool");

                if ($(event.target).closest('li.question').hasClass('bank')) {
                    $('.question-editor').questioneditor('editQuestionPool', 'bank', event);
                    if (editorPoolRelatedContent.length > 0) {
                        var data = {
                            toBeRemovedFromDom: false
                        };
                        editorPoolRelatedContent.LearningCurveMoreResources("hideRelatedContentForPool", data);
                    }
                }
                else {
                    $('.question-list').questioneditor('editQuestion', event);
                    if (editorPoolRelatedContent.length > 0) {
                        var data = {
                            toBeRemovedFromDom: true
                        };
                        editorPoolRelatedContent.LearningCurveMoreResources("hideRelatedContentForPool", data);
                    }

                }
                PxPage.Loaded('fne-window');
                event.preventDefault();
            },
            EditOverviewQuestion: function (event) {
                //TODO: This is a really dumb way of doing this, should have a route directly to the question editor
                PxPage.Loading("#contentwrapper");
                PxPage.FneLoadedHooks["quiz-editor"] = function () {
                    PxPage.FneLoadedHooks["quiz-editor"] = null;
                    //if question is inside a question pool, open pool first
                    var questionId = $(event.target).closest('li.question').attr('id');
                    var openQuestionEditor = function (e, data) {
                        var selectedQuestion = null;
                        
                        if (data && data.quizId && data.quizId == questionId) {
                            //edit pool
                            selectedQuestion = $('.selected-questions li#' + data.quizId + ' .edit-current-question').first();
                        } else {
                            //edit question
                            selectedQuestion = $(".selected-questions").find("li#" + questionId).find(".edit-current-question");
                        }
                        if (selectedQuestion.length > 0)
                            selectedQuestion.click();
                        PxPage.Loaded("#contentwrapper");

                    };
                    if ($(event.target).closest('li.question.bank').length > 0) {
                        $(".selected-questions").find("li#" + $(event.target).closest('li.question.bank').attr('id')).find(".show-current-questions").click();
                        $(PxPage.switchboard).one("questionpool-loaded", openQuestionEditor);
                    } else {
                        openQuestionEditor();
                    }

                };

            },
            SetUsedQuestionMenuStatus: function (event) {
                var target = event.target;
                var checked = $('.selected-questions input[name=selected]:checked');

                if (checked != null && checked.length > 0) {
                    $('.question-bank-header-right .questions-actions .remove-btn-wrapper').show();
                }
                else {
                    $('.question-bank-header-right .questions-actions .remove-btn-wrapper').hide();
                }

                var questionElementSelector = '.preview-current-question, .edit-current-question, .move-current-question, .delete-current-question';
                if (!$(target).closest(".question").hasClass("active")) {
                    $(target).closest(".question").addClass("selected-question-in-quiz-editor");
                    $(target).closest(".question").find(questionElementSelector).addClass("displayquestionmenu");
                }
                else {
                    $(event.target).closest(".question").removeClass("selected-question-in-quiz-editor");
                    $(event.target).closest(".question").find(questionElementSelector).removeClass("displayquestionmenu");
                }
            },
            RemoveCurrentQuestion: function (event) {
                var dialog = $(event.target).closest('.question-list').find('.question-dialog-text').first();

                dialog.clone().dialog({
                    width: 400,
                    height: 130,
                    modal: true,
                    resizable: false,
                    draggable: false,
                    closeOnEscape: true,
                    dialogClass: 'no-title-dialog',
                    buttons: {
                        "Remove": function () {
                            var target = $(event.target).closest('li.question');
                            // changing the text of the bankcount
                            var parentContainer = $(target).closest('.question-list');
                            var questionBank = $(parentContainer).closest('li.question');
                            var bankcount = $(questionBank).find('.bank-count');
                            bankcount.text(bankcount.text() - 1);

                            if ($('.question-display .question-list').length > 0) {
                                target = $('.question-display .question-list').find('li[id=' + target.attr('id') + ']');
                            }

                            _static.fn.RemoveSelected(target);
                            $('.question-list').questionListGearbox('updateQuestionsMenu', event);
                            _static.fn.ModifyInUsedQuestions($('div.select-menu'));
                            PxQuiz.UpdateSelectedQuestionsMenu('.selected-questions .question-list'); //IE7 fix.
                            _static.fn.SetUsedQuestionMenuStatus(event);
                            if (PxQuiz.QuestionMode != PxQuiz.EnumQuestionMode.browser) {
                                if ($(target).hasClass('selected')) {
                                    $(".question-editor").questioneditor("switchMode", [PxQuiz.EnumQuestionMode.browser]);
                                }
                            }
                            $('.ui-dialog').remove();
                        },
                        "Cancel": function () {
                            $('.ui-dialog').remove();
                        }
                    }
                });
                $('.ui-dialog').find('.question-dialog-text').html("<div class='removePrompt'>Do you want to remove the selected question?</div>");
                if (event != null) {
                    event.preventDefault();
                }
            },
            RemoveSelectedQuestions: function (event) {
                var checked = PxQuiz.cfind(event.target, 'input[name=selected]:checked');
                if (checked.length > 0) {
                    $('.question-dialog-text').first().clone().dialog({
                        width: 400,
                        height: 130,
                        modal: true,
                        resizable: false,
                        draggable: false,
                        closeOnEscape: true,
                        dialogClass: 'no-title-dialog',
                        buttons: {
                            "Remove": function () {
                                var target = checked.closest('li.question');
                                var parentContainer = $(target).closest('.question-list');
                                _static.fn.UpdateBankCounts(target);

                                _static.fn.RemoveSelected(target);
                                $('.question-list').questionListGearbox('updateQuestionsMenu', event);
                                _static.fn.ModifyInUsedQuestions($('div.select-menu'));
                                PxQuiz.UpdateSelectedQuestionsMenu('.selected-questions .question-list'); //IE7 fix.
                                _static.fn.SetUsedQuestionMenuStatus(event);
                                //if all the questions are deleted we should hide the parentContainer.                    
                                $(parentContainer).ajaxStop(function () {
                                    _static.fn.CheckForNoQuestionsLeft(parentContainer);
                                });
                                $(event.target).closest(".quiz-overview").find('.various-actions .remove').hide();
                                $(event.target).closest(".quiz-overview").find('.various-actions .edit-settings').hide();
                                $('.selected-questions .questions-actions input[name=selectall]').prop('checked', false);
                                $(this).dialog('destroy').remove();
                            },
                            "Cancel": function () {
                                $(this).dialog('destroy').remove();
                            }
                        }
                    });
                    $(".ui-dialog").find('.question-dialog-text').html("<div class='removePrompt'>Do you want to remove the selected questions?</div>");
                }
                else {
                    PxPage.Toasts.Warning("Please select questions to be removed.");
                }
                if (event != null) {
                    event.preventDefault();
                }
            },
            ExpandAllQuestions: function (event) {

                var questions = $("li.question .question-text.collapsed").parents(".available-questions ul.questions li.question, .quiz-overview ul.questions li.question");
                    
                $(questions).each(function() {
                    _static.fn.ExpandQuestionAction(this);
                });

                //Expand all question pools
                var pools = PxQuiz.cfind(event.target, ".questions li.question .show-current-questions").click();
                var poolsToLoad = pools.length;
                if (poolsToLoad > 0) {
                    //wait until all questions pools have loaded
                    $(PxPage.switchboard).bind("questionpool-loaded", function () {
                        poolsToLoad--;
                        if (poolsToLoad <= 0) {
                            //expand all questions inside pools once all pools have loaded
                            $(PxPage.switchboard).unbind("questionpool-loaded");
                            questions = $("li.question .question-text.collapsed").parents(".available-questions ul.questions li.question, .quiz-overview ul.questions li.question");
                            $(questions).each(function() {
                                _static.fn.ExpandQuestionAction(this);
                            });
                        }
                    });
                }
            
            },
            CollapseAllQuestions: function (event) {
                //collapse all pools
                PxQuiz.cfind(event.target, ".questions li.question .hide-current-questions").click();
                //get all expanded questions
                var questions = $("li.question .question-text:not(.collapsed)").parents(".available-questions ul.questions li.question, .quiz-overview ul.questions li.question");

                $(questions).each(function () {
                    _static.fn.ExpandQuestionAction(this);
                });
            },
            PrintQuiz: function(event) {
                var printUrl = $(this).data("href");
                if (printUrl && printUrl.length) {
                        PxModal.CreateConfirmDialog({
                            "title": "Print Quiz",
                            "content": "A printable version of the assessment will open in a new window. <br /> <br />" +
                                "It will include a \"student\" version of the questions, followed by an \"instructor\" version with correct answers marked.  <br /> <br />" +
                                "You can use your browser's print function to print or save a PDF of either or both versions. <br \> <br \>" +
                                "<em>Consult the LaunchPad help documentation for assistance.</em>",
                            "buttons": {
                                "Print": {
                                    text: "Print",
                                    click: function() {
                                        window.open(printUrl);
                                    },
                                    show: true
                                },
                                "Cancel": {
                                    text: "Cancel",
                                    show: true
                                }
                            }
                        });
                }
            },
            EditSelectedQuestionsSettings: function (event) {
                //auto deselect the checkboxes which belongs to question-pool or itself is a question-pool.
                //PxQuiz.cfind(target, "ul.questions:visible li.question [name=selected], li.quiz-item:visible [name=selected]").attr('checked', value);
                $('.question-list .questions li.bank [name=selected]').prop('checked', false);

                var checked = PxQuiz.cfind(event.target, 'input[name=selected]:checked');

                if (checked.length > 0) {
                    var quizId = PxQuiz.cfind(event.target, $('.quiz')).attr('id');
                    var questionIds = checked.map(function () { return $(this).closest('.main-question-wrapper').find('.question-id').val(); }).toArray().join(',');
                    //var questionTypes = checked.map(function () { return $(this).closest('.main-question-wrapper').find('.question-type').val(); }).toArray().join(',');

                    if ($.trim($('.QuizType:first').text()) == "Homework") {
                        $.get(PxPage.Routes.question_settings, { quizId: quizId, questionId: questionIds }, function (data) {
                            $("#edit-settings-dialog").html(data);
                            $("#edit-settings-dialog").dialog("open");
                        });
                    }
                    else {
                        $.get(PxPage.Routes.quiz_question_settings, { quizId: quizId, questionId: questionIds }, function (data) {
                            $("#edit-settings-dialog2").html(data);
                            $("#edit-settings-dialog2").dialog("open");
                        });
                    }
                }
                else {
                    PxPage.Toasts.Warning("Please select questions to change settings for.");
                }
            },
            AvailableQuestionsUpdated: function (event, questions) {
                $('.available-questions li.question').addClass('available');
                $('.available-questions li.quiz-item').addClass('available');

                _static.fn.MakeAvailableQuestionsDraggable(questions);
                _static.fn.ShowQTip();
                _static.fn.ModifyInUsedQuestions($('div.select-menu'));
            },
            MakeAvailableQuestionsDraggable: function (questions) {
                if (questions == null) {
                    questions = $(".available-questions ul.questions > li");
                }
                $(questions).draggable({
                    handle: '.drag-indicator',
                    revert: false,
                    scroll: false,
                    cancel: ".in-used-question",
                    connectToSortable: '.selected-questions ul:not(.is-question-pool)',
                    appendTo: document.body,
                    zIndex: 3000,
                    cursor: "pointer",
                    cursorAt: { left: -15, top: -15 },
                    opacity: 0.9,
                    helper: function (event) {
                        // Make sure the question, quiz being dragged becomes selected
                        $(event.target).closest('li.question, li.quiz-item').find('input[name=selected]').prop('checked', true);
                        $('.qtip-defaults').hide();
                        // Add class to droppable area for visual indication
                        //                        $(".selected-questions .question-container").droppable({
                        //                            activeClass: "active"
                        //                        });

                        // If we've more than one selected question or quizes being dragged, then make a note of it on the dragged div
                        var selectedCount = $('.available-questions .questions > li input[name=selected]:checked').length;
                        if (selectedCount == 0) {
                            //there are possibility that quizes are being dragged rather than questions
                            selectedCount = $('.available-questions input[name=selected]:checked').length;
                        }
                        var manyQuestionText = "Drag the currently selected questions to the location in the assessment  you would like them.";
                        var singleQuestionText = "Drag the currently selected question to the location in the assessment you would like them.";
                        var message = (selectedCount > 1) ? manyQuestionText : singleQuestionText;
                        return $("<div class='quiz-dragging-helper'><span>" + message + "</span></div>");
                    }

                });

            },
            ShowQTip: function () {
                $('.question-in-use, .question-add').each(function () {
                    $(this).qtip({
                        content: { text: $(this).parents("li.question").find('#question-tool-tip').html() },
                        show: { solo: true, event: 'click' },
                        name: 'cream',
                        hide: 'unfocus',
                        position: { my: "top left", at: "bottom center", viewport: $(window) },
                        style: {
                            classes: 'qtip-shadow qtip-rounded'
                        }
                    });

                    //.click(function (event) { event.stopPropagation(); });
                    //                    
                    //                            padding: 0, 
                    //                            background: '#FAF9E8', 
                    //                            border: { color: '#D2EEA3' }, 
                    //                            color: 'black', 
                    //                            width: { min: 140, max: 250},
                });
                if ($(".metadata-label").length > 0) {
                    $(".metadata-label").qtip(
                    {
                        content: {
                            text: function (api) { return $(this).attr('data'); },
                            title: {
                                text: function (api) { return $(this).attr('name'); }
                            }
                        },
                        position: {
                            my: "bottom center",
                            at: "top center"
                        },
                        hide: {
                            fixed: true,
                            distance: 50
                        },
                        style: {
                            classes: "qtip-shadow qtip-rounded"
                        }

                    });
                }
                
            },
            // dialog that shows only units and assessments in the launchpad faux-tree
            ShowAddQuestionCardToQuiz: function (questionId) {
                var options = {
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    width: '700px',
                    height: '400px',
                    resizable: false,
                    autoOpen: false,
                    title: 'Use question in existing assessment',
                    stack: true,
                    sticky: true
                };
                var tag = $("<div id='px-dialog' class='px-dialog add-question-to-existing-quiz-dialog'></div>"); //This tag will the hold the dialog content.
                var args = {
                    questionId: questionId
                };
                tag.dialog({
                    modal: options.modal,
                    title: options.title,
                    draggable: options.draggable,
                    closeOnEscape: options.closeOnEscape,
                    width: options.width,
                    resizable: options.resizable,
                    autoOpen: options.autoOpen,
                    close: function () {
                        $(this).find(".unit-subitems-wrapper").ContentTreeWidget("destroy");
                        $(this).dialog('destroy').remove();
                    }
                }).dialog('open');
                tag.load(PxPage.Routes.show_add_question_to_existing_assessment, args, function (data, textStatus, XMLHttpRequest) {
                    tag.dialog("option", "position", "center");
                });
                //trigger add question to quiz when user selects a a quiz from the fauxtree
                $(document).off('click', '.add-question-to-existing-quiz-dialog .item-type-quiz').on("click", '.add-question-to-existing-quiz-dialog .item-type-quiz', function () {
                    _static.fn.AddQuestionToQuiz();
                });

            },
            // The actual act of adding the question to the given quiz
            AddQuestionToQuiz: function (questionId, targetId, newQuiz) {
                $.blockUI();
                if (questionId == null || questionId == undefined || questionId == "") {
                    questionId = $(".add-question-to-existing-quiz-dialog #question-id").val();
                    var targetSelector = $(".add-question-to-existing-quiz-dialog .faux-tree-node.active");
                    var targetId = targetSelector.attr("data-ud-id");
                    if (targetSelector.hasClass("item-type-pxunit")) {
                        $.unblockUI();
                        PxPage.Toasts.Error("Target item must be of an assessment type");
                        return false;
                    }
                    $(".px-dialog").hide();
                    $(".add-question-to-existing-quiz-dialog").dialog("close");
                } else {
                    if (targetId == null || targetId == undefined || targetId == "") {
                        PxPage.Toasts.Error("Question was not added to the new quiz");
                        return;
                    }
                }
                $.post(PxPage.Routes.add_question_to_quiz,
                    {
                        "quizQuestionId": questionId,
                        "targetQuizId": targetId
                    }, function (response) {
                        $.unblockUI();

                        if (response.success) {
                            PxPage.Toasts.Success(response.message);
                            $('.question-editor').questioneditor('updateQuizEditorSingleQuestion', questionId, true);
                        }
                        else {
                            PxPage.Toasts.Error(response.message);
                        }
                    });
                return;
            },
            // show elsewhere menu
            UsedElseWhere: function (usedElseWhereBody) {
                var usedElseWhereButton = $('.ui-button.buttonuse'); 
                if ($(usedElseWhereButton).parents(".ui-dialog").find(".question-used-elsewhere").length > 0 && $(usedElseWhereButton).parents(".ui-dialog").find(".question-used-elsewhere").is(":visible")) {
                    $(usedElseWhereButton).parents(".ui-dialog").find(".question-used-elsewhere").remove();
                    return;
                }

                $(usedElseWhereButton).parents(".ui-dialog").remove(".question-used-elsewhere");
                $(usedElseWhereButton).parents(".ui-dialog").append("<div class='question-used-elsewhere px-default-text'></div>");
                $(usedElseWhereButton).parents(".ui-dialog").find(".question-used-elsewhere").append($(usedElseWhereBody));
                var position = usedElseWhereButton.position();
                position.top = position.top + 67;
                $(usedElseWhereButton).parents(".ui-dialog").find(".question-used-elsewhere").css(position);
            },
            
            PointsClick: function (event) {
                //if (!$(event.target).closest("li.question").hasClass("bank")) {
                $(".point-textbox").hide();
                $(".point-label").show();

                if ($(event.target).closest(".description, .total-points").find(".point-label").show()) {
                    $(event.target).closest(".description, .total-points").find(".point-label").hide();
                    $(event.target).closest(".description, .total-points").find(".point-textbox").show();

                    var point = $(event.target).closest('.total-points').find('.questions-points-original').val();
                    $(event.target).closest('.total-points').find('.questions-points').attr('value',point);
                    $(event.target).closest(".total-points").find(".point-textbox").find('input.questions-points').focus().select();
                }
                //}
                if (event != null) {
                    event.preventDefault();
                }
                return false;
            },
            // When a question is selected, make it active and check its checkbox
            QuestionSelect: function (event) {
                var question = $(event.target).closest('li.question');
                var questionElementSelector = '.preview-current-question, .edit-current-question, .move-current-question, .delete-current-question';
                if (!$(question).hasClass("selected-question-in-quiz-editor")) {
                    $(question.find('div.select > input[type=checkbox]')[0]).prop('checked', true);
                    $(event.target).closest(".question").find(questionElementSelector).addClass("displayquestionmenu");
                    question.addClass('active');
                    question.addClass('selected-question-in-quiz-editor');
                    $(event.target).closest(".selected-questions").find('.question-bank-header-right .questions-actions .remove-btn-wrapper').show();
                    $(event.target).closest(".available-questions").find('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').show();
                    $(event.target).closest(".available-questions").find('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').show();
                    $(event.target).closest(".quiz-overview").find('.various-actions .remove').show();
                    $(event.target).closest(".quiz-overview").find('.various-actions .edit-settings').show();
                }
                else {
                    $(question.find('div.select > input[type=checkbox]')[0]).prop('checked', false);
                    $(event.target).closest(".question").find(questionElementSelector).removeClass("displayquestionmenu");
                    question.removeClass('active');
                    question.removeClass('selected-question-in-quiz-editor');

                    var target = event.target;
                    var checked = PxQuiz.cfind(target, 'input[name=selected]:checked');
                    if (checked != null && checked.length > 0) {
                        $(event.target).closest(".quiz-overview").find('.various-actions .remove, .various-actions .edit-setting').show();
                        $(event.target).closest(".selected-questions").find('.question-bank-header-right .questions-actions .remove-btn-wrapper').show();
                        $(event.target).closest(".available-questions").find('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').show();
                        $(event.target).closest(".available-questions").find('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').show();
                    }
                    else {
                        $(event.target).closest(".quiz-overview").find('.various-actions .remove, .various-actions .edit-settings').hide();
                        $(event.target).closest(".selected-questions").find('.question-bank-header-right .questions-actions .remove-btn-wrapper').hide();
                        $(event.target).closest(".available-questions").find('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').hide();
                        $(event.target).closest(".available-questions").find('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').hide();
                    }
                }
                
                $('.question-list').questionListGearbox('updateQuestionsMenu', event);

            },

            QuestionCheckSelect: function (event) {
                $('.question-list').questionListGearbox('updateQuestionsMenu', event);
                PxQuiz.MarkQuestionActive(event);
                window.setTimeout(function () { $('#fne-window').removeClass('require-confirm'); }, 500);
            },

            ShowAddToNewAssessment: function () {
                var questionId = $(this).closest('div').find('#used-elsewhere-question-id').val();
                if ($(this).parents('.qtip').length > 0) {
                    $(this).parents('.qtip').qtip('toggle', false);
                }

                _static.fn.CloseQuestionCard();

                var div = '<div class="add-to-new-assessment-dialog"></div>';
                $('body').append(div);

                var tag = $(_static.sel.addToNewAssessmentDlg);

                var options = {
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    width: '700px',
                    height: '400px',
                    resizable: false,
                    autoOpen: false,
                    title: 'Use question in new assessment:'
                };

                tag.dialog({
                    modal: options.modal,
                    title: options.title,
                    draggable: options.draggable,
                    closeOnEscape: options.closeOnEscape,
                    width: options.width,
                    resizable: options.resizable,
                    close: function (event, ui) {
                        $(this).find(".unit-subitems-wrapper").ContentTreeWidget("destroy");
                        tag.dialog('destroy');
                        tag.remove();
                    }
                }).dialog('open');

                var data = {
                    questionId: questionId
                };

                PxPage.Loading(tag);
                $.get(PxPage.Routes.show_add_to_new_assessment, data, function (response) {
                    tag.html(response);
                    tag.dialog("option", "position", "center");
                    PxPage.Loaded(tag);
                });


            },
            DeserializeQuestionList: function (target, response, state) {
                // Some li's have id=undefined. For now just ignore them.
                if (PxQuiz.cfind(target, "ul.questions li")) {
                    var lis = PxQuiz.cfind(target, "ul.questions li").not('.question-table-header, #undefined');


                    if (state.needsRefresh || $(target).hasClass('is-question-pool') || $(target).hasClass('update-question-list')) {
                        $(target).removeClass('update-question-list');
                        var uniqueNewQuestionsToAdd = state.newQuestionsToAdd.filter(function(itm, i, newQuestionsToAdd) {
                            return i == newQuestionsToAdd.indexOf(itm);
                        });


                        //update all questions with the appropriate data from the server
                        var newQuestions = $(response).find("li.question");
                        lis.each(function(index, li) {
                            var lielem = $(li);
                            var id = lielem.find("input.question-id").val();
                            var questionType = lielem.find(".question-type").val();
                            var isBank = (questionType == "BANK");


                            if ($.inArray(id, uniqueNewQuestionsToAdd) >= 0) {
                                var newQuestion = newQuestions.filter("li#" + id).hide().addClass("fade-effect");
                                lielem.replaceWith(newQuestion);
                            }
                            if (isBank) {
                                //$(".quiz-editor-questions .question-display.quiz .questions li input.question-id[value='" + id + "']")
                                lielem.find(".points-container").first().replaceWith(
                                    newQuestions.filter("li#" + id).find(" .points-container:first"));
                            }
                        });
                        newQuestions.show();

                        $(".quiz-editor-questions .question-display.quiz .questions").show();

                        PxQuiz.UpdateAddedQuestions();
                        _static.fn.ClearAllQuestions($('.available-questions'));

                    } else {
                        PxPage.Loaded($(".quiz-editor-questions .question-container"));
                    }
                }
                $(PxPage.switchboard).trigger("questionlistupdated");
            },
            SerializeQuestionList: function (target) {
                var state = {};
                state.questionIds = [];
                state.bankCounts = [];
                state.questions = [];
                state.newQuestionsToAdd = [];
                // If we're in a question bank li, then use that as the quiz id, otherwise, use the main quiz id
                var bank = $(target).closest('.question.bank');
                state.quizId = bank.length ? bank.attr('id') : $('.question-display')[0].id;
                state.mainQuizId = $("#content-item-id").text();


                // Some li's have id=undefined. For now just ignore them.
                var lis = [];
                if ($(target).is("ul.questions")) {
                    lis = $(target).children("li").not('.question-table-header, #undefined');
                } else {
                    lis = $(target).find("ul.questions:first>li").not('.question-table-header, #undefined');
                }
                
                state.needsRefresh = false;
                $(lis).remove('.level .question');
                var qIndex = 0;
                lis.each(function (index, li) {
                    // When we discover the first li from the avaialable questions or quizes, then add
                    // all selected questions in order they appear in the available questions 
                    // list.  Also mark them as not available any more.
                    if ($(li).hasClass('available')) {
                        $('.available-questions li.question:visible:has(input[name=selected]:checked)').each(function (i, v) {
                            state.newQuestionsToAdd.push($(v).attr("id"));
                            $(v).removeClass('available');
                            $(v).find("input[name=selected]").prop("checked", false);
                            state.needsRefresh = true;
                           
                        });

                        $('.available-questions li.quiz-item:visible:has(input[name=selected]:checked)').each(function (i, v) {
                            state.newQuestionsToAdd.push($(v).attr("id"));
                            $(v).removeClass('available');
                            $(v).find("input[name=selected]").prop("checked", false);
                            state.needsRefresh = true;
                            
                        });
                    }

                    _static.fn.AddQuestion(state, qIndex, li);
                    qIndex++;

                });
                return state;
            },

            //target - target question-list to update
            //callback - called when question list has been updated
            //Note: newly added questions will be repalced automatically
            SaveQuestionList: function (target, hideToast) {
                PxPage.Loading($(".question-display"));
                
                $(target).each(function (i, v) {
                    var itarget = $(v);

                    PxPage.log('Called Saved Question List');

                    if ($(itarget).hasClass('is-question-pool')) {
                        //user has dragged a question into a question pool, save entire quiz along with currently opened pools
                        var totalQuestionList = $(itarget).parents('.question-list').length;
                        var topQuestionList = $(itarget).parents('.question-list')[totalQuestionList - 1];

                        $(topQuestionList).addClass('update-question-list');

                        itarget = topQuestionList;
                    }

                    var state = _static.fn.SerializeQuestionList(itarget);

                    // Now save the question list and update the left side on the callback
                    $.ajax({
                        url: PxPage.Routes.question_order,
                        type: "POST",
                        data: JSON.stringify({
                            QuizQuestions:
                            {
                                "quizid": state.quizId,
                                "questionids": (state.questionIds).join(","),
                                "bankcounts": state.bankCounts.join(","),
                                "mainQuizId": state.mainQuizId,
                                "questions": state.questions
                            }
                        }),
                        contentType: 'application/json',
                        success: function (response) {
                            if (!hideToast) {
                                PxPage.Toasts.Success("Quiz Updated");
                            }
                            _static.fn.DeserializeQuestionList(itarget, response, state);

                            $(PxPage.switchboard).trigger("saveQuestionOnComplete");
                                
                            PxPage.Fade();
                            PxPage.Loaded($(".question-display"));
                        }
                    });

                    // Mark all questionsi in the availble side as available
                    $('.available-questions li.question, .available-questions li.quiz-item').addClass('available');
                    
                });
            },
            // removeFrom and addTo refers to the subject's quiz id
            UpdateMetaData: function (questionIdsToDelete, removeFrom, addTo) {
                $.post(
                    PxPage.Routes.update_questions_meta, {
                        QuestionIds: questionIdsToDelete,
                        removeFrom: removeFrom,
                        addTo: addTo
                    },
                    function (response) {
                        PxPage.log('saved metadeta: ' + response);
                    }
                );
            },
            
            RemoveSelected: function (itemLi) {
                var questionList = $(itemLi).parents('.question-list');

                //refresh the question List.
                var questionIdsToDelete = $(itemLi).map(function (i, n) {
                    return $(n).attr('id');
                }).get().join(',');

                while ($(questionList).find(itemLi).length) {
                    $(itemLi).remove();
                }
                var mainList = $(questionList).closest('.question-display').find('.question-list');
                var quizId = $(".quiz .content-item-id").text();

                //on removing the question we've to update it's metadata too.
                _static.fn.UpdateMetaData(questionIdsToDelete, quizId);

                $(questionList).ajaxStart(function () { PxPage.Loading($(questionList)); });
                $(questionList).ajaxStop(function () {
                    _static.fn.CheckForNoQuestionsLeft(questionList);
                }
                );

                _static.fn.NumberQuestions(mainList);
                _static.fn.SaveQuestionList(questionList);
                PxQuiz.UpdateAddedQuestions();

                $('#fne-content').find('#lnkPoints').bind('click', function (event) {
                    event.preventDefault();
                    return false;
                });
            },
            AddQuestion: function (state, index, li) {
                var id = $(li).find(".question-id").val();
                var entityId = null;
                $.each(li.className.split(/\s+/), function (i, v) {
                    if (v.substr(0, 9) === "entityId-") {
                        entityId = v.substr(9);
                        return false;
                    }
                });
                //fix for indexOf not working on IE8 and below browsers. PLATX-4886
                if (!($.inArray(id + '|' + entityId, state.questionIds) >= 0) && id != "") {
                    state.questionIds.push(id + '|' + entityId);
                    var bankCount, useCount;
                    var questionType = $(li).find(".question-type").val();

                    var question = {
                        quizId: state.quizId,
                        questionId: id,
                        entityId: entityId,
                        mainQuizId: state.mainQuizId,
                        isBank: (questionType == "BANK"),
                        bankQuestions: [],
                        isEmpty: false
                    };

                    if (question.isBank) {
                        bankCount = +$(li).find(".bank-question-count").text();
                        useCount = +$(li).find(".questions-count").val();
                        //if (useCount == bankCount) { //Verify: This is incorrect
                        //    useCount = -1;
                        //}
                        state.bankCounts.push([index, useCount, false].join("|"));

                        question.useCount = useCount;

                        //Get list of questions
                        var questionList = $(li).find(".question-list:first");
                        if (questionList.length) {
                            var bankState = _static.fn.SerializeQuestionList(questionList);
                            if (bankState != null && bankState.questions != null && bankState.questions.length > 0) {
                                question.bankQuestions = bankState.questions;
                                state.newQuestionsToAdd = state.newQuestionsToAdd.concat(bankState.newQuestionsToAdd); //if new questions have been added to bank, make sure those LIs are updated
                                state.needsRefresh = state.needsRefresh || bankState.needsRefresh;
                            } else {
                                //bank is empty
                                question.isEmpty = true;
                            }
                        }


                    }

                    if ($(li).hasClass("quiz-item")) {
                        useCount = -1;
                        state.bankCounts.push([index, useCount, true].join("|"));
                        state.needsRefresh = true;

                        question.useCount = useCount;
                    }
                    state.questions.push(question);
                }
            },
            // target: source of the items to copy
            // destination: destination LI
            // sibling: item to add next to
            ProcessMetaDataUpdate: function (target, destination, sibling, callback) {
                //copy but don't clone the questions.
                if (target == null) {
                    target = $('.available-questions');
                }
                if (destination == null) {
                    destination = $('.selected-questions');
                }
                _static.fn.ProcessCopyingQuestions({ target: target, destination: destination, sibling: sibling }, true, callback);
            },
            UpdateQuestionPoint: function (draggedFrom, draggedTo, question) {
                if (draggedFrom == null || draggedTo == null || question == null)
                    return;
                
                //If the question is dragged from question pool to question list
                if ($(draggedFrom).hasClass('is-question-pool') && !$(draggedTo).hasClass('is-question-pool')) {
                    var questionPoolPointData = $(draggedFrom).parents('.bank').find('.questions-points');
                    var qusetionPoolPoint = questionPoolPointData != null && !!questionPoolPointData.length ? questionPoolPointData.val() : 1;
                    question.find('.point-label').html(qusetionPoolPoint + (qusetionPoolPoint > 1? 'pts' : 'pt'));
                }
            },
            AddQuestionFromAvailable: function (question, destination) {
                var clone = $(question).clone();//.hide();
                clone.find("input[name=selected]").prop("checked", false);
                clone.addClass("fade-effect");
                if (destination == null) {
                    destination = $(".selected-questions .question-list ul.questions:first");
                    destination.append(clone);
                } else {
                    $(destination).append(clone);
                }
                return clone;
            },
            ProcessCopyingQuestions: function (event, toClone, callback) {
                var questionsToAdd = [];
                var availableQuestions = PxQuiz.cfind(event.target, ".questions:visible li.question, .children-list:visible .quiz-item");

                // special case triggered from question card
                if (availableQuestions == undefined || availableQuestions.length == 0) {
                    availableQuestions = $('.questions:visible li.question');
                }

                if (availableQuestions != null) {
                    var entityId = "";
                    availableQuestions.each(function (i, v) {
                        if ($(v).find("input[name=selected]").prop('checked')) {
                            if (toClone) {
                                if ($(event.destination).find("li.question input.question-id[value=" + $(v).attr("id") + "]").length == 0) { //only clone if this container doesnt already have the same id as the checked question
                                    var clone = _static.fn.AddQuestionFromAvailable(v, event.destination);
                                    if (event.sibling) {
                                        $(event.sibling).after(clone);
                                    }
                                }

                            }

                            $.each(v.className.split(/\s+/), function (i, v1) {
                                if (v1.substr(0, 9) === "entityId-") {
                                    entityId = v1.substr(9);
                                    return false;
                                }
                            });
                            questionsToAdd.push(v.id + "|" + entityId);
                        }
                    });
                }
                _static.fn.CopyQuestions(questionsToAdd.join(','), callback);
            },
            CopyQuestions: function (questionIdsToCopy, callback) {
                callback.call(this, null);

            },
            UpdateBankCounts: function (target) {
                var parentContainer = $(target).closest('.question-list');
                var questionBanks = $(parentContainer).closest('li.question');

                $(questionBanks).each(function (i, v) {
                    var selectedCounts = $(v).find('input[name=selected]:checked').length;
                    var bankCount = $(v).find('.bank-count');
                    bankCount.text(bankCount.text() - selectedCounts);

                });
            },
            CheckForNoQuestionsLeft: function (parentContainer) {
                PxPage.Loaded($(parentContainer));
                //only show the overview-no-questions if it is a view tab.
                var isViewPage = ($(parentContainer).find('.table-layout').length > 0);
                if (($(parentContainer).find('li.question:not(.question-table-header)').length === 0) && isViewPage) {
                    $(parentContainer).hide();
                    $(parentContainer).parent().find('.various-actions').hide();
                    $(parentContainer).parent().find('.overview-no-questions').show();
                }
            },
            //#region deprecated
            /*SaveTimeLimit: function (event) {
                var timelimit = $(event.target).siblings('.timelimit').val();
                if (timelimit == undefined) {
                    timelimit = $(event.target).val();
                }

                if (!timelimit.match('^(0|[1-9][0-9]*)$')) {
                    PxPage.Toasts.Error("Please enter a numeric value");
                    $(event.target).closest(".description").find(".timelimit-textbox").find('input.timelimit').focus();
                    $(event.target).closest(".description").find(".timelimit-textbox").find('input.timelimit').select();
                    return;
                }

                var questionid = $(event.target).parents('li.question').find('input.question-id').val();
                var quizId = $('#content-item-id').text();

                $.post(
                    PxPage.Routes.save_timelimits, {
                        questionId: questionid,
                        quizId: quizId,
                        timelimits: timelimit
                    },
                    function (response) {
                        PxPage.log('saved timelimits: ' + response);
                    }
                );

                $(event.target).closest(".description").find(".timelimit-label").text(timelimit + " Minutes");
                $(event.target).closest(".description").find(".timelimit-label").show();
                $(event.target).closest(".description").find(".timelimit-textbox").hide();
            },*/
            //#endregion deprecated
            
            NumberQuestions: function (target) {
                var number = 1,
                    numberElement,
                    id,
                    idsSeen = [],
                    foundIndex,
                    itemQuestionCountElem,
                    itemQuestionCount;

                var questions = PxQuiz.cfind(target, '.question-container > ul.questions > li.question, .question-container > ul.questions > li.quiz-item');

                if (questions == undefined || questions == null || questions.length == 0 && !$(target).hasClass('question-pool') && !$(target).hasClass('is-question-pool')) {
                    $(" #fne-window .selected-questions  ul.questions").html("<li class='selected-question-info'>" + $(".selected-questions  .no-question-text").html() + "</li>");
                    return;
                }
                else {
                    $("#fne-window .selected-questions  ul.questions").find('.selected-question-info').remove();
                }
                //TODO:- get only questions that don't have parent as question bank.

                questions = $(questions).filter(function () {
                    return $(this).parents('.question-pool').length < 1;
                });

                questions = $(questions).not('.question-table-header');

                questions.each(function (index, element) {
                    $(element).find(".questions-points").unbind('focusout').bind('focusout', _static.fn.SavePoints);
                    /*deprecated 
                    $(element).find(".save-timelimit").unbind().click(_static.fn.SaveTimeLimit);
                    */

                    id = $(element).id;
                    if (id === undefined || id === null) {
                        id = element.id;
                    }

                    if (id === undefined || id === null || id == "") {
                        id = $(element).find(".question-id").val();
                    }

                    foundIndex = $.inArray(id, idsSeen);
                    if (foundIndex < 0) {
                        itemQuestionCountElem = $(element).find(".questions-count");
                        itemQuestionCount = (itemQuestionCountElem.length > 0) ? +itemQuestionCountElem.val() : 1;
                        numberElement = $(element).find('div.description span.number');
                        numberElement.text(itemQuestionCount == 1 ? number : number + "-" + (number + itemQuestionCount - 1));
                        number += itemQuestionCount;
                        idsSeen.push(id);
                    } else {
                        $(element).remove();
                    }
                });
                _static.fn.HideActionLinks(questions, target);
                PxQuiz.FneResize();
            },
            HideActionLinks: function (questions, target) {
                if (questions.length > 0) {
                    $(".selected-questions .initial-drop-target, .quiz-overview .overview-no-questions").hide();
                } 
            },
            SavePoints: function (event) {
                var points = $(event.target).siblings('.questions-points').val();
                if (points == undefined) {
                    points = $(event.target).val();
                }
                var originalpoint = $(event.target).siblings('.questions-points-original').val();

                if (points == originalpoint) {
                    $(event.target).closest(".description, .total-points").find(".point-label").text(points + (Number(points) > 1 ? " pts" : " pt"));
                    $(event.target).closest(".description, .total-points").find(".point-label").show();
                    $(event.target).closest(".description, .total-points").find(".point-textbox").hide();
                    return;
                }

                var questionid = $(event.target).parents('li.question').find('input.question-id').val();

                if (!points.match('^(0|[1-9][0-9]*)$')) {
                    PxPage.Toasts.Error("Please enter a numeric value");
                    $(event.target).closest(".description, .total-points").find(".point-textbox").find('input.questions-points').focus();
                    $(event.target).closest(".description, .total-points").find(".point-textbox").find('input.questions-points').select();
                    return;
                }
                if (points < 0 || points > 100) {
                    PxPage.Toasts.Error("Points should be between 0 and 100");
                    $(event.target).closest(".description, .total-points").find(".point-textbox").find('input.questions-points').focus();
                    $(event.target).closest(".description, .total-points").find(".point-textbox").find('input.questions-points').select();
                    return;
                }

                PxPage.Loading("'#' + questionid");


                $.post(
                    PxPage.Routes.save_points, {
                        //quizId: $(event.target).closest('.quiz').attr('id'),
                        quizId: $('.quiz .item-id').val(),
                        questionId: questionid,
                        points: points
                    },
                    function (response) {
                        PxPage.log('saved points: ' + response);
                        $(event.target).closest(".description, .total-points").find(".point-label").text(points + (Number(points) > 1 ? " pts" : " pt"));

                        $(event.target).closest(".total-points .point-textbox").find('.questions-points-original').val(points);
                        $(event.target).closest(".description, .total-points").find(".point-label").show();
                        $(event.target).closest(".description, .total-points").find(".point-textbox").hide();
                        _static.fn.NumberQuestions(event.target);
                        if ($('#fne-window').is(':visible')) {
                            $('#fne-window').removeClass('require-confirm');
                        }
                        PxPage.Loaded("'#' + questionid");
                    }
                );

            },
            EditQuestionSettings: function (quizId, questionId, entityId) {
                var route = $.trim($('.QuizType:first').text()) == "Homework" ? PxPage.Routes.question_settings : PxPage.Routes.quiz_question_settings;
                
                $.get(route, { quizId: quizId, questionId: questionId, entityId: entityId }, function (data) {
                    $("#edit-settings-dialog").html(data);
                    $("#edit-settings-dialog").dialog("open");
                });
                
            },
            ValidateEditQuestionSettings: function () {
                var isValid = true;
                $("#Points, #TimeLimit").each(function (i, v) {
                    var val = $(v).val();
                    if (!val) {
                        PxPage.Toasts.Error($(v).attr("data-val-required"));
                        isValid = false;
                    } else if (val != +val || val < 0 || val.indexOf(".") >= 0) {
                        var errorMessage = $(v).attr("data-val-number");
                        errorMessage = errorMessage.replace('number', 'positive integer');
                        PxPage.Toasts.Error(errorMessage);
                        isValid = false;
                    }
                    return isValid;
                });
                if (!isValid) return false;

                var numAttempts = $("[name^='NumberOfAttempts']");
                if (numAttempts.val() == -1) {
                    PxPage.Toasts.Warning(numAttempts.prev().text() + " is required.");
                    return false;
                }

                var radioErrors = {};
                $(".assessment-settings-wrapper input").each(function (i, v) {
                    var name = $(v).attr('name');
                    if (!$("input:radio[name='" + name + "']:checked").length) {
                        radioErrors[name] = $("input:radio[name='" + name + "']").parent().prev().text() + " is required.";
                    }
                });
                $.each(radioErrors, function (k, v) {
                    PxPage.Toasts.Error(v);
                    isValid = false;
                    return false;
                });

                return isValid;
            },
            OpenEditQuestionSettingsDialog: function () {
                $("#edit-settings-dialog").dialog({
                    title: "Edit Question Settings",
                    autoOpen: false,
                    width: 900,
                    height: 500,
                    minWidth: 900,
                    minHeight: 500,
                    zIndex: 2000,
                    modal: true,
                    draggable: true,
                    closeOnEscape: true,
                    buttons: {
                        "Save": function () {
                            if (_static.fn.ValidateEditQuestionSettings()) {

                                $.post(PxPage.Routes.question_settings, $("#edit-settings-form").serialize(), function (data) {
                                    var questionId = $("#edit-settings-form").children("input[name='Id']").val();

                                    var attemptsValue = $("#edit-settings-form").find("select[name='NumberOfAttempts.Attempts']").val();
                                    var pointsValue = $("#edit-settings-form").find("input[name='Points']").val();

                                    $.each(questionId.split(","), function (i, v) {
                                        if (Number(attemptsValue) > 0) {
                                            $("#" + v).find(".attempt-label").text(attemptsValue + (Number(attemptsValue) > 1 ? " attempts" : " attempt"));

                                        }
                                        else {
                                            $("#" + v).find(".attempt-label").text("Unlimited attempts");
                                        }
                                        $("#" + v).find(".point-label").text(pointsValue + (Number(pointsValue) > 1 ? " pts" : " pt"));
                                    });

                                    // If the question editor is open, update values in the menubar
                                    if ($("#hts-editor-ui").length > 0) {

                                        $("#hts-editor-ui .hts-editor-links-container .links").find(".points").text(pointsValue);
                                        $("#hts-editor-ui .hts-editor-links-container .links").find(".attempts").text(attemptsValue);
                                    }

                                    $("#edit-settings-dialog").dialog("close");


                                });
                            }
                        },
                        "Cancel": function () {
                            $("#edit-settings-dialog").dialog("close");
                        }
                    }
                });
            },
            OpenEditQuestionSettingsDialog2: function () {
                $("#edit-settings-dialog2").dialog({
                    title: "Edit Question Settings",
                    autoOpen: false,
                    width: 350,
                    height: 200,
                    minWidth: 100,
                    minHeight: 150,
                    zIndex: 1900,
                    modal: true,
                    draggable: true,
                    closeOnEscape: true,
                    buttons: {
                        "Save": function () {
                            _static.fn.SaveQuestionSettings();
                        },
                        "Cancel": function () {
                            $("#edit-settings-dialog2").dialog("close");
                        }
                    }
                });
                $('#edit-settings-dialog2').unbind('keypress').bind('keypress', function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        _static.fn.SaveQuestionSettings();
                        return false;
                    }
                });

            },
            ValidateQuestionSettings: function () {
                var isValid = true;
                $("#Points").each(function (i, v) {
                    var val = $(v).val();
                    if (!val) {
                        alert($(v).attr("data-val-required"));
                        isValid = false;
                    } else if (val != +val || val < 0 || val.indexOf(".") >= 0) {
                        var errorMessage = $(v).attr("data-val-number");
                        errorMessage = errorMessage.replace('number', 'positive integer');
                        PxPage.Toasts.Error(errorMessage);
                        isValid = false;
                    }
                    else if (val > 100) {
                        PxPage.Toasts.Error("Points possible should be between 0 and 100");
                        isValid = false;
                    }
                });

                return isValid;
            },
            SaveQuestionSettings: function () {
                if (_static.fn.ValidateQuestionSettings()) {
                    PxPage.Loading('ui-dialog', true);
                    $.post(PxPage.Routes.quiz_question_settings, $("#edit-quiz-settings-form").serialize(), function (data) {
                        var questionId = $("#edit-quiz-settings-form").children("input[name='Id']").val();
                        var pointsValue = $("#edit-quiz-settings-form").find("input[name='Points']").val();
                        $.each(questionId.split(","), function (i, v) {
                            $("#" + v).find(".point-label").text(pointsValue + (Number(pointsValue) > 1 ? " pts" : " pt"));
                            $("#" + v).find('.questions-points-original, .questions-points').val(pointsValue);

                        });
                        $('#fne-window').removeClass('require-confirm');

                        // If the question editor is open, update values in the menubar
                        if ($("#hts-editor-ui").length > 0) {
                            $("#hts-editor-ui .hts-editor-links-container .links").find(".points").text(pointsValue);
                        }
                        PxPage.Loaded('ui-dialog', true);
                        $("#edit-settings-dialog2").dialog("close");
                    });
                }

            },
            AddToNewAssessment: function () {
                var tag = $('.add-to-new-assessment-dialog');
                var questionId = tag.find('#assessment-questionid').val();

                var title = tag.find('#assessment-name').val();
                if ($.trim(title) == '') {
                    PxPage.Toasts.Error("Please specify title for new assessment!");
                    return false;
                }

                var parentId = tag.find('.faux-tree-node.active').ftattr('id');
                if (parentId == undefined) {
                    parentId = "PX_MULTIPART_LESSONS";
                }

                var templateId = tag.find('#assessment-type :selected').val();
                var type = tag.find('#assessment-type :selected').text();

                _static.fn.CloseShowAddToNewAssessment();

                PxContentTemplates.onItemFromTemplate({
                    parentId: parentId,
                    templateId: templateId,
                    type: type,
                    title: title,
                    openFNE: false,
                    callback: function (targetId) {

                        $(".question-list").questionlist("addQuestionToQuiz", questionId, targetId);
                    }
                });

            },
            
            CloseShowAddToNewAssessment: function () {
                var tag = $('.add-to-new-assessment-dialog');
                $(tag).dialog('close');
            },
            ModifyInUsedQuestions: function () {
                if ($('#fne-content').is(':visible')) {
                    var usedIds = $('.quiz-editor-questions').find('.questions > li').map(function(i, n) {
                        return $(n).attr('id');
                    }).get();

                    var questions = $(".available-questions ul.questions li.question");
                    $(questions).each(function() {
                        var q = $(this);
                        if (q.hasClass("active"))
                            q.removeClass("active");
                        if (q.hasClass("in-used-question") && $.inArray(q.attr('id'),usedIds) == -1) { //no longer used
                            q.removeClass("in-used-question");
                            q.find("input[name='selected']").show();
                            q.filter(":not(.filteredout)").fadeTo(500, 1.0);
                        }
                        if ($.inArray(q.attr('id'),usedIds) >= 0) {//now used
                            if (!q.hasClass("in-used-question")) {
                                q.fadeTo(500, 0.5);
                                q.find("input[name='selected']").hide(); //hide the checkbox for used questions
                                q.addClass('in-used-question');
                            }
                            
                        }
                    });
                    
                    return false;
                   
                }
            },
            InlineQuestionPreview: function (quid, quCustomUrl, targetId, entityId) {
                $.ajax({
                    url: PxPage.Routes.custom_inline_preview,
                    success: function (xml) {
                        var questionPreview =  $(".custom_preview_" + quid);
                        questionPreview.empty();
                        $(targetId + " #custom_preview_" + quid).html(xml);
                        // replace expanded preview with new content
                        if (targetId === '.question-dialog-text' && !questionPreview.parents('.preview').hasClass('collapsed')) {
                            questionPreview.first().html(xml);          
                        }
                    },
                    data: { questionId: quid, customUrl: quCustomUrl, entityId: entityId },
                    type: "POST"
                });
            },

            IframeQuestionPreview: function (quid, quCustomUrl, targetId, entityId) {
                var url = PxPage.Routes.custom_inline_preview + "?" + "questionId=" + quid + "&customUrl=" + quCustomUrl + "&entityid=" + entityId;
                var questionPreview = $(".custom_preview_" + quid);
                questionPreview.empty();
                var targetDiv = null;

                var $frame = $('<iframe class="question_preview_wrap" src="' + url + '" width="100%" height="100%"></iframe>');             
                if (targetId === '.question-dialog-text' && !questionPreview.parents('.preview').hasClass('collapsed')) {
                    targetDiv = questionPreview.first();
                    targetDiv.html($frame);
                }
                targetDiv = $(targetId + " #custom_preview_" + quid);
                targetDiv.html($frame);
            }
            //end private functions
        }
    };

    var api = {
        init: function (options) {

            _static.fn.BindFneHooks();
            $(this).each(function (i, v) {
                $(v).hide();
                _static.fn.SetupSorting(v);
                _static.fn.NumberQuestions(v);
                $(v).show();
            });
            PxQuiz.UpdateAddedQuestions();

            //expand/collapse question preview
            $(document).off('click', _static.sel.btnExpandAvailQuestion).on('click', _static.sel.btnExpandAvailQuestion, _static.fn.ExpandQuestion);
            $(document).off('click', _static.sel.btnExpandQuestion).on('click', _static.sel.btnExpandQuestion, _static.fn.ExpandQuestion);
            //expand collapse question pool
            $(document).off('click', _static.sel.btnShowQuestionPool).on('click', _static.sel.btnShowQuestionPool, _static.fn.ViewQuestions);
            $(document).off('click', _static.sel.btnHideQuestionPool).on('click', _static.sel.btnHideQuestionPool, _static.fn.HideQuestions);

            //Question Preview
            //$(_static.sel.btnShowPreview).on('click', _static.fn.OpenQuestionDialog);
            $(document).off('click', _static.sel.btnShowPreview).on('click', _static.sel.btnShowPreview, _static.fn.OpenQuestionDialog);
            $(document).off('click', _static.sel.btnShowPreviewAvail).on('click', _static.sel.btnShowPreviewAvail, _static.fn.OpenQuestionDialog);

            //question editing events
            $(document).off('click', _static.sel.btnEditQuestion).on('click', _static.sel.btnEditQuestion, _static.fn.EditCurrentQuestion);
            
            $(document).off('click', _static.sel.btnDeleteQuestion).on('click', _static.sel.btnDeleteQuestion, _static.fn.RemoveCurrentQuestion);
            $(document).off('click', _static.sel.btnDeleteSelectedQuestions).on('click', _static.sel.btnDeleteSelectedQuestions, _static.fn.RemoveSelectedQuestions);

            $(document).off('click', _static.sel.btnExpandAll).on('click', _static.sel.btnExpandAll, _static.fn.ExpandAllQuestions);
            $(document).off('click', _static.sel.btnCollapseAll).on('click', _static.sel.btnCollapseAll, _static.fn.CollapseAllQuestions);

            $(document).off('click', _static.sel.btnPrintQuiz).on('click', _static.sel.btnPrintQuiz, _static.fn.PrintQuiz);
            //settings
            $(document).off('click', _static.sel.btnChangePoints).on('click', _static.sel.btnChangePoints, _static.fn.PointsClick);
            $(document).off('click', _static.sel.btnEditSelectedQuestionSettings).on('click', _static.sel.btnEditSelectedQuestionSettings, _static.fn.EditSelectedQuestionsSettings);

            //question selection
            $(document).off('click', _static.sel.selectQuestion).on('click', _static.sel.selectQuestion, _static.fn.QuestionSelect);
            //trigger when questions are slected
            $(document).off('click', _static.sel.selectQuestionCheckbox).on('click', _static.sel.selectQuestionCheckbox, _static.fn.QuestionCheckSelect);

            
            //go directly to edit screen from overview
            $(document).off('click', _static.sel.btnOverviewEdit).on('click', _static.sel.btnOverviewEdit, _static.fn.EditOverviewQuestion);

            // Edit Question Settings
            _static.fn.OpenEditQuestionSettingsDialog();
            _static.fn.OpenEditQuestionSettingsDialog2();
            
            $(PxPage.switchboard).unbind(".questionlist");
            $(PxPage.switchboard).bind("questionlistupdated.questionlist", function () {
                _static.fn.CommonPostProcessing();
                _static.fn.ModifyInUsedQuestions($('div.select-menu'));
                PxPage.Loaded('.selected-questions');
            });
            $(PxPage.switchboard).bind("availablequestionsupdated.questionlist", _static.fn.AvailableQuestionsUpdated);
            $(PxPage.switchboard).trigger("availablequestionsupdated");

            $(document).off('click', ".selected-questions input[type=checkbox]").on('click', ".selected-questions input[type=checkbox]", _static.fn.SetUsedQuestionMenuStatus);

            //$(PxPage.switchboard).bind("removequestionfromcard.questionlist", function () {
            //    _static.fn.RemoveCurrentQuestion(event);
            //});
           
            $("body").rebind('mousedown.questionlist', function () { _static.defaults.MouseDown = true; });
            $("body").rebind('mouseup.questionlist', function () { _static.defaults.MouseDown = false; });
            $(document).off('click', ".used-elsewhere-quizzes .add-to-existing-quiz").on("click", ".used-elsewhere-quizzes .add-to-existing-quiz", function () {
                _static.fn.CloseQuestionCard();
                _static.fn.ShowAddQuestionCardToQuiz($(this).parents(".used-elsewhere-quizzes").find("#used-elsewhere-question-id").val());
            });
            
            
          

            $(document).off('click', _static.sel.addToNewAssessment).on('click', _static.sel.addToNewAssessment, _static.fn.ShowAddToNewAssessment);
        },
        searchQuestionMetaData: function (searchTerm) {
            window.location.hash = "#state/";

            _static.fn.CloseQuestionCard();

            PxPage.SearchTermOperation({ searchString: searchTerm, mode: "questions" });
        },
        addQuestionToQuiz: function (questionid, quizid) {
            _static.fn.AddQuestionToQuiz(questionid, quizid);
        },
        addQuestionsToPool: function (questions, poolId) {
            if (questions.length > 0) {

                var poolLI = $(".quiz-editor-questions li#" + poolId);
                var isExpanded = $(poolLI).hasClass("bank-expanded");
                try {
                    var bankQuestionCount = parseInt($('li#' + poolId + ' .question-container .bank-question-count').html().trim());
                    $('li#' + poolId + ' .question-container .bank-question-count').html(bankQuestionCount + questions.length);
                } catch (e) { PxPage.log('Update Bank Question size failed: ' + e.message); }

                if (!isExpanded) {
                    $(poolLI).find('.show-current-questions').click();
                    $(PxPage.switchboard).one("questionpool-loaded", function (quizId, isQuestionOverview, mainQuizId) {
                        var poolQuestions = $(poolLI).find(".questions");

                        $(questions).each(function (i, v) {
                            _static.fn.AddQuestionFromAvailable($(v), poolQuestions);
                        });
                        _static.fn.SaveQuestionList($(".selected-questions .question-list:first"));
                    });
                } else {

                    $(questions).each(function (i, v) {
                        var poolQuestions = $(poolLI).find(".questions");
                        _static.fn.AddQuestionFromAvailable($(v), poolQuestions);
                    });
                    _static.fn.SaveQuestionList($(".selected-questions .question-list:first"));
                }
            }
        },
        addSelected: function (event) {
            PxPage.Loading('.selected-questions');
            //$('.selected-questions .question-list ul.questions').hide();
            $('.available-questions li.question .quick-add-question').hide();
            _static.fn.ProcessCopyingQuestions(event, true, function (arg) {
                _static.fn.NumberQuestions($(".selected-questions .question-list"));
                $(PxPage.switchboard).one("saveQuestionOnComplete", function() {
                    PxQuiz.UpdateAddedQuestions();
                    _static.fn.ClearAllQuestions($('.available-questions'));
                    _static.fn.ModifyInUsedQuestions($('div.select-menu'));
                    PxQuiz.UpdateSelectedQuestionsMenu('.selected-questions .question-list'); //IE7 fix.
                    PxPage.Loaded('.selected-questions');
                    $('.quiz-editor-questions').animate({
                        scrollTop: $('.quiz-editor-questions').height()
                    }); //scroll to the bottom of the question editor after adding questions
                });
                _static.fn.SaveQuestionList($(".selected-questions .question-list:first"), false);
            });
            return false;
        },

        moveToTopCurrentQuestion: function (event) {
            var target = $(event.target).closest('li.question');
            var currentTarget = $(target).closest('ul.questions');
            var prevAll = $(target).prevAll('li.question');
            if (prevAll.length > 0) {
                PxPage.Loading(currentTarget);
                $(prevAll[prevAll.length - 1]).before(target);
                _static.fn.NumberQuestions(currentTarget);
                _static.fn.SaveQuestionList(currentTarget);
            }
            $(PxPage.switchboard).one("questionlistupdated", function () {
                $(target).addClass("fade-effect");
                PxPage.Fade();
            });
        },

        moveToBottomCurrentQuestion: function (event) {
            var target = $(event.target).closest('li.question');
            var currentTarget = $(target).closest('ul.questions');
            var nextAll = $(target).nextAll('li.question');
            if (nextAll.length > 0) {
                PxPage.Loading(currentTarget);
                $(nextAll[nextAll.length - 1]).after(target);
                _static.fn.NumberQuestions(currentTarget);
                _static.fn.SaveQuestionList(currentTarget);
                $(PxPage.switchboard).one("questionlistupdated", function () {
                    $(target).addClass("fade-effect");
                    PxPage.Fade();
                });
            }

        },
        numberQuestions: function (target) {
            _static.fn.NumberQuestions(target);
        },
        refreshQuestionList: function () {
            var updateMe = $('.selected-questions').length ? $('.selected-questions') : $('.quiz-overview');
            var category = "question-pool";

            $.post(
                PxPage.Routes.render_children,
                { id: $('.question-display')[0].id, category: category, mainQuizId: $("#content-item-id").text() },
                function (response) {

                    $(updateMe).find(".questions").first().append($(response).find(".questions .question:last").last().parent().html());

                    $(PxPage.switchboard).trigger("questionlistupdated");
                    $('#fne-content').find('#lnkPoints').bind('click', function (event) {
                        event.preventDefault();
                        return false;
                    });
                    $('.question-list').questionListGearbox('updateQuestionsMenu', { target: $('.question-list .questions-menu') });
                    PxPage.Loaded('.selected-questions');
                }
            );
            return false;
        },
        checkForNoQuestionsLeft: function (parentContainer) {
            return _static.fn.CheckForNoQuestionsLeft(parentContainer);
        },
        removeSelected: function (itemLi) {
            return _static.fn.RemoveSelected(itemLi);
        },
        editQuestionSettings: function (quizId, questionId, entityId) {
            _static.fn.EditQuestionSettings(quizId, questionId, entityId);
        },
        addToNewAssessment: function() {
            _static.fn.AddToNewAssessment();
        },
        closeShowAddToNewAssessment: function () {
            _static.fn.CloseShowAddToNewAssessment();
        },
        modifyInUsedQuestions: function (target) {
            _static.fn.ModifyInUsedQuestions(target);
        },
        saveQuestionList: function(target, hideToast) {
            _static.fn.SaveQuestionList(target, hideToast);
        },
        clearAllQuestions: function (target) {
            return _static.fn.ClearAllQuestions(target);
        }//end of public functions
    };
    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.questionlist = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        }
        else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };
    
    window.questionlist = {
        init: function (options) {
            api.init.apply(this, arguments);
        }
    };

} (jQuery));