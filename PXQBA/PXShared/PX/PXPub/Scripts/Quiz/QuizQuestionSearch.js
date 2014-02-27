//jQuery plugin for Quiz Question Search
(function ($) {
    var _static = {
        pluginName: "questionSearch",
        dataKey: "questionSearch",
        bindSuffix: ".questionSearch",
        dataAttrPrefix: "data-qa-",
        currentScrollTopForSearch: 0,
        tempScrollTopForSearch: 0,
        //selectors for commonly accessed elements
        sel: {


        },
        //private functions
        fn: {
            OnQuestionSearchKeyPress: function (e) {
                if (e.which == 13) {
                    _static.fn.ProcessSearchQuestions(e);
                    e.preventDefault();
                }
            },
            ProcessSearchQuestions: function (e) {
                _static.fn.SearchQuestions(e.target.id);
                e.preventDefault();
            },
            SearchQuestions: function (id) {
                switch (id) {
                    case "txtSearchQuiz":
                    case "btnSearchQuiz":
                        $('.searchQuestion .search-type').val("searchedByText");
                        $('.searchQuestion .search-category').html('');
                        if ($.trim($("#txtSearchQuiz").val()) == '')
                            return;
                        break;
                    case "currently-in-use":
                        $('.searchQuestion .search-type').val("searchedByInUse");
                        $('.searchQuestion .search-category').html("Currently in use");
                        break;
                    case "not-currently-in-use":
                        $('.searchQuestion .search-type').val("searchedByNotInUse");
                        $('.searchQuestion .search-category').html("Not currently in use");
                        break;
                }
                PxPage.Loading("children-list", true);
                PxQuiz.isProcessing = true;

                var data = _static.fn.GetAdvancedSearchData();
                $.post(PxPage.Routes.search_question, data, function (response) {
                    PxQuiz.isProcessing = false;
                    // Save any filters so they can be reapplied when this search returns from the server
                    var filters = $('.select2-search-choice div');

                    $('.available-questions .children-list').html(response);
                    $(".available-questions-header .quiz-bank-title").text('Search Results for "' + data.IncludeWords + '"'); //show results title
                    $(".available-questions-header #question-list-level").show(); //show breadcrumb link
                    PxPage.Loaded("children-list", true);
                    
                    $(PxPage.switchboard).trigger("availablequestionsupdated");
                    //_static.fn.ShowSearchTotal(id);
                    PxQuiz.UpdateAvailableQuestionsMenu('.available-questions .children-list .ResultList');
                    $(".question-list").questionListGearbox('updateAddMenu');
                    $(PxPage.switchboard).trigger("updatequestionfilter", [".available-questions "]);
                    $(".quiz-editor .available-questions .children-list #question-filter-questiontype").click();

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
                });
            },
            
           
            GetAdvancedSearchData: function () {
                var data;
                switch ($.trim($('.searchQuestion .search-type').val())) {
                    case "searchedByText":
                        data = {
                            IncludeWords: $.trim($("#txtSearchQuiz").val()),
                            Rows: 20,
                            Start: 0,
                            ClassType: 'question'

                        };
                        break;
                    case "searchedByInUse":
                        data = {
                            ExactQuery: "In_Use",
                            Rows: 20,
                            Start: 0,
                            ClassType: 'question'

                        };
                        break;
                    case "searchedByNotInUse":
                        data = {
                            ExactQuery: "Not_In_Use",
                            Rows: 20,
                            Start: 0,
                            ClassType: 'question'

                        };
                }
                return data;
            }
            //end of private functions
        }

    };

    var api = {
        init: function () {
            $(document).off('click', '#btnSearchQuiz, .browsemyquestions a.my-edited-question').on('click', '#btnSearchQuiz, .browsemyquestions a.my-edited-question', _static.fn.ProcessSearchQuestions);
            
            $(document).off('keypress', '.available-questions .searchQuestion #txtSearchQuiz').on('keypress', '.available-questions .searchQuestion #txtSearchQuiz', _static.fn.OnQuestionSearchKeyPress);
            $(document).off('click', '.available-questions .searchQuestion #txtSearchQuiz').on('click', '.available-questions .searchQuestion #txtSearchQuiz', function () { return false; });
            $(document).off('focusout', '.available-questions .searchQuestion #txtSearchQuiz').on('focusout', '.available-questions .searchQuestion #txtSearchQuiz', function () { return false; });

        },
        searchQuestions: function(id) {
            _static.fn.SearchQuestions(id);
        }//end of public functions
    };
    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.questionSearch = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

}(jQuery));
