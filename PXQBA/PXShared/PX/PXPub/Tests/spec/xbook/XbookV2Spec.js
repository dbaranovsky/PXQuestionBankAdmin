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
///    <reference path="../../../Scripts/ContentWidget/ContentAreaWidget.js"/>
///    <reference path="../../../Scripts/Xbook/XbookV2.js"/>

describe("XbookV2:", function () {
    var widgetId = "PX_LAUNCHPAD_ASSIGNED_WIDGET";
    var itemParentId = "PX_MULTIPART_LESSONS";

    var htmlXbookDoc = "";

    var testNodeId = "";
    var openedNodeFixture = "";

    var liNode = $.fn.fauxtreeObj()._static.sel.node; // faux-tree-node
    var titleSelector = $.fn.fauxtreeObj()._static.sel.nodeTitle; // faux-tree-node-title
    var iconSelector = $.fn.fauxtreeObj()._static.sel.nodeIcon; // .icon

    //window.PxPage this has been moved to the HTML file because it needs to be declared before XBookV2 is loaded.
    PxXbookV2.Init();
    window._ = PxXbookV2.PrivateTest();
    window.toastr = {
        success: function () {}
    };

    describe('tab switching', function () {
        var foo = {
            mockcallback: function () {

            }
        };

        beforeEach(function() {
            var data = {
                viewPath: 'Summary',
                viewModel: JSON.stringify({
                    MenuItems: [{
                            Id: 'PX_MENU_ITEM_XBOOK',
                            Properties: 
                            {
                                'bfw_usehash': {
                                    Value: true
                                },
                                'bfw_hashcomponent': {
                                    Value: 'xbook'
                                }
                            },
                            Parameters: {
                                pageDefnId: 'PX_XBOOK_TAB'
                            },
                            Url: 'blah',
                            Target: '#container_PX_ContainerWidget_XBook',
                            Controller: 'Home',
                            Action: 'LoadPageDefinition'
                        }, {
                            Id: 'PX_MENU_ITEM_ASSIGNMENTS',
                            Properties: 
                            {
                                'bfw_usehash': {
                                    Value: true
                                },
                                'bfw_hashcomponent': {
                                    Value: 'assignments'
                                }
                            },
                            Parameters: { pageDefnId: 'PX_XBOOK_ASSIGNMENTS' },
                            Url: 'blah',
                            Target: '#container_PX_ContainerWidget_XBook',
                            Controller: 'Home',
                            Action: 'LoadPageDefinition'
                        }, {
                            Id: 'PX_MENU_ITEM_GRADES',
                            Properties: 
                            {
                                'bfw_usehash': {
                                    Value: true
                                },
                                'bfw_hashcomponent': {
                                    Value: 'gradebook'
                                }
                            },
                            Parameters: { pageDefnId: 'PX_XBOOK_GRADEBOOK' },
                            Url: 'blah',
                            Target: '#container_PX_ContainerWidget_XBook',
                            Controller: 'Home',
                            Action: 'LoadPageDefinition'
                        },
                        {
                            Id: 'NO_TARGET',
                            Properties:
                            {
                                'bfw_usehash': {
                                    Value: true
                                },
                                'bfw_hashcomponent': {
                                    Value: 'gradebook'
                                }
                            },
                            Parameters: { pageDefnId: 'PX_XBOOK_GRADEBOOK' },
                            Url: 'blah',
                            Target: '',
                            Controller: 'Home',
                            Action: 'LoadPageDefinition'
                        }]
                }),
                viewModelType: 'Bfw.PX.PXPub.Models.Menu'
            };

            var view = PxViewRender.RenderView('PXPub', 'MenuWidget', data);

            //Can't test this until we get mocking of ajax working
            //view = view + '<div id="container_PX_ContainerWidget_XBook"></div>'
            setFixtures(view);
            PxXbookV2.PrivateTest().itemFocused($('#PX_MENU_ITEM_XBOOK'));
        });

        it('won\'t load tabs with the same id twice', function () {
            spyOn(foo, 'mockcallback');
            PxXbookV2.PrivateTest().changeTabs($('#PX_MENU_ITEM_XBOOK'), foo.mockcallback);
            expect(foo.mockcallback).not.toHaveBeenCalled();
        });
        
        it('won\'t load tab if missing target', function () {
            spyOn(foo, 'mockcallback');
            PxXbookV2.PrivateTest().changeTabs($('#NO_TARGET'), foo.mockcallback);
            expect(foo.mockcallback).not.toHaveBeenCalled();
        });
        
        it('will load tabs that arn\'t active', function () {
            spyOn(foo, 'mockcallback');
            PxXbookV2.PrivateTest().changeTabs($('#PX_MENU_ITEM_ASSIGNMENTS'), foo.mockcallback);
            expect(foo.mockcallback).toHaveBeenCalled();
        });
        
        it('will set changed tab to active', function () {
            spyOn(foo, 'mockcallback');
            PxXbookV2.PrivateTest().changeTabs($('#PX_MENU_ITEM_ASSIGNMENTS'));
            expect($('#PX_MENU_ITEM_ASSIGNMENTS').hasClass('active'))
        });

        xit('will keep the item url in synch with node', function () {

            // fakers
            window.Modernizr = {};
            window.get_cookie = function () {};

            // load toc
            var arrange = $.fn.GetArrangeXbookSet1();
            var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
            htmlLaunchpadDoc = $.format("<div class='widgetItem {0}' id='{0}' itemid='{0}'> \ {1} \ </div>", widgetId, domToc);
            setFixtures(htmlLaunchpadDoc);

            var link = $('#launchpad-widget-' + widgetId).find('.faux-tree-link');
            
            // make sure a link is in dom
            expect(link.length).not.toEqual(0);

            // grab item from link parent
            _.router.id = link.parents('li').eq(0).attr('data-ud-id');

            spyOn($.fn.fauxtreeObj().api, 'openNode');
            spyOn($.fn.fauxtreeObj().api, 'setActiveNode');
            spyOn(hasher, 'setHash').andCallThrough();

            // assert synch
            _.synchToc();

            // check results for an id being present
            expect($.fn.fauxtreeObj().api.openNode).toHaveBeenCalled();
            expect($.fn.fauxtreeObj().api.setActiveNode).toHaveBeenCalled();
            expect(window.location.hash.split('/')[3].split('?mode=')[0]).toEqual(_.router.id);
            expect(hasher.setHash).toHaveBeenCalledWith(link.attr('href'));

            // for undefined id
            $.fn.fauxtreeObj().api.openNode.reset();
            $.fn.fauxtreeObj().api.setActiveNode.reset();
            window.location.hash = '';

            _.router.id = void 0;
            _.synchToc();

            expect($.fn.fauxtreeObj().api.openNode).not.toHaveBeenCalled();
            expect($.fn.fauxtreeObj().api.setActiveNode).not.toHaveBeenCalled();
            expect(window.location.hash).toEqual('');

        });

    });

    describe('can handle', function() {
        it('a successful html quiz submit and send a success toast', function() {
            spyOn(PxPage.Toasts, 'Success');

            $(PxPage.switchboard).trigger('htmlquiz-submit-complete', ['success']);

            expect(PxPage.Toasts.Success).toHaveBeenCalled();
        });
        it('a successful html quiz submit and reload the content', function () {
            spyOn(_, 'reloadContent');

            $(PxPage.switchboard).trigger('htmlquiz-submit-complete', ['success']);

            expect(_.reloadContent).toHaveBeenCalled();
        });
        it('a bad html quiz submit and send an error toast', function () {
            spyOn(PxPage.Toasts, 'Error');

            $(PxPage.switchboard).trigger('htmlquiz-submit-complete', ['ERROR']);

            expect(PxPage.Toasts.Error).toHaveBeenCalled();
        });
        it("close of a FNE window when getCurrentNode has data-ud-islc", function () {
            var fakeNode = "<div id='test' data-ud-islc='true'></div>";
            spyOn(PxXbookV2, "getCurrentNode").andReturn($(fakeNode));
            spyOn(_, "synchToc");

            var fakeUrl = "#this is docviewer url";
            window._._docViewerUrl = fakeUrl;

            _._ui.vent.trigger('fneclosed');

            expect(window.location.href).toContain(fakeUrl);
            expect(_.synchToc).toHaveBeenCalled();
        });
    });
    
    describe('content navigation', function () {
        var previousButtonEvent = 'fneclickPreviousNodeTitle.launchpad',
        nextButtonEvent = 'fneclickNextNodeTitle.launchpad',
        helper = {
            
            getTocView: function () {
                var arrange = $.fn.GetArrangeXbookSet1();
                domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);

                var temp = $('<div id="temp"></div>').appendTo('body');

                // need to mock a xbookv2 toc link
                temp.html(domToc)
                    .find('.faux-tree-node')
                    .attr('data-ft-chapter', 'MODULE_bsi__80FB0BED__2F85__46BC__9EE4__4BB9D1D4EB28')
                    .html('<div class="faux-tree-node-title col faceplate-hover-div showaslink"><div class="fp-img-wrapper"><span class="fpimage"></span></div><span class="icon-placeholder"></span><span class="fptitle"><a href="#state/item/MODULE_bsi__80FB0BED__2F85__46BC__9EE4__4BB9D1D4EB28/bsi__B5A77DF8__A02F__4E52__81C2__D51630832767?mode=Preview&amp;getChildrenGrades=True&amp;includeDiscussion=False&amp;renderFNE=false&amp;toc=syllabusfilter" class="faux-tree-link">Practicing The Genre: Explaining an Academic Concept</a></span><span class="item_subtitle"></span></div>');

                return temp;
            },

            getFneNavView: function () {
                var jsView = '<div id="fne-window"><span id="nav-container"><span id="content-fullscreen" class="xb-btn"></span>'
                            + '<span id="back" class="navigate-back">'
                            + '<span class="navbtn-back-icon"></span></span>'
                            + '<span id="next" class="navigate-next"><span class="navbtn-next-icon"></span></span></span></div>';
                var toc = helper.getTocView();
                $(toc).append(jsView);
                setFixtures(toc);
            },

            getFaceplateNavView: function () {
                var view = PxViewRender.RenderView('PXPub', 'ContentAreaWidget', { viewPath: 'ContentArea' });
                setFixtures(view);
            }

        };
        
        it('initializes and updates the navigation', function () {

            spyOn(PxXbookV2.contentNav.staticTest, '_updateNavigation');
            PxXbookV2.contentNav.init();
            
            // publish initial load
            expect(PxXbookV2.contentNav.staticTest._updateNavigation).toHaveBeenCalled();

        });

        it('will open first toc item (for unit with content)', function () {

            helper.getTocView();

            var node = $('.faux-tree-node').eq(0),
                link = node.find('.faux-tree-link');

            window.location.hash = '';
            _.loadFirstTocItem(node);

            expect(window.location.hash).toEqual(link.attr('href'));

        });

        it('will open first toc item (for unit without content)', function () {

            helper.getTocView();

            var node = $('.faux-tree-node').eq(0);
            node.find('.faux-tree-link').remove();

            spyOn($.fn.fauxtreeObj().api, 'openNode');
            _.loadFirstTocItem(node);

            expect($.fn.fauxtreeObj().api.openNode).toHaveBeenCalledWith(node);

        });

        describe('updates navigation controls', function() {
            it('when contentloaded is fired', function() {
                spyOn(PxXbookV2, 'getNextNode').andReturn(function() {
                    return $('.nothing');
                });
                spyOn(PxXbookV2, 'getPrevNode').andReturn(function () {
                    return $('.nothing');
                });

                PxXbookV2.vent.trigger('contentloaded');

                expect(PxXbookV2.getNextNode).toHaveBeenCalled();
                expect(PxXbookV2.getPrevNode).toHaveBeenCalled();
            });
            it('when onNodesLoaded is fired if the selected content is an unloaded unit', function () {
                var isNotLoaded = true;
                spyOn(PxXbookV2, 'getNextNode').andReturn(function () {
                    return $('.nothing');
                });
                spyOn(PxXbookV2, 'getPrevNode').andReturn(function () {
                    return $('.nothing');
                });
                spyOn($.fn, 'fauxtree').andCallFake(function(operation, args) {
                    if (operation === 'isNotLoaded') {
                        return isNotLoaded;
                    }
                    return false;
                });

                PxXbookV2.vent.trigger('contentloaded');

                expect(PxXbookV2.getNextNode).not.toHaveBeenCalled();
                expect(PxXbookV2.getPrevNode).not.toHaveBeenCalled();

                isNotLoaded = false;
                PxXbookV2.vent.trigger('onNodesLoaded');
                
                expect(PxXbookV2.getNextNode).toHaveBeenCalled();
                expect(PxXbookV2.getPrevNode).toHaveBeenCalled();
            });
        });
        
        describe('xb doc viewer nav', function () {

            beforeEach(function () {
                helper.getFaceplateNavView();
                PxXbookV2.vent.trigger('contentloaded');
            });

            it('publishes a previous node call to `ContentTreeWidget` once the prev button is clicked', function () {
                var result = '';
                var expectedVal = PxXbookV2.contentNav.staticTest._validPrevChapterNode();
                $(PxPage.switchboard).bind(previousButtonEvent, function(event, selector) {
                    result = selector;
                });
                $('#content-back').removeClass('disabled').click();
                expect(result).toBe(expectedVal);
            });

            it('does not publish a previous node call to `ContentTreeWidget` once the prev button is clicked and is disabled', function () {
                var result = true;
                $(PxPage.switchboard).bind(previousButtonEvent, function(event, selector) {
                    result = false;
                });
                $('#content-back').addClass('disabled').click();
                expect(result).toBeTruthy();
            });

            it('publishes a next node call to `ContentTreeWidget` once the next button is clicked', function () {
                var result = '';
                var expectedVal = PxXbookV2.contentNav.staticTest._validNextChapterNode();
                $(PxPage.switchboard).bind(nextButtonEvent, function(event, selector) {
                    result = selector;
                });
                $('#content-fwd').removeClass('disabled').click();
                expect(result).toBe(expectedVal);
            });

            it('does not publish a next node call to `ContentTreeWidget` once the next button is clicked and is disabled', function () {
                var result = true;
                $(PxPage.switchboard).bind(nextButtonEvent, function(event, selector) {
                    result = false;
                });
                $('#content-fwd').addClass('disabled').click();
                expect(result).toBeTruthy();
            });

        });

        describe('fne nav', function () {

            beforeEach(function () {
                helper.getFneNavView();
                PxXbookV2.vent.trigger('contentloaded');
            });

            it('publishes a previous node call to `ContentTreeWidget` once the prev button is clicked in fne mode', function () {
                var result = '';
                $('li').removeClass('hide-in-fne');
                $('li.item-type-htmlquiz').addClass('last-viewed');
                var expectedVal = PxXbookV2.contentNav.staticTest._validPrevChapterNode();
                $(PxPage.switchboard).bind(previousButtonEvent, function(event, selector) {
                    result = selector;
                });
                $('#back').removeClass('disabled').click();
                expect(result).toBe(expectedVal)
            });

            it('does not publish a previous node call to `ContentTreeWidget` once the prev button is clicked and it is disabled in fne mode', function () {
                var result = true;
                $(PxPage.switchboard).bind(previousButtonEvent, function(event, selector) {
                    result = false;
                });
                $('#back').addClass('disabled').click();
                expect(result).toBeTruthy();
            });

            it('publishes a next node call to `ContentTreeWidget` once the next button is clicked in fne mode', function () {
                var result = '';
                $('li.item-type-pxunit').addClass('last-viewed');
                var expectedVal = PxXbookV2.contentNav.staticTest._validNextChapterNode();
                $(PxPage.switchboard).bind(nextButtonEvent, function(event, selector) {
                    result = selector;
                });
                $('#next').removeClass('disabled').click();
                expect(result).toBe(expectedVal);
            });

            it('does not publish a next node call to `ContentTreeWidget` once the next button is clicked and it is disabled in fne mode', function () {
                var result = true;
                $(PxPage.switchboard).bind(nextButtonEvent, function (event, selector) {
                    result = false;
                });
                $('#next').addClass('disabled').click();
                expect(result).toBeTruthy();
            });

            it('can replace all cases `renderFNE=true` in url', function () {

                var params = ['FNE=true', 'Fne=True', 'fne=TRUE'];

                params.forEach(function (value, index) {
                    window.location.hash = value;
                    _._fneOpened();
                    expect( _._fneUrl.substring(_._fneUrl.indexOf('#') + 1) ).toEqual('FNE=False');
                });

            });

            it('can add a node\'s content type as a class to fne window', function () {
                var node = $('.faux-tree-node').eq(0).addClass('active'),
                    contentType = node.data('ud-itemtype');
                _._ui.vent.trigger('fneloaded.xbookv2');
                expect( $(_._ui.fneWindow).hasClass(contentType) ).toBeTruthy();
            });

        });
    });

    describe('content creation', function () {
        describe('on xbook tab', function() {
            beforeEach(function() {
                _._state = 'xbook';
            });
            it("will save assignment unit", function() {
                var content = {
                    contentType: "PXUNIT",
                    unitType: "AssignmentUnit",
                    id: "this is an item id",
                    name: "this is a title",
                    toc: "syllabusfilter"
                };

                spyOn(_, "saveNewAssignmentUnit");

                _._ui.vent.trigger('contentcreated', [content]);

                expect(_.saveNewAssignmentUnit).toHaveBeenCalled();
            });
            it("will not save pxunit", function() {
                var content = {
                    contentType: "PXUNIT",
                    id: "this is an item id",
                    name: "this is a title",
                    toc: "syllabusfilter"
                };

                spyOn(_, "saveNewAssignmentUnit");

                _._ui.vent.trigger('contentcreated', [content]);

                expect(_.saveNewAssignmentUnit).not.toHaveBeenCalled();
            });
            it("will not save non unit type", function () {
                var content = {
                    contentType: "HtmlDocument",
                    id: "this is an item id",
                    name: "this is a title",
                    toc: "syllabusfilter"
                };

                spyOn(_, "saveNewAssignmentUnit");
            
                _._ui.vent.trigger('contentcreated', [content]);

                expect(_.saveNewAssignmentUnit).not.toHaveBeenCalled();
            });
        });
        describe('on assignment tab', function() {
            beforeEach(function () {
                _._state = 'assignments';
            });
            it('will not save assignment unit', function() {
                var content = {
                    contentType: "PXUNIT",
                    unitType: "AssignmentUnit",
                    id: "this is an item id",
                    name: "this is a title",
                    toc: "assignmentfilter"
                };

                spyOn(_, "saveNewAssignmentUnit");

                _._ui.vent.trigger('contentcreated', [content]);

                expect(_.saveNewAssignmentUnit).not.toHaveBeenCalled();
            });
            it('will not save any content type', function () {
                var content = {
                    contentType: "HtmlDocument",
                    id: "this is an item id",
                    name: "this is a title",
                    toc: "assignmentfilter"
                };

                spyOn(_, "saveNewAssignmentUnit");

                _._ui.vent.trigger('contentcreated', [content]);

                expect(_.saveNewAssignmentUnit).not.toHaveBeenCalled();
            });
            it('will set new assignment unit to the _unitCreated state ', function () {
                var content = {
                    contentType: "PxUnit",
                    unitType: "AssignmentUnit",
                    id: "this is an item id",
                    name: "this is a title",
                    toc: "assignmentfilter"
                };
                _._ui.vent.trigger('contentcreated', [content]);

                expect(_._unitCreated).toEqual(content);
            });
            it('will create new gradebook category after content created and saved', function() {
                var content = {
                    contentType: "PxUnit",
                    unitType: "AssignmentUnit",
                    id: "this is an item id",
                    name: "this is a title",
                    toc: "assignmentfilter"
                };
                spyOn(_, "createGradebookCategory");
                
                _._ui.vent.trigger('contentcreated', [content]);
                _._ui.vent.trigger('contentsaved');
                
                expect(_.createGradebookCategory).toHaveBeenCalledWith(content);
            });
        });
    });

    describe('content save', function() {
        describe('after new unit created', function () {
            var newUnit = {
                contentType: "PXUNIT",
                unitType: "AssignmentUnit",
                id: "this is an item id",
                name: "this is a title",
                toc: "assignmentfilter"
            };
            beforeEach(function() {
                _._unitCreated = newUnit;
            });
            it('should create new gradebook category with new unit', function() {
                spyOn(_, 'createGradebookCategory');
                $(PxPage.switchboard).trigger('contentsaved');

                expect(_.createGradebookCategory).toHaveBeenCalledWith(newUnit);
            });
            it('should clear unitCreated state', function() {
                $(PxPage.switchboard).trigger('contentsaved');
                expect(_._unitCreated).toBeUndefined();
            });
        });
        describe('with no new unit', function() {
            it('shouldn\'t create a new gradebook category', function() {
                spyOn(_, 'createGradebookCategory');
                $(PxPage.switchboard).trigger('contentsaved');
                expect(_.createGradebookCategory).not.toHaveBeenCalled();
            });
        });
    });

    describe('content load', function () {
        
        it('doesn\'t make item gradable if overload due date requirement is false', function() {
            setFixtures('<div id="content-item"> <input id="OverrideDueDateReq" value="false"/></div>')
            spyOn(ContentWidget, 'MakeContentItemGradable');
            _.checkGradability();

            expect(ContentWidget.MakeContentItemGradable).not.toHaveBeenCalled();
        });
        it('doesn\'t make item gradable if overload due date requirement is not present', function () {
            spyOn(ContentWidget, 'MakeContentItemGradable');
            _.checkGradability();

            expect(ContentWidget.MakeContentItemGradable).not.toHaveBeenCalled();
        });
        
        describe('and content overloads due date req', function () {
            var contentId = 'contentId';
            beforeEach(function () {
                
                //Default the fixture to having the override and all fields set correctly
                setFixtures('<div id="content-item"> <span id="content-item-id">' + contentId + '</span> \
                <input id="Content_BHParentId" name="Content.BHParentId" type="hidden" value="PX_MANIFEST"> \
                <input id="Content_IsGradable" name="Content.IsGradable" type="hidden" value="True"> \
                <input id="Content_MaxPoints" name="Content.MaxPoints" type="hidden" value="10"> \
                <input id="Content_OverrideDueDateReq" name="Content.OverrideDueDateReq" type="hidden" value="True"></div>');
                spyOn(ContentWidget, 'MakeContentItemGradable');
            });

            it('will not make the item gradable if all fields are set correctly', function () {
                _.checkGradability();
                expect(ContentWidget.MakeContentItemGradable).not.toHaveBeenCalled();
            });
            
            it('will not make the item gradable parentid is not there', function () {
                $('#Content_BHParentId').remove();
                _.checkGradability();
                expect(ContentWidget.MakeContentItemGradable).not.toHaveBeenCalled();
            });
            
            it('will not make the item gradable isGradable is not there', function () {
                $('#Content_IsGradable').remove();
                _.checkGradability();
                expect(ContentWidget.MakeContentItemGradable).not.toHaveBeenCalled();
            });
            
            it('will not make the item gradable maxPoints is not there', function () {
                $('#Content_MaxPoints').remove();
                _.checkGradability();
                expect(ContentWidget.MakeContentItemGradable).not.toHaveBeenCalled();
            });
            
            it('will make the item gradable if parentid is not PX_MANIFEST', function () {
                $('#Content_BHParentId').val('ParentId');
                _.checkGradability();
                expect(ContentWidget.MakeContentItemGradable).toHaveBeenCalledWith(contentId, true);
            });
            
            it('will make the item gradable if isGradable is not true', function () {
                $('#Content_IsGradable').val('FALSE');
                _.checkGradability();
                expect(ContentWidget.MakeContentItemGradable).toHaveBeenCalledWith(contentId, true);
            });
            
            it('will make the item gradable if maxPoints is less than 1', function () {
                $('#Content_MaxPoints').val('0');
                _.checkGradability();
                expect(ContentWidget.MakeContentItemGradable).toHaveBeenCalledWith(contentId, true);
            });
        });
    });

    describe("ADD menu button", function () {
        beforeEach(function () {
            var arrange = $.fn.GetArrangeXbookSet1("Index");

            var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
            htmlXbookDoc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                        {1} \
                        </div>", widgetId, domToc);

            setFixtures(htmlXbookDoc);
        });

        it("can call _loadBrowseMoreResources on clicking 'Browse more resources' link menu", function () {
            $.fn.FacePlateBrowseMoreResources = function (func, mode, label) { };
            spyOn($.fn, "FacePlateBrowseMoreResources");

            $(_._ui.browseResourcesLink).click();

            expect($.fn.FacePlateBrowseMoreResources).toHaveBeenCalledWith('showMoreResourcesWindow', 'none', 'coming soon...');
        });
    });
});

