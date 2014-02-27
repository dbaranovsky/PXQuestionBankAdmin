describe('Quiz Hts functionalities Tests: ', function () {
    beforeAll(function () {
        TestQuizHelper.clearCache();
        TestQuizHelper.defaultSpyOn();
        TestQuizHelper.spyOnEasyXdm(true );
        $.fn.questioneditor = jasmine.createSpy('questioneditor');
    });
    beforeEach(function () {
        setFixtures('<div id="main">' +
            '<div id="px-log"></div><div id="custom-hts-editor"></div>'+
            '</div>');
        PxPage.log.andCallFake(function(text) {
            $('#px-log').html($('#px-log').html() + text);
        });
    });

    it('Expect exception should be handled in GetDataForGraphQuestion', function() {

        var originalMethod = document.getElementById;
        spyOn(document, 'getElementById').andCallFake(function (element) {
            if (element === "flash") {
                document.getElementById = originalMethod;

                throw "Test exception";
            }
        });
        
        PxQuizHts.GetDataForGraphQuestion('randomQuestionId');

        expect($('#px-log').html()).toEqual("ending GetDataForGraphQuestion | could not retrieve XML for custom question:randomQuestionIdending GetDataForGraphQuestion | returns xml");
    });

    describe('Test Quiz Hts SetHtsRpcHooks', function() {
        it('SetHtsRpcHooks should be initialize successfully: Expect OK', function () {
            PxQuizHts.SetHtsRpcHooks();
            expect(easyXDM.Rpc).toHaveBeenCalled();
        });
        it('test questionSaved in SetHtsRpcHooks: Expect OK', function () {
            easyXDM.Rpc.andCallFake(function (local, config) {
                config.local.questionSaved('questionId', function(){},1231236);
            });
            spyOn($, 'post').andCallFake(function (method, promises, callback) {
                callback('success');
            });
            PxQuizHts.SetHtsRpcHooks();
            expect(easyXDM.Rpc).toHaveBeenCalled();
        });
    });
    
    describe('Test Quiz Hts StoreAndPreviewGraphQuestion', function () {
        var originalFunction = $.fn.dialog;
        beforeAll(function () {
            originalFunction = $.fn.dialog;
        });
        afterAll(function () {
            $.fn.dialog = originalFunction;
        });
        it('Called StoreAndPreviewGraphQuestion on FMA_GRAPH question, Expect dialog shows up with title "Graph Exercise Preview"', function () {
            $.fn.dialog = jasmine.createSpy('dialog').andCallFake(function(config) {
                expect(config.title).toEqual('Graph Exercise Preview');
            });
            spyOn($, 'ajax').andCallFake(function (settings) {
                settings.success('a');
            });
            PxQuizHts.StoreAndPreviewGraphQuestion('FMA_GRAPH');

        });
        
        it('Called StoreAndPreviewGraphQuestion on FMA_GRAPH question, Expect dialog shows up with title "Graph Exercise Preview"', function () {
            $.fn.dialog = jasmine.createSpy('dialog').andCallFake(function (config) {
                expect(config.title).toEqual('Advanced Question Preview');
            });
            spyOn($, 'ajax').andCallFake(function (settings) {
                settings.success();
            });
            PxQuizHts.StoreAndPreviewGraphQuestion();

        });
        
        it('Click on "RegenerateVariables" button in "Graph Exercise Preview" dialog, Expect dialog refreshes', function () {
            $.fn.dialog = jasmine.createSpy('dialog').andCallFake(function (config) {
                expect(config.buttons.RegenerateVariables).toBeDefined();
                config.buttons.RegenerateVariables.click();
            });
            spyOn($, 'ajax').andCallFake(function (settings) {
                settings.success('refreshed');
            });
            PxQuizHts.StoreAndPreviewGraphQuestion();

        });
        
        it('Click on "Close" button in "Graph Exercise Preview" dialog, Expect dialog closes', function () {
            var dialogDestroyed = false;
            $.fn.dialog = jasmine.createSpy('dialog').andCallFake(function (config) {
                if (config === 'destroy') {
                    dialogDestroyed = true;
                    return $('<div></div>');
                } else {
                    expect(config.close).toBeDefined();
                    config.close();
                }
            });
            spyOn($, 'ajax').andCallFake(function (settings) {
                settings.success('refreshed');
            });
            PxQuizHts.StoreAndPreviewGraphQuestion();
            expect(dialogDestroyed).toBeTruthy();

        });
    });
});