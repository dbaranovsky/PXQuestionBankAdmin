//jQuery plugin for Quiz Editor functionality

(function ($) {

    var _static = {
        pluginName: "questioneditor",
        dataKey: "questioneditor",
        bindSuffix: "questioneditor",
        dataAttrPrefix: "data-qe-",
        //plugin defaults
        defaults: {
        },
        //css settings
        css: {

        },
        //selectors for commonly accessed elements
        sel: {
            //CONTAINER ELEMENTS
            questionlistContainer: ".questionlist",
            questionlist: ".question-list ul.questions", //actual UL of the questions
            questionDisplay: ".question-display", //container when questions are being previewed
            quizOverview: ".quiz-overview", //container when quiz are being previewed

            //BUTTONS
            btnNav: ".question-editor .questions-nav button.change",  //question next/previous in question API
            btnUndo: ".question-editor .undo-changes",
            btnSave: ".question-editor .question-nav .save",
            btnShowFeedback: ".question-editor .edit-question-feedback",

            //Question pool editor
            btnPoolSave: ".question-pool-inner-container .pool-actions .save",
            btnPoolUndo: ".question-pool-inner-container .pool-actions .undo-question-pool",
            //non-quiz selectors
            fneDone: "#fne-window #fne-done",
            
            //link buttons
            lnkCustomProperties: ".question-editor .customquestion-properties"
        },
        //private functions
        fn: {
            //#region Edit a question 

            ///<summary>
            /// An EDIT link button is clicked; called from QuizQuestionList > EditCurrentQuestion
            ///</summary>
            EditQuestion: function (event) {
                PxPage.log("entering EditQuestion");

                if (event.isDefaultPrevented()) {
                    event.preventDefault();
                }
                
                var question = $(event.target).closest("li");
                
                // If PxQuiz.QuestionMode == {browser}, just load the question editor dialog
                if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.browser) {
                    _static.fn.OpenQuestionContainer(question);
                    PxPage.log("ending EditQuestion");
                    _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.questioneditor);
                    return;
                }
                else if (_static.fn.IsCurrentlyOpenedQuestion(event)) {
                    PxPage.log("ending EditQuestion | editing the currently opened question");
                    return;
                }
                
                var saveFunction = function() {
                    if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor)
                    {
                        $(PxPage.switchboard).one("saveQuestionPoolOnComplete", function() {
                            _static.fn.OpenQuestionContainer(question);
                        });
                        _static.fn.SaveQuestionPool();
                        
                    } else {
                        // check if the mode is in removing a blank question
                        if (PxQuiz.QuestionMode != PxQuiz.EnumQuestionMode.none) {
                            $(PxPage.switchboard).one("saveQuestionOnComplete", function () {
                                _static.fn.OpenQuestionContainer(question);
                            });
                            _static.fn.SaveQuestion();
                        } else {
                            _static.fn.OpenQuestionContainer(question);
                        }
                    }
                };

                var donotSaveFunction = function() {
                    _static.fn.OpenQuestionContainer(question);
                };

                _static.fn.CreateConfirmDialog(saveFunction, donotSaveFunction);

                PxPage.log("ending EditQuestion");
            },

            ///<summary>
            /// Loads question editor
            ///</summary>
            OpenQuestionContainer: function (question, noReload) {
                PxPage.log("entering OpenQuestionContainer");

                if (!noReload && question) {
                    var quizId = $(question).closest(".quiz").attr('id');
                    var isLast = _static.fn.IsLastEditableQuestion(question);
                    var questionId = question[0].id;

                    _static.fn.LoadQuestion(questionId, quizId, {isLast: isLast, isConvert: false}, null);
                }

                PxPage.log("ending OpenQuestionContainer");
            },

            //#endregion Edit a question

            //#region Edit a question pool

            ///<summary>
            /// An EDIT link button of a Question Pool is clicked; called from QuizQuestionList > EditCurrentQuestion
            ///</summary>
            EditQuestionPool: function (event) {
                PxPage.log("entering EditQuestionPool");

                var target = event ? $(event.target).closest('li.question')[0] : null;
         
                var donotSaveFunction = function () {
                    _static.fn.OpenPoolContainer(target);
                };
                
                // If PxQuiz.QuestionMode == {browser}, just load the question editor dialog
                if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.browser) {
                    _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.pooleditor);
                    donotSaveFunction();
                    PxPage.log("ending EditQuestionPool");
                    return;
                }
                else if (_static.fn.IsCurrentlyOpenedQuestion(event)) {
                    PxPage.log("ending EditQuestion | editing the currently opened question");
                    return;
                }
                
                var saveFunction = function () {
                    if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor) {
                        $(PxPage.switchboard).one("saveQuestionPoolOnComplete", function () {
                            _static.fn.OpenPoolContainer(target);
                        });
                        _static.fn.SaveQuestionPool();
                    }
                    else {
                        // check if the mode is in removing a blank question
                        if (PxQuiz.QuestionMode != PxQuiz.EnumQuestionMode.none) {
                            $(PxPage.switchboard).one("saveQuestionOnComplete", function() {
                                _static.fn.OpenPoolContainer(target);
                            });
                            _static.fn.SavePreviousQuestion();
                        } else {
                            _static.fn.OpenPoolContainer(target);
                        }
                    }
                };
                
                _static.fn.CreateConfirmDialog(saveFunction, donotSaveFunction);

                PxPage.log("ending EditQuestionPool");
            },

            OpenPoolContainer: function (question) {
                PxPage.log("entering OpenPoolContainer");

                if (question != null) {
                    _static.fn.OpenQuestionPool(question);
                } else {
                    _static.fn.CreateNewQuestionPool();
                }

                PxPage.log("ending OpenPoolContainer");
            },

            OpenQuestionPool: function (question) {
                PxPage.log("entering OpenQuestionPool");

                _static.fn.HideQuestionPoolValidations();

                var title = $.trim($($(question).find('.description .text')[0]).text()); //IE7 fix for trim.
                var questionId = $(question).attr('id');

                $(".question-pool-inner-container #txtPoolName").val(title);
                $(".question-pool-inner-container #txtPoolCount").val($(question).find('.questions-count').val());
                $(".question-pool-inner-container #txtPoolPoints").val($(question).find('.questions-points').val());
                $(".question-pool-inner-container .poolSize").text($(question).find('.bank-question-count').text());
                $(".quiz-editor .question-pool-container").find('#question-pool-id').val(questionId);

                _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.pooleditor);

                _static.fn.SelectEditingQuestion(questionId);

                //Load the related content.
                var editorPoolRelatedContent = $("#related-content-editor-pool");
                if (editorPoolRelatedContent.length > 0) {
                    var data = {
                        questionId: questionId,
                        type: "pool"
                    };
                    editorPoolRelatedContent.LearningCurveMoreResources("getRelatedContent", data);
                }

                PxPage.log("ending OpenQuestionPool");
            },

            //#endregion Edit a question pool

            //#region Common functions for editing

            ///<summary>
            /// Highlights the selected question
            ///</summary>
            SelectEditingQuestion: function (questionId) {
                PxPage.log("entering SelectEditingQuestion");

                questionId = "#" + questionId;
                var question = $(".quiz-editor .selected-questions .question-list ul.questions, .quiz-overview .question-list ul.questions").find(questionId);
                $('div.select > input[type=checkbox]').prop('checked', false);
                if (question.find('div.select > input[type=checkbox]').length > 1) {
                    $(question.find('div.select > input[type=checkbox]')[0]).prop('checked', true);
                } else {
                    question.find('div.select > input[type=checkbox]').prop('checked', true);
                }
                $(".question-container ul.questions li.question").removeClass('active selected');
                var questiontype = question.find('.question-edited-type').val();
                var editor = $(".quiz-editor .edit-question-container");
                var isEditorVisible = editor.is(":visible");
                if (questiontype != null && isEditorVisible) {
                    editor.find('.question-type').html(questiontype);
                }
                question.addClass('active selected');

                PxPage.log("ending SelectEditingQuestion");
            },

            //#endregion Common functions for editing

            //#region Edit points

            EditQuestionPointsClick: function (event) {
                PxPage.log("entering EditQuestionPointsClick");

                $(".point-textbox").hide();
                $(".point-label").show();

                if ($(event.target).show()) {
                    $(event.target).closest(".total-points").find(".point-label").hide();
                    $(event.target).closest(".total-points").find(".point-textbox").show();

                    var point = $(event.target).closest('.total-points').find('.questions-points-original').val();
                    $(event.target).closest('.total-points').find('.questions-points').val(point);
                    $(event.target).closest(".total-points").find(".point-textbox").find('input.questions-points').focus().select();
                }

                event.preventDefault();

                PxPage.log("ending EditQuestionPointsClick");
                return false;
            },

            EditQuestionSavePoints: function (event) {
                PxPage.log("entering EditQuestionSavePoints");

                var points = $(event.target).siblings('.questions-points').val();
                if (points == undefined) {
                    points = $(event.target).val();
                }
                $(event.target).closest(".description, .total-points").find(".point-label").text(points + (Number(points) > 1 ? " points" : " point"));
                $(event.target).closest(".total-points .point-textbox").find('.questions-points-original').val(points);
                $(event.target).closest(".description, .total-points").find(".point-label").show();
                $(event.target).closest(".description, .total-points").find(".point-textbox").hide();
                
                PxPage.log("ending EditQuestionSavePoints");
            },

            //#endregion Edit points

            //#region Save a question

            SaveCustomQuestion: function () {
                PxPage.log("entering SaveCustomQuestion");
                var questionId = $(".question-editor input.id").val();
                var savePointsReload = function () {
                    _static.fn.SaveNewQuestionPoints(function () { 
                        _static.fn.UpdateQuestionType(); 
                        $(PxPage.switchboard).trigger("saveQuestionOnComplete");
                        _static.fn.RefreshSavedQuestion(questionId);
                        PxPage.Loaded(".question-editor");
                    });
                };

                var saveMetaDataFunction = function () {
                    _static.fn.SaveQuestionMetadata(function () {
                        savePointsReload();
                    });
                };

                var isHTSQuestion = $("#custom-hts-editor").length > 0;
                if (isHTSQuestion) {
                    var askUserOption = function(result) {
                        if (result == true) {
                            PxQuiz.HTS_RPC.saveQuestion(saveMetaDataFunction);
                        } else {
                            savePointsReload();
                        }
                    };
                    PxQuiz.HTS_RPC.isDirty(askUserOption);
                } else {
                    saveMetaDataFunction();
                }

                PxPage.log("ending SaveCustomQuestion");
            },

            SavePreviousQuestion: function () {
                PxPage.log("entering SavePreviousQuestion");

                if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.questioneditor) {
                    $('.question-editor input.is-new-question').val("False");
                    
                    if (_static.fn.IsCustomQuestionEditor()) {
                        _static.fn.SaveCustomQuestion();
                    } else {
                        $('.question-list').questioneditor('saveQuestion');
                    }
                }

                PxPage.log("ending SavePreviousQuestion");
                return false;
            },
            
            SaveQuestion: function () {
                PxPage.log("entering SaveQuestion");

                //get the selected question
                PxPage.Loading(".question-editor");
                
                if ($("#related-content-editor").is(":visible") && 
                    PxQuiz.QuestionMode != PxQuiz.EnumQuestionMode.none) {
                    _static.fn.SaveRelatedContent();
                }
                else if (_static.fn.IsCustomQuestionEditor()) {
                    _static.fn.SaveCustomQuestion();
                } 
                else if (_static.fn.ValidateQuestionSettingsInEditor()) {
                    var questionId = $(".question-editor input.id").val();

                    var wrap = function() {
                        _static.fn.SaveQuestionMetadata(function() { // question meta-data after question has been saved by BH
                            _static.fn.SaveNewQuestionPoints(function (isSuccess) {
                                if (isSuccess) {
                                    _static.fn.UpdateQuestionType(); //update question type label on question editor
                                    _static.fn.RefreshSavedQuestion(questionId); 
                                }
                                $(PxPage.switchboard).trigger("questionsaved", [false, isSuccess]);
                            });
                        });
                    };
                    $(PxPage.switchboard).unbind("componentsaved");
                    $(PxPage.switchboard).bind("componentsaved", wrap);
                    PxPage.FrameAPI.saveComponent("questioneditor", "quizeditorcomponent", function(result) {
                        if (result == false) {
                            PxPage.Loaded(".question-editor");
                        }
                    });
                }

                PxPage.log("ending SaveQuestion");
            },
            ShowHideFeedback: function() {
                PxPage.FrameAPI.callComponentMethod("questioneditor", "quizeditorcomponent", "isEditFeedbackVisible", [], function (visible) {
                    var text = visible ? "Edit Feedback" : "Hide Feedback";
                    $(_static.sel.btnShowFeedback).html(text);
                    PxPage.FrameAPI.callComponentMethod("questioneditor", "quizeditorcomponent", "showEditFeedback", [!visible]);
                });
            },
          
            SaveRelatedContent: function () {
                PxPage.log("entering SaveRelatedContent");
                
                var questionId = $(".question-editor input.id").val();
                
                var title = $("#related-content-editor .related-content-text").val();
                var data = {
                    questionId: questionId,
                    title: title
                };
                $("#related-content-editor").LearningCurveMoreResources("saveRelatedContent", data);
                $(PxPage.switchboard).trigger("questionsaved");

                PxPage.log("ending SaveRelatedContent");
            },

            //#endregion Save a question

            //#region Save a question pool

            SaveQuestionPool: function () {
                PxPage.log("entering SaveQuestionPool");
                
                if (!PxQuiz.HasQuestionBankChanged()) {
                    $(PxPage.switchboard).trigger("saveQuestionPoolOnComplete");
                    return;
                }
                
                _static.fn.PostQuestionPool(function () {
                    _static.fn.LoadQuestionPool();
                    
                    $(PxPage.switchboard).trigger("saveQuestionPoolOnComplete");
                });
                var editor = $("#related-content-editor-pool #related-content-editor");
                if (editor.length > 0 && editor.is(":visible")) {
                    //also save the related content.                 
                    var title = editor.find(".related-content-text").val();
                    var data = {
                        questionId: $(".quiz-editor .question-pool-container").find("#question-pool-id").val(),
                        title: title
                    };
                    editor.LearningCurveMoreResources("saveRelatedContent", data);
                }

                PxPage.log("ending SaveQuestionPool");
            },

            //#endregion Save a question pool

            //#region Save points

            SaveNewQuestionPoints: function (callback) {
                PxPage.log("entering SaveNewQuestionPoints");

                var quizId = $(".question-editor .quiz-id").val();
                var questionId = $(".question-editor .id").val();
                var entityId = $(".question-editor .question-xml").attr("actualentityid");
                var pointsValue = $(".hts-editor-links .total-points .questions-points-original").val();
                if (questionId && questionId != '') {
                    $.post(PxPage.Routes.quiz_question_settings, { QuizId: quizId, Id: questionId, EntityId: entityId, Points: pointsValue }, function (data) {
                        if (data != true) {
                            _static.fn.RemoveBlankQuestionFromQuiz(data);
                        }
                        $.each(questionId.split(","), function (i, v) {
                            $('.selected-questions').find("#" + v).find(".point-label").text(pointsValue + (Number(pointsValue) > 1 ? " pts" : " pt"));
                            $('.selected-questions').find("#" + v).find('.questions-points-original, .questions-points').val(pointsValue);
                        });
                        if ($.isFunction(callback)) {
                            callback.call(this, data === true);
                        }
                    });
                } else {
                    if ($.isFunction(callback)) {
                        callback.call(this, false);
                    }
                }

                PxPage.log("ending SaveNewQuestionPoints");
            },

            //#endregion Save points

            //#region Update a question on server
            
            PostQuestionPool: function (callback) {
                PxPage.log("entering PostQuestionPool");

                var editTitle = $('#txtPoolName').val();
                var poolCount = $('#txtPoolCount').val();
                var poolPoints = $('#txtPoolPoints').val();
                
                if (_static.fn.ValidateQuestionPool(editTitle, poolCount, poolPoints)) {
                    _static.fn.HideQuestionPoolValidations();

                    PxPage.Loading('.selected-questions');
                    var questionId = $('.quiz-editor .question-pool-container').find('#question-pool-id').val();
                    var parentId = $(".content-item-id").text();
                    if ($.trim(parentId) == "") {
                        //faceplate fix
                        parentId = $("#hidden-content-id").val();
                    }
                    $.post(
                        PxPage.Routes.edit_pool,
                        {
                            parentid: parentId,
                            editTitle: editTitle,
                            poolCount: poolCount,
                            questionId: questionId,
                            poolPoints: poolPoints
                        },
                        function () {
                            if ($.isFunction(callback)) {
                                callback.call(this);
                            }
                            PxPage.Loaded('.selected-questions');
                            $(PxPage.switchboard).trigger("questionsaved");
                        }
                    );
                }

                PxPage.log("ending PostQuestionPool");
            },
            
            ///<summary>
            /// Saves question meta-data after the question has been saved by BH
            ///</summary>
            SaveQuestionMetadata: function (callback) {
                PxPage.log("entering SaveQuestionMetadata");

                var questionEditor = $('.edit-question .question-editor');
                var questionId = $(questionEditor).find(".id").val();
                var isUserCreated = $.trim($(questionEditor).find(".is-user-created").val().toLowerCase()); //IE7 fix for trim.
                var isPublisherUpdated = $.trim($(questionEditor).find(".is-publisher-edited").val().toLowerCase());
                var isCustomQuestion = $(".custom-question-component").is(":visible");
                var customQuestionXml = "";

                if (questionId != null) {
                    if (isCustomQuestion) { /*This is for Graph Question*/
                        try {
                            var xml = PxQuizHts.GetDataForGraphQuestion(questionId);
                            customQuestionXml = $("<div/>").text(xml).html();
                        } catch (err) {
                            //Handle errors here
                            PxPage.log("SaveQuestionMetadata | could not retrieve XML for custom question:" + questionId);
                        }
                    }
                    updateCallback = callback;
                    // Create an update callback to happen before the callback function will be called
                    if (!(isUserCreated == "true" || isPublisherUpdated == "true") || isCustomQuestion) {
                        // We used to ONLY call PxQuiz.UpdatePreviousQuestionOnServer in this scenario.
                        // But that call needs to happen after PxQuiz.RemoveQuestionFromCacheOnServer, otherwise the cached
                        // question's settings will overwrite the edited question's settings. 
                        var updateCallback = function () {
                            PxQuiz.UpdatePreviousQuestionOnServer(questionId, customQuestionXml, callback);
                        };
                    }
                    PxQuiz.RemoveQuestionFromCacheOnServer(questionId, updateCallback);
                } else {
                    if ($.isFunction(callback)) {
                        callback.call(this);
                    }
                }

                PxPage.log("ending SaveQuestionMetadata");
            },

            //#endregion Update a question on server

            //#region Load/Refresh Question Editor

            LoadQuestion: function (questionId, quizId, options, callback) {
                PxPage.log("entering LoadQuestion");

                var fromLearningCurve = $(".edit-current-question").parents("div .learningCurveActivity").length > 0;
                var bLoaded = false;
                $(".faceplate-fne-gearbox").hide();
                var questionSelector = ".quiz-editor-questions ul.questions li#" + questionId;
                var isPoolQuestion = $(questionSelector + " .isPoolQuestion").val();

                var loadingClass = (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor) ? ".question-pool-container" : ".edit-question";

                PxPage.Loading($(".quiz-editor " + loadingClass));

                bLoaded = $(".quiz-editor .edit-question").load(
                                PxPage.Routes.edit_question,
                                { questionId: questionId, quizId: quizId, isLast: options.isLast, isPoolQuestion: isPoolQuestion, isAdvancedConvert: options.isConvert, isFromLearningCurve: fromLearningCurve },
                                function () {
                                    PxPage.Loaded($(".quiz-editor " + loadingClass));

                                    _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.questioneditor);

                                    _static.fn.QuestionFneInit($('.selected-questions .question-list'));
                                    _static.fn.SelectEditingQuestion(questionId);

                                    PxQuiz.BindQtipForRelatedContent();
                                    if ($.isFunction(callback)) {
                                        callback.call(this);
                                    }

                                    return true;
                                }
                            );

                if (bLoaded == false) {
                    _static.fn.QuestionFneInit($('.selected-questions .question-list'));
                    _static.fn.SelectEditingQuestion(questionId);

                    if ($.isFunction(callback)) {
                        callback.call(this);
                    }
                }

                //TODO: to replace interval with event handling
                PxQuiz.IntervalId = setInterval(_static.fn.QuestionFneResize, 100);
                var removeTimeout = function () {
                    clearInterval(PxQuiz.IntervalId);
                    $("#fne-unblock-action").unbind('click', removeTimeout);
                };
                $("#fne-unblock-action").bind('click', removeTimeout);

                PxPage.log("ending LoadQuestion");
            },
            
            LoadQuestionPool: function () {
                PxPage.log("entering LoadQuestionPool");

                var poolContainer = $('.quiz-editor .question-pool-container');

                if (poolContainer != null) {
                    var questionId = poolContainer.find('#question-pool-id').val();
                    var questionSelector = "ul.questions li#" + questionId;

                    var title = poolContainer.find("#txtPoolName").val();
                    var poolSize = poolContainer.find("#txtPoolCount").val();
                    var poolPoints = poolContainer.find("#txtPoolPoints").val();
                    
                    $(questionSelector).find(".questions-count").eq(0).val(poolSize);
                    $(questionSelector).find(".questions-points").eq(0).val(poolPoints);
                    $(questionSelector).find(".description .title .text").eq(0).text(title);
                    $(questionSelector).find(".bank-use").eq(0).text(poolSize);
                    $(questionSelector).find(".bank-points").eq(0).text(poolPoints);

                    $(questionSelector).addClass("fade-effect");
                    PxPage.Fade();
                }

                PxPage.log("ending LoadQuestionPool");
            },

            RefreshSavedQuestion: function (questionId, isUsed) {
                PxPage.log("entering RefreshSavedQuestion");

                var validateAndLogError = function (element, name) {
                    if (element == null || element == "") {
                        PxPage.log(name + " was null or empty. Cannot load single question.");
                        return false;
                    }
                    return true;
                };

                if (!validateAndLogError(questionId, "questionId")) return;

                // Get all necessary data for the server-side call from the DOM, and validate.
                var questionSelector = ".selected-questions ul.questions li#" + questionId;
                var questionSelectorForAvailableQuestionList = '.available-questions ul.questions li#' + questionId;
                
                if (!validateAndLogError(questionSelector, "questionSelector")) return;

                var questionNumber = $(questionSelector + " .number").text();

                var quizId = $(questionSelector + " .quizId").val();
                var entityId = $(questionSelector + " .question-entity-id").val();
                var allowSelection = $(questionSelector + " .allowSelection").val();
                var allowDrag = $(questionSelector + " .allowDrag").val();
                var showExpand = $(questionSelector + " .showExpand").val();
                var showAddLink = $(questionSelector + " .showAddLink").val();
                var showPoints = $(questionSelector + " .showPoints").val();
                var extraClass = $(questionSelector + " .extraClass").val();
                var isOdd = $(questionSelector + " .isOdd").val();

                var isReused = $(questionSelector + " .isReused").val();
                if (isUsed) {
                    isReused = isUsed;
                }

                var questionEditedType = $(questionSelector + " .question-edited-type").val();
                var isPrimary = $(questionSelector + " .isPrimary").val();
                var mode = $(questionSelector + " .mode").val();
                var isQuestionOverview = $(questionSelector + " .isQuestionOverview").val();
                var mainQuizId = $(questionSelector + " .mainQuizId").val();
                var isPoolQuestion = $(questionSelector + " .isPoolQuestion").val();
                var showReused = $(questionSelector + " .showReused").val();

                if (!validateAndLogError(quizId, "quizId")) return;
                if (!validateAndLogError(entityId, "entityId")) return;
                if (!validateAndLogError(allowSelection, "allowSelection")) return;
                if (!validateAndLogError(allowDrag, "allowDrag")) return;
                if (!validateAndLogError(showExpand, "showExpand")) return;
                if (!validateAndLogError(showAddLink, "showAddLink")) return;
                if (!validateAndLogError(showPoints, "showPoints")) return;
                if (!validateAndLogError(extraClass, "extraClass")) return;
                if (!validateAndLogError(isOdd, "isOdd")) return;
                if (!validateAndLogError(isReused, "isReused")) return;
                if (!validateAndLogError(questionEditedType, "questionEditedType")) return;
                if (!validateAndLogError(isPrimary, "isPrimary")) return;
                if (!validateAndLogError(mode, "mode")) return;
                if (!validateAndLogError(isQuestionOverview, "isQuestionOverview")) return;
                if (!validateAndLogError(showReused, "showReused")) return;

                if (mainQuizId == null) mainQuizId = "";   // empty string won't cause a problem
                if (isPoolQuestion == null) isPoolQuestion = false;
                
                
                $.get(
                        PxPage.Routes.single_question,
                        {
                            quizId: quizId,
                            questionId: questionId,
                            entityId: entityId,
                            allowSelection: allowSelection,
                            allowDrag: allowDrag,
                            showExpand: showExpand,
                            showAddLink: showAddLink,
                            showPoints: showPoints,
                            extraClass: extraClass,
                            isOdd: isOdd,
                            isReused: isReused,
                            questionEditedType: questionEditedType,
                            isPrimary: isPrimary,
                            mode: mode,
                            isQuestionOverview: isQuestionOverview,
                            mainQuizId: mainQuizId,
                            isPoolQuestion: isPoolQuestion,
                            showReused: showReused,
                            questionNumber:questionNumber
                        }
                    )
                    .done(function (data) {
                        PxPage.log("returned from loading single question");
                        if (data != null && data != "") {
                            var questionSelectorForSelectedQuestionList = $('.selected-questions ul.questions').find(' li#' + questionId);
                            if (questionSelectorForSelectedQuestionList.length > 0) {
                                questionSelectorForSelectedQuestionList.replaceWith(data);
                                questionSelectorForSelectedQuestionList.find('.number').text(questionNumber);
                                questionSelectorForSelectedQuestionList.addClass("fade-effect");
                            }
                            
                            if ($(questionSelectorForAvailableQuestionList).length > 0) {
                                $(questionSelectorForAvailableQuestionList).replaceWith(data);
                            }
                            
                            $(PxPage.switchboard).trigger("availablequestionsupdated");
                            PxQuiz.UpdateAddedQuestions();
                            PxPage.Fade();
                        } else {
                            PxPage.log("data returned was null or empty");
                        }
                    });

                PxPage.log("ending RefreshSavedQuestion");
            },

            UpdateQuizEditorQuestions: function (event, callback) {
                PxPage.log("entering UpdateQuizEditorQuestions");

                if ($('.question-display')[0] != undefined) {
                    $(".quiz-editor-questions").load(
                        PxPage.Routes.question_list,
                        { quizId: $('.question-display')[0].id },
                        function () {
                            $(PxPage.switchboard).trigger("questionlistupdated");
                            if ($.isFunction(callback)) {
                                callback.call(this);
                            }
                        }
                    );
                }

                PxPage.log("entering UpdateQuizEditorQuestions");
            },

            UpdateQuestionType: function () {
                PxPage.log("entering UpdateQuestionType");

                var questionTypeDiv = $('.hts-editor-links-container .question-type-info .question-type');
                if (questionTypeDiv.length > 0) {
                    questionTypeDiv.html(questionTypeDiv.html().replace('not edited', 'edited'));
                }

                PxPage.log("ending UpdateQuestionType");
            },

            //#endregion

            //#region Reload editors

            ConvertToHtsQuestion: function (event) {

                PxPage.log('entering ConvertToHtsQuestion');

                var html = '<div class="modal-content"></div>';

                $(html).dialog({
                    title: 'Edit this question in the advanced question editor?',
                    width: 600,
                    height: 300,
                    modal: true,
                    draggable: false,
                    resizable: false,
                    buttons: {
                        openButton: {
                            text: 'Open advanced editor',
                            click: function () {
                                _static.fn.OnConfirmOpenQuestionEditor(event, true);
                                $(this).dialog('destroy').remove();
                            }
                        },
                        cancelButton: {
                            text: 'Cancel',
                            'class': 'btn-link',
                            click: function () {
                                $(this).dialog('destroy').remove();
                            }
                        }
                    },
                    open: function () {
                        var content = '<p>After saving this question in the advanced editor,'
                                    + 'you will not be able edit it using the basic editor.</p>'
                                    + '<p>The advanced editor, you can:</p>'
                                    + '<ul class="list-bulleted"><li>Create multi-step with multiple responses per step</li>'
                                    + '<li>Use algorithmic variables</li>'
                                    + '<li>Insert mathematical formulas with the formula editor</li>'
                                    + '</ul>';
                        $(this).html(content);
                    }
                });
    
                event.preventDefault();

                PxPage.log('ending ConvertToHtsQuestion');

            },
            
            OnConfirmOpenQuestionEditor: function (event, isAdvancedConvert) {
                PxPage.log("entering OnConfirmOpenQuestionEditor");

                var questionId = $(event.target).closest('.question-editor').find('input.id').val();
                var quizId = $(event.target).closest('.question-editor').find('input.quiz-id').val();

                if (questionId != null && quizId != null) {
                    if (event.preventDefault) {
                        event.preventDefault();
                    }
                    
                    // Right now setting it to false. 
                    // TODO:- Once we have to implement the next/prev module. IsLastEditableQuestion needs to be called. 
                    var question = $(".quiz-editor .selected-questions .question-list ul.questions").find('#' + questionId);
                    var isLast = _static.fn.IsLastEditableQuestion(question);
                    _static.fn.LoadQuestion(questionId, quizId, { isLast: isLast, isConvert: isAdvancedConvert }, null);
                }

                PxPage.log("ending OnConfirmOpenQuestionEditor");
            },

            ReLoadQuestionEditor: function (event) {
                PxPage.log("entering ReLoadQuestionEditor");

                if (confirm("The changes will be lost. Press OK to continue.")) {
                    _static.fn.OnConfirmOpenQuestionEditor(event);
                }

                PxPage.log("ending ReLoadQuestionEditor");
            },

            ReLoadQuestionPoolEditor: function (event) {
                PxPage.log("entering ReLoadQuestionPoolEditor");

                if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor) {
                    //get the qestion-pool id as quiz id.
                    var target = $(".selected-questions .question-list .questions");

                    var prevQuestionId = $(".quiz-editor .question-pool-container").find("#question-pool-id").val();
                    var prevQuestion = $(target).find('#' + prevQuestionId)[0];

                    _static.fn.OpenQuestionPool(prevQuestion);

                }
                if (event != null) {
                    event.preventDefault();
                    event.stopPropagation();
                }

                PxPage.log("ending ReLoadQuestionPoolEditor");
            },

            //#endregion Reload editors
            
            //#region New questions and question pools
            
            OpenEditorForNewQuestions: function (editType) {
                PxPage.log("entering OpenEditorForNewQuestions");
                
                if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.none || PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.browser) {
                    _static.fn.CreateNewQuestion(editType);
                }
                else {
                    var saveFunction = function () {
                        var isAdvancedQuestion = $("#custom-hts-editor").length > 0;
                        if (isAdvancedQuestion || _static.fn.IsCustomQuestionEditor()) {
                            _static.fn.CreateNewQuestion(editType, true);
                        }
                        else if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor) {
                            $(PxPage.switchboard).one("saveQuestionPoolOnComplete", function () {
                                _static.fn.CreateNewQuestion(editType);
                            });
                            _static.fn.SaveQuestionPool();
                        }
                        else if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.questioneditor) {
                            $(PxPage.switchboard).one("questionsaved", function () {
                                _static.fn.CreateNewQuestion(editType);
                            });
                            $('.question-editor').questioneditor('saveQuestion');
                        }
                    };

                    var donotsaveFunction = function() {
                        _static.fn.CreateNewQuestion(editType, false);
                    };
                    
                    _static.fn.CreateConfirmDialog(saveFunction, donotsaveFunction, {
                        "title": "Creating",
                        "content": "Would you like to save?"
                    });
                }
                
                PxPage.log("ending OpenEditorForNewQuestions");
            },

            CreateNewQuestion: function (editType, reloadQuestions) {
                PxPage.log("entering CreateNewQuestion");

                PxPage.Loading($(".quiz-editor .edit-question"));
                
                var quizId = $($(".content-item-id")[0]).text(); //FIX of PLATX-5826.
                if (jQuery.trim(quizId) == "") {
                    quizId = $($(".content-item-id")[0]).val(); //FIX of PLATX-5826.
                }

                $(".quiz-editor .edit-question").load(
                    PxPage.Routes.create_question,
                    { quizId: quizId, type: editType, loadComponent: true },
                    function () {
                        _static.fn.QuestionFneInit($('.selected-questions .question-list'));
                        PxPage.Loaded($(".quiz-editor .edit-question"));

                        _static.fn.UpdateQuizEditorQuestions(null, function () {
                            var questionId = $('.question-editor .id').val();
                            if (questionId != null) {
                                _static.fn.SelectEditingQuestion(questionId);
                            }
                        });
                        
                        //TODO: to replace interval with event handling
                        PxQuiz.IntervalId = setInterval(_static.fn.QuestionFneResize, 1500);
                        var removeTimeout = function () {
                            clearInterval(PxQuiz.IntervalId);
                            $("#fne-unblock-action").unbind('click', removeTimeout);
                        };
                        $("#fne-unblock-action").bind('click', removeTimeout);
                    }
                );
                
                _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.questioneditor);

                if (reloadQuestions) {
                    PxPage.log("Questions Reloaded");
                    _static.fn.UpdateQuizEditorQuestions(null, null);
                }

                PxPage.log("ending CreateNewQuestion");
            },

            CreateNewQuestionPool: function () {
                PxPage.log("entering CreateNewQuestionPool");

                _static.fn.HideQuestionPoolValidations();
                var quizId = $(".content-item-id").text();
                if (quizId == "") {
                    //faceplate fix
                    quizId = $("#hidden-content-id").val();
                }
                var title = "New question pool";
                PxPage.Loading('.quiz-editor-questions');
                $.post(
                    PxPage.Routes.add_new_pool,
                    {
                        parentid: quizId,
                        title: title
                    },
                    function (data) {
                        //append the data.poolId to the poolId of the question.
                        if (data != null) {
                            $(".quiz-editor .question-pool-container").find('#question-pool-id').val(data.poolId);
                        }
                        _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.pooleditor);
                        $(".question-pool-inner-container #txtPoolName").val(title);
                        $(".question-pool-inner-container #txtPoolCount").val('');
                        $(".question-pool-inner-container #txtPoolPoints").val(1);
                        $(".question-pool-inner-container .poolSize").text('0');
                        PxQuiz.UpdateQuestionList(null);
                        PxPage.Loaded('.quiz-editor-questions');
                    }
                );

                PxPage.log("ending CreateNewQuestionPool");
            },

            //#endregion Loading Editor

            //#region Saving New Question
            
            ShowSavedBlankQuestionMessage: function () {
                var message = '<div class="question-removed-message"><div class="message-content">Blank questions cannot be saved, and so this question has been removed. Please fill out questions before saving</div></div>';
                $('.question-editor .blockMsg').html(message);
                $('.question-editor .blockMsg').css('top', '0px');

                var height = $('.question-editor .blockMsg .question-removed-message').height();
                var width = $('.question-editor .blockMsg .question-removed-message').width();
                $('.question-editor .blockMsg .question-removed-message .message-content').css('padding-left', width / 6);
                $('.question-editor .blockMsg .question-removed-message .message-content').css('padding-right', width / 6);
                var messageHeight = $('.question-editor .blockMsg .question-removed-message .message-content').height();
                $('.question-editor .blockMsg .question-removed-message').css('padding-top', height / 2 - messageHeight);
            },

            RemoveBlankQuestionFromQuiz: function (questionData) {
                PxPage.log("entering RemoveBlankQuestionFromQuiz");

                if (!questionData)
                    return;

                $(questionData).each(function () {
                    if (this.Data && !this.Data.Success) {
                        _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.none);

                        var questionLi = $('.selected-questions li#' + this.Data.Id);
                        if (questionLi && questionLi.length > 0) {

                            $('.selected-questions li#' + this.Data.Id).remove();
                            $('.question-list').questionlist('numberQuestions', $('.selected-questions .question-list'));
                        }
                        $(".question-editor input.id").remove();

                        _static.fn.ShowSavedBlankQuestionMessage();
                    }
                });

                PxPage.log("ending RemoveBlankQuestionFromQuiz");
            },
            
            //#endregion
            
            //#region Save&Close Dialogs

            CreateConfirmDialog: function (saveFunction, donotSaveFunction, options) {
                PxModal.CreateConfirmDialog({
                    "title": (options) ? options.title : "Confirmation",
                    "content": (options) ? options.content : "Do you want to save changes to this question?",
                    "buttons": {
                        "Save": {
                            text: "Save",
                            click: saveFunction,
                            show: true
                        },
                        "Don't Save": {
                            text: "Don't Save",
                            click: donotSaveFunction,
                            show: true
                        },
                        "Cancel": {
                            text: "Cancel",
                            show: true
                        }
                    }
                });
            },
            
            ConfirmAndSavePreviousQuestion: function (event, target) {
                PxPage.log("entering ConfirmAndSavePreviousQuestion");

                if ((PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.questioneditor  
                    || PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor)) {

                    if (event != null) {
                        event.preventDefault();
                        event.stopImmediatePropagation();
                    }

                    var gotoHome = event && event.data && event.data.home;
                    var eTarget = target; 

                    var callbackFunction = function() {
                        if (gotoHome) {
                            $("#fne-window").removeClass("require-confirm-custom");
                            window.setTimeout(function () {
                                PxPage.TriggerClick($(eTarget));
                            }, 0);
                        }

                        _static.fn.SwitchMode(PxQuiz.EnumQuestionMode.browser);
                        
                        PxPage.Loaded('#content-item');
                    };
                    
                    var saveFunction = function () {
                                            if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.questioneditor) {
                                                $(PxPage.switchboard).one("saveQuestionOnComplete", callbackFunction);
                                                _static.fn.SaveQuestion();
                                            }
                                            else if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor) {
                                                $(PxPage.switchboard).one("saveQuestionPoolOnComplete", callbackFunction);
                                                _static.fn.SaveQuestionPool();
                                            }

                                            PxPage.log("entering ConfirmAndSavePreviousQuestion | click on 'Save' button");
                                            return true;
                                        };

                    var donotsaveFunction = function() {
                                                callbackFunction();
                                                PxPage.log("entering ConfirmAndSavePreviousQuestion | click on 'Don't Save' button");
                                                return true;
                                            };
                    
                    _static.fn.CreateConfirmDialog(saveFunction, donotsaveFunction, {
                        "title": "Editing",
                        "content": "Are you sure you want to leave?<br/>All your changes will be lost!"
                    });
                }

                PxPage.log("ending ConfirmAndSavePreviousQuestion");
            },
            
            OnDoneEditingClick: function (value) {
                PxPage.log("entering OnDoneEditingClick");

                if (value != undefined && value.code === -32099) {
                    PxPage.Loaded('#content-item');
                    PxPage.Loaded('#contentwrapper');
                    return;
                }
                $(PxPage.switchboard).trigger("questionsaved", [true]);

                PxPage.log("ending OnDoneEditingClick");
            },

            //#endregion 

            //#region Preview

            LoadPreview: function (event) {
                PxPage.log("entering LoadPreview");

                event.preventDefault();

                var questionId = $('.question-editor input.id').val();
                var questionUrl = $('.question-editor input.question-custom-url').val();
                var questionType = $('.question-editor input.type').val();
                var courseId = $('.homepage-course-info #CourseId').val();
                if (questionType == 'CUSTOM' && questionUrl == 'HTS') {
                    PxQuiz.HTS_RPC.previewQuestion(courseId, PxQuizHts.LoadHTSPreview);
                }
                else if (questionType == 'CUSTOM') {
                    PxQuizHts.StoreAndPreviewGraphQuestion(questionUrl);
                }
                else {
                    _static.fn.PreviewQuestion(questionId);
                }

                PxPage.log("ending LoadPreview");
            },
            
            PreviewQuestion: function (questionId) {
                PxPage.log("entering PreviewQuestion");

                //Save the question first if it is in edit mode.
                var previewDiv = $('<div></div>');
                previewDiv.addClass('preview-question-dialog');

                previewDiv.dialog({
                    width: 700,
                    height: 300,
                    minWidth: 700,
                    minHeight: 300,
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    title: 'Question Preview',
                    close: function () {
                        $(this).dialog('destroy').empty();
                    }

                });

                previewDiv.html($("#" + questionId + ", .selected").not(".row-layout").find(".question-text").html());

                PxPage.log("ending PreviewQuestion");
                return false;
            },

            PreviewCustomQuestion: function (questionId, customUrl, fromQBA, disciplineCourseId) {
                //Save the question first if it is in edit mode.
                var previewDiv = $('<div></div>');
                previewDiv.addClass('preview-question-dialog');
                var width, height, minWidth, minHeight, header, courseId;
                var title = 'Advanced Question Preview';
                if (customUrl === 'FMA_GRAPH') {
                    title = 'Graph Exercise Preview';
                }

                if (fromQBA == undefined || fromQBA == null || fromQBA.length == 0) {
                    width = minWidth = 700;
                    height = minHeight = 300;
                    header = {};
                    courseId = ""; /* Take from Context Course Id in server*/
                } else {
                    width = minWidth = 700;
                    height = minHeight = 500;
                    header = { "QBA": "true" };
                    courseId = disciplineCourseId;
                }

                previewDiv.dialog({
                    width: width,
                    height: height,
                    minWidth: minWidth,
                    minHeight: minHeight,
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    title: title,
                    close: function () {
                        $(this).dialog('destroy').empty().remove();
                    }
                });

                $.ajax({
                    url: PxPage.Routes.custom_inline_preview,
                    headers: header,
                    cache: false,
                    success: function (xml) {
                        $("#custom_preview_" + questionId).empty();
                        previewDiv.html(xml);
                    },
                    error: function (x, status, error) {
                        PxPage.log(" calling PxPage.Routes.custom_inline_preview failed | " + status + " | " + error);
                    },
                    data: { questionId: questionId, customUrl: customUrl, entityId: courseId },
                    type: "POST"
                });

                PxPage.log("ending PreviewCustomQuestion");
                return false;
            },

            //#endregion Preview

            //#region Validation

            ValidateQuestionPool: function (title, poolCount, poolPoints) {
                PxPage.log("entering ValidateQuestionPool");

                if (title == "") {
                    $('#spnPoolTitleError').show();
                    $('#txtPoolName').focus();

                    PxPage.log("ending ValidateQuestionPool | title is empty");
                    return false;
                }
                else if (poolCount == "") {
                    $('#spnPoolCountError').show();
                    $('#txtPoolCount').focus();

                    PxPage.log("ending ValidateQuestionPool | pool count is empty");
                    return false;
                }
                else if (!poolCount.match('^(0|[1-9][0-9]*)$')) {
                    $('#spnPoolIntegerError').show();
                    $('#txtPoolCount').focus();

                    PxPage.log("ending ValidateQuestionPool | pool count is invalid");
                    return false;
                }
                else if (poolPoints == "") {
                    $('#spnPoolPointsError').show();
                    $('#txtPoolPoints').focus();

                    PxPage.log("ending ValidateQuestionPool | pool points is empty");
                    return false;
                }
                else if (!poolPoints.match('^(0|[1-9][0-9]*)$')) {
                    $('#spnPoolPointsError').show();
                    $('#txtPoolPoints').focus();

                    PxPage.log("ending ValidateQuestionPool | pool points is invalid");
                    return false;
                }
                else {
                    PxPage.log("ending ValidateQuestionPool | is valid");
                    return true;
                }
            },

            ValidateQuestionSettingsInEditor: function () {
                PxPage.log("entering ValidateQuestionSettingsInEditor");

                var isValid = true;
                $("#Points").each(function (i, v) {
                    var val = $(v).val();
                    if (!val) {
                        PxPage.Toasts.Error("The Points possible field is required.");
                        isValid = false;
                    } else if (val != +val || val < 0 || val.indexOf(".") >= 0) {
                        PxPage.Toasts.Error("The field Points possible must be a positive integer.");
                        isValid = false;
                    }
                    else if (val > 100) {
                        PxPage.Toasts.Error("Points possible should be between 0 and 100");
                        isValid = false;
                    }
                });

                PxPage.log("ending ValidateQuestionSettingsInEditor");
                return isValid;
            },

            //#endregion Validation

            //#region miscelleneous

            BindFneHooks: function () {
                PxPage.log("entering BindFneHooks");

                PxPage.FneResizeHooks["question-editor"] = _static.fn.QuestionFneResize;
                PxPage.FneInitHooks["question-editor"] = _static.fn.QuestionFneInit;

                PxPage.log("ending BindFneHooks");
            },
            HideQuestionPoolValidations: function () {
                PxPage.log("entering HideQuestionPoolValidations");

                $('#spnPoolCountError').hide();
                $('#spnPoolIntegerError').hide();
                $('#spnPoolTitleError').hide();

                PxPage.log("ending HideQuestionPoolValidations");
            },
            IsCurrentlyOpenedQuestion: function (event) {
                //TODO: To hide the EDIT link if it is in edit mode

                if (event == null) {
                    return false;
                }

                //If this question is already opened in editor, don't do anything
                var id = 0;

                var currentlyEditingQuestionid = 0;
                if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.questioneditor) {
                    var question = $(event.target).closest("li")[0];
                    id = question.id;
                    currentlyEditingQuestionid = $(".quiz-editor .edit-question-container input.id").val();
                }
                else if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor) {
                    var pool = $(event.target).closest('li.question')[0];
                    id = pool.id;
                    currentlyEditingQuestionid = $(".quiz-editor .question-pool-container input#question-pool-id").val();
                }

                if (currentlyEditingQuestionid && currentlyEditingQuestionid != '' && currentlyEditingQuestionid == id) {
                    return true;
                }

                return false;
            },
            IsCustomQuestionEditor: function () {
                PxPage.log("entering IsCustomQuestionEditor");

                var isCustomQuestion = $('.custom-question-component').is(":visible");

                PxPage.log("ending IsCustomQuestionEditor");
                return isCustomQuestion;
            },
            IsLastEditableQuestion: function (question) {
                PxPage.log("entering IsLastEditableQuestion");

                if (!question.next) {
                    return false;
                }
                
                var test;
                for (test = question.next() ; test.length > 0; test = test.next()) {
                    if (!test.hasClass('bank')) {
                        PxPage.log("ending IsLastEditableQuestion | returns false");
                        return false;
                    }
                }

                PxPage.log("ending IsLastEditableQuestion");
                return true;
            },
            QuestionFneInit: function (questionList) {
                PxPage.log("entering QuestionFneInit");

                if (!questionList) {
                    questionList = $('.selected-questions .question-list');
                }

                // Show or hide the prev/next buttons as necessary
                // Get this question's ID, and find it in the list of questions.
                var questionId = $(".question-editor input.id").val(),
                    formerQuestionId = $(".question-editor input.former-id").val(),
                    id,
                    last;

                // If this question is not in the list, then append it
                var isNew = true;
                questionList.find('ul.questions li.question:not(#question-list-table)').each(function (i, e) {
                    if (e.id === formerQuestionId || e.id === questionId) {
                        isNew = false;
                    }
                    return isNew;
                });

                if (isNew) {
                    // questionId is sometimes undefined (like when you click "Edit" on a question). This used to cause
                    // list-item's with undefined id's to be appended to the DOM (see PX-3281).
                    if (typeof (questionId) != 'undefined') {
                        questionList.find('ul.questions').append("<li class='question' id='" + questionId + "'></li>");
                    }
                }

                $(".question-editor button").removeAttr('qid');
                $(".question-editor button.next").attr('qid', "new_question");
                var seen = {};
                questionList.find('ul.questions li.question:not(#question-list-table)').each(function (i, e) {
                    if (!$(e).hasClass('bank')) {

                        var txt = e.id;
                        if (!seen[txt]) {
                            seen[txt] = true;

                            id = e.id;
                            if (id === questionId || id === formerQuestionId) {
                                $(".question-editor button.prev").attr('qid', last);
                            }
                            if (last === formerQuestionId || last === questionId) {
                                $(".question-editor button.next").attr('qid', id);
                            }
                            last = id;
                        }
                    }
                });

                //updating the "former id" in the list of questions to the "new id" that was generated on edit of the "current" question.
                //this needs to be done so that prev and next will work as expected since the ids are always changing.
                // But only do this if we actually have a (non-blank) former question id
                if (formerQuestionId) {
                    $('.question-list ul.questions li.question#' + formerQuestionId).attr("id", questionId);
                }

                $(".question-editor .questions-nav button").hide();
                //$(".question-editor button.save").show();           
                $(".question-editor .questions-nav button[qid]").show();

                // If this is an HTS question, then click on the advanced link
                var isHts = false;
                if ($(".question-editor input.is-hts").length > 0) {
                    isHts = $(".question-editor input.is-hts").val().toString().toLowerCase() == "true";
                }

                var type = $(".question-editor input.type").val();
                if (type != null) {
                    if (type.toLowerCase() == "hts") {
                        isHts = true;
                    }
                }

                // If this is not an HTS question, then show the BH IFrame.
                if (!isHts && !_static.fn.IsCustomQuestionEditor()) {
                    PxPage.SetFrameApiHooks();
                    $('.bh-component').show();
                }
                PxPage.log("ending QuestionFneInit");
            },
            QuestionFneResize: function () {
                // PxPage.log("entering QuestionFneResize");

                var topHeight = $("#fne-window .question-editor a.advanced").is(':visible') ?
                    $("#fne-window .question-editor a.advanced").outerHeight(true) :
                    $("#fne-window .question-editor .hts-data").outerHeight(true);
                var bottomHeight = $("#fne-window .question-editor .question-nav").outerHeight(true);

                // Behave differently depending on whther we're in the quiz editor or not
                if ($('.quiz-editor').is(':visible')) {
                    $('.question-editor').height($('#fne-content').innerHeight() - $('.edit-question-container .breadcrumb-trail').outerHeight());
                    $('#question-editor .bh-component-wrapper').height($('.question-editor').innerHeight() - 200);
                    $('#question-editor .bh-component').height($('.question-editor').innerHeight() - 200);
                    $('#question-editor .custom-question-component').height($('.question-editor').innerHeight() - 200);
                    $('#custom-hts-editor').height($('.question-editor').innerHeight() - 160);
                }
                else {
                    $("#fne-window .question-editor .content").height(
                        $("#fne-window .question-editor").innerHeight() - (topHeight + bottomHeight + 350)
                    );
                }

                // PxPage.log("ending QuestionFneResize");
            },
            
            SwitchMode: function(mode)
            {
                $(".quiz-editor .available-questions, " +
                  ".quiz-editor .edit-question-container, " +
                  ".quiz-editor .question-pool-container, " +
                  ".quiz-editor .browse-question-banks-btn-wrapper").hide();

                $(".quiz-editor .edit-question").show();
                
                switch (mode) {                 
                    case PxQuiz.EnumQuestionMode.questioneditor:
                        $(".quiz-editor .edit-question-container").show();
                        $(".quiz-editor .browse-question-banks-btn-wrapper").show();
                        $("#fne-window").addClass("require-confirm-custom");
                        break;
                    case PxQuiz.EnumQuestionMode.pooleditor:
                        $(".quiz-editor .question-pool-container").show();
                        $(".quiz-editor .browse-question-banks-btn-wrapper").show();
                        $("#fne-window").addClass("require-confirm-custom");
                        break;
                    case PxQuiz.EnumQuestionMode.browser:
                        $(".quiz-editor .available-questions").show();
                        $("#fne-window").removeClass("require-confirm-custom");
                        break;
                    case PxQuiz.EnumQuestionMode.none:
                        $(".quiz-editor .edit-question").hide();
                        $(".quiz-editor .edit-question-container").show();
                        $(".quiz-editor .edit-question-container .question-removed-message").show();
                        $(".quiz-editor .browse-question-banks-btn-wrapper").show();
                        $("#fne-window").addClass("require-confirm-custom");
                    default:
                        break;
                }

                PxQuiz.QuestionMode = mode;
            },
            
            CloseDialogForCustomProperties:function(event, ui)
            {
                var newTitle = $(event.target).find("#qmeta_title_customeditorcomponent").val();
                $('#txtMetaTitle_customComponent').val(newTitle);
            },
            
            OpenDialogForCustomProperties: function (event) {
                $('#customquestion-meta-title-dialog').dialog({
                    autoOpen: true,
                    show: {
                        effect: "fold",
                        duration: 1000
                    },
                    hide: {
                        effect: "fold",
                        duration: 1000
                    },
                    modal: true,
                    width: '40%' ,
                    title: "Question Properties",
                    dialogClass: "custom-properties-meta-title",
                    buttons:    [
                                    {
                                        text: "Close", click: function () {
                                            $(this).dialog("close");
                                        } }
                    ],
                    appendTo: "#question-editor",
                    position: { my: "left top", at: "left bottom", of: ".question-editor .customquestion-properties" },
                    close: function (newevent, ui) {
                        _static.fn.CloseDialogForCustomProperties(newevent, ui);
                    }
                });
            }

            //#endregion miscelleneous

            //end of private funtion.
        }
    };

    var api = {
        init: function () {
            //Question Editor
            _static.fn.BindFneHooks();
            $(document).off('click', _static.sel.btnUndo).on('click', _static.sel.btnUndo, _static.fn.ReLoadQuestionEditor); //undo chagnes
            $(document).off('click', _static.sel.btnSave).on('click', _static.sel.btnSave, _static.fn.SaveQuestion); //save changes

            $(document).off('click', _static.sel.lnkCustomProperties).on('click', _static.sel.lnkCustomProperties, _static.fn.OpenDialogForCustomProperties);

            $(PxPage.switchboard).unbind('validateNavigateAway', _static.fn.ConfirmAndSavePreviousQuestion);
            $(PxPage.switchboard).bind("validateNavigateAway", {home: true}, _static.fn.ConfirmAndSavePreviousQuestion);
            $(".browse-question-banks").unbind("click").bind("click", _static.fn.ConfirmAndSavePreviousQuestion);

            $(document).off('click', _static.sel.btnPoolSave).on('click', _static.sel.btnPoolSave, _static.fn.SaveQuestionPool);
            $(document).off('click', _static.sel.btnPoolUndo).on('click', _static.sel.btnPoolUndo, _static.fn.ReLoadQuestionPoolEditor);
            $(document).off('click', _static.sel.btnShowFeedback).on('click', _static.sel.btnShowFeedback, _static.fn.ShowHideFeedback);

            $(PxPage.switchboard).unbind(".questioneditor");
            $(PxPage.switchboard).bind("questionlistupdated.questioneditor", function () {
                $('.question-list').questionListGearbox('updateQuestionsMenu', { target: $(".quiz-editor-questions .questions-menu") });
                if (PxQuiz.QuestionMode == PxQuiz.EnumQuestionMode.pooleditor) {
                    _static.fn.ReLoadQuestionPoolEditor();
                }
            });

            $(PxPage.switchboard).bind("questionsaved.questioneditor", function (e, hideToast, isSuccess) {
                if (!hideToast) {
                    if (isSuccess === false)
                        PxPage.Toasts.Error("Blank questions cannot be saved, and so the blank question you were editing has been removed.");
                    else
                        PxPage.Toasts.Success("Question Saved");
                }

                $('.question-list').questionlist("saveQuestionList",
                                                  $(".selected-questions .question-list:first"), true); // sort questions in custom order

                PxPage.Loaded(".question-editor");
            });

            //hts editor binding
            $(document).off('focusout', "#hts-editor-ui .questions-points").on('focusout', "#hts-editor-ui .questions-points", _static.fn.EditQuestionSavePoints);
            $(document).off('click', "#hts-editor-ui a.portal-question-preview:contains('Preview')").on('click', "#hts-editor-ui a.portal-question-preview:contains('Preview')", _static.fn.LoadPreview);

            $(document).off('click', "#hts-editor-ui a.point-label").on('click', "#hts-editor-ui a.point-label", _static.fn.EditQuestionPointsClick);

            $(PxPage.switchboard).unbind('save-quiz-data');
            $(PxPage.switchboard).bind('save-quiz-data', function (event) {
                if ($('#editQueRes').length > 0) {
                    event.stopPropagation(); /* There are two events fired from fne-done-button, avoiding it by checking, if event was already fired.  */
                }

                var questionUrl = $('.question-editor input.question-custom-url').val();
                var questionType = $('.question-editor input.type').val();

                if (questionType == 'CUSTOM' && questionUrl == 'HTS') {
                    var userSaveFunction = function () {
                        PxQuiz.HTS_RPC.saveQuestion(_static.fn.OnDoneEditingClick, _static.fn.OnDoneEditingClick);
                    };
                    var userDoNotSaveFunction = function () {
                        $(PxPage.switchboard).trigger("questionsaved", [true]);
                        $('.fne-hidden #fne-done').click();
                        setTimeout(function () { PxPage.Loaded('.content'); }, 1500);
                    };
                    var cancelFunction = function () {
                        $('#fne-done').addClass('fne-done-link');
                        PxPage.Loaded('#content-item');
                        PxPage.Loaded('#contentwrapper');
                        return;
                    };

                    var askUserOption = function (result) {
                        if (result == true) {
                            _static.fn.EditQuestionResponseFromUser(event, userSaveFunction, userDoNotSaveFunction, cancelFunction);
                        } else {
                            userDoNotSaveFunction(event);
                        }
                    };

                    PxQuiz.HTS_RPC.isDirty(askUserOption);
                    event.stopImmediatePropagation();
                }
            });
        },
        editQuestion: function (event) {
            _static.fn.EditQuestion(event);
        },
        saveQuestion: function () {
            _static.fn.SaveQuestion();
        },
        previewCustomQuestion: function (questionId, customUrl, fromQBA, disciplineCourseId) {
            return _static.fn.PreviewCustomQuestion(questionId, customUrl, fromQBA, disciplineCourseId);
        },
        openEditorForNewQuestions: function (editType) {
            _static.fn.OpenEditorForNewQuestions(editType);
        },
        updateQuizEditorQuestions: function (event, callback) {
            _static.fn.UpdateQuizEditorQuestions(event, callback);
        },
        saveNewQuestionPoints:function(event, callback) {
            _static.fn.SaveNewQuestionPoints(event, callback);
        },
        saveEditedQuestionPool: function (editType, callback) {
            return _static.fn.saveEditedQuestionPool(callback);
        },
        updateQuizEditorSingleQuestion: function (questionId, isUsed) {
            return _static.fn.RefreshSavedQuestion(questionId, isUsed);
        },
        convertToHtsQuestion: function (event) {
            _static.fn.ConvertToHtsQuestion(event);
        },
        editQuestionPool: function (editType, event) {
            _static.fn.EditQuestionPool(event);
        },
        questionFneInit: function (questionList) {
            _static.fn.QuestionFneInit(questionList);
        },
        questionFneResize: function () {
            _static.fn.QuestionFneResize();
        },
        switchMode: function(mode) {
            _static.fn.SwitchMode(mode);
        },
        onConfirmOpenQuestionEditor: function(event) {
            _static.fn.OnConfirmOpenQuestionEditor(event);

        }
    };

    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.questioneditor = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        }

        $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
    };

    $.fn.questionEditorObj = function () {
        return {
            api: api,
            _static: _static
        };
    };
})(jQuery);