(function ($) {
    var entityId = "57704";
    var disciplineId = "120559";
    var itemParentId = "PX_MULTIPART_LESSONS";

    $.fn.GetArrangeXbookSet1 = function(optViewPath) {
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
            Level: 1,
            ParentId: 'PX_MULTIPART_LESSONS',
            Path: '/',
            TOC: 'syllabusfilter',
            Item: {
                ActualEntityid: entityId,
                Categories: categories,
                Container: 'LaunchPad',
                DefaultCategoryParentId: itemParentId,
                Description: 'Lipsum',
                DisciplineId: disciplineId,
                Id: 'MODULE_bsi__3DFEAA19__18E9__4F4C__80E4__958C3F567574',
                InstructorCommentsAccess: 'ShowOnlyToSubmittingStudent',
                ParentId: itemParentId,
                Sequence: 'a',
                SubContainerIds: {
                    DlapType: 'exact',
                    Toc: 'syllabusfilter',
                    Value: ''
                },
                SyllabusFilter: itemParentId,
                Thumbnail: '',
                Title: 'PART 1 Writing Activities',
                Type: 'PxUnit',
                Url: '#state/item/megatron',
                __type: 'Bfw.PX.PXPub.Models.PxUnit, Bfw.PX.PXPub.Models'
            },
            Settings: tocSettings
        });

        tocItems.push({
            Level: 2,
            ParentId: 'MODULE_bsi__80FB0BED__2F85__46BC__9EE4__4BB9D1D4EB28',
            Path: '/',
            TOC: 'syllabusfilter',
            Item: {
                ActualEntityid: entityId,
                Categories: categories,
                Container: 'LaunchPad',
                DefaultCategoryParentId: itemParentId,
                Description: 'Lipsum',
                DisciplineId: disciplineId,
                Id: 'bsi__AD79E682__A704__43EF__AA10__74147727888E',
                InstructorCommentsAccess: 'ShowOnlyToSubmittingStudent',
                ParentId: itemParentId,
                Sequence: 'a',
                SubContainerIds: {
                    DlapType: 'exact',
                    Toc: 'syllabusfilter',
                    Value: ''
                },
                SyllabusFilter: itemParentId,
                Thumbnail: '',
                Title: 'PART 1 Writing Activities',
                Type: 'htmlquiz',
                Url: '#state/item/MODULE_bsi__80FB0BED__2F85__46BC__9EE4__4BB9D1D4EB28/bsi__70FA5C76__307E__4E2C__9745__DE506412D134?mode=Preview&amp;getChildrenGrades=False&amp;includeDiscussion=False&amp;renderFNE=False&amp;toc=syllabusfilter',
                __type: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
            },
            Settings: tocSettings
        });

        /* View Arrangement */
        var viewPath = (optViewPath)? optViewPath : "Launchpad";

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
}(jQuery));

jasmine.Spy.prototype.reset = function () { 
    this.wasCalled = false;
    this.callCount = 0;
    this.argsForCall = [];
    this.calls = [];
    this.mostRecentCall = {};
};

if (window._phantom) {
  // Patch since PhantomJS does not implement click() on HTMLElement. In some 
  // cases we need to execute the native click on an element. However, jQuery's 
  // $.fn.click() does not dispatch to the native function on <a> elements, so we
  // can't use it in our implementations: $el[0].click() to correctly dispatch.
  if (!HTMLElement.prototype.click) {
    HTMLElement.prototype.click = function() {
      var ev = document.createEvent('MouseEvent');
      ev.initMouseEvent(
          'click', true, true, window, null, 0, 0, 0, 0, false, false, false, false, 0, null
      );
      this.dispatchEvent(ev);
    };
  }
}
