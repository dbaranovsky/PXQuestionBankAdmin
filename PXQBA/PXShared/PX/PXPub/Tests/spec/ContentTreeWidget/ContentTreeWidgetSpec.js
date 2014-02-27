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
///    <reference path="../../../Scripts/jquery/modernizr.custom.js"/>

///    <reference path="../../../Scripts/jquery/jquery.fauxtree.js"/>
///    <reference path="../../../Scripts/jquery/jquery.sortElements.js"/>

///    <reference path="../../../Scripts/ContentTreeWidget/ContentTreeWidget.js"/>
///    <reference path="../../../Scripts/Xbook/XbookV2.js"/>
///    <reference path="../../../Scripts/ContentWidget/ContentWidget.js"/>
///    <reference path="../../../Scripts/ManagementCard/ManagementCard.js"/>

describe("Content Tree Widget (TOC):", function () {
    var widgetId = "PX_LAUNCHPAD_ASSIGNED_WIDGET";
    var itemParentId = "PX_MULTIPART_LESSONS";

    var htmlXbookDoc = "";
    var htmlLaunchpadDoc = "";

    var testNodeId = "";
    var openedNodeFixture = "";

    var liNode = $.fn.fauxtreeObj()._static.sel.node; // faux-tree-node
    var titleSelector = $.fn.fauxtreeObj()._static.sel.nodeTitle; // faux-tree-node-title
    var iconSelector = $.fn.fauxtreeObj()._static.sel.nodeIcon; // .icon
    
    window.PxPage = {
        CloseNonModal: function () {
        },
        Fade: function() {
        },
        switchboard: $(document),
        Loading: function (args) {
        },
        Loaded: function (args) {
        },
        log: function (args) {
        },
        Routes: {
            resource_type_list: "resource_type_list",
            resource_results: "resource_results",
            resource_removed: "resource_removed",
            resource_mine: "resource_mine",
            resource_facets: "resource_facets",
            resource_facets_chapter: "resource_facets_chapter",
            resource_facets_type: "resource_facets_type"
        },
        TouchEnabled: function () {
            return true;
        },
        UpdateFneSize: function () { },
        isIE: function () {
            return true;
        }
    };

    window.get_cookie = jasmine.createSpy(window, "get_cookie");
    window.get_cookie.andReturn(""); // insert cookie for testing 

    describe('processContentAssignment', function () {
        beforeEach(function() {
            spyOn($.fn.ContentTreeWidgetObj()._static.fn, "GetSettings").andCallFake(function () {
                return { toc: "syllabusfilter" };
            });
            spyOn($.fn.ContentTreeWidgetObj()._static.sel, "getItem").andCallFake(function (id) {
                var item = "<li data-ft-id='" + id + "'></li>";
                return item;
            });
        });
        it('can parse assignment dates with date/time parts', function () {
            var dueDate, dueTime, startDate, startTime;
            spyOn($.fn.ContentTreeWidgetObj()._static.fn, "saveNavigationState").andCallFake(function (data) {
                dueDate = data.changed.udattr("due-date");
                dueTime = data.changed.udattr("due-time");
                startDate = data.changed.udattr("start-date");
                startTime = data.changed.udattr("start-time");
            });
            var argsParam = {
                id: 'itemId',
                date: '02/13/2014 11:59 PM',
                startdate: '02/12/2014 10:59 PM'
            };

            $.fn.ContentTreeWidgetObj()._static.fn.processContentAssignment(argsParam);

            expect(dueDate).toBe('02/13/2014');
            expect(dueTime).toBe('11:59 PM');
            expect(startDate).toBe('02/12/2014');
            expect(startTime).toBe('10:59 PM');
        });

        it('will pass unitId and categoryId into saveNavigationState', function() {
            var unitId = 'unitId',
                categoryId = 'categoryId',
                retval = false;
            spyOn($.fn.ContentTreeWidgetObj()._static.fn, 'saveNavigationState').andCallFake(function(data) {
                if (data.unitContainer && data.unitContainer.unitId === unitId &&
                    data.unitContainer.categoryId === categoryId) {
                    retval = true;
                }
            });
            $.fn.ContentTreeWidgetObj()._static.fn.processContentAssignment({
                unit: {
                    unitId: unitId,
                    categoryId: categoryId
                }
            });
            expect(retval).toBeTruthy();

        });
    });

    describe('when an item is created', function() {
        var toc = 'syllabusfilter';

        beforeEach(function () {
            var arrange = $.fn.GetArrangeLaunchpadSet1();

            var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
            var htmleveryoneDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                                            {1} \
                                            </div>", widgetId, domToc);

            setFixtures(htmleveryoneDoc);

            $("#launchpad-widget-" + widgetId).ContentTreeWidget({
                readOnly: true,
                showManagementCard: true,
                showExpandIcon: true,
                showExpandIconLevel: 1,
                togglePastDue: false,
                toggleDueLater: false,
                enableDragAndDrop: false,
                showCollapseUnassigned: false,
                collapseUnassigned: false,
                collapseDueLaterByDefault: false,
                collapsePastDueByDefault: false,
                grayOutPastDueLater: false,
                dueSoonDays: 14,
                sortByDueDate: false,
                courseNumber: 129860,
                splitAssignedUnassigned: true,
                triggerOpenContentOnClick: false,
                toc: toc,
                assignmentToc: "",
                showAssignmentUnitFlow: false
            });
        });

        describe('it', function() {
            it("will add item if any tree exists and toc matches", function() {

                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "onAddExistingContent");

                //TODO: need to update 'id' to be contentItem{id, title}
                $(PxPage.switchboard).trigger("contentcreated", [
                    {
                        id: 'id',
                        name: '',
                        toc: toc,
                        contentType: ''
                    }, 'parentId', 'newItem', null]);

                expect($.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent).toHaveBeenCalled();
            });
            
            it("will add item if toc is undefined", function () {

                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "onAddExistingContent");

                //TODO: need to update 'id' to be contentItem{id, title}
                $(PxPage.switchboard).trigger("contentcreated", [
                    {
                        id: 'id'
                    }, 'parentId', 'newItem', null]);

                expect($.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent).toHaveBeenCalled();
            });

            it("will update item if item already exists in tree", function() {
                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "refreshItem");
                //TODO: need to update 'id' to be contentItem{id, title}
                $(PxPage.switchboard).trigger("contentcreated", [
                    {
                        id: 'MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407',
                        name: 'title',
                        toc: 'syllabusfilter',
                        contentType: 'type'
                    }, 'parentId', 'newItem', null]);

                expect($.fn.ContentTreeWidgetObj()._static.fn.refreshItem).toHaveBeenCalled();
            });

            it("won't add item if toc doesn't match", function() {
                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "onAddExistingContent");
                //TODO: need to update 'id' to be contentItem{id, title}
                $(PxPage.switchboard).trigger("contentcreated", [
                    {
                        id: 'id',
                        name: '',
                        toc: 'nomatch',
                        contentType: ''
                    }, 'parentId', 'newItem', null]);

                expect($.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent).not.toHaveBeenCalled();
            });
            
            it("wont't add/update tree item if no tree exists", function() {
                var result = false;

                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "onAddExistingContent").andCallFake(function () {
                    result = true;
                });

                $('.faux-tree').addClass('faux-tree-removed');
                $('.faux-tree').removeClass('faux-tree');
                //TODO: need to update 'id' to be contentItem{id, title}
                $(PxPage.switchboard).trigger("contentcreated", ['id', 'parentId', 'newItem', null]);
                $('.faux-tree-removed').addClass('faux-tree');
                $('.faux-tree-removed').removeClass('faux-tree-removed');

                expect(result).toBeFalsy();
            });
            
            it("will not have span element with class name \"icon-placeholder\" for levels deeper than 1", function () {
                var nodes = $("li.faux-tree-node[data-ft-level=1][data-ft-state='closed']");

                expect(nodes.length).toBeGreaterThan(0);

                var node = nodes[0];
                var testId = $(node).data("ud-id");

                $(node).ftattr("level", 2);

                /* act */
                $.fn.ContentTreeWidgetObj()._static.fn.scanFauxTree();

                var scannedItem = $("li[data-ud-id='" + testId + "']");

                expect(scannedItem.length).toBeGreaterThan(0);
                expect($(scannedItem).find(".icon-placeholder").length).toBe(0);
            });

            it('will move item to "Assigned" tree when assigned a due date to a item', function () {
                expect($(".faceplate-unit-subitems-unassigned li").length).toBe(2);
                expect($(".faceplate-unit-subitems li").length).toBe(0);

                var fakeResponse = ' <li class="faux-tree-node pxunit-item-row unitrowlevel1 item-type-pxunit hide-in-fne hide-in-fne instructor due-later " data-ud-itemtype="PxUnit" data-ud-points="0" data-ud-isvisibletostudents="True" data-ft-parent="PX_MULTIPART_LESSONS" data-ft-id="module_pdx_morris1e_01" data-ud-id="module_pdx_morris1e_01"data-ud-islc="False" data-ud-date-mode="range" data-ft-sequence="e" data-ft-has-children="true" data-ud-start-date="04/22/2014" data-ud-start-time="11:59:00 PM" data-ud-due-date="04/30/2014" data-ud-due-time="11:59:01 PM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel1 item-type-pxunit hide-in-fne hide-in-fne instructor due-later " data-ud-read-only="false" data-ud-is-assigned="true" data-ft-level = "1" data-ft-chapter = "" hide-student-data = "False" > <div class="faux-tree-node-title col faceplate-hover-div "> <div class="fp-img-wrapper"> <img class="fpimage" alt="" src="http://www.whfreeman.com/BrainHoney/Resource/6716/assets/pdx_files/thumbnails/morris1e/chapter_1_thumbnail.jpg" width="30" height="45" /></div> <span class="icon-placeholder"></span> <span class="unitfptitle"> Chapter 1. Life: Chemical, Cellular, and Evolutionary Foundations </span> </div> <div class="description" style="display: none"> <div class="px-default-text" style="display: block"> Every day, remarkable things happen within and all around you. Strolling through a local market, you come across a bin full of crisp apples, pick one up, and take a bite. Underlying this unremarkable occurrence is a remarkable series of events. Your eyes sense the apple from a distance, and nerves carry that information to your brain, permitting identification. Biologists call this cognition, an area of biological study. Stimulated by the apple and recognizing it as ripe and tasty, your brain transmits impulses through nerves to your muscles. How we respond to external cues motivates behavior, another biological discipline.</div> <div style="display: block;"> <img class="fpimageLarge" alt="" delayedsrc="http://www.whfreeman.com/BrainHoney/Resource/6716/assets/pdx_files/thumbnails/morris1e/chapter_1_thumbnail.jpg" /> </div> <div style="clear:both"></div> </div> <div class="addContentBtn"> <div class="btn-wrapper gradient"> <a id="add-assignment-btn">Add to this Unit <span class="pxicon pxicon-down-open"></span></a> </div> </div> <div class="faceplate-item-status"> <div class="faceplate-row-on-hover"> <div class="pxunit-subitems-menu"> <a class="view-pxunit-menu" href="#"> <div id="face-plate-unit-menu" class="face-plate-unit-menu"> <input type="button" class="faceplate-item-assign " value="Assign" style="display:none;" /> <div class="gearbox gradient" style="float: right"> <span class="gearbox-icon pxicon pxicon-gear"></span> </div> <div style="clear: both;"> </div> </div> </a> </div> </div> <div class="due_date pxunit-display-duedate " style="display: block" > <div class="dd_cal"> <div class="dd_cal_month"> <span class="dd_cal_month_inner"> <span class="due_date_month"> Apr </span> </span> </div> <div class="dd_cal_date"> <span class="start_date_day"> 22 </span>- <span class="due_date_day"> 30 </span> </div> </div> </div> <div class="pxunit-display-points col"> </div> <div class="col faceplate-item-assign"> <span></span> </div> </div> <div class="faceplate-right-menu " id="faceplate-right-menu"> </div> <div style="clear: both"> </div> </li> <li class="faux-tree-node pxunit-item-row unitrowlevel2 item-type-externalcontent hide-in-fne-ebook instructor " data-ud-itemtype="ExternalContent" data-ud-points="5" data-ud-isvisibletostudents="True" data-ft-parent="module_pdx_morris1e_01" data-ft-id="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" data-ud-id="pdx_morris1e_e_book_e8a4a8cb71d77662d8a70564419ad39c"data-ud-islc="False" data-ud-date-mode="single" data-ft-state="barren" data-ft-sequence="i" data-ft-has-children="false" data-ud-start-date="01/01/0001" data-ud-start-time="12:00:00 AM" data-ud-due-date="04/22/2014" data-ud-due-time="11:59:01 PM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel2 item-type-externalcontent hide-in-fne-ebook instructor " data-ud-read-only="false" data-ud-is-assigned="true" data-ft-level = "2" data-ft-chapter = "module_pdx_morris1e_01" hide-student-data = "False" data-ud-default-points="5" data-ud-max-points="5" > <div class="faux-tree-node-title col faceplate-hover-div showaslink"> <div class="fp-img-wrapper"> <span class="fpimage"></span> </div> <span class="icon-placeholder"></span> <span class="fptitle"> <a href="#state/item/module_pdx_morris1e_01/pdx_morris1e_e_book_e8a4a8cb71d77662d8a70564419ad39c?mode=Preview&getChildrenGrades=False&includeDiscussion=False&readOnly=False&renderFNE=True&toc=syllabusfilter" class="faux-tree-link" >Chapter 1 Summary</a> </span> <span class="item_subtitle"> e-Book </span> </div> <div class="faceplate-item-status"> <div class="faceplate-row-on-hover"> <div class="pxunit-subitems-menu"> <a class="view-pxunit-menu" href="#"> <div id="face-plate-unit-menu" class="face-plate-unit-menu"> <input type="button" class="faceplate-item-assign " value="Assign" style="display:none;" /> <div class="gearbox gradient" style="float: right"> <span class="gearbox-icon pxicon pxicon-gear"></span> </div> <div style="clear: both;"> </div> </div> </a> </div> </div> <div class="due_date pxunit-display-duedate " style="display: block" > <div class="dd_cal"> <div class="dd_cal_month"> <span class="dd_cal_month_inner"> <span class="due_date_month"> Apr </span> </span> </div> <div class="dd_cal_date"> <span class="due_date_day"> 22 </span> </div> </div> </div> <div class="pxunit-display-points col"> 5 pts </div> <div class="col faceplate-item-assign"> <span></span> </div> </div> <div class="faceplate-right-menu " id="faceplate-right-menu"> </div> <div style="clear: both"> </div> </li>';
                $.fn.ContentTreeWidgetObj()._static.fn.saveNavigationStateParseViewResponse(fakeResponse, false);
                expect($(".faceplate-unit-subitems-unassigned li").length).toBe(1);
                expect($(".faceplate-unit-subitems li").length).toBe(1);

            });
        });

        describe("& when contentcreated event is triggered", function() {
            var myid = "this is id";
            var myname = "this is chicken's name";
            var myapp = "this is my app";
            var myParentFolderTitle = "this is my parent folder title";
            var entity;
            var newNode, savedNewNode, parentNode, resultTitle;

            beforeEach(function () {
                entity = {
                    id: myid,
                    name: myname,
                    startDate: "01/01/0001",
                    endDate: "01/01/0001"
                };

                window.PxPage.Toasts = {
                    AppName: function () {
                        return myapp;
                    }
                };
                window.PxPage.Context = {
                    IsSandboxCourse: false
                };

                newNode = '<li class="faux-tree-node" data-ft-id="' + myid + '" data-ud-itemtype="HtmlDocument" data-ft-level="1"> \
                                <div class="faux-tree-node-title"><span class="fptitle">Add new item</span></div></li>';
                savedNewNode = '<li class="faux-tree-node" data-ft-id="' + myid + '" data-ud-itemtype="HtmlDocument" data-ft-level="1"> \
                                <div class="faux-tree-node-title"><span class="fptitle"><a href="#">' + myname + '</a></span></div></li>';
                parentNode = '<li class="faux-tree-node" data-ft-id="' + myid + '" data-ud-itemtype="HtmlDocument" data-ft-level="1"> \
                                <div class="faux-tree-node-title"><span class="fptitle">' + myname + '</span></div></li>';
                resultTitle = "";

                spyOn($.fn.ContentTreeWidgetObj()._static.sel, "getItem").andCallFake(function(id) {
                    if (id === "PX_MULTIPART_LESSONS") {
                        return undefined;
                    }

                    return $.fn.ContentTreeWidgetObj()._static.sel.tree + " li.faux-tree-node[data-ft-id=\"" + id + "\"]";;
                });

                spyOn($, "ajax").andCallFake(function (option) {
                    option.success(savedNewNode);
                });
            });

            it("can accept newEntity as an object", function () {
                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "SaveNavigationToast").andCallFake(function (op, title, parentFolderTitle) {
                    resultTitle = title;
                });

                $.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent(entity);

                expect(resultTitle).toEqual(myname);
            });
            it("can accept newEntity as a string", function () {
                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "SaveNavigationToast").andCallFake(function (op, title, parentFolderTitle) {
                    resultTitle = title;
                });

                $.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent(myid);

                expect(resultTitle).toEqual(myname);
            });
            it("can pass rawtitle and foldertitle only as string to SaveNavigationToast", function () {
                var isString;
                var strTitle;
                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "SaveNavigationToast").andCallFake(function (op, title, foldertitle) {
                    expect(op).toEqual($.fn.ContentTreeWidgetObj()._static.operation.newItem);

                    isString = typeof title === "string";
                    strTitle = title;
                });

                $.fn.ContentTreeWidgetObj()._static.fn.SaveNavigationToast($.fn.ContentTreeWidgetObj()._static.operation.newItem, { rawtitle: "is an object" });

                expect(isString).toBeFalsy();

                $.fn.ContentTreeWidgetObj()._static.fn.SaveNavigationToast($.fn.ContentTreeWidgetObj()._static.operation.newItem, "is a string");

                expect(isString).toBeTruthy();
                expect(strTitle).toEqual("is a string");
            });
            it("can call SaveNavigationToast on success", function () {
                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "SaveNavigationToast").andCallFake(function (op, title, foldertitle) {
                    expect(op).toEqual($.fn.ContentTreeWidgetObj()._static.operation.newItem);

                    expect(title).toEqual(myname);
                    expect(foldertitle).toEqual(myapp);
                });

                $.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent(entity);
            });
            it("can call GetParentFolderTitle when SaveNavigationToast method is called", function () {
                var expectedFolderTitle = "";

                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "SaveNavigationToast").andCallFake(function (op, title, foldertitle) {
                    expectedFolderTitle = foldertitle;
                });

                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "GetParentFolderTitle").andReturn(myParentFolderTitle);

                $.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent(entity);

                expect(expectedFolderTitle).toEqual(myParentFolderTitle);
            });
            it("can retrieve parent folder name from .fptitle element", function () {
                var myNewNode;
                var parentNodeId = "";
                var parentFolderName = "";
                var nodes = $("li[data-ud-itemtype='PxUnit'][data-ft-level=1]");
                if (nodes.length > 0) {
                    var node = nodes[0];
                    parentNodeId = $(node).data("ud-id");
                    parentFolderName = $(node).find(".unitfptitle").html().trim();
                }

                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "saveNavigationState").andCallFake(function (data) {
                    myNewNode = data.changed;

                    // add parent Id
                    $(myNewNode).ftattr("parent", parentNodeId);
                });

                $.fn.ContentTreeWidgetObj()._static.fn.onAddExistingContent(entity);

                var folderTitle = $.fn.ContentTreeWidgetObj()._static.fn.GetParentFolderTitle(myNewNode);

                expect(folderTitle).toEqual(parentFolderName);
            });
        });
    });

    describe("When an item is removed", function() {
        var toc = 'syllabusfilter';

        beforeEach(function () {
            var arrange = $.fn.GetArrangeLaunchpadSet1();

            var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
            var htmleveryoneDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                                            {1} \
                                            </div>", widgetId, domToc);

            setFixtures(htmleveryoneDoc);

            $("#launchpad-widget-" + widgetId).ContentTreeWidget({
                readOnly: true,
                showManagementCard: true,
                showExpandIcon: true,
                showExpandIconLevel: 1,
                togglePastDue: false,
                toggleDueLater: false,
                enableDragAndDrop: false,
                showCollapseUnassigned: false,
                collapseUnassigned: false,
                collapseDueLaterByDefault: false,
                collapsePastDueByDefault: false,
                grayOutPastDueLater: false,
                dueSoonDays: 14,
                sortByDueDate: false,
                courseNumber: 129860,
                splitAssignedUnassigned: true,
                triggerOpenContentOnClick: false,
                toc: toc,
                assignmentToc: "",
                showAssignmentUnitFlow: false
            });
        });

        describe("it", function() {
            it("can call 'saveNavigationState' with data having correct operation and removeFrom properties", function() {
                var itemId = "This is my item id";
                var removeFrom = "MyFILTER_A, MyFILTER_B";
                
                var item = {                    
                    contentItemId: itemId,
                    removeFrom: removeFrom
                };

                spyOn($.fn.ContentTreeWidgetObj()._static.sel, "getItem").andCallFake(function(id) {
                    return item;
                });

                spyOn($.fn.ContentTreeWidgetObj()._static.fn, "saveNavigationState").andCallFake(function(data) {
                    expect(data.operation).toBe($.fn.ContentTreeWidgetObj()._static.operation.remove);
                    expect(data.removeFrom).toBe(removeFrom);
                    expect(data.changed[0].contentItemId).toBe(itemId);
                });

                $(PxPage.switchboard).trigger("onremoveitem", [item]);
            });
            
            it("can send data with correct operation and removeFrom properties to server via 'saveNavigationState'", function() {
                var itemId = "This is my item id";
                var removeFrom = "MyFILTER_A, MyFILTER_B";
                
                var item = {                    
                    contentItemId: itemId,
                    removeFrom: removeFrom
                };

                spyOn($, "ajax").andCallFake(function (args) {
                    var obj = JSON.parse(args.data);
                    
                    expect(obj.state.operation).toBe($.fn.ContentTreeWidgetObj()._static.operation.remove);
                    expect(obj.state.removeFrom).toBe(item.removeFrom);
                });

                spyOn($.fn.ContentTreeWidgetObj()._static.sel, "getItem").andCallFake(function(id) {
                    return item;
                });

                $(PxPage.switchboard).trigger("onremoveitem", [item]);
            });
            
            it("can send data with correct operation and removeFrom properties to server via 'saveNavigationState' even if removeFrom is null", function() {
                var itemId = "This is my item id";
                
                var item = {                    
                    contentItemId: itemId
                };

                spyOn($, "ajax").andCallFake(function (args) {
                    var obj = JSON.parse(args.data);

                    expect(obj.state.operation).toBe($.fn.ContentTreeWidgetObj()._static.operation.remove);
                    expect(obj.state.removeFrom).toBe(toc);
                });

                spyOn($.fn.ContentTreeWidgetObj()._static.sel, "getItem").andCallFake(function(id) {
                    return item;
                });

                $(PxPage.switchboard).trigger("onremoveitem", [item]);
            });
        });
    });
    
    describe("When showAssignmentUnitFlow is TRUE:", function () {
        beforeEach(function () {
            spyOn($.fn.ContentTreeWidgetObj()._static.fn, "Loading");

            if (htmlXbookDoc == "") {
                var arrange = $.fn.GetArrangeXbookSet1();

                var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
                htmlXbookDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                            {1} \
                           </div>", widgetId, domToc);

                setFixtures(htmlXbookDoc);
            }

            var jfixtures = $("#jasmine-fixtures").html();
            if (!jfixtures) {
                if (openedNodeFixture) {
                    setFixtures(openedNodeFixture);
                } else {
                    setFixtures(htmlXbookDoc);
                }
            }

            $("#launchpad-widget-" + widgetId).ContentTreeWidget({
                readOnly: false,
                showManagementCard: true,
                showExpandIcon: true,
                showExpandIconLevel: 0,
                togglePastDue: false,
                toggleDueLater: false,
                enableDragAndDrop: false,
                showCollapseUnassigned: false,
                collapseUnassigned: false,
                collapseDueLaterByDefault: false,
                collapsePastDueByDefault: false,
                grayOutPastDueLater: false,
                dueSoonDays: 14,
                sortByDueDate: false,
                courseNumber: 129860,
                splitAssignedUnassigned: true,
                triggerOpenContentOnClick: false,
                toc: "syllabusfilter",
                assignmentToc: "assignmentfilter", 
                showAssignmentUnitFlow: true
            });
        });

        afterEach(function () {
            openedNodeFixture = $("#jasmine-fixtures").html();
        });

        xdescribe("When an icon is clicked", function () {
            it("can expand the treenode if it has (+) sign (NOT IMPLEMENTED)", function () {

            });

            it("can contract the treenode if it has (-) sign (NOT IMPLEMENTED)", function () {

            });
        });

        describe("When a parent node is clicked", function () {
            var testNode;
            beforeEach(function () {
                if (testNodeId) {
                    testNode = $($.format("li[data-ud-id='{0}']", testNodeId));
                } else {
                    testNode = $($.format("li{0}", "[data-ft-state='closed']")).first();
                    testNodeId = testNode.data("ud-id");
                }
            });

            it("can display all children", function () {
                spyOn($, "ajax").andCallFake(function (response) {
                    response.success('<li class="faux-tree-node pxunit-item-row  HideForEbook item-type-externalcontent hide-in-fne-ebook instructor even unitrowlevel2" data-ud-itemtype="ExternalContent" data-ud-points="0" data-ud-isvisibletostudents="True" data-ft-parent="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" data-ft-id="bsi__22954099__1854__4DAA__830F__AB7767C756FB" data-ud-id="bsi__22954099__1854__4DAA__830F__AB7767C756FB" data-ud-date-mode="single" data-ft-state="barren" data-ft-sequence="b" data-ft-has-children="false" data-ud-start-date="01/01/0001" data-ud-start-time="12:00:00 AM" data-ud-due-date="01/01/0001" data-ud-due-time="12:00 AM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel2 HideForEbook item-type-externalcontent hide-in-fne-ebook instructor  " data-ud-read-only="false" data-ud-is-assigned="false" data-ft-level="2" data-ft-chapter="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" hide-student-data="False" data-ud-default-points="10" style="padding-left: 0px;" data-ft-visible="true">            <div class="icon" style="display: none">            </div>            <div class="faux-tree-node-title col faceplate-hover-div showaslink">                                    <div class="fp-img-wrapper">                        <span class="fpimage"></span>                    </div>                                    <span class="icon-placeholder"></span>                    <span class="fptitle">                    <a href="#state/item/MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407/bsi__22954099__1854__4DAA__830F__AB7767C756FB?mode=Preview&amp;getChildrenGrades=False&amp;includeDiscussion=False&amp;renderFNE=True&amp;toc=syllabusfilter" class="faux-tree-link">LearningCurve: Exploring Economics</a>                                            </span><span class="item_subtitle">                                                          </span>                                    </div>                            <div class="description" style="display: none">                    <div class="px-default-text" style="display: none">                        </div>                                    </div>                        <div class="faceplate-item-status">                <div class="faceplate-row-on-hover">                    <div class="pxunit-subitems-menu">                        <a class="view-pxunit-menu" href="#">                                                                        <div id="face-plate-unit-menu" class="face-plate-unit-menu">                                                                                    <input type="button" class="faceplate-item-assign " value="Assign" style="">                                          <div class="gearbox gradient" style="float: right">                                              <span class="gearbox-icon pxicon pxicon-gear"></span>                                          </div>                                          <div style="clear: both;">                                          </div>                                      </div>                                                                </a>                    </div>                </div>                        <div class="due_date pxunit-display-duedate  " style="display: none">            <div class="dd_cal">                <div class="dd_cal_month">                    <span class="dd_cal_month_inner">                                                <span class="due_date_month">                                                    </span>                                            </span>                </div>                <div class="dd_cal_date">                                        <span class="due_date_day">                                            </span>                    </div>            </div>        </div>        <div class="pxunit-display-points col">            </div>        <div class="col faceplate-item-assign">            <span></span>                    </div>                        </div>                            <div class="faceplate-right-menu " id="faceplate-right-menu">                </div>                                        <div style="clear: both">            </div>        </li><li class="faux-tree-node pxunit-item-row  item-type-externalcontent hide-in-fne-ebook instructor odd unitrowlevel2" data-ud-itemtype="ExternalContent" data-ud-points="0" data-ud-isvisibletostudents="True" data-ft-parent="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" data-ft-id="stoneecon2_1_0" data-ud-id="stoneecon2_1_0" data-ud-date-mode="single" data-ft-state="barren" data-ft-sequence="c" data-ft-has-children="false" data-ud-start-date="01/01/0001" data-ud-start-time="12:00:00 AM" data-ud-due-date="01/01/0001" data-ud-due-time="12:00 AM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel2 item-type-externalcontent hide-in-fne-ebook instructor  " data-ud-read-only="false" data-ud-is-assigned="false" data-ft-level="2" data-ft-chapter="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" hide-student-data="False" style="padding-left: 0px;" data-ft-visible="true">            <div class="icon" style="display: none">            </div>            <div class="faux-tree-node-title col faceplate-hover-div showaslink">                                    <div class="fp-img-wrapper">                        <span class="fpimage"></span>                    </div>                                    <span class="icon-placeholder"></span>                    <span class="fptitle">                    <a href="#state/item/MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407/stoneecon2_1_0?mode=Preview&amp;getChildrenGrades=False&amp;includeDiscussion=False&amp;renderFNE=True&amp;toc=syllabusfilter" class="faux-tree-link">CHAPTER INTRODUCTION</a>                                            </span><span class="item_subtitle">                                                          </span>                                    </div>                            <div class="description" style="display: none">                    <div class="px-default-text" style="display: block">                        ebook</div>                                    </div>                        <div class="faceplate-item-status">                <div class="faceplate-row-on-hover">                    <div class="pxunit-subitems-menu">                        <a class="view-pxunit-menu" href="#">                                                                        <div id="face-plate-unit-menu" class="face-plate-unit-menu">                                                                                    <input type="button" class="faceplate-item-assign " value="Assign" style="">                                          <div class="gearbox gradient" style="float: right">                                              <span class="gearbox-icon pxicon pxicon-gear"></span>                                          </div>                                          <div style="clear: both;">                                          </div>                                      </div>                                                                </a>                    </div>                </div>                        <div class="due_date pxunit-display-duedate  " style="display: none">            <div class="dd_cal">                <div class="dd_cal_month">                    <span class="dd_cal_month_inner">                                                <span class="due_date_month">                                                    </span>                                            </span>                </div>                <div class="dd_cal_date">                                        <span class="due_date_day">                                            </span>                    </div>            </div>        </div>        <div class="pxunit-display-points col">            </div>        <div class="col faceplate-item-assign">            <span></span>                    </div>                        </div>                            <div class="faceplate-right-menu " id="faceplate-right-menu">                </div>                                        <div style="clear: both">            </div>        </li><li class="faux-tree-node pxunit-item-row  item-type-pxunit hide-in-fne hide-in-fne instructor even unitrowlevel2" data-ud-itemtype="PxUnit" data-ud-points="0" data-ud-isvisibletostudents="True" data-ft-parent="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" data-ft-id="bsi__3F58EE91__B2B4__41C7__A490__27698E55F516" data-ud-id="bsi__3F58EE91__B2B4__41C7__A490__27698E55F516" data-ud-date-mode="range" data-ft-sequence="d" data-ft-has-children="true" data-ud-start-date="01/01/0001" data-ud-start-time="12:00:00 AM" data-ud-due-date="01/01/0001" data-ud-due-time="12:00 AM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel2 item-type-pxunit hide-in-fne hide-in-fne instructor  " data-ud-read-only="false" data-ud-is-assigned="false" data-ft-level="2" data-ft-chapter="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" hide-student-data="False" style="padding-left: 0px;" data-ft-state="closed" data-ft-visible="true">            <div class="icon" style="display: none">            </div>            <div class="faux-tree-node-title col faceplate-hover-div ">                                                               <div class="fp-img-wrapper">                            <img class="fpimage" alt="" src="/launchpad/stoneecon2/120559/Style/Images/unit_default_thumbnail.png"></div>                                       <span class="iconlapsed"></span>                                            <span class="fptitle">                            What Is Economics About?                        </span>                                        </div>                            <div class="description" style="display: none;">                    <div class="px-default-text" style="display: none;">                        </div>                                    </div>                        <div class="faceplate-item-status">                <div class="faceplate-row-on-hover">                    <div class="pxunit-subitems-menu">                        <a class="view-pxunit-menu" href="#">                                                                        <div id="face-plate-unit-menu" class="face-plate-unit-menu">                                                                                    <input type="button" class="faceplate-item-assign " value="Assign" style="">                                          <div class="gearbox gradient" style="float: right">                                              <span class="gearbox-icon pxicon pxicon-gear"></span>                                          </div>                                          <div style="clear: both;">                                          </div>                                      </div>                                                                </a>                    </div>                </div>                        <div class="due_date pxunit-display-duedate  " style="display: none">            <div class="dd_cal">                <div class="dd_cal_month">                    <span class="dd_cal_month_inner">                                                <span class="due_date_month">                                                    </span>                                            </span>                </div>                <div class="dd_cal_date">                                        <span class="due_date_day">                                            </span>                    </div>            </div>        </div>        <div class="pxunit-display-points col">            </div>        <div class="col faceplate-item-assign">            <span></span>                    </div>                        </div>                            <div class="faceplate-right-menu " id="faceplate-right-menu"></div><div style="clear: both"></div></li>');
                });

                var totalSiblings = testNode.siblings().length;

                // expand an item 
                testNode.find(titleSelector).trigger("click");

                // if an item is expanded, see if it has children
                expect(testNode.siblings("li").length).toBeGreaterThan(totalSiblings);
            });

            it("can show (-) sign when expanded", function () {
                var css = testNode.find(iconSelector).attr("class");
                expect(css).toBe("icon expanded");
            });
        });

        describe("When a parent node with 'PxUnit' type is clicked", function() {
            var pxTestNode;
            var pxTestNodeId;
            beforeEach(function () {
                spyOn($, "ajax").andCallFake(function (response) {
                    response.success('<li class="faux-tree-node pxunit-item-row  HideForEbook item-type-externalcontent hide-in-fne-ebook instructor even unitrowlevel2" data-ud-itemtype="ExternalContent" data-ud-points="0" data-ud-isvisibletostudents="True" data-ft-parent="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" data-ft-id="bsi__22954099__1854__4DAA__830F__AB7767C756FB" data-ud-id="bsi__22954099__1854__4DAA__830F__AB7767C756FB" data-ud-date-mode="single" data-ft-state="barren" data-ft-sequence="b" data-ft-has-children="false" data-ud-start-date="01/01/0001" data-ud-start-time="12:00:00 AM" data-ud-due-date="01/01/0001" data-ud-due-time="12:00 AM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel2 HideForEbook item-type-externalcontent hide-in-fne-ebook instructor  " data-ud-read-only="false" data-ud-is-assigned="false" data-ft-level="2" data-ft-chapter="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" hide-student-data="False" data-ud-default-points="10" style="padding-left: 0px;" data-ft-visible="true"><div class="icon" style="display: none"></div><div class="faux-tree-node-title col faceplate-hover-div showaslink"><div class="fp-img-wrapper"><span class="fpimage"></span></div><span class="icon-placeholder"></span><span class="fptitle"><a href="#state/item/MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407/bsi__22954099__1854__4DAA__830F__AB7767C756FB?mode=Preview&amp;getChildrenGrades=False&amp;includeDiscussion=False&amp;renderFNE=True&amp;toc=syllabusfilter" class="faux-tree-link">LearningCurve: Exploring Economics</a></span><span class="item_subtitle"></span></div><div class="description" style="display: none"><div class="px-default-text" style="display: none"></div></div><div class="faceplate-item-status"><div class="faceplate-row-on-hover"><div class="pxunit-subitems-menu"><a class="view-pxunit-menu" href="#">                                                                        <div id="face-plate-unit-menu" class="face-plate-unit-menu">                                                                                    <input type="button" class="faceplate-item-assign " value="Assign" style="">                                          <div class="gearbox gradient" style="float: right">                                              <span class="gearbox-icon pxicon pxicon-gear"></span>                                          </div>                                          <div style="clear: both;">                                          </div>                                      </div>                                                                </a>                    </div>                </div>                        <div class="due_date pxunit-display-duedate  " style="display: none">            <div class="dd_cal">                <div class="dd_cal_month">                    <span class="dd_cal_month_inner">                                                <span class="due_date_month">                                                    </span>                                            </span>                </div>                <div class="dd_cal_date">                                        <span class="due_date_day">                                            </span>                    </div>            </div>        </div>        <div class="pxunit-display-points col">            </div>        <div class="col faceplate-item-assign">            <span></span>                    </div>                        </div>                            <div class="faceplate-right-menu " id="faceplate-right-menu">                </div>                                        <div style="clear: both">            </div>        </li><li class="faux-tree-node pxunit-item-row  item-type-externalcontent hide-in-fne-ebook instructor odd unitrowlevel2" data-ud-itemtype="ExternalContent" data-ud-points="0" data-ud-isvisibletostudents="True" data-ft-parent="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" data-ft-id="stoneecon2_1_0" data-ud-id="stoneecon2_1_0" data-ud-date-mode="single" data-ft-state="barren" data-ft-sequence="c" data-ft-has-children="false" data-ud-start-date="01/01/0001" data-ud-start-time="12:00:00 AM" data-ud-due-date="01/01/0001" data-ud-due-time="12:00 AM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel2 item-type-externalcontent hide-in-fne-ebook instructor  " data-ud-read-only="false" data-ud-is-assigned="false" data-ft-level="2" data-ft-chapter="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" hide-student-data="False" style="padding-left: 0px;" data-ft-visible="true">            <div class="icon" style="display: none">            </div>            <div class="faux-tree-node-title col faceplate-hover-div showaslink">                                    <div class="fp-img-wrapper">                        <span class="fpimage"></span>                    </div>                                    <span class="icon-placeholder"></span>                    <span class="fptitle">                    <a href="#state/item/MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407/stoneecon2_1_0?mode=Preview&amp;getChildrenGrades=False&amp;includeDiscussion=False&amp;renderFNE=True&amp;toc=syllabusfilter" class="faux-tree-link">CHAPTER INTRODUCTION</a>                                            </span><span class="item_subtitle">                                                          </span>                                    </div>                            <div class="description" style="display: none">                    <div class="px-default-text" style="display: block">                        ebook</div>                                    </div>                        <div class="faceplate-item-status">                <div class="faceplate-row-on-hover">                    <div class="pxunit-subitems-menu">                        <a class="view-pxunit-menu" href="#">                                                                        <div id="face-plate-unit-menu" class="face-plate-unit-menu">                                                                                    <input type="button" class="faceplate-item-assign " value="Assign" style="">                                          <div class="gearbox gradient" style="float: right">                                              <span class="gearbox-icon pxicon pxicon-gear"></span>                                          </div>                                          <div style="clear: both;">                                          </div>                                      </div>                                                                </a>                    </div>                </div>                        <div class="due_date pxunit-display-duedate  " style="display: none">            <div class="dd_cal">                <div class="dd_cal_month">                    <span class="dd_cal_month_inner">                                                <span class="due_date_month">                                                    </span>                                            </span>                </div>                <div class="dd_cal_date">                                        <span class="due_date_day">                                            </span>                    </div>            </div>        </div>        <div class="pxunit-display-points col">            </div>        <div class="col faceplate-item-assign">            <span></span>                    </div>                        </div>                            <div class="faceplate-right-menu " id="faceplate-right-menu">                </div>                                        <div style="clear: both">            </div>        </li><li class="faux-tree-node pxunit-item-row  item-type-pxunit hide-in-fne hide-in-fne instructor even unitrowlevel2" data-ud-itemtype="PxUnit" data-ud-points="0" data-ud-isvisibletostudents="True" data-ft-parent="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" data-ft-id="bsi__3F58EE91__B2B4__41C7__A490__27698E55F516" data-ud-id="bsi__3F58EE91__B2B4__41C7__A490__27698E55F516" data-ud-date-mode="range" data-ft-sequence="d" data-ft-has-children="true" data-ud-start-date="01/01/0001" data-ud-start-time="12:00:00 AM" data-ud-due-date="01/01/0001" data-ud-due-time="12:00 AM" data-ud-wasduedatemanuallyset="False" data-ud-sortlevel=" unitrowlevel2 item-type-pxunit hide-in-fne hide-in-fne instructor  " data-ud-read-only="false" data-ud-is-assigned="false" data-ft-level="2" data-ft-chapter="MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407" hide-student-data="False" style="padding-left: 0px;" data-ft-state="closed" data-ft-visible="true">            <div class="icon" style="display: none">            </div>            <div class="faux-tree-node-title col faceplate-hover-div ">                                                               <div class="fp-img-wrapper"><img class="fpimage" alt="" src="/launchpad/stoneecon2/120559/Style/Images/unit_default_thumbnail.png"></div><span class="iconlapsed"></span><span class="fptitle">What Is Economics About?</span></div><div class="description" style="display: none;"><div class="px-default-text" style="display: none;"></div></div><div class="faceplate-item-status"><div class="faceplate-row-on-hover"><div class="pxunit-subitems-menu"><a class="view-pxunit-menu" href="#"><div id="face-plate-unit-menu" class="face-plate-unit-menu"><input type="button" class="faceplate-item-assign " value="Assign" style=""><div class="gearbox gradient" style="float: right"><span class="gearbox-icon pxicon pxicon-gear"></span></div><div style="clear: both;"></div></div></a></div></div><div class="due_date pxunit-display-duedate  " style="display: none"><div class="dd_cal"><div class="dd_cal_month"><span class="dd_cal_month_inner"><span class="due_date_month"></span></span></div><div class="dd_cal_date"><span class="due_date_day"></span></div></div></div><div class="pxunit-display-points col"></div><div class="col faceplate-item-assign"><span></span></div></div><div class="faceplate-right-menu " id="faceplate-right-menu"></div><div style="clear: both"></div></li>');
                });
                
                if (pxTestNodeId) {
                    pxTestNode = $($.format("li[data-ud-id='{0}']", pxTestNodeId));
                }
            });

            afterEach(function() {
            });
            
            it("can show 'Add to this Unit' menu if its level == 1 AND type == PxUnit", function () {
                pxTestNode = $("li.faux-tree-node[data-ud-itemtype='PxUnit'][data-ft-state='closed'][data-ft-level='1']").first();
                pxTestNodeId = pxTestNode.data("ud-id");
                
                // expand an item 
                pxTestNode.find(titleSelector).trigger("click");

                // if an item is expanded, see if it has children
                expect(pxTestNode.find(".addContentBtn").attr("style").toLowerCase().replace(' ', '')).not.toContain("display:none;");
            });

            it("does not show 'Add to this Unit' menu if its type != PxUnit", function () {
                pxTestNode.udattr("itemtype", "nada");
                // click on node again to close it
                pxTestNode.find(titleSelector).trigger("click");
                
                waitsFor(function () {
                    return pxTestNode.data("ft-state") === "closed";
                }, "Closing an item", 1000);
                
                runs(function() {
                    // expand an item again
                    pxTestNode.find(titleSelector).trigger("click");

                    // if an item is expanded, see if it has children
                    expect(pxTestNode.find(".addContentBtn").attr("style").toLowerCase().replace(' ', '')).toContain("display:none;");
                });
            });
        });

        describe("When an expanded parent node is clicked", function () {
            var testNode;
            beforeEach(function () {
                if (testNodeId) {
                    testNode = $($.format("li[data-ud-id='{0}']", testNodeId));
                }
            });

            it("can hide all children", function () {
                // click on node again
                testNode.find(titleSelector).trigger("click");

                waitsFor(function () {
                    return testNode.data("ft-state") === "closed";
                }, "Closing an item", 1000);

                runs(function () {
                    var lis = $("li[data-ft-parent='" + testNodeId + "']");
                    expect(lis.length).toBeGreaterThan(0);
                    lis.each(function () {
                        var $this = $(this);
                        expect($this.data("ft-state")).not.toBe("open");
                    });
                });
            });

            it("can show (+) sign when collapsed", function () {
                // show (+) sign when collapsed
                var css = testNode.find(iconSelector).attr("class");
                expect(css).toBe("icon collapsed");
            });
        });

        describe("When an item is clicked", function () {
            var testNode;
            var documentViewerHtml;
            beforeEach(function () {
                testNode = $("li[data-ft-has-children=false]").first();

                if (!$("#content-px").html()) {
                    appendSetFixtures("<div id='content-px'><p>This is a test for section 1</p></div>");
                }
                documentViewerHtml = $("#content-px").html();

                XBookV2.Init();
            });

            it("does not take action if the content link is invalid", function () {
                spyOn($.fn.fauxtreeObj()._static.fn, "clickNode");

                window.location.hash = "#state/MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407/stoneecon2_1_0?mode=Preview&getChildrenGrades=False&includeDiscussion=False&renderFNE=True&toc=syllabusfilter";
                testNode.find(titleSelector).trigger("click");

                expect($.fn.fauxtreeObj()._static.fn.clickNode).toHaveBeenCalled();
                expect($("#content-px").html()).toEqual(documentViewerHtml);
            });

            it("can apply the active state only to the selected item", function () {
                $.fn.fauxtreeObj()._static.fn.clickNode.apply(testNode, [{ event: {} }]);

                $("#launchpad-widget-" + widgetId + " li").each(function (ind) {
                    var $this = $(this);
                    if ($this.data("ud-id") === testNode.data("ud-id")) {
                        expect($this.attr("class").indexOf("active")).toBeGreaterThan(-1);
                    } else {
                        expect($this.attr("class").indexOf("active")).toBe(-1);
                    }
                });
            });
        });

        describe("When a setting icon is clicked", function () {
            var testNode;
            beforeEach(function () {
                testNode = $("li[data-ud-read-only=false]")[0];
                if (testNode) {
                    testNode = $(testNode);
                }

                $.fn.DatePicker = function () {
                };

                $.fn.placeholder = function () {
                };

                $.fn.ptTimeSelect = function () {
                };

                window.tinyMCE = undefined;
                window.PxPage.isIE = function() {
                    return false;
                };
            });

            it("can open a management card if it is already closed", function () {
                var domMc = "";
                waitsFor(function () {
                    var arrange = $.fn.GetArrangeManagementCardForXbook();
                    domMc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);

                    return domMc != "";
                }, "getting managementcard.ascx view", 1000);

                runs(function () {
                    spyOn($, "post").andCallFake(function (url, data, callback) {
                        if (callback && data.contentItemId) {
                            callback(domMc);
                            PxManagementCard.Init(testNode.data("ud-id"), "contentcreate", true);
                        }
                    });

                    var fpRightMenu = testNode.find(".faceplate-right-menu");

                    if (fpRightMenu && fpRightMenu.length)
                        fpRightMenu.hide();

                    var btnSetting = testNode.find($.fn.ContentTreeWidgetObj()._static.sel.itemAssignButton);
                    if (btnSetting.length) {
                        btnSetting[0].click();
                    }

                    expect(fpRightMenu.attr("style")).not.toContain("display: none;");
                    //expect(fpRightMenu.attr("style")).toContain("display: block;");
                });
            });

            it("can show the Assignment Unit section on Management Card", function () {
                var domMc = '';
                waitsFor(function () {
                    var arrange = $.fn.GetArrangeManagementCardForXbook();
                    domMc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);

                    return domMc != "";
                }, "getting managementcard.ascx view", 1000);
                runs(function () {
                    spyOn($, "post").andCallFake(function (url, data, callback) {
                        if (callback && data.contentItemId) {
                            callback(domMc);
                            PxManagementCard.Init(testNode.data("ud-id"), "contentcreate", true);
                        }
                    });

                    var fpRightMenu = testNode.find(".faceplate-right-menu");

                    if (fpRightMenu && fpRightMenu.length)
                        fpRightMenu.hide();

                    var btnSetting = testNode.find($.fn.ContentTreeWidgetObj()._static.sel.itemAssignButton);
                    if (btnSetting) {
                        btnSetting.click();
                    }

                    expect(testNode.find(".faceplate-assign-dropdown-container").length).toBeGreaterThan(0);
                });
            });
        });

        describe("when 'create new assignment' button is clicked", function () {
            var testNode;
            beforeEach(function () {
                testNode = $("li[data-ud-read-only=false]")[0];
                if (testNode) {
                    testNode = $(testNode);
                }

                $.fn.DatePicker = function () {
                };

                $.fn.placeholder = function () {
                };

                $.fn.ptTimeSelect = function () {
                };

                window.tinyMCE = undefined;
            });
            
            it("can open a dialog to create a new Assignment Unit", function () {
                var createAssignmentUnit = false;
                spyOn($, "get").andCallFake(function (url, data, callback) {
                    createAssignmentUnit = true;
                });

                spyOn($, "post").andCallFake(function (url, data, callback) {
                    createAssignmentUnit = true;
                });

                window.PxContentTemplates = {
                    CreateItemFromTemplate: function (templateId, callback) {
                        if (callback) {
                            var item = {
                                id: "12345"
                            };
                            callback(item);
                        }
                    }
                };

                PxManagementCard.Init(testNode.data("ud-id"), "contentcreate", true);

                var btnAssignment = testNode.find(".assignment-selection-btn");
                expect(btnAssignment.length).toBeGreaterThan(0);

                btnAssignment.click();

                expect(createAssignmentUnit).toEqual(true);
            });
        });
    });

    describe("When showAssignmentUnitFlow is FALSE:", function () {
        var toc = 'syllabusfilter';
        beforeEach(function () {
            spyOn($.fn.ContentTreeWidgetObj()._static.fn, "markLastViewed").andCallFake(function (tree, chapterid) {
                return true;
            });

            if (htmlLaunchpadDoc == "") {
                var arrange = $.fn.GetArrangeLaunchpadSet1();

                var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
                htmlLaunchpadDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                            {1} \
                           </div>", widgetId, domToc);

                setFixtures(htmlLaunchpadDoc);
            }

            var jfixtures = $("#jasmine-fixtures").html();
            if (!jfixtures) {
                setFixtures(openedNodeFixture);
            }

            $("#launchpad-widget-" + widgetId).ContentTreeWidget({
                readOnly: true,
                showManagementCard: true,
                showExpandIcon: true,
                showExpandIconLevel: 1,
                togglePastDue: false,
                toggleDueLater: false,
                enableDragAndDrop: false,
                showCollapseUnassigned: false,
                collapseUnassigned: false,
                collapseDueLaterByDefault: false,
                collapsePastDueByDefault: false,
                grayOutPastDueLater: false,
                dueSoonDays: 14,
                sortByDueDate: false,
                courseNumber: 129860,
                splitAssignedUnassigned: true,
                triggerOpenContentOnClick: false,
                toc: toc,
                assignmentToc: "",
                showAssignmentUnitFlow: false
            });
        });

        afterEach(function () {
            openedNodeFixture = $("#jasmine-fixtures").html();
        });

        it("does not show carets on top level items (chapters)", function () {
            var iconCollapsed = iconSelector + ".collapsed";
            var iconExpanded = iconSelector + ".expanded";

            $("li[data-ft-parent='" + itemParentId + "']").each(function (ind) {
                var $this = $(this);
                expect($this.find(iconCollapsed).length).toBe(0);
                expect($this.find(iconExpanded).length).toBe(0);
            });
        });

        it("does not have Assignment Units dropdown list and 'Create New Assignment' button", function () {
            var firstNode = $("li.faux-tree-node").first();

            var domMc = "";
            waitsFor(function () {
                var arrange = $.fn.GetArrangeManagementCardForLaunchpad();
                domMc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);

                return domMc != "";
            }, "getting managementcard.ascx view", 1000);

            runs(function () {
                var fpRightMenu = firstNode.find(".faceplate-right-menu");
                fpRightMenu.html(domMc);

                expect(firstNode.find(".faceplate-assign-dropdown-container").length).toBe(0);
            });
        });

        it("can add/update tree item if any tree exists", function () {
            var result = false;

            var item = {
                id: 'id',
                title: 'title'
            };
            //expect event is bound to PxPage.switch
            expect($._data($(PxPage.switchboard)[0], "events").contentcreated.length).toBeGreaterThan(0);
            //spyon the handlerc d
            spyOn($._data($(PxPage.switchboard)[0], "events").contentcreated[0], "handler").andCallFake(function () {
                result = true;
            });
            $(PxPage.switchboard).trigger("contentcreated", [item, 'parentId', 'newItem', null]);

            expect(result).toBeTruthy();

        });

        it("can't add/update tree item if no tree exists", function () {
            var result = false;

            spyOn($.fn.ContentTreeWidgetObj()._static.fn, "onAddExistingContent").andCallFake(function () {
                result = true;
            });

            $('.faux-tree').addClass('faux-tree-removed');
            $('.faux-tree').removeClass('faux-tree');
           
            var item = {
                id: 'id',
                title: 'title'
            };
            $(PxPage.switchboard).trigger("contentcreated", [item, 'parentId', 'newItem', null]);
            $('.faux-tree-removed').addClass('faux-tree');
            $('.faux-tree-removed').removeClass('faux-tree-removed');

            expect(result).toBeFalsy();
        });

        describe("Management Card:", function () {
            it("will clear up the content when hidden", function () {
                var managementCard = "<div class='faceplate-right-menu'>Here's the content of the management card!</div>";
                setFixtures(managementCard);
                
                $.fn.ContentTreeWidgetObj()._static.fn.hideManagementCard();
                
                waitsFor(function () {
                    return $('.faceplate-right-menu:visible').length === 0;
                }, 'Closing the card', 1000);
                
                runs(function () {
                    expect($('.faceplate-right-menu').html()).toBe('');
                });
            });
        });

        describe('fix ie select block (selectEventUnblock) ', function () {

            var _;
 
                beforeAll(function () {
                var arrange = $.fn.GetArrangeLaunchpadSet1();
                var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
                    htmlLaunchpadDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                              {1} \
                             </div>", widgetId, domToc);
 
                setFixtures(htmlLaunchpadDoc);
 
                appendSetFixtures(pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', $.fn.GetArrangeManagementCardForLaunchpad()));
                
                $('.faux-tree-node').eq(0).append($('.FacePlateAsssign'));

                _ = $.fn.ContentTreeWidgetObj()._static;
 
              });
 
              it('bind event to select mousedown', function () {  
                _.fn.selectEventUnblock(true);
                expect( typeof $._data($('#selgradebookweights')[0], 'events')).toEqual('object');
              });

              it('unbind event to select mousedown', function () {  
                _.fn.selectEventUnblock(false);
                expect($._data($('#selgradebookweights')[0], 'events')).toEqual(null);
              });
 
          });

    });

    describe("Rendering", function() {
        describe("tree nodes (LaunchpadItem.ascx)", function () {
            //Test external content nodes
            describe('with external content items', function() {
                var ui = {
                    xb_ecnode: '.faux-tree-node[data-ud-id="xbook_ec"]',
                    lp_ecnode: '.faux-tree-node[data-ud-id="launchpad_ec"]'
                };
                beforeEach(function () {
                    var items = [
                        TreeWidgetViewItems.XBook.ExternalContentItem,
                        TreeWidgetViewItems.LaunchPad.ExternalContentItem
                    ];
                    var model = {
                        viewPath: 'LaunchpadItem',
                        viewModel: JSON.stringify(items),
                        viewModelType: 'System.Collections.Generic.List\`1[[Bfw.PX.PXPub.Models.TreeWidgetViewItem, Bfw.PX.PXPub.Models]]'
                    };
                    var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', model);
                    setFixtures(view);
                });
                describe("it", function () {
                    it('will have data-ud-islc equal to false', function () {
                        expect($(ui.xb_ecnode).attr('data-ud-islc') === 'False');
                        expect($(ui.lp_ecnode).attr('data-ud-islc') === 'False');
                    }),
                    it('will render with FNE link when FneOnlyLearningCurve is false', function () {
                        var hasFneLink = $(ui.lp_ecnode + ' a').attr('href').indexOf('FNE=True');
                        expect(hasFneLink > 1);
                    });
                    it('will NOT render with FNE link when FneOnlyLearningCurve is true', function () {
                        var noFneLink = $(ui.xb_ecnode + ' a').attr('href').indexOf('FNE=False');
                        expect(noFneLink > 1);
                    });
                });
            });
            //Test learning curve nodes
            describe("with learning curve items", function() {
                var ui = {
                    xb_lcnode: '.faux-tree-node[data-ud-id="xbook_lc"]',
                    lp_lcnode: '.faux-tree-node[data-ud-id="launchpad_lc"]'
                };
                beforeEach(function () {
                    var items = [
                        TreeWidgetViewItems.XBook.LearningCurveItem,
                        TreeWidgetViewItems.LaunchPad.LearningCurveItem
                    ];
                    var model = {
                        viewPath: 'LaunchpadItem',
                        viewModel: JSON.stringify(items),
                        viewModelType: 'System.Collections.Generic.List\`1[[Bfw.PX.PXPub.Models.TreeWidgetViewItem, Bfw.PX.PXPub.Models]]'
                    };
                    var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', model);
                    setFixtures(view);
                });
                describe("it", function () {
                    it('will have data-ud-islc equal to true', function() {
                        expect($(ui.xb_lcnode).attr('data-ud-islc') === 'True');
                        expect($(ui.lp_lcnode).attr('data-ud-islc') === 'True');
                    }),
                    it('will render with FNE link when FneOnlyLearningCurve is true', function () {
                        var hasFneLink = $(ui.xb_lcnode + ' a').attr('href').indexOf('FNE=True');
                        expect(hasFneLink > 1);
                    });
                    it('will render with FNE link when FneOnlyLearningCurve is false', function () {
                        var hasFneLink = $(ui.lp_lcnode + ' a').attr('href').indexOf('FNE=True');
                        expect(hasFneLink > 1);
                    });
                });
            });
        });
    });
    
    describe("_static.defaults.splitAssignedUnassigned", function () {
        it("can get unassigned items (from 'unassigned' section) if the value is TRUE", function () {
            // arrange
            var arrange = $.fn.GetArrangeLaunchpadSet1();

            var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
            var htmlDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                        {1} \
                        </div>", widgetId, domToc);

            setFixtures(htmlDoc);

            $("#launchpad-widget-" + widgetId).ContentTreeWidget({
                readOnly: true,
                showManagementCard: true,
                showExpandIcon: true,
                showExpandIconLevel: 1,
                splitAssignedUnassigned: true,
                toc: "syllabusfilter",
                assignmentToc: "",
                showAssignmentUnitFlow: false
            });

            // act
            var selectorClass = $.fn.ContentTreeWidgetObj()._static.sel.getUnassignedItems();

            // assert
            expect($(selectorClass).length).toBeGreaterThan(0);
        });

        it("can get unassigned items (from 'assignment' section) if the value is FALSE", function () {
            // arrange
            var arrange = $.fn.GetArrangeLaunchpadSet1();

            var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
            var htmlDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                        {1} \
                        </div>", widgetId, domToc);

            setFixtures(htmlDoc);

            $("#launchpad-widget-" + widgetId).ContentTreeWidget({
                readOnly: true,
                showManagementCard: true,
                showExpandIcon: true,
                showExpandIconLevel: 1,
                splitAssignedUnassigned: false,
                toc: "syllabusfilter",
                assignmentToc: "assignmentfilter",
                showAssignmentUnitFlow: true
            });

            // act
            var selectorClass = $.fn.ContentTreeWidgetObj()._static.sel.getUnassignedItems();

            // assert
            expect($(selectorClass).length).toBeGreaterThan(0);
        });
    });
});

