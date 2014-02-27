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

/// <reference path="../../../Scripts/other/date.js" />
/// <reference path="../../../Scripts/Common/dateFormat.js" />

/// <reference path="../../../Scripts/ContentWidget/ContentWidget.js" />

describe("ContentWidget", function () {
    var fixture = '';

    //mock pxpage
    PxPage = {
        switchboard: $(document),
        log: function () {
        },
        Loading: function () {
        },
        Loaded: function () {
        },
        Update: function () {
        },
        CloseNonModal: function () {
        },
        Routes: {
            assign_item: ''
        }
    };

    PxContentTemplates = {
        CreateItemFromTemplate: function (templateId, callback, parentId, title) {

        },
        ShowMoreDialog: function () {

        },
        GetTemplateReloadMode: function () {

        }
    };

    beforeEach(function () {
        PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
        PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
    });

    afterEach(function () {
        $('body').children('div').not('#HTMLReporter').remove();
    });

    describe('CreateAndAssign', function () {
        it("can submit correct value of 'Include in Gradebook'", function () {
            helper.GetCreateAssignmentTabViewFixture();

            var result = '';

            $('#btnAssign').bind('click', function () {
                result = ContentWidget.CreateAndAssign();
            });

            $('#selIncludeGbbScoreTrigger').prop('checked', true);
            $('#btnAssign').trigger('click');

            expect(result.indexOf('IncludeGbbScoreTrigger=2') > 0).toEqual(true);
        });

        it("can submit correct value of 'Include in Gradebook'", function () {
            helper.GetCreateAssignmentTabViewFixture();

            $('#btnAssign').bind('click', function () {
                ContentWidget.ContentAssigned('assign', 1, null, $('.contentcreate'), null, false);
            });

            jasmine.Ajax.useMock();

            var response = {
                success: {
                    status: 200,
                    responseText: '{"status":"success","behavior":"assign"}'
                }
            };

            $('#selIncludeGbbScoreTrigger').prop('checked', true);
            $('#btnAssign').trigger('click');

            var request = mostRecentAjaxRequest();
            request.response(response.success);

            expect(request.data().IncludeGbbScoreTrigger == 2).toEqual(true);
        });
    });

    describe('ContentAssigned', function () {
        it("can generate warning message when assigning with no points (without submissions)", function () {
            helper.GetAssignTabViewFixture();

            spyOn(window, 'confirm').andCallFake(function (msg) {
                expect(msg).toEqual("You have chosen to assign a due date without putting this item in the gradebook. Are you sure this is what you want to do? To have the item appear in the gradebook, enter gradebook points and choose a gradebook category.");
            });

            ContentWidget.ContentAssigned('assign', 1, null, $('.assigntab'), null, false);
        });

        it("can generate warning message when assigning with no points (with submissions)", function () {
            helper.GetAssignTabViewFixture();

            $(".faceplate-student-completion-stats").html('1');

            spyOn(window, 'confirm').andCallFake(function (msg) {
                expect(msg).toEqual("Setting the points to zero removes the gradebook entry for this item. Existing gradebook contains a student grade.");
            });

            ContentWidget.ContentAssigned('assign', 2, null, $('.assigntab'), null, false);
        });
        it("enables/disable Group Picker dropdown when an item is assigned/unassigned", function () {
            helper.GetAssignTabViewFixture();
            spyOn(window, 'confirm').andCallFake(function (msg) {
                return true;
            });
            expect($("#ddlSettingsList").prop("disabled")).toBe(true);

            jasmine.Ajax.useMock();
            var response = {
                success: {
                    status: 200,
                    responseText: '{"status":"success","behavior":"assign"}'
                }
            };

            ContentWidget.ContentAssigned('assign', 2, null, $('.assigntab'), null, false);

            var request = mostRecentAjaxRequest();
            request.response(response.success);

            expect($("#ddlSettingsList").prop("disabled")).toBe(false);
            //test unassign (calls InitAssign)
            $.fn.DatePicker = jasmine.createSpy('DatePicker');//required for InitAssign
            window.tinyMCE = jasmine.createSpyObj('tinyMCE', ['activeEditor']); //required for InitAssign

            response = {
                success: {
                    status: 200,
                    responseText: '{"status":"success","behavior":"unassign"}'
                }
            };

            ContentWidget.ContentAssigned('unassign', 2, null, $('.assigntab'), null, false);

            var request = mostRecentAjaxRequest();
            request.response(response.success);

            expect($("#ddlSettingsList").prop("disabled")).toBe(true);

        });
    });

    describe('InitAssign', function () {
        it("should not do post to PxPage.Routes.get_submission_status_management_card when parameter getSubmissionStatus is false", function () {
            PxPage.Routes.get_submission_status_management_card = 'getSubmissionStatusUrl';
            var calledCount = 0;
            spyOn($, "post").andCallFake(function (uri) {
                if (uri == 'getSubmissionStatusUrl')
                    calledCount++;
            });
            $.fn.DatePicker = jasmine.createSpy('DatePicker');
            window.tinyMCE = jasmine.createSpyObj('tinyMCE', ['activeEditor']);
            ContentWidget.InitAssign('someAssignmenetClass', false, false);
            expect(calledCount).toEqual(0);
        });

        it("should do post to PxPage.Routes.get_submission_status_management_card when parameter getSubmissionStatus is true", function () {
            PxPage.Routes.get_submission_status_management_card = 'getSubmissionStatusUrl';
            var calledCount = 0;
            spyOn($, "post").andCallFake(function (uri) {
                if (uri == 'getSubmissionStatusUrl')
                    calledCount++;
            });
            $.fn.DatePicker = jasmine.createSpy('DatePicker');
            window.tinyMCE = jasmine.createSpyObj('tinyMCE', ['activeEditor']);
            ContentWidget.InitAssign('someAssignmenetClass', false, true);
            expect(calledCount).toEqual(1);
        });

        it("should do post to PxPage.Routes.get_submission_status_management_card when parameter getSubmissionStatus is null", function () {
            PxPage.Routes.get_submission_status_management_card = 'getSubmissionStatusUrl';
            var calledCount = 0;
            spyOn($, "post").andCallFake(function (uri) {
                if (uri == 'getSubmissionStatusUrl')
                    calledCount++;
            });
            $.fn.DatePicker = jasmine.createSpy('DatePicker');
            window.tinyMCE = jasmine.createSpyObj('tinyMCE', ['activeEditor']);
            ContentWidget.InitAssign('someAssignmenetClass', false, null);
            expect(calledCount).toEqual(1);
        });

        it("should do post to PxPage.Routes.get_submission_status_management_card when parameter getSubmissionStatus is undefined", function () {
            PxPage.Routes.get_submission_status_management_card = 'getSubmissionStatusUrl';
            var calledCount = 0;
            spyOn($, "post").andCallFake(function (uri) {
                if (uri == 'getSubmissionStatusUrl')
                    calledCount++;
            });
            $.fn.DatePicker = jasmine.createSpy('DatePicker');
            window.tinyMCE = jasmine.createSpyObj('tinyMCE', ['activeEditor']);
            ContentWidget.InitAssign('someAssignmenetClass', false);
            expect(calledCount).toEqual(1);
        });

        it("should retrieve assignments for the assigned date", function () {
            helper.GetCreateAssignmentTabViewFixture();
            $.fn.DatePicker = jasmine.createSpy('DatePicker');
            $.fn.DatePickerSetDate = jasmine.createSpy('DatePickerSetDate');
            window.tinyMCE = jasmine.createSpyObj('tinyMCE', ['activeEditor']);
            ContentWidget.UpdateAssignedItems = jasmine.createSpy("ContentWidget.UpdateAssignedItems");

            ContentWidget.InitAssign('contentcreate', false);

            expect(ContentWidget.UpdateAssignedItems).wasCalled();
        });
    });

    describe("AssignmentDateSelected", function () {
        it("should retrieve assignments for the selected date", function () {
            helper.GetCreateAssignmentTabViewFixture();
            $.fn.DatePicker = jasmine.createSpy('DatePicker');
            $.fn.DatePickerSetDate = jasmine.createSpy('DatePickerSetDate');
            window.tinyMCE = jasmine.createSpyObj('tinyMCE', ['activeEditor']);
            ContentWidget.UpdateAssignedItems = jasmine.createSpy("ContentWidget.UpdateAssignedItems");
            ContentWidget.IsFormChanged = jasmine.createSpy("ContentWidget.IsFormChanged");

            ContentWidget.AssignmentDateSelected('01/01/2014', null);

            expect(ContentWidget.UpdateAssignedItems).wasCalled();
        });
    });

    describe('ContentCreated', function () {
        var itemid = 'itemid',
            toc = 'toc',
            name = 'name',
            oldname = 'old',
            contentType = 'contentType';
        var testfixture = '<div id="nonmodal-content">' +
            '<div id="content-item">' +
            '<input type="hidden" class="toc" value="' + toc + '">' +
            '<input type="hidden" class="item-id" value="' + itemid + '">' +
            '<input type="hidden" class="content-item-title" value="' + oldname + '">' +
            '<input type="hidden" id="Content_Type" value="' + contentType + '">' +
            '</div>' +
            '</div>';
        it('won\'t trigger contentcreated on switchboard if response is cancel', function () {
            var result = true;
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function () {
                result = false;
            });
            ContentWidget.ContentCreated('cancel');
            expect(result).toBe(true);
        });

        it('won\'t trigger contentcreated on switchboard if response itemid doesn\'t exist in dom', function () {
            var result = true;
            setFixtures(testfixture);
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function () {
                result = false;
            });
            ContentWidget.ContentCreated(testfixture.replace('itemid', 'wrongitemid'));
            expect(result).toBe(true);
        });

        it('will trigger contentcreated on switchboard if a response exists', function () {
            var result = false;
            setFixtures(testfixture);
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function () {
                result = true;
            });
            ContentWidget.ContentCreated(testfixture);
            expect(result).toBe(true);
        });

        it('will remove fne window if class "fne-window" exists', function () {
            testfixture += "<div class='product-type-blablabla'><div id='fne-window' class='fne-edit require-confirm'></div></div>";
            setFixtures(testfixture);
            spyOn(ContentWidget, 'ContentLoaded');
            spyOn(ContentWidget, 'NavigateAway');
            spyOn(PxContentTemplates, 'ShowMoreDialog');
            var message = "";
            var testMessage = "hoorayy";
            window.PxPage.RestoreDocForm = function () {
            };
            window.PxPage.Toasts = {
                Success: function () {
                    message = testMessage;
                }
            };
            var result = false;
            $(PxPage.switchboard).bind('contentcreated', function () {
                result = true;
            });
            ContentWidget.ContentCreated(testfixture);
            expect($("#fne-window").attr("class")).toBe("fne-edit");
            expect(message).toBe(testMessage);
        });

        it('will set itemid properly when firing contentcreated', function () {
            var result = '';
            setFixtures(testfixture);
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                result = item.id;
            });
            ContentWidget.ContentCreated(testfixture);
            expect(result).toBe(itemid);
        });

        it('will set name properly when firing contentcreated', function () {
            var result = '';
            setFixtures(testfixture);
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                result = item.name;
            });
            ContentWidget.ContentCreated(testfixture.replace(oldname, name));
            expect(result).toBe(name);
        });

        it('will set toc properly when firing contentcreated', function () {
            var result = '';
            setFixtures(testfixture);
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                result = item.toc;
            });
            ContentWidget.ContentCreated(testfixture);
            expect(result).toBe(toc);
        });

        it('will set toc to syllabusfilter if not defined when firing contentcreated', function () {
            var result = '';
            setFixtures(testfixture.replace('toc', 'nottoc'));
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                result = item.toc;
            });
            ContentWidget.ContentCreated(testfixture);
            expect(result).toBe('syllabusfilter');
        });

        it('will set contenttype properly when firing contentcreated', function () {
            var result = '';
            setFixtures(testfixture);
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                result = item.contentType;
            });
            ContentWidget.ContentCreated(testfixture);
            expect(result).toBe(contentType);
        });

        it('will select the proper content-item div tag', function () {
            var result = '';
            setFixtures(testfixture + '<div id="content-item"><input type="hidden" class="toc" value="othertoc"></div>');
            spyOn(ContentWidget, 'ContentLoaded');
            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                result = item.toc;
            });
            ContentWidget.ContentCreated(testfixture);
            expect(result).toBe(toc);
        });

        it("will set the unitType to 'AssignmentUnit' if a new ASSIGNMENT UNIT is created", function () {
            var unitType = "";
            var templateId = "this-is-a-template-id";
            var itemId = "this-is-an-item-id";

            window.tinyMCE = function () {
            };
            $.fn.PxNonModal = function (data) {
            };

            spyOn(PxContentTemplates, "CreateItemFromTemplate").andCallFake(function (templateId, callback, parentId, title) {
                if (typeof callback === "function") {
                    callback({ id: itemId, title: "Unit" });
                }
            });
            spyOn($, "get").andCallFake(function (url, data, callback) {
                if (typeof callback === "function") {
                    setFixtures(testfixture);
                    callback(testfixture);
                }
            });
            spyOn($.fn, "PxNonModal").andCallFake(function (data) {
                if (typeof data.onCompleted === "function") {
                    data.onCompleted();
                }
            });
            spyOn(ContentWidget, 'ContentLoaded');

            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                unitType = item.unitType;
            });

            ContentWidget.CreateAssignmentUnit(templateId, toc);
            ContentWidget.ContentCreated($("#jasmine-fixtures").html());

            expect(unitType).toBe("AssignmentUnit");
        });

        it("will NOT set the unitType to 'AssignmentUnit' if a new UNIT is created", function () {
            var unitType = "";
            setFixtures(testfixture);

            spyOn(ContentWidget, 'ContentLoaded');

            $(PxPage.switchboard).bind('contentcreated', function (event, item) {
                unitType = item.unitType;
            });

            ContentWidget.ContentCreated(testfixture);

            expect(unitType).toBeUndefined();
        });
    });

    describe('ValidateConfirmation', function () {
        it('can execute callback if validation fails', function () {
            var callback = jasmine.createSpy("callback");
            setFixtures('<div id="dialog-confirm" /><button class="savebtn" onclick=false />');
            ContentWidget.ValidateConfirmation(callback, '');

            $('button.ui-state-focus').trigger('click');

            expect(callback.wasCalled).toBeFalsy();
        });

        it('can execute callback if validation passes', function () {
            var callback = jasmine.createSpy("callback");
            setFixtures('<div id="dialog-confirm" /><button class="savebtn" onclick=true />');
            ContentWidget.ValidateConfirmation(callback, '');

            $('button.ui-state-focus').trigger('click');

            expect(callback.wasCalled).toBeTruthy();
        });
    });

    describe('BindRequireConfirmControls', function () {
        it('can add required flag to FNE window if element changed (keypress)', function () {
            setFixtures('<div id="fne-window"><input type="text" id="test" /></div>');
            ContentWidget.BindRequireConfirmControls();

            $('#test').trigger('keypress');

            expect($('#fne-window.require-confirm').length > 0).toBeTruthy();
        });

        it('can add required flag to FNE window if element changed (keyup)', function () {
            setFixtures('<div id="fne-window"><select id="test" /></div>');
            ContentWidget.BindRequireConfirmControls();

            $('#test').trigger('keyup');

            expect($('#fne-window.require-confirm').length > 0).toBeTruthy();
        });
    });

    var helper = {
        GetCreateAssignmentTabViewFixture: function () {
            fixture = helper.GetAssignmentTabView();
            jasmine.Fixtures.prototype.addToContainer_(fixture);
        },
        GetAssignmentTabView: function () {
            var viewData = {
                accessLevel: {
                    dataType: "System.String",
                    dataValue: "instructor"
                }
            };

            var data = {
                viewPath: "Gradebook",
                viewModel: JSON.stringify({
                    SourceType: "Assessment",
                    IsGradeable: true,
                    IsItemLocked: false,
                    Type: "dropbox",
                    AvailableSubmissionGradeAction: JSON.stringify([]),
                    AssignTabSettings: {
                        ShowIncludeScore: true,
                        ShowCalculationType: false,
                        ShowPointsPossible: false
                    }
                }),
                viewData: JSON.stringify(viewData),
                viewModelType: "Bfw.PX.PXPub.Models.AssignedItem"
            };

            var view = '<div class="FacePlateAsssign contentwrapper"><div class="contentcreate"><input type="textbox" id="settingsAssignDueDate" value="08/31/2013" /><input type="textbox" id="DueDate" value="08/31/2013" /><input type="textbox" id="Score_Possible" value="10" />';
            view += PxViewRender.RenderView('PXPub', 'ContentWidget', data);
            view += '</div><input type="button" id="btnAssign" value="Assign" class="assign btnAssign submit-action" style="display: inline"></div>';

            return view;
        },
        GetAssignTabViewFixture: function () {
            fixture = helper.GetAssignTabView();
            jasmine.Fixtures.prototype.addToContainer_(fixture);
        },
        GetAssignTabView: function () {
            var viewData = {
                accessLevel: {
                    dataType: "System.String",
                    dataValue: "instructor"
                }
            };

            var data = {
                viewPath: "AssignTab",
                viewModel: JSON.stringify({
                    Content: {
                        Title: "Bogus Title"
                    },
                    AssignedItem: {
                        SourceType: "",
                        IsContentCreateAssign: false,
                        AssignTabSettings: {
                            ShowAssignedSameDay: true
                        },
                        Score: {
                            Possible: 10
                        }
                    }
                }),
                viewModelType: "Bfw.PX.PXPub.Models.ContentView",
                viewData: JSON.stringify(viewData)
            };

            var view = PxViewRender.RenderView('PXPub', 'ContentWidget', data);

            return view;
        }
    };
});
