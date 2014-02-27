describe("Question Searching functionality in Quizzing Tests: ", function () {
    beforeAll(function () {
        TestQuizHelper.clearCache();
    });
    beforeEach(function () {
        $(document).unbind();
        TestQuizHelper.defaultSpyOn();

        var model = TestQuizHelper.Model.generateQuizModel();
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'set');
        
    });
    
    it('Initialization, Expect success', function () {
        $('.question-list').questionSearch();
        var eventData = $(document).data('events');
        var rval = $('.available-questions .searchQuestion #txtSearchQuiz').trigger('click');
        expect(rval.length).toEqual(1);

        rval = $('.available-questions .searchQuestion #txtSearchQuiz').trigger('focusout');
        expect(rval.length).toEqual(1);
        
        expect(eventData.click).toBeDefined();
        expect(eventData.keypress).toBeDefined();
        expect(eventData.focusout).toBeDefined();

    });
    
    it('Call undefined function, Expect fail', function () {
        var initShouldFail = function() {
            $('.question-list').questionSearch('UndefinedFunction');
        };
        expect(initShouldFail).toThrow(new Error('Method UndefinedFunction does not exist on jQuery.questionSearch'));
    });
    
    it('Press Enter in search, Expect search result', function () {
        $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');

        $('.question-list').questionSearch();
        $("#txtSearchQuiz").val('a');
        var simulateEnterEvent = jQuery.Event("keypress");
        simulateEnterEvent.which = 13;
        simulateEnterEvent.keyCode = 13;
        var callback = false;

        spyOn($, 'ajax').andCallFake(function (e) {
            e.success('QuestionSeach_PressEnter_ExpectSearchResult');
            callback = true;
        });

        $('.available-questions .searchQuestion #txtSearchQuiz').trigger(simulateEnterEvent);
        waitsFor(function () {
            return callback;
        });
        runs(function () {
            expect($('.available-questions .children-list').html()).toEqual('QuestionSeach_PressEnter_ExpectSearchResult');
        });
    });
    
    it('Search question with filter, Expect events trigger', function () {
        $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');
        var changeEventTrigger = false;
        var clickEventTrigger = false;
        var availablequestionsupdatedEventTrigger = false;
        var updatequestionfilterEventTrigger = false;

        PxPage.switchboard = 'body';
        $('.question-filter').append($('<div class="select2-choices"></div>'));
        $('.question-filter').append($('<span id="question-filter-questiontype"></span>'));
        $('.select2-choices').html('<ul class="select2-choices" style="">  <li class="select2-search-choice">    <div>Essay</div>    <a href="#" onclick="return false;" class="select2-search-choice-close" tabindex="-1"></a></li><li class="select2-search-field">    <input type="text" autocomplete="off" class="select2-input" id="s2id_autogen5" style="width: 10px;">  </li></ul>');
        $('#question-filter-questiontype').bind('change', function () { changeEventTrigger = true; });
        $(PxPage.switchboard).bind('availablequestionsupdated', function () { availablequestionsupdatedEventTrigger = true; });
        $(PxPage.switchboard).bind('updatequestionfilter', function () { updatequestionfilterEventTrigger = true; });

        $('.show-filter-available-question').bind('click', function () { clickEventTrigger = true;});
        $('.question-list').questionSearch();
        $("#txtSearchQuiz").val('a');
        var simulateEnterEvent = jQuery.Event("keypress");
        simulateEnterEvent.which = 13;
        simulateEnterEvent.keyCode = 13;
        var callback = false;

        spyOn($, 'ajax').andCallFake(function (e) {
            e.success('QuestionSeach_PressEnter_ExpectSearchResult');
            callback = true;
        });

        $('.available-questions .searchQuestion #txtSearchQuiz').trigger(simulateEnterEvent);
        waitsFor(function () {
            return callback;
        });
        runs(function () {
            expect($('.available-questions .children-list').html()).toEqual('QuestionSeach_PressEnter_ExpectSearchResult');
            expect(clickEventTrigger).toBeTruthy();
            expect(changeEventTrigger).toBeTruthy();
            expect(availablequestionsupdatedEventTrigger).toBeTruthy();
            expect(updatequestionfilterEventTrigger).toBeTruthy();

        });
    });

    it('Seach currently in use questions, Expect search result', function () {
        $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');

        $("#txtSearchQuiz").val('a');
        var callback = false;
        spyOn($, 'ajax').andCallFake(function (e) {
            e.success('QuestionSeach_SeachCurrentlyInUse_ExpectSearchResult');
            callback = true;
        });
        $('.question-list').questionSearch('searchQuestions', 'currently-in-use');
        waitsFor(function () {
            return callback;
        });
        runs(function () {
            expect($('.available-questions .children-list').html()).toEqual('QuestionSeach_SeachCurrentlyInUse_ExpectSearchResult');
        });

    });
    
    it('Seach currently not in use questions, Expect search result', function () {
        $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');

        $("#txtSearchQuiz").val('a');
        var callback = false;

        spyOn($, 'ajax').andCallFake(function (e) {
            e.success('QuestionSeach_SeachCurrentlyNotInUse_ExpectSearchResult');
            callback = true;
        });
        $('.question-list').questionSearch('searchQuestions', 'not-currently-in-use');

        waitsFor(function () {
            return callback;
        });
        runs(function () {
            expect($('.available-questions .children-list').html()).toEqual('QuestionSeach_SeachCurrentlyNotInUse_ExpectSearchResult');
        });
    });
    it('FilterQuestion, Expect questions filtered ', function () {
        $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');
        
        $('.question-filter').append($('<div class="select2-choices"></div>'));
        $('.select2-choices').html('<ul class="select2-choices" style="">  <li class="select2-search-choice">    <div>Essay</div>    <a href="#" onclick="return false;" class="select2-search-choice-close" tabindex="-1"></a></li><li class="select2-search-field">    <input type="text" autocomplete="off" class="select2-input" id="s2id_autogen5" style="width: 10px;">  </li></ul>');

        $("#txtSearchQuiz").val('a');
        var callback = false;
        spyOn($, 'ajax').andCallFake(function (e) {
            e.success('QuestionSeach_SeachCurrentlyNotInUse_ExpectSearchResult');
            callback = true;
        });
        $('.question-list').questionSearch('searchQuestions', 'txtSearchQuiz');
        waitsFor(function () {
            return callback;
        });
        runs(function () {
            expect($('.available-questions .children-list').html()).toEqual('QuestionSeach_SeachCurrentlyNotInUse_ExpectSearchResult');
        });

    });
    
    it('Scroll for more questions, Expect more question', function () {
        $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');
        var viewModel = TestQuizHelper.generateViewModel(TestQuizHelper.Model.generateQuizSearchResultsModel());
        TestQuizHelper.setFixtureFromView(viewModel, 'append');
        $("#txtSearchQuiz").val('a');
        var callback = false;
        spyOn($, 'ajax').andCallFake(function (e) {
            e.success('<div class="question-search-list">QuestionSeach_SeachCurrentlyNotInUse_ExpectSearchResult</div>');
            callback = true;
        });
        $('.question-list').questionSearch('searchQuestions', 'txtSearchQuiz');
        waitsFor(function () {
            return callback;
        });
        runs(function () {
            $.fn.scrollTop = jasmine.createSpy('scrollTop').andCallFake(function () {
                return 4000;
            });
            
            $('.question-search-list').trigger('scroll');
            expect($('.available-questions .children-list').html()).toEqual('<div class="question-search-list">QuestionSeach_SeachCurrentlyNotInUse_ExpectSearchResult</div>');
        });


    });
    
    it('seach empty text, Expect return nothing', function () {
        $.fn.questionListGearbox = jasmine.createSpy('questionListGearbox');
        $("#txtSearchQuiz").val('');

        $('.question-list').questionSearch('searchQuestions', 'txtSearchQuiz');

        runs(function () {
            expect($('.searchQuestion .search-category').html()).toEqual('');
        });
    });
});