describe('Load more resources functionality in LearningCurve Tests: ', function () {

    describe('When editQuestionSettings is called', function () {
        beforeEach(function () {
            window.PxModal = jasmine.createSpyObj('PxModal', ['CreateConfirmDialog']);
        });

        it('expect PxModal.CreateConfirmDialog() have been called', function () {
            $.fn.LearningCurveMoreResources('editQuestionSettings');
            expect(PxModal.CreateConfirmDialog).toHaveBeenCalled();
        });
        
        it('expect dialog title is "Question Settings Confirmation"', function () {
            var title;
            PxModal.CreateConfirmDialog.andCallFake(function(data) {
                title = data.title;
            });
            $.fn.LearningCurveMoreResources('editQuestionSettings');
            expect(title).toEqual('Question Settings Confirmation');
        });
        
        it('expect dialog content is asking user for confirmation', function () {
            var content;
            var expectedContent = 'This action will remove all unsaved data in your question. <br /> <br /> Are you sure you want to proceed?';
            PxModal.CreateConfirmDialog.andCallFake(function (data) {
                content = data.content;
            });
            $.fn.LearningCurveMoreResources('editQuestionSettings');
            expect(content).toEqual(expectedContent);
        });
        
        it('expect dialog has "Yes" and "No" buttons', function () {
            var buttons;
            PxModal.CreateConfirmDialog.andCallFake(function (data) {
                buttons = data.buttons;
            });
            $.fn.LearningCurveMoreResources('editQuestionSettings');
            expect(buttons.Yes).toBeDefined();
            expect(buttons.No).toBeDefined();
        });
        
        it('expect dialog "Yes" button binds to a function', function () {
            var func;
            PxModal.CreateConfirmDialog.andCallFake(function (data) {
                func = data.buttons.Yes.click;
            });
            $.fn.LearningCurveMoreResources('editQuestionSettings');
            expect(func).toBeDefined();
        });
        
        it('expect dialog "No" button not bind to a function', function () {
            var func;
            PxModal.CreateConfirmDialog.andCallFake(function (data) {
                func = data.buttons.No.click;
            });
            $.fn.LearningCurveMoreResources('editQuestionSettings');
            expect(func).not.toBeDefined();
        });
    });
});