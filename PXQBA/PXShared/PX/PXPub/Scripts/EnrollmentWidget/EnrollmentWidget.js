PxEnrollmentWidget = function ($) {
    var _static = {
        initialized: false,
        sel: {
            schoolName: "#school-name",
            domain: "#domain",
            terms: "#terms",
            instructors: "#instructors",
            findContinueButton: ".find-continue-button",
            courses: "#courses",
            optionCourses: 'select#courses',
            findSchoolPopup: '#FindSchoolPopup'
        },
        fn: {
            init: function (schools) {
                $(document).off('change', _static.sel.terms).on('change', _static.sel.terms,
                function () {
                    var termid = this.value;
                    // $('select#instructors').html('');
                    $(_static.sel.courses).select2("disable");

                    $(_static.sel.optionCourses).html('');
                    $(_static.sel.courses).select2('val', '');

                    var domainid = $(_static.sel.schoolName).attr("item-id");
                    var termid = $("select#terms option:selected").attr("value");

                    $.getJSON(PxPage.Routes.GetEnrollmentCourses + "?id=" + domainid + "&term=" + termid, function (results) {
                        var html = ""; //<option value=''>--Select your Course--</option>";
                        for (k = 0; k < results.length; k++) {

                            if (results.length == 1) {
                                html += ("<option value='" + results[k].Id + "' selected>" + results[k].CourseNumber ? results[k].CourseNumber + " - " : "" + results[k].CourseProductName + results[k].SectionNumber ? " - " + results[k].SectionNumber : "" + " (" + results[k].InstructorName + ")</option>");
                            }
                            else {
                                html += ("<option value='" + results[k].Id + "'>" + (results[k].CourseNumber ? results[k].CourseNumber + " - " : "") + results[k].CourseProductName + (results[k].SectionNumber ? " - " + results[k].SectionNumber : "") + " (" + results[k].InstructorName + ")</option>");
                            }
                        }
                        $(_static.sel.optionCourses).html(html);

                        $(_static.sel.optionCourses).attr("size", "4");
                        $(_static.sel.courses).select2("enable");
                    });
                });

                $(_static.sel.findContinueButton).bind('click', function (event) {
                    var courseSelected = $('.course-item #courses').val();
                    if (courseSelected == null || courseSelected == "") {
                        return false;
                    }
                });

                $(_static.sel.schoolName).unbind("autocompleteopen");

                $(_static.sel.schoolName).bind("autocompleteopen", function (event, ui) {
                    var element = $('.ui-autocomplete');
                    var findschoolelement = $('<li class="ui-menu-item find-school" role="menuitem"><a class="ui-corner-all" href="#" id="ui-active-menuitem">Find your school</a></li>');

                    element.append('<li class="ui-menu-item" role="menuitem"><hr /></li>');
                    element.append(findschoolelement);

                    $(document).off('click', '.find-school').on('click', '.find-school',function (event) {
                        event.preventDefault();
                        _static.fn.FindSchoolAction();

                        $(_static.sel.schoolName).autocomplete("close");
                    });
                });

                $(_static.sel.schoolName).unbind("autocompleteselect").bind("autocompleteselect", function (event, ui) {
                    event.preventDefault();

                    $(this).val(ui.item.label);
                    $(this).attr("style", "border:default");
                    //              _static.fn.InitializeSelectedSchoolSearch(ui.item.label, ui.item.value);

                    $(this).attr("item-id", ui.item.value);

                    var domainId = ui.item.value;
                    $('select#terms').html('');
                    $('select#instructors').html('');
                    $('select#courses').html('');

                    var html = "<option value=''>--Select your Term--</option>";
                    $.getJSON(PxPage.Routes.GetEnrollmentTerms + "?id=" + domainId, function (results) {
                        for (k = 0; k < results.length; k++) {
                            html += ("<option value='" + results[k].Id + "'>" + results[k].Name + "</option>");
                        }

                        $('select#terms').html(html).removeAttr("disabled");
                    });
                });

                $(_static.sel.schoolName).unbind("autocompletefocus");
                $(_static.sel.schoolName).bind("autocompletefocus", function (event, ui) {
                    event.preventDefault();
                    $(this).val(ui.item.label);
                    $(this).attr("style", "border:default");
                    //                _static.fn.InitializeSelectedSchoolSearch(ui.item.label, ui.item.value);
                });

                $(_static.sel.schoolName).autocomplete({
                    source: schools,
                    minLength: 1
                });

                $(_static.sel.courses).select2({
                    allowClear: true,
                    placeholder: '--Select your Course--'
                });

               $(_static.sel.courses).select2("disable");;
            },
            FindSchoolAction: function () {
                //                TODO: Need to revisit displaying find course link
                //                $(_static.sel.findSchoolPopup).show();
                //                $(_static.sel.selectedSchool).hide();
            }
        }
    };

    return {
        Init: function (schools) {
            _static.fn.init(schools);
        }
    };
} (jQuery);