window.FACEPLATE = {
    Init: function () {
        PxFacePlate.Init();
    }
};

var PxFacePlate = function ($) {
    var _static = {
        sel: {
            browseResourcesLink: ".faceplate-nav .faceplate-add-content-menu #browse"
        }
    };
    
    return {
        InitFaceplateFNE: function () {
        },

        Init: function () {
            PxFacePlate.InitRoutes();
            PxPage.LargeFNE.Init();
            PxPage.OnProductLoaded(function () {
                HashHistory.Init('launchpad');
            });
            $(PxPage.switchboard).bind("updateProductState", function (component, func, args) {
                PxFacePlate.UpdateStateHash(component, func, args);
            });

            if (window.PxComments != null)
                PxComments.Init();

            $(PxPage.switchboard).bind("OpenGradeBookWindow", PxFacePlate.OpenGradeBookWindow);
            $(PxPage.switchboard).bind("OpenGradeBookPrefWindow", PxFacePlate.OpenGradeBookPrefWindow);
            $(PxPage.switchboard).bind("OpenManageAnnouncementWindow", PxFacePlate.OpenManageAnnouncementWindow);
            $(PxPage.switchboard).bind("ViewAsStudent", PxFacePlate.ViewAsStudent);
            $(PxPage.switchboard).bind("OpenManageGroupsWindow", PxFacePlate.OpenManageGroupsWindow);
            $(PxPage.switchboard).bind("OpenInstructorConsole", PxFacePlate.OpenInstructorConsole);
            $(PxPage.switchboard).bind("OpenBatchUpdater", PxFacePlate.OpenBatchUpdater);
            
            $(PxPage.switchboard).bind("componentsubmit", function (event, componentType, componentId, componentState, frame) {
                //quiz or homework has been submitted, go to the review screen
                var previewMode = $(frame).closest('.preview-question-dialog').length;
                if ((componentType == "assessment" || componentType == "homework") && (previewMode != 1)) {
                    $(".submission-link").click();
                }
            });
            $(PxPage.switchboard).bind("quizstarted", function () {
                $(PxPage.switchboard).trigger("fnedonemode");
            });
            $(PxPage.switchboard).bind("homeworkattempt", function () {
                $(PxPage.switchboard).trigger("fnedonemode");
            });

            $(".submission-link").live("click", function() {
                $(PxPage.switchboard).trigger("fnedonemode");
            });
            $(PxPage.switchboard).bind("MenuWidget.ItemLoaded", function () {
                window.location.hash = window.location.hash.replace('launchpad', 'state').replace('ebook', 'state');
            });
            //select all checkboxes in the document list when selectAll is clicked
            $('fieldset ul.document-list-controls li input[name="selectall"]').live('click', function () {
                $('fieldset .document-list input[type="checkbox"]').prop('checked', true);
            });

            $('fieldset ul.document-list-controls li input[name="clearall"]').live('click', function () {
                $('fieldset .document-list input[type="checkbox"]').prop('checked', false);
            });

            $('#txtGradePoints').live('paste', function (e) {
                setTimeout(function (e) {
                    var res = parseInt($('#txtGradePoints').val());

                    if (isNaN(res)) {
                        res = 0;
                    }

                    $('#txtGradePoints').val(res);
                }, 0);
            });
            this.InitFaceplateFNE();
            $('#txtGradePoints').live('paste', function (e) {
                setTimeout(function (e) {
                    var res = parseInt($('#txtGradePoints').val());

                    if (isNaN(res)) {
                        res = 0;
                    }

                    $('#txtGradePoints').val(res);
                }, 0);
            });

            $(PxPage.switchboard).bind("removenewnode.launchpadproduct", function (event, close) {
                //ensure that newly created nodes do not show up in my materials
                var args = {
                    contentId: id
                };
                $.post(PxPage.Routes.my_material_remove_item, args, null);
            });

            PxFacePlate.CheckForStudentView();

            $(PxPage.switchboard).bind("saverssfeed", PxFacePlate.SaveRssFeed);

            // binding an event for inactive links from sandbox
            $(".sandbox-inactive").die().live("mouseenter", function (event) {
                $(".inactive-message").remove();
                $('<div class="inactive-message px-default-text">Inactive</div>').appendTo('body');
                var tooltipX = event.pageX;
                var tooltipY = event.pageY - 10;
                $(".inactive-message").css({ top: tooltipY, left: tooltipX });
                return false;
            });
            $(".sandbox-inactive").live("click", function (event) {
                PxPage.Toasts.Warning("This action is not available in a sandbox course.");
                return false;
            });
            $(".sandbox-inactive").live("mouseleave", function (event) {
                $(".inactive-message").remove();
                return false;
            });

            // if there is no launchpad we initialize ContentTreeWidget
            if ($('.unit-content-wrapper').length == 0 &&
                typeof $(this).ContentTreeWidget === "function") {
                $(this).ContentTreeWidget();
            }

            PxFacePlate.ShowIEWarning();
            
            // browse more resources link in "Add" button
            // click handler for `Browse more resources` link
            $(PxPage.switchboard).on("click", _static.sel.browseResourcesLink, function() {
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow');
            });
        },
        ShowIEWarning: function() {
            if (jQuery.browser.msie && parseFloat(jQuery.browser.version) < 9 ) {
                var cookie = get_cookie("IEUserWarned");
                if (!cookie) {
                    PxPage.Toasts.PersistantWarning("<div><p>LaunchPad is not optimized for your browser (Internet Explorer version 8). </p> <p>Please upgrade to a more recent version of Internet Explorer, or use <b>Chrome, Safari, or FireFox,</b> for optimal performance.</p></div>",
                        function() {
                            set_cookie("IEUserWarned", true, 0, '/');
                            return true;
                        });
                }
            }
        },
        UpdateStateHash: function (component, func, args) {
            //function to handle re-routing by replacing /state/ with the currently active launchpad tab or state
            var tab = '';
            if ($('#launchpad-widget-PX_LAUNCHPAD_ASSIGNED_WIDGET').length > 0) {
                //launchpad tab is active
                tab = 'launchpad';
            }
            else if ($('#launchpad-widget-PX_MENU_ITEM_EBOOK').length > 0) {
                //ebook tab is active
                tab = 'ebook';
            } else {
                //we're on the start page;
                tab = 'start';
            }

            //read currently open chapter, preserve state
            if (component == null) {
                //we're resetting the state, set it to currently open chapter
                var currentChapterId = $(PxPage.switchboard).triggerHandler("getActiveChapterId");
                if (currentChapterId != null && currentChapterId.length > 0) {
                    component = "item";
                    func = currentChapterId;
                }
            }
            //window.location.hash = window.location.hash.substring(0, window.location.hash.lastIndexOf('/'));

            var hash = tab + '/' + (component == null ? '' : component + '/' + func);
            if (args != null) {
                hash += '?' + $.map(args, function (a, b) { return b + '=' + a; }).join('&');
            }
            $(PxPage.switchboard).trigger('PX_SET_HASH', [hash, true]);
        },
        InitRoutes: function () {
            //redirect handler - redirects 'state' to appropriate tab or 'start'
            PxRoutes.Init(PxFacePlate.UpdateStateHash);
            //item route (with path)
            PxRoutes.AddComponentRoute('item', ':path*:/{itemid}{?args}', function (state, path, itemid, args) {
                //args.url = ;

                var isChapterNotLoaded = false;

                if (path != null && path.length > 0) {
                    var chapters = path.split("/");
                    $.each(chapters, function () {
                        var chapter = this.toString();
                        if (chapter != null && chapter.length > 0 && chapter != $(PxPage.switchboard).triggerHandler("getActiveChapterId")) {
                            $(PxPage.switchboard).trigger("opencontent", chapter);
                            isChapterNotLoaded = true;
                        }
                    });
                    if (isChapterNotLoaded) {
                        $(PxPage.switchboard).one("onNodesLoaded", function() {
                            $(PxPage.switchboard).trigger("openparents", itemid);
                        });
                    }
                } 

                if (state == 'lms') {
                    //default lms args:getChildrenGrades=False&includeDiscussion=False&renderFNE=True&toc=syllabusfilter
                    args.getChildrenGrades = false; //true for student view?
                    args.includeDiscussion = false;
                    args.renderFNE = true;
                    args.toc = "syllabusfilter";
                    if (args.mode == null || args.mode.toLowerCase() == "view") {
                        args.mode = "Preview";
                    }
                    else if (args.mode.toLowerCase() == "edit") {
                        args.mode = "Edit";
                        args.isBeingEdited = true;
                        args.includeNavigation = true;
                    }
                }

                args.url = PxPage.Routes.display_content + "/" + itemid + $.format("?{0}", $.param(args));
                args.loadFullFne = true;
                PxPage.LargeFNE.OpenFNELink(args.url, args.title, false, null, true);
            });
            //chapter route
            PxRoutes.AddComponentRoute('item', '{path*}', function (state, path) {
                var chapter = path.split("/")[0];
                if (chapter != null && chapter.length > 0 && chapter != $(PxPage.switchboard).triggerHandler("getActiveChapterId")) {
                    $(PxPage.switchboard).trigger("opencontent", chapter);
                }
                var targetSubfolder = path.split("/").pop();
                if (targetSubfolder != chapter && targetSubfolder.length) {
                    //open target subfolder inside chapter once nodes have loaded
                    $(PxPage.switchboard).one("onNodesLoaded", function() {
                        $(PxPage.switchboard).trigger("opencontent", targetSubfolder);
                    });
                }
                if ($("#fne-window").is(":visible")) {
                    PxPage.UnBlockAction(null, false);
                }
            });
            var executeProductRoute = function (launchpadState, component, args, args2) {
                var tab = launchpadState.split("-")[0];
                switch (tab.toLowerCase()) {
                    case 'launchpad':
                        if ($('#launchpad-widget-PX_LAUNCHPAD_ASSIGNED_WIDGET').length == 0) {
                            $('#PX_MENU_ITEM_LAUNCHPAD .menu-link').click();
                        }
                        break;
                    case 'ebook':
                        if ($('#launchpad-widget-PX_MENU_ITEM_EBOOK').length == 0) {
                            $('#PX_MENU_ITEM_EBOOK .menu-link').click();
                        }
                        break;
                    case 'lms':
                        //default to launchpad
                        if ($('#launchpad-widget-PX_LAUNCHPAD_ASSIGNED_WIDGET').length == 0) {
                            $('#PX_MENU_ITEM_LAUNCHPAD .menu-link').click();
                        }
                        break;
                    default:
                }

                //blackboard 
                if (args != null && args.return_url != null && args.return_title != null) {
                    $('.back-blackboard-btn').css('display', 'block');
                }

                if ((component == null || component.length == 0) && $("#fne-window").is(":visible")) {
                    PxPage.UnBlockAction(null, false);
                }
            };
            //product route with an extra query string at the end
            PxRoutes.AddProductRoute('{launchpadState}/:component*:{?args}{?args2}', executeProductRoute, 11);

            //product route with args
            PxRoutes.AddProductRoute('{launchpadState}/:component*:{?args}', executeProductRoute, 11);

            //product route without args
            PxRoutes.AddProductRoute('{launchpadState}/:component*:', executeProductRoute, 10);


            //groups list route
            PxRoutes.AddComponentRoute('groups', 'managegroups:?args:', function (state, args) {
                if (args == null) {
                    args = {};
                }

                args.url = PxPage.Routes.group_management;
                args.url += $.format("?{0}", $.param(args));

                PxFacePlate.AddInstructorConsoleBreadcrumb("Roster & Groups");
                PxPage.LargeFNE.OpenFNELink(args.url, "", false, null);
            });

            //calendar route
            PxRoutes.AddComponentRoute('calendar', '{path}', function (state, path, args) {
                var type = path == "list" ? "agenda" : "month";
                var title = path == "list" ? "Agenda" : "Calendar";

                PxFacePlate.AddCalendarBreadCrumb();
                PxPage.LargeFNE.OpenFNELink(PxPage.Routes.Calendar_View + "?type=" + type, title, false, null);
                $("#fne-window").addClass("OpenCalendarView");
            });
        },
        OpenGradeBookWindow: function (event) {
            var href = PxPage.Routes.greadbook_scorecard;
            PxPage.LargeFNE.OpenFNELink(href, "Gradebook", false, null);
            PxFacePlate.AddInstructorConsoleBreadcrumb("Gradebook");
        },
        OpenGradeBookPrefWindow: function (event) {
            var href = PxPage.Routes.InstructorConsole_GradebookPref;
            PxPage.LargeFNE.OpenFNELink(href, "", false, null);
            PxFacePlate.AddInstructorConsoleBreadcrumb("Gradebook Preferences");
        },
        OpenManageAnnouncementWindow: function (event) {
            var href = PxPage.Routes.AnnouncementsWidget;
            PxPage.LargeFNE.OpenFNELink(href, "Announcements", false, null);
            PxFacePlate.AddInstructorConsoleBreadcrumb();
        },
        OpenInstructorConsole: function (event, view) {
            var href = '';

            if (view == undefined) {
                view = "";
            }

            if (view == "editview") {
                href = PxPage.Routes.InstructorConsole_EditView;
                view = "";
            }
            else {
                href = PxPage.Routes.InstructorConsole_FullView + "?view=" + view;
            }

            if (view == "launchpad") {
                view = "Launch Pad";
            }

            PxPage.LargeFNE.OpenFNELink(href, "", false, null);
            PxFacePlate.AddInstructorConsoleBreadcrumb(view ? view[0].toUpperCase() + view.slice(1) : '');
        },
        OpenBatchUpdater: function (event, view) {
            var href = PxPage.Routes.InstructorConsole_FullView + "?view=BatchDueDateUpdater";
            PxPage.LargeFNE.OpenFNELink(href, "", false, null);
            PxFacePlate.AddInstructorConsoleBreadcrumb("Batch Updater");
        },
        ViewAsStudent: function (event) {
            var bodyPostContent = $.ajax({
                type: "POST",
                url: PxPage.Routes.studentView,
                beforeSend: function (jqXHR, settings) {
                    $.blockUI();
                },
                success: function (data) {
                    window.location.reload();
                }
            });
        },
        OpenManageGroupsWindow: function (event, clearCache) {
            var hash = "state/groups/managegroups";

            if (clearCache != undefined) {
                hash += "?clearCache=" + clearCache;
            }

            window.location.hash = hash;
        },
        AddInstructorConsoleBreadcrumb: function (active) {
            var breadcrumb = '';

            $(".faceplate-fne-add-item-bmr").hide();
            if ($("#student-info .view-select").length == 0) {
                breadcrumb = $("<div class='student-view-gradebook-title'>Gradebook</div>");
            } else if (active) {
                breadcrumb = $("<div><a href='#state/instructorconsole' class='path-item'>Instructor Console</a></div>");
            }
            else {
                breadcrumb = $("<div>Instructor Console</div>");
            }
            $(breadcrumb).addClass("breadcrumb");
            if ($("#student-info .view-select").length > 0 && active) {
                $(breadcrumb).append(" &raquo; " + active);
            }
            $("#fne-title").html($(breadcrumb));
        },
        AddCalendarBreadCrumb: function () {
            $(".faceplate-fne-add-item-bmr").hide();
            var breadcrumb = $("<div class='student-view-gradebook-title'>Calendar</div>");
            $(breadcrumb).addClass("breadcrumb");
            $("#fne-title-breadcrumb").html($(breadcrumb));
        },

        OnStudentViewClick: function () {
            var link = $("#fne-window #content-link").val();
            $("#fne-unblock-action-home").click();
            PxPage.LargeFNE.OpenFNELink(link);
            $(".home-layout").addClass("has-banner");
            $(".home-layout").prepend("<div class='banner student-view-banner'><span class='message'>You are currently viewing the site as a Student. Hit cancel at anytime to return to your faculty view. </span><a href='#' class='student-view-cancel-button' onclick='PxFacePlate.OnStudentViewCancel()'>Cancel</a></div>");
            $("#fne-title").css("top", "50px");
            $("#fne-content").css("top", "50px");
            $("#content-nav").css("margin-top", "60px");
            $("#add-assignment-btn").hide();
            $.unblockUI();
            $(".faceplate-fne-gearbox").hide();
            if ($("#main #viewing-as").length != 0) {
                $("#main #viewing-as").val("student");
            } else {
                $("#main").append('<input type="hidden" id="viewing-as" value="student" />');
            }
            $("#main").append('<input type="hidden" id="need-refresh" value="true" />');
        },

        OnStudentViewCancel: function () {
            $.post(PxPage.Routes.instructorView, null, function (response) {
                window.location.reload();
            });
        },

        HideMoreMenu: function () {
            $(".faceplate-more-options-menu").hide();
        },

        ShowMoreMenu: function () {
            $(".faceplate-more-options-menu").show();
        },

        GetMoreResourcesNode: function (currentNode) {
            var activeNode = (currentNode == null) ? $.fn.fauxtree("getActiveNode") : $.fn.fauxtree("getNode", currentNode);
            var rootId = "";
            if (activeNode === undefined || activeNode.length == 0) {
                var firstNode = $(".faux-tree-node").first();
                if (firstNode && firstNode.attr("data-ft-state") == "closed") {
                    $.fn.fauxtree("toggleNode", firstNode);
                }

                $.fn.fauxtree("setActiveNode", firstNode);
                rootId = firstNode.ftattr("id");
            }
            else {
                var root = $.fn.fauxtree("getOldestAncestors", activeNode);
                rootId = root.ftattr("id");
            }

            var moreResourceNode = $.fn.fauxtree("getNode", rootId + "_ChapterResourcesLinksFixed");

            if (moreResourceNode.length == 0) {
                return;
            }
            //var moreResourceParent = moreResource.parents("li");
            return moreResourceNode;
        },

        GetLastChild: function (parentId) {
            var children = $.fn.fauxtree("getChildren", parentId);
            var lastChild = $(children).last();
            if (lastChild.ftattr("id").indexOf("_ChapterResourcesLinksFixed") >= 0) {
                //last child is more resources
                children = children.splice(0, children.length - 1); //remove last child from array
                lastChild = $(children).last();
            }
            return lastChild;
        },

        clearDateField: function (item) {
            $(item).siblings("#facePlateAssignDueDate").val("");
            $(item).siblings("#facePlateAssignTime").val("");
            $('#cal-box #assignment-calendar#assignment-calendar').DatePickerSetDate('');
            $('.placeholderWrap').removeClass('placeholder-changed');
            $('.invaliddate').text('');
            ContentWidget.AssignmentDateSelected('');
        },

        hideItem: function (itemId) {
            var args = {
                id: itemId,
                parentId: ''
            };

            $.post(PxPage.Routes.content_item_hide, args, function (response) {

            });
        },

        showItem: function (itemId) {
            var args = {
                id: itemId,
                parentId: ''
            };

            $.post(PxPage.Routes.content_item_show, args, function (response) {

            });
        },

        BlockFacePlateUI: function () {
            var overlay = $('<div class="faceplate-overlay"> </div>');
            overlay.appendTo(document.body);
        },

        UnblockFacePlateUI: function () {
            $(".faceplate-overlay").remove();
        },
        
        ShowAssignmentsDueInWeek: function () {
            // disabling for now
            return false;
        },

        CheckForStudentView: function () {
            $(".editpagebtnwrp").hide();

            var key = "reloadlink";
            currentcookie = document.cookie;
            if (currentcookie.length > 0) {
                var firstidx = currentcookie.indexOf(key + "=");
                if (firstidx != -1) {
                    firstidx = firstidx + key.length + 1;
                    var lastidx = currentcookie.indexOf(";", firstidx);
                    if (lastidx == -1) {
                        lastidx = currentcookie.length;
                    }
                    document.cookie = key + '=; expires=Thu, 01-Jan-70 00:00:01 GMT;';
                    var fnelink = unescape(currentcookie.substring(firstidx, lastidx));
                    PxPage.Loading();
                    PxPage.LargeFNE.OpenFNELink(fnelink);
                    PxPage.Loaded();
                }
            }
        },

        ExpandRoot: function (itemId) {
            var root = $.fn.fauxtree("getOldestAncestors", targetId);
            var rootId = root.attr("data-ud-id");
            var rootSelector = $(".faceplate-unit-subitems .faux-tree-node[data-ft-id='" + rootId + "']");
            $.fn.fauxtree("toggleNode", rootSelector);
        },

        ShowCopyItemDialog: function () {
            var sourceId = $("#sourceId").val();
            var targetId = $(".movecopy-dialog .faux-tree-node.active").attr("data-ud-id");

            PxManagementCard.showEditContentTitleDialog(sourceId, targetId, "Copy Item", "", "copy");
        },
        
        ShowMoveCopyDialogFromFne: function () {
            var id = $("#fne-content .item-id").val();
            $(".faceplate-more-options-menu").hide();
            PxManagementCard.ShowMoveCopyDialog(id);
        },

        GetAveragePercentage: function () {
            if ($("#student-info .view-select").length != 0) {
                return;
            }
            var activeNode = $.fn.fauxtree("getActiveNode");
            var args = {
                id: activeNode.ftattr("id")
            };
            $.post(PxPage.Routes.get_average_percentage, args, function (response) {
                if (!response) {
                    return false;
                }
                if (response == 0 || response > 0) {
                    nodeId = activeNode.ftattr("id");
                    $(".faux-tree-node[data-ft-id=\"" + nodeId + "\"] .status-percentage").show();
                    $(".faux-tree-node[data-ft-id=\"" + nodeId + "\"] .status-percentage .average-percentage").width(response);
                }
            });
        },

        CloseDialog: function (event) {
            PxPage.TriggerHtmlSave();
            $('div.ui-dialog:visible').find(".ui-dialog-titlebar-close.ui-corner-all").click();
        },

        ConfirmMessage: function (mode) {
            return true;
        },

        UpdateItemAfterCopyAndMoveAndOpen: function (response) {
            PxFacePlate.UpdateItemAfterCopyAndMove(response, true);
        },

        UpdateItemAfterCopyAndMove: function (response, isOpen) {
            var parentid = $("#newParentId").val();
            var itemId = response.id; 
            $(".create-closecancel").click();
            $('.dialogclose').click();
            $(PxPage.switchboard).trigger("contentcopied", [itemId, null, parentid, function () {
                if ($("#fne-unblock-action-home").is(":visible") || isOpen == true) {
                    $("#fne-unblock-action-home").click();
                    $(PxPage.switchboard).trigger("opencontent", itemId);
                }
            } ]);
        },
        ClearSearchField: function () {
            $("#more-resources-search-box").val("");
            $("#more-resources-search-box").keyup();
        },

        StringSearch: function () {
            $("#more-resources-search-box").keyup();
        },

        SaveRssFeed: function (event, rssArticleUrl, targetId) {
            var articleTitle = $(".rssFeedParent[linkurl='" + rssArticleUrl + "']").attr("linktitle");
            var articleDescription = $(".rssFeedParent[linkurl='" + rssArticleUrl + "']").attr("linkdescription");
            var articlePubDate = $(".rssFeedParent[linkurl='" + rssArticleUrl + "']").attr("pubdate");
            var articleRssFeedUrl = $(".rssFeedParent[linkurl='" + rssArticleUrl + "']").attr("rssurl");
            if (articleTitle == undefined || rssArticleUrl == null) {
                articleTitle = $.trim($(".moreResourceItem[itemid='" + rssArticleUrl + "'] .fptitle .lnkMoreResourceItem").text());
            }

            var url = PxPage.Routes.saveRSSArticle;
            var data = {
                RSSFeedUrl: articleRssFeedUrl,
                ArticleLink: rssArticleUrl,
                ArticleTitle: articleTitle,
                ArticleDescription: articleDescription,
                ArticlePubDate: articlePubDate,
                parentId: targetId,
                sequence: $.fn.fauxtree("getLastSequence", targetId)
            };

            $.post(url, data, function (data) {
                var sourceId = data["RSSArticleId"];
                $(PxPage.switchboard).trigger("addexistingcontent", [sourceId, targetId]);
            });
        }
    };
}(jQuery);
