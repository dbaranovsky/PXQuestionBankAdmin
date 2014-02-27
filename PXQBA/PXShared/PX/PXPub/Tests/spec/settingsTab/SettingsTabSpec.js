describe("Settings Tab Tests:", function () {
    beforeEach(function () {

        var model = {
            viewModel: JSON.stringify({ "AssessmentId": "ANGEL_psychportal__myers10e__master_08C96CC73421A76B1EEB09E0F2230004", "EntityId": "158811", "EntityIdIsCourseId": true, "AssessmentType": 0, "NumberOfAttempts": { "Attempts": 0 }, "SubmissionGradeAction": -1, "TimeLimit": 0, "QuestionDelivery": 0, "AllowSaveAndContinue": true, "AutoSubmitAssessments": false, "RandomizeQuestionOrder": false, "RandomizeAnswerOrder": true, "AllowViewHints": false, "HintSubstractPercentage": 0, "ShowScoreAfter": 0, "ShowQuestionsAnswers": 0, "ShowRightWrong": 0, "ShowAnswers": 1, "ShowFeedbackAndRemarks": 0, "ShowSolutions": 1, "StudentsCanEmailInstructors": true, "GradeRule": 2, "LearningCurveTargetScore": null, "AutoTargetScore": false, "AutoCalibrateDifficulty": false }),
            viewPath: 'Index',
            viewModelType: 'Bfw.PX.PXPub.Models.AssessmentSettings',
            viewData: JSON.stringify({
                IsTemplate: {
                    dataType: "System.Boolean",
                    dataValue: false
                }
            })
        };
        BaseTestHelper.api.setFixtureFromCache(model, 'AssessmentSettings', 'set');
        PxSettingsTab.BindReviewSettings();

    });
    describe('Review settings', function () {
        it('If disable to show question and answer, then the following options should be disabled' +
        '\n\n1) Show whether answers were right/wrong after...' +
        '\n2) Show question score after...' +
        '\n3) Show correct answers after...'+
        '\n4) Show solutions after...', function () {
            $('#review-setting-show-questions-answers .setting-header-never').prop('checked', true);
            var i = 0;
            waitsFor(function () {
                $('#review-setting-show-questions-answers .setting-header-never').click();
                return $('#review-setting-show-answers .setting-header-never').prop('checked') || i++ > 100;
            });
            runs(function () {
                expect($('#review-setting-show-answers .setting-header-never').prop('checked')).toBeTruthy();
                expect($('#review-setting-show-right-wrong .setting-header-never').prop('checked')).toBeTruthy();
                expect($('#review-setting-Show-score-after .setting-header-never').prop('checked')).toBeTruthy();
                expect($('#review-setting-show-solutions .setting-header-never').prop('checked')).toBeTruthy();

            });
        });

        it('If disable to show answers were right/wrong, then the following options should be disabled' +
        '\n1) Show question score after...' +
        '\n2) Show correct answers after...' +
        '\n3) Show solutions after...', function () {
            $('#review-setting-show-right-wrong .setting-header-never').prop('checked', true);
            var i = 0;

            waitsFor(function () {
                $('#review-setting-show-right-wrong .setting-header-never').click();
                return $('#review-setting-show-answers .setting-header-never').prop('checked') || i++ > 100;
            });
            runs(function () {
                expect($('#review-setting-show-answers .setting-header-never').prop('checked')).toBeTruthy();
                expect($('#review-setting-Show-score-after .setting-header-never').prop('checked')).toBeTruthy();
                expect($('#review-setting-show-solutions .setting-header-never').prop('checked')).toBeTruthy();
            });
        });
        
        it('If disable to show correct answer, then the following options should be disabled' +
        '\n1) Show question score after...', function () {
            $('#review-setting-show-answers .setting-header-never').prop('checked', true);
            var i = 0;

            waitsFor(function () {
                $('#review-setting-show-answers .setting-header-never').click();
                return $('#review-setting-Show-score-after .setting-header-never').prop('checked') || i++ > 100;
            });
            runs(function() {
                expect($('#review-setting-Show-score-after .setting-header-never').prop('checked')).toBeTruthy();
            });
        });
    });
});