Categories = {
    CatA: {
        Active: false,
        Id: "syllabusfilter",
        ItemParentId: 'PX_MULTIPART_LESSONS',
        Sequence: "a",
        Text: "Test Category A",
        Type: null
    }
};
TreeWidgetSettings = {
    XBook: {
        AllowDragDrop: true,
        AllowEditing: true,
        CourseId: '6698',
        DueLaterCount: 0,
        DueLaterDays: 14,
        EntityId: '12345',
        GreyoutPastDue: true,
        ShowCollapsedUnassigned: true,
        ShowDescription: false,
        ShowExpandIconAtAllLevels: true, /*** diff btwn lp and xb ***/
        ShowStudentDateData: true,
        SplitAssigned: true,
        SplitAssignedUnassigned: false,
        Title: "Unassigned",
        UserAccess: "Instructor",
        WidgetId: "PX_TOC",
        FneOnlyLearningCurve: true /*** diff btwn lp and xb ***/
    },
    Launchpad: {
        AllowDragDrop: true,
        AllowEditing: true,
        CourseId: '6698',
        DueLaterCount: 0,
        DueLaterDays: 14,
        EntityId: '12345',
        GreyoutPastDue: true,
        ShowCollapsedUnassigned: true,
        ShowDescription: false,
        ShowExpandIconAtAllLevels: false, /*** diff btwn lp and xb ***/
        ShowStudentDateData: true,
        SplitAssigned: true,
        SplitAssignedUnassigned: false,
        Title: "Unassigned",
        UserAccess: "Instructor",
        WidgetId: "PX_TOC",
        FneOnlyLearningCurve: false /*** diff btwn lp and xb ***/
    }
};
TreeWidgetViewItems = {
    XBook: {
        //In XBook we render learning curve items in FNE mode, and everything else in non-fne
        LearningCurveItem: {
            Level: 2,
            ParentId: "a_parent",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: '12345',
                Categories: [Categories.CatA],
                Container: "LaunchPad",
                DefaultCategoryParentId: 'abcd',
                Description: "Test Learning Curve",
                DisciplineId: '6698',
                Id: "xbook_lc",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: 'defg',
                Sequence: "a",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: "subcontainer_a"
                },
                //This is the stuff that makes the item a learning curve
                FacetMetaData: {
                    'meta-content-type': 'LearningCurve'
                },
                SyllabusFilter: 'syllabusfilter',
                Thumbnail: "image.png",
                Title: "Test Learning Curve",
                Type: "ExternalContent",
                Url: 'http://www.google.com',
                __type: "Bfw.PX.PXPub.Models.ExternalContent, Bfw.PX.PXPub.Models"
            },
            Settings: TreeWidgetSettings.XBook
        },
        ExternalContentItem: {
            Level: 2,
            ParentId: "a_parent",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: '12345',
                Categories: [Categories.CatA],
                Container: "LaunchPad",
                DefaultCategoryParentId: 'abcd',
                Description: "Test Learning Curve",
                DisciplineId: '6698',
                Id: "xbook_ec",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: 'defg',
                Sequence: "b",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: "subcontainer_a"
                },
                SyllabusFilter: 'syllabusfilter',
                Thumbnail: "image.png",
                Title: "Test Learning Curve",
                Type: "ExternalContent",
                Url: 'http://www.google.com/poo',
                __type: "Bfw.PX.PXPub.Models.ExternalContent, Bfw.PX.PXPub.Models"
            },
            Settings: TreeWidgetSettings.XBook
        }
    },
    LaunchPad: {
        //Clearly a lot of similarities between this and xbook.  We should make it easier to get these items
        LearningCurveItem: {
            Level: 2,
            ParentId: "a_parent",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: '12345',
                Categories: [Categories.CatA],
                Container: "LaunchPad",
                DefaultCategoryParentId: 'abcd',
                Description: "Test Learning Curve",
                DisciplineId: '6698',
                Id: "launchpad_lc",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: 'defg',
                Sequence: "a",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: "subcontainer_a"
                },
                //This is the stuff that makes the item a learning curve
                FacetMetaData: {
                    'meta-content-type': 'LearningCurve'
                },
                SyllabusFilter: 'syllabusfilter',
                Thumbnail: "image.png",
                Title: "Test Learning Curve",
                Type: "ExternalContent",
                Url: 'http://www.google.com',
                __type: "Bfw.PX.PXPub.Models.ExternalContent, Bfw.PX.PXPub.Models"
            },
            Settings: TreeWidgetSettings.Launchpad
        },
        ExternalContentItem: {
            Level: 2,
            ParentId: "a_parent",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: '12345',
                Categories: [Categories.CatA],
                Container: "LaunchPad",
                DefaultCategoryParentId: 'abcd',
                Description: "Test Learning Curve",
                DisciplineId: '6698',
                Id: "launchpad_ec",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: 'defg',
                Sequence: "b",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: "subcontainer_a"
                },
                SyllabusFilter: 'syllabusfilter',
                Thumbnail: "image.png",
                Title: "Test Learning Curve",
                Type: "ExternalContent",
                Url: 'http://www.google.com/poo',
                __type: "Bfw.PX.PXPub.Models.ExternalContent, Bfw.PX.PXPub.Models"
            },
            Settings: TreeWidgetSettings.Launchpad
        }
    }
};

