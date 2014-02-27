describe("Test Question Filter functionality in Quizzing", function () {
    beforeAll(function () {
        TestQuizHelper.clearCache();
        TestQuizHelper.defaultSpyOn();
        PxPage.QuestionFilter = {};
        PxPage.QuestionFilter.FilterMetadata = [{ Filterable: true, FriendlyName: 'Difficulty', Name: 'difficulty', Seachterm: 'difficulty:' },
        { Filterable: true, FriendlyName: 'Cognitive Level', Name: 'cognitivelevel', Seachterm: 'cognitivelevel:' },
        { Filterable: true, FriendlyName: 'Core Concept', Name: 'coreconcept', Seachterm: 'coreconcept:' },
        { Filterable: true, FriendlyName: "Bloom's level", Name: 'bloomdomain', Seachterm: 'bloomdomain:' },
        { Filterable: true, FriendlyName: 'Suggested Use', Name: 'suggesteduse', Seachterm: 'suggesteduse:' },
        { Filterable: true, FriendlyName: 'Guidance', Name: 'guidance', Seachterm: 'guidance:' }];
        
    });
    beforeEach(function () {
        $(document).unbind();

        var questions = TestQuizHelper.Model.generatePresetQuestion_WithSearchableMetaData();
        var model = TestQuizHelper.Model.generateQuizModel(questions);
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'set');
        model.path = 'Questions';
        viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor-questions');
        model.ShowReused = true;
        model.path = 'QuestionList';
        viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.available-questions .children-list');
        var thisTextIsLongerThan50Character = '123456789012345678901234567890123456789012345678901234567890';
        var question1Metadata = $('li#QuestionMC2 .question-metadata').html().replace('tobereplaced', thisTextIsLongerThan50Character);
        $('li#QuestionMC2 .question-metadata').html(question1Metadata);
    });
    
    it('Initialize question filter: Expect success', function () {
        $('.question-list').questionfilter();
        var events = $(document).data('events');
        expect(PxPage.FneInitHooks['quiz-editor']).toBeDefined();
        expect(events.click).toBeDefined();
        expect(events.showfiltermetadata).toBeDefined();
        expect(events.updatequestionfilter).toBeDefined();

        expect(events.click.length).toEqual(2);
        expect(events.showfiltermetadata.length).toEqual(1);
        expect(events.updatequestionfilter.length).toEqual(1);

    });
    
    it('Call undefined function: Expect Fail', function () {
        var initShouldFail = function () {
            $('.question-list').first().questionfilter('UndefinedFunction');
        };
        expect(initShouldFail).toThrow(new Error('Method UndefinedFunction does not exist on jQuery.questionfilter'));
    });
    
    it('Fne init: Expect PxQuiz.UpdateAddedQuestions is called', function () {
        $('.question-list').first().questionfilter('init');
        PxPage.FneInitHooks['quiz-editor']();
        expect(PxQuiz.UpdateAddedQuestions).toHaveBeenCalled();
    });

    describe('Test link "Show/Hide Filters"', function() {
        it('Click on "Show Filters": Expect div "question-filter" to be shown', function () {
            $('.question-list').questionfilter();
            $('.available-questions .show-filter-available-question').click();
            expect($('.available-questions .question-filter').hasClass('collapsed')).toBeFalsy();
            expect($('.available-questions .filter-count-available-question').text()).toEqual('');
        });
        
        it('Click on "Hide Filters" with no filter selected: Expect div "question-filter" to be hidden', function () {
            $('.question-list').questionfilter();
            $('.show-filter-available-question').first().click();
            $('.show-filter-available-question').first().click();
            expect($('.available-questions .question-filter').hasClass('collapsed')).toBeTruthy();
            expect($('.available-questions .filter-count-available-question').text()).toEqual('');
        });
        
        it('Click on "Hide Filters" with one filter selected: Expect div "question-filter" to be hidden', function () {
            $('.question-list').questionfilter();
            $('.show-filter-available-question').first().click();
            $('#question-filter-questiontype').select2('val', ['Essay']);
            $('.show-filter-available-question').first().click();
            
            expect($('.available-questions .question-filter').hasClass('collapsed')).toBeTruthy();
            expect($('.available-questions .filter-count-available-question').text()).toEqual('(1 filter applied)');
        });
        
        it('Click on "Hide Filters" with two filter selected: Expect div "question-filter" to be hidden', function () {
            $('.question-list').questionfilter();
            $('.show-filter-available-question').first().click();
            $('#question-filter-questiontype').select2('val', ['Essay']);
            $('#question-filter-bloomdomain').select2('val', ['Analysis']);
            $('.show-filter-available-question').first().click();

            expect($('.available-questions .question-filter').hasClass('collapsed')).toBeTruthy();
            expect($('.available-questions .filter-count-available-question').text()).toEqual('(2 filters applied)');
        });
    });

    describe('Test filter', function () {
        it('Added question type filter "Essay": Expect all non-essay question filtered', function () {
            $('.question-list').questionfilter();
            $('#question-filter-questiontype').select2('val', ['ESSAY']);
            $(".question-filter-metadata").first().click();
            expect($('.filteredout').length).toEqual(1);
            expect($('li.essay').hasClass('filteredout')).toBeFalsy();

        });
        
        it('Removed question type filter "Essay": Expect all question to be shown', function () {
            $('.question-list').questionfilter();
            $('#question-filter-questiontype').select2('val', ['ESSAY']);
            $(".question-filter-metadata").first().click();
            expect($('.filteredout').length).toEqual(1);
            expect($('li.essay').hasClass('filteredout')).toBeFalsy();
            $('#question-filter-questiontype').select2('val', 'ESSAY');
            $(".question-filter-metadata").first().click();
            expect($('.filteredout').length).toEqual(0);
            expect($('li.essay').hasClass('filteredout')).toBeFalsy();

        });
        
        it('Added difficulty filter "High": Expect all question filtered', function () {
            $('.question-list').questionfilter();
            $('#question-filter-difficulty').select2('val', ['High']);
            $(".question-filter-metadata").first().click();
            expect($('.filteredout').length).toEqual(2);
            expect($('li.essay').hasClass('filteredout')).toBeTruthy();

        });
        
        it('Added difficulty filter "High" with invalid metadata: Expect question with invalid metadata filtered ', function () {
            $('.question-list').questionfilter();
            $('#QuestionMC1 .question-text .question-metadata').html('invalid data');
            $('#question-filter-difficulty').select2('val', ['High']);
            $(".question-filter-metadata").first().click();
            expect($('li#QuestionMC1').hasClass('filteredout')).toBeTruthy();

        });
    });
    describe('Test search with filter', function() {
        it('Search with selected filter "Essay": Expect the filter "question-filter-questiontype" be added back ', function () {
            $('.question-list').questionfilter();
            $('#question-filter-questiontype').select2('val', ['ESSAY']);
            //$('#question-filter-questiontype').remove();
            $(PxPage.switchboard).bind('showfiltermetadata',function() {
                PxPage.QuestionFilter.FilterMetadata[0].Name = 'question-filter-questiontype';
                $('#question-filter-questiontype').remove();
            });
            $(PxPage.switchboard).trigger("updatequestionfilter", [".available-questions "]);
            expect($('#question-filter-questiontype').length).toEqual(1);
        });
    });
});