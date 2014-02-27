/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" data-cover-adapter="../../lib/blanket/jasmine-blanket.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.validate.min.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Scripts/jquery/jquery.fauxtree.js"/>
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

/// <reference path="../../../Scripts/ManagementCard/ManagementCard.js" />
/// <reference path="../../../Scripts/faceplate/pxfaceplate.js" />
/// <reference path="../../../Scripts/ContentTreeWidget/ContentTreeWidget.js" />

describe("PXFacePlate:", function () {
    describe("Core, ", function () {
        beforeAll(function () {
            setFixtures('<div id="bsi__8707DAAF__3D2F__46CD__849A__3BC5B13BAC8E"><span class="fptitle"><a></a></span></div>');
            var cb = function (id) {
                return $('#' + id);
            };
            BaseTestHelper.spy.spyOnFauxtree({ getNode: cb });
        });
        it("After UpdateItemAfterRename(), the link should not be removed", function () {
            var response = 'bsi__8707DAAF__3D2F__46CD__849A__3BC5B13BAC8E|LearningCurve: 12a. Theories of Emotion and Embodied Emotion22333|||';

            $.fn.ContentTreeWidget('updateItemAfterRename', response, false);

            expect($('#bsi__8707DAAF__3D2F__46CD__849A__3BC5B13BAC8E .fptitle a').html()).toEqual('LearningCurve: 12a. Theories of Emotion and Embodied Emotion22333');
        });
    });
    describe("ADD menu button", function () {
        beforeEach(function () {
            var widgetId = "PX_LAUNCHPAD_ASSIGNED_WIDGET";
            
            var arrange = $.fn.GetLaunchpadSet1("Index");

            var domToc = pxRenderingHelper.controller.generateWidgetModel('PXPub', 'LaunchPadTreeWidget', arrange);
            var doc = $.format("<div class='widgetItem {0}' silentcreation='true' id='{0}' itemid='{0}'> \
                        {1} \
                        </div>", widgetId, domToc);

            setFixtures(doc);
        });

        it("can call _loadBrowseMoreResources on clicking 'Browse more resources' link menu", function () {
            PxPage.LargeFNE = {
                Init: function() {
                }
            };
            PxPage.switchboard = $(document);
            spyOn(PxPage, "OnProductLoaded");
            
            var browseLink = '.faceplate-nav .faceplate-add-content-menu #browse';
            $.fn.FacePlateBrowseMoreResources = function (func, mode, label) { };
            spyOn($.fn, "FacePlateBrowseMoreResources");

            spyOn(PxFacePlate, "InitRoutes");
            
            PxFacePlate.Init();

            $(browseLink).click();

            expect($.fn.FacePlateBrowseMoreResources).toHaveBeenCalledWith('showMoreResourcesWindow');
        });
    });
    describe("ManagementCard", function () {
        var fixture = '';

        beforeEach(function () {
            PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
            PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
            PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
        });

        afterEach(function () {
            $('body').children('div').not('#HTMLReporter').remove();
        });

        it("can open edit/copy dialog in the copy mode", function () {
            helper.GetManagementCardViewFixture();

            var mode = '';

            spyOn($, "ajax").andCallFake(function (params) {
                mode = params.data.mode;
            });

            PxManagementCard.showEditContentTitleDialog("sourceId", "parentId", "title", "source", "copy");

            expect(mode).toEqual("copy");
        });

        it("can open edit/copy dialog in the rename mode", function () {
            helper.GetManagementCardViewFixture();

            var mode = '';

            spyOn($, "ajax").andCallFake(function (params) {
                mode = params.data.mode;
            });

            PxManagementCard.showEditContentTitleDialog("sourceId", "parentId", "title", "source", "rename");

            expect(mode).toEqual("rename");
        });

        it("can generate form with CopyContent action of the FacePlate controller", function () {
            helper.GetEditContentViewFixture("copy");

            expect($('form').attr('action')).toEqual("/FacePlate/CopyContent");
        });

        it("can generate form with EditContentSave action of the LaunchpadTreeWidget controller", function () {
            helper.GetEditContentViewFixture("rename");

            expect($('form').attr('action')).toEqual("/LaunchpadTreeWidget/EditContentSave");
        });

        var helper = {
            GetManagementCardViewFixture: function () {
                fixture = helper.GetManagementCardView();
                jasmine.Fixtures.prototype.addToContainer_(fixture);
            },
            GetManagementCardView: function () {
                var viewData = {
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
                        DueDate: new Date(),
                        SourceType: "",
                        AssignTabSettings: {
                            ShowMakeGradeable: false,
                            ShowGradebookCategory: true
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
                view += "<div data-ft-id='1' class='faux-tree-node'><div class='faceplate-student-completion-stats'></div></div>')";

                return view;
            },
            GetEditContentViewFixture: function (mode) {
                fixture = helper.GetEditContentView(mode);
                jasmine.Fixtures.prototype.addToContainer_(fixture);
            },
            GetEditContentView: function (mode) {
                var viewData = {
                    mode: {
                        dataType: "System.String",
                        dataValue: mode
                    }
                };

                var data = {
                    viewPath: "EditContentView",
                    viewModel: JSON.stringify({

                    }),
                    viewModelType: "Bfw.PX.PXPub.Models.ContentItem",
                    viewData: JSON.stringify(viewData)
                };

                var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', data);

                return view;
            }
        };
    });
});
    
$(function () {
    var entityId = "57704";
    var disciplineId = "120559";
    var itemParentId = "PX_MULTIPART_LESSONS";
    $.fn.GetLaunchpadSet1 = function (optViewPath) {
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
        var viewPath = (optViewPath) ? optViewPath : "Launchpad";

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

        return arrange;
    };
}(jQuery));