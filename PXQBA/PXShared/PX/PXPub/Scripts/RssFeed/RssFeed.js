var PxRssArticle = function ($) {
    return {
        // RSS FEED: Start
        OnClickSave: function (OnSuccess) {
            var RegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;
            if (PxPage.ValidateTitle(true)) {
                var bool = $("#inputRssUrl").hasClass('WaterMarkedTextBox');
                if (bool) {
                    $("#inputRssUrl").val('');
                    $("#inputRssUrl").removeClass('WaterMarkedTextBox');
                }
                if ($.trim($("#inputRssUrl").val()) != "") {
                    if (RegExp.test($.trim($("#inputRssUrl").val()))) {
                        PxContentTemplates.SetTemplateReloadMode('modal');
                        PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }, externalData: ContentWidget.CreateAndAssign,
                            updateSelector: '#content-item', success: OnSuccess
                        });
                    }
                    else {
                        PxPage.Toasts.Error("The specified URL is not a valid RSS feed.");
                        $('#inputRssUrl').focus();
                    }
                }
                else {
                    PxPage.Toasts.Error("You must specify a url.");
                    $('#inputRssUrl').focus();
                }
            }
        },

        // On Ready load for faceplate
        LoadFaceplateRssWidget: function (widgetId, rssRetrievalLimit, rssScrollingRestricted) {
            var args = {
                Id: widgetId,
                retrievalLimit: rssRetrievalLimit,
                scrollingRestricted: rssScrollingRestricted
            };
            $.post(PxPage.Routes.compactSummaryFacePlateRss, args, function (response) {
                $(".rss-data").html(response);
            });
        },

        OnClickSaveAndOpen: function (OnSuccess) {
            var RegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;
            if (PxPage.ValidateTitle()) {
                var bool = $("#inputRssUrl").hasClass('WaterMarkedTextBox');
                if (bool) {
                    $("#inputRssUrl").val('');
                    $("#inputRssUrl").removeClass('WaterMarkedTextBox');
                }
                if ($.trim($("#inputRssUrl").val()) != "") {
                    if (RegExp.test($.trim($("#inputRssUrl").val()))) {
                        PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }, externalData: ContentWidget.CreateAndAssign,
                            updateSelector: '#content-item', success: OnSuccess
                        });
                    }
                    else {
                        PxPage.Toasts.Error("The specified URL is not a valid RSS feed.");
                        $('#inputRssUrl').focus();
                    }
                }
                else {
                    PxPage.Toasts.Error("You must specify a url.");
                    $('#inputRssUrl').focus();
                }
            }
            else { return false; }
        },
        // RSS FEED: End


        // RSS Link(Widget): start

        BindControls: function () {
            // RSS FEED: Start
            $("#inputRssUrl").focus(function () {
                $(this).select();
            });

            $('#inputRssUrl').each(function () {
                if ($(this).val() == '') {
                    $(this).addClass('WaterMarkedTextBox');
                    $(this).val('e.g. “http://feeds.nytimes.com/nyt/rss/US”');
                }
            });


            $('#inputRssUrl').focusout(function (event) {
                event.stopImmediatePropagation();
                if ($(this).val() == '') {
                    $(this).addClass('WaterMarkedTextBox');
                    $(this).val('e.g. “http://feeds.nytimes.com/nyt/rss/US”');
                }
            });

            $('#inputRssUrl').click(function (event) {
                event.stopImmediatePropagation();
                var bool = $(this).hasClass('WaterMarkedTextBox');
                if (bool) {
                    $(this).val('');
                    $(this).removeClass('WaterMarkedTextBox');
                }
            });

            // RSS FEED: End

            $('.btnAssignRSSFeed, .lnkAssignRSSFeed').click(function (event) {
                event.stopImmediatePropagation();
                $("#maindivForDatePicker").remove();
                var widgetId = $(this).closest(".widgetItem").attr('id');
                var feedContainerId = $(this).closest(".rssFeedParent").attr('id');

                var args = {
                    callback: PxRssArticle.AssignDate,
                    customValues: { WidgetId: widgetId, FeedContainerId: feedContainerId },
                    calendarMode: 'single',
                    oldStartDate: '',
                    oldDueDate: ''
                };
                PxPage.ShowDatePicker(args);
            });


            $('.btnAssignRSSArticle').click(function (event) {
                event.stopImmediatePropagation();
                $("#maindivForDatePicker").remove();

                var widgetId = $(this).closest(".maindiv").attr('id');
                var feedContainerId = $(this).closest(".rssFeedParent").attr('id');
                var args = {
                    callback: PxRssArticle.ViewAllArticleAssignDate,
                    customValues: { WidgetId: widgetId, FeedContainerId: feedContainerId },
                    calendarMode: 'single',
                    oldStartDate: '',
                    oldDueDate: ''
                };
                PxPage.ShowDatePicker(args);
            });


            $('.btnUnassignRSSFeed, .lnkUnassignRSSFeed').click(function (event) {
                event.stopImmediatePropagation();
                var btnDiv = $(this).closest('.rssFeedParent');
                var widget = $(this).closest('.widgetItem');
                var widgetId = widget.attr('id'); ;
                PxPage.Loading(widgetId); //Load the spinner
                var unassignBtn = $(this);
                var bodyPostContent = $.ajax({
                    type: "POST",
                    url: PxPage.Routes.unassignRSSArticle,
                    data: { rssArticleId: btnDiv.find("#RSSArticleID").val() },
                    success: function (data) {
                        if (data.Result == "SUCCESS") {
                            btnDiv.find(".btnUnassignRSSFeed").hide();
                            btnDiv.find(".btnAssignRSSFeed").show();
                            btnDiv.find(".lnkUnassignRSSFeed").hide();
                            btnDiv.find(".lnkAssignRSSFeed").show();
                            btnDiv.find("#RSSArticleDueDate").val('');
                        }
                        PxPage.Loaded(widgetId);
                    }
                });
                $('.CompactButtonSetMenu').hide();
            });

            $('.btnUnassignRSSArticle').click(function (event) {
                event.stopImmediatePropagation();
                var btnDiv = $(this).closest('.rssFeedParent');
                var customRSSModal = $(this).closest('.customRSSModal');
                var customRSSModalId = customRSSModal.attr('id');
                PxPage.Loading(customRSSModalId); //Load the spinner
                var unassignBtn = $(this);
                var bodyPostContent = $.ajax({
                    type: "POST",
                    url: PxPage.Routes.unassignRSSArticle,
                    data: { rssArticleId: btnDiv.find("#RSSArticleID").val() },
                    success: function (data) {
                        if (data.Result == "SUCCESS") {
                            btnDiv.find(".btnUnassignRSSArticle").hide();
                            btnDiv.find(".btnAssignRSSArticle").show();
                            var maindiv = btnDiv.closest(".maindiv");
                            var assignedArticlId = maindiv.find("#assignedArticlId");
                            var unAssignedArticlId = maindiv.find("#unAssignedArticlId");

                            var oldUnAssignedIdValue = unAssignedArticlId.val();
                            unAssignedArticlId.val(oldUnAssignedIdValue + '$#$' + btnDiv.find("#RSSArticleID").val());
                            var oldAssignedIdValue = assignedArticlId.val();
                            assignedArticlId.val(oldAssignedIdValue.replace('$#$' + btnDiv.find("#RSSArticleID").val(), ""));
                            btnDiv.find("#RSSArticleDueDate").val('');
                        }
                        PxPage.Loaded(customRSSModalId); //Load the spinner
                    }
                });
            });

            $('.btnArchiveRSSFeed , .lnkArchiveRSSFeed').click(function (event) {
                event.stopImmediatePropagation();
                var archiveBtn = $(this);
                var btnDiv = $(this).closest('.rssFeedParent');
                var widget = $(this).closest('.widgetItem');
                var widgetId = widget.attr('id'); ;
                PxPage.Loading(widgetId); //Load the spinner
                var bodyPostContent = $.ajax({
                    type: "POST",
                    url: PxPage.Routes.archiveRSSArticle,
                    data: { RSSFeedUrl: btnDiv.attr('RssUrl'), ArticleLink: btnDiv.attr('LinkUrl'), ArticleTitle: btnDiv.attr('LinkTitle'), ArticleDescription: btnDiv.attr('LinkDescription'), ArticlePubDate: btnDiv.attr('PubDate') },
                    success: function (data) {
                        //alert(data.RSSArticleId);
                        btnDiv.find("#RSSArticleID").val(data.RSSArticleId);
                        archiveBtn.hide();
                        archiveBtn.siblings(".btnUnarchiveRSSFeed").show();
                        archiveBtn.siblings(".lnkUnarchiveRSSFeed").show();
                        widget.find(".customRSSViewAllBarArticleCount").text(parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) + 1);
                        if (parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) > 0) {
                            widget.find(".customRSSViewAllBarLink").text("View all");
                        }
                        PxPage.Loaded(widgetId);
                    }
                });
                $('.CompactButtonSetMenu').hide();
            });

            $('.btnUnarchiveRSSFeed, .lnkUnarchiveRSSFeed').click(function (event) {
                event.stopImmediatePropagation();
                var btnDiv = $(this).closest('.rssFeedParent');
                var widget = $(this).closest('.widgetItem');
                var widgetId = widget.attr('id'); ;
                PxPage.Loading(widgetId); //Load the spinner
                var unarchiveBtn = $(this);
                var bodyPostContent = $.ajax({
                    type: "POST",
                    url: PxPage.Routes.unarchiveRSSArticle,
                    data: { rssArticleId: btnDiv.find("#RSSArticleID").val(), IsConfirm: false },
                    success: function (data) {
                        if (data.ControllerResponse == "ShowConfirmMessage") {
                            var bool = confirm('Students have received grades for this item. \nUnarchiving it will remove it from the course and permanently delete all associated student grades. \nAre you sure you want to unarchive this item?');
                            if (!bool) {
                                return;
                            }
                            var bodyPostContent = $.ajax({
                                type: "POST",
                                url: PxPage.Routes.unarchiveRSSArticle,
                                data: { rssArticleId: btnDiv.find("#RSSArticleID").val(), IsConfirm: true },
                                success: function (data) {
                                    if (data.ControllerResponse == "ItemDeleted") {
                                        unarchiveBtn.hide();
                                        btnDiv.find(".btnArchiveRSSFeed").show();
                                        btnDiv.find(".lnkArchiveRSSFeed").show();
                                        btnDiv.find(".btnUnassignRSSFeed").hide();
                                        btnDiv.find(".btnAssignRSSFeed").show();
                                        btnDiv.find(".lnkUnassignRSSFeed").hide();
                                        btnDiv.find(".lnkAssignRSSFeed").show();
                                        btnDiv.find("#RSSArticleID").val('');
                                        btnDiv.find("#RSSArticleDueDate").val('');
                                        widget.find(".customRSSViewAllBarArticleCount").text(parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) - 1);
                                        if (parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) == 0) {
                                            widget.find(".customRSSViewAllBarLink").text("");
                                        }
                                    }
                                }
                            });
                        }
                        else if (data.ControllerResponse == "ItemDeleted") {
                            unarchiveBtn.hide();
                            btnDiv.find(".btnArchiveRSSFeed").show();
                            btnDiv.find(".lnkArchiveRSSFeed").show();
                            btnDiv.find(".btnUnassignRSSFeed").hide();
                            btnDiv.find(".btnAssignRSSFeed").show();
                            btnDiv.find(".lnkUnassignRSSFeed").hide();
                            btnDiv.find(".lnkAssignRSSFeed").show();
                            btnDiv.find("#RSSArticleID").val('');
                            btnDiv.find("#RSSArticleDueDate").val('');
                            widget.find(".customRSSViewAllBarArticleCount").text(parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) - 1);
                            if (parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) == 0) {
                                widget.find(".customRSSViewAllBarLink").text("");
                            }

                        }
                        PxPage.Loaded(widgetId);
                    }
                });
                $('.CompactButtonSetMenu').hide();
            });

            $('.btnUnarchiveRSSArticle').click(function (event) {
                event.stopImmediatePropagation();
                var btnDiv = $(this).closest('.rssFeedParent');
                var customRSSModal = $(this).closest('.customRSSModal');
                var customRSSModalId = customRSSModal.attr('id');
                PxPage.Loading(customRSSModalId); //Load the spinner
                var unarchiveBtn = $(this);
                var bodyPostContent = $.ajax({
                    type: "POST",
                    url: PxPage.Routes.unarchiveRSSArticle,
                    data: { rssArticleId: btnDiv.find("#RSSArticleID").val(), IsConfirm: false },
                    success: function (data) {
                        if (data.ControllerResponse == "ShowConfirmMessage") {
                            var bool = confirm('Students have received grades for this item. \nUnarchiving it will remove it from the course and permanently delete all associated student grades. \nAre you sure you want to unarchive this item?');
                            if (!bool) {
                                return;
                            }
                            var bodyPostContent = $.ajax({
                                type: "POST",
                                url: PxPage.Routes.unarchiveRSSArticle,
                                data: { rssArticleId: btnDiv.find("#RSSArticleID").val(), IsConfirm: true },
                                success: function (data) {
                                    if (data.ControllerResponse == "ItemDeleted") {
                                        var maindiv = btnDiv.closest(".maindiv");
                                        var unArchivedArticlId = maindiv.find("#unArchivedArticlId");
                                        var oldValue = unArchivedArticlId.val();
                                        unArchivedArticlId.val(oldValue + '$#$' + btnDiv.find("#RSSArticleID").val());
                                        btnDiv.remove();
                                        maindiv.find(".totalArchivedArticlesViewAll").val(parseInt(maindiv.find(".totalArchivedArticlesViewAll").val()) - 1);
                                        if (parseInt(maindiv.find(".totalArchivedArticlesViewAll").val()) == 0) {
                                            maindiv.find("#NoArchivedArticleSpan").removeClass("NoArticleMessageNotVisible");
                                        }
                                        PxPage.Loaded(customRSSModalId); //Load the spinner
                                    }
                                }
                            });
                        }
                        else if (data.ControllerResponse == "ItemDeleted") {
                            var maindiv = btnDiv.closest(".maindiv");
                            var unArchivedArticlId = maindiv.find("#unArchivedArticlId");
                            var oldValue = unArchivedArticlId.val();
                            unArchivedArticlId.val(oldValue + '$#$' + btnDiv.find("#RSSArticleID").val());
                            btnDiv.remove();
                            maindiv.find(".totalArchivedArticlesViewAll").val(parseInt(maindiv.find(".totalArchivedArticlesViewAll").val()) - 1);
                            if (parseInt(maindiv.find(".totalArchivedArticlesViewAll").val()) == 0) {
                                maindiv.find("#NoArchivedArticleSpan").removeClass("NoArticleMessageNotVisible");
                            }
                            PxPage.Loaded(customRSSModalId); //Load the spinner
                        }
                    }
                });
            });

            $('.customRssViewAllLink').click(function (event) {
                event.stopImmediatePropagation();
                var viewAllRSSArticle = PxPage.Routes.viewAllRSSArticle;
                var widget = $(this).closest(".widgetItem");
                var widgetId = widget.attr("id");
                var widgetHeader = widget.find(".widgetHeader");
                var widgetHeaderText = widgetHeader.text();

                var templateid = widget.attr("templateid");
                if (templateid == "PX_ScientificAmerican_RSS_Feed") {
                    widgetHeaderText = "Scientific American";
                }

                var customRSSModal = $('.customRSSModal').first();
                $(customRSSModal).html('<div style="padding-top:10px;padding-bottom:10px;">Loading RSS Articles..</div>');
                $(customRSSModal).attr('id', 'customRSSModal');
                $(customRSSModal).dialog({ width: 1000, height: 400, minWidth: 1000, minHeight: 400, modal: true, draggable: false, closeOnEscape: true, title: widgetHeaderText + ' >>> Archived Articles',
                    close: function () {

                        var assignedArticleIds = $(customRSSModal).find("#assignedArticlId").val().split('$#$');
                        jQuery.each(assignedArticleIds, function () {
                            var article = widget.find("input[value ='" + this + "']");
                            if (this != '') {
                                var btnDiv = article.closest('.rssFeedParent');
                                btnDiv.find(".btnUnassignRSSFeed").show();
                                btnDiv.find(".btnAssignRSSFeed").hide();

                                btnDiv.find(".lnkUnassignRSSFeed").show();
                                btnDiv.find(".lnkAssignRSSFeed").hide();
                            }
                        });

                        var unAssignedArticleIds = $(customRSSModal).find("#unAssignedArticlId").val().split('$#$');
                        jQuery.each(unAssignedArticleIds, function () {
                            var article = widget.find("input[value ='" + this + "']");
                            if (this != '') {
                                var btnDiv = article.closest('.rssFeedParent');
                                btnDiv.find(".btnUnassignRSSFeed").hide();
                                btnDiv.find(".btnAssignRSSFeed").show();

                                btnDiv.find(".lnkUnassignRSSFeed").hide();
                                btnDiv.find(".lnkAssignRSSFeed").show();
                                btnDiv.find("#RSSArticleDueDate").val('');
                            }
                        });

                        var unArchivedArticleIds = $(customRSSModal).find("#unArchivedArticlId").val().split('$#$');
                        jQuery.each(unArchivedArticleIds, function () {
                            var article = widget.find("input[value ='" + this + "']");
                            if (this != '') {
                                var btnDiv = article.closest('.rssFeedParent');
                                btnDiv.find(".btnArchiveRSSFeed").show();
                                btnDiv.find(".btnUnarchiveRSSFeed").hide();
                                btnDiv.find(".btnUnassignRSSFeed").hide();
                                btnDiv.find(".btnAssignRSSFeed").show();

                                btnDiv.find(".lnkArchiveRSSFeed").show();
                                btnDiv.find(".lnkUnarchiveRSSFeed").hide();
                                btnDiv.find(".lnkUnassignRSSFeed").hide();
                                btnDiv.find(".lnkAssignRSSFeed").show();

                                article.val('');
                                btnDiv.find("#RSSArticleDueDate").val('');
                                widget.find(".customRSSViewAllBarArticleCount").text(parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) - 1);
                            }
                        });
                        if (parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) <= 0) {
                            widget.find(".customRSSViewAllBarLink").text("");
                            widget.find(".customRSSViewAllBarArticleCount").text("0");
                        }
                    }
                });

                var bodyPostContent = $.ajax({
                    type: "GET",
                    url: viewAllRSSArticle,
                    data: { widgetId: widgetId },
                    success: function (widgetHtml) {
                        $(customRSSModal).html(widgetHtml);
                    },
                    error: function (req, status, error) {
                        alert("ERROR_VIEW_ALL_RSS_ARTICLES");
                    }
                });

            });

            $('.customRSSWidgetMaindiv').scroll(function (event) {
                if ($("#PX_FacePlate_RSS_Feed_Widget .retrieval-cap") != undefined && $("#PX_FacePlate_RSS_Feed_Widget .retrieval-cap").length != 0) {
                    var feedCap = $("#PX_FacePlate_RSS_Feed_Widget .retrieval-cap").val();
                    if (feedCap > 0) {
                        event.stopImmediatePropagation();
                        return false;
                    }
                }
                event.stopImmediatePropagation();

                var elem = $(this).closest('.customRSSWidgetMaindiv');

                if (elem[0].scrollHeight - elem.scrollTop() == elem.outerHeight()) {
                    PxRssArticle.GetRssFeeds(elem);
                }

                $('.CompactButtonSetMenu').hide();
            });

            $(".more-rss-feed").click(function (event) {
                var elem = $(this).closest('.customRSSWidgetMaindiv');
                var widget = elem.closest('.widgetItem');
                var bodyPostContent = $.ajax({
                    type: "POST",
                    url: PxPage.Routes.partialListViewCustomRss,
                    data: { widgetId: widget.attr('itemid'), currentPageIndex: parseInt(elem.attr("PageIndex")) },
                    success: function (rssFeeds) {
                        if (rssFeeds.Message != 'NoMoreRSSFeeds') {
                            elem.append(rssFeeds);
                            elem.attr("PageIndex", parseInt(elem.attr("PageIndex")) + 1);
                        }
                        else if (rssFeeds.Message == 'NoMoreRSSFeeds') {
                            elem.append('<div class="CustomRSSLoadStatus">There are no more articles in this feed...</div>');
                        }
                    }
                });
            });

            $('.CompactButtonSetContainer').click(function (event) {
                event.stopImmediatePropagation();
                $('.CompactButtonSetMenu').hide();
                $(this).next('.CompactButtonSetMenu').show();
            });

            $(".CompactButtonSetMenu").hover(
               function () {
                   $(this).show();
               },
                function () {
                    $(this).hide();
                    if ($("#maindivForDatePicker").attr("mode") != "modal") {
                        $("#maindivForDatePicker").remove();
                    }
                }
            );

            $(".btnUnarchiveRSSFeed").hover(
               function () {
                   $(this).find("input").addClass('ArchiveButtonOnMouseOver');
                   $(this).find("input").val("Unarchive");
               },
                function () {
                    $(this).find("input").removeClass('ArchiveButtonOnMouseOver');
                    $(this).find("input").val("Archived");
                }
            );

            $(".lnkArchiveRSSFeed").hover(
               function () {
                   $(this).find(".lnkButtonSet").addClass('ArchiveButtonOnMouseOver');
               },
                function () {
                    $(this).find(".lnkButtonSet").removeClass('ArchiveButtonOnMouseOver');
                }
            );

            $(".lnkUnarchiveRSSFeed").hover(
               function () {
                   $(this).find(".lnkButtonSet").addClass('ArchiveButtonOnMouseOver');
                   $(this).find(".lnkButtonSet").text("Unarchive");
               },
                function () {
                    $(this).find(".lnkButtonSet").removeClass('ArchiveButtonOnMouseOver');
                    $(this).find(".lnkButtonSet").text("Archived");
                }
            );

            $(".btnArchiveRSSFeed").hover(
               function () {
                   $(this).find("input").addClass('ArchiveButtonOnMouseOver');
               },
                function () {
                    $(this).find("input").removeClass('ArchiveButtonOnMouseOver');
                }
            );

            $(".btnUnarchiveRSSArticle").hover(
               function () {
                   $(this).find("input").addClass('ArchiveButtonOnMouseOver');
                   $(this).find("input").val("Unarchive");
               },
                function () {
                    $(this).find("input").removeClass('ArchiveButtonOnMouseOver');
                    $(this).find("input").val("Archived");
                }
            );

            $('.FeedURL').each(function () {
                if ($(this).find("#RssFeedUrl").val() == '') {
                    $(this).find("#RssFeedUrl").addClass('WaterMarkedTextBox');
                    $(this).find("#RssFeedUrl").val('e.g. “http://feeds.nytimes.com/nyt/rss/US”');
                }
            });


            $('.FeedURL').focusout(function (event) {
                event.stopImmediatePropagation();
                if ($(this).find("#RssFeedUrl").val() == '') {
                    $(this).find("#RssFeedUrl").addClass('WaterMarkedTextBox');
                    $(this).find("#RssFeedUrl").val('e.g. “http://feeds.nytimes.com/nyt/rss/US”');
                }
            });

            $('.FeedURL').click(function (event) {
                event.stopImmediatePropagation();
                var bool = $(this).find("#RssFeedUrl").hasClass('WaterMarkedTextBox');
                if (bool) {
                    $(this).find("#RssFeedUrl").val('');
                    $(this).find("#RssFeedUrl").removeClass('WaterMarkedTextBox');
                }
            });

            $(".rssFeedParent").hover(
               function () {
                   $(this).find(".feedContainer").addClass('RSSArticleRow');
                   $(this).find(".compactFeedContainer").addClass('RSSArticleRow');
               },
                function () {
                    $(this).find(".feedContainer").removeClass('RSSArticleRow');
                    $(this).find(".compactFeedContainer").removeClass('RSSArticleRow');
                }
            );

            $(".buttonRSS").hover(
               function () {
                   //$("#maindivForDatePicker").show();
               },
               function () {
                   if ($("#maindivForDatePicker").attr("mode") != "modal") {
                       $("#maindivForDatePicker").remove();
                   }
               }
            );

            $(".btnAssignRSSFeed, .btnAssignRSSArticle").hover(
               function () {
                   $(this).find("input").addClass('AssignButtonOnMouseOver');
               },
                function () {
                    $(this).find("input").removeClass('AssignButtonOnMouseOver');
                }
            );

            $(".btnUnassignRSSFeed").hover(
               function () {
                   $(this).find("input").addClass('AssignButtonOnMouseOver');
                   $(this).find("input").val("Unassign");
               },
                function () {
                    $(this).find("input").removeClass('AssignButtonOnMouseOver');
                    $(this).find("input").val("Assigned");
                }
            );

            $(".btnUnassignRSSArticle").hover(
               function () {
                   $(this).find("input").addClass('AssignButtonOnMouseOver');
                   $(this).find("input").val("Unassign");
               },
                function () {
                    $(this).find("input").removeClass('AssignButtonOnMouseOver');
                    $(this).find("input").val("Assigned");
                }
            );

            $(".lnkUnassignRSSFeed").hover(
               function () {
                   $(this).find(".lnkButtonSet").addClass('AssignButtonOnMouseOver');
                   $(this).find(".lnkButtonSet").text("Unassign");
               },
                function () {
                    $(this).find(".lnkButtonSet").removeClass('AssignButtonOnMouseOver');
                    $(this).find(".lnkButtonSet").text("Assigned");
                }
            );

            $(".lnkAssignRSSFeed").hover(
               function () {
                   $(this).find(".lnkButtonSet").addClass('AssignButtonOnMouseOver');
               },
                function () {
                    $(this).find(".lnkButtonSet").removeClass('AssignButtonOnMouseOver');
                }
            );
        },

        GetRssFeeds: function (elem) {
            var widget = elem.closest('.widgetItem');
            var statusbar = widget.find('.customRSSViewAllBar');

            statusbar.append('<div class="CustomRSSLoadMessageSpinner"></div>');
            statusbar.append('<div class="CustomRSSLoadMessage">Loading</div>');

            var pageIndex = 0;

            if (elem.attr("PageIndex") != undefined) {
                pageIndex = parseInt(elem.attr("PageIndex"));
            }

            var bodyPostContent = $.ajax({
                type: "POST",
                url: PxPage.Routes.partialListViewCustomRss,
                data: { widgetId: widget.attr('itemid'), currentPageIndex: pageIndex },
                success: function (rssFeeds) {
                    elem.find('.CustomRSSLoadStatus').remove();
                    statusbar.find('.CustomRSSLoadMessage').remove();
                    statusbar.find(".CustomRSSLoadMessageSpinner").remove();

                    if (rssFeeds.Message != 'NoMoreRSSFeeds') {
                        elem.append(rssFeeds);
                        elem.attr("PageIndex", parseInt(elem.attr("PageIndex")) + 1);
                    }
                    else if (rssFeeds.Message == 'NoMoreRSSFeeds') {
                        elem.append('<div class="CustomRSSLoadStatus">There are no more articles in this feed...</div>');
                    }
                }
            });
        },

        OpenRSSLink: function (id) {
            PxPage.TriggerClick($('#' + id));
        },

        ViewAllArticleAssignDate: function (dateArgs, CustomValues) {
            var widget = $("#" + CustomValues.WidgetId);
            var widgetId = CustomValues.WidgetId;
            var customRSSModal = widget.closest('.customRSSModal');
            var customRSSModalId = customRSSModal.attr('id');
            var btnDiv = customRSSModal.find("#" + CustomValues.FeedContainerId);
            PxPage.Loading(customRSSModalId); //Load the spinner
            var bodyPostContent = $.ajax({
                type: "POST",
                url: PxPage.Routes.assignRSSArticle,
                data: { RSSFeedUrl: btnDiv.attr('RssUrl'), ArticleLink: btnDiv.attr('LinkUrl'), ArticleTitle: btnDiv.attr('LinkTitle'), ArticleDescription: btnDiv.attr('LinkDescription'), ArticlePubDate: btnDiv.attr('PubDate'), RssArticleId: btnDiv.find("#RSSArticleID").val(), RssAssignDate: dateArgs.DueDate },
                success: function (data) {
                    btnDiv.find(".btnUnarchiveRSSArticle").show();
                    btnDiv.find(".btnArchiveRSSArticle").hide();
                    btnDiv.find(".lnkUnarchiveRSSArticle").show();
                    btnDiv.find(".lnkArchiveRSSArticle").hide();
                    btnDiv.find(".btnUnassignRSSArticle").show();
                    btnDiv.find(".btnAssignRSSArticle").hide();
                    btnDiv.find(".lnkUnassignRSSArticle").show();
                    btnDiv.find(".lnkAssignRSSArticle").hide();

                    btnDiv.find("#RSSArticleDueDate").val(data.DueDate);

                    if (btnDiv.find("#RSSArticleID").val() != data.RSSArticleId) {
                        btnDiv.find("#RSSArticleID").val(data.RSSArticleId);
                        widget.find(".customRSSViewAllBarArticleCount").text(parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) + 1);
                        if (parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) > 0) {
                            widget.find(".customRSSViewAllBarLink").text("View all");
                        }
                    }

                    var maindiv = btnDiv.closest(".maindiv");
                    var assignedArticlId = maindiv.find("#assignedArticlId");
                    var unAssignedArticlId = maindiv.find("#unAssignedArticlId");

                    var oldAssignedIdValue = assignedArticlId.val();
                    assignedArticlId.val(oldAssignedIdValue + '$#$' + btnDiv.find("#RSSArticleID").val());
                    var oldUnAssignedIdValue = assignedArticlId.val();
                    unAssignedArticlId.val(oldUnAssignedIdValue.replace('$#$' + btnDiv.find("#RSSArticleID").val(), ""));

                    PxPage.Loaded(customRSSModalId);
                }
            });
            $('.CompactButtonSetMenu').hide();
        },

        AssignDate: function (dateArgs, CustomValues) {

            var widget = $("#" + CustomValues.WidgetId);
            var btnDiv = widget.find("#" + CustomValues.FeedContainerId);
            var widgetId = CustomValues.WidgetId;

            PxPage.Loading(widgetId); //Load the spinner
            var bodyPostContent = $.ajax({
                type: "POST",
                url: PxPage.Routes.assignRSSArticle,
                data: { RSSFeedUrl: btnDiv.attr('RssUrl'), ArticleLink: btnDiv.attr('LinkUrl'), ArticleTitle: btnDiv.attr('LinkTitle'), ArticleDescription: btnDiv.attr('LinkDescription'), ArticlePubDate: btnDiv.attr('PubDate'), RssArticleId: btnDiv.find("#RSSArticleID").val(), RssAssignDate: dateArgs.DueDate },
                success: function (data) {
                    btnDiv.find(".btnUnarchiveRSSFeed").show();
                    btnDiv.find(".btnArchiveRSSFeed").hide();
                    btnDiv.find(".lnkUnarchiveRSSFeed").show();
                    btnDiv.find(".lnkArchiveRSSFeed").hide();
                    btnDiv.find(".btnUnassignRSSFeed").show();
                    btnDiv.find(".btnAssignRSSFeed").hide();
                    btnDiv.find(".lnkUnassignRSSFeed").show();
                    btnDiv.find(".lnkAssignRSSFeed").hide();

                    btnDiv.find("#RSSArticleDueDate").val(dateArgs.DueDate);

                    if (btnDiv.find("#RSSArticleID").val() != data.RSSArticleId) {

                        btnDiv.find("#RSSArticleID").val(data.RSSArticleId);
                        widget.find(".customRSSViewAllBarArticleCount").text(parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) + 1);
                        if (parseInt(widget.find(".customRSSViewAllBarArticleCount").text()) > 0) {
                            widget.find(".customRSSViewAllBarLink").text("View all");
                        }
                    }
                    PxPage.Loaded(widgetId);
                }
            });
            $('.CompactButtonSetMenu').hide();            
        }
    }
} (jQuery);

// RSS Link(Widget): end
