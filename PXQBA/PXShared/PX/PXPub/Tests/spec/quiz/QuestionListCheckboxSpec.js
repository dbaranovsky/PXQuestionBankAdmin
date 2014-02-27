describe("Checkbox functionality in Quizzing Tests: ", function () {
    beforeAll(function () {
        TestQuizHelper.clearCache();
    });
    
    beforeEach(function () {
        $(document).unbind();
        TestQuizHelper.defaultSpyOn();
        var questions = TestQuizHelper.Model.generatePresetQuestion_NonEmptyQuestionPool();
        var model = TestQuizHelper.Model.generateQuizModel(questions);
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'set');
        model.path = 'Questions';
        viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor-questions');
        $(".question-list").show();

    });
    
    it('Initialization, Expect Success', function () {
        $('.question-list').first().questionlistcheckbox();
        var eventData = $(document).data('events');
        expect(eventData.click).toBeDefined();
        expect(eventData.click.length).toEqual(5);
        expect(eventData.change.length).toEqual(2);

    });
    
    it('Call undefined function, Expect Fail', function () {
        var initShouldFail = function () {
            $('.question-list').first().questionlistcheckbox('UndefinedFunction');
        };
        expect(initShouldFail).toThrow(new Error('Method UndefinedFunction does not exist on jQuery.questionlistcheckbox'));
    });
    
    it('Select all questions, Expect Success', function () {
        $('.question-list').first().questionlistcheckbox('init');        
        var totalNumberOfQuestions = $('.selected-questions ul.questions:visible li.question').length;
        $('.selected-questions .questions-actions input[name=selectall]').click();
        waitsFor(function () {
            return $('.selected-questions .questions-actions input[name=selectall]').prop('checked');
        });
        runs(function () {
            var selectedNumberOfQuestions = $('.selected-questions .selected-question-in-quiz-editor').length;
            expect(totalNumberOfQuestions).toEqual(selectedNumberOfQuestions);
            expect($('ul.questions:visible li.question .preview-current-question').hasClass('displayquestionmenu')).toBeTruthy();
            expect($('ul.questions:visible li.question .edit-current-question').hasClass('displayquestionmenu')).toBeTruthy();
            expect($('ul.questions:visible li.question .move-current-question').hasClass('displayquestionmenu')).toBeTruthy();
            expect($('ul.questions:visible li.question .delete-current-question').hasClass('displayquestionmenu')).toBeTruthy();
        });
        
    });
    
    it('Deselect all questions, Expect Success', function () {

        $('.question-list').first().questionlistcheckbox();
        $('.selected-questions .questions-actions input[name=selectall]').click();
        $('.selected-questions .questions-actions input[name=selectall]').click();
        var selectedNumberOfQuestions = $('.selected-questions .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(0);
        expect($('ul.questions:visible li.question .preview-current-question').hasClass('displayquestionmenu')).toBeFalsy();
        expect($('ul.questions:visible li.question .edit-current-question').hasClass('displayquestionmenu')).toBeFalsy();
        expect($('ul.questions:visible li.question .move-current-question').hasClass('displayquestionmenu')).toBeFalsy();
        expect($('ul.questions:visible li.question .delete-current-question').hasClass('displayquestionmenu')).toBeFalsy();

    });
    
    it('Select all available questions, Expect Success', function () {
        addAvailableQuestionView();
        
        $('.question-list').questionlistcheckbox();
        var totalNumberOfQuestions = $('.available-questions ul.questions:visible li.question').length;

        $('.available-questions #select-gearbox .select-all').click();
        var selectedNumberOfQuestions = $('.available-questions .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(totalNumberOfQuestions);
        expect($('.available-questions ul.questions:visible li.question .preview-available-question').hasClass('displayquestionmenu')).toBeTruthy();
        expect($('.available-questions ul.questions:visible li.question .add-to-pool-available-question').hasClass('displayquestionmenu')).toBeTruthy();

    });
    
    it('Deselect all available questions, Expect Success', function () {
        addAvailableQuestionView();
        
        $('.question-list').questionlistcheckbox();
        $('.available-questions #select-gearbox .select-all').click();
        $('.available-questions #select-gearbox .select-none').click();
        var selectedNumberOfQuestions = $('.available-questions .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(0);
        expect($('.available-questions ul.questions:visible li.question .preview-available-question').hasClass('displayquestionmenu')).toBeFalsy();
        expect($('.available-questions ul.questions:visible li.question .add-to-pool-available-question').hasClass('displayquestionmenu')).toBeFalsy();

    });
    
    it('Select all quiz questions in overview, Expect all question in overview got selected', function () {
        initQuizOverview();
        $('.question-list').questionlistcheckbox();
        var totalNumberOfQuestions = $('.quiz-overview ul.questions:visible li.question').length;
        $('.quiz-overview #select-gearbox-current .select-all').click();
        var selectedNumberOfQuestions = $('.selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(totalNumberOfQuestions);
        expect($('.quiz-overview ul.questions:visible li.question .preview-current-question').hasClass('displayquestionmenu')).toBeTruthy();
        expect($('.quiz-overview ul.questions:visible li.question .edit-current-question').hasClass('displayquestionmenu')).toBeTruthy();
        if ($('.quiz-overview ul.questions:visible li.question .move-current-question').length)
            expect($('.quiz-overview ul.questions:visible li.question .move-current-question').hasClass('displayquestionmenu')).toBeTruthy();
        expect($('.quiz-overview ul.questions:visible li.question .delete-current-question').hasClass('displayquestionmenu')).toBeTruthy();

    });
    
    it('Deselect all quiz questions in overview, Expect all questions not selected', function () {
        initQuizOverview();
        $('.question-list').questionlistcheckbox();
        $('.quiz-overview #select-gearbox-current .select-all').click();
        $('.quiz-overview #select-gearbox-current .select-none').click();
        var selectedNumberOfQuestions = $('.quiz-overview .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(0);
        expect($('.quiz-overview ul.questions:visible li.question .preview-current-question').hasClass('displayquestionmenu')).toBeFalsy();
        expect($('.quiz-overview ul.questions:visible li.question .edit-current-question').hasClass('displayquestionmenu')).toBeFalsy();
        if ($('.quiz-overview ul.questions:visible li.question .move-current-question').length)
            expect($('.quiz-overview ul.questions:visible li.question .move-current-question').hasClass('displayquestionmenu')).toBeFalsy();
        expect($('.quiz-overview ul.questions:visible li.question .delete-current-question').hasClass('displayquestionmenu')).toBeFalsy();

    });
    
    it('Select two available questions, Expect two questions selected', function () {
        addAvailableQuestionView();
        $('.question-list').questionlistcheckbox();
        $('.available-questions input[type=checkbox]').first().click();
        $('.available-questions input[type=checkbox]').last().click();

        var selectedNumberOfQuestions = $('.available-questions .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(2);
        
    });
    
    it('Deselect two available questions, Expect all questions are not selected.', function () {
        addAvailableQuestionView();
        $('.question-list').questionlistcheckbox();
        $('.available-questions input[type=checkbox]').first().click();
        $('.available-questions input[type=checkbox]').last().click();
        var selectedNumberOfQuestions = $('.available-questions .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(2);
        $('.available-questions input[type=checkbox]:checked').first().click();
        $('.available-questions input[type=checkbox]').last().click();


        selectedNumberOfQuestions = $('.available-questions .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(0);

    });
    
    it('Select two Questions from quiz overview, Expect two questions got selected', function () {
        initQuizOverview();
        $('.question-list').questionlistcheckbox();
        $('.quiz-overview input[type=checkbox]').first().click();
        $('.quiz-overview input[type=checkbox]').last().click();

        var selectedNumberOfQuestions = $('.quiz-overview .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(2);

    });

    it('Deselect two questions from quiz overview, Expect all question are not selected', function () {
        initQuizOverview();
        $('.question-list').questionlistcheckbox();
        $('.question-list .question').first().find(".question-text").removeClass('collapsed');
        $('.quiz-overview input[type=checkbox]').first().click();
        $('.quiz-overview input[type=checkbox]').last().click();
        $('.quiz-overview input[type=checkbox]').first().click();
        $('.quiz-overview input[type=checkbox]').last().click();
        var selectedNumberOfQuestions = $('.quiz-overview .selected-question-in-quiz-editor').length;
        expect(selectedNumberOfQuestions).toEqual(0);

    });

    function initQuizOverview() {

        var model = TestQuizHelper.Model.generateQuizModel();
        model.path = 'Quiz';
        model.ShowContentView = true;
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'set');
        $('.quiz-overview .select-menu-current').html('<ul id="select-gearbox-current" class="contextMenu" style="display: none; top: 304px; left: 185.5px;">' +
            '<li title="" class="select-all "><a href="javascript:" action_href="#select-all" rel="">All</a></li>' +
            '<li title="" class="select-none "><a href="javascript:" action_href="#select-none" rel="">None</a></li></ul>');
        $(".question-list").show();
    }
    
    function addAvailableQuestionView() {
        var model = TestQuizHelper.Model.generateQuizModel();
        model.ShowReused = true;
        model.path = 'QuestionList';
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor .children-list');
        $('.select-menu').html('<div class="gearbox"></div>' +
                    '<ul id="select-gearbox" class="contextMenu" style="display: block; top: 23px; left: 1px;">' +
                    '<li title="" class="select-all "><a href="javascript:" action_href="#select-all" rel="">All</a></li><li title="" class="select-none "><a href="javascript:" action_href="#select-none" rel="">None</a></li><li title="" class="select-used-previously "><a href="javascript:" action_href="#select-used-previously" rel="">Used in previous quiz</a></li><li title="" class="select-never-used "><a href="javascript:" action_href="#select-never-used" rel="">Never used before</a></li><li title="" class="select-random "><a href="javascript:" action_href="#select-random" rel="">Randomly select x questions</a></li></ul>');
        $(".question-list").show();
    }
});