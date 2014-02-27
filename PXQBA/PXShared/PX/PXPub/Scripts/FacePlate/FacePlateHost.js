// Defines a singleton object that coordinates all client-side behavior of the AssignmentCenter page.
(function ($) {
    // static data
    var _static = {
        defaults: {
            mode: "syllabus",
            autoFocus: false
        },
        settings: {},
        modes: {
            assignmentPad: "assignment_pad",
            syllabus: "syllabus",
            noSyllabus: "nosyllabus"
        },
        // commonly used CSS classes
        css: {},
        // commonly used jQuery selectors
        sel: {
            leftNav: ".unit-subitems-wrapper",
            // ".nav",
            viewMode: ".selViewMode",
            actionMenu: "#assignmentmenu .action-menu",
            autoFocusReturn: ".auto-focus-return"
        },
        // static functions
        fn: {
            showAssignedContentButtons: function (node) {
                node.find(".faceplate-item-assign").hide();
                var parentId = node.attr("data-ft-parent");
                if (parentId.length) {
                    $(".faux-tree-node[data-ft-id=\"" + parentId + "\"]").find(".faceplate-item-assign").hide();
                }
            },

            showNotAssignedContentButtons: function (node) {
                node.find(".faceplate-item-assign").show();
            },
            // when the view mode is changed in the drop down we need to redraw the left nav
            onViewModeChanged: function (event) {
                var mode = $(event.target).val();
                $(_static.sel.leftNav).block({
                    message: null
                });
                $.get(PxPage.Routes.load_ac_navigation, {
                    mode: mode
                }, function (response) {
                    $(_static.sel.leftNav).replaceWith(response);
                    $(_static.sel.leftNav).Navigation({
                        readOnly: (mode === _static.modes.assignmentPad)
                    });
                });
            },
            // when something adds existing content to the assignment center we need to track the
            // new item
            onAddExistingContent: function (event, id, categoryId, parentId, callback) {
                if (!id) {
                    return;
                }

                var data = {
                    filterId: '',
                    templateId: '',
                    type: '',
                    parentId: parentId
                };

                var item = {
                    id: id,
                    title: 'Existing Resource Link'
                };

                var stubData = PxFacePlate.InjectStubItem(data, item);

                var args = {
                    contentId: id,
                    newParentId: stubData.parentId,
                    previousParentId: ''
                };

                $.post(PxPage.Routes.faceplate_moveitem, args, function (response) {
                    _static.fn.onAddNewContent(event, id, "");
                    if (callback) {
                        callback();
                    }
                });

                return;
            },
            // when something adds new content to the assignment center we need to track the
            // new item
            onAddNewContent: function (event, id, existing) {
                if (!id) {
                    return;
                }
                var below = $.fn.fauxtree("getNode", id).next();

                _static.fn.saveNavigationState(id, existing, "newitem", false, null, below);
            },

            OnRemoveItem: function (event, id) {
                var item = $.fn.fauxtree("getNode", id);

                var callback = function () {
                    $.fn.fauxtree("removeNode", id);
                    PxPage.LargeFNE.CloseFNE();
                    PxPage.Loaded("faceplate_launchpad", true);
                    $(PxPage.switchboard).trigger("faceplateUpdateResourceChapterId");
                };
                _static.fn.saveNavigationState(id, "", "Removed", false, null, null, callback);
            },
            saveNavigationState: function (id, existing, operation, reload, above, below, callback) {
                PxPage.Loading("saveNavigationState");
                var data = {
                    changed: id,
                    operation: operation,
                    mode: existing,
                    isSetChangedNodeActive: true,
                    above: above,
                    below: below,
                    onSuccess: function (node) {
                        if (operation == "Removed") {
                            if (callback)
                                callback();
                        }
                        else {
                            var node = $.fn.fauxtree("getNode", id);
                            var parentId = node.attr("data-ft-parent");
                            if (reload) {
                                $.fn.fauxtree("removeChildren", parentId);
                                $.fn.fauxtree("toggleNode", parentId);
                                node = $.fn.fauxtree("getNode", parentId);

                                if (node.attr("data-ft-state") == "closed") {
                                    $.fn.fauxtree("toggleNode", node);
                                }
                                $(node).addClass("active");
                                $(PxPage.switchboard).trigger("faceplateUpdateResourceChapterId");
                                PxPage.Loaded();
                            } else {
                                //reload the carousel item if applicable
                                var data = {
                                    category: $(".nav-category"),
                                    changed: id,
                                    tree: $(".faux-tree"),
                                    above: null,
                                    below: null,
                                    operation: operation,
                                    url: PxPage.Routes.load_faceplate_navigation,
                                    replacementMode: "replace",
                                    itemId: id,
                                    callback: function () {
                                        $.fn.fauxtree("scanTree", $(".faux-tree"));
                                        var node = $.fn.fauxtree("getNode", id);
                                        //$(node).addClass("active");
                                        $(node).addClass("fade-effect");
                                        PxPage.Fade();
                                        $(PxPage.switchboard).trigger("faceplateUpdateResourceChapterId");
                                        PxPage.Loaded();
                                    }
                                };

                                $(_static.sel.leftNav).FacePlateNavigation("loadChildrenDataForParent", data);
                            }
                        }

                    }
                };

                $(_static.sel.leftNav).FacePlateNavigation("saveNavigationState", data);
            },

            onSetVisibility: function (event, args) {
                $(_static.sel.leftNav).FacePlateNavigation("onSetVisibility", arg);
            },

            onContentUnAssigned: function (event, args) {
                var data = {
                    changed: args.id,
                    operation: "datesunassigned"
                },
                    item = $(_static.sel.leftNav).FacePlateNavigation("getItem", args.id),
                    category = $(_static.sel.leftNav).FacePlateNavigation("getCategory", args.id);

                item.fpnattr("due-date", '1/1/0001');
                item.fpnattr("start-date", '1/1/0001');

                if (args.category == undefined) {
                    return;
                }

                item.fpnattr("points", "");
                item.fpnattr("gradebookcategory", "");

                data.onSuccess = function () {
                    $("#nav-category").removeAttr("style");
                    $("#PX_HOME_FACEPLATE_ZONE_2").removeAttr("style");


                    item.find(".pxunit-display-points").html('');

                    item.find(".faceplate-item-assign").click();
                    item.find(".faceplate-right-menu").show();
                    _static.fn.showNotAssignedContentButtons(item);
                    $(PxPage.switchboard).trigger("loadcurrentdueitem");

                    PxPage.Loaded();

                    $("#nav-category").removeAttr("style");
                    $("#PX_HOME_FACEPLATE_ZONE_2").removeAttr("style");
                };


                data.onFailure = function () {
                    item.find(".faceplate-item-assign").click();
                    item.find(".faceplate-right-menu").show();
                };

                item.find(".faceplate-right-menu").hide();
                //todo: call showRowMenuAndCalendarClose in FaceplateNav //$(_static.sel.leftNav).FacePlateNavigation("showRowMenuAndCalendarClose");


                $(_static.sel.leftNav).FacePlateNavigation("saveNavigationState", data);
            },
            // when an item has been assigned through the PxPage event contentassigned
            // we need to update our state
            onContentAssigned: function (event, args) {
                var data = {
                    changed: args.id,
                    operation: "datesassigned"
                },
                    item = $(_static.sel.leftNav).FacePlateNavigation("getItem", args.id),
                    category = $(_static.sel.leftNav).FacePlateNavigation("getCategory", args.id);

                var dueDateObj = new Date(args.date);
                var startDateObj = new Date(args.startdate);
                item.fpnattr("due-date", dueDateObj.format("mm/dd/yyyy"));
                item.fpnattr("due-time", dueDateObj.format("HH:MM:ss"));

                if (args.startdate != "" && args.startdate != undefined) {
                    item.fpnattr("start-date", startDateObj.format("mm/dd/yyyy"));
                    item.fpnattr("start-time", startDateObj.format("HH:MM:ss"));
                }

                if (args.category == undefined) {
                    return;
                }

                if (item.fpnattr("itemtype") != "PxUnit" && args.points != undefined) {
                    item.fpnattr("points", args.points);
                }

                item.fpnattr("gradebookcategory", args.gradebookcategory);

                data.onSuccess = function () {

                    //show points
                    if (args.points != undefined && ((args.points * 1) > 0)) {
                        item.find(".pxunit-display-points").html(args.points + 'pts');
                    } else {
                        item.find(".pxunit-display-points").html('');
                    }

                    //show/hide Assign button
                    $(".faux-tree-node.pxunit-item-row").each(function () {
                        var currentItem = $(this);
                        var dueDate = currentItem.find(".pxunit-display-duedate span.due_date_day").html();
                        dueDate = jQuery.trim(dueDate);
                        if (dueDate != "") {
                            if (currentItem.fpnattr("id") != item.fpnattr("id")) {
                                _static.fn.showAssignedContentButtons(currentItem);
                            }
                        }
                    });

                    _static.fn.showAssignedContentButtons(item);

                    if ($(".FacePlateAsssign").length > 0) {
                        //item.find(".faceplate-item-assign").click();

                        $(".faceplate-right-menu").hide();
                        $(".faux-tree-node").removeClass("managementcard");

                        var gearbox = item.find(".view-pxunit-menu .gearbox");
                        gearbox.trigger('click');
                    }

                    PxPage.Loaded();

                    $("#nav-category").removeAttr("style");
                    $("#PX_HOME_FACEPLATE_ZONE_2").removeAttr("style");
                    $(PxPage.switchboard).trigger("loadcurrentdueitem");
                };

                $(_static.sel.leftNav).FacePlateNavigation("saveNavigationState", data);
            },
            CreateDateDiv: function (date) {
                var d = new Date(date);
                var month = d.getMonth(); //$.datepicker.formatDate("mmm", date);
                var day = d.getDay(); //$.datepicker.formatDate("dd", date);

                return d.toString();

                var div = String.format("<div class=\"due_date\"> \
                <div class=\"dd_cal\">                              \
                    <div class=\"dd_cal_month\">                    \
                        <span class=\"dd_cal_month_inner\">{0}</span>\
                            </div>                                   \
                            <div class=\"dd_cal_date\">{1}           \
                                </div>                              \
                                </div></div>", month, day);
                return div;

            },

            // when an item has been removed through the PxPage event contentRemoved
            // we need to update our state
            onContentRemoved: function (event, id) {
                item = $(_static.sel.leftNav).FacePlateNavigation("getItem", id), category = $(_static.sel.leftNav).FacePlateNavigation("getCategory", id);

                $(_static.sel.leftNav).FacePlateNavigation("removeItem", {
                    categoryId: category.fpnattr("id"),
                    nodeId: id
                });
            },

            // when a new category is created we need to alert the navigation
            onCategoryCreated: function (event, category) {
                $(_static.sel.leftNav).FacePlateNavigation("addCategory", category);
            }
        }
    };

    // FacePlateHost is a singleton that lives on the window
    window.FacePlateHost = {
        // initializes the sub-components like the left nav
        init: function (options) {
            $.extend(true, _static.settings, _static.defaults, options);
            $(_static.sel.viewMode).change(_static.fn.onViewModeChanged);
//            $(PxPage.switchboard).bind("existingcontentadded", _static.fn.onAddExistingContent);
//            $(PxPage.switchboard).bind("contentcreated", _static.fn.onAddNewContent);
//            $(PxPage.switchboard).bind("contentassigned", _static.fn.onContentAssigned);
//            $(PxPage.switchboard).bind("syllabuscategorycreated", _static.fn.onCategoryCreated);
//            $(PxPage.switchboard).bind("contentremoved", _static.fn.onContentRemoved);
//            $(PxPage.switchboard).bind("reloadsyllabus", _static.fn.onReloadSyllabus);
//            $(PxPage.switchboard).bind("contentunassigned", _static.fn.onContentUnAssigned);
//            $(PxPage.switchboard).bind("onremoveitem", _static.fn.OnRemoveItem);
            //$(PxPage.switchboard).bind("settingsUpdate", _static.fn.onSetVisibility);
        },
        // opens the AssignmentCenterContentDialog in the given mode
        createContent: function (mode) {
            PxFacePlate.CreateItems();
        },
        // called when an item is saved and we need to initialize the dialog to show the templates again
        onShowTemplates: function () {
            PxFacePlate.CreateItems();
        },
        // called when content is "saved"
        onSaveContent: function (response) {
            //contentItemId = $(response).find(".item-id").val();

            $(PxPage.switchboard).trigger("addexistingcontent", response);
            ContentWidget.ContentCreated(response, null, null, 'closeKeepNode', PxTemplates.TemplateContexts.FacePlate.value, null);

            //open create new dialog
            PxFacePlate.CreateItems();
        },
        // called when content is "saved and opened"
        onSaveAndOpenContent: function (response) {
            ContentWidget.ContentCreated(response, null, null, 'closeKeepNode', PxTemplates.TemplateContexts.FacePlate.value, null);

            if ($(response).find("#view").length > 0) {
                var fneLink = $(response).find("#view").attr("href");
                PxPage.OpenFneLinkForFaceplateItem(fneLink);
            } else if ($(response).find("#OnSuccessActionUrl").length > 0) {
                var fneLink = $(response).find("#OnSuccessActionUrl").val();
                PxPage.OpenFneLinkForFaceplateItem(fneLink);
            } else {
                alert('error: fne url not found');
            }
        },
        // called when content is "saved and opened"
        onCancelAndShowCreate: function (id) {
            PxPage.CloseCreateNewScreen({
                reason: 'cancel',
                id: id
            });
            // as per new requirenment, we are not re-opening the create new screen after cancel.
            //ContentWidget.ContentCreated('cancel', null, null, 'modal', PxTemplates.TemplateContexts.FacePlate.value, FacePlateHost.onShowTemplates);
            var args = {
                contentId: id
            };
            $.post(PxPage.Routes.my_material_remove_item, args, null);
            return false;
        },
        contentRemoved: function (event, itemId) {
            jQuery(PxPage.switchboard).trigger('contentremoved', [itemId]);
            PxSyllabusCategory.ShowActionLink(event);
        },
        addExistingContent: function (event, itemId) {
            jQuery(PxPage.switchboard).trigger('existingcontentadded', [itemId]);
            PxSyllabusCategory.ShowActionLink(event);
        }
    };
} (jQuery));