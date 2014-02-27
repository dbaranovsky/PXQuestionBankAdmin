describe("Quiz Fne functionalities Tests: ", function () {
    beforeAll(function () {
        TestQuizHelper.defaultSpyOn();
        TestQuizHelper.spyOnPxBreadcrumb();
    });
    afterAll(function() {
        if (!!$('body #fne-window').length)
            $('body #fne-window').remove();
    });
    beforeEach(function () {
        setFixtures('<div id="fne-window"><div id="fne-content"><div id="content">' +
            '<div class="show-quiz"><div class="gradebook-component"><div class="info"></div></div></div>' +
            '<div class="quiz-editor"><div class="selected-questions"><div class="question-pool"><div class="questions"></div></div><div>' +
            '<div class="available-questions"><div class="toc"></div></div></div></div></div></div>');

    });
    
    it('Call undefined function, Expect error', function () {
        var initShouldFail = function () {
            $('#fne-window').quizFne('UndefinedFunction');
        };
        expect(initShouldFail).toThrow(new Error('Method UndefinedFunction does not exist on jQuery.quizFne'));
    });
    describe("Test Quiz fne init", function () {
        beforeAll(function () {
            TestQuizHelper.spyOnPxBreadcrumb();
            TestQuizHelper.spyOnContentWidget();
        });
        beforeEach(function () {
            $(document).unbind();
            TestQuizHelper.defaultSpyOn();
            var model = TestQuizHelper.Model.generateQuizModel();
            var viewModel = TestQuizHelper.generateViewModel(model);
            TestQuizHelper.setFixtureFromCache(viewModel, 'target', '#content');
            model.path = 'Questions';
            viewModel = TestQuizHelper.generateViewModel(model);
            TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor-questions');
            model.ShowReused = true;
            model.path = 'QuestionList';
            viewModel = TestQuizHelper.generateViewModel(model);
            TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor .children-list');

            $.fn.block = jasmine.createSpy('block');
            $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');
            $.fn.resizable = jasmine.createSpy('resizable');
            if (!$('#fne-content #lnkPoints').length)
                $('#fne-content').append($('<div id="lnkPoints">1</div>'));
            if (!$(".default-breadcrumb .default-path-item").length)
                $('#fne-content').append($('<div class="default-breadcrumb"><div class="default-path-item"></div></div>'));
            $('#fne-window').quizFne('fneInit');

        });
        it('Initialize, Expect success', function () {
            var eventData = $(document).data('events');
            expect(eventData.breadcrumb).toBeDefined();
            expect(eventData.click).toBeDefined();
            expect(eventData.focusout).toBeDefined();
            expect(eventData.keypress).toBeDefined();

        });
        it('Click on "new-question", expect "openEditorForNewQuestions" in questioneditor get called', function () {
            var methodCalled = false;
            
            $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                methodCalled = methodCalled || method == 'openEditorForNewQuestions';
            });
            $('.selected-questions .new-question').click();
            waitsFor(function () {
                return methodCalled;
            });
            runs(function () {
                expect(methodCalled).toBeTruthy();
            });
        });
        
        it('Click on "add-available-question-at-top", Expect function "addSelected" in questionlist get called ', function () {
            var methodCalled = false;
            $.fn.questionlist = jasmine.createSpy('questionlist').andCallFake(function(method) {
                methodCalled = methodCalled || method == 'addSelected';
            });
            $('.available-questions .add-available-question-at-top').click();

            waitsFor(function() {
                return methodCalled;
            });
            runs(function() {
                expect(methodCalled).toBeTruthy();
            });
        });
        
        it('Click on "#txtPoolName", Expect save button clicked', function () {
            $("#txtSearchQuiz").val('a');
            var simulateEnterEvent = jQuery.Event("keypress");
            simulateEnterEvent.which = 13;
            simulateEnterEvent.keyCode = 13;
            var saveBtnClicked = false;
            $(".question-pool-inner-container .pool-actions .save").bind('click', function() {
                saveBtnClicked = true;
            });
            $('#txtPoolCount').trigger('click');
            $('#txtPoolCount').trigger('focusout');
            $('#txtPoolCount').trigger(simulateEnterEvent);
            waitsFor(function () {
                return saveBtnClicked;
            });
            runs(function () {
                expect(saveBtnClicked).toBeTruthy();
            });
        });
        
        it('Event "breadcrumb.selectionChanged" triggered, Expect function "UpdateChildList" in PxQuiz get called', function () {
            PxPage.switchboard = 'body';
            $(PxPage.switchboard).trigger('breadcrumb.selectionChanged');
            expect(PxQuiz.UpdateChildList).toHaveBeenCalled();
        });
        
        it('Click on "lnkPoints", Expect function "preventDefault" get called', function () {
            var events = { type:'click',preventDefault: function() {}};
            var e = spyOn(events, ['preventDefault']);
            $('#lnkPoints').trigger(events);
            
            expect(e).toHaveBeenCalled();
        });
        it('Click on "default-breadcrumb", Expect load default page', function () {
            PxQuiz.FindDefaultRoot.andCallFake(function () {
                return "PX_MY_QUESTIONS";
            });
            $.fn.load = jasmine.createSpy('load').andCallFake(function(url, callback) {
                callback();
            });
            $(".default-breadcrumb .default-path-item").click();
            expect($('.default-path-item').text()).toEqual('Show All');
            expect(PxQuiz.PostChildLoadedProcessing).toHaveBeenCalled();

        });
    });
    it('Bind Fne hooks, Expect Success', function () {
        PxPage.FneInitHooks = {};
        PxPage.FneResizeHooks = {};
        $('#fne-window').quizFne('bindFneHooks');
 
        expect(PxPage.FneInitHooks['quiz-editor']).toBeDefined();
        expect(PxPage.FneInitHooks['quiz-overview']).toBeDefined();
        expect(PxPage.FneInitHooks['show-quiz']).toBeDefined();
        expect(PxPage.FneResizeHooks['quiz-editor']).toBeDefined();
        expect(PxPage.FneResizeHooks['show-quiz']).toBeDefined();
    });
    
    it('Test fne resize, Expect question pool height set to 100px', function () {
        $('#fne-window').quizFne('fneResize');
        var poolQuestionsBlock = $('.quiz-editor .selected-questions .question-pool .questions');
        expect(poolQuestionsBlock.css('height')).toEqual('100px');
        expect(poolQuestionsBlock.css('min-height')).toEqual('100px');
        expect(poolQuestionsBlock.hasClass('is-question-pool')).toBeTruthy();

    });
    describe('Test QuizFne showFneInit', function () {
        var originalFunction = $.fn.dialog;
        beforeAll(function () {
            originalFunction = $.fn.dialog;
        });
        afterAll(function () {
            $.fn.dialog = originalFunction;
        });
        it('Test fne method "showFneInit", Expect adjusted the height of "show-quiz" div  ', function() {
            $('#fne-content').height(300);
            $('.show-quiz .info').height(200);
            $('#fne-window').quizFne('showFneInit');
            expect($('.show-quiz .gradebook-component').height()).toEqual(100);

        });
        
        it('Click on button.collapse, Expect adjusted the height of "show-quiz" div  ', function () {
            $('#fne-content').height(300);
            $('.show-quiz .info').height(200);
            $('.show-quiz').append($('<button class="collapse" />'));
            $('#fne-window').quizFne('showFneInit');
            $('.show-quiz .gradebook-component').height(10);
            $('.show-quiz button.collapse').click();
            expect($('.show-quiz .gradebook-component').height()).toEqual(100);

        });
        
        it('Click on button.expand, Expect adjusted the height of "show-quiz" div  ', function () {
            $('#fne-content').height(300);
            $('.show-quiz .info').height(200);

            $('.show-quiz').append($('<button class="expand" />'));
            $('#fne-window').quizFne('showFneInit');

            $('.show-quiz .gradebook-component').height(10);
            $('.show-quiz button.expand').click();
            expect($('.show-quiz .gradebook-component').height()).toEqual(100);

        });
        
        it('Click on ui-widget-overlay, Expect stopImmediatePropagation & dialog to be called', function () {
            $('#fne-window').quizFne('showFneInit');
            $('.show-quiz').append($('<button class="ui-widget-overlay" />'));
            var events = { type: 'click', stopImmediatePropagation: function () { } };
            var e = spyOn(events, ['stopImmediatePropagation']);
            $.fn.dialog = jasmine.createSpy('dialog');

            $('.ui-widget-overlay').trigger(events);
            expect(e).toHaveBeenCalled();
            expect($.fn.dialog).toHaveBeenCalled();


        });
        
        it('Click on quizDirectionsModal, Expect stopImmediatePropagation & dialog to be called', function () {
            $('#fne-window').quizFne('showFneInit');
            $('.show-quiz').append($('<button id="quizDirectionsModal" />'));
            var events = { type: 'click', stopImmediatePropagation: function () { } };
            var e = spyOn(events, ['stopImmediatePropagation']);
            $.fn.dialog = jasmine.createSpy('dialog');

            $('#quizDirectionsModal').trigger(events);
            expect(e).toHaveBeenCalled();
            expect($.fn.dialog).toHaveBeenCalled();

        });
    });
    
    it('Test QuizFne overviewFneInit, Expect sortable is called and style changed', function () {
        $('#fne-content').append($('<div id="content-item"></div>'));
        $('#content-item').append($('<div class="quiz-overview"></div>'));

        $.fn.sortable = jasmine.createSpy('sortable');
        
        $('#fne-window').quizFne('overviewFneInit');
        expect($.fn.sortable).toHaveBeenCalled();
        expect($('.quiz-overview').parents('#fne-content #content-item').css('padding-right')).toEqual('10px');

    });
});