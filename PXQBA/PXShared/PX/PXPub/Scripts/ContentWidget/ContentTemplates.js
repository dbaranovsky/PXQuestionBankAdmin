// Represents the functions involved with templates, specifically as they relate to the content section

var PxContentTemplates = function ($) {
    return {
        CreateItemFromTemplate: function (templateItemId, callback, parentId, title) {
            $.ajax({
                url: PxPage.Routes.item_from_template,
                type: "POST",
                data: {
                    templateItemId: templateItemId,
                    parentId: parentId,
                    title: title
                },
                success: function (response) {
                    if (callback) {
                        callback(response);
                    }
                }
            });
        },

        ShowMoreDialog: function (context, callback) {
            if (!context) {
                context = PxTemplates.TemplateContexts.Content.value;
            }

            PxPage.Loading("nonmodal");
            if (!callback) {
                callback = function () {
                    $("button.add-from-selected-template").rebind("click", function () {
                        PxPage.Loading();
                        PxContentTemplates.AddFromSelectedTemplate();
                    });

                    PxContentTemplates.MakeTemplatesDraggable();
                    PxPage.Loaded("nonmodal");
                    $(document).off('mouseenter', '.templateLineItem').on('mouseenter', '.templateLineItem', PxTemplates.ShowActionLink);
                };
            }

            PxContentTemplates.ContentCreationTemplates(context, callback);
        },

        CreateSelectedItem: function (item) {
            var parent = $(item).parents('li');
            PxContentTemplates.TemplateItemSelected({ target: parent });
            //PxPage.Loading();
            PxContentTemplates.AddFromSelectedTemplate();

        },

        AddFromSelectedTemplate: function () {
            var selectedTemplateId = $(".template-list li.selected").attr("itemid");
            if (selectedTemplateId) {
                $(PxPage.switchboard).trigger("openactivenode", function () {
                    PxContentTemplates.CreateItemFromTemplate(selectedTemplateId, function (item) {
                        ContentWidget.AddItemToActiveNode(item);
                    });
                });
            } else {
                PxPage.Loaded();
            }
        },

        // from faceplate - similar to above
        AddFromSelectedTemplateFaceplate: function (parentId, toc) {
            PxPage.Loading("nonmodal-window");
            var selectedTemplateId = $(".template-list li.selected").attr("itemid");
            var itemTitle = $(".template-list li.selected .item-title").html();
            var itemType = itemTitle;
            
            if (!selectedTemplateId) {
                PxPage.Toasts.Error('Invalid Template Selection');
                PxPage.Loaded();
                return false;
            }

            PxContentTemplates.onItemFromTemplate({
                templateId: selectedTemplateId,
                title: itemTitle,
                type: itemType,
                parentId: parentId,
                toc: toc
            });

            PxPage.Loaded();
        },

        showActionLink: function (event) {
            event.preventDefault();

            $('.tocAssign').hide();
            $('.tocUnAssign').hide();
            $('.tocAssignRight').hide();
            $('.lnkAIW').hide();
            $('.padding').show();

            if ($(this).hasClass('templateLineItem')) {
                $(this).find('.padding').hide();
                $(this).find('.tocAssign').show();
            } else {
                var isPartOfAssignmentCenter = $(this).siblings('.IsPartOfAssignmentCenter').val();
                if (isPartOfAssignmentCenter == "True") {
                    $(this).siblings('.tocUnAssign').show();
                    $(this).parent().find('.tocAssignRight .lnkAIW').hide();
                } else {
                    $(this).siblings('.tocAssign').show();
                    $(this).parent().find('.tocAssignRight .lnkAIW').show();
                }
            }

            $(this).find('.tocAssignRight').first().show();
            return true;
        },

        onItemFromTemplate: function (data) {
            var width = data.type == 'Folder' ? 800 : 600;
            var tocfilter = data.toc || "syllabusfilter";
            PxContentTemplates.CreateItemFromTemplate(data.templateId, function (item) {
                var itemUrl = PxPage.Routes.display_content + "/" + item.id + "?mode=Edit&includeNavigation=false";

                if (data.type == "Folder" || data.type == "Unit") {
                    $.get(itemUrl, null, function (response) {
                        $(response).PxNonModal({
                            title: 'Create New ' + data.type,
                            widthOverride: width,
                            centerFaceplate: true,
                            onCompleted: function () {
                                $.unblockUI();
                                var parentId = data.parentId;
                                var filterId = data.filterId;
                                var type = data.templateId;
                                // moreResourceParent.appendTo(parent);

                                // change the parent id from PX_TEMP parent id, so that when clicking
                                // save button we can create a new item with this parent id.
                                $('#nonmodal-content').find('#Content_DefaultCategoryParentId').val(parentId);
                                $('#nonmodal-content').find('#Content_ParentId').val(parentId);
                                $('#nonmodal-content').find('#Content_Title').val($('#nonmodal-content').find('#Content_Title').val());
                                $('#nonmodal-content').find('.toc').val(tocfilter);
                                
                                if (parentId == "PX_MULTIPART_LESSONS") {
                                    $('#nonmodal-content').find('#Content_SyllabusFilter').val(filterId);
                                } else {
                                    $('#nonmodal-content').find('#Content_SyllabusFilter').val(parentId);
                                }

                                if (tinyMCE && tinyMCE.activeEditor) {
                                    tinyMCE.activeEditor.setContent('');
                                }

                                $('#Content_SourceTemplateId').val(type);
                            }
                        });
                        PxPage.Update();
                    });
                }
                else {
                    itemUrl += "&isBeingEdited=true&includeNavigation=true&renderFNE=true";
                    var openFNE = data.openFNE == null || data.openFNE;

                    var content = {
                        contentType: data.type,
                        id: item.id,
                        name: item.title,
                        toc: tocfilter
                    };
                    
                    $(PxPage.switchboard).trigger("contentcreated", [content, true, data.parentId,
                        function () {
                            if (openFNE) {
                                PxPage.LargeFNE.OpenFNELink(itemUrl, item.title, false, null, true);
                            }
                        } ]
                    );

                    if (data.callback != null) {
                        data.callback(item.id);
                    }
                    PxPage.CloseNonModal();
                }

                if (tinyMCE && tinyMCE.activeEditor) {
                    tinyMCE.activeEditor.setContent('');
                }

            }, data.parentId, data.title);
        },

        TemplateItemSelected: function (event) {

            var itemId = $(event.target).attr("itemid");
            //$("#nonmodal .details").load(PxPage.Routes.template_details, { itemId: itemId });
            var li = $(event.target).closest("li");
            var title = li.find(".item-title").html();
            var description = li.find("input.item-description").val();
            var descriptionEl = $(".template-picker .description");
            descriptionEl.html(description);

            var policiesText = li.find("input.item-policies").val();
            var policies = policiesText && policiesText.split("|");
            var policiesNodes = $(".template-picker .details .policies-header, .template-picker .details .policies");
            if (policies && policies.length) {
                policiesNodes.show();
                var policiesEl = $(".template-picker .details .policies");
                policiesEl.empty();
                $.each(policies, function (i, v) {
                    policiesEl.append("<li>" + v + "</li>");
                });
            }
            else {
                policiesNodes.hide();
            }
            $(".template-picker .details .title").html(title);

            // Highlight the selected item
            $(".template-list li").removeClass("selected");
            li.addClass("selected");

            if ($(event.target).hasClass('contentBrowser')) {
                PxPage.Loading();
                return PxContentTemplates.CreateSelectedItem($(event.target));
            }
        },

        MakeTemplatesDraggable: function () {
            $(".template-list li").liveDraggable({
                revert: false,
                connectToSortable: '#toc > ul > .section',
                cursor: "move",
                cursorAt: { top: -12, left: -20 },
                helper: "clone"
            });
        },

        ContentCreationTemplates: function (templateContext, callback, callbackOnResize, center, width) {
            $.get(PxPage.Routes.template_picker, { 'context': templateContext }, function (response) {
                $(response).PxNonModal({
                    title: 'Create New',
                    centerFaceplate: center != null ? center : false,
                    widthOverride: width != null ? width : '',
                    onCompleted: function () {
                        PxContentTemplates.TemplateItemSelected({ target: $(".template-list li:first") });
                        if ($.isFunction(callback)) {
                            callback();
                        }
                    },
                    onResize: function () {
                        $(".template-list li").rebind("click", PxContentTemplates.TemplateItemSelected);
                        if ($.isFunction(callbackOnResize)) {
                            callbackOnResize();
                        }
                    }
                });
            });
        },

        // callback for `Create new` link, loads the templates dialog
        CreateItems: function (enableUnit, enableFolder, parentId, toc) {

            PxPage.log('templates : showing template dialog');

            PxPage.CloseNonModal(); // Close Nonmodal if exisits
            PxPage.Loading('nonmodal');

            //set create defaults
            if (enableUnit != null) {
                window.PxContentTemplates.CreateItems.enableUnit = enableUnit;
            } else {
                window.PxContentTemplates.CreateItems.enableUnit = true;
            }
            if (enableFolder != null) {
                window.PxContentTemplates.CreateItems.enableFolder = enableFolder;
            } else {
                window.PxContentTemplates.CreateItems.enableFolder = false;
            }
            if (parentId != null) {
                window.PxContentTemplates.CreateItems.parentId = parentId;
            } else { //fallback
                window.PxContentTemplates.CreateItems.parentId = "PX_MULTIPART_LESSONS";
            }

            PxContentTemplates.ContentCreationTemplates(PxTemplates.TemplateContexts.FacePlate.value,
                function () {
                    //oncompleted
                    PxPage.Loaded('nonmodal');
                    //hide folder template
                    $(".templateLineItem .item-title:contains('Folder')").closest(".templateLineItem").hide();
                    if (window.PxContentTemplates.CreateItems.enableFolder) {
                        //change UNIT description and title to FOLDER desc and title
                        var folderDesc = $(".templateLineItem .item-title:contains('Folder')").parent().find(".item-description").val();
                        $(".templateLineItem .item-title:contains('Unit')").parent().find(".item-description").val(folderDesc);

                        if (window.PxContentTemplates.CreateItems.parentId == "PX_MULTIPART_LESSONS") {
                            $("#templates .details .title").text("Unit");
                        } else {
                            $(".templateLineItem .item-title:contains('Unit')").text("Folder");
                            $("#templates .details .title").text("Folder");
                        }
                        $("#templates .details .description").html(folderDesc);
                    }
                    
                    // bind links
                    $(PxPage.switchboard).unbind("AddFromSelectedTemplate").bind("AddFromSelectedTemplate", function () {
                        PxContentTemplates.AddFromSelectedTemplateFaceplate(window.PxContentTemplates.CreateItems.parentId, toc);
                    });

                    //bind DONE button
                    $("button.add-from-selected-template").rebind("click", function () {
                        PxContentTemplates.AddFromSelectedTemplateFaceplate(window.PxContentTemplates.CreateItems.parentId);
                    });

                    //bind CREATE button
                    $(".tocAssign a.assignmentCenter").rebind("click", function () {
                        $(".templateLineItem").removeClass("selected");
                        $(this).parents(".templateLineItem").addClass("selected");
                        PxContentTemplates.AddFromSelectedTemplateFaceplate(window.PxContentTemplates.CreateItems.parentId);
                    });

                    $('.nonmodal-window').addClass('ac-create-content');

                    $('.templateLineItem')
                        .bind('mouseenter', PxContentTemplates.showActionLink)
                        .filter(function () {
                            return $(this).css('display') != 'none';
                        })
                        .each(function (index) {
                            $(this).addClass("row-" + !(index % 2));
                        });
                },
                function () {
                    //onresize
                    $("#nonmodal").css({ position: "fixed" });
                }, true, 800);
        },

        SetTemplateReloadMode: function (mode) {
            $('#targetMode').val(mode);
        },

        GetTemplateReloadMode: function () {
            var retval = null;
            var mode = $('#targetMode');
            if (mode.length > 0) {
                retval = $('#targetMode').val();
            }
            return retval;
        }
    };
} (jQuery);
