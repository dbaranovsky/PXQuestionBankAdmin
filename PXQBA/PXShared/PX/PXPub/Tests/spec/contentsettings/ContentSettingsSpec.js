/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" data-cover-adapter="../../lib/blanket/jasmine-blanket.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

/// <reference path="../../../Scripts/Datepicker/datepicker.js" />
/// <reference path="../../../Scripts/other/date.js" />
/// <reference path="../../../Scripts/Common/dateFormat.js" />

/// <reference path="../../../Scripts/SettingsTab/ContentSettings.js" />

describe("ContentWidget", function () {
    var fixture = '';

    beforeEach(function () {
        PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
        PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
    });

    afterEach(function () {
        $('body').children('div').not('#HTMLReporter').remove();
    });

    describe("Visibility", function () {
        beforeEach(function () {
            $.fn.ptTimeSelect = function () { };

            PxPage.Toasts = jasmine.createSpy("Toasts Spy");
            PxPage.Toasts.Error = jasmine.createSpy("Toasts Error Spy").andReturn(function (message) { return message; });

            ContentWidget = jasmine.createSpy("ContentWidget Spy");
            ContentWidget.AssignmentDateSelected = jasmine.createSpy("ContentWidget AssignmentDateSelected Spy");

            helper.GetVisibilityViewFixture();

            api.init();
        });

        it("can make item visible", function () {
            $('#visibleforstudent').click();

            expect($('#visibleforstudent').is(':checked')).toBeTruthy();
            expect($(".restricted-calendar").is(':visible')).toBeFalsy();
        });

        it("can make item invisible", function () {
            $('#hidefromstudent').click();

            expect($('#hidefromstudent').is(':checked')).toBeTruthy();
            expect($(".restricted-calendar").is(':visible')).toBeFalsy();
        });

        it("can make item restricted by date", function () {
            $('#restrictedbydate').click();

            expect($('#restrictedbydate').is(':checked')).toBeTruthy();
            expect($(".restricted-calendar").is(':visible')).toBeTruthy();
        });

        it("can validate restricted date and return true", function () {
            $("#settingsAssignDueDate").val("11/1/2023");
            $("#settingsAssignTime").val("11:59 PM");

            $('#restrictedbydate').click();
            $('#savecontentsettings').click();

            expect($("#savecontentsettings").is(':visible')).toBeTruthy();
        });

        it("can validate restricted date and return false if invalid", function () {
            $("#settingsAssignDueDate").val("33/22/2023");
            $("#settingsAssignTime").val("11:59 PM");

            $('#restrictedbydate').click();
            $('#savecontentsettings').click();

            expect(PxPage.Toasts.Error.mostRecentCall.args[0]).toBe('Invalid restriction date');
        });

        it("can validate restricted date and return false if less than current time", function () {
            $("#settingsAssignDueDate").val("11/1/2003");
            $("#settingsAssignTime").val("11:59 PM");

            $('#restrictedbydate').click();
            $('#savecontentsettings').click();

            expect(PxPage.Toasts.Error.mostRecentCall.args[0]).toBe("Restriction date can not be prior to today's date");
        });

        it("can set datetime in EST", function () {
            $("#TimeZoneStandardOffset").val('-300'); 
            $("#settingsAssignDueDate").val("11/1/2023");
            $("#settingsAssignTime").val("05:00 PM");

            $('#restrictedbydate').click();
            $('#savecontentsettings').click();

            expect($("#DueDate").val()).toBe("Wed, 01 Nov 2023 22:00:00 GMT");
        });

        it("can set datetime in PST", function () {
            $("#TimeZoneStandardOffset").val('-480'); 
            $("#settingsAssignDueDate").val("11/1/2023");
            $("#settingsAssignTime").val("05:00 PM");

            $('#restrictedbydate').click();
            $('#savecontentsettings').click();

            expect($("#DueDate").val()).toBe("Thu, 02 Nov 2023 01:00:00 GMT");
        });
    });

    var helper = {
        GetVisibilityViewFixture: function () {
            fixture = helper.GetVisibilityView();
            jasmine.Fixtures.prototype.addToContainer_(fixture);
        },
        GetVisibilityView: function () {
            var data = {
                viewPath: "Visibility",
                viewModel: JSON.stringify({
                    Content: {
                        Id: 1
                    }
                }),
                viewModelType: "Bfw.PX.PXPub.Models.ContentView"
            };

            var view = PxViewRender.RenderView('PXPub', 'ContentWidget', data);
            view += "<input type='textbox' id='TimeZoneStandardOffset' /><input type='button' id='savecontentsettings' />";

            return view;
        }
    };
});