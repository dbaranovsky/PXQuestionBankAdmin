var PxLinkLibrary = function ($) {

    var isProcessing = false;
    return {

        ToggleSection: function () {
            $('.toc .level .expandContainer a.expand.active').parent().parent().children('span.children').toggle();
            PxLinkLibrary.SetSectionAddLink();
        },

        SetSectionAddLink: function () {
            $(".expandContainer").bind("mouseenter", function () {
                $('#lnk_' + this.id).show();
            });

            $(".expandContainer").bind("mouseleave", function () {
                $('#lnk_' + this.id).hide();
            });
            $('#divToc').find('div.level:odd').addClass('odd');
            $('#search-results-panel').hide();
            $('#divToc').show();
        },

        SearchLinks: function (e) {
            if (isProcessing == false) {
                if ($.trim($("#txtSearchLink").val()) != '') {
                    var _ContentTypes = "";
                    $('#search-results').block({ message: 'Loading...' });
                    isProcessing = true;

                    var data = {
                        IncludeWords: $.trim($("#txtSearchLink").val()),
                        ContentTypes: _ContentTypes,
                        Rows: 10,
                        Start: 0
                    };

                    $.post(PxPage.Routes.search_links, data, function (response) {
                        isProcessing = false;
                        $('#search-results-panel #search-results').html(response);
                        $('#search-results').unblock();
                        $('#divToc').hide();
                        $('#search-results-panel').show();
                        var searchTotal = $('#search-results #Results_numFound').last().val();
                        $('#search-total').html(searchTotal + ' Results Returned.');
                    });
                }
            }
        },

        AdvancedSearchScrolled: function () {

            var offSet = ($('#fne-content').height() - $('#search-results').height()) - 470;
            var currentTotal = $('#search-results li.Result').length;
            var searchTotal = $('#search-results #Results_numFound').last().val();

            if ($('#search-results').scrollTop() >= offSet) {
                if (isProcessing == false) {
                    if (currentTotal < searchTotal) {
                        PxLinkLibrary.OnAdvancedNextClicked();
                    }
                }
            }
        },

        OnAdvancedNextClicked: function () {
            isProcessing = true;

            $("div#search-loading").show();
            var data = PxLinkLibrary.GetAdvancedSearchData();

            data.Start = $('#search-results li.Result').length;

            $.post(PxPage.Routes.search_links, data, function (response) {

                $('#search-results ul.ResultList').last().append(response);
                $("div#search-loading").hide();
                isProcessing = false;
            });

        },

        GetAdvancedSearchData: function () {

            var data = {
                IncludeWords: $.trim($("#txtSearchLink").val()),
                Rows: 10,
                Start: 0
            };

            return data;
        },


        AddLink: function (e) {
            PxComments.UseLinkOrNote(e, "link", this);
        },

        CloseForm: function (event) {
            var newNote = false;
            var topForNewNote;
            if ($("#highlight-block-0").hasClass("active")) {
                newNote = true;
                topForNewNote = $("#newNotePosition").val().trim();
                $("#newNotePosition").remove();
                $("#highlight-new-container").children('.active').find('.linkLibraryWrapper').show();
            }
            PxLinkLibrary.CloseLinks(event);
            PxComments.SetHighlightPositions(event);

            if (!newNote) {
                $("#highlight-new-container").hide();
                $("#highlightList").children('.active').find('.linkLibraryWrapper').show();
            }
            else {
                $("#highlight-new-container").attr('style', 'top:' + topForNewNote);
                $("#highlight-block-0").attr('style', 'top:0px');
            }
            //to find the top property of the active note.            
            var top = newNote ? topForNewNote : $("#highlight-container").find('.active').css("top");
            //$('#document-viewer').scrollTop(parseInt(top, 10));
            $('#fne-content').animate({ scrollTop: top }, 'fast');

            event.stopPropagation();
        },

        CloseLinks: function (event) {
            $("#linklibrary").remove();
            $('.document-viewer-frame-host').show();
            $("#highlightList").children().show();
          //  $('#content-nav').show();
            $('.breadcrumb').show();
            $('.content-title').show();
            $('.highlight-bottom-menu').show();
            $('.commentLibraryWrapper').hide();
        },

        OnSearchKeyPress: function (event) {
            if (event.which == 13) {
                PxLinkLibrary.SearchLinks(event);
            }
        },

        BindControls: function () {

            $('#fne-unblock-action').unbind('click', PxLinkLibrary.CloseLinks).bind('click', PxLinkLibrary.CloseLinks);

            // $('.txtSearchLink').die().live('keypress', PxLinkLibrary.OnSearchKeyPress);
            $('.txtSearchLink').keypress(function (event) {
                PxLinkLibrary.OnSearchKeyPress(event);
                event.preventDefault;
            });

            $('.searchLinkBox #txtSearchLink').live('focus', function () {
                if ($('.searchLinkBox #txtSearchLink').val() == "Search our Library") {
                    $('.searchLinkBox #txtSearchLink').val("");
                }
            });

            $('.searchLinkBox #txtSearchLink').live('focusout', function () {
                if ($('.searchLinkBox #txtSearchLink').val() == "") {
                    $('.searchLinkBox #txtSearchLink').val("Search our Library");
                }
            });

            $('#linklibrary')
                .ajaxStart(function () { PxPage.Loading("fne-content"); })
                .ajaxStop(function () { PxPage.Loaded("fne-content"); });

            $("#btnClose").die().live('click', function (e) {
                e.preventDefault;
                PxLinkLibrary.CloseForm(e);
            });

            $(".addLinkToNote").die().live('click', PxLinkLibrary.AddLink);

            $("#btnSearchLink").die().live('click', PxLinkLibrary.SearchLinks);

            $('#linklibrary #search-results').rebind('scroll', PxLinkLibrary.AdvancedSearchScrolled);



            //            $('.txtSearchLink').die().live({
            //                'keypress': PxLinkLibrary.OnSearchKeyPress,
            //                'click': function() { return false; }
            //            });
        },

        Init: function () {
            this.BindControls();
        }
    };
} (jQuery);