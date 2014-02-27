/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" data-cover-adapter="../../lib/blanket/jasmine-blanket.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Scripts/jquery/jquery.fauxtree.js"/>
/// <reference path="../../../Scripts/Datepicker/datepicker.js"/>
/// <reference path="../../../Scripts/ContentWidget/ContentWidget.js"/>
/// <reference path="../../../Scripts/Other/date.js"/>
/// <reference path="../../../Scripts/Common/dateFormat.js"/>
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

/// <reference path="../../../Scripts/ManagementCard/ManagementCard.js" />

describe("ManagementCard", function () {
    PxPage = {
        log: function() {
        },
        Toasts: {
            Error: function() {
            }
        },
        CloseNonModal: function() {
        },
        Routes: {
            get_submission_status_management_card: ''
        },
        switchboard: $(document)
    };
    ContentWidget = {
        AssignmentDateSelected: function() {
        },
        InitAssign: function() {
        },
        IsAssignDateValid: function() {
        },
        ContentAssignedAssignmentCenter: function(){}
    };
    
    beforeEach(function () {
        PxPage.switchboard = $(document);
        PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
        PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
        PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
        
        $.fn.placeholder = function () { };
        $.fn.ptTimeSelect = function () { };
    });

    afterEach(function () {
        $('body').children('div').not('#HTMLReporter').remove();
    });

    describe("warning message", function() {
        beforeEach(function() {
            window.HelperManagementCard.SetManagementCardViewFixture();
        });
        
        it("can generate warning message when assigning with no points (without submissions)", function () {
            $('.faux-tree-node .faceplate-student-completion-stats').html(0);

            spyOn($.fn, "fauxtree").andCallFake(function (arg) {
                return $("<li data-ft-id='1' id='1' data-ud-itemtype='dropbox'></li>");
            });

            spyOn(window, 'confirm').andCallFake(function (msg) {
                expect(msg).toEqual("You have chosen to assign a due date without putting this item in the gradebook. Are you sure this is what you want to do? To have the item appear in the gradebook, enter gradebook points and choose a gradebook category.");
            });

            PxManagementCard.assign(1);
        });

        it("can generate warning message when assigning with no points (with submissions)", function () {
            $('.faux-tree-node .faceplate-student-completion-stats').html(1);

            spyOn($.fn, "fauxtree").andCallFake(function (arg) {
                return $("<li data-ft-id='1' id='1' data-ud-itemtype='dropbox'></li>");
            });

            spyOn(window, 'confirm').andCallFake(function (msg) {
                expect(msg).toEqual("Setting the points to zero removes the gradebook entry for this item. Existing gradebook contains a student grade.");
            });

            PxManagementCard.assign(1);
        });

        it("can generate error message if points provided are greater than 100", function () {
            $('#txtGradePoints').val(101);

            spyOn($.fn, "fauxtree").andCallFake(function (arg) {
                return $("<li data-ft-id='1' id='1' data-ud-itemtype='dropbox'></li>");
            });

            PxPage.Toasts.Error.isSpy = false;

            spyOn(PxPage.Toasts, "Error").andCallFake(function (msg) {
                expect(msg).toEqual("Points should be between 0 and 100");
            });

            PxManagementCard.assign(1);
        });
    });
    
    it("should not do post to PxPage.Routes.get_submission_status_management_card more than once", function () {
        window.temp = window.ContentWidget;
        PxPage.Routes.get_submission_status_management_card = 'getSubmissionStatusUrl';
        var calledCount = 0;
        spyOn($, "post").andCallFake(function (uri) {
            if (uri == 'getSubmissionStatusUrl')
                calledCount++;
        });
        window.ContentWidget = jasmine.createSpyObj('ContentWidget', ['InitAssign']);
        PxManagementCard.Init('someContentItemId', 'someAssignmentClassName', 'False');
        expect(calledCount).toEqual(1);
        window.ContentWidget = window.temp;
        window.temp = null;
    });
    
    it('sets the date from a calendar day click', function () {
        // fakers
        window.tinyMCE = {};

        $('<div id="fne-window" style="">').appendTo('body'); // needed for contentwidget.js

        window.HelperManagementCard.SetManagementCardViewFixture();

        spyOn(ContentWidget, 'AssignmentDateSelected').andCallThrough();

        // render calendar
        var cal = $('#assignment-calendar').DatePicker({
            flat: true,
            date: '',
            calendars: 1,
            mode: 'range',
            starts: 0,
            onChange: ContentWidget.AssignmentDateSelected
        });

        // check for rendered calendar
        expect(cal.find('.datepickerContainer').length).not.toBe(0);

        var dayInput = $('#facePlateAssignDueDate').val('');

        // locate a day and click it
        cal.find('.datepickerDays').find('td').not('.datepickerNotInMonth').eq(0).find('a').click();

        expect(ContentWidget.AssignmentDateSelected).toHaveBeenCalled();
    });

    //#region Assignment Units

    describe("when a management card has Assignment Units section", function () {
        var id = 'id';
        describe('with a valid selected assignment unit', function() {
            var assignedUnitId = 'assignedUnitId',
                assignedCatId = 'categoryId';

            beforeEach(function() {
                var viewData = {
                    ShowAssignmentUnitWorkflow: {
                        dataType: "System.Boolean",
                        dataValue: true
                    },
                    isRange: {
                        dataType: "System.Boolean",
                        dataValue: false
                    },
                    HiddenFromStudents: {
                        dataType: "System.Boolean",
                        dataValue: false
                    }
                };

                var data = {
                    viewPath: "ManagementCard",
                    viewModel: JSON.stringify({
                        Id: id,
                        DueDate: new Date(),
                        SourceType: "",
                        AssignTabSettings: {
                            ShowMakeGradeable: false,
                            ShowGradebookCategory: true
                        },
                        AvailableSubmissionGradeAction: {                            
                            
                        },
                        Score: {
                            Possible: 0
                        },
                        AssignmentUnits: [{
                            Id: 'id1',
                            CategoryId: 'cat1',
                            Selected: false
                        }, {
                            Id: assignedUnitId,
                            CategoryId: assignedCatId,
                            Selected: true
                        }],
                        GradeBookWeights: {
                            GradeWeightCategories: null
                        }
                    }),
                    viewModelType: "Bfw.PX.PXPub.Models.AssignedItem",
                    viewData: JSON.stringify(viewData)
                };

                var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', data);
                view += "<div data-ft-id='1' class='faux-tree-node'><div class='faceplate-student-completion-stats'></div></div>";

                setFixtures(view);
            });

            it("can get the selected assignment unit", function() {
                //act
                var elem = $(".assign-showCalendar-close");
                var unit = PxManagementCard.GetSelectedUnit(elem);

                // assert
                expect(unit).not.toBeNull();
                expect(unit.unitId).toBe(assignedUnitId);
                expect(unit.categoryId).toBe(assignedCatId);
            });

            it('can verify a valid unit is selected', function() {
                var unitContainer = $(".faceplate-assign-dropdown-container");
                var isValid = PxManagementCard.VerifySelectedUnit(unitContainer);

                // NOT to show required messages
                expect(isValid).toBeTruthy();
                expect(unitContainer.attr("class")).not.toContain("required");
                messages = unitContainer.find(".required");
                expect(messages.length).toBe(0);
            });
            
            it("can add a newly created item into the dropdown list", function () {
                spyOn(window.PxPage, "CloseNonModal");

                var itemId = "1111";
                var itemName = "xxxx";
                var categoryId = "10";
                var item = {
                    id: itemId,
                    name: itemName,
                    contentType: "pxunit",
                    categoryId: categoryId
                };

                PxManagementCard.PopulateAssignmentUnits(item);

                var droplist = $("#selassignmentunits");
                var newlyAddedOption = $("#selassignmentunits option").eq(1);
                expect(droplist.length).toBeGreaterThan(0);
                expect(droplist.val()).toEqual(itemId);
                expect(newlyAddedOption.data("category")).toEqual(parseInt(categoryId));
                expect(window.PxPage.CloseNonModal).toHaveBeenCalled();
            });
        });
        describe('without a valid assignment unit', function () {
            beforeEach(function () {
                var viewData = {
                    ShowAssignmentUnitWorkflow: {
                        dataType: "System.Boolean",
                        dataValue: true
                    },
                    isRange: {
                        dataType: "System.Boolean",
                        dataValue: false
                    },
                    HiddenFromStudents: {
                        dataType: "System.Boolean",
                        dataValue: false
                    }
                };

                var data = {
                    viewPath: "ManagementCard",
                    viewModel: JSON.stringify({
                        Id: id,
                        DueDate: new Date(),
                        SourceType: "",
                        AssignTabSettings: {
                            ShowMakeGradeable: false,
                            ShowGradebookCategory: true
                        },
                        AvailableSubmissionGradeAction: {

                        },
                        Score: {
                            Possible: 0
                        },
                        AssignmentUnits: [{
                            Id: 'id1',
                            CategoryId: 'cat1',
                            Selected: false
                        }, {
                            Id: 'id2',
                            CategoryId: 'cat2',
                            Selected: false
                        }],
                        GradeBookWeights: {
                            GradeWeightCategories: null
                        }
                    }),
                    viewModelType: "Bfw.PX.PXPub.Models.AssignedItem",
                    viewData: JSON.stringify(viewData)
                };

                var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', data);
                view += "<div data-ft-id='1' class='faux-tree-node'><div class='faceplate-student-completion-stats'></div></div>";

                setFixtures(view);
            });
            
            it("can verify the invalid selection", function() {
                var unitContainer = $(".faceplate-assign-dropdown-container");
                var isValid = PxManagementCard.VerifySelectedUnit(unitContainer);

                // show required messages
                expect(isValid).toBeFalsy();
                expect(unitContainer.attr("class")).toContain("required");
                var messages = unitContainer.find(".required");
                expect(messages.length).toBeGreaterThan(0);
            });
            
            it('will return null for a selected unit', function () {
                var doneButton = $('.assign-showCalendar-close');
                var unit = PxManagementCard.GetSelectedUnit(doneButton);
                expect(unit).toBeNull();
            });
        });
    });

    describe("when a management card has no Assigment unit section", function() {
        beforeEach(function() {
            var viewData = {
                ShowAssignmentUnitWorkflow: {
                    dataType: "System.Boolean",
                    dataValue: false
                },
                isRange: {
                    dataType: "System.Boolean",
                    dataValue: false
                },
                HiddenFromStudents: {
                    dataType: "System.Boolean",
                    dataValue: false
                }
            };

            var data = {
                viewPath: "ManagementCard",
                viewModel: JSON.stringify({
                    Id: 'id',
                    DueDate: new Date(),
                    SourceType: "",
                    AssignTabSettings: {
                        ShowMakeGradeable: false,
                        ShowGradebookCategory: true
                    },
                    AvailableSubmissionGradeAction: {
                        
                    },
                    Score: {
                        Possible: 0
                    },
                    GradeBookWeights: {
                        GradeWeightCategories: null
                    }
                }),
                viewModelType: "Bfw.PX.PXPub.Models.AssignedItem",
                viewData: JSON.stringify(viewData)
            };

            var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', data);
            view += "<div data-ft-id='1' class='faux-tree-node'><div class='faceplate-student-completion-stats'></div></div>";

            setFixtures(view);
        });

        it('verifing the selected unit returns true', function() {
            var doneButton = $('.collapse-menu.assign-showCalendar-close')
            var isValid = PxManagementCard.VerifySelectedUnit(doneButton);

            expect(isValid).toBeTruthy();
        });
    });

    describe("when an 'assign' button is clicked", function () {
        var id = 'id',
            dueDate = new Date(),
            assignedUnitId = 'assignedUnitId',
            categoryId = 'categoryId';
        
        beforeEach(function() {
            dueDate.setDate(dueDate.getDate() + 13);
            var viewData = {
                ShowAssignmentUnitWorkflow: {
                    dataType: "System.Boolean",
                    dataValue: true
                },
                isRange: {
                    dataType: "System.Boolean",
                    dataValue: false
                },
                HiddenFromStudents: {
                    dataType: "System.Boolean",
                    dataValue: false
                }
            };

            var data = {
                viewPath: "ManagementCard",
                viewModel: JSON.stringify({
                    Id: id,
                    DueDate: dueDate,
                    SourceType: "",
                    AssignTabSettings: {
                        ShowMakeGradeable: false,
                        ShowGradebookCategory: true
                    },
                    AvailableSubmissionGradeAction: {

                    },
                    Score: {
                        Possible: 0
                    },
                    GradeBookWeights: {
                        GradeWeightCategories: null
                    },
                    AssignedUnitId: assignedUnitId,
                    Category: categoryId
                }),
                viewModelType: "Bfw.PX.PXPub.Models.AssignedItem",
                viewData: JSON.stringify(viewData)
            };

            var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', data);
            view += "<div data-ft-id='1' class='faux-tree-node'><div class='faceplate-student-completion-stats'></div></div>";

            setFixtures(view);
        });

        it("can un-assign an item if no due dates are selected for already-assigned item", function () {
            spyOn(ContentWidget, "InitAssign");
            spyOn(PxManagementCard, "unAssign");

            $("#facePlateAssignDueDate").val("");
            $("#facePlateAssignTime").val("");

            PxManagementCard.Init(id, "", true);
            $('.collapse-menu.assign-showCalendar-close').click();

            expect(PxManagementCard.unAssign).toHaveBeenCalledWith(id);
        });
        
        it("can pass 'unitId' into 'assign' function", function () {
            /* spies */
            spyOn(PxManagementCard, "assign");

            PxManagementCard.OnAssign(null, true, id, assignedUnitId);

            // assert
            expect(PxManagementCard.assign).toHaveBeenCalledWith(id, assignedUnitId);
        });
    });

    describe("when an 'unassign' button is clicked", function () {
        var options = {};
        
        beforeEach(function() {
            options = {
                id: "this_is_an_item_to_be_assigned",
                dueDate: "",
                showUnits: true
            };
            window.HelperManagementCard.SetManagementCardViewFixture(options);
        });
        
        it("can pop up a dialog to confirm when UNASSIGN button is clicked", function () {
            /* stub */
            window.PxModal = {
                    CenterDialog: function() {
                }
            };
            spyOn(PxModal, "CenterDialog");

            var newNode = document.createElement("LI");
            $(newNode).attr("class", "faux-tree-node");
            $(newNode).attr("data-ud-itemtype", "Px");
            $(newNode).attr("data-ft-id", options.id);
            spyOn($.fn.fauxtreeObj().api, "getNode").andCallFake(function () { }).andReturn($(newNode));

            // act
            PxManagementCard.unAssign(options.id);

            // assert
            expect(PxModal.CenterDialog).toHaveBeenCalled();
        });
    });

    //#endregion Assignment Units

    describe("when an item is removed", function() {
        var myItemId = "This is my item id";
        var myTocs = "FILTERA, FILTERB";
        beforeEach(function() {
            window.PxModal = {
                    CenterDialog: function (tag) {
                }
            };
        });
        
        it("can pass messages if an item has submssions", function () {
            spyOn(PxModal, "CenterDialog").andCallFake(function(tag) {
                var msg = tag.find(".remove-message").text();
                expect(msg.indexOf("You can always re-add this activity from 'Removed items'") > -1).toBeTruthy();
            });

            spyOn($, "post").andCallFake(function(url, args, callback) {
                return callback("true");
            });

            PxManagementCard.checkForSubmissionAndRemoveItem(myItemId, myTocs);
        });

        it("can trigger 'onremoveitem' event when remove button is clicked", function () {
            setFixtures('<div id="px-dialog"" class="px-dialog"> \
                            <input type="button" class="remove" value="remove"/> \
                        </div>');
            
            spyOn(PxModal, "CenterDialog");

            spyOn($, "post").andCallFake(function (url, args, callback) {
                return callback("true");
            });

            spyOnEvent("#px-dialog input.remove", "click");

            $(PxPage.switchboard).bind("onremoveitem", function(e, item) {
                expect(item.contentItemId).toBe(myItemId);
                expect(item.removeFrom).toBe(myTocs);
            });
            
            PxManagementCard.checkForSubmissionAndRemoveItem(myItemId, myTocs);

            $("#px-dialog input.remove").click();

            expect("click").toHaveBeenTriggeredOn($("#px-dialog input.remove"));
        });
    });
    
});