(function ($) {
    var entityId = "57704";
    var disciplineId = "120559";
    var itemParentId = "PX_MULTIPART_LESSONS";

    //#region XbookV2

    $.fn.GetArrangeXbookSet1 = function () {
        //#region Arrange

        /* Categories */
        var categories = [];
        categories.push({
            Active: false,
            Id: "syllabusfilter",
            ItemParentId: itemParentId,
            Sequence: "a",
            Text: "Test Category A",
            Type: null
        });
        categories.push({
            Active: false,
            Id: "syllabusfilter",
            ItemParentId: itemParentId,
            Sequence: "b",
            Text: "Test Category B",
            Type: null
        });

        /* AssociatedTocItems */
        var tocSettings = {
            AllowDragDrop: true,
            AllowEditing: true,
            CourseId: disciplineId,
            DueLaterCount: 0,
            DueLaterDays: 14,
            EntityId: disciplineId,
            GreyoutPastDue: true,
            ShowAssignmentUnitWorkflow: true,
            ShowCollapsedUnassigned: true,
            ShowDescription: true,
            ShowExpandIconAtAllLevels: true, /*** difference between xbook and launchpad ***/
            ShowStudentDateData: true,
            SplitAssigned: true,
            SplitAssignedUnassigned: false,
            Title: "Unassigned",
            UserAccess: "Instructor",
            WidgetId: "PX_LAUNCHPAD_ASSIGNED_WIDGET",
            FneOnlyLearningCurve: true
        };

        var tocItems = [];
        tocItems.push({
            Level: 0,
            ParentId: "PX_MULTIPART_LESSONS",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: entityId,
                Categories: categories,
                Container: "LaunchPad",
                DefaultCategoryParentId: itemParentId,
                Description: "What is economics? In this first unit, we introduce the economic problem: there are not enough resources to satisfy our innumerable wants. We also examine the economic way of thinking. Ten key economic principles are examined. An important takeaway is this: people follow their incentives. Want less of an activity?—Tax it. Want more?—Subsidize it.",
                DisciplineId: disciplineId,
                Id: "MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: itemParentId,
                ReadOnly: false,
                Sequence: "a",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: ""
                },
                SyllabusFilter: itemParentId,
                Thumbnail: "/brainhoney/resource/6712/PX_Registry_Filestore/bsi__73571764__DFC7__46B7__8F0B__961A8D38BF5A/Stone_CE2e_P_1_01.png",
                Title: "Chapter 1. Exploring Economics ",
                Type: "PxUnit",
                __type: "Bfw.PX.PXPub.Models.PxUnit, Bfw.PX.PXPub.Models"
            },
            Settings: tocSettings
        });
        tocItems.push({
            Level: 0,
            ParentId: "PX_MULTIPART_LESSONS",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: entityId,
                Categories: categories,
                Container: "LaunchPad",
                DefaultCategoryParentId: itemParentId,
                Description: "We’ll look at economic growth as the key way to lift countries out of poverty. Production drives economic growth. The production possibilities model gives a simple way of thinking about production and growth. Trade is a way to use the division of labor and comparative advantage to foster growth.",
                DisciplineId: disciplineId,
                Id: "MODULE_bsi__5D1071D1__491B__4526__B519__0CF20E3CE365",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: itemParentId,
                ReadOnly: false,
                Sequence: "b",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: ""
                },
                SyllabusFilter: itemParentId,
                Thumbnail: "/brainhoney/resource/6712/PX_Registry_Filestore/bsi__B58D4874__3463__4D11__96DE__9A259823BE54/Stone_CE2e_P_2_01.png",
                Title: "Chapter 2. Production, Economic Growth, and Trade",
                Type: "PxUnit",
                __type: "Bfw.PX.PXPub.Models.PxUnit, Bfw.PX.PXPub.Models"
            },
            Settings: tocSettings
        });

        /* View Arrangement */
        var viewPath = "Launchpad";

        var viewModel = JSON.stringify({
            Items: tocItems,
            Settings: tocSettings
        });

        var viewModelType = "Bfw.PX.PXPub.Models.TreeWidgetRoot";

        var arrange = {
            viewPath: viewPath,
            viewModel: viewModel,
            viewModelType: viewModelType
        };

        //#endregion

        return arrange;
    };

    $.fn.GetArrangeManagementCardForXbook = function () {
        /* View Arrangement */
        var viewPath = "ManagementCard";

        var viewModel = JSON.stringify({
            Id: "",
            AssignTabSettings: {
                ShowMakeGradable: false,
                ShowGradebookCategory: false,
                ShowIncludeScore: false,
                ShowCalculationType: false
            },
            GradeBookWeights: {
                GradeWeightCategories: null
            },
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
            }
        };
        
        var arrange = {
            viewPath: viewPath,
            viewModel: viewModel,
            viewModelType: viewModelType,
            viewData: viewData
        };

        return arrange;
    };

    //#endregion XbookV2

    //#region Launchpad

    $.fn.GetArrangeLaunchpadSet1 = function () {
        //#region Arrange

        /* Categories */
        var categories = [];
        categories.push({
            Active: false,
            Id: "syllabusfilter",
            ItemParentId: itemParentId,
            Sequence: "a",
            Text: "Test Category A",
            Type: null
        });
        categories.push({
            Active: false,
            Id: "syllabusfilter",
            ItemParentId: itemParentId,
            Sequence: "b",
            Text: "Test Category B",
            Type: null
        });

        /* AssociatedTocItems */
        var tocSettings = {
            AllowDragDrop: true,
            AllowEditing: true,
            CourseId: disciplineId,
            DueLaterCount: 0,
            DueLaterDays: 14,
            EntityId: disciplineId,
            GreyoutPastDue: true,
            ShowAssignmentUnitWorkflow: false,
            ShowCollapsedUnassigned: true,
            ShowDescription: true,
            ShowExpandIconAtAllLevels: false, /*** difference between xbook and launchpad ***/
            ShowStudentDateData: true,
            SplitAssigned: true,
            SplitAssignedUnassigned: false,
            Title: "Unassigned",
            UserAccess: "Instructor",
            WidgetId: "PX_LAUNCHPAD_ASSIGNED_WIDGET"
        };

        var tocItems = [];
        tocItems.push({
            Level: 0,
            ParentId: "PX_MULTIPART_LESSONS",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: entityId,
                Categories: categories,
                Container: "LaunchPad",
                DefaultCategoryParentId: itemParentId,
                Description: "What is economics? In this first unit, we introduce the economic problem: there are not enough resources to satisfy our innumerable wants. We also examine the economic way of thinking. Ten key economic principles are examined. An important takeaway is this: people follow their incentives. Want less of an activity?—Tax it. Want more?—Subsidize it.",
                DisciplineId: disciplineId,
                Id: "MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: itemParentId,
                ReadOnly: false,
                Sequence: "a",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: ""
                },
                SyllabusFilter: itemParentId,
                Thumbnail: "/brainhoney/resource/6712/PX_Registry_Filestore/bsi__73571764__DFC7__46B7__8F0B__961A8D38BF5A/Stone_CE2e_P_1_01.png",
                Title: "Chapter 1. Exploring Economics ",
                Type: "PxUnit",
                __type: "Bfw.PX.PXPub.Models.PxUnit, Bfw.PX.PXPub.Models"
            },
            Settings: tocSettings
        });
        tocItems.push({
            Level: 0,
            ParentId: "PX_MULTIPART_LESSONS",
            Path: "/",
            TOC: "syllabusfilter",
            Item: {
                ActualEntityid: entityId,
                Categories: categories,
                Container: "LaunchPad",
                DefaultCategoryParentId: itemParentId,
                Description: "We’ll look at economic growth as the key way to lift countries out of poverty. Production drives economic growth. The production possibilities model gives a simple way of thinking about production and growth. Trade is a way to use the division of labor and comparative advantage to foster growth.",
                DisciplineId: disciplineId,
                Id: "MODULE_bsi__5D1071D1__491B__4526__B519__0CF20E3CE365",
                InstructorCommentsAccess: "ShowOnlyToSubmittingStudent",
                ParentId: itemParentId,
                ReadOnly: false,
                Sequence: "b",
                SubContainerIds: {
                    DlapType: "exact",
                    Toc: "syllabusfilter",
                    Value: ""
                },
                SyllabusFilter: itemParentId,
                Thumbnail: "/brainhoney/resource/6712/PX_Registry_Filestore/bsi__B58D4874__3463__4D11__96DE__9A259823BE54/Stone_CE2e_P_2_01.png",
                Title: "Chapter 2. Production, Economic Growth, and Trade",
                Type: "PxUnit",
                __type: "Bfw.PX.PXPub.Models.PxUnit, Bfw.PX.PXPub.Models"
            },
            Settings: tocSettings
        });

        /* View Arrangement */
        var viewPath = "Launchpad";

        var viewModel = JSON.stringify({
            Items: tocItems,
            Settings: tocSettings
        });

        var viewModelType = "Bfw.PX.PXPub.Models.TreeWidgetRoot";

        var arrange = {
            viewPath: viewPath,
            viewModel: viewModel,
            viewModelType: viewModelType
        };

        //#endregion

        return arrange;
    };

    $.fn.GetArrangeManagementCardForLaunchpad = function () {
        /* View Arrangement */
        var viewPath = "ManagementCard";

        var viewModel = JSON.stringify({
            Id: "",
            AssignTabSettings: {
                ShowMakeGradable: true,
                ShowGradebookCategory: true,
                ShowIncludeScore: true,
                ShowCalculationType: true
            },
            GradeBookWeights: {
                GradeWeightCategories: null
            },
            Score: {
                Correct: 0,
                Possible: 0
            },
            ShowAssignmentUnitWorkflow: false,
            SourceType: "",
            Type: ""
        });

        var viewModelType = "Bfw.PX.PXPub.Models.AssignedItem";

        var arrange = {
            viewPath: viewPath,
            viewModel: viewModel,
            viewModelType: viewModelType
        };

        return arrange;
    };

    //#endregion Launchpad
}(jQuery));