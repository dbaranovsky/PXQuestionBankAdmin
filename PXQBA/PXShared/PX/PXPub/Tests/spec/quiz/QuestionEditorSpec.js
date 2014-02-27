describe("Quiz Question Editor", function () {
    var gQuestionEditor;
    var pxQuizEditor;
    var mcQuestionId = "11319fbfc7d54c7bbf6d3c0ea10fc2f6";
    var poolQuestionId = "D4B8237C658CC38C56BC9F47C9731A02";
    
    beforeEach(function() {
        PxPage = jasmine.createSpyObj("PxPage", ["Loading", "FneResizeHooks", "FneInitHooks", "Loaded", "log", "Fade"]);
        PxQuiz = jasmine.createSpyObj("PxQuiz", ["HasQuestionBankChanged", "HTS_RPC", "UpdateQuestionList", "UpdateAddedQuestions",
            "UpdatePreviousQuestionOnServer", "RemoveQuestionFromCacheOnServer"]);
        window.PxModal = jasmine.createSpyObj("PxModal", ["CreateConfirmDialog"]);
        PxQuiz.EnumQuestionMode = {
            browser: "browser",
            questioneditor: "editor",
            pooleditor: "pooleditor",
            none: "none"
        };
        PxQuiz.HTS_RPC = jasmine.createSpyObj("HTS_RPC", ["isDirty", "saveQuestion"]);

        PxPage.FrameAPI = jasmine.createSpyObj("FrameAPI", ["saveComponent"]);
        PxPage.Routes = jasmine.createSpyObj("Routes", ["quiz_question_settings"]);

        spyOn($, "post").andCallFake(function(method, promises, callback) { callback(); });

        pxQuizEditor = $.fn.questionEditorObj()._static.fn;

        gQuestionEditor = "<div class='edit-question'>" +
                            "<div class='question-editor'>" +
                            "<input id='test' class='id' value='" + mcQuestionId + "' type='text'/>" +
                            "<input type='text' class='is-user-created' value='0'/>" +
                            "<input type='text' class='is-publisher-edited' value='0'/>" +
                            "</div>" +
                            "</div>";

        /* fixtures set up */
        var dummyQuestions = [];
        var dummy1 = { Id: mcQuestionId, Type: 'MC', Text: 'Question MC ' + mcQuestionId };
        var dummy2 = { Id: poolQuestionId, Type: 'BANK', Text: 'Question BANK ' + poolQuestionId };
        dummyQuestions.push(dummy1);
        dummyQuestions.push(dummy2);
        
        var model = TestQuizHelper.Model.generateQuizModel(dummyQuestions);
        var viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromView(viewModel, 'set');
        viewModel = TestQuizHelper.generateViewModel(model);
        TestQuizHelper.setFixtureFromView(viewModel, 'target', '.quiz-editor-questions');
        
        $(".quiz-editor .edit-question-container").show();
    });
    
    xdescribe("Opening:", function () {
        it("can open a question editor", function () {

        });

        it("can open a question pool editor", function() {

        });

        it("can open a custom question editor", function() {

        });
    });

    xdescribe("Editing a question:", function () {
        var myQuestion, myEvent;
        beforeAll(function() {
            myQuestion = $("li.question#" + mcQuestionId);
            myEvent = $.Event("click", { "target": myQuestion });
        });

        it("can pop up the dialog with save/don't save options", function() {

        });

        it("can edit a question and save a previous question", function () {
            spyOn(pxQuizEditor, "EditQuestion").andCallThrough();
            spyOn(pxQuizEditor, "SaveQuestion");
            PxQuiz.HasQuestionBankChanged.andReturn(false);

            pxQuizEditor.EditQuestion(myEvent);

            expect(pxQuizEditor.EditQuestion).toHaveBeenCalled();
            expect(pxQuizEditor.SaveQuestion).toHaveBeenCalled();
        });

        it("can edit a question and save a previous question pool", function () {
            spyOn(pxQuizEditor, "EditQuestion").andCallThrough();
            PxQuiz.HasQuestionBankChanged.andReturn(true);
            spyOn(pxQuizEditor, "SaveEditedQuestionPool").andCallFake(function (callback) { callback(); });
            spyOn(pxQuizEditor, "UpdateQuizEditorQuestionPool");
            spyOn(pxQuizEditor, "SelectEditingQuestion");
            spyOn(pxQuizEditor, "SaveAndLoadQuestion");

            pxQuizEditor.EditQuestion(myEvent);

            expect(pxQuizEditor.EditQuestion).toHaveBeenCalled();
            expect(pxQuizEditor.SaveAndLoadQuestion).toHaveBeenCalled();
            expect(pxQuizEditor.PostQuestionPool).toHaveBeenCalled();
            expect(pxQuizEditor.LoadQuestionPool).toHaveBeenCalled();
            expect(pxQuizEditor.SelectEditingQuestion).toHaveBeenCalled();
            expect(pxQuizEditor.SaveAndLoadQuestion).toHaveBeenCalled();
        });

        it("can edit a question and save a previous custom question", function() {

        });
    });

    describe("Editing a question pool:", function () {
        var myEvent;
        beforeEach(function () {
            spyOn(pxQuizEditor, "ValidateQuestionPool").andReturn(true);
            
            var question = $("li.question#" + poolQuestionId);
            myEvent = $.Event("click", { "target": question });
        });

        xit("can pop up the dialog with save/don't save options", function () {

        });
        
        xit("can edit a question pool and save a previous question", function () {
            PxQuiz.HasQuestionBankChanged.andReturn(false);
            pxQuizEditor.EditQuestionPool(myEvent);

            var question = $(".quiz-editor .selected-questions .question-list ul.questions, .quiz-overview .question-list ul.questions").find("#" + poolQuestionId);
            var clsQuestion = question.attr("class");
            
            expect(clsQuestion).toContain("active selected");
        });

        it("can save a question pool (including pool name, pool count, pool points)", function () {
            PxQuiz.HasQuestionBankChanged.andReturn(true);
            PxQuiz.QuestionMode = PxQuiz.EnumQuestionMode.browser; //assume user clicks link from browser
            
            var title = "Thanda";
            var poolSize = "13";
            var poolPoints = "5";
            var poolContainers = $('.quiz-editor .question-pool-container');
            poolContainers.find('#question-pool-id').val(poolQuestionId);
            poolContainers.find('#txtPoolName').val(title);
            poolContainers.find('#txtPoolCount').val(poolSize);
            poolContainers.find('#txtPoolPoints').val(poolPoints);
            
            var questionSelector = "ul.questions li#" + poolQuestionId;
            $(questionSelector).find(".description .title .text").eq(0).text("");
            var bankUseExists = $(questionSelector).find(".bank-use").length > 0;
            if (!bankUseExists) {
                $(questionSelector).append("<span class='bank-use'></span>");
                $(questionSelector).append("<span class='bank-points'></span>");
            }
            $(questionSelector).find(".bank-use").eq(0).text("");
            
            pxQuizEditor.SaveQuestionPool();
            
            expect($(questionSelector).find(".description .title .text").eq(0).text()).toBe(title);
            expect($(questionSelector).find(".bank-use").eq(0).text()).toBe(poolSize);
            expect($(questionSelector).find(".bank-points").eq(0).text()).toBe(poolPoints);
            expect($(questionSelector).find(".questions-points").eq(0).val()).toBe(poolPoints);

            $(".quiz-editor").remove(".question-pool-container");
        });

        xit("can edit a question pool and save a custom question", function() {

        });
    });

    xdescribe("Editing a custom question:", function () {
        it("can pop up the dialog with save/don't save options", function () {

        });
        
        it("can edit a custom question and save a previous question", function () {
            spyOn(pxQuizEditor, "EditQuestion").andCallThrough();
            spyOn(pxQuizEditor, "SaveCustomQuestion");
            spyOn(pxQuizEditor, "IsCustomQuestionEditor").andCallFake(function () { return true; });
            PxQuiz.HasQuestionBankChanged.andReturn(false);

            pxQuizEditor.EditQuestion(myEvent);

            expect(pxQuizEditor.EditQuestion).toHaveBeenCalled();
            expect(pxQuizEditor.SaveCustomQuestion).toHaveBeenCalled();
        });

        it("can edit a custom question and save a previous question pool", function() {
            PxQuiz.HasQuestionBankChanged.andReturn(true);
            spyOn(pxQuizEditor, "IsCustomQuestionEditor").andReturn(true);
            spyOn(pxQuizEditor, "SaveCustomQuestion");

            pxQuizEditor.EditQuestion(myEvent);
            
            expect(pxQuizEditor.SaveCustomQuestion).toHaveBeenCalledWith(myQuestion, myEvent);
        });

        it("can edit a custom question and save a previous custom question", function() {

        });
    });

    xdescribe("Saving a question:", function () {
        var myEvent;
        var myQuestion;
        beforeEach(function () {
            myQuestion = $("li.question#" + mcQuestionId);
            myEvent = $.Event("click", { "target": myQuestion });
            
            PxQuiz.UpdatePreviousQuestionOnServer.andCallFake(function (mcQuestionId, customQuestionXml, callback) { callback(); });
            PxQuiz.RemoveQuestionFromCacheOnServer.andCallFake(function (mcQuestionId, callback) { callback(); });
        });
        
        it("can validate a question", function () {
                spyOn(pxQuizEditor, "ValidateQuestionSettingsInEditor")
                            .andCallFake(function () {
                                return true;
                            });

                var returnResult = pxQuizEditor.ValidateQuestionSettingsInEditor();

                expect(returnResult).toEqual(true);
            });

        it("can save a BH question and call FRAME API", function () {
            PxPage.switchboard = $(".quiz-editor");

            $(".quiz-editor").append(gQuestionEditor);

            spyOn(pxQuizEditor, "SaveQuestion").andCallThrough();

            var frameApiComponentSave = false;
            PxPage.FrameAPI.saveComponent.andCallFake(function (componentName, methodName, callback) {
                if (componentName == "questioneditor" && methodName == "quizeditorcomponent" && callback) {
                    frameApiComponentSave = true;
                    $(PxPage.switchboard).trigger("componentsaved");
                    callback();
                }
            });

            var questionSavedTriggered = false;
            $(PxPage.switchboard).one("questionsaved", function () {
                questionSavedTriggered = true;
            });

            pxQuizEditor.SaveQuestion();

            expect(pxQuizEditor.SaveQuestion).toHaveBeenCalled();
            expect(frameApiComponentSave).toEqual(true);
            expect(questionSavedTriggered).toEqual(true);
        });

        it("can save points for a question", function () {
            if ($(".question-editor").length == 0) {
                $(".quiz-editor").append("<div class='question-editor'><input type='text' class='id' value='" + mcQuestionId + "'</div>");

                if ($(".hts-editor-links .total-points .questions-points-original").length == 0) {
                    $(".quiz-editor").append("<div class='hts-editor-links'><div class='total-points'>" +
                                    "<input type='text' class='questions-points-original'/></div></div>");
                }
            }
                
            var questionId = $(".question-editor .id").val();
            var pointsValue = "5";
            $(".hts-editor-links .total-points .questions-points-original").val(pointsValue);

            spyOn(pxQuizEditor, "SaveNewQuestionPoints")
                        .andCallFake(function (callback) { callback(); })
                        .andCallThrough();

            var pointsCallback = jasmine.createSpy();
            pxQuizEditor.SaveNewQuestionPoints(pointsCallback);

            expect(pxQuizEditor.SaveNewQuestionPoints).toHaveBeenCalled();

            $.each(questionId.split(","), function (i, v) {
                var originPoints = $(".selected-questions").find("#" + v).find(".questions-points-original, .questions-points").val();
                expect(originPoints).toEqual(pointsValue);
            });
        });
    });

    xdescribe("Saving a question pool:", function () {
        it("can save a question pool", function () {

        });

        it("can save points for a question pool", function() {

        });
    });
    
    xdescribe("Saving a custom question:", function () {
        it("can save an HTS question", function () {
            spyOn(pxQuizEditor, "EditQuestionResponseFromUser").andCallFake(function (event, userSaveFunction, donotSaveFunction, cancelFunction) {
                userSaveFunction();
            });

            spyOn(pxQuizEditor, "SaveCustomQuestion").andCallThrough();
            spyOn(pxQuizEditor, "IsLastEditableQuestion").andReturn(false);

            PxQuiz.HTS_RPC.isDirty.andCallFake(function (callback) { callback(true); });
            PxQuiz.HTS_RPC.saveQuestion.andCallFake(function (callback) { callback(); });

            $(".quiz-editor").append(gQuestionEditor);

            // set questionType to "HTS"
            var questionType = $("#custom-hts-editor").length > 0 ? "HTS" : "MC";
            if (questionType == "MC") {
                $(document.body).append("<div id='custom-hts-editor'><p>&nbsp;</p></div>");
            }

            pxQuizEditor.SaveCustomQuestion(myQuestion, myEvent);

            expect(pxQuizEditor.SaveCustomQuestion).toHaveBeenCalled();
        });

        it("can save a last editable question", function () {

        });

        it("can save a Graph question", function () {

        });

        it("can save points", function () {

        });

        it("can save a custom question with related contents", function () {

        });
    });

    xdescribe("Saving related content:", function() {

    });

    xdescribe("Saving a blank question", function() {
        it("cannot save a blank question", function() {

        });
    });

    xdescribe("Reverting:", function () {
        it("can undo a saved question", function () {
            // ReLoadQuestionEditor()
        });

        it("can undo a saved question pool", function() {

        });
        
        it("can undo a saved custom question", function () {

        });
    });

    xdescribe("Creating:", function() {
        it("can create a new question", function() {

        });

        it("can create a new question pool", function () {
            var poolContainer = ".question-pool-inner-container #txtPoolName";

            setFixtures("<div class='question-pool-inner-container'><div id='txtPoolName'></div></div>");

            pxQuizEditor.EditQuestionPool(null);
            
            expect($(poolContainer).val()).toBe("New question pool");
        });

        it("can create a new custom question", function() {

        });
    });

    xdescribe("Previewing:", function() {
        it("can preview the question", function() {

        });

        it("can preview and edit the question", function() {

        });
        
        it("can preview the custom question", function() {

        });

        it("can preview and edit the custom question", function() {

        });

        it("can cancel the preview", function() {

        });
    });

    xdescribe("Moving:", function() {
        it("can move a question into a question pool", function() {

        });

        it("can move a question to top", function() {

        });

        it("can move a question to bottom", function () {

        });
        
        it("can move a question pool to top", function() {

        });
        
        it("can move a question pool to bottom", function () {

        });

        it("can move a custom question to top", function() {

        });

        it("can move a custom question to bottom", function () {

        });
    });

    xdescribe("Navigating away:", function () {
        it("can confirm and save current question on closing FNE window", function () {

        });

        it("can confirm and save current question pool on closing FNE window", function () {

        });
        
        it("can confirm and save current custom question on closing FNE window", function () {

        });
        
        it("can navigate away without saving changes", function () {

        });

        it("can confirm and save current question on clicking 'Browse question banks'", function() {

        });

        it("can confirm and save current question pool on clicking 'Browse question banks'", function () {

        });

        it("can confirm and save current custom question on clicking 'Browse question banks'", function () {

        });
        
        it("can navigate away without saving changes on clicking 'Browse question banks'", function() {

        });

        it("can cancel the process", function () {

        });

        it("can confirm and save current question on clicking 'Assignment' tab", function () {

        });

        it("can confirm and save current question pool on clicking 'Assignment' tab", function () {

        });

        it("can confirm and save current custom question on clicking 'Assignment' tab", function () {

        });
        
        it("can confirm and save current question on clicking 'Setting' tab", function () {

        });

        it("can confirm and save current question pool on clicking 'Setting' tab", function () {

        });

        it("can confirm and save current custom question on clicking 'Setting' tab", function () {

        });

        it("can confirm and save current question on clicking 'Basic info' tab", function () {

        });

        it("can confirm and save current question pool on clicking 'Basic info' tab", function () {

        });

        it("can confirm and save current custom question on clicking 'Basic info' tab", function () {

        });
    });
});