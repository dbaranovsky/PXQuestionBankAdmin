//jQuery plugin for Quiz Gearbox/ActionWidget

(function ($) {
    var _static = {
        pluginName: "questionListGearbox",
        dataKey: "questionListGearbox",
        bindSuffix: ".questionListGearbox",
        dataAttrPrefix: "data-qlg-",
        //selectors for commonly accessed elements
        sel: {
            
        },
        //private functions
        fn: {
            addGearboxAction: function (event, name, gearbox) {
                // If this is not coming from the add menu at the top, (such as if it's from the dialog box),
                // make sure you don't add all checked questions, but just the one being viewed.
                var alreadyChecked = [];

                if (!gearbox.hasClass("add-menu") || $('.question-being-previewed').length > 0) {

                    var question = $('.question-being-previewed');

                    if (question.length === 0) {
                        var target = event.target;
                        question = $(target).closest('li.question');
                    }

                    var input = question.find('input[name=selected]');

                    // If something is checked, it will get added to the quiz.
                    // But we want to just add the currently selected question,
                    // so uncheck everything and then recheck it after the one
                    // question is added to the quiz.
                    $('ul.questions .select input:checked').each(function () {
                        var thisInput = $(this);

                        if (!thisInput.is(input)) {
                            alreadyChecked.push(thisInput);
                            thisInput.prop('checked', false);
                        } else {
                            question.removeClass('active selected-question-in-quiz-editor');
                        }
                    });

                    if (!input.checked) {
                        input.prop('checked', true);
                    }
                } else {// questions are being added from the menu at the top
                    $('ul.questions .select input:checked').each(function () {

                        // Make sure the questions don't keep their blue color when
                        // deactivated
                        var thisInput = $(this);
                        var parentQuestion = thisInput.closest('li.question');
                        parentQuestion.removeClass('active selected-question-in-quiz-editor');
                    });
                }

                var selectedQuestions = [];
                var addedQuestionIds = {};
                $('.available-questions input[name=selected]:checked').closest('li.question').each(function (i, v) {
                    var entityId = "";
                    $.each(v.className.split(/\s+/), function (i, v1) {
                        if (v1.substr(0, 9) === "entityId-") {
                            entityId = v1.substr(9);
                            return false;
                        }
                    });

                    var key = v.id + "|" + entityId;
                    if (!addedQuestionIds[key]) {
                        selectedQuestions.push($(v));
                        addedQuestionIds[key] = true;
                    }
                });

                if (name.indexOf('add-to-pool-') === 0) {

                    var poolId = name.substring(12);
                    $('.question-list').questionlist('addQuestionsToPool', selectedQuestions, poolId);
                } else {
                    switch (name) {
                        case "add-to-quiz":
                            $('.question-list').questionlist('addSelected', { target: gearbox });
                            break;
                        case "move-to-top":
                            $('.question-list').questionlist('moveToTopCurrentQuestion', { target: gearbox });
                            break;
                        case "move-to-bottom":
                            $('.question-list').questionlist('moveToBottomCurrentQuestion', { target: gearbox });
                            break;
                        case "add-to-new-pool":
                            $('.ui-dialog-content').dialog("destroy");
                            _static.fn.createQuestionPoolDialog();
                            $(PxPage.switchboard).one("questionpool-created", function () {
                                poolId = $('.selected-questions .questions .question.bank:last')[0].id;
                                $('.question-list').questionlist('addQuestionsToPool', selectedQuestions, poolId);
                            });
                            break;
                    }
                }

                // Recheck what we explicitly unchecked
                alreadyChecked.forEach(function (thisInput) {
                    thisInput.prop('checked', true);
                });

            },

            createQuestionPoolDialog: function () {
                    
                $('.question-pool-dialog-text').dialog({
                    title: "NEW POOL",
                    width: 420,
                    height: 340,
                    minWidth: 420,
                    minHeight: 340,
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    open: function () {
                        var dialog = $(this);
                        var val = $('#question-pool-dialog-container').html();
                        $('.question-pool-dialog-text').html(val);
                        dialog.css("font-size", "13px;");
                        dialog.find("#pool_title").unbind('keyup').bind("keyup", function () {
                            dialog.find('.pool_title_error').hide('slow');
                        });
                    },
                    buttons: {
                        "Create": function () {
                            PxPage.Loading('.question-pool-dialog-text');
                            var editTitle = $(this).find('#pool_title').val();
                            var poolCount = $(this).find('#pool-count').val();
                            var quizId = $(this).find('#Id').val();
                            var container = $(this);
                            if (_static.fn.validateQuestionPoolDialog(editTitle, poolCount, container)) {
                                var callback = function () {
                                    $.post(
                                    PxPage.Routes.add_new_pool,
                                        {
                                            parentid: quizId,
                                            title: editTitle,
                                            poolCount: poolCount
                                        },
                                        function () {
                                            PxPage.Loaded('.question-pool-dialog-text');
                                            $('.question-pool-dialog-text').dialog("close");
                                            var callback = function () {
                                                $(PxPage.switchboard).trigger("questionpool-created");
                                            };
                                            $('.question-editor').questioneditor('updateQuizEditorQuestions', null, callback);
                                        }
                                    );
                                };

                                _static.fn.validateQuestionPoolTitles(quizId, editTitle, callback);

                            } else {
                                PxPage.Loaded('.question-pool-dialog-text');
                                return false;
                            }

                        },
                        "Cancel": function () {
                            $(this).dialog("close");
                        }
                    }
                });
                
            },
            
            editQuizAction: function (event, name, gearbox) {
                if (name == "importer") {
                    $(PxPage.switchboard).trigger("showimporterdialog.questionimport");
                    return false;
                }
                
                var editType = _static.fn.getQuizActionName(name);
                if (editType == "bank") {
                    //if the question bank is clicked from the RHS then just show the dialog
                    if ($('#fne-content .quiz-editor .available-questions').is(":visible")) {
                        //open a dialog box                    
                        _static.fn.createQuestionPoolDialog();

                    }
                    else {
                        //Edit Question Pool.
                        $('.question-editor').questioneditor('editQuestionPool', editType, null);
                    }
                }
                else {
                    $('#previous-question-type').val(editType);
                    $('.question-editor').questioneditor('openEditorForNewQuestions', editType);
                    //PxQuiz.OpenEditorForNewQuestions(editType);

                }
                event.preventDefault();

            },
            
            getQuizActionName: function (name) {

                var editType = "choice";
                switch (name) {
                    case "question-bank":
                        editType = "bank";
                        break;
                    case "multiple-choice":
                        editType = "choice";
                        break;
                    case "short-answer":
                        editType = "text";
                        break;
                    case "essay":
                        editType = "essay";
                        break;
                    case "matching":
                        editType = "match";
                        break;
                    case "multiple-answer":
                        editType = "answer";
                        break;
                    case "hts":
                        editType = "hts";
                        break;
                    case "graph":
                        editType = "graph";
                        break;
                }
                return editType;

            },

            selectAllQuestions: function (target) {
                PxQuiz.SetAllChecks(target, true);
                return false;
            },
            onClickSelectionGearbox: function (event, name, gearbox) {
                var target = $(gearbox).parents(".selected-questions, .available-questions, .quiz-overview");
                
                switch (name) {
                    case "select-all":
                        _static.fn.selectAllQuestions(target);
                        break;
                    case "select-none":
                        $('.question-list').questionlist('clearAllQuestions', target);
                        break;
                    case "select-used-previously":
                        _static.fn.selectUsedPreviously(target);
                        break;
                    case "select-never-used":
                        _static.fn.selectNeverUsed(target);
                        break;
                    case "select-random":

                        $('#divRandomSelect #spnNameError').hide();
                        $('#divRandomSelect #txtRandomSelect').val('');

                        $('.quiz-editor').block({
                            message: $("#divRandomSelect"),
                            css: {
                                padding: 0,
                                margin: 0,
                                top: '30%',
                                left: '30%',
                                right: '30%'
                            }
                        });
                        break;
                }
                if (event != null) {
                    event.preventDefault();
                }
            },
            validateQuestionPoolDialog: function (title, poolCount, container) {

                if (title == "") {
                    $(container).find('.pool_title_error').show();
                    $(container).find('#pool_title').focus();
                    return false;
                }
                else if (poolCount == "") {
                    $(container).find('#spn-pool-count-error').show();
                    $(container).find('#pool-count').focus();
                    return false;
                }
                else if (!poolCount.match('^(0|[1-9][0-9]*)$')) {
                    $(container).find('#spn-pool-integer-error').show();
                    $(container).find('#pool-count').focus();
                    return false;
                }
                else {
                    return true;
                }
            },
            validateQuestionPoolTitles: function (quizId, title, callback) {

                $.post(
                    PxPage.Routes.validate_pool_title,
                        {
                            parentid: quizId,
                            title: title
                        },
                        function (data) {
                            if (data == "True") {
                                PxPage.Toasts.Error("Pool already exist. Please choose another title.");
                                PxPage.Loaded('.question-pool-dialog-text');
                                return true;
                            } else {
                                if (callback) {
                                    callback();
                                }
                                return false;
                            }
                        }
                );
            },
            
            //validateQuestionPoolTitlesSaveDialog: function (event, quizId, title) {
            //    $.post(
            //        PxPage.Routes.validate_pool_title,
            //            {
            //                parentid: quizId,
            //                title: title
            //            },
            //            function (data) {
            //                if (data == "True") {
            //                    PxPage.Toasts.Error("Pool already exist. Please choose another title.");
            //                    $('#Title').focus();
            //                    event.preventDefault();
            //                    return false;
            //                }
            //                else {
            //                    PxQuiz.CloseDialog();
            //                    $('#spnIntegerError, #spnTitleError, #spnCountError').hide();
            //                    PxPage.Loading('.selected-questions');
            //                    $("#divAddPool form").unbind('#poolBtnQuizSu' +
            //                        'bmit').submit();
            //                }
            //            }
            //    );
            //},
            
            //onSaveQuestionPool: function (event) {
            //    $('#fne-window').removeClass("require-confirm");
            //    var title = $('#Title').val();
            //    var poolCount = $.trim($('#PoolCount').val());
            //    if (title == "") {
            //        $('#spnTitleError').show();
            //        $('#Title').focus();
            //        event.preventDefault();
            //        return false;
            //    }
            //    else if (poolCount == "" || poolCount == "0") {
            //        $('#spnCountError').show();
            //        $('#PoolCount').focus();
            //        event.preventDefault();
            //        return false;
            //    }
            //    else if (!poolCount.match('^(0|[1-9][0-9]*)$')) {
            //        $('#spnIntegerError').show();
            //        $('#PoolCount').focus();
            //        event.preventDefault();
            //        return false;
            //    }
            //    else {
            //        event.preventDefault();
            //        var quizId = $("#content-item-id").text();
            //        _static.fn.validateQuestionPoolTitlesSaveDialog(event, quizId, title);
            //    }
            //},
            
            //showNewPoolFne: function () {
            //    $('#Title').val("");
            //    $('#PoolCount').val("1");
            //    $('#spnIntegerError, #spnTitleError, #spnCountError').hide();
            //    $('.quiz-editor').block({
            //        message: $("#divAddPool"),
            //        css: {
            //            padding: 0,
            //            margin: 0,
            //            top: '30%',
            //            left: '35%',
            //            right: '35%'
            //        }
            //    });
            //    $('#poolBtnQuizSubmit').unbind().click(_static.fn.onSaveQuestionPool);
            //    $("#divAddPool #Title").die().live("keyup", function () { $('#divAddPool #spnTitleError').hide('slow'); });
            //},
            
            //onClickQuestionsMenu: function (event, action) {
            //    switch (action) {
            //        case "new-pool":
            //            _static.fn.ShowNewPoolFne();
            //            break;

            //        case "edit":
            //            var selectedQuestion = PxQuiz.cfind(event.target, '.question input[name=selected]:checked').closest('.question');
            //            if ($(selectedQuestion[0]).hasClass('bank')) {
            //                var s = $(selectedQuestion.find('.description .text')[0]).text();
            //                $('#EditTitle').val(s);
            //                $('#EditPoolCount').val(selectedQuestion.find('.questions-count').val());
            //                $('.divPopupContent .bank-count').val(selectedQuestion.find('.bank-question-count').text());
            //                $('.question-id').val(selectedQuestion.attr('id'));
            //                $('#spnEditTitleError').hide();
            //                $('#spnEditCountError').hide();
            //                $('#spnEditIntegerError').hide();
            //                if (ql.parents('.selected-questions').length) {
            //                    $('.quiz-editor').block({
            //                        message: $("#divEditQuiz"),
            //                        css: {
            //                            padding: 0,
            //                            margin: 0,
            //                            top: '30%',
            //                            left: '30%'
            //                        }
            //                    });
            //                }
            //                else {
            //                    $('body').block({
            //                        message: $("#divEditQuiz"),
            //                        css: {
            //                            padding: 0,
            //                            margin: 0,
            //                            top: '30%',
            //                            left: '30%'
            //                        }
            //                    });
            //                }
            //            }
            //            else {
            //                //var passEvent = ;
            //                $('.question-list').questioneditor('editQuestion', { target: selectedQuestion[0], preventDefault: function () { } });
            //                //($('#fne-content').is(':visible') ? PxQuiz.EditQuestion : PxQuiz.SummaryEditQuestion)(passEvent);
            //            }
            //            break;

            //        case "remove":
            //            // If we're being asked to remove a question, we need to figure out what the real
            //            // target of the removal is, since the event's target is actually the menu link.
            //            // It's going to be the selected item(s) in the same context as the gearbox.
            //            // This means we need to make sure that questions from two different contexts can't
            //            // be selected at the same time: selecting a question in a pool should deselect all
            //            // questions in the quiz containing that pool.
            //            if (confirm("Do you want to remove the selected questions?")) {
            //                var checked = PxQuiz.cfind(event.target, 'input[name=selected]:checked');
            //                var target = checked.closest('li.question');
            //                // changing the text of the bankcount
            //                var bankcount = $(PxQuiz.cfind(event.target, '.question input[name=selected]:checked').parents('.question')[1]).find('.bank-count');
            //                bankcount.text(bankcount.text() - 1);

            //                $('.question-list').questionlist('removeSelected', target);
            //                _static.fn.updateQuestionsMenu(event);
            //                $('.question-list').questionlist('modifyInUsedQuestions', $('div.select-menu'));
            //            }
            //            break;
            //    }
            //},
            
            selectUsedPreviously: function (target) {
                PxQuiz.SetAllChecks(target, false);
                PxQuiz.cfind(target, "ul.questions li.question").removeClass('active');
                PxQuiz.cfind(target, "ul.questions li.question").filter('.reused').addClass('active');
                PxQuiz.cfind(target, "ul.questions li.question").filter('.reused').find("input[name='selected']").prop('checked', true);
                return false;
            },

            selectNeverUsed: function (target) {
                PxQuiz.SetAllChecks(target, true);
                PxQuiz.cfind(target, "ul.questions li.question").filter('.reused').removeClass('active');
                PxQuiz.cfind(target, "ul.questions li.question").filter('.reused').find("input[name='selected']").prop('checked', false);
                return false;
            },
            selectRandomQuestion: function () {

                PxQuiz.SetAllChecks($('div.select-menu'), false);
                var target = PxQuiz.cfind($('div.select-menu'), ".questions li.question:not(.in-used-question) [name=selected], li.quiz-item [name=selected]");

                var randomvalue = $('#divRandomSelect #txtRandomSelect').val();
                if (randomvalue == "") {
                    $('#divRandomSelect #spnNameError').text("Please enter a number");
                    $('#divRandomSelect #spnNameError').show();
                }
                else if (!randomvalue.match('^(0|[1-9][0-9]*)$')) {
                    $('#divRandomSelect #spnNameError').text("Please enter a numeric value");
                    $('#divRandomSelect #spnNameError').show();
                }
                else if (randomvalue > target.length) {
                    $('#divRandomSelect #spnNameError').text("Enter a number lower than the total questions available");
                    $('#divRandomSelect #spnNameError').show();
                }
                else if (randomvalue == target.length) {
                    var areaTarget = $('#fne-content .available-questions');
                    PxQuiz.CloseDialog();
                    _static.fn.selectAllQuestions(areaTarget);
                }
                else {
                    $('#divRandomSelect #spnNameError').hide();

                    var list = [];
                    for (var init = 0; init < target.length; init++) {
                        list.push(init);
                    }
                    var num;
                    for (var val = 0; val < randomvalue; val++) {
                        num = _static.fn.randomFromTo(0, list.length - 1);
                        var randomNumber = list[num];
                        list = $.grep(list, function (value) {
                            return value != randomNumber;
                        });
                        $(target[randomNumber]).prop('checked', true);
                        $($(".quiz-editor .available-questions ul.questions li.question:not(.in-used-question)")[randomNumber]).addClass('active');

                        $($(".quiz-editor .available-questions ul.questions li.question:not(.in-used-question)")[randomNumber]).addClass('selected-question-in-quiz-editor');
                        $($(".quiz-editor .available-questions ul.questions li.question:not(.in-used-question) .preview-available-question")[randomNumber]).addClass('displayquestionmenu');
                        $($(".quiz-editor .available-questions ul.questions li.question:not(.in-used-question) .add-to-pool-available-question")[randomNumber]).addClass('displayquestionmenu');
                        $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').show();
                        $('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').show();
                    }
                    PxQuiz.CloseDialog();
                }
            },
            randomFromTo: function (from, to) {
                return Math.floor(Math.random() * (to - from + 1) + from);
            },
            //Update the menu above the available question list that specifies what buttons to show based on what options are selected
            updateQuestionsMenu: function (event) {
                //var ql = PxQuiz.cfind(event.target, '.question-list');
                //if (!ql || !ql.length) {
                //    return;
                //}
                
                //var selectedQuestions = ql.find('div.select > input[type=checkbox]:checked');

                //var options = [
                //    { name: "select-all", text: "Select1 All" },
                //    { name: "clear-all", text: "Clear1 All" },
                //    { name: "expand-all", text: "Expand1 All" },
                //    { name: "collapse-all", text: "Collaps1e All" }
                //];

                //// Only show the remove option is there are more than 0 quesions selected
                //if (selectedQuestions.length > 0) {
                //    options.unshift({ name: "remove", text: "Remove" });
                //}

                //// Only show the create new pool option if we're in selected questions
                //if (ql.parents('.selected-questions').length) {
                //    options.unshift({ name: "new-pool", text: "Create new question pool" });
                //}

                //// Only show the edit option if there is 1 question selected
                //if (selectedQuestions.length == 1) {
                //    options.push({ name: "edit", text: "Edit" });
                //}

                //// Find the gearbox we want to apply this to.  If we're in a question pool, then we
                //// want to grab the gearbox for the containing quiz.
                //var isInPool = !!ql.closest('.question-pool').length;
                //if (isInPool) {
                //    ql = ql.parent().closest('.question-list');
                //}
                //var gearbox = ql.find('.questions-menu');
                //gearbox.ActionWidget({
                //    menu: {
                //        id: "question-actions",
                //        options: options
                //    },
                //    action: _static.fn.onClickQuestionsMenu
                //});
            }//end of private functions
        }

    };

    var api = {
        init: function () {
            $(document).off('click', '#linkRandomCancel').on('click', '#linkRandomCancel', function () { PxQuiz.CloseDialog(); });
            $(document).off('click', '#btnRandomSave').on('click', '#btnRandomSave', _static.fn.selectRandomQuestion);

        },
        initQuestionGearbox: function () {
            var options = [{ name: 'add-to-quiz', text: 'Add' }];
            options.push({ name: 'add-to-new-pool', text: 'Add to new pool' });

            $('.selected-questions .question-list li.bank').each(function (i, v) {
                var id = v.id;
                options.push({ name: 'add-to-pool-' + id, text: 'Add to "' + $(v).find('.bank >.title .text').text() + '" pool' });
            });

            $('.add-menu').ActionWidget({
                menu: {
                    id: 'add-gearbox',
                    options: options
                },
                action: _static.fn.addGearboxAction
            });

        },

        initSelectionGearboxForAvailableQuestions: function () {
            // Add the selection gearbox for available questions
            $('.available-questions .select-menu').ActionWidget({
                menu: {
                    id: 'select-gearbox',
                    options: [{ name: 'select-all', text: 'All' },
                                { name: 'select-none', text: 'None' },
                                { name: 'select-used-previously', text: 'Used in previous quiz' },
                                { name: 'select-never-used', text: 'Never used before' },
                                { name: 'select-random', text: 'Randomly select x questions' }]
                },
                action: _static.fn.onClickSelectionGearbox
            });

            $('.quiz-overview .select-menu-current').ActionWidget({
                menu: {
                    id: 'select-gearbox-current',
                    options: [{ name: 'select-all', text: 'All' },
                                { name: 'select-none', text: 'None' }]
                },
                action: _static.fn.onClickSelectionGearbox
            });
        },
        showHideGearbox: function (isVisible) {
            if (isVisible && !$('.available-questions .select-menu:visible').length) {
                $('.available-questions .select-menu').show();                
            }
            else if(!isVisible) {
                $('.available-questions .select-menu').hide();
            }
        },
        updateAddMenu: function () {
            var options = [{ name: 'add-to-new-pool', text: 'Add to new pool' }];
            $('.selected-questions .question-list li.bank').each(function (i, v) {
                var id = v.id;
                options.push({ name: 'add-to-pool-' + id, text: 'Add to "' + $(v).find('.bank >.title .text').text() + '" pool' });
            });
            $('.available-questions .add-menu').ActionWidget({
                menu: {
                    id: 'add-gearbox',
                    options: options
                },
                action: _static.fn.addGearboxAction
            });

            options.unshift({ name: 'add-to-quiz', text: 'Add' }); // Add questions to assessment option for dropdown menu
            
            $(".add-to-pool-available-question").ActionWidget({ 
                menu: {
                    id: 'add-gearbox',
                    options: options
                },
                action: _static.fn.addGearboxAction
            });
          
            var moveOptions = [{ name: 'move-to-top', text: 'Top' }];
            
            moveOptions.push({ name: 'move-to-bottom', text: 'Bottom' });
            $('.move-current-question').ActionWidget({
                menu: {
                    id: 'add-gearbox',
                    options: moveOptions
                },
                action: _static.fn.addGearboxAction
            });
        },
        updateNewMenu: function () {
            var options = [{ name: 'question-bank', text: 'Question Pool' }];
            options.push({ name: 'multiple-choice', text: 'Multiple Choice' });
            options.push({ name: 'short-answer', text: 'Short Answer' });
            options.push({ name: 'essay', text: 'Essay' });
            options.push({ name: 'matching', text: 'Matching' });
            options.push({ name: 'multiple-answer', text: 'Multiple Answer' });
            options.push({ name: 'hts', text: 'Advanced Question' });
            options.push({ name: 'graph', text: 'Graphing Exercise' });
            options.push({ name: 'importer', text: 'Import Questions' });

            $('.selected-questions .new-question-menu').ActionWidget({
                menu: {
                    id: 'open-editor',
                    options: options
                },
                action: _static.fn.editQuizAction
            });
        },
        //Update the menu above the available question list that specifies what buttons to show based on what options are selected
        updateQuestionsMenu: function (event) {
            //_static.fn.updateQuestionsMenu(event);
        }//end of public functions
    };
    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.questionListGearbox = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

}(jQuery));