///    <reference path="../../lib/jasmine-1.3.1/jasmine.js"/>
///    <reference path="../../lib/jasmine-1.3.1/jasmine-beforeAll.js"/>
///    <reference path="../../lib/jasmine-1.3.1/jasmine-html.js"/>

///    <!-- include dependencies here (e.g. jquery) !-->
///    <reference path="../../lib/jquery/jquery.js"/>
///    <reference path="../../lib/jquery/jquery-migrate.js"/>

///    <!-- include vendor files -->
///    <reference path="../../lib/jasmine-jquery.js"/>

///    <!-- include source files here !-->
///    <reference path="../../../Scripts/jquery/jquery.utils.js"/>
///    <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js"/>
///    <reference path="../../../Scripts/jquery/jquery.blockUI.js"/>
///    <reference path="../../../Scripts/jquery/jquery.validate.min.js"/>
///    <reference path="../../../Scripts/jquery/jquery.qtip.min.js"/>

///    <reference path="../../../Scripts/Common/PxCommon.js"/>
///    <reference path="../../../Scripts/Common/PxRoutes.js"/>
///    <reference path="../../../Scripts/Common/dateFormat.js"/>
///    <reference path="../../../Scripts/Signals/signals.js"/>
///    <reference path="../../../Scripts/Hasher/hasher.js"/>
///    <reference path="../../../Scripts/Common/HashHistory.js"/>
///    <reference path="../../../Scripts/Crossroads/crossroads.js"/>

///    <reference path="../../../Scripts/jquery/jquery.fauxtree.js"/>
///    <reference path="../../../Scripts/jquery/jquery.sortElements.js"/>
///    <reference path="../../../Scripts/jquery/jquery.sticky.js"/>

///    <reference path="../../../Scripts/ContentTreeWidget/ContentTreeWidget.js"/>
///    <reference path="../../../Scripts/ContentWidget/ContentWidget.js"/>
///    <reference path="../../../Scripts/ManagementCard/ManagementCard.js"/>
///    <reference path="../../../Scripts/Xbook/XbookV2.js"/>

