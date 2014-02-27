describe("Quiz Functionality Tests: ", function () {

    beforeEach(function () {
        setFixtures('<div id="fne-window"><div id="fne-content"><div id="content">' +
            '<div class="show-quiz"><div class="gradebook-component"><div class="info"></div></div></div>' +
            '<div class="quiz-editor"><div class="selected-questions"><div class="question-pool"><div class="questions"></div></div><div>' +
            '<div class="available-questions"><div class="toc"></div></div></div></div></div></div>' +
            '<div class="quizDirectionModal"></div>');
    });

    describe('Test Quiz - student ui', function () {

        it('Click on quizDirectionsModal, Expect specific classes and ids on the dialog html (required by css)', function () {

            var divPassedInDialog;
            $.fn.dialog = jasmine.createSpy('dialog').andCallFake(function (div) {
                divPassedInDialog = this;
            });
            PxQuiz.ShowQuizDirections();

            //
            expect(divPassedInDialog.hasClass('quizDirectionModal')).toBeTruthy();

            var innerDiv1 = divPassedInDialog[0].childNodes[0];
            expect($(innerDiv1).is("#quizDirectionsModal")).toBeTruthy();

            var innerDiv2 = innerDiv1.childNodes[0];
            expect($(innerDiv2).is("#content")).toBeTruthy();
            expect($(innerDiv2).hasClass("html-container")).toBeTruthy();
            expect($(innerDiv2).hasClass("description-content")).toBeTruthy();
        });
    });

});