describe('PxCommon Tests: ', function () {
    beforeAll(function () {
    });
       
    it('expect PxModal.CreateConfirmDialog() have been called', function () {
        //spy on functions that we are not testing against
        spyOn(PxPage, "OnAssignmentClicked");
        PxPage.AssignmentDatesCallback.CustomValues = jasmine.createSpyObj('CustomValues', ['StartDate', 'EndDate']);
        Date.prototype.format = jasmine.createSpy('format');
        PxPage.AssignmentDateSelected("NaN/NaN/NaN", new Date("NaN/NaN/NaN"));
        
        //we expect PxPage.OnAssignmentClicked when user did not click on a date
        //user can do other things ie: changing current month.
        expect(PxPage.OnAssignmentClicked).not.toHaveBeenCalled();
    });
    describe('Fne Link, ', function() {
        it('when link has class "require-confirm-custom", expect fne window has class "require-confirm-custom" ', function () {
            var fixture = '<div id="fne-window"></div><a class="fne-link require-confirm-custom"></a>';
            setFixtures(fixture);
            PxPage.SetFneLinks();
            $('.fne-link').click();
            expect($('#fne-window').hasClass('require-confirm-custom')).toBeTruthy();
        });
        
        it('when link does NOT have class "require-confirm-custom", expect fne window NOT have class "require-confirm-custom" ', function () {
            var fixture = '<div id="fne-window"></div><a class="fne-link"></a>';
            setFixtures(fixture);
            PxPage.SetFneLinks();
            $('.fne-link').click();
            expect($('#fne-window').hasClass('require-confirm-custom')).toBeFalsy();
        });
    });
    
    describe('Validate title, ', function () {
        beforeAll(function() {
            PxPage.Toasts = jasmine.createSpyObj('PxPage.Toasts', ['Error', 'Success', 'Info']);
        });
        it('when title contains proper html tags, expect success ', function () {
            var fixture = '<input type="text" class="title" />';
            setFixtures(fixture);
            $('input.title').val('<div style="display:none;"><strong>Title</strong></div>');
            var result = PxPage.ValidateTitle();
            expect(result).toBeTruthy();
        });

        it('when title contains improper html tags, expect error ', function () {
            var fixture = '<input type="text" class="title" />';
            setFixtures(fixture);
            $('input.title').val('<div style="display:none;"><strong>Title</strong></div><iframe></iframe>');
            var result = PxPage.ValidateTitle();
            expect(result).toBeFalsy();
        });
        
        it('when title html tag contains improper attribute, expect return false', function () {
            var fixture = '<input type="text" class="title" />';
            setFixtures(fixture);
            $('input.title').val('<div style="display:none;" onClick="doThis()"><strong>Title</strong></div>');
            var result = PxPage.ValidateTitle();
            expect(result).toBeFalsy();
        });
    });
});