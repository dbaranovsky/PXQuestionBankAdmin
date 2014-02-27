// Full Screen FNE window overlay 
//
// Responible for displaying items in a full screen FNE
//
//

PxPage.LargeFNE = (function ($) {
    //privately scoped data and functions used by the plugin
    var _static = {
        fneOverlayElem: null, //element containing the overlay
        bodyScrollTop: 0, //stores current scrolling position of blocked background content
        fneIsVisible: false, //indicates FNE is already open, thus we don't need to create a new page
        activityTimestamp: 0, //stores the time when student begin taking an activity

        defaults: {
            opts: {
                //default blockUI options
                message: $('#fne-window'),
                position: 'fixed'
            }
        },
        modes: {
            view: "view",
            edit: "edit",
            results: "results",
            done: "done"
        },
        itemStates: {
            inCourseAssigned: "inCourse",
            inCourseUnassigned: "inCourseUnassigned",
            notInCourse: "notInCourse"
        },
        css: {

        },
        sel: {

        },
        fn: {

            onValidateNavigateAway: function () {
                if (PxPage.ValidateFneClosing()) {
                    $('#fne-window').removeClass('require-confirm-custom');
                    $(PxPage.switchboard).unbind('validateNavigateAway', _static.fn.onValidateNavigateAway);
                    PxPage.TriggerClick($('#fne-header #fne-done'));
                }
            },
            //Opens an FNE-link of a content item
            onOpenFNELink: function (anchor) {
                //takes an fne-link anchor element

                //set up hidden fields
                // setting a hidden field to track the active fne link
                var link = $(anchor).find(".fne-link").attr("href");
                if ($("#fne-window #content-link").length > 0) {
                    $("#fne-window #content-link").val(link);
                } else {
                    $("#fne-window").append('<input type="hidden" id="content-link" value="' + link + '" />');
                }
                // setting a hidden field to keep track of content id, this is useful for complicated content such as quiz
                if ($("#fne-window #hidden-content-id").length != 0) {
                    $("#fne-window #hidden-content-id").val($(anchor).attr("data-ft-id"));
                } else {
                    $("#fne-window").append('<input type="hidden" id="hidden-content-id" value="' + $(anchor).attr("data-ft-id") + '" />');
                }

                // setting a hidden field to keep track of which fne link was clicked
                if ($(" #fne-link-clicked-from").length != 0) {
                    $("#fne-link-clicked-from").val("faceplate-widget");
                } else {
                    $("body").append('<input type="hidden" id="fne-link-clicked-from" value="faceplate-widget" />');
                }
                // setting a hidden field to keep track of content id, this is useful for complicated content such as quiz
                if ($("#hidden-content-id").length != 0) {
                    $("#hidden-content-id").val($(anchor).attr("data-ft-id"));
                } else {
                    $("body").append('<input type="hidden" id="hidden-content-id" value="' + $(anchor).attr("data-ft-id") + '" />');
                }
            },
            onFneLoaded: function () {
                // Initialize the timer that contacts the server every 5 minutes
                // while the student is viewing the item
                PxPage.LargeFNE.InitStudentTimer();

                PxPage.LargeFNE.StudentPreviewSetup();

                _static.fn.GetSettingsList();

                // adjusting content-nav if student banner is visible in faceplate
                if ($(".product-type-faceplate .student-view-banner").length) {
                    $("#content-nav").css("margin-top", "60px");
                    $(".student-view-banner").css("width", "100%");
                }
                //fne-edit menu
                $("a#fne-edit").hover(function () {
                    clearTimeout(_static.editMenuTimeout);
                    _static.editMenuTimeout = 0;
                    _static.fn.ShowEditMenu($(this));
                }, function () {
                    _static.editMenuTimeout = setTimeout(_static.fn.HideEditMenu, 800);
                });
                $("#fne-edit-menu a").hover(function () {
                    clearTimeout(_static.editMenuTimeout);
                    _static.editMenuTimeout = 0;
                }, function () {
                    _static.editMenuTimeout = setTimeout(_static.fn.HideEditMenu, 800);
                });

                //if item is not in course, add current chapter name to header
                if ($("#fne-header-view.not-in-course").length > 0) {

                    if ($('#PX_MENU_ITEM_EBOOK').hasClass('active')) {
                        $('#fne-actions').hide();
                    }
                    else {
                        var activeChapterId = $(PxPage.switchboard).triggerHandler("getActiveChapterId");
                        var activeChapterName = $(PxPage.switchboard).triggerHandler("getActiveChapterName");
                        if (activeChapterId != null && activeChapterName != null) {
                            if (activeChapterName == "LaunchPad") {
                                $("#fne-header-view span#chapter-name").text('course');
                            }
                            else {
                                $("#fne-header-view span#chapter-name").text('"' + activeChapterName + '"');
                            }
                            $("#fne-header-view a#fne-add").bind("click", function () {
                                if ($("#fne-window input.item-id").length > 0) {
                                    //item is a content item
                                    var sourceId = $("#fne-window input.item-id").val();
                                    $(PxPage.switchboard).trigger("addexistingcontent", [sourceId, activeChapterId]);
                                } else if ($("#fne-window #fne-content iframe").length > 0) {
                                    //item is an external URL
                                    var rssArticleUrl = $("#fne-window #fne-content iframe").attr("src");
                                    $(PxPage.switchboard).trigger("saverssfeed", [rssArticleUrl, activeChapterId]);
                                }
                                PxPage.LargeFNE.CloseFNE();
                            });
                        }
                        else {
                            $('#fne-header-view span#chapter-name').text('course');
                            $('#fne-header-view a#fne-add').bind('click', PxPage.LargeFNE.AddLink);
                        }
                    }
                }

                PxPage.LargeFNE.ShowItemInFNE($("#fne-content #content-item .item-id").val());

                if ($('#fne-title').length > 0) {
                    document.title = $.trim($('#fne-title').text());
                }

                _static.fn.resizeHtmlBody();//resize body if neccessary
                
                if (!$(".faceplate-content-fne-header").is(":visible")) {
                    return false;
                }
            },
            onFneResize: function () {
            },
            //Resize size of body for touch devices
            resizeHtmlBody: function (iframeId) {
                if (PxPage.TouchEnabled()) { //for touchscreens, we set the height of the body = the height of the FNE window
                    var height = $("#fne-window").height();
                    $("body").height(height);
                }
            },
            onFneClosing: function () {
                if (typeof (fneTimer) != 'undefined' && fneTimer != null) {
                    // Stop the timer that sends updates to the server every 5 minutes
                    clearInterval(fneTimer);
                    // save the time student spent on this item
                    PxPage.LargeFNE.StoreStudentDuration();
                    fneTimer = null;
                }

                // resettting bmr z-index
                if ($(".instructor-console-wrapper").is(":visible")) {
                    $('#browseResultsPanel').css("z-index", "1000");
                }
                //TODO: this should be in the browse more resources widget
                // check to see whether we need to refresh the current page in faceplate
                if ($("#main #need-refresh").val() == "true") {
                    $("#main #need-refresh").val("");
                    window.location.reload();
                }
                document.title = "LaunchPad" + $(".coursetitle").length ? $(".coursetitle").text() : "Launch Pad Home";

                $("body").css("overflow", "auto");
                $("body").scrollTop(_static.bodyScrollTop); //scrolling here works in Chrome
                $("html").scrollTop(_static.bodyScrollTop); //scrolling here works in Firefox
               
            },
            ValidateFneClosingAttempt: function () {
                var fneClass = "";
                var quizValidation = true;

                var closeAssignmentFne = true;

                //check if dirty buffer is active and/or this is a quiz, disable closing
                if (fneClass == "quiz-editor" || (fneClass == "show-quiz")) {
                    quizValidation = false;
                }
                if (fneClass == "assignment-editor") {
                    closeAssignmentFne = false;
                }

                if (quizValidation == false) {
                    return false;
                }
                if (closeAssignmentFne == false) {
                    return false;
                }
                return true;
            },
            cleanUpFNE: function () {
                //var htmlScrollTop = $("body").scrollTop();
                $("html").removeClass("fne-scrollbars");
                $("body").css("overflow", "auto");

                if ($(".student-view-banner").length) {
                    $(".student-view-banner").css("width", "96%");
                }
                //$("body").scrollTop(htmlScrollTop); //Firefox needs to scroll back to original position after showing overflow
            },
            LaunchpadFNEPrep: function (url) {
                PxPage.FneInitHooks['content'] = ContentWidget.OnContentFneLoaded;

                //Xbook is using this now too, don't think we need that cleanup code as of right now.
                if ($('.product-type-faceplate').length > 0) {
                    PxPage.FneCloseHooks[""] = function () {
                        _static.fn.cleanUpFNE();
                    };
                }
                
                _static.bodyScrollTop = $("body").scrollTop();
                if (_static.bodyScrollTop == 0) {
                    //Browser fix: firefox reads "html" element instead
                    _static.bodyScrollTop = $("html").scrollTop();
                }
              
                if (PxPage.TouchEnabled()) {
                    //scroll the body to the top for touch devices, where the FNE-window is not a static element but an absolute element at the top of the page.
                    $("body").scrollTop(0); //for chrome
                    $("html").scrollTop(0); //for firefox
                }
                
                if ($(".student-view-banner").is(":visible")) {
                    $("#fne-title").css("top", "50px");
                    $("#fne-content").css("top", "50px");
                    $("#content-nav").css("margin-top", "60px");
                    $("#add-assignment-btn").hide();
                }
                //var htmlScrollTop = $("html").scrollTop();
                $("body").css("overflow", "hidden"); //hide overflow on body
                $("html").addClass("fne-scrollbars"); //hide overflow on html
                //$("body").scrollTop(htmlScrollTop);
                $('#fne-content').empty();

                return url;
            },

            ///Set mode: extrnal URL
            ExternalUrlPrep: function (href) {
                if ($(".add-rss-feed-button").length != 0) {
                    $(".add-activity-label").empty();
                    $(".faceplate-content-fne-title-left").append("<div class='add-activity-label px-default-text'><span>This activity is not in your course.</span><br/><input class='add-rss-feed-button' type='button' value='Add to course'></div>");
                    if ($(".faceplate-content-fne-title-left .current-rss-link").length) {
                        $(".faceplate-content-fne-title-left .current-rss-link").val(href);
                    } else {
                        $(".faceplate-content-fne-title-left").append("<input class='current-rss-link' type='hidden' value='" + href + "'>");
                    }
                } else {
                    $(".add-activity-label").empty();
                }
                if ($("#student-info .view-select").length == 0) {
                    $(".add-rss-feed-button").hide();
                }
            },
            OpenFneLink: function (href, title, isExternalUrl, callback, loadFullFne) {
                var useIFrame = false;
                if (isExternalUrl) {
                    useIFrame = true;
                    _static.fn.ExternalUrlPrep(href, title);
                }

                //remove fne-actions
                $("#fne-actions").remove();
                $(".fne-edit-tabs").remove();

                var argsFne = {
                    url: href,
                    title: title,
                    useLocal: false,
                    fixed: false,
                    requireConfirm: false,
                    autoSubmit: false,
                    hasChanges: false,
                    minimize: false,
                    useIFrame: useIFrame,
                    loadFullFne: loadFullFne
                };

                PxPage.OpenFNEWithHistory(argsFne);
                if (callback) {
                    callback();
                }
                return false;
            },

            NextItem: function () {
                if (typeof (fneTimer) != 'undefined' && fneTimer != null) {
                    // Stop the timer that sends updates to the server every 5 minutes
                    clearInterval(fneTimer);
                    // save the time student spent on this item
                    PxPage.LargeFNE.StoreStudentDuration();
                    fneTimer = null;
                }

                if ($("#nav-container .navigate-next").hasClass("disabled-navigation")) {
                    return;
                }
                $("#fne-window").block({
                    message: null
                });
                $(PxPage.switchboard).trigger("fneclickNextNodeTitle");
                $("#fne-window").unblock();
            },

            PreviousItem: function () {
                if (typeof (fneTimer) != 'undefined' && fneTimer != null) {
                    // Stop the timer that sends updates to the server every 5 minutes
                    clearInterval(fneTimer);
                    // save the time student spent on this item
                    PxPage.LargeFNE.StoreStudentDuration();
                    fneTimer = null;
                }

                if ($("#nav-container .navigate-back").hasClass("disabled-navigation")) {
                    return;
                }
                $("#fne-window").block({
                    message: null
                });
                $(PxPage.switchboard).trigger("fneclickPreviousNodeTitle");
                $("#fne-window").unblock();
            },
            ShowEditMenu: function ($this) {
                if (!$("#fne-edit-menu").is(":visible")) {
                    $("#fne-edit-menu").fadeIn('fast');
                    $("#fne-edit-menu").position({
                        my: "left top",
                        at: "left bottom",
                        of: $this,
                        collision: "none"
                    });
                }
            },
            HideEditMenu: function () {
                _static.editMenuTimeout = 0;
                $("#fne-edit-menu").fadeOut('slow');
                if ($(this)) {

                }

            },
            GetSettingsList: function (currentItem) {
                if ($('#ddlSettingsList').length > 0) {
                    $.get(PxPage.Routes.group_tab_list, function (data) {
                        var isFirst = false;
                        var options = "";


                        if (currentItem == null && $('#SettingsEntityId').length > 0) {
                            currentItem = $('#SettingsEntityId').val();
                        };

                        var dataOptions = $.map(data, function (item) {
                            options = "";
                            if (item.Value.indexOf("9999") != -1 && isFirst == false) {
                                isFirst = true;
                                options = ("<option disabled='disabled'>---------------------------</option>");
                            }

                            var value = item.Value.split(",")[1];
                            var selectedItem = item.Selected ? "selected='selected'" : "";

                            if (selectedItem == "") {
                                selectedItem = (currentItem == value) ? "selected='selected'" : "";
                            }

                            return options + "<option " + selectedItem + " value='" + value + "'>" + item.Text + "</option>";
                        });

                        $("#ddlSettingsList").empty().append("<option value='EntireClass'>Entire class</option>");
                        $("#ddlSettingsList").append(dataOptions.join(""));
                        $("#ddlSettingsList").append("<option disabled='disabled'>---------------------------</option>");
                        $("#ddlSettingsList").append("<option value='AddIndividual'>Add Individual</option>");
                        $("#ddlSettingsList").append("<option disabled='disabled'>---------------------------</option>");
                        $("#ddlSettingsList").append("<option value='ManageGroups'>Manage Groups</option>");
                    });
                }
            },
            OnChangeSettingsList: function (event) {
                var selectedVal = $('#ddlSettingsList').val();
                $('.studentName').val("");
                $('.spnNameError').hide();

                if (selectedVal == "AddIndividual") {
                    ContentWidget.ShowSearchRoster();
                }
                else if (selectedVal == "ManageGroups") {
                    event.href = PxPage.Routes.group_management;
                    event.preventDefault = function () { };
                    event.title = "Group Management";

                    PxPage.OpenFneLink(event);
                }
                else {
                    if (selectedVal == "EntireClass") {
                        var courseId = $('#CourseId').val();
                        selectedVal = courseId;
                        var urlContent = $(".assignTabGroupLink").attr('href');
                        if (urlContent != null) {
                            var urlA = urlContent.substring(0, urlContent.indexOf("groupId=") + 8);
                            var urlB = urlContent.substring(urlContent.indexOf("&includeNavigation"), urlContent.length);
                            var finalUrl = urlA + courseId + urlB;
                            $(".assignTabGroupLink").attr('href', finalUrl);
                        }
                    }

                    if ($(".assignTabGroupLink").length > 0 && $(".assignTabGroupLink").closest('li').hasClass('active')) {
                        PxPage.Loading();

                        var href = $(".assignTabGroupLink").attr("href");

                        if (href.indexOf("groupIdValue") < 0) {
                            var urlContent = $(".assignTabGroupLink").attr('href');
                            var urlA = urlContent.substring(0, urlContent.indexOf("groupId=") + 8);
                            var urlB = urlContent.substring(urlContent.indexOf("&includeNavigation"), urlContent.length);
                            var href = urlA + selectedVal + urlB;
                        }
                        else {
                            href = href.replace("groupIdValue", selectedVal);
                        }

                        $(".assignTabGroupLink").attr("href", href);

                        PxPage.TriggerClick($(".assignTabGroupLink"));
                    }
                    else if ($(".settingsTabGroupLink").length > 0 && $(".settingsTabGroupLink").closest('li').hasClass('active')) {
                        PxSettingsTab.UpdateQuizSettings($(event.target).val());
                    }
                    else {
                        var url = $('.content .bh-component').attr('rel');
                        var newUrl = PxSettingsTab.ReplaceString(url, selectedVal);
                        $(".content .bh-component iframe").remove();
                        $(".content .bh-component").attr('rel', newUrl);

                        PxPage.SetFrameApiHooks();
                    }
                }
            }
        }
    };

    return {
        Init: function () {
            $(PxPage.switchboard).bind("fneprep", function (args) {
                _static.fn.LaunchpadFNEPrep(args);
            });
            $(PxPage.switchboard).bind("fneloaded", function () {
                _static.fn.onFneLoaded();
            });
            $(PxPage.switchboard).bind("updatefnesize", function () {
                _static.fn.onFneResize();
            });
            $(PxPage.switchboard).bind("iframe-resized", _static.fn.resizeHtmlBody); //triggers when the content iframe height has been set
            
            $(PxPage.switchboard).bind("fneclosing", function () {
                _static.fn.onFneClosing();
            });
            $(PxPage.switchboard).bind("fneclosed", function () {
                //after the FNE-window has been closed, resize body to original size
                if (PxPage.TouchEnabled()) {
                    //set body height back to original value (body height = fne height on a touchscreen)
                    $("body").css("height", "");
                } else {
                    $("body .single-column").css("position", "static");
                }
            });
            $(PxPage.switchboard).bind("fneclosing-beforeunblock", function () {
                $('#fne-content').empty();
            });
            $(PxPage.switchboard).bind("clickFneLink", function (link) {
                _static.fn.onOpenFNELink(link);
            });

            $(PxPage.switchboard).bind("fnedonemode", function () {
                //take the done button and append it to the header
                var doneButton = $(".fne-hidden #fne-done").clone();
                $("#fne-header").prepend(doneButton);
                $(".fne-hidden").empty().append($("#fne-header #fne-actions"));
                $(".fne-hidden").empty().append($("#fne-header #fne-unblock-action-home"));
                
                // To avoid negative ramifications if other scripts might be attaching bebaviours to the same element
                // we specific only unbind this handler (reference of the function) to the element
                $(PxPage.switchboard).unbind('validateNavigateAway', _static.fn.onValidateNavigateAway);
                $(PxPage.switchboard).bind('validateNavigateAway', _static.fn.onValidateNavigateAway);
            });
            
            //Handle the done click in an FNE window.
            $(PxPage.switchboard).bind("fne-done-link", function () {
                //take itemurl and trigger FNE window

                var link = $(".fne-done-link");
                var triggerDone = function () {
                    //link.addClass("fne-link loadFullFne");
                    link.removeClass("fne-done-link");
                    link.attr("href", link.attr("itemurl"));
                };

                var valid = ContentWidget.ValidateNavigateAway(link, link, triggerDone);
                if (valid) {
                    window.setTimeout(function () {
                        PxPage.TriggerClick($(link));
                    }, 0);
                }
            });
            
            $(PxPage.switchboard).bind("contentResize", function (event, args) {
                if ($("#fne-window").is(":visible")) {
                    var width = args.width;
                    var height = args.height;
                    if (width)
                        $("#fne-content iframe#document-body-iframe").width(width);
                    if (height)
                        $("#fne-content iframe#document-body-iframe").height(height);
                }
                
            });
            $(PxPage.switchboard).bind("fnedonemode-exit", function () {
                var doneButton = $("#fne-header #fne-done");
                if (doneButton != null && doneButton.length > 0) {
                    PxPage.TriggerClick(doneButton);
                }

            });
            $(PxPage.switchboard).bind("componentcancelled", function (event, componentType, componentId, componentState, frame) {
                var fromPreviewDialog = $(frame).closest('.preview-question-dialog').length;
                if (fromPreviewDialog) {
                    $(frame).closest('.preview-question-dialog').dialog('close');
                }
                else {
                    $(PxPage.switchboard).trigger("fnedonemode-exit");
                }
            });

            $(document).off('click', '.navigate-next').on('click', '.navigate-next', _static.fn.NextItem);
            $(document).off('click', '.navigate-back').on('click', '.navigate-back', _static.fn.PreviousItem);

            // binding home icon in faceplate fne
            $(document).off('click', '#fne-unblock-action-home').on("click", '#fne-unblock-action-home', PxPage.UnBlock);

            $(PxPage.switchboard).bind("ValidateFneClosing", _static.fn.ValidateFneClosing);
            //open hook
            //on loaded hook
            PxPage.fneOpts = {
                css: {
                    message: $('#fne-window'),
                    position: 'fixed'
                }
            };
        },

        OpenFNELink: function (href, title, isExternalUrl, callback, loadFullFne) {
            return _static.fn.OpenFneLink(href, title, isExternalUrl, callback, loadFullFne);
        },

        CloseFNE: function (callback) {
            if (callback) {
                $(PxPage.switchboard).one('fneclosed', callback);
            }
            if ($('#fne-unblock-action-home:visible').length > 0) {
                $('#fne-unblock-action-home').trigger('click');
            }
        },

        //set up navigation buttons (prev/next) based on the item that is shown
        ShowItemInFNE: function (itemId) {
            var thisNode = $.fn.fauxtree("getNode", itemId);
            if (thisNode == null || thisNode.length == 0) {
                $("#nav-container #next.navigate-next").not('.calendar').addClass("disabled-navigation-button");
                $("#nav-container #back.navigate-back").not('.calendar').addClass("disabled-navigation-button");
                return false;
            }
            if (!$(thisNode).hasClass("active")) { //node was NOT opened via FNE-tree
                $(thisNode).addClass("active"); //ensure faux-tree node is active before getting next node
                $('.last-viewed').removeClass('last-viewed');
                $(thisNode).addClass("last-viewed");
            }
            var nextNode = $.fn.fauxtree("getNextNode", itemId);
            if (nextNode == null) {
                if (thisNode.ftattr('level') == 1) {
                    $("#nav-container #next.navigate-next").not('.calendar').addClass("disabled-navigation-button");
                    $("#nav-container #back.navigate-back").not('.calendar').addClass("disabled-navigation-button");
                }

                return false;
            }

            var href = nextNode.find(".fne-link").attr("href");
            var title = nextNode.find(".fne-link").text();

            var hasPrev = true;
            var hasNext = $.fn.fauxtree("hasNextNode", "hide-in-fne");

            hasPrev = thisNode.ftattr('level') > 1 && thisNode.prevAll("[data-ft-chapter=" + thisNode.ftattr('chapter') + "]").not('.item-type-pxunit').length > 0;
            hasNext = thisNode.ftattr('level') > 1 && thisNode.nextAll("[data-ft-chapter=" + thisNode.ftattr('chapter') + "]").not('.item-type-pxunit').length > 0;

            if (hasPrev == false) {
                $("#nav-container #back.navigate-back").not('.calendar').addClass("disabled-navigation-button");
            } else {
                $("#nav-container #back.navigate-back").not('.calendar').removeClass("disabled-navigation-button");
            }

            if (hasNext == null || hasNext == false) {
                $("#nav-container #next.navigate-next").not('.calendar').addClass("disabled-navigation-button");
            } else {
                $("#nav-container #next.navigate-next").not('.calendar').removeClass("disabled-navigation-button");
            }
        },
        InitStudentTimer: function () {

            if (typeof (fneTimer) != 'undefined' && fneTimer != null) {
                // Stop the timer if it persisted for some reason
                clearInterval(fneTimer);
                fneTimer = null;
            }

            if ((PxPage.Context.IsInstructor != "true") && ($("#fne-actions .pxunit-display-points").length > 0)) {
                var itemId = $("#fne-content #content-item .item-id").val();
                var duration = 300000; // 5 minutes

                if (itemId != null && itemId.length > 0) {
                    // Store the current time in the object
                    _static.activityTimestamp = Date.now();
                    // Set the timer to ping the server every 5 minutes
                    fneTimer = setInterval(PxPage.LargeFNE.StoreStudentDuration, duration);
                }
            }

            return;
        },
        StoreStudentDuration: function () {
            var itemId = $("#fne-content #content-item .item-id").val();
            var trackMinutesSpent = $('#fne-content .track-minutes-spent').val() == 'true';
            var entityId = $("#CourseId").val();
            var enrollmentId = $("#fne-content #content-item .enrollment-id").val();
            var startTimeMilliseconds = _static.activityTimestamp;
            var currentTimeMilliseconds = Date.now();
            var durationMilliseconds = currentTimeMilliseconds - startTimeMilliseconds;

            if (trackMinutesSpent && PxPage.Context.IsInstructor != "true" && $("#fne-actions .pxunit-display-points").length > 0 && itemId != null && itemId.length > 0 && entityId != null && entityId.length > 0 && enrollmentId != null && enrollmentId.length > 0 && startTimeMilliseconds != 0 && !isNaN(durationMilliseconds) && durationMilliseconds > 0) {
                var args = {
                    itemId: itemId,
                    durationMilliseconds: durationMilliseconds
                };

                // For testing, uncomment this to only call the server once
                //clearInterval(fneTimer);

                $.post(PxPage.Routes.set_content_duration, args, function() {
                    $(PxPage.switchboard).trigger("refreshitem", args);
                }).fail(function (jqXHR) {
                    $.ajax({
                        type: 'POST',
                        data: { errorName: 'StoreStudentDuration', errorMessage: 'data:' + JSON.stringify(args) + ', response: ' + jqXHR.responseText },
                        url: PxPage.Routes.log_javascript_errors
                    });

                });
            }

            return;
        },
        StudentPreviewSetup: function () {
            if ($(".student-view-banner").length > 0) {
                $("html, body").scrollTop(0);
                var topOffset = ($(".student-view-banner").outerHeight() - 1);
                $(".blockUI").css("top", topOffset + "px");
            }
        },
        GetSettingsList: function (currentItem) {
            _static.fn.GetSettingsList(currentItem);
        },
        OnChangeSettingsList: function (event) {
            _static.fn.OnChangeSettingsList(event);
        },
        AddLink: function (event) {
            PxPage.Loading('fne-window');

            var link = $('#fne-content').find('iframe').attr('src');
            var title = $(".rssFeedParent[linkurl='" + link + "']").attr("linktitle");
            var parentId = "PX_MULTIPART_LESSONS";
            var foldertitle = "Launchpad";

            var node = $.fn.fauxtree("getActiveNode");
            if (node.length != 0) {
                if (node.udattr('itemtype') == "PxUnit") {
                    parentId = node.ftattr('id');
                }
                else {
                    parentId = node.ftattr('parent');
                }

                if ($.trim(node.find('.fptitle').text()).length == 0) {
                    foldertitle = $.trim(node.find('.unitfptitle').text());
                }
                else {
                    foldertitle = $.trim(node.find('.fptitle').text());
                }
            }

            var url = PxPage.Routes.addLink;
            var data = {
                link: link,
                title: title,
                parentId: parentId
            };

            $.post(url, data, function (data) {
                var args = {
                    itemId: data.Id,
                    targetId: data.parentId,
                    assignedItem: {
                        id: data.Id
                    },
                    operation: "NewItem"
                };

                var data = {
                    itemId: args.itemId,
                    targetId: args.targetId,
                    assignedItem: args.assignedItem,
                    operation: args.operation
                };

                url = PxPage.Routes.launchpad_item_operation;
                dataType = "json";

                $.ajax({
                    url: url,
                    type: "POST",
                    data: JSON.stringify(data),
                    dataType: dataType,
                    contentType: "application/json; charset=utf-8",
                    complete: function () {
                        // if faux-tree exists update it
                        if ($('#PX_HOME_FACEPLATE_ZONE_MENUCONTENT').length > 0) {
                            url = PxPage.Routes.load_page_definiton + "?pageDefnId=PX_HOME_FACEPLATE_ASSIGNMENT_TAB";

                            $.get(url, null, function (response) {
                                $('#PX_HOME_FACEPLATE_ZONE_MENUCONTENT').html(response);
                                $.fn.fauxtree('toggleNode', targetId);
                                PxPage.Loaded('fne-window');
                            });
                        }
                        else {
                            PxPage.Loaded('fne-window');
                        }

                        var req = {
                            id: args.itemId
                        };

                        PxPage.openContent(req);

                        PxPage.Toasts.Success("Item has been added to " + foldertitle);
                    }
                });
            });

            return false;
        }
        
    };

} (jQuery));