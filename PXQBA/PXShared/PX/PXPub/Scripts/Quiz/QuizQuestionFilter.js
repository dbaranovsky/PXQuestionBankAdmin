//jQuery plugin for Quiz Preview functionality

(function ($) {
    var _static = {
        pluginName: "questionfilter",
        dataKey: "questionfilter",
        bindSuffix: "questionfilter",
        dataAttrPrefix: "data-qf-",
        //static variables:
        isInitialized: false,
        //plugin defaults
        defaults: {
            MouseDown: false,
            //button text
            hideFilters: "Hide Filters",
            showFilters: "Show Filters",
            editFilters: "Edit Filters"


        },
        //css settings
        css: {

        },
        //selectors for commonly accessed elements
        sel: {

            //CONTAINERS for different question list modes:
            availableQuestions: ".available-questions", //list of question available to be added (quiz editor)
            selectedQuestions: ".selected-questions", //list of questions in current quiz (quiz editor)
            quizOverview: ".quiz-overview", //list of questions in quiz (quiz preview)

            //button
            btnShowFilterAvailableQuestions: ".available-questions .show-filter-available-question",
            filterCountAvailableQuestion: ".filter-count-available-question",

            allAvailableQuestion: ".available-questions .questions .question",
            poolQuestionType: ".pool-question-type",
            questionFilterDiv: ".question-filter",
            spanQuestionFilterMetadata: "span.question-filter-metadata",
            questionMetadata: ".question-text .question-metadata",
            questionFilterQuestiontype: "question-filter-questiontype",
            allQuestions: ".questions .question",

            //class name
            filteredOut: "filteredout",
            collapsed: "collapsed",
            classQuestionFilterMetadata: "question-filter-metadata",
            classLabel:"question-filder-label"

        },
        //hooks for the FNE window
        fneHooks: {
            FneInit: function () {
                PxQuiz.UpdateAddedQuestions();
            }
        },
        //private functions
        fn: {
            // Initiating the filter fields.
            SetFilterMetadata: function (event, source, reload) {
                var isAnyFilterApplied = false;
                var quizEditor = "";
                if (source != undefined) {
                    quizEditor = source;
                }

                // Is any filter is already applied, then dont reload.
                var questionFilterMetadata = $(quizEditor + _static.sel.spanQuestionFilterMetadata);
                questionFilterMetadata.each(function (index, filterelement) {
                    var filterMedataDataId = $(filterelement).attr("id");
                    var jsonObjectMetaDataField = filterMedataDataId.substring(filterMedataDataId.lastIndexOf("-") + 1, filterMedataDataId.length);
                    if ($("#" + filterMedataDataId).select2("val") != "") {
                        isAnyFilterApplied = true;
                        return false;
                    }
                });

                if (!isAnyFilterApplied || reload) // relaod (true): only in case of search, if filter is already applied.
                {
                    var questions = $(quizEditor + _static.sel.allQuestions);

                    // Question Type Filter : DEFAULT
                    var questionFilterQuestionType = [];
                    questions.each(function (index, element) {
                        questionFilterQuestionType.push($(element).find(_static.sel.poolQuestionType).text());
                    });
                    var uniqueQuestionFilterQuestionTypes = questionFilterQuestionType.filter(function (itm, i) {
                        return i == questionFilterQuestionType.indexOf(itm);
                    });
                    uniqueQuestionFilterQuestionTypes = uniqueQuestionFilterQuestionTypes.sort();

                    $(quizEditor + _static.sel.questionFilterDiv).empty();
                    $(quizEditor + _static.sel.questionFilterDiv).append('' +
                        '<div class="question-filter-row"><span class="' + _static.sel.classLabel + '" >' + "Question Type" + '</span>' +
                               '<span id="' + _static.sel.questionFilterQuestiontype + '" class ="' + _static.sel.classQuestionFilterMetadata + '"> ' +
                               '</span></div>');
                    $(quizEditor + "#" + _static.sel.questionFilterQuestiontype).select2(
                    {
                        placeholder: "Select a question type",
                        multiple: true,
                        tags: uniqueQuestionFilterQuestionTypes
                    });

                    // Metadata Filter: Only those metadata will appear in Filter list if that metadata is added in course script (meta-available-question-data) 
                    // And atleast one of the question must have some value for that metadata.
                    var questionFilterMetadataKeys = $.map(PxPage.QuestionFilter.FilterMetadata, function (val) { return val.Name; });

                    // Retrieving all possible metadata values
                    var number;
                    for (number = 0; number < questionFilterMetadataKeys.length; ++number) {
                        var questionFilterMetadataValues = [];
                        questions.each(function (index, element) {
                            var metadata = $.trim(($(element).find(_static.sel.questionMetadata).html()));
                            if (metadata != null && metadata != "null" && metadata.length > 0) {
                                var jsonMetadata = {};
                                try {
                                    jsonMetadata = jQuery.parseJSON(metadata);
                                }
                                catch(ex){}
                                $.each(jsonMetadata, function (key, value) {
                                    if (key == questionFilterMetadataKeys[number]) {
                                        var valueArray = value.split('|');
                                        for (var i = 0; i < valueArray.length; i++) {
                                            if ($.trim(valueArray[i]) != null && $.trim(valueArray[i]) != "") {
                                                questionFilterMetadataValues.push($.trim(valueArray[i]));
                                            }
                                        }
                                    }
                                });
                            }
                        });

                        // Getting all unique values
                        var uniqueQuestionFilterMetadataValues = questionFilterMetadataValues.filter(function (itm, i) {
                            return i == questionFilterMetadataValues.indexOf(itm);
                        });
                        uniqueQuestionFilterMetadataValues = uniqueQuestionFilterMetadataValues.sort();

                        // Creating Select2 with unique keys and values
                        var JSonData = [];
                        for (valNumber = 0; valNumber < uniqueQuestionFilterMetadataValues.length; ++valNumber) {
                            var filterItem = {};
                            filterItem["id"] = $.trim(uniqueQuestionFilterMetadataValues[valNumber]);
                            filterItem["text"] = $.trim(uniqueQuestionFilterMetadataValues[valNumber]);
                            if (uniqueQuestionFilterMetadataValues[valNumber].length > 50) {
                                filterItem["text"] = $.trim(uniqueQuestionFilterMetadataValues[valNumber]).substring(0, 49) + "...";
                            }
                            JSonData.push(filterItem);
                        }
                        if (JSonData.length > 0) {
                            
                            var friendlyName = '';

                            $.each(PxPage.QuestionFilter.FilterMetadata,
                                function () {
                                    if (this.Name == questionFilterMetadataKeys[number]) {
                                        friendlyName = this.Friendlyname;
                                    }
                            }); 

                            if (friendlyName == null || friendlyName.length == 0) {
                                friendlyName = questionFilterMetadataKeys[number];
                            }

                            $(quizEditor + _static.sel.questionFilterDiv).append(
                           '<div class="question-filter-row"><span class="' + _static.sel.classLabel + '" >' + friendlyName + '</span>' +
                               '<span id="question-filter-' + questionFilterMetadataKeys[number] + '" class ="' + _static.sel.classQuestionFilterMetadata + '"> ' +
                               '</span></div>');
                            $(quizEditor + "#question-filter-" + questionFilterMetadataKeys[number]).select2(
                            {
                                placeholder: "Select a " + friendlyName.toLowerCase(),
                                multiple: true,
                                tags: JSonData
                            });
                        }
                        else {
                            $(quizEditor + "#question-filter-" + questionFilterMetadataKeys[number]).remove();
                        }
                    }
                }
            },

            // Whenever user will select/deselect any filter value.
            QuestionFilterSelect: function (event) {
                var questions = $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.allQuestions);
                var questionFilterMetadata = $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.spanQuestionFilterMetadata);
                var currentFilterData = {}; //stores the current filter applied to the question list
                var doFilter = false; //only run filter when there are filters to apply
                questionFilterMetadata.each(function(index, filterelement) {
                    var filterMedataDataId = $(filterelement).attr("id");
                    var jsonObjectMetaDataField = filterMedataDataId.substring(filterMedataDataId.lastIndexOf("-") + 1, filterMedataDataId.length);

                    if ($(event.target).closest(_static.sel.availableQuestions).find("#" + filterMedataDataId).select2("val") != "") {
                        var currentFilterSelcetedValue = $(event.target).closest(_static.sel.availableQuestions).find("#" + filterMedataDataId).select2("val");
                        currentFilterData[jsonObjectMetaDataField] = currentFilterSelcetedValue;
                        doFilter = true;
                    }
                });
                if (!doFilter) { //no filters are applied, unhide all questions
                    $("." + _static.sel.filteredOut).removeClass(_static.sel.filteredOut);
                    return;
                }
                questions.each(function(index, element) {
                    var currentQuestionType = $(element).find(_static.sel.poolQuestionType).text();
                    $(element).removeClass(_static.sel.filteredOut);
                    var metadata = $.trim(($(element).find(_static.sel.questionMetadata).html()));
                    var jsonMetadata = {};
                    try {
                        jsonMetadata = jQuery.parseJSON(metadata);
                    } catch(ex) {
                        jsonMetadata = [];
                    }
                    //if (metadata != null && metadata != "null" && metadata.length > 0) {

                    var questionMatchesAllFilters = true;
                    $.each(currentFilterData, function (name, filterVales) {
                        var questionMatchesFilter = false;
                        var questionValues = [];
                        if (name == "questiontype") {
                            questionValues.push(currentQuestionType);
                        } else {
                            if (jsonMetadata[name] != null) {
                                questionValues = jsonMetadata[name].split("|");
                            }
                        }
                        //iterate through all filter values, if any of them match one of the question values, 
                        //flag question as a match
                        $.each(filterVales, function (index, filterValue) {
                            // Question Type Filter
                            if ($.inArray(filterValue, questionValues) != -1) {
                                questionMatchesFilter = true;
                            }
                        });
                        questionMatchesAllFilters = questionMatchesAllFilters && questionMatchesFilter;
                    });
                    if (!questionMatchesAllFilters) {
                        $(element).addClass(_static.sel.filteredOut); // Hiding out current question.
                    } else {
                        $(element).removeClass(_static.sel.filteredOut);
                    }

                    //}
                });

                    
            },

            // To show/hide the filter area.
            ShowFilterAvailableQuestion: function (event) {
                if ($(event.target).closest(_static.sel.availableQuestions).find(_static.sel.questionFilterDiv).hasClass(_static.sel.collapsed)) {
                    $(event.target).text(_static.defaults.hideFilters);
                    $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.filterCountAvailableQuestion).text("");
                    $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.questionFilterDiv).removeClass(_static.sel.collapsed);
                }
                else {
                    var isAnyFilterApplied = false;
                    var questionFilterMetadata = $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.spanQuestionFilterMetadata);
                    var totalFilterApplied = 0;
                    questionFilterMetadata.each(function (index, filterelement) {
                        var filterMedataDataId = $(filterelement).attr("id");
                        var jsonObjectMetaDataField = filterMedataDataId.substring(filterMedataDataId.lastIndexOf("-") + 1, filterMedataDataId.length);
                        if ($("#" + filterMedataDataId).select2("val") != "") {
                            isAnyFilterApplied = true;
                            totalFilterApplied = totalFilterApplied + 1;
                        }
                    });
                    if (!isAnyFilterApplied) {
                        $(event.target).text(_static.defaults.showFilters);
                        $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.filterCountAvailableQuestion).text("");
                    }
                    else {
                        $(event.target).text(_static.defaults.editFilters);
                        var filterAppliedText = " filter applied";
                        if (totalFilterApplied > 1) {
                            var filterAppliedText = " filters applied";
                        }
                        $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.filterCountAvailableQuestion).text("(" + totalFilterApplied + filterAppliedText + ")");
                    }
                    $(event.target).closest(_static.sel.availableQuestions).find(_static.sel.questionFilterDiv).addClass(_static.sel.collapsed);
                }
            },

            // Update the Filter list, in case of search.
            UpdateQuestionFilter: function (event, source) {
                var quizEditor = "";
                if (source != undefined) {
                    quizEditor = source;
                }

                // Reloading the filter list with current question (based on search) metadata filter.
                $(PxPage.switchboard).trigger("showfiltermetadata", [quizEditor, true]);

            },

            BindFneHooks: function () {
                PxPage.FneInitHooks['quiz-editor'] = _static.fneHooks.FneInit;
            }
        }//end private functions


    };

    var api = {
        init: function (options) {
            _static.fn.BindFneHooks();
            //trigger when questions are slected
            $(document).off('click', '.question-filter-metadata').on('click', '.question-filter-metadata', _static.fn.QuestionFilterSelect);
            $(document).off('click', _static.sel.btnShowFilterAvailableQuestions).on('click', _static.sel.btnShowFilterAvailableQuestions, _static.fn.ShowFilterAvailableQuestion);
            $(PxPage.switchboard).rebind("showfiltermetadata", _static.fn.SetFilterMetadata);
            $(PxPage.switchboard).rebind("updatequestionfilter", _static.fn.UpdateQuestionFilter);
            _static.fn.SetFilterMetadata();
        }
    };
    // register the plugin with jQuery and provide access to
    // the api.

    $.fn.questionfilter = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

    /*https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Array/filter */
    if (!Array.prototype.filter) {
        Array.prototype.filter = function (fun /*, thisp */) {
            "use strict";
            if (this == null)
                throw new TypeError();

            var t = Object(this);
            var len = t.length >>> 0;
            if (typeof fun != "function")
                throw new TypeError();

            var res = [];
            var thisp = arguments[1];
            for (var i = 0; i < len; i++) {
                if (i in t) {
                    var val = t[i]; // in case fun mutates this        
                    if (fun.call(thisp, val, i, t))
                        res.push(val);
                }
            }
            return res;
        };
    }

    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (searchElement /*, fromIndex */) {
            "use strict";
            if (this == null) {
                throw new TypeError();
            }
            var t = Object(this);
            var len = t.length >>> 0;
            if (len === 0) {
                return -1;
            }
            var n = 0;
            if (arguments.length > 1) {
                n = Number(arguments[1]);
                if (n != n) { // shortcut for verifying if it's NaN
                    n = 0;
                } else if (n != 0 && n != Infinity && n != -Infinity) {
                    n = (n > 0 || -1) * Math.floor(Math.abs(n));
                }
            }
            if (n >= len) {
                return -1;
            }
            var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
            for (; k < len; k++) {
                if (k in t && t[k] === searchElement) {
                    return k;
                }
            }
            return -1;
        }
    }

} (jQuery));