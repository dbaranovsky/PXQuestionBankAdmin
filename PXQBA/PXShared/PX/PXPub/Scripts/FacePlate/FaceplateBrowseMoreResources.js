// FacePlateBrowseMoreResources
//
// This plugin is responsible for the client-side behaviors of the
// FacePlate's Browse More Resources window
var FacePlateBrowseMoreResources = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "FacePlateBrowseMoreResources",
        dataKey: "FacePlateBrowseMoreResources",
        bindSuffix: ".FacePlateBrowseMoreResources",
        dataAttrPrefix: "data-ud-",
        //default option values
        defaults: {
            defaultChapterId: "PX_MULTIPART_LESSONS",
            searchMode: "Resources"
        },
        //class names used by plugin
        css: {
            categoryTitle: "nav-category-title",
            active: "active",
            itemCount: "nav-category-item-count",
            category: "nav-category",
            fauxTree: "faux-tree"
        },
        // jquery selectors used for commonly accessed elements
        sel: {
            category: ".nav-category",
            categoryTitle: ".nav-category-title",
            categoryById: function (id) {
                return '.nav-category[data-ac-id="' + id + '"]';
            },
            active: ".nav-category.active",
            nodeTitle: ".faux-tree-node-title",
            node: ".faux-tree-node",

            faceplateHideMoreResourcesButton: "#browseResultsPanel .close",
            faceplateSearchResourcesLink: ".more-resources-search-link",
            pxUnitChapterLink: ".unitrowlevel1 .faux-tree-node-title",

            qtipTitle: "qtip_title"
        },
        //private functions
        fn: {
            //show overlay for more resources of a chapter
            showMoreResourcesWindow: function (event, mode, label) {

                if (mode == "disableediting") {
                    $('.faceplate-browse-resources').addClass('disableediting');
                }

                if (event != null && $(event.target).closest('.quiz-window-resources').length > 0) {
                    mode = 'learningCurve';
                }

                var chapterId = $(_static.sel.active).data("resourcesChapterId");
                set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                var learningCurveFlag = false;
                if (mode == "quiz" || mode == "learningCurve") {
                    chapterId = "";
                    if (mode == "learningCurve") {
                        learningCurveFlag = true;
                    }
                }

                //setup search
                if ($("#SearchBox").val() && $("#more-resources-search-box") != undefined && $("#more-resources-search-box").length != 0 && mode == "search") {
                    $("#more-resources-search-box").val($("#SearchBox").val());
                    $("#more-resources-search-box").keyup();
                }
                $("#more-resources-search-box").attr("placeholder", "Search Course");

                $("#more-resources-search-box").placeholder();

                $('#browseResultsPanel').addClass('closed').show();
                $('.faceplate-browse-resources').show();
                _static.fn.displayMoreResourcesVisibility();
                
                //Show and position the resources window
                if ($(".instructor-console-wrapper").is(":visible")) {
                    $('#browseResultsPanel').css("z-index", "1002");
                }

                $(window).resize();

                if (mode != undefined && mode == "search") {
                    _static.fn.updateMoreResourcesSearchView();
                    return;
                }
                if (mode != undefined && (mode == "quiz" || mode == "learningCurve")) {
                    $('#browseResultsPanel').addClass('quiz-window-resources');

                }
                else {
                    $('#browseResultsPanel').removeClass('quiz-window-resources');
                }

                var resultsUrl = ""; 
                if (mode == "none") {
                    if (label && label.length) {
                        $("#browseResultsPanel #browseResults").html(label);
                    }
                }
                else if (mode == "start") {
                    resultsUrl = PxPage.Routes.resource_type_list;
                }
                else if (mode == "rss") {
                    
                    //TODO: RSS feeds: launch RSS link

                } else if (mode == "removed") {
                    //                   
                    resultsUrl = PxPage.Routes.resource_removed;
                } else if (mode == "learningCurve") {
                    resultsUrl = PxPage.Routes.resource_facets_type;

                } else if (mode == "chapter" || mode == "type") {
                    if (label == null || label.length == 0) {
                        if (mode == "chapter") {
                            resultsUrl = PxPage.Routes.resource_facets_chapter;
                        }
                        else if (mode == "type") {
                            resultsUrl = PxPage.Routes.resource_facets_type;
                        }
                    } else {
                        if (mode == "chapter") {
                            _static.fn.getMoreResourcesResults(label, 'meta-topics/meta-topic', learningCurveFlag);
                        }
                        else if (mode == "type") {                            
                            _static.fn.getMoreResourcesResults(label, 'meta-content-type', learningCurveFlag);
                        }
                        return false;
                    }
                } else if (mode == "myresources") {
                    resultsUrl = PxPage.Routes.resource_mine;
                }
                if (resultsUrl.length > 0) {
                    PxPage.Loading("#browseResults");
                    $(".faceplate-browse-resources #browseResults").load(resultsUrl, function () {
                        PxPage.Loaded("#browseResults");
                    });
                }
                else {
                    if ($('#browseResultsPanel').hasClass('needsupdate')) {
                        _static.fn.updateResultsFromCurrentUrl();
                        $('#browseResultsPanel').removeClass('needsupdate');
                    }
                }

                if (event != null) {
                    event.preventDefault();
                }

            },
            // pops up the show more resource window when the serach button is pressed from header
            showMoreResourcesWindowFromHeaderSearch: function (event) {
                if ($("#more-resources-search-box").is(":visible")) {
                    $("#more-resources-search-box").val($("#SearchBox").val());
                    $("#more-resources-search-box").keyup();
                }

                _static.fn.showMoreResourcesWindow(event, "search");

            },
            // opens the show more resources window and loads the rss feeds
            showRssFeedsInshowMoreResourcesWindow: function (event) {
                _static.fn.showMoreResourcesWindow(event, "rss");
            },
            // opens the show more resources window and loads the rss feeds
            showRemovedMoreResourcesWindow: function (event) {
                _static.fn.showMoreResourcesWindow(event, "removed");
            },
            // adds the rss link from fne
            addRssFeedsFromFne: function (event) {
                if (confirm("Are you sure you want to add this RSS link to the currently selected unit?")) {
                    $("#fne-unblock-action-home").click();
                    var targetId = $(_static.sel.active).data("resourcesChapterId");
                    var rssArticleUrl = $(".current-rss-link").val();
                    $(PxPage.switchboard).trigger("saverssfeed", [rssArticleUrl, activeChapterId]);
                } else {
                    return false;
                }
            },

            updateHiddenFneLinkSource: function () {
                // setting a hidden field to keep track of which fne link was clicked
                if ($(".product-type-lms-faceplate #fne-link-clicked-from").length != 0) {
                    $(".product-type-lms-faceplate #fne-link-clicked-from").val("browse-more-resource");
                } else {
                    $(".product-type-lms-faceplate").append('<input type="hidden" id="fne-link-clicked-from" value="browse-more-resource" />');
                }

                if ($(".product-type-faceplate #fne-link-clicked-from").length != 0) {
                    $(".product-type-faceplate #fne-link-clicked-from").val("browse-more-resource");
                } else {
                    $(".product-type-faceplate").append('<input type="hidden" id="fne-link-clicked-from" value="browse-more-resource" />');
                }
            },

            getMoreResourcesResults: function (searchphrase, includedfields, learningCurveFlag) {
                var chapterId = $(_static.sel.active).data("resourcesChapterId");
                set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                if (chapterId == undefined) {
                    chapterId = "";
                }
                var data = {
                    ExactPhrase: searchphrase, 
                    MetaIncludeFields: includedfields, 
                    Start: '0',
                    Rows: '100',
                    chapterId: chapterId,
                    ebookOnly: false,
                    fromLearningCurve: learningCurveFlag
                };
                PxPage.Loading("browseResults");
                $('#browseResults').load(PxPage.Routes.resource_results + "?" + $.param(data), function () {
                    PxPage.Loaded("browseResults");
                });
                
            },
            getMyResources: function () {
                PxPage.Loading("browseResults");
                var chapterId = $(_static.sel.active).data("resourcesChapterId");
                set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                $('#browseResultsContent').load(PxPage.Routes.show_faceplate_resources_mine, chapterId, function () {
                    PxPage.Loaded("browseResults");
                });
            },
            getRemovedResources: function () {
                PxPage.Loading("browseResults");
                var chapterId = $(_static.sel.active).data("resourcesChapterId");
                set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                $('#browseResultsContent').load(PxPage.Routes.show_faceplate_resources_removed, chapterId, function () {
                    PxPage.Loaded("browseResults");
                });
            },
            getRssFeed: function () {
                PxPage.Loading("browseResults");
                var chapterId = $(_static.sel.active).data("resourcesChapterId");
                set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                var url = $('#moreResourcesTitle #selResourceType option:selected').val();
                var data = {
                    rssUrl: url,
                    chapterId: chapterId
                };

                $('#browseResultsContent').load(PxPage.Routes.show_faceplate_resources_rss, data, function () {
                    PxPage.Loaded("browseResults");
                });
            },
            showMoreResourcesResults: function (event) {
                $('[' + _static.sel.qtipTitle + ']').each(function () {
                    $(this).qtip({
                        content: { text: $(this).attr(_static.sel.qtipTitle) },
                        position: {
                            my: 'bottom left',
                            at: 'top center'
                        }
                    });
                });

                var chapterId = $(_static.sel.active).data("resourcesChapterId");
                set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                
                var showInUseMenu = function (event) {
                    var item = $(this).closest('.moreResourceItem');

                    var popup = $('#faceplate-move-resource-menu').clone();
                    $(popup).attr('id', item.attr('itemid'));
                    $(popup).attr('parentid', item.attr('parentid'));

                    if ($(this).hasClass('lblIncluded')) {
                        $(popup).find('#li_move').remove();
                        $(popup).find('#li_copy').remove();
                    }

                    $(item).qtip({
                        content: { text: $(popup) },
                        show: { solo: true },
                        hide: { event: 'unfocus' },
                        position: {
                            my: "top left",
                            at: "left bottom",
                            of: this,
                            collision: "none"
                        },
                        events: {
                            hide: function (event, api) {
                                $('.moreResourceItem').qtip('destroy');
                            }
                        }
                    });

                    $(item).qtip('show');

                    var parent = $(this).closest('.moreResourceItem').attr('parentname');

                    if (parent == "PX_MULTIPART_LESSONS") {
                        parent = "the Launchpad";
                    }

                    $('.faceplate-move-resource-menu #lblInUseDescription').html(parent);

                    event.stopPropagation();
                };

                if ($("#search-results.studentView").length == 0) {
                    //if instructor view
                    //attach events to INCLUDE/IN USE/ADD/REMOVE
                    var disableEditing = $(".PX_FACEPLATEHOMEPAGEWIDGET .faceplate-browse-resources").hasClass("disableediting");
                    if ($('#browseResultsPanel').hasClass('quiz-window-resources')) {
                        $('#search-results .moreResourceItem').hover(
                        function () {
                            //show ADD if item not added
                            $(".quiz-window-resources").find('.lblAdd, .lblInUse').hide();
                            var itemId = $(this).attr('itemId');
                            var itemExists = false;
                            $("#related-content-editor-question").find('.related-content-id').each(function (i, v) {
                                if ($(v).text() == itemId) {
                                    itemExists = true;
                                }
                            });
                            if (!itemExists) {
                                $(this).find('.lblAdd').css('display', 'block');
                            }
                            else {
                                $(this).find('.lblInUse').css('display', 'block');
                            }
                            //hide the add button from those which are already added.
                        });
                    }
                    else if (!disableEditing) {
                        $('#search-results .moreResourceItem').hover(
                        function () {
                            if ($(this).find('.lblInUse').is(':visible')) {
                                //create meny pointer if item is in use
                                //$(this).find('.lblAddInUse').css('display', 'block');
                                //$(this).find('.lblInUse').hide();
                                $(this).find('.lblInUse').click(showInUseMenu);
                            } else if ($(this).find('.lblIncluded').is(':visible')) {
                                //show remove if item is included
                                //$(this).find('.lblIncluded').hide();
                                //$(this).find('.lblRemove').css('display', 'block');
                            } else {
                                //show ADD if item not added
                                $(this).find('.lblAdd').css('display', 'block');
                            }
                        }, function () {
                            //show ADD if item not added
                            $(this).find('.lblAdd').hide();
                            if ($(this).find('.lblRemove').is(':visible')) {
                                //$(this).find('.lblRemove').hide();
                                //$(this).find('.lblIncluded').css('display', 'block');
                            }
                            if ($(this).find('.lblAddInUse').is(':visible')) {
                                $(this).find('.lblAddInUse').hide();
                                $(this).find('.lblInUse').css('display', 'block');
                                $(this).find('.lblAddInUse').unbind("click");
                            }
                        });
                    }
                }
                var targetId = chapterId;
                $('#search-results .moreResourceItem .lblAdd').click(function () {
                    var item = $(this).closest('.moreResourceItem');
                    var type = item.attr("itemtype");

                    if (type == "feed") { //RSS feed
                        //save RSS feed, get item
                        var url = PxPage.Routes.saveRSSArticle;
                        var data = {
                            RSSFeedUrl: item.attr("parentName"),
                            ArticleLink: item.attr("itemid"),
                            ArticleTitle: item.find("a").attr("title"),
                            ArticleDescription: item.find("a").attr("title"),
                            ArticlePubDate: item.find(".moreResourceRssDueDate").text(),
                            parentId: targetId,
                            sequence: $.fn.fauxtree("getLastSequence", targetId)
                        };
                        $.post(url, data, function (data) {
                            var sourceId = data["RSSArticleId"];
                            $(PxPage.switchboard).trigger("addexistingcontent", [sourceId, targetId]);
                        });
                    } else {
                        var sourceId = item.attr("itemid");

                        $(PxPage.switchboard).trigger("addexistingcontent", [sourceId, targetId]);
                        //$(this).siblings('.lblIncluded').show();
                        $(this).hide();
                    }


                });
                $(document).off('click', '.faceplate-move-resource-menu:visible #remove').on('click', '.faceplate-move-resource-menu:visible #remove', function () {
                    var sourceId = $(this).parents('.faceplate-move-resource-menu').attr('id');

                    $('.qtip-content').hide();
                    PxManagementCard.checkForSubmissionAndRemoveItem(sourceId);
                    //PxFacePlate.removeItem(sourceId);
                    //                    $(this).siblings('.lblIncluded').hide();
                    //                    var browseMoreResourceItem = $("#search-results .moreResourceItem[itemid='" + sourceId + "']");
                    //                    browseMoreResourceItem.attr("parentid", "FACEPLATE_REMOVED");
                    //                    $(this).hide();

                    return false;
                });
                $(document).off('click', '.faceplate-move-resource-menu:visible #move').on('click', '.faceplate-move-resource-menu:visible #move', function () {
                    var sourceId = $(this).parents('.faceplate-move-resource-menu').attr('id');
                    var sourceRootId = $(this).parents('.faceplate-move-resource-menu').attr("parentid");

                    var targetRootId = null;
                    $('.qtip-content').hide();
                    $(PxPage.switchboard).trigger("movenode", [sourceId, targetId, sourceRootId, null]);

                    return false;
                });
                $(document).off('click', '.faceplate-move-resource-menu:visible #copy').on('click', '.faceplate-move-resource-menu:visible #copy', function () {
                    var sourceId = $(this).parents('.faceplate-move-resource-menu').attr('id');
                    targetId = $(PxPage.switchboard).triggerHandler("getActiveChapterId");

                    $('.qtip-content').hide();
                    PxManagementCard.showEditContentTitleDialog(sourceId, targetId, "Name this copy", "browsemoredialog", "copy");

                    return false;
                });


                if ($('#PX_MENU_ITEM_EBOOK').hasClass('active')) {
                    $('.moreResourcesAction').css('visibility', 'hidden');
                }
                else {
                    $('.moreResourcesAction').css('visibility', 'visible');
                }
            },
            updateMoreResourcesChapterId: function () {                
                var node = $(this).closest(".unitrowlevel1");
                if (node.length == 0) {
                    var currentChapterId = $(PxPage.switchboard).triggerHandler("getActiveChapterId");
                    node = $.fn.fauxtree("getNode", currentChapterId);
                }
                var chapterId = null;
                if ($(node).ftattr("state") == "closed") {
                    chapterId = _static.defaults.defaultChapterId;
                } else {
                    chapterId = $(node).ftattr("id");
                }

                if (chapterId == undefined) {
                    chapterId = _static.defaults.defaultChapterId;
                }

                var category = $(_static.sel.active);

                if (category.data("resourcesChapterId") != chapterId) {
                    category.data("resourcesChapterId", chapterId);
                    set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                }
            },
            //close more resources window
            hideMoreResourcesWindow: function (event) {
                _static.fn.toggleMoreResourcesVisibility();
            },

            displayMoreResourcesVisibility: function () {
                $('#browseResultsPanel').show();
                $('#browseResultsPanel.closed').switchClass("closed", "open", 250, "easeOutCubic");
                //$('#browseResultsPanel.open').switchClass("open", "closed", 200, "easeInOutQuad");
            },

            //toggles and animates window
            toggleMoreResourcesVisibility: function () {
                $('#browseResultsPanel').show();
                $('#browseResultsPanel.closed').switchClass("closed", "open", 250, "easeOutCubic");
                $('#browseResultsPanel.open').switchClass("open", "closed", 200, "easeInOutQuad");
            },
            // updates the view within more resources to show the search option by keyword
            updateMoreResourcesSearchView: function () {
                if ($(".SearchBox").val()) {
                    $("#more-resources-search-box").val($(".SearchBox").val());
                    $("#more-resources-search-box").keyup();
                }
                //$(".more-resources-search-link").removeAttr("href");
                //$(".more-resources-link").attr("href", "#");
                $(".more-resources-string-search").addClass("show-more-resources-option");
                //$(".more-resources-string-search").show();
                //$(".more-resources-non-string-search").hide();
                //$("#browseResults").hide();
            },
            // updates the view within more resources to show the search option by keyword
            updateMoreResourcesRssFeedView: function () {
                var feedName = $.trim($("#PX_FacePlate_RSS_Feed_Widget .widgetHeaderText").text());
                $("#selResourceType option:contains(" + feedName + ")").attr('selected', 'selected');
                $("#selResourceType").change();
            },
            searchAndShowResults: function (clickedFromHeader) {
                var searchMode = _static.defaults.searchMode;
                var runSearch = (function () {
                    var searchString = $("#more-resources-search-box").val();

                    if (searchString == undefined || searchString == "") {
                        searchString = $("#SearchBox").val();
                    }

                    if (searchString == "Search course") {
                        $("#more-resources-search-box").val('');
                        searchString = '';
                    }

                    if (searchString == undefined || searchString == "" || searchString == null || searchString.length < 4 || (window.lastRunSearch == searchString && $("#search-results").length)) {
                        PxPage.Loaded("browseResults");
                        category.data("searchTimeout", null);
                        $('#browseResults').html("Search string too small");
                        return;
                    }

                    if (clickedFromHeader == true) {
                        searchString = $(".SearchBox").val();
                        $("#more-resources-search-box").val(searchString);
                    }
                    var chapterId = $(_static.sel.active).data("resourcesChapterId");
                    set_cookie($("#CourseId").val() + "_currentChapterId", chapterId, '0', '/');
                    var data = {
                        IncludeWords: searchString,
                        Start: '0',
                        Rows: '100',
                        chapterId: chapterId
                    };
                    //$("#browseResults").show();
                    window.lastRunSearch = searchString;
                    var url = PxPage.Routes.show_faceplate_resources_string_search;
                    if (searchMode == "questions") {
                        data["questionSearch"] = true;
                        data.Rows = '25';
                    }
                    var filters = $('.select2-search-choice div');
                    $('#browseResults').load(url, data, function (response) {
                        if (searchString != $("#more-resources-search-box").val()) {
                            //search string has been updated, run search again
                            runSearch();
                        }

                        //PxPage.Loaded();
                        PxPage.Loaded("browseResults");
                        category.data("searchTimeout", null);

                        $('#browseResults').html(response);
                        
                        //$(PxPage.switchboard).trigger("updatequestionfilter", [".available-questions "]);

                        // Reprocess any filters
                        if (filters != null && filters.length > 0) {
                            var filterSelectors = [];
                            var filterTexts = '';
                            var counter = 1;

                            filters.each(function () {
                                filterTexts += $(this).text() + ((counter === filters.length) ? '' : ',');
                                filterSelectors.push('.select2-search-choice div:contains("' + $(this).text() + '")');
                                ++counter;
                            });

                            // Put each filter that was in the filters box BEFORE the search back in the box
                            $('#question-filter-questiontype').val(filterTexts).trigger("change");

                            // Show the filters box again
                            $('.show-filter-available-question').trigger('click');

                            // Click it or the filters will be visible but the search
                            // won't actually be filtered 
                            $('#question-filter-questiontype').click();
                        }
                        //_static.fn.showMoreResourcesResults();
                    });
                });
                var category = $(_static.sel.active);
                if (category.data("searchTimeout") == undefined) {
                    var searchString = $("#more-resources-search-box").val();
                    if (searchString == undefined || searchString == "" || searchString == null || searchString.length < 4 || (window.lastRunSearch == searchString && $("#search-results").length)) {
                        return;
                    }
                    category.data("searchTimeout", runSearch);
                    setTimeout(runSearch, 1000);
                    PxPage.Loading("browseResults");
                } else {
                    category.data("searchTimeout", runSearch);
                    PxPage.Loading("browseResults");
                }
            },
            showLearningCurveResults: function () {
                PxPage.Loading();
                var targetId = $(this).closest('.moreResourceItem').attr("parentid");
                var itemId = $(this).attr("id");
                args = {
                    id: $(this).attr("id"),
                    mode: "Preview",
                    includeDiscussion: false,
                    includeNavigation: false,
                    renderDialog: true
                };
                $("#divShowResourceContent").load(PxPage.Routes.display_content, args, function () {
                    $(this).find("div #content-item").css("height", "100%");
                    $(this).find("div .contentwrapper").css("height", "100%");
                    $(this).find("div .content").css("height", "100%");
                    $(this).find("div #document-viewer-wrapper").css("height", "100%");
                    $(this).find("div #document-viewer").css("height", "100%");
                    $(this).find("div .document-body").css("height", "100%");
                    var dWidth = $(window).width() * 0.75;
                    var dHeight = $(window).height() * 0.9;
                    $("#divShowResourceContent").dialog("option", "width", dWidth);
                    $("#divShowResourceContent").dialog("option", "height", dHeight);
                    $("#divShowResourceContent").data("targetId", targetId);
                    $("#divShowResourceContent").data("itemId", itemId);
                    $("#divShowResourceContent").dialog("open");
                    PxPage.Loaded();
                });
            },
            //resets the more resources window to the initial resource type list
            resetMoreResourcesWindow: function () {
                var resultsUrl = PxPage.Routes.resource_type_list;
                if (resultsUrl.length > 0) {
                    PxPage.Loading("#browseResults");
                    $(".faceplate-browse-resources #browseResults").load(resultsUrl, function () {
                        PxPage.Loaded("#browseResults");
                    });
                }
            },
            //refreshes current view from the most recent search/source url
            updateResultsFromCurrentUrl: function () {
                if ($("#browse-resources-url").length > 0) {
                    var currentUrl = $("#browse-resources-url").val();

                    PxPage.Loading("#browseResults");

                    $("#browseResults").load(encodeURI(currentUrl), function () {
                        PxPage.Loaded("#browseResults");
                    });

                    $('#browseResultsPanel').removeClass('needsupdate');

                    return false;
                }
            },
            
            displayMessage: function(label) {
                
            }
        }
    },
    //public interface for interacting with this plugin
        api = {
            init: function (options) {
                return this.each(function () {
                    var settings = $.extend(true, {}, _static.defaults, options),
                        $this = $(this),
                        data = $this.data(_static.dataKey);
                    if (!data) {
                        $this.data(_static.dataKey, {
                            target: $this,
                            settings: settings
                        });
                        data = $this.data(_static.dataKey);
                    }
                    $(document).off('click', _static.sel.faceplateHideMoreResourcesButton, _static.fn.hideMoreResourcesWindow).on('click', _static.sel.faceplateHideMoreResourcesButton, _static.fn.hideMoreResourcesWindow);
                    
                    $(PxPage.switchboard).unbind("faceplateUpdateResourceChapterId");
                    $(PxPage.switchboard).bind("faceplateUpdateResourceChapterId", function (event) {
                        //update chapter id for more resources and trigger appropriate event
                        _static.fn.updateMoreResourcesChapterId();

                        if ($('#browseResultsPanel').hasClass('open')) {
                            _static.fn.updateResultsFromCurrentUrl();
                        }
                        else {
                            $('#browseResultsPanel').addClass('needsupdate');
                        }
                    });
                    _static.fn.updateMoreResourcesChapterId();
                    $(PxPage.switchboard).bind("hideQuestionBrowser", _static.fn.resetMoreResourcesWindow);
                    //search
                    $("#SearchBox").unbind("keypress");
                    $('#searchPanel #SearchBox').keypress(function (event) {
                        var keycode = (event.keyCode ? event.keyCode : event.which);
                        if (keycode == '13') {
                            _static.fn.showMoreResourcesWindowFromHeaderSearch();
                        }
                    });
                    // search
                    $(document).off('click', '#searchPanel #btnSearch').on('click', '#searchPanel #btnSearch', _static.fn.showMoreResourcesWindowFromHeaderSearch);
                    $(document).off('keyup', '#more-resources-search-box').on("keyup", '#more-resources-search-box', _static.fn.searchAndShowResults);
                    $(document).off('click', '#browseResultsPanel .modeResources').on("click", '#browseResultsPanel .modeResources', function () {
                        _static.defaults.searchMode = "resources";
                        window.lastRunSearch = "";
                        _static.fn.searchAndShowResults();
                        $(this).parent().addClass('active');
                        $('.menuResultsModes .questions').removeClass('active');

                    });
                    $(document).off('click', '#browseResultsPanel .modeQuestions').on("click", '#browseResultsPanel .modeQuestions', function () {
                        _static.defaults.searchMode = "questions";
                        window.lastRunSearch = "";
                        _static.fn.searchAndShowResults();
                        $(this).parent().addClass('active');
                        $('.menuResultsModes .resources').removeClass('active');

                    });
                    // end search
                    $(document).off('click', '.rss-more-resources-link').on('click', '.rss-more-resources-link', _static.fn.showRssFeedsInshowMoreResourcesWindow);
                    $(document).off('click', '.more-resources-removed').on('click', '.more-resources-removed', _static.fn.showRemovedMoreResourcesWindow);
                    $(document).off('click', ".add-rss-feed-button").on('click', ".add-rss-feed-button", _static.fn.addRssFeedsFromFne);
                    $(document).off('click', ".lnkMoreResourceItem").on("click", ".lnkMoreResourceItem", _static.fn.updateHiddenFneLinkSource);
                    $(document).off('click', ".learning_curve_resource").on("click", ".learning_curve_resource", _static.fn.showLearningCurveResults);

                    var resizeContentPanel = function () {
                        //40: title margin + buttons
                        //24: panel padding
                        var contentheight = $("#browseResultsPanel").height() - $("#moreResourcesTitle").height() - 48 - 24;
                        $("#browseResults").height(contentheight + 63);
                    };

                    $(window).resize(resizeContentPanel);

                }
                );
            },
            showMoreResourcesWindow: function (mode, label) {
                _static.fn.showMoreResourcesWindow(null, mode, label);
            },
            showMoreResourcesResults: function () {
                _static.fn.showMoreResourcesResults();
            },
            showMoreResourcesSearch: function (mode) {
                _static.defaults.searchMode = mode;
                window.lastRunSearch = "";
                _static.fn.searchAndShowResults();
                $(this).parent().addClass('active');
                //$('.menuResultsModes .resources').removeClass('active');
            }
        };
    //Handle the custome attributes
    $.fn.udattr = function (name, value) {
        var target = this.first(),
            aName = _static.dataAttrPrefix + name;
        if (value != null)
            target.attr(aName, value);
        return target.attr(aName);
    };
    
    // Associate the plugin with jQuery
    $.fn.FacePlateBrowseMoreResources = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
            return null;
        }
    };
} (jQuery);
