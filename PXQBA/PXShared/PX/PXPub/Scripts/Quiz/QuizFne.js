//jQuery plugin for Quiz FNE window
(function ($) {
    var _static = {
        pluginName: "quizFne",
        dataKey: "quizFne",
        bindSuffix: ".quizFne",
        dataAttrPrefix: "data-qf-",
        //selectors for commonly accessed elements
        sel: {
            
        },
        //private functions
        fn: {
            fneResize: function() {
                var matches, container, containerHeight, offsetTop, titleHeight, buttonHeight;
                titleHeight = $("#fne-title").outerHeight();

                $('div.quiz-editor-questions, div.expandable-toc').height($('div.fne-content').innerHeight());
                $('.quiz-editor .selected-questions .questions').css('min-height', $('.quiz-editor .selected-questions').innerHeight() - $('.question-bank-header-right').innerHeight() - 20);
                var poolQuestionsBlock = $('.quiz-editor .selected-questions .question-pool .questions');
                poolQuestionsBlock.css('height', '').css('min-height', '100px');
                poolQuestionsBlock.addClass('is-question-pool');

                matches = $(".quiz-editor .available-questions");
                if (matches !== null && matches.length > 0) {
                    container = matches[0];
                    if (container !== null) {
                        containerHeight = container.offsetHeight;
                        matches = $(".quiz-editor .toc");
                        if (matches.length > 0) {
                            offsetTop = $(".quiz-editor .toc")[0].offsetTop;
                        }
                        $(".quiz-editor .toc").height(containerHeight - (offsetTop - titleHeight));
                    }
                }

                $('#fne-window .question-pool-inner-container').width(
                    $('#fne-content').width() - $('.selected-questions').outerWidth(true) - 3
                );
                $('#fne-window .edit-question').width(
                    $('#fne-content').width() - $('.selected-questions').outerWidth(true) - 3
                );
            },
            showFneResize: function() {
                $('.show-quiz .gradebook-component').height(
                    $('#fne-content').height() -
                        $('.show-quiz .info').outerHeight(true)
                );
            },
            showFneInit: function () {
                $(".show-quiz button.collapse, .show-quiz button.expand").unbind('click');
                $(".show-quiz button.collapse").click(function () {

                    $('.show-quiz .description, .show-quiz button.collapse').hide();
                    $('.show-quiz button.expand').show();
                    _static.fn.showFneResize();
                });
                $(".show-quiz button.expand").click(function () {
                    $('.show-quiz .description, .show-quiz button.collapse').show();
                    $('.show-quiz button.expand').hide();
                    _static.fn.showFneResize();
                });

                $(document).off('click', ".ui-widget-overlay").on("click", ".ui-widget-overlay", function (event) {
                    event.stopImmediatePropagation();
                    $("#quizDirectionsModal").dialog("close");
                });

                $(document).off('click', "#quizDirectionsModal").on("click", "#quizDirectionsModal", function (event) {
                    event.stopImmediatePropagation();
                    $("#quizDirectionsModal").dialog("close");
                });

                $('#fne-content').addClass('fne-scrollbars');
                _static.fn.showFneResize();
            },
            overviewFneInit: function() {
                $('.quiz-overview').parents('#fne-content #content-item').css('padding-right', '10px');
                $('.quiz-overview').parents('#fne-content #content-item').find('.questions.ui-sortable').sortable("destroy");
            },
            fneInit: function() {
                PxQuiz.updatedTimes = 0;
                
                $('.selected-questions .new-question').unbind('click').click(_static.fn.createNewQuestion);
                
                $('.available-questions .add-available-question-at-top').unbind('click').click(function () {
                    $(this).block({ message: null });
                    $('.question-list').questionlist('addSelected', { target: $('.questions-actions > .add-quiestion-btn-wrapper > .add-available-question-at-top') });
                    $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').hide();
                    $('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').hide();
                    PxPage.Loaded($(this));
                });

                $(document).off('keypress', '#txtPoolCount, #txtPoolName, #txtPoolPoints').on('keypress', '#txtPoolCount, #txtPoolName, #txtPoolPoints', _static.fn.onQuestionPoolKeyPress);
                $(document).off('click', '#txtPoolCount, #txtPoolName, #txtPoolPoints').on('click', '#txtPoolCount, #txtPoolName, #txtPoolPoints', function () { return false; });
                $(document).off('focusout', '#txtPoolCount, #txtPoolName, #txtPoolPoints').on('focusout', '#txtPoolCount, #txtPoolName, #txtPoolPoints', function () { return false; });
				
                PxQuiz.LoadDefaultBreadcrumbTrail();
                _static.fn.loadDefaultPage();
                $(".default-breadcrumb .default-path-item").unbind('click').click(_static.fn.loadDefaultPage);

                if (typeof ContentWidget != 'undefined') {
                    $(PxPage.switchboard).unbind("breadcrumb.selectionChanged", ContentWidget.BreadcrumbChanged);
                }
                $(PxPage.switchboard).unbind("breadcrumb.selectionChanged").bind("breadcrumb.selectionChanged", function (event, itemid) {
                    PxQuiz.UpdateChildList(itemid);
                });

                $('.question-list .questions-menu').show();
                $(PxPage.switchboard).trigger("availablequestionsupdated");

                $(".question-list").questionListGearbox('initSelectionGearboxForAvailableQuestions');

                $(".question-list").questionListGearbox('updateNewMenu');

                $('#fne-content').find('#lnkPoints').bind('click', function (event) {
                    event.preventDefault();
                    return false;
                });

                // Make the divider between the left and right sides resizable
                $('#fne-content .selected-questions').resizable({
                    alsoResizeReverse: '#fne-content .available-questions',
                    maxWidth: $('#fne-window').width() - 300,
                    minWidth: 300
                });

            },
            onQuestionPoolKeyPress: function (event) {
                if (event.which == 13) {
                    $(".question-pool-inner-container .pool-actions .save").click();
                    event.preventDefault();
                }
            },
            createNewQuestion: function () {
                var previousType = $("#previous-question-type").val();
                if (previousType == null || $.trim(previousType) == "") {
                    previousType = "choice";
                }
                $('.question-editor').questioneditor('openEditorForNewQuestions', previousType);
                //PxQuiz.OpenEditorForNewQuestions(previousType)
            },
            loadDefaultPage: function (event) {
                $('.breadcrumb-trail .breadcrumb').hide();
                $('.default-path-item').text('Show All');
                $('.available-questions').block();
               

                //$('.question-search-list').hide();
                $('.children-list').show();
                PxPage.Loading($('.quiz-editor .children-list'));
                $('.quiz-editor .children-list').load(
                    PxPage.Routes.render_base_page,
                    function () {
                        PxPage.Loaded($('.quiz-editor .children-list'));
                        PxQuiz.PostChildLoadedProcessing();
                    }
                );
                PxBreadcrumb.RunResizers();
                if (event != null) {
                    event.preventDefault();
                }
            }
            //end of private functions
        }

    };

    var api = {
        bindFneHooks: function () {
            PxPage.FneInitHooks['quiz-editor'] = _static.fn.fneInit;
            PxPage.FneResizeHooks['quiz-editor'] = _static.fn.fneResize;
            PxPage.FneInitHooks['quiz-overview'] = _static.fn.overviewFneInit;
            PxPage.FneInitHooks['show-quiz'] = _static.fn.showFneInit;
            PxPage.FneResizeHooks['show-quiz'] = _static.fn.showFneResize;
        },
        fneResize: function() {
            _static.fn.fneResize();
        },
        showFneInit: function () {
            _static.fn.showFneInit();
        },
        overviewFneInit: function() {
            _static.fn.overviewFneInit();
        },
        fneInit: function() {
            _static.fn.fneInit();
        }//end of public functions
    };
    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.quizFne = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

}(jQuery));