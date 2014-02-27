describe("Question Importer Tests: ", function () {
    beforeAll(function () {
        TestQuizHelper.clearCache();
    });
    beforeEach(function () {
        $(document).unbind();
        TestQuizHelper.defaultSpyOn();
        $.fn.qtip = jasmine.createSpy('qtip');
        PxPage.switchboard = 'body';
        var questions = TestQuizHelper.Model.generatePresetQuestion_NonEmptyQuestionPool();
        var model = TestQuizHelper.Model.generateQuizModel(questions);
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'set');
        model.path = 'Questions';
        viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor-questions');
        model.path = 'ImporterDialog';
        viewModel = TestQuizHelper.generateViewModel(model);

        $('.question-list').first().append('<div id="temp-holder"></div><div id="pxpage-toasts-error"></div>');
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '#temp-holder');

    });
    
    it('Initialize question importer, Expect success', function () {
        $(PxPage.switchboard).unbind();
        $(".question-list").questionimport();
        var eventData = $(PxPage.switchboard).data('events');
        expect(eventData.showimporterdialog).toBeDefined();
        
        expect(eventData.showimporterdialog.length).toEqual(1);
    });
    
    
    it('Call undefined function, Expect Fail', function () {
        var initShouldFail = function () {
            $('.question-list').first().questionimport('UndefinedFunction');
        };
        expect(initShouldFail).toThrow(new Error('Method UndefinedFunction does not exist on jQuery.questionimport'));
    }); 

    describe('Test question importer dialog, ', function () {
        afterEach(function () {
            destroyDialog();
        });
        it('Trigger event "showimporterdialog.questionimport", Expect dialog shows up', function () {
            setupDialog('SUCCESS');

            expect(!!$('.ui-dialog').length).toBeTruthy();
            expect($('.ui-dialog-buttonset button').first().hasClass('importer-button-import')).toBeTruthy();
            expect($($('.ui-dialog-buttonset button').get(1)).hasClass('importer-button-test')).toBeTruthy();

        });
        
        it('Click "Cancel", Expect dialog closes', function () {
            setupDialog('SUCCESS');
            $('.ui-dialog-buttonset button').get(2).click();
            expect($('.ui-dialog').length).toEqual(0);
        });
        
        it('Click "importer-help-link", Expect to show sample questions', function () {
            setupDialog('SUCCESS');
            $('.importer-sample-text').html('');
            $('.importer-help-link').first().click();
            expect($('.importer-help-area').is(':visible')).toBeTruthy();
        });
        
        it('Click "importer-help-link" again, Expect to hide sample questions', function () {
            setupDialog('SUCCESS');
            $('.importer-help-link').first().click();
            $('.importer-help-link').first().click();

            expect($('.importer-help-area').is(':visible')).toBeFalsy();
        });
        
        it('Click "importer-sample-fill-in-blank", Expect to see "Fill in the blank" sample question', function () {
            setupDialog('SUCCESS');
            $('.importer-help-link').first().click();
            $('.importer-sample-fill-in-blank').first().click();
            
            expect($('.importer-sample-fill-in-blank').hasClass('active')).toBeTruthy();
            expect($('.importer-sample-fill-in-blank-placeholder').is(':visible')).toBeTruthy();
        });
        
        it('Click "importer-sample-multiple-choice", Expect to see "Multiple choice" sample question', function () {
            setupDialog('SUCCESS');
            $('.importer-help-link').first().click();
            $('.importer-sample-multiple-choice').first().click();
            
            expect($('.importer-sample-multiple-choice').hasClass('active')).toBeTruthy();
            expect($('.importer-sample-multiple-choice-placeholder').is(':visible')).toBeTruthy();
        });
        
        it('Click "Test questions" with valid question, Expect display "No errors found"', function () {
            setupDialog('SUCCESS');
            $('.ui-dialog-buttonset button').get(1).click();
            expect($('.importer-success-header').html()).toEqual('No errors found');
        });
        
        it('Click "Test questions" with valid question, Expect ajax do "POST" instead of "GET"', function () {
            setupDialog('SUCCESS');
            var ajaxUsePost;
            $.ajax.andCallFake(function (e) {
                ajaxUsePost = e.type === "POST";
            });
            $('.ui-dialog-buttonset button').get(1).click();

            expect(ajaxUsePost).toBeTruthy();
        });
        
        it('Click "Test questions" with empty content, Expect error "Please provide text to parse!"', function () {
            setupDialog('SUCCESS');
            $('#importer-text').val('');
            $('.ui-dialog-buttonset button').get(1).click();
            expect($('#pxpage-toasts-error').html()).toEqual('Please provide text to parse!');
        });
        
        it('Click "Test questions" with invalid question, Expect error "Missing correct answer exception encountered" in line 1', function () {
            setupDialog(['Missing correct answer exception encountered|1|Each Queasdfasf']);
            $('#importer-text').val('123123123123');
            $('.ui-dialog-buttonset button').get(1).click();
            expect($('.importer-error-body div').first().html()).toEqual('1');
            expect($($('.importer-error-body div').get(1)).html()).toEqual('line Missing correct answer exception encountered');
        });
        it('Click "Close" on Error message, Expect error message removes', function () {
            setupDialog(['Missing correct answer exception encountered|1|Each Queasdfasf']);
            $('#importer-text').val('123123123123');
            $('.ui-dialog-buttonset button').get(1).click();
            expect($('.importer-error-body div').first().html()).toEqual('1');
            expect($($('.importer-error-body div').get(1)).html()).toEqual('line Missing correct answer exception encountered');
            $('.importer-error-close').click();
            expect($('.importer-response').html()).toEqual('');
        });
        
        it('Click "Import Question" with empty Content, Expect error "Please provide text to parse!"', function () {
            setupDialog('SUCCESS');
            $('#importer-text').val('');
            $('.ui-dialog-buttonset button').get(0).click();
            expect($('#pxpage-toasts-error').html()).toEqual('Please provide text to parse!');
        });
        
        it('Click "Import Question" with invalid question, Expect error "Missing correct answer exception encountered" in line 1', function () {
            setupDialog(['Missing correct answer exception encountered|1|Each Queasdfasf']);
            $('#importer-text').val('123123123123');
            $('.ui-dialog-buttonset button').get(0).click();
            expect($('.importer-error-body div').first().html()).toEqual('1');
            expect($($('.importer-error-body div').get(1)).html()).toEqual('line Missing correct answer exception encountered');
        });
        
        it('Click "Import Question" with valid question, Expect display "No errors found"', function () {
            setupDialog('SUCCESS');
            $('.ui-dialog-buttonset button').get(0).click();
            expect(PxQuiz.UpdateQuestionList).toHaveBeenCalled();
        });
        
        it('Click "Import Question" with valid question, Expect ajax do "POST" instead of "GET"', function () {
            setupDialog('SUCCESS');
            var ajaxUsePost;
            $.ajax.andCallFake(function (e) {
                ajaxUsePost = e.type === "POST";
            });
            $('.ui-dialog-buttonset button').get(0).click();
            expect(ajaxUsePost).toBeTruthy();
        });
    });
    
    function setupDialog(message) {
        PxPage.switchboard = 'body';
        var called = false;
        spyOn($, 'get').andCallFake(function (url, callback) {
            var content = !!$('#temp-holder').length ? $('#temp-holder').html() : 'Dummy content';
            $('#temp-holder').remove();
            callback(content);
            called = true;

        });
        $('.question-list').first().attr('tooltip', '');
        $(".question-list").questionimport('init');

        $(PxPage.switchboard).trigger('showimporterdialog.questionimport');
        waitsFor(function () {
            return called;
        });
        $('#importer-dialog').first().append($('<div id="pxpage-toasts-error"></div>'));
        spyOn($, 'ajax').andCallFake(function (e) {
            e.success(message);
            e.complete();

        });
    }
    function destroyDialog() {
        if (!!$('.ui-dialog-content').length) {
            $('.ui-dialog-content').dialog('destroy');
            $('.ui-dialog ').remove();
        }
        $('.question-pool-dialog-text').remove();
    }
});