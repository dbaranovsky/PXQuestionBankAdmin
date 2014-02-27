var PxQuiz = function ($) {

    var isProcessing = false;
    var updatedTimes = 0;
    var enumQuestionMode = {
                                browser: "browser",
                                questioneditor: "editor",
                                pooleditor: "pooleditor",
                                none: "none"
                            };

    // Searches for a selector only within the context of the closest Quiz container
    var cfind = function (target, selector) {
        var lookingAt = $(target);
        if (lookingAt != null && lookingAt.length) {
            var found = $(lookingAt).find(selector);
            if (found.length) {
                return found;
            } else {
                var parents = lookingAt.parents(".question-pool, .selected-questions, .available-questions, .quiz-overview");
                lookingAt = parents.find(selector);
            }
        }
        return lookingAt;
    };
    
    return {
        IntervalId: 0,
        Initialized: false,
        cfind: cfind,
        updatedTimes: updatedTimes,
        QuestionSearchMode: true, //question search mode diables automatic loading and shows the total number of results
        EnumQuestionMode: enumQuestionMode,
        QuestionMode: enumQuestionMode.browser,
        Init: function (quizQuestionsContainerId) {
            $(PxPage.switchboard).rebind("contentloaded.quiz", PxQuiz.DoInit);
            PxQuiz.DoInit(quizQuestionsContainerId);
        },
        DoInit: function (quizQuestionsContainerId) {

            PxQuiz.BindControls();

            $('#fne-window').quizFne('bindFneHooks'); //Quiz Fne bindings
            
            if (quizQuestionsContainerId && quizQuestionsContainerId.length) {
                $("#" + quizQuestionsContainerId).questionlist(); //Question List Init
            }
           
            $(".question-list").questionlistcheckbox(); //intialize checkbox functionality
            $(".question-list").questionfilter(); //Filter functionality

            $(".question-list").questionimport(); //Import functionality

            $(".question-editor").questioneditor(); //Question Editor init
            $(".question-list").quizhomework(); //Homework functionality

            $('.question-list').questionListGearbox();            
            $('.question-list').questionSearch();

            $(PxPage.switchboard).trigger("availablequestionsupdated");

            $('.question-list').questionListGearbox('updateQuestionsMenu', { target: $('.quiz-overview .questions') });

            // Don't show the 'Start Homework' button if none of the 'Attempt' buttons is visible
            if ($('.homework-question-show').length && !$('.homework-question-show .linkButton').length) { $('.linkButton.start-quiz').hide(); }

            updatedTimes = 0;
        },

        BindControls: function () {
            $(document).off('click', '#linkQuizCancel').on('click', '#linkQuizCancel', function () { PxQuiz.CloseDialog(); });

            $(document).off('click', ".start-quiz").on("click", ".start-quiz", function (event) {
                var itemId = $(event.target).attr('id');
                $(PxPage.switchboard).trigger("quizstarted", [itemId]);
            });

            $(document).off('click', ".attempt-homework").on("click", ".attempt-homework", function (event) {
                var itemId = $(event.target).attr('id');
                $(PxPage.switchboard).trigger("homeworkattempt", [itemId]);
            });

            //Handler for a click of the submission link from the previous attempts view
            $(document).off('click', ".submission-link").on("click", ".submission-link", function () {
                $(PxPage.switchboard).trigger("fnedonemode");
            });
        },
        
        RefreshQuestionList: function () {
            $(".question-list").questionlist('refreshQuestionList');
        },
        UpdateQuestionList: function (callback) {
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
        },
        SetAllChecks: function (target, value) {
            if ($(target).length > 0) {
                var questions = cfind(target, "ul.questions:visible li.question [name=selected], li.quiz-item:visible [name=selected]");
                $(questions).each(function () {
                    var q = $(this);
                    if (q.prop('checked') != value)
                        q.prop('checked', value);
                    if (q.hasClass('selected-question-in-quiz-editor'))
                        q.removeClass('selected-question-in-quiz-editor');
                    q.find('.displayquestionmenu').removeClass('displayquestionmenu');
                    
                    if (value && !q.hasClass('active')) {
                        q.addClass('active');
                    } else if(!value && q.hasClass('active')) {
                        q.removeClass('active');
                    }
                    

                });
                if (!value) {
                    $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').hide();
                    $('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').hide();
                }
            }
        },

        UpdateAddedQuestions: function () {
            $(".question-list").questionListGearbox('updateAddMenu');
        },

        UpdatePreviousQuestionOnServer: function (questionId, customQuestionXML, callback, fromQBA, disciplineCourseId) {
            var header = {},  courseId = "", title="" ;
            if (fromQBA != undefined || fromQBA != null) {
                header = { "QBA": "true" };
                courseId = disciplineCourseId;
            }
            if (customQuestionXML != null) {
                title = $('#txtMetaTitle_customComponent').val();
            }
            $.ajax({
                type: "POST",
                url: PxPage.Routes.update_previous_question,
                headers: header,
                data: {
                    questionId: questionId,
                    customQuestionXML: customQuestionXML,
                    courseId: courseId,
                    title:title
                },
                success:
                    function (response) {
                        PxPage.log('updated previous question: ' + response);
                        if ($.isFunction(callback)) {
                            callback.call(this, response);
                        }
                    }
            });
        },

        RemoveQuestionFromCacheOnServer: function (questionId, callback, fromQBA, disciplineCourseId) {
            var header = {}, courseId = "";
            if (fromQBA != undefined || fromQBA != null) {
                header = { "QBA": "true" };
                courseId = disciplineCourseId;
            }

            $.ajax({
                type: "POST",
                url: PxPage.Routes.remove_question_from_cache,
                headers: header,
                data:{
                    questionId: questionId,
                    courseId: courseId },
                success:            
                        function (response) {
                            PxPage.log('remove_question_from_cache : ' + response);
                            if ($.isFunction(callback)) {
                                callback.call(this, response);
                            }
                        }
            });
        },

      

        HasQuestionBankChanged: function () {
            var changed = false;
            if ($(".quiz-editor .question-pool-container").is(':visible')) {
                var target = $('.selected-questions .question-list .questions');

                var prevQuestionId = $('.quiz-editor .question-pool-container').find('#question-pool-id').val();
                if (prevQuestionId === "") return false;
                
                var prevQuestion = $(target).find('#' + prevQuestionId)[0];
                var prevTitle = $($(prevQuestion).find('.description .text')[0]).text();
                var prevPoolCount = $(prevQuestion).find('.questions-count').val();
                var prevPoolPoints = $(prevQuestion).find('.questions-points').val();

                var currentPoolTitle = $('#txtPoolName').val();
                var currentPoolCount = $('#txtPoolCount').val();
                var currentPoolPoints = $('#txtPoolPoints').val();

                if ((currentPoolCount != prevPoolCount) || (currentPoolTitle != prevTitle) || prevPoolPoints != currentPoolPoints) {
                    changed = true;
                }
            }
            return changed;
        },

        FindDropdownRoot: function () {
            return $('.root-selector').val();
        },

        UpdateDefaultRoot: function (event) {
            var rootValue = $(event.target).parents('.browse-question-category').find('#browsermainpagetype').val();
            PxQuiz.SetDefaultRoot($.trim(rootValue));
        },

        FindDefaultRoot: function () {
            return $('.default-root-selector').val();
        },
        SetDefaultRoot: function (value) {
            $('.default-root-selector').val(value);
        },

        LoadDefaultBreadcrumbTrail: function () {
            $('.qtip-defaults ').hide();
            PxQuiz.ShowQTipForDefaultBreadcrumb();
            $('.default-bread-crumb-siblings-list a.default-sibling-item').unbind('click').click(function (event) {
                $('.default-path-item').text('Show All > ');
                PxQuiz.LoadBreadcrumbTrail(event.target);
                event.preventDefault();
            });
        },

        //mode = resources or editor
        PostChildLoadedProcessing: function (questions, mode) {
            var category = "";
            if (PxQuiz.FindDefaultRoot() == "PX_MY_QUESTIONS") {
                category = PxQuiz.FindDefaultRoot();
            }
            
            var classes = category;
            // Give available questions and quizes a class to designate them as from the available questions list
            $('.available-questions li.question').addClass(classes);
            $('.available-questions li.quiz-item').addClass(classes);

            $(PxPage.switchboard).trigger("availablequestionsupdated", [questions]);

            PxQuiz.UpdateAvailableQuestionsMenu('.available-questions .children-list'); //IE7 fix.
            PxQuiz.UpdateSelectedQuestionsMenu('.selected-questions .question-list'); //IE7 fix.

            $('.quiz-editor .children-list a.child-item').unbind('click');
            $('.quiz-editor .children-list a.child-item').click(function (event) {
                //TODO:- update the root if root is not defined.
                var rootItem = PxBreadcrumb.GetRoot();
                if ($(event.target).parents('.browse-question-category').find('#browsermainpagetype').length > 0) {
                    PxQuiz.UpdateDefaultRoot(event);
                    rootItem = PxQuiz.FindDefaultRoot();
                    if (PxQuiz.FindDefaultRoot() == "PX_MY_QUESTIONS") {
                        category = PxQuiz.FindDefaultRoot();
                        rootItem = "PX_TOC";
                        PxQuiz.SetDefaultRoot(rootItem);
                    }
                }
                $('.default-path-item').text('Show All > ');

                var rootItemIds = [];
                rootItemIds.push(rootItem);
                var currentQuizId = $("#content-item-id").text();
                var data = { itemId: event.target.id, rootItemIds: rootItemIds, category: category, isQuestionBank: true, quizId: currentQuizId };
                $('.quiz-editor .available-questions .breadcrumb-trail').load(PxPage.Routes.render_trail, $.param(data, true), PxBreadcrumb.RunResizers);
                PxQuiz.UpdateChildList(data.itemId);


                event.preventDefault();
            });

            //            $('.available-questions .folder-title').text(PxBreadcrumb.GetSelectedItemName($('.quiz-editor .breadcrumb')));
            //$('.available-questions .question-count').text($('.question-container .question-count').text());
            var selectedItemTitle = $(".quiz-editor .available-questions .breadcrumb-trail .selected-item-title").val();
            if (selectedItemTitle != undefined && selectedItemTitle != null) {
                $(".available-questions-header .quiz-bank-title").text(selectedItemTitle);
            }
            $(PxPage.switchboard).trigger("showfiltermetadata", [".available-questions "]);

            //$(".question-list").questionListGearbox('updateAddMenu');

            $('#fne-content').find('#lnkPoints').bind('click', function (event) {
                event.preventDefault();
                return false;
            });
            PxPage.Loaded('.available-questions');
            PxQuiz.FneResize();

            //$('.children-list').bind('scroll', PxQuiz.ScrollMoreQuestions);
           

            PxQuiz.LoadDefaultBreadcrumbTrail();

        },

        LoadBreadcrumbTrail: function (target) {
            var category = "";
            var itemId = "";
            var rootItem = "";
            PxQuiz.SetDefaultRoot(target.id);
            if (PxQuiz.FindDefaultRoot() == "PX_MY_QUESTIONS") {
                itemId = "PX_TOC";
                rootItem = "PX_TOC";
            }
            else {
                itemId = PxQuiz.FindDefaultRoot();
                rootItem = PxQuiz.FindDefaultRoot();

            }
            var rootItemIds = [];
            rootItemIds.push(rootItem);
            var currentQuizId = $("#content-item-id").text();
            var data = { itemId: itemId, rootItemIds: rootItemIds, category: category, isQuestionBank: true, quizId: currentQuizId };

            $('.quiz-editor .available-questions .breadcrumb-trail').load(PxPage.Routes.render_trail, $.param(data, true), PxBreadcrumb.RunResizers);
            PxQuiz.UpdateChildList(data.itemId);
        },

        UpdateChildList: function (itemId) {
            var selectedItem = itemId;
            if (selectedItem == null)
                selectedItem = PxBreadcrumb.GetSelectedItem($('.quiz-editor .breadcrumb'));
            
            var category = "";
            if (PxQuiz.FindDefaultRoot() == "PX_MY_QUESTIONS") {
                category = PxQuiz.FindDefaultRoot();
            }

            //if there are multiple available-questions loaded, reset the browse more resources screen
            //this is done so that we dont have any selector conflicts.
            if ($('.available-questions').length > 1) {
                $(PxPage.switchboard).trigger("hideQuestionBrowser");
            }
            PxPage.Loading('.available-questions');

            //$('.question-search-list').hide();
            $('.children-list').show();

            $('.quiz-editor .children-list').load(
                PxPage.Routes.render_children,
                { id: selectedItem, category: category, startIndex: "0", lastIndex: "20", mainQuizId: $("#content-item-id").text() },
                function () {
                    PxQuiz.PostChildLoadedProcessing();
                }
            );
            PxBreadcrumb.RunResizers();
        },

        LoadMoreQuestions: function (resetProcessing, mode) {
            if (resetProcessing) {
                isProcessing = false;
            }
            var searchTotal = parseInt($('#QuizPaging_TotalCount').last().val());
            var currentIndex = parseInt($('#QuizPaging_LastIndex').last().val());
            var actualTotal = $('.available-questions li.question').length;
            
            //var currentTotal = $('.available-questions li.question').length;
            
            if (isProcessing == false) {
                if (currentIndex < searchTotal) {
                    $('.question-list').questionListGearbox('showHideGearbox', false);

                    isProcessing = true;
                    PxQuiz.LoadMoreQuestionsGetData(mode);
                } else {
                    $('.question-list').questionListGearbox('showHideGearbox', true);

                    $('.question-container .questions-loaded-count, .question-count .questions-loaded-count').text('');
                    $(".quiz-list-loading").remove();
                    $(".question-count").text(actualTotal + " questions available");
                }
            }
        },

        LoadMoreQuestionsGetData: function (mode) {
            var questionTotal = $('.hidden-question-count').val();
            
            if ($(".load-more-questions").length) { //put loading on ShowMoreQuestions link
                PxPage.Loading($(".load-more-questions"));
            }
            
            var data = PxQuiz.LoadMoreQuestionsQueryData(mode);
           
            $.get(PxPage.Routes.render_children, data, function (response) {
                isProcessing = false;
                PxPage.Loaded($(".load-more-questions"));
                var questions = $(response).find('.questions').children();

                $('.available-questions .question-list > .question-container ul.questions').last().append(questions);

                var question_count = $('.available-questions .question-list li.question').length;

                PxQuiz.MarkQuestionActive();
                PxQuiz.PostChildLoadedProcessing(questions, mode);

                if (!PxQuiz.QuestionSearchMode) {
                    //only show loaded %s when automatically loading
                    var percentLoaded = Math.floor(100 * question_count / questionTotal);
                    var text = $.format("Showing {0} questions out of approximately {1} ({2}% loaded...)",question_count, questionTotal, percentLoaded);
                    $(".question-count .questions-loaded").text(text);
                    PxQuiz.LoadMoreQuestions(false, mode);
                } else {
                    var text = $.format("Showing {0} questions out of {1} found",question_count, questionTotal);
                    
                    $(".question-count .questions-loaded").text(text);
                }

            });

        },

        LoadMoreQuestionsQueryData: function (mode) {
            var selectedItem = $("input#questionlist_quizid").val();
            var lastStartIndex = parseInt($(".available-questions #QuizPaging_LastIndex").val());
            var category = "";
            var query = $(".available-questions #Query_IncludeWords").val();
            ;
            if (PxQuiz.FindDropdownRoot() == "PX_MY_QUESTIONS") {
                category = PxQuiz.FindDropdownRoot();
            }
            var data = {
                id: selectedItem,
                category: category,
                startIndex: lastStartIndex,
                lastIndex: 20 + parseInt(lastStartIndex),
                mainQuizId: $("#content-item-id").text()
            };
            if (query) {
                data.query = query;
            }
            if (mode) {
                data.mode = mode;
            }
            $(".available-questions #QuizPaging_LastIndex").val(data.lastIndex);
            return data;
        },

        BindQtipForRelatedContent: function () {
            try {
                $('.question-editor .question-nav .edit-related-content').qtip("destroy");
            } catch (e) { }

            $('.question-editor .question-nav .edit-related-content').qtip({
                content: { text: $('.question-editor').find('.related-content') },
                show: 'mouseover',
                delay: 5000,
                hide: { event: 'unfocus' },
                position: {
                    my: 'bottom right',
                    at: 'left top'
                },
                style: {
                    classes: 'related-content-qtip'
                }
            });
        },

        ShowQTipForDefaultBreadcrumb: function () {
            $('a.default-path-item').each(
                function () {
                    $(this).qtip({
                        content: { text: $(this).parent().find('.default-bread-crumb-siblings-list') },
                        show: 'mouseover',
                        hide: { when: 'unfocus', fixed: true },
                        position: { corner: { tooltip: "topLeft", target: "bottomLeft" } },
                        style: { padding: 0, width: { min: 140, max: 300 } }
                    }).click(function (event) { //event.stopPropagation(); 
                    });
                }
            );
        },

        // When a question is selected, make it active and check its checkbox
        MarkQuestionActive: function (event) {
            if (event != null) {
                var question = $(event.target).closest('li.question');

                if ($(event.target).prop('checked')) {
                    question.addClass('active');
                }
                else {
                    if (question.hasClass("active")) {
                        question.removeClass("active");
                    }
                }
            }
        },

        SetOffsetPosition: function () {
            $('.selected-questions .quiz-editor-questions').scrollTop($('.selected-questions .quiz-editor-questions').prop("scrollHeight"));
        },

        UpdateSelectedQuestionsMenu: function (target) {
            //only show the questions menu if there are atleast one question/quiz in available questions.
            if ($(target).find('input[type=checkbox]').length > 0) {
                $('.question-bank-header-right .select-menu-used').show();
            }
            else {
                $('.question-bank-header-right .select-menu-used').hide();
            }
        },

        UpdateAvailableQuestionsMenu: function (target) {
            //only show the questions menu if there are atleast one question/quiz in available questions.
            if ($(target).find('input[type=checkbox]').length > 0) {
                $('.available-questions-header .add-btn-wrapper, .available-questions-header .expand-btn-wrapper, .available-questions-header .collapse-btn-wrapper').show();
            }
            else {
                $('.available-questions-header .select-menu, .available-questions-header .add-btn-wrapper, .available-questions-header .expand-btn-wrapper, .available-questions-header .collapse-btn-wrapper').hide();
            }

        },

        CloseDialog: function () {
            $('#fne-window').removeClass("require-confirm");
            if ($('.selected-questions').length) {
                PxPage.Loaded('.quiz-editor');
            }
            else {
                PxPage.Loaded();
            }
        },
        FneResize: function () {
            $('#fne-window').quizFne('fneResize');
        },

        OverviewFneInit: function () {
            $('#fne-window').quizFne('overviewFneInit');
        },
        
        FneInit: function () {
            $('#fne-window').quizFne('fneInit');
        },

        ShowFneInit: function () {
            $('#fne-window').quizFne('showFneInit');
        },

        ShowQuizDirections: function () {
            var quizDirectionsModal = $('.quizDirectionModal').first();
            var descrHtml = $('.show-quiz .qDescription').html();
            descrHtml = '<div id="quizDirectionsModal"><div id="content" class="html-container description-content">' + descrHtml + '</div></div>';
            $(quizDirectionsModal).html(descrHtml);
            $(quizDirectionsModal).dialog({
                width: 1000, height: 500, minWidth: 1000, minHeight: 500, modal: true, draggable: false, closeOnEscape: true,
                open: function (event, ui) {
                    $(".ui-dialog-content").attr("style", "font-size:12px;width: 950px;height: 500px;overflow:auto;");
                },
                close: function (event, ui) {
                    $(quizDirectionsModal).html('');
                }
            });
        },

        ConvertToHtsQuestion: function (event) {
            if (!$('.question-editor').questioneditor)
                $('.question-editor').questioneditor();
            $('.question-editor').questioneditor('convertToHtsQuestion', event);
        },

        PreviewCustomQuestion: function (questionId, customUrl, fromQBA, disciplineCourseId) {
            return $(".question-editor").questioneditor('previewCustomQuestion', questionId, customUrl, fromQBA, disciplineCourseId);
        },

        AddToNewAssessment: function () {
            if (!$('.question-list').questionlist)
                $('.question-list').questionlist();
            
            $('.question-list').questionlist('addToNewAssessment');
        },
        
        CloseShowAddToNewAssessment: function () {
            if (!$('.question-list').questionlist)
                $('.question-list').questionlist();

            $('.question-list').questionlist('closeShowAddToNewAssessment');
        }
        
        //#region Depreciated 
        //SaveEverythingBeforeClosing: function () {
        //    //also remove the questions which are empty.            

        //    var quizId = $("#content-item-id").text();
        //    if (quizId == "") {
        //        //faceplate fix
        //        quizId = $("#hidden-content-id").val();
        //    }

        //    if (PxQuiz.HTS_RPC) PxQuiz.HTS_RPC.destroy();

        //    if ($(".quiz-editor .edit-question-container").is(":visible")) {

        //        $('.question-editor').questioneditor('savePreviousQuestion', true, function () {
        //            PxQuiz.ValidateQuestions(function () {
        //                PxQuiz.RefreshItem();
        //                PxPage.Loaded($(".quiz-editor-questions .question-container"));
        //            });
        //        });
        //    }
        //    else {
        //        $(".quiz-editor-questions .question-container").block();
        //        if ($(".quiz-editor .question-pool-container").is(':visible')) {
        //            PxQuiz.ValidateQuestions(function () {
        //                $('.question-editor').questioneditor('saveEditedQuestionPool', null, null);
        //                PxQuiz.RefreshItem();
        //                PxPage.Loaded($(".quiz-editor-questions .question-container"));
        //            });
        //        }
        //        else {
        //            if ($('#fne-content .show-quiz').length > 0) {
        //                PxQuiz.RefreshItem();
        //            }
        //            else {
        //                PxQuiz.ValidateQuestions(function () {
        //                    PxQuiz.RefreshItem();
        //                    PxPage.Loaded($(".quiz-editor-questions .question-container"));
        //                });
        //            }
        //        }

        //    }
        //},

        //ValidateQuestions: function (callback) {
        //    var quizId = $("#content-item-id").text();
        //    if (quizId == "") {
        //        //faceplate fix
        //        quizId = $("#hidden-content-id").val();
        //    }
        //    $.ajax({
        //        url: PxPage.Routes.validate_question_list,
        //        data: { quizId: quizId },
        //        type: "POST",
        //        success: function (response) {
        //            if ($.isFunction(callback)) {
        //                callback.call(this);
        //            }
        //        }
        //    });
        //},

        //RefreshItem: function () {
        //    updatedTimes = 0;
        //    var questionId = $('.question-editor input.id').val();
        //    $(".quiz-overview").html($('.loading-quiz-msg').html());
        //    var itemId = $("#content-item-id").text();
        //    if (itemId == "") {
        //        //faceplate fix
        //        itemId = $("#hidden-content-id").val();
        //    }
        //    $('.qtip-defaults ').hide();

        //    if ($('#fne-window.auto-submit').length) return false;

        //    //for faceplate, things should behave differently.
        //    var isFacePlate = $('.product-type-faceplate').length > 0;
        //    if (itemId != null && itemId != '') {
        //        $('#fne-window #fne-minimize-action').hide();
        //        $.unblockUI();
        //        PxPage.ResetNavigations();
        //        PxPage.ResizeHooksInUse = {};
        //        var getChildrenGrades = ($('#fne-content .show-quiz').length > 0);
        //        if (isFacePlate) {
        //            var currentTarget = $("#fne-window");
        //            PxPage.Loading(currentTarget);
        //        }
        //        ContentWidget.ShowContentItem(itemId, 4, PxQuiz.PreviewLoaded, getChildrenGrades);
        //        //if (!isFacePlate) {
        //        //    PxQuiz.CloseFne();
        //        //} else {
        //        $(".faceplate-fne-gearbox").show();
        //        //}

        //    }
        //    else {
        //        $('#fne-window #fne-minimize-action').hide();
        //        $.unblockUI();
        //        PxPage.ResetNavigations();
        //        PxPage.ResizeHooksInUse = {};
        //    }
        //},

        //UpdateView: function () {
        //    // validates question pool counts to ensure sufficient questions exist for random selection
        //    var mismatch = false;

        //    $(".quiz-editor-questions .bank-label").each(function (index) {
        //        var qused = parseInt(0 + $(this).children(".bank-use").text(), 10);
        //        var qcount = parseInt(0 + $(this).children(".bank-count").text(), 10);
        //        if (qcount < qused) {
        //            mismatch = true;
        //        }
        //    });

        //    if (mismatch == true) {
        //        PxQuiz.OpenQuestionPoolDialog();
        //    }
        //    else {
        //        PxQuiz.SaveEverythingBeforeClosing();
        //    }
        //},

        //PreviewLoaded: function () {
        //    //Removed the div tag when we close the F&E window ,otherwise CloseDialog is not working in summary page 
        //    $('.selected-questions').remove();
        //    if ($('.quiz-overview .question-display').length > 0) {
        //        $('.question-list').questionlist('numberQuestions', $('.quiz-overview .question-display'));
        //    }
        //    PxQuiz.Init();
        //    ContentWidget.Init();
        //    var isFacePlate = $('.product-type-faceplate').length > 0;
        //    if (isFacePlate) {
        //        $("#fne-window").unblock();
        //    }
        //},
        //OpenQuestionPoolDialog: function (event) {
        //    $('.question-validate-text').dialog({
        //        width: 600,
        //        height: 300,
        //        minWidth: 600,
        //        minHeight: 300,
        //        modal: true,
        //        draggable: false,
        //        closeOnEscape: true,
        //        dialogClass: 'no-title-dialog',
        //        buttons: {
        //            "Yes, I'm done": function () {
        //                $(this).dialog("close");
        //                var quizId = $("#content-item-id").text();
        //                if (quizId == "") {
        //                    //faceplate fix
        //                    quizId = $("#hidden-content-id").val();
        //                }
        //                $.ajax({
        //                    url: PxPage.Routes.update_question_bank_totals,
        //                    data: { quizId: quizId },
        //                    type: "POST",
        //                    success: function (response) {
        //                        PxQuiz.SaveEverythingBeforeClosing();
        //                    }
        //                });
        //                $(this).dialog("close");
        //            },

        //            "No, I'd like to add more questions": function () {
        //                $(this).dialog("close");
        //            }
        //        }
        //    });

        //    $('.question-validate-text').html('Some of your pools still contain fewer questions than the pull number you specified. If you continue, these pools will pull only as many questions as are available. Are you really done?');

        //    return false;

        //},
        //#endregion Depreciated

    };
}(jQuery);