describe("XbookV2.ManagementCard:", function () {
    ContentWidget = {
        CreateNewGradebookCategory: function () {
        },
        AddGradebookCategoryToUnit: function () {
        }
    };
    PxPage = {
        Routes: {
            launchpad_item_operation: 'launchpad_item_operation'
        },
        Toasts: {
            AppName: function () {
                return "xbook";
            },
            Success: function (message) {
            },
            Error: function (message) {
            }
        },
        log: function (args) {
        }
    }
    window._ = PxXbookV2.PrivateTest();
    
    var widgetId = "PX_LAUNCHPAD_ASSIGNED_WIDGET";
    var htmlXbookDoc = "";
    var contentItemId = "item_bla_bla_chapterx";
    
    beforeEach(function() {
        // Arrange
        var arrangedData = $.fn.GetArrangedManagementCard();
        
        var dom = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrangedData);
        htmlXbookDoc = $.format("<div id='main'><div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                            {1} \
                           </div></div>", widgetId, dom);

        setFixtures(htmlXbookDoc);
        
        $('#main').addClass('faux-tree-node managementcard'); // classes needed to run _managementCardShow

        // Act
        PxXbookV2.managementCard.init();
    });

    describe("DONE button on Management Card is clicked", function () {
        it("xbook will handle the click", function () {
            $(PxXbookV2.managementCard.ui.vent).trigger('managementcard.show', contentItemId);
            spyOn(PxXbookV2.managementCard.fn, "doneClick");
        
            $(PxXbookV2.managementCard.ui.done).click();

            var node = $(PxXbookV2.managementCard.ui.done);
            
            expect(node).not.toBeNull();
            expect(PxXbookV2.managementCard.fn.doneClick).toHaveBeenCalledWith(node[0], contentItemId);
            expect(PxXbookV2.managementCard.fn.doneClick.mostRecentCall.args[0] instanceof HTMLInputElement).toBeTruthy();
        });
        it('will verify unit information before assigning', function() {
            spyOn(PxManagementCard, 'VerifySelectedUnit').andCallFake(function() {
                return true;
            });
            spyOn(PxManagementCard, 'GetSelectedUnit');
            spyOn(PxManagementCard, 'OnAssign');
            $(PxXbookV2.managementCard.ui.vent).trigger('managementcard.show', contentItemId);
            $(PxXbookV2.managementCard.ui.done).click();

            expect(PxManagementCard.VerifySelectedUnit).toHaveBeenCalled();
            expect(PxManagementCard.OnAssign).toHaveBeenCalled();
        });
        it('won\'t assign if validation fails', function() {
            spyOn(PxManagementCard, 'VerifySelectedUnit').andCallFake(function () {
                return false;
            });
            spyOn(PxManagementCard, 'GetSelectedUnit');
            spyOn(PxManagementCard, 'OnAssign');
            
            expect(PxManagementCard.GetSelectedUnit).not.toHaveBeenCalled();
            expect(PxManagementCard.OnAssign).not.toHaveBeenCalled();
        })
    });
        
    describe('saving new assignment unit', function () {
        var item = {
            itemId: 'itemId',
            targetId: 'parentId',
            operation: 'NewItem',
            toc: 'assignmentfilter'
        };
        it('will populate the managementcard dropdown if on xbook tab', function () {
            _._state = 'xbook';
            var categoryId = 'categoryId';
            spyOn($, 'ajax').andCallFake(function (data) {
                data.success();
            });
            spyOn(ContentWidget, 'CreateNewGradebookCategory').andCallFake(function (item, callback) {
                callback(categoryId, 'unitid');
            });
            spyOn(ContentWidget, 'AddGradebookCategoryToUnit').andCallFake(function (categoryId, unitId, callback) {
                callback();
            });
            spyOn(PxManagementCard, 'PopulateAssignmentUnits');
            _.saveNewAssignmentUnit(item);

            expect(PxManagementCard.PopulateAssignmentUnits).toHaveBeenCalled();
            expect(item.categoryId).toBe(categoryId);
        });
        it('won\'t populate the managementcard dropdown if not on xbook tab', function () {
            _._state = 'assignments';
            spyOn($, 'ajax').andCallFake(function (data) {
                data.success();
            });
            spyOn(ContentWidget, 'CreateNewGradebookCategory').andCallFake(function (item, callback) {
                callback('categoryid', 'unitid');
            });
            spyOn(ContentWidget, 'AddGradebookCategoryToUnit').andCallFake(function (categoryId, unitId, callback) {
                callback();
            });
            spyOn(PxManagementCard, 'PopulateAssignmentUnits');
            _.saveNewAssignmentUnit(item);

            expect(PxManagementCard.PopulateAssignmentUnits).not.toHaveBeenCalled();
        });
    });

    describe('When a management card is opened', function () {

        beforeEach(function () { // spy and publish event to show card
            spyOn($.fn, 'sticky');
            $(PxXbookV2.managementCard.ui.vent).trigger('managementcard.show', contentItemId);
        });

        it('disable jQuery stickyness', function () {
            expect($.fn.sticky).toHaveBeenCalledWith('toggle', false);
        });

        it('bind ui events to `Done` button', function () {
            var boundClickEvents = $(PxXbookV2.managementCard.ui.done).data('events').click;
            expect(typeof boundClickEvents).toEqual('object');
        });

    });

    describe('When a management card is closed', function () {

        beforeEach(function () { // spy and publish event to show card
            spyOn($.fn, 'sticky');
            $(PxXbookV2.managementCard.ui.vent).trigger('managementcard.hide', contentItemId);
        });

        it('enable jQuery stickyness', function () {
            expect($.fn.sticky).toHaveBeenCalledWith('toggle', true);
        });

    });
});

(function ($) {
    $.fn.GetArrangedManagementCard = function () {
        //#region Arrange

        var viewPath = "ManagementCard";

        var viewModel = JSON.stringify({
            Id: "item_bla_bla_11223",
            AssignTabSettings: {
                ShowMakeGradable: false,
                ShowGradebookCategory: false,
                ShowIncludeScore: false,
                ShowCalculationType: false,
                ShowSyllabusCategory: false
            },
            GradeBookWeights: {
                GradeWeightCategories: null
            },
            ParentDisplayText: "",
            Score: {
                Correct: 0,
                Possible: 0
            },
            ShowAssignmentUnitWorkflow: true,
            SourceType: "",
            Type: ""
        });

        var viewModelType = "Bfw.PX.PXPub.Models.AssignedItem";

        var viewData = {
            ShowAssignmentUnitWorkflow: {
                dataType: "System.Boolean",
                dataValue: true
            },
            isRange: {
                dataType: "System.Boolean",
                dataValue: false
            },
            HasSubmissions: {
                dataType: "System.Boolean",
                dataValue: false
            },
            HiddenFromStudents: {
                dataType: "System.Boolean",
                dataValue: true
            }
        };

        var arrangedData = {
            viewPath: viewPath,
            viewModel: viewModel,
            viewModelType: viewModelType,
            viewData: viewData
        };

        return arrangedData;
        
        //#endregion
    };
}(jQuery));