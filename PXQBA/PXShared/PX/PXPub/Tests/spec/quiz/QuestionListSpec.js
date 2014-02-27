/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.6.4-vsdoc.js" />


/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jquery/jquery.js" />
/// <reference path="../../lib/jasmine-jquery.js" />
/// <reference path="../../lib/mock-ajax.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-beforeAll.js" />
/// <reference path="../../lib/blanket/blanket.min.js" />
/// <reference path="../../lib/blanket/jasmine-blanket.js" />
/// <reference path="../../lib/jquery/jquery-migrate.js" />
/// <reference path="../../../Scripts/Other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />

/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js"/>
/// <reference path="../../../Scripts/jquery/jquery.fauxtree.js"/>
/// <reference path="../../../Scripts/Handlebar/Handlebar.js"/>
/// <reference path="../../../Scripts/LAB.src.js"/>
/// <reference path="../../../Scripts/jquery/modernizr.custom.js"/>

/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />
/// <reference path="../../../Scripts/Common/PxPage.AjaxForm.js"/>
/// <reference path="../../../Scripts/Faceplate/PxFacePlate.js" />
/// <reference path="../../../Scripts/Quiz/Quiz.js" />
/// <reference path="../../../Scripts/Rangy/rangy-core.js" />
/// <reference path="../../../Scripts/Quiz/QuizQuestionList.js" />
/// <reference path="../../../Scripts/ContentTreeWidget/ContentTreeWidget.js" />
/// <reference path="../../runners/quiz/helpers/QuizResponses.js" />

 

