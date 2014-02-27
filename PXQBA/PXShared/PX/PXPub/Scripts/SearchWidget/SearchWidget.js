//Script file for controlling actions associated with the BeingMeta Search Widget
var PxSearchWidget = function ($) {
    var _static = {
        sel: {
            TocTab: '.searchWidget .toctab',
            TocLnks: '.searchWidget .toctab, .noSearchContent .toclnk',
            IndexTab: '.searchWidget .indextab',
            IndexLnks: '.searchWidget .indextab, .noSearchContent .indexlnk',
            SearchResultsTab: '.searchWidget .resultstab',
            //TocContent: '#PX_TOCWidget', //this will be the TOC Widget
            IndexContent: '.searchcontent .indexcontent',
            SearchResultsContent: '.searchcontent .resultscontent',
            searchTabs: '.searchtabwrapper span',
            selectedTab: 'selectedtab',
            disabledTab: 'disabledtab',
            searchTextbox: '.searchWidget .searchBox',
            beingMetaTemplate: '#being_meta_template',
            searchButton: '.searchWidget .searchBtn',
            searchResultHeader: '.resultscontent .searchResultHeader',
            resultItem: '.resultscontent .resultitem',
            searchText: '.resultscontent .searchText',
            correctedSearch: '.resultscontent .correctedSearch',
            correctedSearchText: '.resultscontent .correctedSearchText',
            searchResultsWrapper: '.resultscontent .searchResultsWrapper',
            searchResult: '.searchresult',
            searchResultContent: '.searchResultContent',
            noSearchContent: '.resultscontent .noSearchContent',
            indexlnk: '.noSearchContent .indexlnk',
            toclnk: '.noSearchContent .toclnk',
            itemCount: '.itemcount',
            searchResultsWrapperID: '.resultscontent #searchResultsWrapper',
        },

        fn: {
            init: function (options) {
                _static.sel = $.extend(_static.sel, options.sel || {});

                // result views
                $(_static.sel.TocLnks).click(_static.fn.tocTabClicked);
                $(_static.sel.IndexLnks).click(_static.fn.indexTabClicked);
                $(_static.sel.SearchResultsTab).click(_static.fn.resultsTabClicked);
                
                $(PxPage.switchboard).bind('SearchBeingMeta', _static.fn.searchBeingMeta);

                // search functionality
                $(_static.sel.searchButton).click(function() {
                    $(PxPage.switchboard).trigger('SearchBeingMeta');
                });
                
                //perform the search when they 'Enter' key is hit
                $(_static.sel.searchTextbox).keydown(function (event) {
                    //event.stopImmediatePropagation();
                    //search BeingMeta if the enter key is hit
                    if (event.keyCode == 13) {
                        _static.fn.searchBeingMeta();
                    }
                });
                   
                 //Implementing Autocomplete functionality using Predictive Search from BeingMeta
                 $(_static.sel.searchTextbox).autocomplete({
                    source: function (request, response) {
                        //make an ajax call to the Predictive Search Endpoint of BeingMeta
                        $.ajax({
                            type: 'GET',
                            url: PxPage.Routes.beingmeta_predict,
                            data: { 
                                    PREFIX: request.term, //request object contains a single property called 'term', which is the value keyed into the search textbox
                                    WINDOW: '6' // max number of predicts to be returned by BeingMeta
                                  },
                            success: function (data) {
                                response(data); //send the returned json data to the response callback of autocomplete
                            },
                            error: function (req, status, error) {
                                PxPage.log("Error while making a request to Predictive Search");
                            }
                        });
                    },
                    minLength: 1, //minimum characters that need to be keyed in for the search to happen. This restricts too many results from being returned.
                    select: function (event, ui) {
                         var selectedText = ui.item ? ui.item.value : '';
                         $(PxPage.switchboard).trigger('SearchBeingMeta', [selectedText]);
                    }
                });

                 $(document).off('click', _static.sel.searchResultHeader).on('click', _static.sel.searchResultHeader, _static.fn.searchResultHeaderClicked);
                 $(document).off('click', _static.sel.resultItem).on('click', _static.sel.resultItem, _static.fn.resultItemClicked);

                //register the handlebars helper functions
                _static.fn.registerHandleBarsHelpers();
            },

            tocTabClicked: function () {
                //make TOC tab selected
                $(_static.sel.searchTabs).removeClass(_static.sel.selectedTab);
                $(_static.sel.TocTab).addClass(_static.sel.selectedTab);

                //show the TOC content
                $(_static.sel.TocContent).show();
                $(_static.sel.IndexContent).hide();
                $(_static.sel.SearchResultsContent).hide();
            },

            indexTabClicked: function () {
                //make index tab selected
                $(_static.sel.searchTabs).removeClass(_static.sel.selectedTab);
                $(_static.sel.IndexTab).addClass(_static.sel.selectedTab);

                //show the index content
                $(_static.sel.TocContent).hide();
                $(_static.sel.IndexContent).show();
                $(_static.sel.SearchResultsContent).hide();
            },

            resultsTabClicked: function () {
                // if no search results exist, the tab is disabled, do nothing
                if ($(_static.sel.SearchResultsTab).hasClass(_static.sel.disabledTab))
                    return;

                //make Search Results tab selected
                $(_static.sel.searchTabs).removeClass(_static.sel.selectedTab);
                $(_static.sel.SearchResultsTab).addClass(_static.sel.selectedTab);

                //show the Search Results content
                $(_static.sel.TocContent).hide();
                $(_static.sel.IndexContent).hide();
                $(_static.sel.SearchResultsContent).show();
            },

            //search BeingMeta and get the results using the Search Endpoint
            searchBeingMeta: function (event, searchText) {
                if(searchText == null || searchText == undefined)
                    searchText = $.trim($(_static.sel.searchTextbox).val());

                // requires more than one characters to search
                if (searchText.length <= 1)
                    return;

                PxPage.Loading(_static.sel.searchResultsWrapperID); //Load the spinner
                //ajax call to get the search results from BeingMeta
                var bodyPostContent = $.ajax({
                    type: 'GET',
                    url: PxPage.Routes.beingmeta_search,
                    data: { QTEXT: searchText },
                    success: function (results) {
                        $(_static.sel.correctedSearch).hide();
                        //Atleast one cluster should be present for the search results tab to be active
                        if (results.clusters != null && results.clusters != undefined && results.clusters != '' && results.clusters.length > 0) {
                            //parse the Handlebars template
                            var source = $(_static.sel.beingMetaTemplate).html();
                            var template = Handlebars.compile(source);
                            var context = results;
                            var renderedHtml = template(context);
                            $(_static.sel.searchResultsWrapper).show();
                            $(_static.sel.searchResultsWrapper).html(renderedHtml);
                            $(_static.sel.noSearchContent).hide();

                            //display the search term
                            if (results.query != null && results.query != undefined && results.query != '' && results.query.length > 0) {
                                var searchText = results.query[0].text;
                                var correctSearchText = results.query[0].term;
                                $(_static.sel.searchText).text(searchText);
                                //display the correct search text if BeingMeta searched for a corrected text
                                if (searchText != correctSearchText) {
                                    $(_static.sel.correctedSearchText).text(correctSearchText);
                                    $(_static.sel.correctedSearch).show();
                                }
                            }

                            //collapse all the clusters with more than 3 items
                            _static.fn.collapseClusters();

                        }
                        else {
                            //display the no search results contents
                            var searchText = $(_static.sel.searchTextbox).val();
                            $(_static.sel.searchText).text(searchText);
                            $(_static.sel.searchResultsWrapper).html('');
                            $(_static.sel.searchResultsWrapper).hide();
                            $(_static.sel.noSearchContent).show();
                        }

                        //show the search results tab
                        $(_static.sel.SearchResultsTab).removeClass(_static.sel.disabledTab);
                        _static.fn.resultsTabClicked();

                    },
                    complete: function () {
                        PxPage.Loaded(_static.sel.searchResultsWrapperID); //close the spinner
                    },
                    error: function (req, status, error) {
                        PxPage.log("Error getting the response from BeingMeta. Search Text: " + searchText);
                    }
                });
            },

            // Toggle the clusters when clicked on the cluster header (result header)
            searchResultHeaderClicked: function (event) {
                var searchResultHeader = $(this);
                var itemcount = searchResultHeader.find(_static.sel.itemCount);
                var searchResultContent = searchResultHeader.closest(_static.sel.searchResult).find(_static.sel.searchResultContent);
                if (itemcount.is(":visible")) {
                    itemcount.hide();
                    searchResultContent.show();
                }
                else {
                    itemcount.show();
                    searchResultContent.hide();
                }
            },

            //collapse all the clusters with more than 3 items
            collapseClusters: function () {
                resultHeaders = $(_static.sel.searchResultHeader);
                $.each(resultHeaders, function (index, resultHeader) {
                    resultHeader = $(resultHeader);
                    if (resultHeader.attr('itemcount') >= 4) {
                        var itemcount = resultHeader.find(_static.sel.itemCount);
                        var searchResultContent = resultHeader.closest(_static.sel.searchResult).find(_static.sel.searchResultContent);
                        itemcount.show();
                        searchResultContent.hide();
                    }
                });
            },

            //when a result item is clicked in the results
            resultItemClicked: function () {
                var resultItem = $(this);
                var itemIds = resultItem.attr('refids').split(',');
                var indexid = resultItem.attr('indexid');
                $(PxPage.switchboard).trigger("BeingMetaResultsClicked", [itemIds, indexid]);
            },

            //register the handlebars helper functions
            registerHandleBarsHelpers: function () {
                //getRefs function takes an array of itemids and returns back a comma seperated string.
                Handlebars.registerHelper('getRefs', function (refs) {
                    if ($.isArray(refs)) {
                        return refs.join(',');
                    }
                    else {
                        return refs;
                    }
                });
            }

        }
    };

    return {

        Init: function (options) {
            _static.fn.init(options);
        }
    };
} (jQuery);