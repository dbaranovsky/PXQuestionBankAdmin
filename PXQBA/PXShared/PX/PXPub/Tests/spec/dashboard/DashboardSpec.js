/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" /> 
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" />

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
/// <reference path="../../../Scripts/DashboardWidget/PxDashboard.js" />

describe("PxDashboard Tests:", function () {

    describe("PxDashboardWidget", function () {
        var fixture = '';
        var isLaunchpad = true;

        beforeEach(function () {
            PxPage.Routes.get_current_date_time = "dummypath";
            PxPage.Routes.show_roster_info = "dummypath";
            PxPage.Routes.InstructorConsole_FullViewRaw = "dummypath";
            PxPage.Routes.show_course_activation = "dummypath";

            if (this.description == "can show url on mouse enter" || this.description == "can hide url on mouse leave") {
                fixture = '';
                isLaunchpad = false;
            }

            if (fixture == '') {
                fixture = helper.GetDashboardSummaryView();
            }

            if (this.description == "can show url on mouse enter" || this.description == "can hide url on mouse leave") {
                isLaunchpad = true;
            }

            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var fixtureMain = "<div id='main' class='product-type-learningcurve'" + (this.description == "can generate edit links for learning curve" ? "class='product-type-learningcurve'" : "") + "></div>";

            jasmine.Fixtures.prototype.addToContainer_(fixtureMain);
            $('#main').PxDashboardWidget();

            PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
            PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
        });

        afterEach(function () {
            $('body').children('div').not('#HTMLReporter').remove();
        });

        it("should throw error if plugin method not found", function () {
            var errorThrown = false;

            $.error = jasmine.createSpy("jquery error Spy");
            $.error.andCallFake(function () {
                errorThrown = true;
            });

            $('#main').PxDashboardWidget('dummyMethod');

            expect(errorThrown).toEqual(true);
        });

        it("can show create course popup from launchpad", function () {
            helper.SetSpies();

            $('#school-list').val(helper.GetSchoolList());

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("dummyhtml");
            });

            $(PxPage.switchboard).trigger("PxDashboard.CreationDialog");

            expect($('.ui-dialog .dashboard-course-item')).toBeVisible();
        });

        it("can show create course popup from launchpad - find school autocomplete prepared", function () {
            helper.SetSpies();

            $('#school-list').val(helper.GetSchoolList());

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("dummyhtml");
            });

            $(PxPage.switchboard).trigger("PxDashboard.CreationDialog");

            $('#school-name').trigger("autocompleteopen");

            expect($('.ui-autocomplete .ui-menu-item').length > 0).toEqual(true);
        });

        it("can show create course popup from launchpad - initializes selected school search on select", function () {
            helper.SetSpies();

            $('#school-list').val(helper.GetSchoolList());

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("dummyhtml");
            });

            $(PxPage.switchboard).trigger("PxDashboard.CreationDialog");

            var ui = { item: { label: "" } };

            $('#school-name').trigger("autocompleteselect", ui);

            expect($('#cover #FindSchool')).toHaveClass('disable');
        });

        it("can show create course popup from launchpad - initializes selected school search on focus", function () {
            helper.SetSpies();

            $('#school-list').val(helper.GetSchoolList());

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("dummyhtml");
            });

            $(PxPage.switchboard).trigger("PxDashboard.CreationDialog");

            var ui = { item: { label: "" } };

            $('#school-name').trigger("autocompletefocus", ui);

            expect($('#cover #FindSchool')).toHaveClass('disable');
        });

        it("can show create course popup from launchpad - finds school", function () {
            helper.SetSpies();
            $('#school-list').val(helper.GetSchoolList());

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("dummyhtml");
            });

            $(PxPage.switchboard).trigger("PxDashboard.CreationDialog");

            $('#school-name').trigger("autocompleteopen");
            $('.find-school').trigger('click');

            expect($('.ui-autocomplete .ui-menu-item').length > 0).toEqual(true);
        });

        it("can toggle branch instructions - 1", function () {
            $('.instruction-1').trigger('click');
        });

        it("can toggle branch instructions - 2", function () {
            $('.instruction-2').trigger('click');
        });

        it("can show activate dashboard course popup", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.activate-dashboard-course[data-dw-id='1']").last().trigger('click');

            expect($('.ui-dialog #ActivateDashBoardCourse')).toHaveClass('activate-dashboard-course');
        });

        it("can activate dashboard course - error", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.activate-dashboard-course[data-dw-id='1']").last().trigger('click');
            $('.ui-dialog .primary-green').trigger('click');

            expect($('.ui-dialog #ActivateDashBoardCourse')).toHaveClass('activate-dashboard-course');
        });

        it("can activate dashboard course - success", function () {
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({ status: "True" });
            });

            $("a.activate-dashboard-course[data-dw-id='1']").last().trigger('click');
            $('.ui-dialog .primary-green').trigger('click');

            expect($('.ui-dialog #ActivateDashBoardCourse')).toHaveClass('activate-dashboard-course');
        });

        it("can show deactivate dashboard course popup", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.deactivate-dashboard-course").trigger('click');

            expect($('.ui-dialog').find('.pxdeactivatecourse-dialog-buttonpane').length > 0).toEqual(true);
        });

        it("can deactivate dashboard course - error", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.deactivate-dashboard-course").trigger('click');
            $('.ui-dialog .pxdeactivatecourse-dialog-buttonpane .primary-green').trigger('click');

            expect($('.ui-dialog').find('.pxdeactivatecourse-dialog-buttonpane').length > 0).toEqual(true);
        });

        it("can deactivate dashboard course - success", function () {
            $('.ui-dialog').remove();
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({ status: "True" });
            });
            
            $("a.deactivate-dashboard-course").first().trigger('click');
            $('.ui-dialog .pxdeactivatecourse-dialog-buttonpane .primary-green').trigger('click');
            expect($('.ui-dialog').length).toEqual(0);
        });

        it("can delete dashboard course", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.delete-dashboard-course").last().trigger('click');

            expect($('.ui-dialog').find('.pxdeletecourse-dialog-buttonpane').length > 0).toEqual(true);
        });

        it("can show roster info", function () {
            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("dummyhtml");
            });

            $("a.show-url-view-roster").trigger('click');

            expect($('.ui-dialog.roster-information-dialog').length > 0).toEqual(true);
        });

        it("can edit course info from activation dialog", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            var fne = jasmine.createSpyObj('LargeFNE', ['OpenFNELink']);
            PxPage.LargeFNE = fne;

            $("a.activate-dashboard-course[data-dw-id='1']").last().trigger('click');
            $('#ActivateDashBoardCourse .editactivatecourse').last().trigger('click');

            expect($(".dashboard-course-item").length == 0 || $(".dashboard-course-item").css('display') == 'none').toEqual(true);
        });

        it("can edit course info", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.edit-dashboard-course").last().trigger('click');

            expect($('.ui-dialog.dashboard-course-item').length > 0).toEqual(true);
        });

        it("can edit course info - find school autocomplete prepared", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.edit-dashboard-course").last().trigger('click');

            $('#school-name').val('dummytext');
            $('#school-name').trigger("focus");

            expect($('#school-name').val() == "dummytext").toEqual(true);
        });

        it("can edit course info - initializes selected school search on select", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.edit-dashboard-course").last().trigger('click');

            var ui = { item: { label: "" } };

            $('#school-name').trigger("autocompleteselect", ui);

            expect($('#cover #FindSchool')).toHaveClass('disable');
        });

        it("can edit course info - initializes selected school search on focus", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("a.edit-dashboard-course").last().trigger('click');

            var ui = { item: { label: "" } };

            $('#school-name').trigger("autocompletefocus", ui);

            expect($('#cover #FindSchool')).toHaveClass('disable');
        });

        it("can save course info - success", function () {
            helper.SetSpies();

            $("a.edit-dashboard-course").last().trigger('click');

            helper.PopulateEditCoursePopup();

            jasmine.Ajax.useMock();

            var courseResponse = {
                success: {
                    status: 200,
                    responseText: '{ "Domain": { "Id": "1", "Name": "Default" }, "AcademicTerm": "1" }'
                }
            };

            $('.ui-dialog').last().find('.primary-green').trigger('click');
            var request = mostRecentAjaxRequest();
            request.response(courseResponse.success);

            expect($(".dashboard-course-item")).toBeHidden();
        });

        it("can save course info - error", function () {
            helper.SetSpies();

            $("a.edit-dashboard-course").last().trigger('click');

            helper.PopulateEditCoursePopup();

            jasmine.Ajax.useMock();

            var courseResponse = {
                success: {
                    status: 200,
                    responseText: null
                }
            };

            $('.ui-dialog').last().find('.primary-green').trigger('click');

            var request = mostRecentAjaxRequest();
            request.response(courseResponse.success);

            expect($(".dashboard-course-item")).toBeVisible();
        });

        it("can show branch popup - no branches", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();
            $("a.create-another-branch-link").last().trigger('click');

            expect($('.ui-dialog.dashboard-course-item').length > 0).toEqual(true);
        });

        it("can show branch popup - branches exist (thru parent course link)", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("#isBranchCreated").val('true');
            $("a.create-another-branch-link").last().trigger('click');

            expect($('.ui-dialog.dashboard-course-item').length > 0).toEqual(true);
        });

        it("can show create course option popup", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("div#create-course").trigger('click');

            expect($('.ui-dialog .courseoption-dialog-buttonpane').length > 0).toEqual(true);
        });

        it("In show create course popup, data should not be prefilled with other course info.", function () {
            helper.SetSpies();
            spyOn($, 'ajax');
            $("div#create-course").trigger('click');
            //When option 'yes' is selected, selected course data will be saved.
            $('#courseoptionyes').trigger('click');
            //However, if option no is selected afterward, it should clear previous selected course data.
            $('#courseoptionno').trigger('click');

            $('.courseoption-dialog-next').trigger('click');

            expect(window.currentCourse.length).toEqual(0);
        });

        it("can show create course popup", function () {
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    Domain: { Id: "1", Name: "Default" },
                    LmsIdRequired: true
                });
            });

            $("div#create-course").trigger('click');
            $('.ui-state-default').trigger('click');

            expect($('.ui-dialog').find('#dashboard-course-creation-screen').length > 0).toEqual(true);
        });

        it("can create course from popup - course title not provided", function () {
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    Domain: { Id: "1", Name: "Default" },
                    LmsIdRequired: true
                });
            });

            $("div#create-course").trigger('click');
            $('.ui-state-default').trigger('click');

            $('#school-list').val(helper.GetSchoolList());

            $('.ui-dialog').find('.primary-green').trigger('click');

            expect($('#error-message').css('display') == "block").toEqual(true);
        });

        it("can create course from popup - academic term not provided", function () {
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    Domain: { Id: "1", Name: "Default" },
                    LmsIdRequired: true
                });
            });

            $("div#create-course").trigger('click');
            $('.ui-state-default').trigger('click');

            $('#school-list').val(helper.GetSchoolList());
            $('#cover #course-title').val("Test Course");

            $('.ui-dialog').find('.primary-green').trigger('click');

            expect($('#error-message').css('display') == "block").toEqual(true);
        });

        it("can create course from popup - ajax exception", function () {
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    Id: "1",
                    AcademicTerm: "Summer",
                    CourseTimeZone: "EST",
                    Domain: { Id: "1", Name: "Default" },
                    LmsIdRequired: true
                });
            });

            $("div#create-course").trigger('click');
            $('.ui-state-default').trigger('click');

            helper.PopulateEditCoursePopup();
            helper.ResetAjaxSpy(this);

            spyOn($, 'ajax').andCallFake(function (params) {
                params.error("dummyerror");
            });

            $('.ui-dialog').find('.primary-green').trigger('click');

            expect($('.ui-dialog .dashboard-course-item').length > 0).toEqual(true);
        });

        it("can create course from popup - not a branch", function () {
            helper.SetSpies();
            
            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    Id: "1",
                    AcademicTerm: "Summer",
                    CourseTimeZone: "EST",
                    Domain: { Id: "1", Name: "Default" },
                    LmsIdRequired: true
                });
            });

            $("div#create-course").trigger('click');
            $('.ui-state-default').trigger('click');

            helper.PopulateEditCoursePopup();

            $('.ui-dialog').find('.primary-green').trigger('click');

            expect($('.ui-dialog:visible').length == 0).toEqual(true);
        });

        it("can create course from popup - a branch", function () {
            $('.ui-dialog').remove();
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    Id: "1",
                    AcademicTerm: "Summer",
                    CourseTimeZone: "EST",
                    Domain: { Id: "1", Name: "Default" },
                    LmsIdRequired: true
                });
            });
            
            $("a.create-another-branch-link").last().trigger('click');
            $('.ui-dialog .primary-green').trigger('click');

            helper.PopulateEditCoursePopup();

            $('.ui-dialog .primary-green').eq(1).trigger('click');

            expect($("#isBranchCreated").val() == "true").toEqual(true);
        });

        it("can update course from instructor console", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            var fne = jasmine.createSpyObj('LargeFNE', ['OpenFNELink']);
            PxPage.LargeFNE = fne;

            $("a.activate-dashboard-course[data-dw-id='1']").last().trigger('click');
            $('#ActivateDashBoardCourse .editactivatecourse').last().trigger('click');

            spyOn(JSON, 'parse').andCallFake(function () {
                return { Id: "1", AcademicTermText: "Summer" };
            });

            $(PxPage.switchboard).trigger("InstructorDashboard.CourseSaved");

            expect($(".dashboard-course-item").length == 0 || $(".dashboard-course-item").css('display') == 'none').toEqual(true);
        });

        it("can display list of courses to create a copy", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("div#create-course").trigger('click');
            $("#CreateCourseOption #courseoptionyes").trigger('click');

            expect($('.ui-dialog').find('table.dashboardgrid').length > 0).toEqual(true);
        });

        it("can hide list of courses to create a copy", function () {
            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("div#create-course").trigger('click');
            $("#CreateCourseOption #courseoptionyes").trigger('click');
            $("#CreateCourseOption #courseoptionno").trigger('click');

            expect($('.ui-dialog').find('table.dashboardgrid').length == 0).toEqual(true);
        });

        it("can display list of academic terms on the create/edit course screen", function () {
            helper.SetSpies();

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success({
                    Domain: {
                        Id: "1", Name: "Default"
                    },
                    LmsIdRequired: true
                });
            });

            $("div#create-course").trigger('click');
            $('.ui-state-default').trigger('click');

            helper.ResetAjaxSpy(this);

            spyOn($, 'ajax').andCallFake(function (params) {
                params.success(helper.GetAcademicTerms());
            });

            $('#cover #school-name').trigger("blur");

            expect($('select#academicTerm').find('option').length > 0).toEqual(true);
        });

        it("can show url", function () {
            var priorValue = $("a.show-url-hover").last().nextAll('.show-url').css('display');

            $("a.show-url-hover").last().trigger('click');

            expect($("a.show-url-hover").last().nextAll('.show-url').css('display') == priorValue).toEqual(false);
        });

        it("can show url on mouse enter", function () {
            $("table .entityidofcourse").last().trigger('mouseenter');

            expect($("table .entityidofcourse").last().find('.edit-course-link').css('display') == 'block').toEqual(true);
        });

        it("can hide url on mouse leave", function () {
            $("table .entityidofcourse").last().trigger('mouseleave');

            expect($("table .entityidofcourse").last().find('.edit-course-link').css('display') == 'none').toEqual(true);
        });

        it("can show delete confirmation popup", function () {
            $("a.show-url-delete").last().trigger('click');

            expect($('.red-background-button').length > 0).toEqual(true);
        });

        it("can click cancel on delete confirmation dialog", function () {
            $("a.show-url-delete").last().trigger('click');
            $('.courseoption-dialog-cancel').trigger('click');

            expect($('.ui-dialog .courseoption-dialog-cancel:visible').length == 0).toEqual(true);
        });

        it("can delete course from delete confirmation dialog - 0 level", function () {
            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("True");
            });

            $("a.show-url-delete").first().trigger('click');
            $('.red-background-button').trigger('click');

            expect($('.red-background-button:visible').length == 0).toEqual(true);
        });

        it("can delete course from delete confirmation dialog - 1 level", function () {
            spyOn($, 'ajax').andCallFake(function (params) {
                params.success("True");
            });

            $("a.show-url-delete").last().trigger('click');
            $('.red-background-button').trigger('click');

            expect($('.red-background-button:visible').length == 0).toEqual(true);
        });

        it("can activate course from launchpad", function () {
            var activationLinkFixture = helper.GetActivationLinkFixture();
            jasmine.Fixtures.prototype.addToContainer_(activationLinkFixture);

            helper.SetSpies();
            helper.SetDateTimeAjax();

            $('#lnkActivateCourse').trigger('click');

            expect($('.ui-dialog')).toHaveClass('dashboard-course-item');
        });

        it("can generate edit links for learning curve", function () {
            expect($('a.edit-course-link').attr('href').indexOf('dummypath') > -1).toEqual(true);
        });

        it("can activate course from activation dialog", function () {
            var activationFixture = helper.GetActivationSummaryView();
            jasmine.Fixtures.prototype.addToContainer_(activationFixture);

            helper.SetSpies();
            helper.SetDateTimeAjax();

            $("#lnkActivateCourse").trigger('click');

            expect($('.ui-dialog #ActivateDashBoardCourse')).toHaveClass('activate-dashboard-course');
        });

        var helper = {
            SetSpies: function () {
                spyOn(PxPage, 'Update').andCallFake(function () {
                    return null;
                });
            },
            SetDateTimeAjax: function () {
                spyOn($, 'ajax').andCallFake(
                    function (options) {
                        options.success(new Date());
                    }
                );
            },
            GetDashboardSummaryView: function () {
                var courses = [];
                courses.push({ CourseId: "1", Level: 0, Course: { Id: "1", Title: "Test Course", AcademicTerm: "Summer", ActivatedDate: new Date() } });
                courses.push({ CourseId: "2", Level: 1, Course: { Id: "2", Title: "Test Course", AcademicTerm: "Summer", ActivatedDate: new Date() } });
                courses.push({ CourseId: "3", Level: 0, Course: { Id: "3", Title: "Test Course", AcademicTerm: "Summer", ActivatedDate: '01/01/0001' }, Status: "open" });
                courses.push({ CourseId: "4", Level: 1, Course: { Id: "4", Title: "Test Course", AcademicTerm: "Summer", ActivatedDate: '01/01/0001' }, Status: "open" });

                var data = {
                    viewPath: "Summary",
                    viewModel: JSON.stringify({
                        LaunchPadMode: isLaunchpad,
                        AllowActivateButtonColumn: true,
                        AllowCreateAnotherBranchColumn: true,
                        AllowCoureTitleColumn: true,
                        AllowCourseIdColumn: true,
                        AllowEnrollmentCountColumn: true,
                        AllowViewingRoster: true,
                        AllowEditingCourseInformation: true,
                        AllowDeleteButtonColumn: true,
                        PossibleAcademicTerms: helper.GetAcademicTerms(),
                        InstructorCourses: courses,
                        Course: {
                            Title: "Test Course",
                            AcademicTerm: "Summer",
                            Domain: {
                                Id: "1",
                                Name: "Default"
                            }
                        }
                    }),
                    viewModelType: "Bfw.PX.PXPub.Models.DashboardData"
                };

                return PxViewRender.RenderView('PXPub', 'DashboardCoursesWidget', data);
            },
            GetActivationSummaryView: function () {
                var viewData = {
                    PossibleAcademicTerms: {
                        dataType: "System.Collections.Generic.IEnumerable`1[Bfw.PX.PXPub.Models.CourseAcademicTerm]",
                        dataValue: helper.GetAcademicTerms()
                    }
                };

                var data = {
                    viewPath: "Summary",
                    viewModelType: "Bfw.PX.PXPub.Models.Course",
                    viewModel: JSON.stringify({
                        Id: "1", Title: "Test Course", AcademicTerm: "Summer", ActivatedDate: new Date()
                    }),
                    viewData: JSON.stringify(viewData)
                }

                return PxViewRender.RenderView('PXPub', 'CourseActivationWidget', data);
            },
            PopulateEditCoursePopup: function () {
                $('#school-list').val(helper.GetSchoolList());
                $('#cover #course-title').val("Test Course");
                $("#school-name").val('Yale University');
            },
            GetAcademicTerms: function () {
                var possibleAcademicTerms = [];

                possibleAcademicTerms.push({ Name: "Summer", Id: "1" });
                possibleAcademicTerms.push({ Name: "Winter", Id: "2" });

                return possibleAcademicTerms;
            },
            GetSchoolList: function () {
                var schools = '[{ "label": "Aaron Academy (New York, NY)", "value": "109883" },{ "label": "Abington High School (Abington, MA)", "value": "117068" },{ "label": "Academic Enterprise Inc (Woonsocket, RI)", "value": "117071" },{ "label": "Academy for Information Tech", "value": "129207" },{ "label": "Academy of Pac Rim-Charter Sch (Hyde Park, MA)", "value": "117069" },{ "label": "Academy of the Pacific Rim (Hyde Park, MA)", "value": "116518" },{ "label": "Alabama Southern Community College", "value": "65083" },{ "label": "Alabama State Fire College", "value": "65075" },{ "label": "Alta Colleges (Denver, CO)", "value": "118010" },{ "label": "Andover College (South Portland, ME)", "value": "110566" },{ "label": "Arapahoe CC", "value": "63049" },{ "label": "Arapahoe CC", "value": "109901" },{ "label": "Attleboro High School (Attleboro, MA)", "value": "117759" },{ "label": "Babson College (Wellesley Hills, MA)", "value": "117815" },{ "label": "Baruch College CUNY", "value": "66159" },{ "label": "Baruch College CUNY", "value": "128464" },{ "label": "Bellingham Public School (Bellingham, MA)", "value": "117773" },{ "label": "Berkeley College (Woodbridge, NJ)", "value": "86733" },{ "label": "Bevill State CC - Sumiton", "value": "65081" },{ "label": "bfwusers", "value": "111316" },{ "label": "BFWUsers", "value": "8841" },{ "label": "Bishop Feehan High School (Attleboro, MA)", "value": "117793" },{ "label": "Boston University (Boston, MA)", "value": "120714" },{ "label": "Brevard CC", "value": "63058" },{ "label": "Brooklyn Friends School", "value": "109881" },{ "label": "Brown University (Providence, RI)", "value": "117799" },{ "label": "Bryant University (Smithfield, RI)", "value": "117072" },{ "label": "Cal Sch of Prof Psych-fresno", "value": "65274" },{ "label": "California Graduate Institute", "value": "65280" },{ "label": "Career Systems Dev Corp (North Grafton, MA)", "value": "116515" },{ "label": "Caritas Academy (Jersey City, NJ)", "value": "86695" },{ "label": "Central Falls High School (Central Falls, RI)", "value": "117702" },{ "label": "Chubb Institute (Parsippany, NJ)", "value": "96359" },{ "label": "City College CUNY (New York, NY)", "value": "89639" },{ "label": "Clark University (Worcester, MA)", "value": "118112" },{ "label": "College of Staten Island", "value": "63055" },{ "label": "Colorado Col Timberline", "value": "84809" },{ "label": "Computer Ed Business Institute (North Providence, RI)", "value": "118955" },{ "label": "Computer-ed Inc (Lincoln, RI)", "value": "117918" },{ "label": "Cumberland High School (Cumberland, RI)", "value": "117701" },{ "label": "Depaul University", "value": "73408" },{ "label": "Depaul University 12323", "value": "73624" },{ "label": "Devry university", "value": "74415" },{ "label": "Eastern Kentucky &amp; Test", "value": "63063" },{ "label": "Emmanuel College (Boston, MA)", "value": "118111" },{ "label": "Fairview College", "value": "63315" },{ "label": "Fairview College", "value": "63431" },{ "label": "Fairview College", "value": "65162" },{ "label": "Fairview College", "value": "84489" },{ "label": "Fashion Inst of Technology", "value": "128830" },{ "label": "Finger Lakes CC", "value": "63066" },{ "label": "foo", "value": "110305" },{ "label": "Fordham University (Bronx, NY)", "value": "89464" },{ "label": "Forsyth Tech", "value": "63061" },{ "label": "Franklin High School (Franklin, MA)", "value": "119204" },{ "label": "General High Schools (New York, NY)", "value": "96351" },{ "label": "Genesis Center (Providence, RI)", "value": "116517" },{ "label": "Georgia State University", "value": "63047" },{ "label": "Georgia Tech", "value": "63056" },{ "label": "Grand Valley State University", "value": "63062" },{ "label": "Harvard Medical School (Boston, MA)", "value": "120379" },{ "label": "Harvard U/Sch of Public Health (Boston, MA)", "value": "120563" },{ "label": "Hennepin Technical College", "value": "73403" },{ "label": "Hope High School (Providence, RI)", "value": "119471" },{ "label": "Hunter College", "value": "63053" },{ "label": "Ivy Tech", "value": "63067" },{ "label": "Johnson ", "value": "120373" },{ "label": "Johnson and Wales University (Providence, RI)", "value": "114875" },{ "label": "La Guardia Comm Coll Cuny (Long Island City, NY)", "value": "119708" },{ "label": "Lakeland College", "value": "63525" },{ "label": "Liberty High School (Jersey City, NJ)", "value": "86761" },{ "label": "Lincoln High School (Lincoln, RI)", "value": "119174" },{ "label": "Magellan Distance Learning Ctr", "value": "65096" },{ "label": "Manhattan College (Bronx, NY)", "value": "89631" },{ "label": "Manhattan Village Academy (New York, NY)", "value": "86826" },{ "label": "Maricopa County Community Colleges AND Chandler-Gilbert CC", "value": "63064" },{ "label": "Math for America (New York, NY)", "value": "95536" },{ "label": "Mattersnot", "value": "116386" },{ "label": "Medicine Hat College-brooks", "value": "63546" },{ "label": "Medicine Hat College-brooks", "value": "84808" },{ "label": "Metropolitan College of NY (New York, NY)", "value": "120713" },{ "label": "Michigan Technological University", "value": "63051" },{ "label": "Middlesex Co Voc-woodbridge (Woodbridge, NJ)", "value": "110718" },{ "label": "Middlesex County College (Edison, NJ)", "value": "87390" },{ "label": "Mohave Comm Coll-Mohave Valley", "value": "65166" },{ "label": "Morehouse", "value": "63069" },{ "label": "New York City College of Technology", "value": "125981" },{ "label": "New York University", "value": "62079" },{ "label": "New York University", "value": "62084" },{ "label": "New York University", "value": "73387" },{ "label": "Olds College", "value": "84798" },{ "label": "Park University", "value": "65177" },{ "label": "Passaic CC", "value": "63054" },{ "label": "Pikes Peak CC", "value": "63059" },{ "label": "Pima County CC- Desert Vista", "value": "65191" },{ "label": "Royal Roads University", "value": "65262" },{ "label": "Rutgers-Newark Urban Teacher Education", "value": "63045" },{ "label": "Santa Fe College", "value": "63050" },{ "label": "Sawyer School (Providence, RI)", "value": "120371" },{ "label": "School of Fish", "value": "110306" },{ "label": "School of Hard Knocks", "value": "110530" },{ "label": "School of the Future (New York, NY)", "value": "107483" },{ "label": "School of Visual Arts (New York, NY)", "value": "128489" },{ "label": "Seaside High School", "value": "25459" },{ "label": "Shelton State CC - Skyland", "value": "65094" },{ "label": "Some University", "value": "73636" },{ "label": "Southeastern Business Academy", "value": "65089" },{ "label": "Southern Polytech State U", "value": "63052" },{ "label": "Southwest College of Naturopathic Medicine", "value": "65197" },{ "label": "Southwestern College", "value": "122670" },{ "label": "Stonehill College (North Easton, MA)", "value": "118073" },{ "label": "Test Domain 1", "value": "65050" },{ "label": "Test Domain 2", "value": "65091" },{ "label": "Test Domain 3", "value": "65092" },{ "label": "Test Domain 4", "value": "65093" },{ "label": "TEST Univ of Cantstandya (New York, NY)", "value": "120715" },{ "label": "Texas A&amp;M CC", "value": "63057" },{ "label": "Touro College (New York, NY)", "value": "86780" },{ "label": "Travel Institute Inc (Jersey City, NJ)", "value": "88644" },{ "label": "U of Delaware", "value": "63048" },{ "label": "UConn Waterbury", "value": "63555" },{ "label": "UFT Math Teachers Comm (New York, NY)", "value": "107569" },{ "label": "Univ of Alaska Ft Richardson", "value": "73401" },{ "label": "Univ of Arizona-South", "value": "65264" },{ "label": "Univ of Rhode Island Providenc (Providence, RI)", "value": "113353" },{ "label": "University of Advancing Technology", "value": "65164" },{ "label": "University Of Central Texas", "value": "84445" },{ "label": "University of Deleware", "value": "62078" },{ "label": "University of Georgia", "value": "25455" },{ "label": "University of Mississippi", "value": "63046" },{ "label": "University of Redlands", "value": "73402" },{ "label": "University of Rhode Island", "value": "63044" },{ "label": "University of Rhode Island (Kingston, RI)", "value": "113354" },{ "label": "University of Rhode Island (Kingston, RI)", "value": "117353" },{ "label": "University of Rhode Island (Kingston, RI)", "value": "117591" },{ "label": "University of Rhode Island (Kingston, RI)", "value": "117881" },{ "label": "Uofpheonix", "value": "75609" },{ "label": "Uofpheonix", "value": "80744" },{ "label": "UW- Eau Claire", "value": "63065" },{ "label": "Wallington High School (Wallington, NJ)", "value": "100672" },{ "label": "Wayne State", "value": "63060" },{ "label": "Wayne State", "value": "63068" },{ "label": "Yale University", "value": "113498" }]';

                return schools;
            },
            GetActivationLinkFixture: function () {
                var link = '<a id="lnkActivateCourse" name="lnkActivateCourse" data-dw-id="125007" class="creation-button activate-button activation-button activate-from-course-page"><div class="create-instructions"><span class="acticon pxicon pxicon-warningsign"></span><p>Click here to <b>activate your course</b> when you\'re ready to distribute the courseURL to your students.</p></div></a>';

                return link;
            },
            ResetAjaxSpy: function (obj) {
                var spy = obj.spies_[1];
                spy.baseObj["ajax"] = spy.originalValue;
            }
        };
    });
});