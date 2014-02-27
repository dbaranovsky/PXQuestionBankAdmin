describe('Quiz Homework functionalities Tests: ', function () {
    beforeAll(function () {
        TestQuizHelper.clearCache();
        TestQuizHelper.defaultSpyOn();
    });
    afterAll(function() {
        
    });
    beforeEach(function () {
        $(document).unbind();
        var model = TestQuizHelper.Model.generateQuizModel();
        model.path = 'DisplayQuestionList';
        model.QuizType = 'Homework';
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'set');
        $('.question-list').append($('<div id="pxpage-toasts-error"></div>'));
        $(".question-list").show();
        $.fn.block = jasmine.createSpy('block');
        $.fn.unblock = jasmine.createSpy('unblock');
        $.fn.questionlist = jasmine.createSpy('questionlist');
    });

    it('Initialization, Expect Success', function () {
        $('.question-list').first().quizhomework('init');
        var eventData = $(document).data('events');
        expect(eventData.click).toBeDefined();
        expect(eventData.focusout).toBeDefined();
        expect(eventData.click.length).toEqual(1);
        expect(eventData.focusout.length).toEqual(1);

    });

    it('Call undefined function, Expect Fail', function () {
        var initShouldFail = function () {
            $('.question-list').first().quizhomework('UndefinedFunction');
        };
        expect(initShouldFail).toThrow(new Error('Method UndefinedFunction does not exist on jQuery.quizhomework'));
    });
    
    it('Click on attempt limit, Expect to let user to change attempt limit', function () {
        $('.question-list').first().quizhomework();
        $('ul.questions li.question#0 .total-attempts a.link-attempt').first().click();
        expect($('li.question#0 .attempt-label')).toBeHidden();
        expect($('li.question#0 .attempt-textbox')).not.toBeHidden();
    });
    
    it('Modified attempt limit with invalid number, Expect error', function () {
        $('.question-list').first().quizhomework();
        $('ul.questions li.question#0 .total-attempts a.link-attempt').first().click();
        $('ul.questions li.question#0 .questions-attempts option').first().val('abc');
        $('ul.questions li.question#0 .questions-attempts').val('abc');
        $('ul.questions li.question#0 .questions-attempts').trigger('focusout');
        expect($('#pxpage-toasts-error').html()).toEqual('Please enter a numeric value');
    });
    
    it('Modified attempt limit to 1 attempt, Expect sucess', function () {
        $('.question-list').first().quizhomework();
        var ajaxCalled = false;
        spyOn($, 'post').andCallFake(function (url, data, success) {
            expect(data.quizId).toBeDefined();
            expect(data.questionId).toBeDefined();
            expect(data.attempts).toBeDefined();
            expect(data.entityId).toBeDefined();
            success();
            ajaxCalled = true;
        });
        $('ul.questions li.question#0 .questions-attempts').val('1');
        $('ul.questions li.question#0 .questions-attempts').trigger('focusout');
        waitsFor(function() {
            return ajaxCalled;
        });
        runs(function() {
            expect($.fn.block).toHaveBeenCalled();
            expect($.fn.unblock).toHaveBeenCalled();
            expect($('ul.questions li.question#0 .attempt-label').text()).toEqual('1 attempt');
            expect($('ul.questions li.question#0 .attempt-label')).not.toBeHidden();
            expect($('ul.questions li.question#0 .attempt-textbox')).toBeHidden();


        });


    });
    
    it('Modified attempt limit to 1 attempt, Expect sucess', function () {
        $('.question-list').append($('<div id="fne-window" class="require-confirm"></div>'));
        $('.question-list').first().quizhomework();
        var ajaxCalled = false;
        spyOn($, 'post').andCallFake(function (url, data, success) {
            expect(data.quizId).toBeDefined();
            expect(data.questionId).toBeDefined();
            expect(data.attempts).toBeDefined();
            expect(data.entityId).toBeDefined();
            success();
            ajaxCalled = true;
        });
        $('ul.questions li.question#0 .questions-attempts').val('0');
        $('ul.questions li.question#0 .questions-attempts').trigger('focusout');
        waitsFor(function () {
            return ajaxCalled;
        });
        runs(function () {
            expect($.fn.block).toHaveBeenCalled();
            expect($.fn.unblock).toHaveBeenCalled();
            expect($.fn.questionlist).toHaveBeenCalled();
            expect($('ul.questions li.question#0 .attempt-label').text()).toEqual('Unlimited attempts');
            expect($('ul.questions li.question#0 .attempt-label')).not.toBeHidden();
            expect($('ul.questions li.question#0 .attempt-textbox')).toBeHidden();
            expect($('#fne-window').hasClass('require-confirm')).toBeFalsy();


        });


    });
});