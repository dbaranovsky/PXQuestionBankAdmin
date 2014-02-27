// LearningCurveMoreResources
//
// This plugin is responsible for the client-side behaviors of the

(function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "LearningCurveMoreResources",

        sel: {
            relatedResource: "#related-content-editor .add-related-resource",
            removeRelatedResource: "#related-content-editor .related-content-grid .remove-related-content",
            questionPoolContainer: ".question-pool-inner-container",
            editRelatedContent: ".question-editor .question-nav .edit-related-content",
            editRelatedContentForPool: ".question-pool-inner-container .edit-related-content",
            editFeedback: ".question-editor .question-nav .edit-question-feedback",
            questionEditor: "#hts-editor-ui #question-editor",
            questionPoolEditor: ".quiz-editor .question-pool-container .question-pool-field",
            relatedContentEditorForQuestion: "#related-content-editor-question",
            relatedContentEditorForPool: "#related-content-editor-pool",
            questionNav: ".question-editor .question-rightnav",
            questionPoolNav: ".quiz-editor .question-pool-inner-container .leftnav",
            questionRightNavAlternative: ".question-editor .question-rightnav-alternate",
            poolRightNavAlternative: ".quiz-editor .question-pool-inner-container .rightnav",
            relatedContentToolTip: ".question-editor .related-content",
            relatedContentToolTipForPool: ".question-pool-inner-container .related-content",
            questionSettings: ".quiz-editor .learning-curve-question-settings",
            learningCurveActivityQuizEditor: ".learningCurveActivity",
            learningCurveQuestionSettingForm: "#edit-settings-form-learningCurve",
            learningCurveQuestionSettingsDialog: "#edit-lc-question-settings-dialog",
            learningCurvePreviewQuestionDialog: "#preview-lc-question-dialog",
            selectedQuestionList: ".selected-questions .quiz-editor-questions .question-list",
            previewQuestionviewerFrame: "#preview-lc-question-dialog .lc_iframe-host",
            divShowResourceContentDialog: "#divShowResourceContent"
        },
        // private functions
        fn: {
            // get category information
            editRelatedContent: function (event) {
                $(PxPage.switchboard).unbind("addexistingcontent").bind("addexistingcontent", _static.fn.addRelatedResource);
                $(_static.sel.questionEditor).hide();
                $(_static.sel.relatedContentEditorForQuestion).show();
                $(_static.sel.questionNav).hide();
                $(_static.sel.questionRightNavAlternative).show();
                try {
                    $(_static.sel.editRelatedContent).qtip("hide");
                } catch (e) { }
                return false;
            },

            editRelatedContentForPool: function (event) {
                $(PxPage.switchboard).unbind("addexistingcontent").bind("addexistingcontent", _static.fn.addRelatedResourceToPool);
                $(_static.sel.questionPoolEditor).hide();
                $(_static.sel.relatedContentEditorForPool).show();
                $(_static.sel.questionPoolNav).hide();
                $(_static.sel.poolRightNavAlternative).show();
                try {
                    $(_static.sel.editRelatedContentForPool).qtip("hide");
                } catch (e) { }
                return false;
            },

            showRelatedResource: function (event, mode) {
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', ['learningCurve']);                
                return false;
            },

            addRelatedResource: function (event, sourceId, targetId) {
                var questionId = $('.question-editor input.id').val();
                $(_static.sel.relatedContentEditorForQuestion).block({ message: "Loading..." });
                var args = {
                    itemId: questionId,
                    itemIdToAdd: sourceId
                };
                $.post(PxPage.Routes.add_related_content, args, function (response) {
                    var title = $(".related-content-text").val();
                    $(_static.sel.relatedContentEditorForQuestion).html(response);
                    $(".related-content-text").val(title);
                    $(_static.sel.relatedContentEditorForQuestion).unblock();
                    PxQuiz.BindQtipForRelatedContent();
                });
                return false;
            },

            addRelatedResourceToPool: function (event, sourceId, targetId) {
                var itemId = _static.fn.getCurrentPoolId();
                $(_static.sel.relatedContentEditorForPool).block({ message: "Loading..." });
                var args = {
                    itemId: itemId,
                    itemIdToAdd: sourceId
                };
                $.post(PxPage.Routes.add_related_content, args, function (response) {
                    $(_static.sel.relatedContentEditorForPool).html(response);
                    $(_static.sel.relatedContentEditorForPool).unblock();
                    PxQuiz.BindQtipForRelatedContent();
                });
                return false;
            },

            backToQuestion: function (event) {
                $(_static.sel.questionNav).show();
                $(_static.sel.questionRightNavAlternative).hide();
                $(_static.sel.relatedContentEditorForQuestion).hide();
                $(_static.sel.questionEditor).show();
            },

            backToQuestionPool: function (event) {
                $(_static.sel.questionPoolNav).show();
                $(_static.sel.poolRightNavAlternative).hide();
                $(_static.sel.relatedContentEditorForPool).hide();
                $(_static.sel.questionPoolEditor).show();
            },
            showRelatedContentToolTip: function (event) {
                $(_static.sel.relatedContentToolTip).show();
            },

            hideRelatedContentToolTip: function (event) {
                $(_static.sel.relatedContentToolTip).hide();
            },

            removeRelatedContent: function (event) {
                if (confirm("Are you sure you want to remove this related content ?")) {
                    var questionId = $('.question-editor input.id').val();
                    var editor = $(_static.sel.relatedContentEditorForQuestion);
                    if ($(".quiz-editor .question-pool-container").is(":visible")) {
                        //it's a pool.
                        questionId = _static.fn.getCurrentPoolId();
                        type = "pool";

                        editor = $(_static.sel.relatedContentEditorForPool);
                    }

                    editor.block({ message: "Loading..." });

                    var itemToRemove = $(event.target).next('.related-content-id').text();
                    var args = {
                        itemId: questionId,
                        itemIdToRemove: itemToRemove
                    };
                    $.post(PxPage.Routes.remove_related_content, args, function (response) {
                        editor.html(response);
                        editor.unblock();
                        if (type == "pool") {
                            $(_static.fn.relatedContentQtip("pool"));
                        }
                        else {
                            PxQuiz.BindQtipForRelatedContent();
                        }

                    });
                }
            },

            getCurrentPoolId: function () {
                return $('.quiz-editor .question-pool-container').find('#question-pool-id').val();
            },

            saveRelatedContent: function (itemId, title) {
                if (title == null || $.trim(title) == '') {
                    //check with design team if they want any error message to show.
                }
                else {
                    var editor = $(_static.sel.relatedContentEditorForQuestion);
                    if ($(".quiz-editor .question-pool-container").is(":visible")) {
                        editor = $(_static.sel.relatedContentEditorForPool);
                    }
                    editor.block({ message: "Loading..." });
                    var args = {
                        itemId: itemId,
                        title: title

                    };
                    $.post(PxPage.Routes.save_related_content, args, function (response) {
                        editor.unblock();
                        PxQuiz.BindQtipForRelatedContent();
                        if (response.status != undefined && response.status == "fail") {
                            return;
                        }
                        if (response.status == "success") {
                            PxPage.Toasts.Success("Saved Successfully");
                        }
                    });
                }

            },

            getRelatedContent: function (itemId, type) {
                if (itemId != null) {
                    $(_static.sel.relatedContentEditorForPool).block({ message: "Loading..." });
                    $(_static.sel.questionPoolContainer).block();
                    //get the related content.
                    var args = {
                        itemId: itemId
                    };
                    $.get(PxPage.Routes.get_related_content, args, function (response) {
                        $(PxPage.switchboard).unbind("addexistingcontent").bind("addexistingcontent", _static.fn.addRelatedResourceToPool);
                        if (type == "pool") {
                            $(_static.sel.relatedContentEditorForPool).html(response);
                            $(_static.sel.relatedContentEditorForPool).unblock();
                            $(_static.fn.relatedContentQtip("pool"));
                            $(_static.sel.questionPoolContainer).unblock();
                        }
                    });
                }
            },

            hideRelatedContentForPool: function (toBeRemovedFromDom) {
                if (toBeRemovedFromDom) {
                    $(_static.sel.relatedContentEditorForPool).find("#related-content-editor").remove();
                }
                else {
                    $(_static.sel.relatedContentEditorForPool).hide();
                    $(_static.sel.questionPoolNav).show();
                    $(_static.sel.poolRightNavAlternative).hide();
                }
            },

            relatedContentQtip: function (type) {
                var contentTarget;
                var textToShow;
                if (type == "pool") {
                    contentTarget = $(_static.sel.editRelatedContentForPool);
                    textToShow = $(_static.sel.relatedContentToolTipForPool);
                }
                else {
                    contentTarget = $(_static.sel.editRelatedContent);
                    textToShow = $(_static.sel.relatedContentToolTip);
                }
                try {
                    contentTarget.qtip("destroy");
                } catch (e) { }

                contentTarget.qtip({
                    content: { text: textToShow },
                    show: 'mouseover',                    
                    delay: 5000,
                    hide: { event: 'unfocus' },
                    position: { my: 'bottom right',
                            at: 'left top'
                    },
                    style: {
                        classes: 'related-content-qtip'
                    }
                });
            },
            confirmEditQuestionSettings: function(data) {
                PxModal.CreateConfirmDialog({
                    'title': 'Question Settings Confirmation',
                    'content': 'This action will remove all unsaved data in your question. <br /> <br /> Are you sure you want to proceed?',
                    'buttons': {
                        'Yes': {
                            text: "Yes",
                            click: function() {
                                _static.fn.editQuestionSettings(data);
                            },
                            show: true
                        },
                        'No': {
                            text: "No",
                            show: true
                        }
                    }
                });
            },
            editQuestionSettings: function (data) {
                
                $(_static.sel.learningCurveActivityQuizEditor).block({ message: "Loading..." });
                $.get(PxPage.Routes.lc_question_settings, { quizId: data.quizId, questionId: data.questionId, entityId: data.entityId }, function (data) {
                    $("#edit-lc-question-settings-dialog").html(data);
                    $(_static.sel.learningCurveActivityQuizEditor).unblock();
                    $("#edit-lc-question-settings-dialog").dialog("open");
                });
            },

            updatePrimaryQuestionIcon: function (data) {
                var isPrimary = data.isPrimary;
                var quizID = data.quizID;
                var questionId = data.questionId;

                var visibilityClass = isPrimary ? "show-primary-icon" : "";
                var questionLi = $(_static.sel.selectedQuestionList).find("#" + quizID).find("#" + questionId);
                questionLi.find('.question-edit-wrapper').first().find('.primary-question-image').removeClass('show-primary-icon');
                if (isPrimary) {
                    questionLi.find('.question-edit-wrapper').first().find('.primary-question-image').addClass('show-primary-icon');
                }
            },

            previewQuestion: function (event) {
                var questionViewer = $(_static.sel.previewQuestionviewerFrame);
                questionViewer.empty();
                var questionId = $(event.target).closest('.question-editor').find(".id").val();
                var itemId = $('.question-display')[0].id;
                var args = {
                    questionId: questionId
                };
                $.post(PxPage.Routes.get_lc_question_preview_url, args, function (response) {
                    questionViewer.attr('rel', response.url);
                    questionViewer.LearningCurve({ modelId: itemId });
                    $(_static.sel.learningCurvePreviewQuestionDialog).dialog("open");
                });

            },

            previewQuestionsDialog: function () {
                $(_static.sel.learningCurvePreviewQuestionDialog).dialog({
                    title: "Preview",
                    autoOpen: false,
                    width: 900,
                    height: 600,
                    minWidth: 900,
                    minHeight: 600,
                    zIndex: 2000,
                    modal: true,
                    draggable: true,
                    closeOnEscape: true,
                    buttons: {
                        "Done": function () {
                            //TODO. close the dialog.
                            $(_static.sel.learningCurvePreviewQuestionDialog).dialog("close");
                        }
                    }
                });
            },

            questionSettingsDialog: function () {
                $(_static.sel.learningCurveQuestionSettingsDialog).dialog({
                    title: "Edit Question Settings",
                    autoOpen: false,
                    width: 900,
                    height: 300,
                    minWidth: 900,
                    minHeight: 300,
                    zIndex: 5000,
                    modal: true,
                    draggable: true,
                    closeOnEscape: true,
                    buttons: {
                        "Save": function () {
                            $(_static.sel.learningCurveQuestionSettingsDialog).block({ message: "Loading..." });
                            $.post(PxPage.Routes.lc_question_settings, $(_static.sel.learningCurveQuestionSettingForm).serialize(), function (data) {

                                var toggledData = {
                                    questionId: $(_static.sel.learningCurveQuestionSettingForm).children("input[name='Id']").val(),
                                    quizID: $(_static.sel.learningCurveQuestionSettingForm).children("input[name='QuizId']").val(),
                                    isPrimary: data.PrimaryQuestion
                                }

                                $(_static.sel.learningCurveQuestionSettingsDialog).unblock();
                                $(_static.sel.learningCurveQuestionSettingsDialog).dialog("close");

                                _static.fn.updateDifficultyLevel(data.DifficultyLevel);
                                //update the RHS primary question icon.                                
                                _static.fn.updatePrimaryQuestionIcon(toggledData);
                                
                                //If there is question editor opened, refresh it.
                                if ($('.question-editor').length > 0) {
                                    var simulatedEvent = $.Event('click');
                                    simulatedEvent.target = $('.question-editor .undo-changes').get(0);
                                    $('.question-editor').questioneditor('onConfirmOpenQuestionEditor', simulatedEvent);
                                }

                            });
                        },
                        "Cancel": function () {
                            $(_static.sel.learningCurveQuestionSettingsDialog).unblock();
                            $("#edit-lc-question-settings-dialog").dialog("close");
                        }
                    }
                });
            },

            relatedResourceContentDialog: function() {
                $(_static.sel.divShowResourceContentDialog).dialog({
                    title: "Preview",
                    autoOpen: false,
                    minWidth: 900,
                    minHeight: 800,
                    zIndex: 9999,
                    modal: true,
                    draggable: true,
                    closeOnEscape: true,
                    position: { my: "center" },
                    buttons: {
                        "Add to Question": function () {
                            var targetId = $(this).data("targetId");
                            var itemId = $(this).data("itemId");
                            $(PxPage.switchboard).trigger("addexistingcontent", [itemId, targetId]);
                            $(this).dialog("close");
                        }
                    }
                });
            },

            updateDifficultyLevel: function(level) {
                var value = "";
                switch (level) 
                {
                    case "1":
                        value = "Level 1 (Easy)";
                        break;
                    case "2":
                        value = "Level 2 (Medium)";
                        break;
                    case "3":
                        value = "Level 3 (Hard)";
                        break;
                }
                $(".difficultyLevel").text(value);
            }
        }
    },
    // The public interface for interacting with this plugin.
    api = {
        init: function (opts) {
            return this.each(function () {
                $(document).off('click', _static.sel.editRelatedContent).on('click', _static.sel.editRelatedContent, _static.fn.editRelatedContent);
                $(document).off('click', _static.sel.editRelatedContentForPool).on('click', _static.sel.editRelatedContentForPool, _static.fn.editRelatedContentForPool);
                $(document).off('click', _static.sel.relatedResource).on('click', _static.sel.relatedResource, _static.fn.showRelatedResource);
                $(document).off('click', _static.sel.removeRelatedResource).on('click', _static.sel.removeRelatedResource, _static.fn.removeRelatedContent);
                $(_static.sel.questionRightNavAlternative).find('.back-question').unbind('click').bind('click', _static.fn.backToQuestion);
                $(_static.sel.poolRightNavAlternative).find('.back-question').unbind('click').bind('click', _static.fn.backToQuestionPool);
                $(document).off('click', "#hts-editor-ui a.lc-question-preview:contains('Preview')").on('click', "#hts-editor-ui a.lc-question-preview:contains('Preview')", _static.fn.previewQuestion);
               
                $(_static.sel.editFeedback).hover(function () {

                }, function () {

                });
                _static.fn.questionSettingsDialog();
                _static.fn.previewQuestionsDialog();
                _static.fn.relatedResourceContentDialog();
            });

        },

        saveRelatedContent: function (data) {
            var itemId = data.questionId;
            var title = data.title;
            _static.fn.saveRelatedContent(itemId, title);
        },

        getRelatedContent: function (data) {
            var itemId = data.questionId;
            var type = data.type;
            _static.fn.getRelatedContent(itemId, type);
        },

        hideRelatedContentForPool: function (data) {
            var toBeRemovedFromDom = data.toBeRemovedFromDom;
            _static.fn.hideRelatedContentForPool(toBeRemovedFromDom);
        },

        editQuestionSettings: function (data) {
            _static.fn.confirmEditQuestionSettings(data);
        }
    };

    // Associate the plugin with jQuery
    $.fn.LearningCurveMoreResources = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        }
        else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };
} (jQuery))