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

/// <reference path="../../../Scripts/InstructorConsoleWidget/InstructorConsole.js" />

describe("PxInstructorConsoleWidget Tests: ", function () {
    describe('Test Gradebook Preference, ', function() {
        beforeAll(function() {
            window.BaseTestHelper.api.clearCache();
        });
        beforeEach(function() {
            var model = {
                path: "GradebookPreferences",
                type: "Bfw.PX.PXPub.Models.Course"
            };
            var categoryModel =
            {
                viewModel: JSON.stringify({ "PassingScore": 0, "UseWeightedCategories": false, "GradeBookWeights": { "WeightedCategories": false, "CategoryWeightTotal": 100, "Total": 0, "TotalWithExtraCredit": 0, "GradeWeightCategories": [{ "Id": "0", "Text": "Uncategorized", "DropLowest": "0", "Sequence": "a", "Weight": "100", "ItemWeightTotal": "0", "Percent": "0", "PercentWithExtraCredit": null, "Items": [] }, { "Id": "1", "Text": "test", "DropLowest": "0", "Sequence": "b", "Weight": "0", "ItemWeightTotal": "0", "Percent": "0", "PercentWithExtraCredit": null, "Items": [] }], type: 'Bfw.PX.PXPub.Models.GradeBookWeights' } }),
                viewPath: 'GradebookCategoriesList',
                viewModelType: 'Bfw.PX.PXPub.Models.GradebookPreferences'
            };

            var viewModel = window.BaseTestHelper.model.generateViewModel(model);
            
            window.BaseTestHelper.api.setFixtureFromCache(viewModel, 'InstructorConsoleWidget', 'set');
            window.BaseTestHelper.api.setFixtureFromCache(categoryModel, 'InstructorConsoleWidget', 'target', '#gradebookCategoriesList');

        });
        
        it("should have no repeated bind events", function () {
            PxInstructorConsoleWidget.Init();
            var expectedNumberOfBindEvent = 0, c, actualNmberOfBindEvent = 0;
            //We get the number of bind event after initialization
            for (c in $('#passingScore').data('events')) {
                expectedNumberOfBindEvent += $('#passingScore').data('events')[c].length;
            }
            //call initialization again, this should not increase the number of binded event.
            PxInstructorConsoleWidget.Init();
            PxInstructorConsoleWidget.Init();
            for (c in $('#passingScore').data('events')) {
                actualNmberOfBindEvent += $('#passingScore').data('events')[c].length;
            }
            //If numbers are not the same, then we have repeated bindings and the test should failed.
            expect(expectedNumberOfBindEvent).toBe(actualNmberOfBindEvent);

        });

        it('when click on "remove" category, a dialog should pop up', function() {

            PxInstructorConsoleWidget.Init();
            $('.removeCategory').click();
            expect($('#px-dialog.ui-dialog-content').length).toNotBe(0);
            $('#px-dialog.ui-dialog-content').dialog('destroy').remove();
        });
        
        it('when click on "remove" category, expect to update category list', function () {

            PxInstructorConsoleWidget.Init();
            $('.removeCategory').click();
            expect($('#px-dialog.ui-dialog-content').length).toNotBe(0);
            spyOn(PxInstructorConsoleWidget, 'updateCategoryListbyAjax');
            $('#px-dialog').parent().find('.ui-dialog-buttonset button').first().click();
            expect(PxInstructorConsoleWidget.updateCategoryListbyAjax).toHaveBeenCalled();
            $('#px-dialog.ui-dialog-content').dialog('destroy').remove();
        });
        
        it('when click on "cancel" category, expect the dialog closes', function () {

            PxInstructorConsoleWidget.Init();
            $('.removeCategory').click();
            expect($('#px-dialog.ui-dialog-content').length).toNotBe(0);
            $('#px-dialog').parent().find('.ui-dialog-buttonset button').get(1).click();
            expect($('#px-dialog.ui-dialog-content').length).toBe(0);

        });
        
    });

    describe("Summary View", function () {
        var fixture = '';

        afterEach(function () {
            $('body').children('div').not('#HTMLReporter').remove();
        });

        it("can render the Return to Welcome Screen button if enabled", function () {
            var viewData = {
                ShowWelcomeReturn: {
                    dataType: "System.Boolean",
                    dataValue: true
                }
            };

            fixture = helper.GetSummaryView(viewData);
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            expect($('#CW_ReturnButton').length > 0).toBeTruthy();
        });

        it("can't render the Return to Welcome Screen button if disabled", function () {
            var viewData = {
                ShowWelcomeReturn: {
                    dataType: "System.Boolean",
                    dataValue: false
                }
            };

            fixture = helper.GetSummaryView(viewData);
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            expect($('#CW_ReturnButton').length > 0).toBeFalsy();
        });
    });

    describe("Edit View", function () {
        var fixture = '';

        afterEach(function () {
            $('body').children('div').not('#HTMLReporter').remove();
        });

        it("can render the Return to Welcome Screen checkbox if enabled", function () {
            var viewData = {
                IsLoadStartOnInit: {
                    dataType: "System.Boolean",
                    dataValue: true
                }
            };

            fixture = helper.GetEditView(viewData);
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            expect($('#ShowWelcomeReturn').length > 0).toBeTruthy();
        });

        it("can't render the Return to Welcome Screen checkbox if disabled", function () {
            var viewData = {
                IsLoadStartOnInit: {
                    dataType: "System.Boolean",
                    dataValue: false
                }
            };

            fixture = helper.GetEditView(viewData);
            jasmine.Fixtures.prototype.addToContainer_(fixture);

            expect($('#ShowWelcomeReturn').length > 0).toBeFalsy();
        });
    });

    describe("Batch Updater", function () {
        var fixture = '';

        beforeEach(function () {
            if (fixture == '') {
                fixture = helper.GetBatchUpdaterView();                
            }

            jasmine.Fixtures.prototype.addToContainer_(fixture);            

            var fixtureMain = "<div id='main'></div>";
            jasmine.Fixtures.prototype.addToContainer_(fixtureMain);

            PxInstructorConsoleWidget.Init();

            PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
            PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
            PxPage.Toasts.Warning = jasmine.createSpy("PxPage.Toasts.Warning Spy");
            PxPage.Toasts.Info = jasmine.createSpy("PxPage.Toasts.Info Spy");
        });

        afterEach(function () {
            $('body').children('div').not('#HTMLReporter').remove();
        });

        it("can show date picker - from date", function () {
            $('#fromDueDateCal').trigger('click');

            expect($('.ui-dialog #PXAssignCalendar')).toBeVisible();
        });

        it("can show date picker - to date", function () {
            $('#toDueDateCal').trigger('click');

            expect($('.ui-dialog #PXAssignCalendar')).toBeVisible();
        });

        it("can show date picker - new date", function () {
            $('#newDueDateCal').trigger('click');

            expect($('.ui-dialog #PXAssignCalendar')).toBeVisible();
        });

        it("can show warning if selected to date lesser than from date", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            expect($('#toDueDateCal').val() == '').toEqual(true);
        });

        it("can show warning if selected from date greater than to date", function () {
            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            expect($('#toDueDateCal').val() == '').toEqual(true);
        });

        it("can show number of days to shift after selecting from and to dates", function () {
            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({ itemCount: 41, fromDate: '07/28/2013', toDate: '09/07/2013' });
            });

            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            expect($("#totalItems").text() == "You have 41 assignments due between 07/28/2013 and 09/07/2013.").toEqual(true);
        });

        it("can show number of days to shift after selecting to and from dates", function () {
            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({ itemCount: 41, fromDate: '07/28/2013', toDate: '09/07/2013' });
            });

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            expect($("#totalItems").text() == "You have 41 assignments due between 07/28/2013 and 09/07/2013.").toEqual(true);
        });

        it("can show confirmation message", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#newDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            expect($("#totalDaysShifted").text() == "All selected due dates will be shifted  +41  days.").toEqual(true);
        });

        it("cannot update assignment dates - from date is missing", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#newDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $(".dueDate #fromDate").val('');

            $('#btnBatchDueDateUpdate').trigger('click');

            expect($("#totalDaysShifted").text() != '').toEqual(true);
        });

        it("cannot update assignment dates - to date is missing", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#newDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $(".dueDate #toDate").val('');

            $('#btnBatchDueDateUpdate').trigger('click');

            expect($("#totalDaysShifted").text() != '').toEqual(true);
        });

        it("cannot update assignment dates - new date is missing", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#newDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $(".dueDate #newDueDate").val('');

            $('#btnBatchDueDateUpdate').trigger('click');

            expect($("#totalDaysShifted").text() != '').toEqual(true);
        });

        it("cannot update assignment dates - new date is less than from date (year leap)", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            var newDate = new Date($(".dueDate #fromDate").val());
            newDate.addYears(-1);
            $(".dueDate #toDate").val(newDate.toString('MM/dd/yyyy'));

            PxInstructorConsoleWidget.setToDate({ DueDate: $(".dueDate #toDate").val() });

            expect(PxPage.Toasts.Warning.wasCalled).toBeTruthy();
        });

        it("cannot update assignment dates - from date is greater than new date (year leap)", function () {
            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            var newDate = new Date($(".dueDate #toDate").val());
            newDate.addYears(1);
            $(".dueDate #fromDate").val(newDate.toString('MM/dd/yyyy'));

            PxInstructorConsoleWidget.setFromDate({ DueDate: $(".dueDate #fromDate").val() });

            expect(PxPage.Toasts.Warning.wasCalled).toBeTruthy();
        });

        it("can update assignment dates - new date is greater than from date (year leap)", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            var newDate = new Date($(".dueDate #fromDate").val());
            newDate.addYears(1);
            $(".dueDate #toDate").val(newDate.toString('MM/dd/yyyy'));

            PxInstructorConsoleWidget.setToDate({ DueDate: $(".dueDate #toDate").val() });

            expect(PxPage.Toasts.Warning.wasCalled).toBeFalsy();
        });

        it("can update assignment dates - from date is less than new date (year leap)", function () {
            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            var newDate = new Date($(".dueDate #toDate").val());
            newDate.addYears(-1);
            $(".dueDate #fromDate").val(newDate.toString('MM/dd/yyyy'));

            PxInstructorConsoleWidget.setFromDate({ DueDate: $(".dueDate #fromDate").val() });

            expect(PxPage.Toasts.Warning.wasCalled).toBeFalsy();
        });

        it("can update assignment days", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#newDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({ status: 'success' });
            });

            $('#btnBatchDueDateUpdate').trigger('click');

            expect($("#totalDaysShifted").text() == '').toEqual(true);
        });

        it("can pass restricted by date flag", function () {
            $('#fromDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').first().trigger('click');

            $('#toDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#newDueDateCal').trigger('click');
            $('.ui-dialog .datepickerDays').find('a span').last().trigger('click');

            $('#updateRestrictedDates').prop('checked', true);

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    success: expect(params.data.updateRestrictedDates).toEqual(true)
                });
            });

            $('#btnBatchDueDateUpdate').trigger('click');
        });
    });
    describe("About Course Widget", function() {
        var fixture = '';

        beforeEach(function () {
            if (fixture == '') {
                fixture = helper.GetAboutCourseWidgetView();
            }

            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var fixtureMain = "<div id='main'></div>";
            jasmine.Fixtures.prototype.addToContainer_(fixtureMain);

            PxInstructorConsoleWidget.Init();

            PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
            PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
            PxPage.Toasts.Warning = jasmine.createSpy("PxPage.Toasts.Warning Spy");
            PxPage.Toasts.Info = jasmine.createSpy("PxPage.Toasts.Info Spy");
            PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
        });

        afterEach(function () {
            $('body').children('div').not('#HTMLReporter').remove();
        });

        it("shows LMS student id when show is clicked", function() {
            $(".lms-action-show").trigger("click");
            expect($(".lms-id-label")).toBeVisible();
        });
        
        it("hides LMS student id when hide is clicked", function () {
            $(".lms-action-hide").trigger("click");
            expect($(".lms-id-label").is(":visible")).toBeFalsy();
        });
        
        it("shows LMS student textbox when edit is clicked", function () {
            $(".lms-action-edit").trigger("click");
            expect($("#txtLmsId").is(":visible")).toBeTruthy();
        });
        
        it("saves LMS id when save is clicked and label is updated, toast is shown", function () {
            var ajaxCalled = false;
            $("#txtLmsId").val("newLmsId");
            spyOn($, 'post').andCallFake(function (url, data, callback){
                if (data.lmsId == "newLmsId") {
                    ajaxCalled = true;
                }
                callback({ success: true });
                return data;
            });

            $(".lms-action-save").trigger("click");
            expect(ajaxCalled).toBeTruthy();
            expect($(".lms-id-label").text()).toEqual("newLmsId");
            expect(PxPage.Toasts.Success.wasCalled).toBeTruthy();
        });
        
        it("hides text box when cancel is clicked", function() {
            $(".lms-action-cancel").trigger("click");
            expect($("#txtLmsId").is(":visible")).toBeFalsy();
        });

    });
    var helper = {
        GetSummaryView: function (viewData) {
            var data = {
                viewPath: "Summary",
                viewData: JSON.stringify(viewData)
            };

            return PxViewRender.RenderView('PXPub', 'InstructorConsoleWidget', data);
        },
        GetEditView: function (viewData) {
            var data = {
                viewPath: "EditView",
                viewData: JSON.stringify(viewData)
            };

            return PxViewRender.RenderView('PXPub', 'InstructorConsoleWidget', data);
        },
        GetBatchUpdaterView: function () {
            var data = {
                viewPath: "BatchDueDateUpdater"
            };

            return PxViewRender.RenderView('PXPub', 'InstructorConsoleWidget', data);
        },
        GetAboutCourseWidgetView: function () {
            var data = {
                viewPath: "Index",
                viewModelType: "Bfw.PX.PXPub.Models.AboutCourse",
                viewModel: JSON.stringify({
                    LmsIdEnabled: "True",
                    CampusLmsId: "lmsId",
                    AccessLevel: 4
                })
            };

            return PxViewRender.RenderView('PXPub', 'AboutCourseWidget', data);
        }
    };
});