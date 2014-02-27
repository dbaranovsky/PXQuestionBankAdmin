describe("Gearbox functionality in Quizzing Tests: ", function () {
    beforeAll(function () {
        TestQuizHelper.clearCache();
    });

    beforeEach(function () {
        $(document).unbind();
        TestQuizHelper.defaultSpyOn();
        var model = TestQuizHelper.Model.generateQuizModel();
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'set');
        model.path = 'Questions';
        viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor-questions');
        model.ShowReused = true;
        model.path = 'QuestionList';
        viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromCache(viewModel, 'target', '.quiz-editor .children-list');
        $(".question-list").show();
    });

    it('Initialization, Expect success', function () {
        $('.question-list').first().questionListGearbox();
        var eventData = $(document).data('events');
        expect(eventData.click).toBeDefined();
        expect(eventData.click.length).toEqual(2);

    });

    it('Call undefined function, Expect fail', function () {
        var initShouldFail = function () {
            $('.question-list').first().questionListGearbox('UndefinedFunction');
        };
        expect(initShouldFail).toThrow(new Error('Method UndefinedFunction does not exist on jQuery.questionListGearbox'));

    });
    it('Select one random question, Expect one question got selected', function () {
        $('.question-list').first().questionListGearbox();
        $('#divRandomSelect #txtRandomSelect').val('1');
        $('#btnRandomSave').click();
        var selectedQuestions = $("ul.questions:visible li.question [name=selected]:checked, li.quiz-item:visible [name=selected]:checked").length;
        expect($('#divRandomSelect #spnNameError').is(':visible')).toBeFalsy();
        expect(selectedQuestions).toEqual(1);
    });

    it('Select two random question, All questions got selected', function () {
        $('body').attr('id', 'fne-content');

        $('.question-list').first().questionListGearbox();
        $('#divRandomSelect #txtRandomSelect').val('2');
        $('#btnRandomSave').click();
        var selectedQuestions = $("ul.questions:visible li.question [name=selected]:checked, li.quiz-item:visible [name=selected]:checked").length;
        expect(selectedQuestions).toEqual(2);
    });

    it('Select five random questions, Expect error, "Enter a number lower than the total questions available"', function () {
        $('.question-list').first().questionListGearbox();
        $('#divRandomSelect #txtRandomSelect').val('5');
        $('#btnRandomSave').click();
        var errorText = $('#divRandomSelect #spnNameError').text();
        expect(errorText).toEqual('Enter a number lower than the total questions available');
    });

    it('Select "empty" number of questions, Expect error, "Please enter a number"', function () {
        $('.question-list').first().questionListGearbox();
        $('#btnRandomSave').click();
        var errorText = $('#divRandomSelect #spnNameError').text();
        expect(errorText).toEqual('Please enter a number');

    });

    it('Select "invalid" number of questions, Expect error, "Please enter a numeric value"', function () {
        $('.question-list').first().questionListGearbox();
        $('#divRandomSelect #txtRandomSelect').val('Five');
        $('#btnRandomSave').click();
        var errorText = $('#divRandomSelect #spnNameError').text();
        expect(errorText).toEqual('Please enter a numeric value');

    });

    //#region test the gearbox with class "add-menu" used in preveiw question dialog
    describe('Test the "add-menu" gearbox in question preview dialog ', function () {
        beforeEach(function () {
            $('li#0.question').addClass('question-being-previewed');

            $('.question-list').first().questionListGearbox('initQuestionGearbox');
            $('.add-menu .gearbox').click();
        });
        it('Click on option "Add" in the question menu, Expect method "addSelected" in questionlist is called', function () {
            var methodCalled = false;
            $.fn.questionlist = jasmine.createSpy('questionlist').andCallFake(function (method) {
                methodCalled = methodCalled || method == 'addSelected';
            });


            $('.add-menu .add-to-quiz').click();

            expect(methodCalled).toBeTruthy();

        });
        it('Click on option "Add to Existing pool" in the question menu, Expect method "addQuestionsToPool" in questionlist is called', function () {
            var methodCalled = false;
            var correctPoolId = false;
            $.fn.questionlist = jasmine.createSpy('questionlist').andCallFake(function (method, questionId, poolId) {
                methodCalled = methodCalled || method == 'addQuestionsToPool';
                correctPoolId = correctPoolId || poolId === '0';
            });

            $('.add-menu .add-to-pool-0').click();

            expect(methodCalled).toBeTruthy();
            expect(correctPoolId).toBeTruthy();


        });
        describe('Test option "Add to new pool" ', function () {
            beforeEach(function () {
            });

            it('Click on option "Add to new pool", Expect dialog pops up', function () {
                initQuestionGearbox();

                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                destroyCreateQuestionPoolDialog();
            });

            it('Click on option "Add to new pool with 1 checked questions", Expect dialog pops up', function () {
                $('ul.questions .select input').first().prop('checked', true);
                $('ul.questions .select input').last().prop('checked', true);
                $('.question-list').first().questionListGearbox('initQuestionGearbox');
                $('.add-menu .gearbox').click();
                $('.add-menu .add-to-new-pool').click();
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                destroyCreateQuestionPoolDialog();
            });

            it('Click on option "Add to new pool with 1 checked questions", Expect dialog pops up with defined html', function () {
                $('ul.questions .select input').first().prop('checked', true);
                $('ul.questions .select input').last().prop('checked', true);
                $('.question-list').first().questionListGearbox('initQuestionGearbox');
                $('.add-menu .gearbox').click();
                $('.add-menu .add-to-new-pool').click();
                var expectedHtml = $('#question-pool-dialog-container').html();
                var actualHtml = $('#show-question-pool-dialog').html();
                expect(actualHtml).toEqual(expectedHtml);
                destroyCreateQuestionPoolDialog();
            });
            
            it('Click on "Cancel" button  in the "Add to new pool" dialog, Expect dialog closes', function () {
                initQuestionGearbox();
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                $('.ui-dialog-buttonset').first().find('.ui-button').get(1).click();
                expect($('.question-pool-dialog-text').is(':visible')).toBeFalsy();
                destroyCreateQuestionPoolDialog();
            });

            it('Click on "Create" button in the "Add to new pool" dialog with no Title, Expect error', function () {
                initQuestionGearbox();
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                $('.ui-dialog-buttonset').first().find('.ui-button').get(0).click();
                expect($('.pool_title_error').is(':visible')).toBeTruthy();
                destroyCreateQuestionPoolDialog();
            });

            it('Click on "Create" button in the "Add to new pool" dialog with empty pool count, Expect error', function () {
                initQuestionGearbox();
                $('.question-pool-dialog-text #pool_title').val('test pool');
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                $('.ui-dialog-buttonset').first().find('.ui-button').get(0).click();
                expect($('.question-pool-dialog-text #spn-pool-count-error').is(':visible')).toBeTruthy();
                destroyCreateQuestionPoolDialog();
            });

            it('Click on "Create" button in the "Add to new pool" dialog with invalid pool count, Expect error', function () {
                initQuestionGearbox();
                $('.question-pool-dialog-text #pool_title').val('test pool');
                $('.question-pool-dialog-text #pool-count').val('ABC');
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                $('.ui-dialog-buttonset').first().find('.ui-button').get(0).click();
                expect($('.question-pool-dialog-text #spn-pool-integer-error').is(':visible')).toBeTruthy();
                destroyCreateQuestionPoolDialog();
            });

            it('Click on "Create" button in the "Add to new pool" dialog with already existed pool, Expect error', function () {
                initQuestionGearbox();
                $('body').append($('<div id="pxpage-toasts-error" style="display:none"></div>'));
                $('.question-pool-dialog-text #pool_title').val('test pool');
                $('.question-pool-dialog-text #pool-count').val('1');
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                var callback = false;

                spyOn($, 'ajax').andCallFake(function (e) {
                    e.success('True');
                    callback = true;
                });
                $('.ui-dialog-buttonset').first().find('.ui-button').get(0).click();

                waitsFor(function () {
                    return callback;
                });
                runs(function () {
                    expect($('#pxpage-toasts-error').html()).toEqual('Pool already exist. Please choose another title.');
                });
                destroyCreateQuestionPoolDialog();
            });

            it('Click on "Create" button in the "Add to new pool" dialog with valid info, Expect question pool with title "test pool" is created', function () {
                initQuestionGearbox();
                $('.question-pool-dialog-text #pool_title').val('test pool');
                $('.question-pool-dialog-text #pool-count').val('1');
             
                PxPage.Routes.validate_pool_title = '';
                var eventTriggered = false;
                var addedQuestionToPool = false;
                PxPage.switchboard = 'body';
                $(PxPage.switchboard).bind('questionpool-created', function () {
                    eventTriggered = true;
                });
                PxPage.Routes.validate_pool_title = 'validate title';
                PxPage.Routes.add_new_pool = 'add pool';
                spyOn($, "post").andCallFake(function (url, data, callback) {
                    if (url == 'add pool') {
                        expect(data.parentid).toBeDefined();
                        expect(data.title).toBeDefined();
                        expect(data.poolCount).toBeDefined();

                    }
                    callback();

                });
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method, args, callback) {
                    callback();
                });
                $.fn.questionlist = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    addedQuestionToPool = addedQuestionToPool || method == 'addQuestionsToPool';
                });

                $('.ui-dialog-buttonset').first().find('.ui-button').get(0).click();
                waitsFor(function () {
                    return eventTriggered;
                });
                runs(function () {
                    expect(eventTriggered).toBeTruthy();
                    expect(addedQuestionToPool).toBeTruthy();

                    destroyCreateQuestionPoolDialog();
                });
            });

        });
    });
    //#endregion test the gearbox with class "add-menu" used in preveiw question dialog

    //#region test the gearbox with class "move-current-question" used in selected-question-list
    describe('Test the gearbox with class "move-current-question" used in selected-question-list ', function () {
        beforeEach(function() {
            $('.question-list').first().questionListGearbox('updateAddMenu');
            $('.move-current-question .gearbox').click();

        });
        it('Click on option "Move to top" in the question menu, Expect method "moveToTopCurrentQuestion" in questionlist is called', function () {
            var methodCalled = false;
            $.fn.questionlist = jasmine.createSpy('questionlist').andCallFake(function (method) {
                methodCalled = methodCalled || method == 'moveToTopCurrentQuestion';
            });
            
            $('.move-current-question .move-to-top').first().click();

            expect(methodCalled).toBeTruthy();
        });

        it('Click on opton " Move to bottom" in the question menu, Expect method "moveToBottomCurrentQuestion" in questionlist is called', function () {
            var methodCalled = false;
            $.fn.questionlist = jasmine.createSpy('questionlist').andCallFake(function (method) {
                methodCalled = methodCalled || method == 'moveToBottomCurrentQuestion';
            });

            $('.move-current-question .move-to-bottom').first().click();

            expect(methodCalled).toBeTruthy();
        });
    });
    //#endregion test the gearbox with class "move-current-question" used in selected-question-list

    //#region test the gearbox dispalyed on top of avilable-question-list
    describe('Test the gearbox dispalyed on top of avilable-question-list ', function () {

        describe('Test the "Add to pool" gear box, ', function () {
            it('Click on "Add to pool", Expected a drop down list with 2 options appears', function() {
                $('.question-list').first().questionListGearbox('updateAddMenu');
                $('.add-menu .gearbox').click();
                expect($('.available-questions .add-menu li').length).toEqual(2);
                expect($('.available-questions .add-menu li.add-to-pool-0').length).toEqual(1);

            });

            it('select option "add to pool", expect a dialog pops up', function() {
                $('.question-list').first().questionListGearbox('updateAddMenu');
                $('.add-menu .gearbox').click();
                expect($('.available-questions .add-menu li').length).toEqual(2);
                expect($('.available-questions .add-menu li.add-to-pool-0').length).toEqual(1);
                $('.available-questions ul.questions .select input').prop('checked', true);
                $('.add-menu .add-to-new-pool').click();
                expect($('#show-question-pool-dialog').hasClass('ui-dialog-content')).toBeTruthy();
                $('#show-question-pool-dialog').dialog('destroy');
                $('#show-question-pool-dialog').remove();
            });


            it('list of pools should be updated when a new pool is created', function() {
                $('.question-list').first().questionListGearbox('updateAddMenu');
                $('.available-questions .add-menu .gearbox').click();
                expect($('.available-questions .add-menu li').length).toEqual(2);
                //add pool to selected questions
                $('.selected-questions .question-list ul.questions').append('<li class="question bank quiz" id="pool1">' +
                    '<div class="description bank">' +
                    '<div class="title"><span class="text truncated">my pool1</span>' +
                    '</div></li>');
                $('.question-list').first().questionListGearbox('updateAddMenu');
                $('.available-questions .add-menu .gearbox').click();
                expect($('.available-questions .add-menu li').length).toEqual(3);
                expect($('.available-questions .add-menu li.add-to-pool-pool1').length).toEqual(1); //check if the pool was added to the menu
                
            });
        });
        describe('Test the "select" gear box ', function () {
            beforeEach(function () {
                TestQuizHelper.setAllCheckTarget = '.available-questions';
                $('.question-list').first().questionListGearbox('initSelectionGearboxForAvailableQuestions');
                $('.available-questions .select-menu .gearbox').click();
            });

            it('Click on "select" button, Expected initialize "select" gear box', function () {
                
                expect($('.available-questions .select-menu li').length).toEqual(5);

            });

            it('Click on "select-all" option, Expected all question selected', function () {
                var totalNumberOfQuestions = $('.available-questions-body .questions .question').length;
                $('.available-questions .select-menu li.select-all').click();
                var selectedQuestions = $('.available-questions-body li.question [name=selected]:checked').length;
                expect(totalNumberOfQuestions).toEqual(selectedQuestions);

            });

            it('Click on "select-none" option, Expected all question de-selected', function () {
                var totalNumberOfQuestions = $('.available-questions-body .questions .question').length;
                $('.available-questions .select-menu li.select-all').click();
                var selectedQuestions = $('.available-questions-body li.question [name=selected]:checked').length;

                expect(totalNumberOfQuestions).toEqual(selectedQuestions);
                var methodCalled = false;

                $.fn.questionlist = jasmine.createSpy('questionlist').andCallFake(function (method) {
                    methodCalled = methodCalled || method == 'clearAllQuestions';
                });
                $('.available-questions .select-menu li.select-none').click();

                expect(methodCalled).toBeTruthy();

            });

            it('Click on "select-never-used" option, Expected all question selected', function () {
                var totalNumberOfQuestions = $('.available-questions-body .questions .question').length;
                $('.available-questions .select-menu li.select-never-used').click();
                var selectedQuestions = $('.available-questions-body li.question [name=selected]:checked').length;

                expect(totalNumberOfQuestions).toEqual(selectedQuestions);

            });

            it('Click on "select-used-previously " option, Expected no question selected', function () {
                $('.available-questions .select-menu li.select-used-previously').click();
                var selectedQuestions = $('.available-questions-body li.question [name=selected]:checked').length;

                expect(0).toEqual(selectedQuestions);

            });

            it('Click on "select-random" option, Expected dialog pops up', function () {
                $.fn.block = jasmine.createSpy('block');
                $('.available-questions .select-menu li.select-random ').click();

                expect($.fn.block).toHaveBeenCalled();
            });

            it('Show hide gear function should show gearbox when isVisible is true ', function () {
                $('.available-questions .select-menu').hide();
                $('.question-list').questionListGearbox('showHideGearbox', true);

                expect($('.available-questions .select-menu').is(":visible")).toBeTruthy();
            });
            it('Show hide gear function should hide gearbox when isVisible is false ', function () {
                $('.available-questions .select-menu').show();
                $('.question-list').questionListGearbox('showHideGearbox', false);

                expect($('.available-questions .select-menu').is(":visible")).toBeFalsy();
            });
            it('Show hide gear function should do nothing when isVisible is equal to visiblity ', function () {
                $('.available-questions .select-menu').show();
                $('.question-list').questionListGearbox('showHideGearbox', true);
                expect($('.available-questions .select-menu').is(":visible")).toBeTruthy();
                $('.available-questions .select-menu').hide();
                $('.question-list').questionListGearbox('showHideGearbox', false);
                expect($('.available-questions .select-menu').is(":visible")).toBeFalsy();
            });
           
        });
        
    });

    //#endregion test the gearbox with class "move-current-question" used in selected-question-list

    //#region test the gearbox dispalyed on top of selected-question-list
    describe('Test the gearbox dispalyed on top of selected-question-list ', function () {
        beforeEach(function () {
            $('.question-list').first().questionListGearbox('updateNewMenu');
            $('.selected-questions .new-question-menu .gearbox').click();
        })
        describe('Test the "Create" gear box ', function () {
            it('Click on "Create", Expected a drop down list with 9 options appears', function () {
                expect($('.selected-questions .new-question-menu li').length).toEqual(9);

            });
            
            it('Click on "Importer" option, Expected event "showimporterdialog.questionimport" trigger', function () {
                $('.question-list').first().questionListGearbox('updateNewMenu');
                var isEventTrigger = false;
                PxPage.switchboard = 'body';
                $(PxPage.switchboard).bind('showimporterdialog.questionimport', function() {
                    isEventTrigger = true;
                });
                $('.selected-questions .new-question-menu li.importer').click();
                waitsFor(function () {
                    return isEventTrigger;
                });
                runs(function() {
                    expect(isEventTrigger).toBeTruthy();
                });
            });
            
            it('Click on "Question pool" option, Expected "create-question-pool" dialog pops up', function () {
                $('.question-list').append($('<div class="question-editor"></div>'));
                $('.question-list').first().questionListGearbox('updateNewMenu');
                $('.selected-questions .new-question-menu li.question-bank').click();
                expect($('.question-pool-dialog-text').hasClass('ui-dialog-content')).toBeTruthy();
                destroyCreateQuestionPoolDialog();
                
            });
            
            it('Click on "Question pool" option when quiz editor is opened, Expected "editQuestionPool" is called in questioneditor', function () {
                $('.question-list').append($('<div class="question-editor"></div>'));
                $('.quiz-editor .available-questions').hide();
                $('.question-list').first().questionListGearbox('updateNewMenu');
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'editQuestionPool';
                });
                $('.selected-questions .new-question-menu li.question-bank').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                });
            });
            
            it('Click on "Multiple Choice" option when quiz editor is opened, Expected "openEditorForNewQuestions" is called in questioneditor', function () {
                setupForTestingCreateQuestion();
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'openEditorForNewQuestions';
                });
                $('.selected-questions .new-question-menu li.multiple-choice').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                    expect($('#previous-question-type').val()).toEqual('choice');
                });
            });
            
            it('Click on "short answer" option when quiz editor is opened, Expected "openEditorForNewQuestions" is called in questioneditor', function () {
                setupForTestingCreateQuestion();
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'openEditorForNewQuestions';
                });
                $('.selected-questions .new-question-menu li.short-answer').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                    expect($('#previous-question-type').val()).toEqual('text');
                });
            });
            
            it('Click on "essay " option when quiz editor is opened, Expected "openEditorForNewQuestions" is called in questioneditor', function () {
                setupForTestingCreateQuestion();
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'openEditorForNewQuestions';
                });
                $('.selected-questions .new-question-menu li.essay').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                    expect($('#previous-question-type').val()).toEqual('essay');
                });
            });
            
            it('Click on "matching " option when quiz editor is opened, Expected "openEditorForNewQuestions" is called in questioneditor', function () {
                setupForTestingCreateQuestion();
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'openEditorForNewQuestions';
                });
                $('.selected-questions .new-question-menu li.matching').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                    expect($('#previous-question-type').val()).toEqual('match');
                });
            });
            
            it('Click on "multiple-answer " option when quiz editor is opened, Expected "openEditorForNewQuestions" is called in questioneditor', function () {
                setupForTestingCreateQuestion();
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'openEditorForNewQuestions';
                });
                $('.selected-questions .new-question-menu li.multiple-answer').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                    expect($('#previous-question-type').val()).toEqual('answer');
                });
            });
            
            it('Click on "Advanced Question" option when quiz editor is opened, Expected "openEditorForNewQuestions" is called in questioneditor', function () {
                setupForTestingCreateQuestion();
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'openEditorForNewQuestions';
                });
                $('.selected-questions .new-question-menu li.hts').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                    expect($('#previous-question-type').val()).toEqual('hts');
                });
            });
            
            it('Click on "Graphing Exercise" option when quiz editor is opened, Expected "openEditorForNewQuestions" is called in questioneditor', function () {
                setupForTestingCreateQuestion();
                var isFunctionCalled = false;
                $.fn.questioneditor = jasmine.createSpy('questioneditor').andCallFake(function (method) {
                    isFunctionCalled = isFunctionCalled || method == 'openEditorForNewQuestions';
                });
                $('.selected-questions .new-question-menu li.graph').click();

                waitsFor(function () {
                    return isFunctionCalled;
                });
                runs(function () {
                    expect(isFunctionCalled).toBeTruthy();
                    expect($('#previous-question-type').val()).toEqual('graph');
                });
            });
        });

    });

    //#endregion test the gearbox with class "move-current-question" used in selected-question-list
    //#region implementation

    function destroyCreateQuestionPoolDialog() {
        $('.question-pool-dialog-text').dialog('destroy');
        $('.ui-dialog').remove();
    }
    function initQuestionGearbox() {
        $('.question-list').first().questionListGearbox('initQuestionGearbox');
        $('.add-menu .gearbox').click();
        $('.add-menu .add-to-new-pool').click();
    }

    function setupForTestingCreateQuestion() {
        $('.question-list').append($('<div class="question-editor"></div>'));
        $('.question-list').append($('<input type="hidden" id="previous-question-type" />'));
        $('.question-list').first().questionListGearbox('updateNewMenu');
    }

//#endregion implementation
});