describe("Quizzing", function () {
    var request;
    var onSuccess, onFailure;
    var originalQuestionListGearbox = $.fn.questionListGearbox;
    var originalQuestionEditor = $.fn.questioneditor;
    var originalContentTreeWidget = $.fn.ContentTreeWidget;
    beforeAll(function () {
        
        jasmine.getFixtures().fixturesPath = '../../fixtures/html';
        jasmine.getStyleFixtures().fixturesPath = '../../fixtures/style';
        
        originalContentTreeWidget = $.fn.ContentTreeWidget;
        originalQuestionListGearbox = $.fn.questionListGearbox;
        originalQuestionEditor = $.fn.questioneditor;

        $.fn.questionListGearbox = jasmine.createSpy("questionListGearbox Spy");
        $.fn.questioneditor = jasmine.createSpy("questioneditor");
        $.fn.ContentTreeWidget = jasmine.createSpy("ContentTreeWidget");
        PxContentTemplates = jasmine.createSpyObj('PxFacePlate', ['onItemFromTemplate']);

        PxPage = jasmine.createSpyObj('PxPage', ['Loading', 'Loaded', 'Routes', 'log', 'Toasts', 'FneInitHooks', 'FneLoadedHooks', 'Fade', 'SearchTermOperation']);
        PxQuiz = jasmine.createSpyObj('PxQuiz', ['isProcessing', 'UpdateAvailableQuestionsMenu', 'cfind', 'SetAllChecks',
                                                'FneResize', 'UpdateQuestionsMenu', 'MarkQuestionActive', 'UpdateAddedQuestions',
                                                'UpdateSelectedQuestionsMenu']);
        

        PxQuiz.cfind.andCallFake(function (target, selector) {
            var classes = '.selected-questions, .available-questions, .quiz-overview, .question-pool';
            var lookingAt = $(target);
            if (selector == 'test')
                return $('<div id="test"></div>');
            for (var l = 0; l < 100; l++) {
                if (lookingAt.is(classes)) {
                    return lookingAt.find(selector);

                } else {
                    lookingAt = lookingAt.parent();
                }

            }
        });

        PxQuiz.SetAllChecks.andCallFake(function (target, value) {
            target = $(target);
            if (target.length > 0) {
                target.find("ul.questions:visible li.question [name=selected], li.quiz-item:visible [name=selected]").prop('checked', value);
                target.find("ul.questions:visible li.question").removeClass("selected-question-in-quiz-editor");
                target.find("ul.questions:visible li.question .preview-available-question").removeClass("displayquestionmenu");
                target.find("ul.questions:visible li.question .add-to-pool-available-question").removeClass("displayquestionmenu");
                $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').hide();
                $('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').hide();

                target.find("ul.questions li.question").removeClass('active');
                if (value) {
                    target.find("ul.questions li.question").addClass('active');
                }
            }

        });

        PxPage.Toasts.Error = jasmine.createSpy(PxPage, "Error");
        PxPage.Toasts.Warning = jasmine.createSpy(PxPage, "Warning");
        PxPage.Toasts.Success = jasmine.createSpy(PxPage, "Success");


        PxPage.Routes.save_points = "savepoints.html";
        PxPage.Routes.quiz_question_settings = "quiz_question_settings.html";
        PxPage.Routes.add_question_to_quiz = "addquestiontoquiz.html";
        PxPage.Routes.question_settings = "question_settings.html";
        PxPage.Routes.get_mainpoollist = "get_mainpoollist.html";
        PxPage.Routes.get_poolquestionlist = "get_poolquestionlist.html";
        PxPage.Routes.custom_inline_preview = "inline_preview.html";
        PxPage.Routes.question_order = "question_order.html";
        PxPage.Routes.render_children = "render_children.html";
        PxPage.Routes.show_add_question_to_existing_assessment = "show_add_question_to_existing_assessment.html";
        PxPage.Routes.show_add_to_new_assessment = "show_add_to_new_assessment.html";
        PxPage.Routes.update_questions_meta = "update_questions_meta.html";
        PxPage.switchboard = "#main";
        
        }
    );

    afterAll(function () {
        $.fn.questionListGearbox = originalQuestionListGearbox;
        $.fn.questioneditor = originalQuestionEditor;
        $.fn.ContentTreeWidget = originalContentTreeWidget;
        $("#edit-settings-dialog2").remove();
        $('.blockUI').remove();
    });
    beforeEach(function () {
        
       
    });
    
    describe("questionlist ", function () {
        var btnHideQuestionPool = ".hide-current-questions:first";
        var btnShowQuestionPoolQuestions = ".show-current-questions:first";
        var btnEditSelectedQuestionSettings = ".various-actions .edit-settings";
        var btnExpandAll = ".various-actions .expandAll";
        var btnCollapseAll = ".various-actions .collapseAll";

        beforeEach(function () {
            loadFixtures('questionlists.html');
            jasmine.Ajax.useMock();
            onSuccess = jasmine.createSpy('onSuccess');
            onFailure = jasmine.createSpy('onFailure');
        });

        afterEach(function () {
            jasmine.Fixtures.prototype.cleanUp();
            $('#undefined').remove();
            $('.ui-widget-overlay').remove();
            $('.ui-dialog').remove();
            $('.question-dialog-text').remove();
        });
        
        it("should be sorted", function() {
            //TODO
            loadFixtures('questionlists.html');
            $('.question-list').questionlist('');
            var sortableItem = $(".question-display:first").find('.questions li.question:first');
            PxQuiz.updatedTimes = 0;
            $(".question-display:first").find('.questions').trigger('sortupdate', { ui: { sender: null }, item: sortableItem });
            
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.question_order);
            expect(request.method).toBe('POST');
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();
            expect(PxQuiz.UpdateSelectedQuestionsMenu).toHaveBeenCalled();

        });
        
        it("should mark the question to be dragged as selected", function () {            
            
            var fixture = "<div id='main'>" + readFixtures('questionlists.html') + "</div>";
            var draggingmessage = "";
            expect(draggingmessage).toEqual("");
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            
            var target = $('li.question:first');
            spyOn($.fn, "draggable").andCallFake(function (arg) {
                if (arg.helper) {
                    draggingmessage = arg.helper({ target: target });
                }
            });
            
            $('.question-list').questionlist('');

            expect(draggingmessage).toHaveClass('quiz-dragging-helper');

        });
        

        
        it("should throw error if plugin method not found", function () {
            var fixture = "<div id='main'></div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            $('#undefined').remove();
            var shouldFailFn = function () {
                $('#main').questionlist('dummyMethod');
            };
            expect(shouldFailFn).toThrow(new Error('Method dummyMethod does not exist on jQuery.questionlist'));

        });
        
        it("should be able to trigger the questionlistupdated.questionlist updated event", function () {
            
            var fixture = "<div id='main'>" + readFixtures('questionlists.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            $('.question-list').questionlist('');
            $(PxPage.switchboard).trigger("questionlistupdated.questionlist");

            expect(PxQuiz.FneResize).toHaveBeenCalled();
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();
            expect(PxPage.Loaded).toHaveBeenCalled();

        });
        
        
        
        it("should be able to initialize the FneHooks for quiz-editor", function () {

            var fixture = "<div id='main'>" + readFixtures('questionlists.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            
            $('.question-list').questionlist('');
            $(PxPage.switchboard).trigger("questionlistupdated.questionlist");

            expect(PxQuiz.FneResize).toHaveBeenCalled();
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();
            expect(PxPage.Loaded).toHaveBeenCalled();
            expect(PxPage.FneInitHooks['quiz-editor']).toBeDefined();
        });
        
        it("should should show the correct description if no questions exist in the quiz", function () {
            
            var parentContainer = $('li.question:first').closest('.question-list');
            $(parentContainer).find('li.question').remove();
            $('.question-list').questionlist('checkForNoQuestionsLeft', parentContainer);
            expect(PxPage.Loaded).toHaveBeenCalled();
            expect($(parentContainer)).toBeHidden();
            expect($(parentContainer).parent().find('.overview-no-questions')).toBeVisible();

        });
       
        it("should be able to hide question pools", function () {

      
            var questionBankToHide = $(btnHideQuestionPool).closest('li.question.bank');
            $(questionBankToHide).addClass('bank-expanded');
            
            $('.question-list').questionlist('');
            $(btnHideQuestionPool).trigger('click');

            expect($(questionBankToHide).find('.show-current-questions')).not.toBeHidden();
            expect($(questionBankToHide).find('.hide-current-questions')).toBeHidden();
            expect($(questionBankToHide).find('.question-pool')).toBeEmpty();
            expect($(questionBankToHide)).not.toHaveClass('bank-expanded');
            
        });

        it("should be able to bind the fne hooks", function () {
            $('.question-list').questionlist('');
            expect(PxPage.FneInitHooks['quiz-editor']).toBeDefined();
            PxPage.FneInitHooks['quiz-editor']();
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();
        });
        
        it("should be able to bind the fne hooks", function () {
            $('.question-list').questionlist('');
            expect(PxPage.FneInitHooks['quiz-editor']).toBeDefined();
            PxPage.FneInitHooks['quiz-editor']();
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();
        });

        
        it("should call UpdateQuestionMenu after hiding question pools", function () {
            
           
            $('.question-list').questionlist('');
            $(btnHideQuestionPool).trigger('click');
            
            expect($('.question-list').questionListGearbox).toHaveBeenCalled();
            
        });
        
        it("should be able to show question pool questions", function () {

       
            var questionBankToShow = $(btnShowQuestionPoolQuestions).closest('li.question.bank');
            $(questionBankToShow).removeClass('bank-expanded');

            $('.question-list').questionlist('');
            $(btnShowQuestionPoolQuestions).trigger('click');

            expect($(questionBankToShow).find('.show-current-questions')).toBeHidden();
            expect($(questionBankToShow).find('.hide-current-questions')).not.toBeHidden();
            expect($(questionBankToShow).find('.question-pool')).not.toBeEmpty();
            expect($(questionBankToShow)).toHaveClass('bank-expanded');
            
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);
            
            expect(request.url).toBe(PxPage.Routes.get_mainpoollist);
            expect(request.method).toBe('POST');
            
        });
       

        it("should be able to open 'edit selected question settings' dialog", function() {

            expect($('.ui-dialog').length).toEqual(0);
            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');
            

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);

            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('GET');
            expect($('#edit-settings-dialog2.ui-dialog-content').length).toEqual(1);
            
        });
        
        it("should be able to open 'edit selected question settings' dialog from hts editor", function () {

            expect($('.ui-dialog').length).toEqual(0);
            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $('.question-list').questionlist('editQuestionSettings', '', '68C8CC49582AB2AAA2DE51DC170402C8','51280');
            

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);

            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('GET');
            expect($('#edit-settings-dialog.ui-dialog-content').length).toEqual(1);
            $("#edit-settings-dialog").dialog("close");

        });
        
        //updating the points from question settings dialog.
        it("should be able to edit and save selected question settings", function () {
            
            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);
            expect($.trim($(questionId).find(".point-label").text())).toEqual($.trim("1  pt"));
            
            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');
            
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);
            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('GET');
            
            var dialog = '#edit-settings-dialog2.ui-dialog-content';
            var saveButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            $(saveButton).trigger('click');
            
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);
            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('POST');
            
            
            expect(PxPage.Loading).toHaveBeenCalled();
            expect(PxPage.Loaded).toHaveBeenCalled();
            
            expect($(questionId).find(".point-label").text().trim()).toEqual("3 pts");
            expect($(questionId).find('.questions-points-original').val()).toEqual('3');
            

        });
        
        //updating the points from question settings dialog.
        it("should be able to save the question settings on pressing enter key on the 'question settings dialog'", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);
            expect($.trim($(questionId).find(".point-label").text())).toEqual($.trim("1  pt"));

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);
            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('GET');

            var e = $.Event("keypress");
            e.keyCode = 13;
            e.which = 13; // # Some key code value
            $("#edit-settings-dialog2").trigger(e);

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);
            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('POST');


            expect(PxPage.Loading).toHaveBeenCalled();
            expect(PxPage.Loaded).toHaveBeenCalled();

            expect($(questionId).find(".point-label").text().trim()).toEqual("3 pts");
            expect($(questionId).find('.questions-points-original').val()).toEqual('3');


        });
        
        
        it("should throw warning if none of the questions are selected for editing", function () {

  
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            expect(PxPage.Toasts.Warning).toHaveBeenCalled();

        });
        
        it("should throw warning if the points being edited is greater than 100", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);
            expect($.trim($(questionId).find(".point-label").text())).toEqual($.trim("1  pt"));

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);
            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('GET');

            $("#Points").val('101');
            
            var dialog = '#edit-settings-dialog2.ui-dialog-content';
            var saveButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            $(saveButton).trigger('click');

            request = mostRecentAjaxRequest();
            expect(request.method).not.toBe('POST');

            expect(PxPage.Toasts.Error).toHaveBeenCalled();


        });
        
        it("should throw warning if the points being edited is less than 0 ", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);
            expect($.trim($(questionId).find(".point-label").text())).toEqual($.trim("1  pt"));

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);
            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('GET');

            $("#Points").val('-1');

            var dialog = '#edit-settings-dialog2.ui-dialog-content';
            var saveButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            $(saveButton).trigger('click');

            request = mostRecentAjaxRequest();
            expect(request.method).not.toBe('POST');

            expect(PxPage.Toasts.Error).toHaveBeenCalled();


        });
        
        it("should be able to close the 'edit question settings' dialog using cancel button", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);
            expect(request.url).toContain(PxPage.Routes.quiz_question_settings);
            expect(request.method).toBe('GET');
            
            var dialog = '#edit-settings-dialog2.ui-dialog-content';
            var cancelButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:last');
            $(cancelButton).trigger('click');

            expect($('#edit-settings-dialog2.ui-dialog-content')).toBeHidden();

        });
        
        it("should be able to expand all questions", function () {
            var fixture = "<div id='main'>" + readFixtures('questionlists.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            
            

            var questionBankToShow = $(btnShowQuestionPoolQuestions).closest('li.question.bank');
            $(questionBankToShow).removeClass('bank-expanded');
            
            $('.question-list').questionlist('');


            $(btnExpandAll).trigger('click');
            
            expect($(questionBankToShow).find('.show-current-questions')).toBeHidden();
            expect($(questionBankToShow).find('.hide-current-questions')).not.toBeHidden();
            expect($(questionBankToShow).find('.question-pool')).not.toBeEmpty();
            expect($(questionBankToShow)).toHaveClass('bank-expanded');
            
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.get_mainpoollist);
            expect(request.method).toBe('POST');
            

        });
        
        it("should be able to collapse all questions", function () {

   
            var questionBankToHide = $(btnHideQuestionPool).closest('li.question.bank');
            $(questionBankToHide).addClass('bank-expanded');
            $(questionBankToHide).find('.show-current-questions').hide();
            $(questionBankToHide).find('.hide-current-questions').show();
            $('.questions li.question .expand-current-question').text('Collapse');
            $('.questions li.question .expand-current-question').closest(".description").find(".question-text").removeClass('collapsed');
            
           
            $('.question-list').questionlist('');
            $(btnCollapseAll).trigger('click');

            expect($(questionBankToHide).find('.show-current-questions')).not.toBeHidden();
            expect($(questionBankToHide).find('.hide-current-questions')).toBeHidden();
            expect($(questionBankToHide).find('.question-pool')).toBeEmpty();
            expect($(questionBankToHide)).not.toHaveClass('bank-expanded');
            expect($('.questions li.question .expand-current-question').closest(".description").find(".question-text")).toHaveClass('collapsed');
            
        });

        describe("questions", function () { 

            var selectLink = ".table-layout .question:first a.expand-link";
            var selectQuestionCheckbox = ".question input[name=selected]";
            var btnExpandQuestion = ".expand-current-question";
            var btnOverviewEdit = ".quiz-overview .edit-current-question:first";
            var savePoints = "li.question .questions-points:first";
            beforeAll(function() {
                PxQuiz.MarkQuestionActive = jasmine.createSpy("MarkQuestionActive Spy");
            });
            beforeEach(function() {
                loadFixtures('questionlists.html');
                $('.question-list').questionlist('');
            });

            afterEach(function () {
                jasmine.Fixtures.prototype.cleanUp();
                $('.ui-widget-overlay').remove();
                $('.ui-dialog').remove();
                $('.question-dialog-text').remove();
                $('.add-to-new-assessment-dialog').remove();
                $("#px-dialog").remove();
            });

            it("should close questioncard on opening of the use in new assessment popup", function () {
                $(".preview-current-question:first").trigger('click');
                $('.reused').trigger('click');
                $('.add-to-new-assessment:visible').trigger('click');

                expect($('.questioncard').length == 0).toBeTruthy();
            });

            it("should close questioncard on opening of the use in existing assessment popup", function () {
                $(".preview-current-question:first").trigger('click');
                $('.reused').trigger('click');
                $('.add-to-existing-quiz:visible').trigger('click');

                expect($('.questioncard').length == 0).toBeTruthy();
            });

            it("should be able to edit the points", function () {

                var originalPointValue = '10';
               
                $('ul.questions li.question:first .total-points .questions-points-original').val(originalPointValue);
                
                $('ul.questions li.question:first .total-points .point-label').trigger('click');

                expect($('ul.questions li.question:first .total-points .questions-points').val()).toEqual(originalPointValue);

            });

            it("should hide the point label when editing the points", function() {

                var originalPointValue = '10';
                
                $('ul.questions li.question:first .total-points .questions-points-original').val(originalPointValue);

                $('ul.questions li.question:first .total-points .point-label').trigger('click');

                expect($('ul.questions li.question:first .total-points .point-label')).toBeHidden();
                

            });
            
            it("should show the point texbox when editing the points", function () {

                var originalPointValue = '10';
                
                $('ul.questions li.question:first .total-points .questions-points-original').val(originalPointValue);

                $('ul.questions li.question:first .total-points .point-label').trigger('click');

                expect($('ul.questions li.question:first .total-points .point-textbox')).toBeVisible();
                

            });
            
            it("should be able to save points", function () {

                var originalPointValue = '10';

                $('ul.questions li.question:first .total-points .questions-points-original').val(originalPointValue);

                $(savePoints).trigger('focusout');

                expect(PxPage.Loading).toHaveBeenCalled();

                request = mostRecentAjaxRequest();
                request.responseText = 'html';
                request.response(QuizResponses.points.success);

                expect(request.url).toBe(PxPage.Routes.save_points);
                expect(request.method).toBe('POST');
                

                expect($(savePoints).closest(".description, .total-points").find(".point-label").text().trim()).toEqual("1 pt");
                expect($(savePoints).closest(".total-points .point-textbox").find('.questions-points-original').val()).toEqual('1');
                expect($(savePoints).closest(".description, .total-points").find(".point-textbox")).toBeHidden();
                

            });
            

            it("should not call ajax request if the points edited is same as original point", function () {

                var originalPointValue = '1';

                $('ul.questions li.question:first .total-points .questions-points-original').val(originalPointValue);
                
                $(savePoints).trigger('focusout');

                request = mostRecentAjaxRequest();
                request.response(QuizResponses.questions.success);

                expect(request.args).toBeUndefined();
               


            });
            
            it("should return error if the points is greater than 100", function () {
                

                $(savePoints).val('101');

                $(savePoints).trigger('focusout');

                expect(PxPage.Toasts.Error).toHaveBeenCalled();


            });
            
            it("should return error if the points is less than 0", function () {


                $(savePoints).val('-1');

                $(savePoints).trigger('focusout');

                expect(PxPage.Toasts.Error).toHaveBeenCalled();


            });


            
            it("should be able to expand ", function () {
                
                $(btnExpandQuestion).closest(".description").find(".question-text").addClass('collapsed');

                $(btnExpandQuestion).trigger('click');

                expect($(btnExpandQuestion).closest(".description").find(".question-text")).not.toHaveClass('collapsed');

            });
            
            it("should be able to expand for custom questions  ", function () {

                // setup - clone some question and tweak it into custom type
                var qid = "#itCustomExpand";
                var someQuestion = $(btnExpandQuestion).closest(".questions .question")[0];
                var customQuestion = $(someQuestion).clone();
                var btnExpand = $(customQuestion, btnExpandQuestion);
                customQuestion.attr("id", 'itCustomExpand');
                
                $(".question-type", customQuestion).val('CUSTOM');
                $(".question-custom-url",customQuestion).val('HTS');
                $(".question-id", customQuestion).val('1111');
                $(".question-entity-id", customQuestion).val('8888');
                $(".question-text", customQuestion).addClass('collapsed');
                var expandDiv = $('<div id="custom_preview_1111" class="custom_preview custom_preview_1111 custom_inline_preview"></div>');
                customQuestion.append(expandDiv);

                // add to dom and test inputs
                customQuestion.appendTo($(".questions")[0]);
                expect($(qid)).toExist();
                //jasmine.log($(qid + " .question-type").val());
                //jasmine.log($(qid + " .question-custom-url").val());
                //jasmine.log($(qid + " .question-id").val());
                //jasmine.log($(qid + " .question-entity-id").val());
                
              
                // test expand function - should open in iframe
                $(btnExpandQuestion).trigger('click');
                var iFrame = $(qid + " iframe.question_preview_wrap");
                expect(iFrame).toExist();
                var expectedUrl = PxPage.Routes.custom_inline_preview +
                    "?questionId="  + $(qid + " .question-id").val()
                    + "&customUrl=" + $(qid + " .question-custom-url").val()
                    + "&entityid="  + $(qid + " .question-entity-id").val();
                expect (iFrame.attr("src")).toEqual(expectedUrl);
                
                // cleanup
                $(qid).remove();
            });

            it("should be able to collapse", function() {

                $(btnExpandQuestion).closest(".description").find(".question-text").removeClass('collapsed');

                $(btnExpandQuestion).trigger('click');

                expect($(btnExpandQuestion).closest(".description").find(".question-text")).toHaveClass('collapsed');


            });
            

            it("should call questionListGearbox.UpdateQuestionsMenu when the question is selected", function () {

                $(selectLink).trigger('click');

                expect($.fn.questionListGearbox).toHaveBeenCalled();


            });
            
            it("should call questionListGearbox.UpdateQuestionsMenu when the checkbox of question is selected", function () {

                $(selectQuestionCheckbox).trigger('click');

                expect($.fn.questionListGearbox).toHaveBeenCalled();


            });
            
            it("should call PXQuiz.MarkQuestionActive when the checkbox of question is selected", function () {

                $(selectQuestionCheckbox).trigger('click');

                expect(PxQuiz.MarkQuestionActive).toHaveBeenCalled();


            });

            it ("should remove the class require-confirm from the fne window when the checkbox of question is selected", function () {
                
                var fixture = "<div id='fne-window' class='require-confirm'>" + readFixtures('questionlists.html') + "</div>" ;
                jasmine.Fixtures.prototype.addToContainer_(fixture);
                $('.question-list').questionlist('');
                expect($("#fne-window")).toHaveClass("require-confirm");
                
                jasmine.Clock.useMock();
                $(selectQuestionCheckbox).trigger('click');
                jasmine.Clock.tick(501);
                
                expect($("#fne-window")).not.toHaveClass("require-confirm");

                
            }) ;
            
           
            
            it("should display the question menu when selected", function () {

                var question = $(selectLink).closest('li.question');

                $(selectLink).trigger('click');

                expect($(question).find(".preview-current-question")).toHaveClass('displayquestionmenu');
                expect($(question).find(".edit-current-question")).toHaveClass('displayquestionmenu');
                expect($(question).find(".delete-current-question")).toHaveClass('displayquestionmenu');
                


            });
            
            it("should make the question as active when selected ", function () {

                
                var question = $(selectLink).closest('li.question');
                
                $(selectLink).trigger('click');

                expect($(question)).toHaveClass('selected-question-in-quiz-editor');
                expect($(question)).toHaveClass('active');
                

            });
            
            it("should revert the active question as inactive when selected again", function () {

                
                var question = $(selectLink).closest('li.question');
                $(question).addClass('selected-question-in-quiz-editor');

                $(selectLink).trigger('click');

                expect($(question)).not.toHaveClass('selected-question-in-quiz-editor');
                

            });
            

            it("should show the settings link when selected", function() {
                
                var question = $(selectLink).closest('li.question');
                
                $(selectLink).trigger('click');
                
                expect($(question).closest('.quiz-overview').find('.various-actions .edit-settings')).toBeVisible();
                

            });
            
            it("should show the menu settings link when selected", function () {

  
                var question = $(selectLink).closest('li.question');

                $(selectLink).trigger('click');

                expect($(question).closest('.quiz-overview').find('.various-actions .edit-settings')).toBeVisible();


            });
            
            xit("should show the menu settings link when the child questions are selected", function () {
                var question = $(selectLink).closest('li.question');

                PxQuiz.cfind.andCallFake(function (target, selector) {
                    return question;
                });

                $(question).addClass('selected-question-in-quiz-editor');

                $(selectLink).trigger('click');

                expect($(question)).not.toHaveClass('selected-question-in-quiz-editor');
                


            });
            
            it("should hide the settings link when unselected ", function () {
                
                var question = $(selectLink).closest('li.question');
                $(question).addClass('selected-question-in-quiz-editor');

                $(selectLink).trigger('click');
                
                expect($(question).closest('.quiz-overview').find('.various-actions .edit-settings')).toBeHidden();
                

            });
            
            it("should show the settings link when selected", function () {
                
                var question = $(selectLink).closest('li.question');
                $(question).find('input[name=selected]').prop('checked', true);
                $(selectLink).trigger('click');

                
                expect($(question).closest('.quiz-overview').find('.various-actions .edit-settings')).toBeVisible();
                

            });
            
            it("should be able to edit the overview question", function () {

                $(btnOverviewEdit).trigger('click');
                
                expect(PxPage.Loading).toHaveBeenCalled();
                expect(PxPage.FneLoadedHooks["quiz-editor"]).toBeDefined();


            });
            
            it("should open the question editor once the fne loaded hooks are called", function () {

                $(btnOverviewEdit).trigger('click');
                PxPage.FneLoadedHooks["quiz-editor"]();
                
                expect(PxPage.FneLoadedHooks["quiz-editor"]).toBeNull();
                expect(PxPage.Loaded).toHaveBeenCalled();


            });
            
            it("should be able to see the preview of the selected questions", function () {
                var btnShowPreview = ".preview-current-question:first";
                
                $(btnShowPreview).trigger('click');

                expect($("#show-question.ui-dialog-content").length).toBeGreaterThan(0);
                

            });
            
            it("should be able to edit the question using the preview dialog", function () {
                var btnShowPreview = ".preview-current-question:first";

                $(btnShowPreview).trigger('click');

                expect($("#show-question.ui-dialog-content").length).toBeGreaterThan(0);
                
                var dialog = '.ui-dialog-content.question-dialog-text';
                var editButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
                $(editButton).trigger('click');

                expect(PxPage.Loading).toHaveBeenCalled();
                expect(PxPage.FneLoadedHooks["quiz-editor"]).toBeDefined();
                expect($(dialog).length).toEqual(0);

            });
            
            it("should be able to see the inused questions using the preview dialog", function () {
                var btnShowPreview = ".preview-current-question:first";
                var question = $(btnShowPreview).closest('li.question');
                var inUsedContentHtml = $(question).find('#question-tool-tip').html();
                
                $(btnShowPreview).trigger('click');

                expect($("#show-question.ui-dialog-content").length).toBeGreaterThan(0);
                
                var dialog = '.ui-dialog-content.question-dialog-text';
                var inUsedButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:last');
                $(inUsedButton).trigger('click');

                expect($(inUsedButton).parents(".ui-dialog").find('.question-used-elsewhere').length).toEqual(1);
                expect($(inUsedButton).parents(".ui-dialog").find('.question-used-elsewhere').html().trim()).toEqual(inUsedContentHtml.trim());
                
            });

        });


       /* describe("css", function () {
            
            beforeEach(function () {
                jasmine.Ajax.reset();
                loadFixtures('questionlists.html');
                loadStyleFixtures('quiz.css');
            });

           
            it("can set quiz container to have min-width of 720px", function() {
                loadFixtures('questionlists.html');
                loadStyleFixtures('quiz.css');
                $('.question-list').questionlist('');
                expect($('.question-container')).toHaveCss({ "min-width": "720px" });
            });

            it("can set question to have min-height of 40px", function() {
                var fixture = readFixtures('questionlists.html');
                jasmine.Fixtures.prototype.addToContainer_(fixture);

                loadStyleFixtures('quiz.css');
                $('.question-list').questionlist('');
                expect($('.table-row')).toHaveCss({ "min-height": "40px" });

                jasmine.Fixtures.prototype.cleanUp();

            });
            
            it("can set question pool margin", function () {
                var fixture = readFixtures('questionlists.html');
                jasmine.Fixtures.prototype.addToContainer_(fixture);

                loadStyleFixtures('quiz.css');
                $('.question-list').questionlist('');
                expect($('.question-pool-questions')).toHaveCss({ "margin-left": "-10px" });

                jasmine.Fixtures.prototype.cleanUp();

            });
            
            it("can set checkbox container width", function () {
                var fixture = readFixtures('questionlists.html');
                jasmine.Fixtures.prototype.addToContainer_(fixture);

                loadStyleFixtures('quiz.css');
                $('.question-list').questionlist('');
                expect($('.select')).toHaveCss({ "width": "20px" });

                jasmine.Fixtures.prototype.cleanUp();

            });

        }); */
    });
    
    

    describe("selected questions list in quiz editor window", function () {
        var btnEditQuestion = ".selected-questions .edit-current-question:first";
        var btnEditQuestionPool = ".question.bank .edit-current-question:first";

        var chkMenuStatus = ".selected-questions .quiz-editor-questions input[type=checkbox]:first";
        var btnDeleteQuestion = ".selected-questions  .delete-current-question:first";
        var btnDeleteSelectedQuestions = ".remove-question, .various-actions .remove";
        var linkToAddInExistingAssessment = ".used-elsewhere-quizzes:first .add-to-existing-quiz";
        var linkToAddToNewAssessment = ".used-elsewhere-quizzes:first .add-to-new-assessment";

       

        var usedElseWhereHtmlContent;
        var existingassessmentHtmlContent;
        
        beforeAll(function() {
            var request = $.ajax({
                type: "GET",
                url: jasmine.getFixtures().fixturesPath + '/usedelsewhere.html' + "?" + new Date().getTime(),
                async: false
            });
            usedElseWhereHtmlContent = request.responseText;
            
            request = $.ajax({
                type: "GET",
                url: jasmine.getFixtures().fixturesPath + '/addquestiontoexistingassessment.html' + "?" + new Date().getTime(),
                async: false
            });
            existingassessmentHtmlContent = request.responseText;
            
            PxPage.Loading = jasmine.createSpy("PxPage Loading Spy");
            PxPage.log = jasmine.createSpy("PxPage log Spy");


            PxQuiz.FneResize = jasmine.createSpy("FneResize Spy");
            PxQuiz.EnumQuestionMode = jasmine.createSpy("EnumQuestionMode  Spy");
            PxQuiz.SetAllChecks = jasmine.createSpy("SetAllChecks Spy");

        });
        afterAll(function () {
            $("#edit-settings-dialog2").remove();
            $("#edit-settings-dialog").remove();
            $('.blockUI').remove();
        });
        

        beforeEach(function () {
            
            loadFixtures('selectedquestionslist.html');
            jasmine.Ajax.useMock();
            
            onSuccess = jasmine.createSpy('onSuccess');
            onFailure = jasmine.createSpy('onFailure');
           
            $('.ui-widget-overlay').remove();
            $('.ui-dialog').remove();
        });
        
        afterEach(function () {
            jasmine.Fixtures.prototype.cleanUp();
            $('.ui-widget-overlay').remove();
            $('.ui-dialog').remove();
            $('.question-dialog-text').remove();
        });
        

        it("should be able to move the current selected question to the top of the list", function () {
            
            var linkForMovingLastQuestion = $("div.move-current-question:last");
            var questionToBeMoved = linkForMovingLastQuestion.closest('li.question');
            var selectedQuestionList = $(linkForMovingLastQuestion).closest('ul.questions');

            
            $('.question-list').questionlist('moveToTopCurrentQuestion', { target: linkForMovingLastQuestion });

            expect(selectedQuestionList.find("li.question:first").attr('id')).toEqual(questionToBeMoved.attr('id'));

        });
        
        it("should save the updated question list after moved", function () {

            var linkForMovingLastQuestion = $("div.move-current-question:last");
            var questionToBeMoved = linkForMovingLastQuestion.closest('li.question');
            var selectedQuestionList = $(linkForMovingLastQuestion).closest('ul.questions');
            

            $('.question-list').questionlist('moveToTopCurrentQuestion', { target: linkForMovingLastQuestion });
            

            expect(selectedQuestionList.find("li.question:first").attr('id')).toEqual(questionToBeMoved.attr('id'));

        });



        it("should fade the selected question when moved to top", function () {
            
            var fixture = "<div id='main'>" + readFixtures('selectedquestionslist.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var linkForMovingLastQuestion = $("div.move-current-question:last");
            var questionToBeMoved = linkForMovingLastQuestion.closest('li.question');
            
            
            $('.question-list').questionlist('moveToTopCurrentQuestion', { target: linkForMovingLastQuestion });
            
            $(PxPage.switchboard).trigger("questionlistupdated");

            expect(PxPage.Fade).toHaveBeenCalled();
            expect(questionToBeMoved).toHaveClass('fade-effect');

        });
        
        
        it("should be able to move the current selected question to the bottom of the list", function () {
            
            
            var fixture = "<div id='main'>" + readFixtures('selectedquestionslist.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            
            var linkForMovingFirstQuestion = $("div.move-current-question:first");
            var questionToBeMoved = linkForMovingFirstQuestion.closest('li.question');
            var selectedQuestionList = $(linkForMovingFirstQuestion).closest('ul.questions');
            $('.question-list').questionlist('moveToBottomCurrentQuestion', { target: linkForMovingFirstQuestion });

            expect(selectedQuestionList.find("li.question:last").attr('id')).toEqual(questionToBeMoved.attr('id'));


        });
        
        it("should fade the selected question when moved to bottom", function () {

            var fixture = "<div id='main'>" + readFixtures('selectedquestionslist.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var linkForMovingFirstQuestion = $("div.move-current-question:first");
            var questionToBeMoved = linkForMovingFirstQuestion.closest('li.question');
            
            $('.question-list').questionlist('moveToTopCurrentQuestion', { target: linkForMovingFirstQuestion });

            $(PxPage.switchboard).trigger("questionlistupdated");

            expect(PxPage.Fade).toHaveBeenCalled();
            expect(questionToBeMoved).toHaveClass('fade-effect');

        });
        
        it("should be able to  number the selected questions", function () {
            
            $('.question-list').questionlist('numberQuestions', $('.quiz-overview .question-display'));
            $(PxPage.switchboard).trigger("questionlistupdated");

        });
        
        it("should be able to refresh the selected questions", function () {

            $('.question-list').questionlist('refreshQuestionList');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.render_children);
            expect(request.method).toBe('POST');
            
            expect($('.question-list').questionListGearbox).toHaveBeenCalled();
            expect(PxPage.Loaded).toHaveBeenCalled();
            

        });
        
        it("should be able to remove the selected questions", function () {

            var questiontarget = $('li.question:first');
            
            $('.question-list').questionlist('removeSelected', questiontarget);

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.question_order);
            expect(request.method).toBe('POST');
            
            expect($(".selected-questions").find(questiontarget).length).toEqual(0);
            expect(PxPage.Loaded).toHaveBeenCalled();
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled(); 

        });
        
        it("should be able to update the questions metaadata when it is removed", function () {
            ajaxRequests = [];
            var questiontarget = $('li.question:first');

            $('.question-list').questionlist('removeSelected', questiontarget);

            if (ajaxRequests.length > 0) {
                request = ajaxRequests[0];
                request.response(QuizResponses.questions.success);
                
                expect(request.url).toBe(PxPage.Routes.update_questions_meta);
                expect(request.method).toBe('POST');
                
            }
            
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.question_order);
            expect(request.method).toBe('POST');
         

            expect($(".selected-questions").find(questiontarget).length).toEqual(0);
            expect(PxPage.Loaded).toHaveBeenCalled();
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();

        });
        
        it("should be able to clear all questions", function () {

            $('.question-list').questionlist('clearAllQuestions', $('.quiz-overview .question-display'));
            expect(PxQuiz.SetAllChecks).toHaveBeenCalled();
            expect($('.ui-dialog.questionCard').length).toEqual(0);

        });
        
        it("should be able to search the question metadata", function () {

            $('.question-list').questionlist('searchQuestionMetaData', 'dummy');
            expect(PxPage.SearchTermOperation).toHaveBeenCalled();

        });
       
        
        it("should be able to edit the selected question", function () {
            
            var editQuestionCalled = false;
            
            $.fn.questioneditor.andCallFake(function () {
                editQuestionCalled = true;
            });
            $('.question-list').questionlist();
            $(btnEditQuestion).trigger('click');
            
            expect(editQuestionCalled).toEqual(true);
            
        });
        
        it("should be able to edit the selected question pool", function () {

            var editQuestionCalled = false;
                
            $.fn.questioneditor.andCallFake(function () {
                editQuestionCalled = true;
            });

            $('.question-list').questionlist();
            $(btnEditQuestionPool).trigger('click');

            expect(editQuestionCalled).toEqual(true);

        });


        
        it("should be able to delete the current selected question", function () {
            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);
            
            $('.question-list').questionlist();
            $(btnDeleteQuestion).trigger('click');
            var dialog = '.ui-dialog-content.question-dialog-text';
            var removeButton = $(dialog).parent().find(' .ui-dialog-buttonset .ui-button:first');
            $(removeButton).trigger('click');
            
            expect($('.selected-questions .question-list .questions li.question').length).toEqual(4);
            expect(PxQuiz.UpdateSelectedQuestionsMenu).toHaveBeenCalled();
            expect($.fn.questionListGearbox).toHaveBeenCalled();
        });
        
        it("should be able to remove the classes from the question while deleting the current selected question", function () {
            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);

            $('.question-list').questionlist();
            $(btnDeleteQuestion).closest('li.question').addClass('active');
            $(btnDeleteQuestion).trigger('click');
            var dialog = '.ui-dialog-content.question-dialog-text';
            var removeButton = $(dialog).parent().find(' .ui-dialog-buttonset .ui-button:first');
            $(removeButton).trigger('click');

            expect($('.selected-questions .question-list .questions li.question').length).toEqual(4);
            expect(PxQuiz.UpdateSelectedQuestionsMenu).toHaveBeenCalled();
            expect($.fn.questionListGearbox).toHaveBeenCalled();

            expect($(btnDeleteQuestion).closest('li.question')).not.toHaveClass('selected-question-in-quiz-editor');
            
        });
        
        it("should not delete the current selected question if the user clicks cancel", function () {
            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);

            $('.question-list').questionlist();
            $(btnDeleteQuestion).trigger('click');
            var dialog = '.ui-dialog-content.question-dialog-text';
            var removeButton = $(dialog).parent().find(' .ui-dialog-buttonset .ui-button:last');
            $(removeButton).trigger('click');

            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);
            expect($('.ui-dialog').length).toEqual(0);
        });
        
        it("should be able to delete all the selected questions", function () {
            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);
            
            $('.selected-questions .question-list .questions li.question').find('input[name=selected]').prop('checked', true);
            $('.question-list').questionlist();
            $(btnDeleteSelectedQuestions).trigger('click');
            var dialog = '.ui-dialog-content.question-dialog-text';
            var removeButton = $(dialog).parent().find(' .ui-dialog-buttonset .ui-button:first');
            $(removeButton).trigger('click');

            expect($('.selected-questions .question-list .questions li.question').length).toEqual(0);
            expect(PxQuiz.UpdateSelectedQuestionsMenu).toHaveBeenCalled();
            expect($.fn.questionListGearbox).toHaveBeenCalled();
        });

        it("should not delete the selected questions if the user clicks cancel", function () {
            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);

            $('.question-list').questionlist();
            var expectedNumber = $('.ui-dialog').length;
            $(btnDeleteSelectedQuestions).trigger('click');
            var dialog = '.ui-dialog-content.question-dialog-text';
            var removeButton = $(dialog).parent().find(' .ui-dialog-buttonset .ui-button:last');
            $(removeButton).trigger('click');

            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);
            expect($('.ui-dialog').length).toEqual(expectedNumber);
        });
        
        it("should warning to select atleast one question to delete", function () {
            expect($('.selected-questions .question-list .questions li.question').length).toEqual(5);

            $('.selected-questions .question-list .questions li.question').find('input[name=selected]').prop('checked', false);
            $('.question-list').questionlist();
            $(btnDeleteSelectedQuestions).trigger('click');
            
            expect(PxPage.Toasts.Warning).toHaveBeenCalled();
            
        });
        
        
        it("should hide the related content window while editing a question", function () {
            var fixture = readFixtures('selectedquestionslist.html');
            fixture = $(fixture).append("<div id='related-content-editor-pool'>something</div>");
            
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            
            var editQuestionCalled = false;

            $.fn.questioneditor.andCallFake(function () {
                editQuestionCalled = true;
            });

            var hideRelatedContentForPool = false;
            var originalLearningCurveMoreResources = $.fn.LearningCurveMoreResources;
            $.fn.LearningCurveMoreResources = jasmine.createSpy('LearningCurveMoreResources').andCallFake(function () {
                hideRelatedContentForPool = true;
            });
            
            $('.question-list').questionlist();
            $(btnEditQuestion).trigger('click');

            $.fn.LearningCurveMoreResources = originalLearningCurveMoreResources;
            expect(hideRelatedContentForPool).toEqual(true);

        });
        
        it("should hide the related content window while editing a question pool", function () {
            var fixture = readFixtures('selectedquestionslist.html');
            fixture = $(fixture).append("<div id='related-content-editor-pool'>something</div>");

            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var editQuestionCalled = false;

            $.fn.questioneditor.andCallFake(function () {
                editQuestionCalled = true;
            });

            var hideRelatedContentForPool = false;
            var originalLearningCurveMoreResources = $.fn.LearningCurveMoreResources;
            $.fn.LearningCurveMoreResources = jasmine.createSpy('LearningCurveMoreResources').andCallFake(function () {
                hideRelatedContentForPool = true;
            });
            

            $('.question-list').questionlist();
            $(btnEditQuestionPool).trigger('click');
            
            $.fn.LearningCurveMoreResources = originalLearningCurveMoreResources;

            expect(hideRelatedContentForPool).toEqual(true);

        });
        

        it("should be able to set the used question menu status", function () {

            $('.question-list').questionlist();
            $(chkMenuStatus).trigger('click');

            var question = $(chkMenuStatus).closest(".question");

            expect($('.question-bank-header-right .questions-actions .remove-btn-wrapper')).toBeVisible();
            expect($(question)).toHaveClass('selected-question-in-quiz-editor');
            
        });
        
        it("should be able to show 'add questioncard to existing quiz' dialog", function () {
            var tag = "#px-dialog";
            
            $('.question-list').questionlist();
            
            $(linkToAddInExistingAssessment).trigger('click');


            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.show_add_question_to_existing_assessment);
            expect(request.method).toBe('POST');
            
            expect($(tag).length).toBeGreaterThan(0);

            $(tag).remove();
        });
        
        it("should show error message if the user adds pxunit items to the quiz", function () {

            var tag = "#px-dialog";
            
            PxContentTemplates.onItemFromTemplate.andCallFake(function (data) {
                data.callback('362565086d164e4bbc221ae4c1f2680c');
            });
            $.fn.questioneditor = jasmine.createSpy('questioneditor');
            $('.question-list').questionlist();

            $(linkToAddInExistingAssessment).trigger('click');

            request = mostRecentAjaxRequest();
            QuizResponses.useElseWhere.success.responseText = existingassessmentHtmlContent;
            request.response(QuizResponses.useElseWhere.success);

            $(".add-question-to-existing-quiz-dialog .faux-tree-node:first").addClass('active item-type-pxunit');
            
            $('.add-to-new-assessment-dialog').find('#assessment-questionid').val('');
            $('.question-list').questionlist('addQuestionToQuiz');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.addQuestionToQuiz_fail.success);
            
            
            expect(PxPage.Toasts.Error).toHaveBeenCalled();
            expect($.fn.questioneditor).not.toHaveBeenCalled();

            $(tag).remove();
        });

        it("should clean up metadata area of question after questioncard is displayed", function () {
            $('.question-list').questionlist();

            $(".preview-current-question").first().click();
            
            expect($(".question-text .questioncard-container").first().html().length == 0).toBeTruthy();
        });
        
        it("should be able to add questioncard to existing quiz using 'question card' dialog", function () {
            var tag = "#px-dialog";

            $('.question-list').questionlist();

            $(linkToAddInExistingAssessment).trigger('click');


            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.show_add_question_to_existing_assessment);
            expect(request.method).toBe('POST');

            expect($(tag).length).toBeGreaterThan(0);

            $(tag).remove();
        });
        
        it("should be able to close 'add questioncard to existing quiz' dialog", function () {
            var tag = "#px-dialog";
            
            $('.question-list').questionlist();

            $(linkToAddInExistingAssessment).trigger('click');


            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.show_add_question_to_existing_assessment);
            expect(request.method).toBe('POST');

            expect($(tag).length).toBeGreaterThan(0);
            
            $("#px-dialog").dialog('close');

            expect($(tag).length).toEqual(0);
            expect($.fn.ContentTreeWidget).toHaveBeenCalled();
            
            $(tag).remove();
        });
        
        it("should be able to open the dialog for 'add questioncard to new assessment'", function () {
            
            var tag = ".add-to-new-assessment-dialog";
            
            $('.question-list').questionlist();

            expect($('.add-to-new-assessment-dialog.ui-dialog-content').length).toEqual(0);
            $(linkToAddToNewAssessment).trigger('click');
            

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toContain(PxPage.Routes.show_add_to_new_assessment);
            expect(request.method).toBe('GET');

            expect($('.add-to-new-assessment-dialog.ui-dialog-content').length).toEqual(1);
            
            $(tag).remove();
        });
        
        it("should be able to add question to new assessment", function () {

            var tag = ".add-to-new-assessment-dialog";

            PxContentTemplates.onItemFromTemplate.andCallFake(function (data) {
                data.callback('362565086d164e4bbc221ae4c1f2680c');
            });
            $('.question-list').questionlist();

            $(linkToAddToNewAssessment).trigger('click');


            request = mostRecentAjaxRequest();
            QuizResponses.useElseWhere.success.responseText = usedElseWhereHtmlContent;
            request.response(QuizResponses.useElseWhere.success);

            $('.question-list').questionlist('addToNewAssessment');
            
            request = mostRecentAjaxRequest();
            request.response(QuizResponses.addQuestionToQuiz.success);
            
            
            expect(request.url).toContain(PxPage.Routes.add_question_to_quiz);
            expect(request.method).toBe('POST');

            expect(PxContentTemplates.onItemFromTemplate).toHaveBeenCalled();
            expect(PxPage.Toasts.Success).toHaveBeenCalled();
            expect($.fn.questioneditor).toHaveBeenCalled();
            //expect(ajaxCalled).toBeTruthy();

            $(tag).remove();
        });
        
        it("should show error message if the server doesn't send success message while adding question to new assessment", function () {

            var tag = ".add-to-new-assessment-dialog";

            PxContentTemplates.onItemFromTemplate.andCallFake(function (data) {
                data.callback('362565086d164e4bbc221ae4c1f2680c');
            });
            $('.question-list').questionlist();

            $(linkToAddToNewAssessment).trigger('click');


            request = mostRecentAjaxRequest();
            QuizResponses.useElseWhere.success.responseText = usedElseWhereHtmlContent;
            request.response(QuizResponses.useElseWhere.success);

            $('.question-list').questionlist('addToNewAssessment');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.addQuestionToQuiz_fail.success);


            expect(request.url).toContain(PxPage.Routes.add_question_to_quiz);
            expect(request.method).toBe('POST');

            expect(PxContentTemplates.onItemFromTemplate).toHaveBeenCalled();
            
            expect(PxPage.Toasts.Error).toHaveBeenCalled();
            
            //expect(ajaxCalled).toBeTruthy();

            $(tag).remove();
        });
        
       
        
        it("should be able to throw error message if the target quiz id is not provided while adding question to the new assessment", function () {

            var tag = ".add-to-new-assessment-dialog";
            PxContentTemplates.onItemFromTemplate.andCallFake(function (data) {
                data.callback();
            });

            $('.question-list').questionlist();

            $(linkToAddToNewAssessment).trigger('click');


            request = mostRecentAjaxRequest();
            QuizResponses.useElseWhere.success.responseText = usedElseWhereHtmlContent;
            request.response(QuizResponses.useElseWhere.success);

            $('.question-list').questionlist('addToNewAssessment');

            expect(request.url).toContain(PxPage.Routes.show_add_to_new_assessment);
            expect(request.method).toBe('GET');

            expect(PxPage.Toasts.Error).toHaveBeenCalled();

            $(tag).remove();
        });
        
        it("should be throw exception if the assessment title is not defined when adding to new assessment", function () {

            var tag = ".add-to-new-assessment-dialog";

            $('.question-list').questionlist();

            $(linkToAddToNewAssessment).trigger('click');

            $('.question-list').questionlist('addToNewAssessment');

            expect(PxPage.Toasts.Error).toHaveBeenCalled();
            $(tag).remove();
        });
        
    });
    
    describe("questions list in quiz editor window", function () {
        
        $.fn.qtip = jasmine.createSpy("qtip Spy");
        beforeEach(function () {

            loadFixtures('QuestionPicker.html');
            jasmine.Ajax.useMock();

            onSuccess = jasmine.createSpy('onSuccess');
            onFailure = jasmine.createSpy('onFailure');
        });

        afterEach(function () {
            jasmine.Fixtures.prototype.cleanUp();
            $('.ui-widget-overlay').remove();
            $('.ui-dialog').remove();
            $('.question-dialog-text').remove();
        });


        it("should be able to modify in used questions", function () {

            $('.question-list').questionlist('modifyInUsedQuestions', $('div.select-menu:last'));

            expect($('.available-questions li.question')).not.toHaveClass('in-used-question');
            expect($('.available-questions li.question')).not.toHaveClass('active');

        });
        
        it("should be able to make available questions draggable", function () {

            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            var draggingmessage = "";
            expect(draggingmessage).toEqual("");
            jasmine.Fixtures.prototype.addToContainer_(fixture);
           

            $('.question-list').questionlist('');
            
            var target = $('li.question:first');
            spyOn($.fn, "draggable").andCallFake(function (arg) {
                if (arg.helper) {
                    draggingmessage = arg.helper({ target: target });    
                }
                
            });
            $(PxPage.switchboard).trigger("availablequestionsupdated");
            
            expect(draggingmessage).not.toBeEmpty();

        });

        it("should be able to add questions to question pool which are not expanded", function () {
            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            
            var questions = $('.available-questions li.question:first');
            var pool = $('.selected-questions li.question.bank:first');
            var poolId = $('.selected-questions li.question.bank:first').attr('id');
            var bankCount = parseInt($('li#' + poolId + ' .question-container .bank-question-count').html().trim());
            $(pool).removeClass('bank-expanded');
            
            expect(bankCount).toEqual(1);

            $('.question-list').questionlist('');
            $('.question-list').questionlist('addQuestionsToPool', questions, poolId);
            
            var newbankCount = parseInt($('li#' + poolId + ' .question-container .bank-question-count').html().trim());
            expect(newbankCount).toEqual(2);
            expect($(pool).find('.show-current-questions')).toBeHidden();
            expect($(pool).find('.hide-current-questions')).not.toBeHidden();
            expect($(pool).find('.question-pool')).not.toBeEmpty();
            expect($(pool)).toHaveClass('bank-expanded');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.get_poolquestionlist);
            expect(request.method).toBe('POST');

        });
        
        it("should be able to add questions to question pool which are expanded", function () {
            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var questions = $('.available-questions li.question:first');
            var pool = $('.selected-questions li.question.bank:first');
            var poolId = $('.selected-questions li.question.bank:first').attr('id');
            $(pool).addClass('bank-expanded');

            $('.question-list').questionlist('');
            $('.question-list').questionlist('addQuestionsToPool', questions, poolId);

            expect($(pool).find('.hide-current-questions')).not.toBeHidden();
            expect($(pool).find('.question-pool')).not.toBeEmpty();
            expect($(pool)).toHaveClass('bank-expanded');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.question_order);
            expect(request.method).toBe('POST');

        });
        
        it("should be able to add available questions to selected questions", function () {
            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            
            var questionToBeAdded = $('.available-questions li.question:first');
            expect($(".selected-questions .question-list ul.questions:first li.question").length).toEqual(5);
            
            $(questionToBeAdded).find("input[name=selected]").prop('checked', true);

            $('.question-list').questionlist('addSelected', { target: $('.questions-actions > .add-quiestion-btn-wrapper > .add-available-question-at-top:first') });


            request = mostRecentAjaxRequest();
            request.response(QuizResponses.savequestions.success);
            expect(request.url).toBe(PxPage.Routes.question_order);
            expect(request.method).toBe('POST');
            
            
            expect(PxPage.Loading).toHaveBeenCalled();
            expect(PxQuiz.cfind).toHaveBeenCalled();
            expect(PxQuiz.FneResize).toHaveBeenCalled();
            expect(PxPage.Toasts.Success).toHaveBeenCalled();
            expect(PxQuiz.SetAllChecks).toHaveBeenCalled();
            expect(PxPage.Loaded).toHaveBeenCalled();
            expect(PxQuiz.UpdateSelectedQuestionsMenu).toHaveBeenCalled();
            expect($(".selected-questions .question-list ul.questions:first li.question").length).toEqual(6);

        });
        
        it("should be able to add available questions to question pool", function () {
            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var questionToBeAdded = $('.available-questions li.question:first');
            expect($(".selected-questions .question-list ul.questions:first li.question").length).toEqual(5);

            $(questionToBeAdded).find("input[name=selected]").prop('checked', true);

            $('.question-list').questionlist('addSelected', { target: $('.questions-actions > .add-quiestion-btn-wrapper > .add-available-question-at-top:first') });


            request = mostRecentAjaxRequest();
            request.response(QuizResponses.savequestions.success);
            expect(request.url).toBe(PxPage.Routes.question_order);
            expect(request.method).toBe('POST');


            expect(PxPage.Loading).toHaveBeenCalled();
            expect(PxQuiz.cfind).toHaveBeenCalled();
            expect(PxQuiz.FneResize).toHaveBeenCalled();
            expect(PxPage.Toasts.Success).toHaveBeenCalled();
            expect(PxQuiz.SetAllChecks).toHaveBeenCalled();
            expect(PxPage.Loaded).toHaveBeenCalled();
            expect(PxQuiz.UpdateSelectedQuestionsMenu).toHaveBeenCalled();
            expect($(".selected-questions .question-list ul.questions:first li.question").length).toEqual(6);

        });
        
        it("should be able to open question dialog from question card", function () {
            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            
            $('.question-list').questionlist('');
            var btnShowPreview = ".preview-current-question:first";

            $(btnShowPreview).trigger('click');

            expect($(".ui-dialog-content").length).toBeGreaterThan(0);
            expect($(".ui-dialog-titlebar.ui-widget-header")).toHaveClass('questioncardTitle');
            expect($(".ui-dialog").find('.question-dialog-text .questioncard-container').length).toEqual(1);
        });
        
        it("should be able to see the preview of the  selected custom questions", function () {
            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            $('.question-list').questionlist('');
            
            var btnShowPreview = ".quiz-editor-questions li.question.custom .preview-current-question:first";
            var question = $(btnShowPreview).closest('li.question');
            
            $(btnShowPreview).trigger('click');
            
            var previewDiv = '#custom_preview_f96e7b99_ab21_468e_81bf_3fea61b6d625';

            expect($(question)).toHaveClass('question-being-previewed');
            expect($('.question-dialog-text ' + previewDiv).find('iframe.question_preview_wrap').length).toEqual(1);
            expect($(".question-dialog-text.ui-dialog-content").length).toEqual(1);
       
            var expFrame = $(previewDiv +" iframe.question_preview_wrap");
            expect(expFrame).toExist();
            var expectedUrl = PxPage.Routes.custom_inline_preview +
                "?questionId=" + $(".question-id", $(question)).val()
                + "&customUrl=" + $(".question-custom-url", $(question)).val()
                + "&entityid=" + $(".question-entity-id", $(question)).val();
            expect(expFrame.attr("src")).toEqual(expectedUrl);
        });
        
        it("should be able to click regenerate variables for selected custom questions", function () {
            var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            jasmine.Fixtures.prototype.addToContainer_(fixture);
            $('.question-list').questionlist('');
            var btnShowPreview = ".quiz-editor-questions li.question.custom .preview-current-question:first";
            var question = $(btnShowPreview).closest('li.question');

            // can't really test variables regeneration functionality...merely checking that preview content is refreshed

            // open preview and check if iframe is there
            $(btnShowPreview).trigger('click');        
            var dialog = '.question-dialog-text.ui-dialog-content';
            var regenerateVariables = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            var previewFrame = $('iframe.question_preview_wrap', $(dialog));        
            expect(previewFrame).toExist();
            expect(regenerateVariables).toExist();

            // now kill the iframe and check if its gone
            previewFrame.remove();
            expect($('iframe.question_preview_wrap', $(dialog)).length).toEqual(0);

            // click regenerate button - iframe should be back
            $(regenerateVariables).trigger('click');
            previewFrame = $('iframe.question_preview_wrap', $(dialog));
            expect(previewFrame).toExist();
        });
        
      
        it("should be able to drag and drop a question from the question selector to available questions and save", function () {
            //var fixture = "<div id='main'>" + readFixtures('QuestionPicker.html') + "</div>";
            //jasmine.Fixtures.prototype.addToContainer_(fixture);
            $('.question-list').questionlist('');
            
            var itemToAdd = $(".available-questions .questions li.question[id='68C8CC49582AB2AAA2DE51DC170402C8']").clone();
            //add item to left-hand side to simulate drag and drop
            $(".selected-questions .questions:first").append(itemToAdd);
            PxQuiz.updatedTimes = 0;
            //trigger sortupdate (happens after completion of drag event
            $(".selected-questions .questions:first").sortable().trigger('sortupdate', { ui: { sender: null }, item: itemToAdd });

            //find question_order AJAX request
            request = $(ajaxRequests.filter(function(i) { return i.url == PxPage.Routes.question_order; })).last()[0];

            var data = JSON.parse(request.params);
            var questions = data.QuizQuestions.questions;
            expect(questions.length).toBe(5);
            expect(questions[4].mainQuizId).toBe("52104f7788e8455d815f329eb7913f77");
            expect(questions[4].quizId).toBe("Div2");
            expect(questions[4].questionId).toBe("68C8CC49582AB2AAA2DE51DC170402C8");
            expect(questions[4].entityId).toBe("42298");
            expect(questions[4].isBank).toBe(false);

            request.response(QuizResponses.questions.success);

            expect(request.url).toBe(PxPage.Routes.question_order);
            expect(request.method).toBe('POST');
            expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();
            expect(PxQuiz.UpdateSelectedQuestionsMenu).toHaveBeenCalled();

        });
        

    });
    
    describe("available questions list in quiz editor window", function () {
        var btnExpandQuestion = ".expand-available-question:first";
        
        beforeEach(function () {

            loadFixtures('availablequestionslist.html');
            jasmine.Ajax.useMock();

            onSuccess = jasmine.createSpy('onSuccess');
            onFailure = jasmine.createSpy('onFailure');
        });

        afterEach(function () {
            jasmine.Fixtures.prototype.cleanUp();
            $('.ui-widget-overlay').remove();
            $('.ui-dialog').remove();
            $('.question-dialog-text').remove();
        });

        
        it("should be able to see the preview of the available questions", function () {

            var btnShowPreviewAvail = ".preview-available-question:first";
            var question = $(btnShowPreviewAvail).closest('li.question');
            
            $('.question-list').questionlist('');

            $(btnShowPreviewAvail).trigger('click');

            expect($(question)).toHaveClass('question-being-previewed');
            expect($("#show-question.ui-dialog-content").length).toBeGreaterThan(0);

        });
        
        it("should be able to expand ", function () {

            loadFixtures('QuestionPicker.html');

            $('.question-list').questionlist('');
            $(btnExpandQuestion).closest(".description").find(".question-text").addClass('collapsed');

            $(btnExpandQuestion).trigger('click');

            expect($(btnExpandQuestion).closest(".description").find(".question-text")).not.toHaveClass('collapsed');

        });
       
    });

    describe("homework question list", function () {
        var btnEditSelectedQuestionSettings = ".various-actions .edit-settings";
        
        beforeEach(function () {

            loadFixtures('homeworkquestionlists.html');
            jasmine.Ajax.useMock();

            onSuccess = jasmine.createSpy('onSuccess');
            onFailure = jasmine.createSpy('onFailure');
        });

        afterEach(function () {
            jasmine.Fixtures.prototype.cleanUp();
            $('.ui-widget-overlay').remove();
            $('.ui-dialog').remove();
            $('.question-dialog-text').remove();
        });
        
        it("should be able to open 'edit selected question settings for homework' dialog from hts editor", function () {

            expect($('.ui-dialog').length).toEqual(0);
            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $('.question-list').questionlist('editQuestionSettings', '', '68C8CC49582AB2AAA2DE51DC170402C8', '51280');


            request = mostRecentAjaxRequest();
            request.response(QuizResponses.questionSettings.success);

            expect(request.url).toContain(PxPage.Routes.question_settings);
            expect(request.method).toBe('GET');
            expect($('#edit-settings-dialog.ui-dialog-content').length).toEqual(1);

        });
        
        //updating the attempts from question settings dialog.
        it("should be able to edit and save selected question settings", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);
            expect($(questionId).find(".attempt-label").text().trim()).toEqual("2  attempts");
            expect($(questionId).find(".point-label").text().trim()).toEqual("1  pt");

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);
            $("#edit-settings-dialog2 #Points").val('1');

            expect(request.url).toContain(PxPage.Routes.question_settings);
            expect(request.method).toBe('GET');

            var dialog = '#edit-settings-dialog.ui-dialog-content';
            var saveButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            $(saveButton).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);
            expect(request.url).toContain(PxPage.Routes.question_settings);
            expect(request.method).toBe('POST');
            

            expect($(questionId).find(".attempt-label").text().trim()).toEqual("3 attempts");
            expect($(questionId).find(".point-label").text().trim()).toEqual("2 pts");


        });
        
        
        
        it("should show the error if the points or timelimit is not provided while editing and saving selected question settings", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            
            $(questionId).find('.select input').prop('checked', true);
            expect($(questionId).find(".attempt-label").text().trim()).toEqual("2  attempts");
            expect($(questionId).find(".point-label").text().trim()).toEqual("1  pt");

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);
            expect(request.url).toContain(PxPage.Routes.question_settings);
            expect(request.method).toBe('GET');
            $("#Points:first").val('');
            var dialog = '#edit-settings-dialog.ui-dialog-content';
            var saveButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            $(saveButton).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);
            

            expect(PxPage.Toasts.Error).toHaveBeenCalled();


        });
        
        it("should show the error if the points or timelimit is negative number", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);
            expect($(questionId).find(".attempt-label").text().trim()).toEqual("2  attempts");
            expect($(questionId).find(".point-label").text().trim()).toEqual("1  pt");

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);
            expect(request.url).toContain(PxPage.Routes.question_settings);
            expect(request.method).toBe('GET');
            $("#Points:first").val('-1');
            var dialog = '#edit-settings-dialog.ui-dialog-content';
            var saveButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            $(saveButton).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);


            expect(PxPage.Toasts.Error).toHaveBeenCalled();


        });
        
      
        
        it("should show the error if the number of Attempts is not provided", function () {

            var questionId = "#68C8CC49582AB2AAA2DE51DC170402C8";
            $(questionId).find('.select input').prop('checked', true);
            expect($(questionId).find(".attempt-label").text().trim()).toEqual("2  attempts");
            expect($(questionId).find(".point-label").text().trim()).toEqual("1  pt");

            $('ul.questions li .select input').prop('checked', true);
            $('.question-list').questionlist('');
            $(btnEditSelectedQuestionSettings).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);
            expect(request.url).toContain(PxPage.Routes.question_settings);
            expect(request.method).toBe('GET');
            $("[name^='NumberOfAttempts']").append("<option value='-1'>Select proper attempts</option>");
            $("[name^='NumberOfAttempts']").val('-1');
            var dialog = '#edit-settings-dialog.ui-dialog-content';
            var saveButton = $(dialog).parent().find('.ui-dialog-buttonset .ui-button:first');
            $(saveButton).trigger('click');

            request = mostRecentAjaxRequest();
            request.response(QuizResponses.homeworkQuestionSettings.success);


            expect(PxPage.Toasts.Warning).toHaveBeenCalled();


        });
    });
});