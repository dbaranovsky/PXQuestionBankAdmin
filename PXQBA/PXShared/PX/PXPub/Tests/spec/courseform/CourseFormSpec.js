/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" data-cover-adapter="../../lib/blanket/jasmine-blanket.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Scripts/jquery/jquery.placeholder.js" />
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

/// <reference path="../../../Scripts/Datepicker/datepicker.js" />
/// <reference path="../../../Scripts/other/date.js" />
/// <reference path="../../../Scripts/Common/dateFormat.js" />

/// <reference path="../../../Scripts/CourseForm/CourseForm.js" />

describe('The General Settings', function () {
    var fixture = '';

    beforeEach(function () {
        if (fixture == '') {
            fixture = helper.GetGeneralSettingsView();
        }

        jasmine.Fixtures.prototype.addToContainer_(fixture);

        var fixtureMain = "<div id='main'></div>";
        jasmine.Fixtures.prototype.addToContainer_(fixtureMain);

        PxInstructorConsoleWidget = jasmine.createSpy("PxInstructorConsoleWidget");
        PxInstructorConsoleWidget.BindEditButton = jasmine.createSpy("PxInstructorConsoleWidget.BindEditButton");

        PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
        PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
        PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
    });

    afterEach(function () {
        jasmine.Fixtures.prototype.cleanUp();
    });

    it("should show validation error if url is invalid", function () {
        CourseForm.Init(1);
        var data = {
            Error: {
                id: "url",
                Message: "invalid url"
            }
        };
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success(data);
        });

        $('#submitForm').click();
        helper.ResetAjaxSpy(this);

        expect($('#urlError').html()).toBe("invalid url");
    });

    it("should show toasts error message if a textbox has html tags", function () {
        CourseForm.Init(1);
        $('input#Title').val('<i>');

        $('#submitForm').click();

        expect(PxPage.Toasts.Error).wasCalled();
    });

    it("should not show toasts success message if LMS ID disabled", function () {
        $('#LmsIdRequiredFalse').attr('checked', 'checked');
        CourseForm.Init(1);
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success({});
        });

        $('#submitForm').click();
        helper.ResetAjaxSpy(this);

        expect(PxPage.Toasts.Success).wasNotCalled();
    });

    it("should show toasts success message if LMS ID changed from disabled to enabled", function () {
        $('#LmsIdRequiredFalse').prop('checked', true);
        CourseForm.Init(1);
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success({});
        });

        $('#LmsIdRequiredTrue').prop('checked', true);
        $('#submitForm').click();
        helper.ResetAjaxSpy(this);

        expect(PxPage.Toasts.Success).wasCalled();
    });

    it("should not show toasts success message if LMS ID changed from enabled to disabled", function () {
        $('#LmsIdRequiredTrue').prop('checked', true);
        CourseForm.Init(1);
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success({});
        });

        $('#LmsIdRequiredFalse').prop('checked', true);
        $('#submitForm').click();
        helper.ResetAjaxSpy(this);

        expect(PxPage.Toasts.Success).wasNotCalled();
    });

    it("should show not toasts success message if LMS ID changed from enabled to enabled (wasn't modified)", function () {
        $('#LmsIdRequiredTrue').prop('checked', true);
        CourseForm.Init(1);
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success({});
        });

        $('#submitForm').click();
        helper.ResetAjaxSpy(this);

        expect(PxPage.Toasts.Success).wasNotCalled();
    });

    it("should show not toasts success message if LMS ID changed from disabled to disabled (wasn't modified)", function () {
        $('#LmsIdRequiredFalse').prop('checked', true);
        CourseForm.Init(1);
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success({});
        });

        $('#submitForm').click();
        helper.ResetAjaxSpy(this);

        expect(PxPage.Toasts.Success).wasNotCalled();
    });

    var helper = {
        GetGeneralSettingsView: function () {
            var possibleAcademicTerms = [];
            possibleAcademicTerms.push({ Name: "Summer", Id: "1" });
            possibleAcademicTerms.push({ Name: "Winter", Id: "2" });

            var contactInfo = [];
            contactInfo.push({ ContactType: "email", ContactValue: "my@email.com" });

            var data = {
                viewPath: "CourseForm",
                viewModelType: "Bfw.PX.PXPub.Models.Course",
                viewModel: JSON.stringify({
                    SyllabusType: "Url",
                    AcademicTerm: "Spring 2013",
                    PossibleAcademicTerms: possibleAcademicTerms,
                    ContactInformation: contactInfo
                })
            };

            var fixture = PxViewRender.RenderView('PXPub', 'Shared', data);
            fixture += "<input type='submit' id='submitForm' />";

            return fixture;
        },
        ResetAjaxSpy: function (obj) {
            var spy = obj.spies_[0];
            spy.baseObj["ajax"] = spy.originalValue;
        }
    };
});
