//GradingScreen plugin for displaying and updating grading information
var PxGradingScreen = function ($) {
    var _static = {
        sel: {
            combobox: "#Students",
            comboboxInput: "#Students ~ .ui-combobox input",
            comboxButton: "#UserEnteredInstructorName ~ .ui-combobox .ui-button",
            saveButton: ".save-button",
            submitButton: ".submit-button",
            unSubmitButton: ".unsubmit-button",
            studentGradingList: ".studentGradingList"
        },
        props: {
            lookup: null
        },
        fn: {
            init: function () {
                _static.fn.bindControls();
            },

            initdropdown: function () {
                $(_static.sel.studentGradingList).msDropDown({ style: 'background-color:#333, font-size:24px' });

                $(_static.sel.studentGradingList).die().live('change', function () {
                    alert('changed test message!');
                });
            },

            bindControls: function () {

                //                $(_static.sel.comboboxInput)
                //                .autocomplete({
                //                    change: function (event, ui) {
                //                        if (!ui.item) {
                //                            var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"), valid = false;

                //                            $(_static.sel.combobox).children("option").each(function () {
                //                                if ($(this).text().match(matcher)) {
                //                                    this.selected = valid = true;
                //                                    return false;
                //                                }
                //                            });

                //                            if (!valid) {
                //                                // remove invalid value, as it didn't match anything
                //                                $(this).val("");
                //                                $(_static.sel.combobox).val("");
                //                                return false;
                //                            }
                //                        }
                //                    }
                //                })
                //                .data("autocomplete")
                //                    ._renderItem = function (ul, item) {
                //                        var listItem = $("<li></li>")
                //                            .data("item.autocomplete", item)
                //                            .append("<a><span>&nbsp;</span>" + item.label + "</a>")
                //                            .appendTo(ul);

                //                        $.each(_static.props.lookup, function (index, itemlst) {
                //                            var arrIn = itemlst.split("|");
                //                            if (arrIn.length > 0) {
                //                                var enrollmentid = arrIn[0],
                //                                status = arrIn[1];

                //                                if (enrollmentid == item.option.value) {
                //                                    if (status == "NeedsGrading" || status == "Submitted") {
                //                                        if (!listItem.find('span').hasClass('submitted-not-graded')) listItem.find('span').addClass('submitted-not-graded');
                //                                    }
                //                                    else if (status == "Graded") {
                //                                        if (!listItem.find('span').hasClass('graded')) listItem.find('span').addClass('graded');
                //                                    }
                //                                    else if (status == "None") {
                //                                        if (!listItem.find('span').hasClass('saved')) listItem.find('span').addClass('saved');
                //                                    }
                //                                }
                //                            }
                //                        });

                //                        return listItem;
                //                    };

                //                $(_static.sel.submitButton).die().live("click", _static.fn.onSubmit);
                //                $(_static.sel.unSubmitButton).die().live("click", _static.fn.onUnsubmit);
                //                $(_static.sel.saveButton).die().live("click", _static.fn.onSave);
            },
            onSelectionChanged: function (e) {
                alert('selection changed');
            },

            onSubmit: function () {
                alert('submit clicked');
                //                var bodyPostContent = $.ajax({
                //                    type: "POST",
                //                    url: PxPage.Routes,
                //                    data: {},
                //                    success: function () {
                //                        alert('successfully saved infromation');
                //                    },
                //                    error: function (req, status, error) {
                //                        alert("Error Saving Grades");
                //                    }
                //                });
            },
            onUnsubmit: function () {
                alert('unsubmit clicked');
            },
            onSave: function () {
                alert('save clicked');
            }
        }
    }

    return {
        init: function () {
            _static.fn.init();
        },
        reload: function () {
        },
        setLookup: function (input) {
            _static.props.lookup = input;
        },
        initdropdown: function () {
            _static.fn.initdropdown();
        }
    };
} (jQuery);