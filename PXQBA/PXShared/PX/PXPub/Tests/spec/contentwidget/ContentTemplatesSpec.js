/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" data-cover-adapter="../../lib/blanket/jasmine-blanket.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.8.18.custom.min.js" />
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

/// <reference path="../../../Scripts/other/date.js" />
/// <reference path="../../../Scripts/Common/dateFormat.js" />

/// <reference path="../../../Scripts/ContentWidget/Templates.js" />
/// <reference path="../../../Scripts/ContentWidget/ContentTemplates.js" />
/// <reference path="../../../Scripts/Template/TemplatePicker.js" />

describe("ContentTemplates", function () {
    var fixture = '';
    
    //mock pxpage
    PxPage = {
        switchboard: $(document),
        log: function() {
        },
        Loading: function() {
        },
        Loaded: function() {
        },
        Update: function() {
        },
        CloseNonModal: function() {
        },
        Routes: {
            
        },
        TouchEnabled: function () {
            return false;
        }
    };

    describe('CreateItems', function () {
        var myParentId = "this-is-a-parent-id";
        var myToc = "this-is-toc";
        
        beforeEach(function () {
            /* arrange */
            helper.GetNewTemplatesFixture();

            $.fn.PxNonModal = function(data) {
            };
            window.tinyMCE = function() {
            };

            /* spies */
            spyOn(PxContentTemplates, "ContentCreationTemplates").andCallFake(function (templateContext, callback, callbackOnResize, center, width) {
                if (typeof callback === "function") {
                    callback();
                }
            });
            spyOn(PxContentTemplates, "CreateItemFromTemplate").andCallFake(function (templateItemId, callback, parentId, title) {
                if (typeof callback === "function") {
                    callback({
                        id: templateItemId,
                        title: title,
                        parentId: parentId
                    });
                }
            });
            spyOn($, "get").andCallFake(function(url, data, success, dataType) {
                if (typeof success == "function") {
                    success("<div>Success</div");
                }
            });
            spyOn($.fn, "PxNonModal").andCallFake(function (data) {
                $("#jasmine-fixtures").append("<div id=\"nonmodal-content\"><input type=\"hidden\" class=\"toc\" value=\"\" /></>");
                
                if (typeof data.onCompleted === "function") {
                    data.onCompleted();
                }
            });

            $(".template-list li:first-child").toggleClass("selected");
            
            $(PxPage.switchboard).unbind("AddFromSelectedTemplate");
        });
        it("can bind and trigger AddFromSelectedTemplate", function () {

            $.each(this.spies_, function () {
                if (this.identity == "ContentCreationTemplates") {
                    this.baseObj["ContentCreationTemplates"] = this.originalValue;
                }
            });

            spyOn(PxContentTemplates, "AddFromSelectedTemplateFaceplate");

            PxContentTemplates.CreateItems(null, false, myToc);
            $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

            PxContentTemplates.CreateItems(null, false, myToc);
            $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

            expect(PxContentTemplates.AddFromSelectedTemplateFaceplate.callCount).toBe(2);
        });
        it("can pass toc to AddFromSelectedTemplateFaceplate", function () {
            /* arrange */
            
            /* spies */
            spyOn(PxContentTemplates, "AddFromSelectedTemplateFaceplate");

            /* act */
            PxContentTemplates.CreateItems(false, false, myParentId, myToc);
            
            $(PxPage.switchboard).trigger("AddFromSelectedTemplate");
            
            /* assert */
            expect(PxContentTemplates.AddFromSelectedTemplateFaceplate).toHaveBeenCalledWith(myParentId, myToc);
        });
        it("can pass toc to onItemFromTemplate", function () {
            /* arrange */
            /* spies */
            spyOn(PxContentTemplates, "onItemFromTemplate");
            
            var firstLi = $(".template-list li:first-child");
            var selectedTemplateId = firstLi.attr("itemid");
            var itemTitle = firstLi.find(".item-title").html();
            var itemType = itemTitle;
            
            /* act */
            PxContentTemplates.CreateItems(false, false, myParentId, myToc);
            
            $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

            /* assert */
            expect(PxContentTemplates.onItemFromTemplate).toHaveBeenCalledWith({
                templateId: selectedTemplateId,
                title: itemTitle,
                type: itemType,
                parentId: myParentId,
                toc: myToc
            });
        });
        describe("on creating a folder", function () {
            beforeEach(function() {
                /* arrange */
                $(".template-list li:first-child .item-title").html("Folder");
            });
            it("can set the default value to the toc - hidden input field's value if toc is null", function () {
                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId, null);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");
                
                /* assert */
                var fldToc = $('#nonmodal-content').find('.toc').val();
                expect(fldToc).toBe("syllabusfilter");
            });
            it("can set the default value to the toc - hidden input field's value if toc is undefined", function() {
                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

                /* assert */
                var fldToc = $('#nonmodal-content').find('.toc').val();
                expect(fldToc).toBe("syllabusfilter");
            });
            it("can set the toc - hidden input field's value", function() {
                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId, myToc);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

                /* assert */
                var fldToc = $('#nonmodal-content').find('.toc').val();
                expect(fldToc).toBe(myToc);
            });
        });
        describe("on creating a unit", function () {
            beforeEach(function() {
                /* arrange */
                $(".template-list li:first-child .item-title").html("Unit");
            });
            it("can set the default value to the toc - hidden input field's value if toc is null", function () {
                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId, null);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");
                
                /* assert */
                var fldToc = $('#nonmodal-content').find('.toc').val();
                expect(fldToc).toBe("syllabusfilter");
            });
            it("can set the default value to the toc - hidden input field's value if toc is undefined", function() {
                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

                /* assert */
                var fldToc = $('#nonmodal-content').find('.toc').val();
                expect(fldToc).toBe("syllabusfilter");
            });
            it("can set the toc - hidden input field's value", function() {
                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId, myToc);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

                /* assert */
                var fldToc = $('#nonmodal-content').find('.toc').val();
                expect(fldToc).toBe(myToc);
            });
        });
        describe("on creating items apart from unit or folder", function () {
            beforeEach(function () {
                /* arrange */
                $(".template-list li:first-child .item-title").html("Html Page");
            });
            it("can pass the default toc value to 'contentcreated' binder if toc is null", function () {
                var thisToc = null;
                $(PxPage.switchboard).bind("contentcreated", function(event, item, existing, parent, callback) {
                    thisToc = item.toc;
                });
                
                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId, null);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

                /* assert */
                expect(thisToc).not.toBeNull();
                expect(thisToc).toBe("syllabusfilter");
            });
            it("can pass the toc value to 'contentcreated' binder", function() {
                var thisToc = null;
                $(PxPage.switchboard).bind("contentcreated", function (event, item, existing, parent, callback) {
                    thisToc = item.toc;
                });

                /* act */
                PxContentTemplates.CreateItems(false, false, myParentId, myToc);

                $(PxPage.switchboard).trigger("AddFromSelectedTemplate");

                /* assert */
                expect(thisToc).not.toBeNull();
                expect(thisToc).toBe(myToc);
            });
        });
    });

    describe('Create new dialog', function () {

        window._ = TemplatePicker.privateTest();

        $(PxPage.switchboard).data('events', {});

        beforeEach(function () {
            spyOn(_.fn, 'createItem').andCallThrough();
            setFixtures(helper.GetTemplatePickerView());
        });

        describe('mouse', function () {

            beforeEach(function () {
                _.fn.BindControls();
            });

            it('will display title and description on mouse over', function () {
                var el = $(_.sel.line).eq(0),
                    desc = el.find(_.sel.description).val(),
                    title = el.find(_.sel.title).text();
                el.eq(0).trigger('mouseenter');
                expect( $(_.sel.details).find(_.sel.detailsDescription).html() ).toEqual(desc);
                expect( $(_.sel.details).find(_.sel.detailsTitle).text().trim() ).toEqual(title);
            });

            it('will create content when an item is clicked', function () {
                $(_.sel.line).eq(0).trigger('click');
                expect(_.fn.createItem).toHaveBeenCalled();
            });

        });

        describe('touch', function () {

            beforeEach(function () {
                _.fn.BindControls();
                PxPage.TouchEnabled = function () {
                    return true;
                }
            });

            it('will display title and description on touch', function () {
                var el = $(_.sel.line).eq(0),
                    desc = el.find(_.sel.description).val(),
                    title = el.find(_.sel.title).text();
                el.eq(0).trigger('touchstart');
                expect( $(_.sel.details).find(_.sel.detailsDescription).html() ).toEqual(desc);
                expect( $(_.sel.details).find(_.sel.detailsTitle).text().trim() ).toEqual(title);
            });

            it('will create content when the button is tapped', function () {
                $(_.sel.createNew).trigger('click');
                expect(_.fn.createItem).toHaveBeenCalled();
            });

        });

    });

    var helper = {
        GetNewTemplatesFixture: function () {
            fixture = helper.GetTemplatePickerView();
            setFixtures(fixture);
        },
        GetTemplatePickerView: function () {
            var viewData = {
                TemplateDisplayContext: {
                    dataType: "System.Int32",
                    dataValue: "5"
                }
            };

            var fakePolicies = [];
            fakePolicies.push("a");
            fakePolicies.push("b");
            
            var data = {
                viewPath: "TemplatePicker",
                viewModel: JSON.stringify([
                    {
                        Id: 'first-link-id',
                        Title: 'first-link-title',
                        Policies: fakePolicies,
                        Description: 'A repository for useful files.'
                    },
                    {
                        Id: 'second-link-id',
                        Title: 'second-link-title',
                        Policies: fakePolicies,
                        Description: 'An assignment primarily designed to assess knowledge.'
                    }
                ]),
                viewModelType: "System.Collections.Generic.List\`1[[Bfw.PX.PXPub.Models.Template, Bfw.PX.PXPub.Models]]",
                viewData: JSON.stringify(viewData)
            };

            var view = PxViewRender.RenderView('PXPub', 'Template', data);
            
            return view;
        }
    };
});