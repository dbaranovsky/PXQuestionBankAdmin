var CreateCourse = function ($) {
    // static data
    var _static = {
        defaults: {

        },
        sel: {
        },
        fn: {
            getCourseLocation: function () {
                var path = "";
                if ($('#courseLocation').length > 0)
                    path = $('#courseLocation').val();

                return path;
            },
            ignoreKey: function(key) {
                var ignoreKeys = [8, 16, 17, 18, 20, 144, 45, 46, 33, 34, 35, 36, 37, 38, 39, 40];
                if ($.inArray(key, ignoreKeys) >= 0)
                    return true;
                return false;
            }
        }
    };
    // static functions
    return {
        Init: function (title, selectedDomain, commonScriptPath) {

            CreateCourse.InitBindings(commonScriptPath);
            
            $('#fne-window').addClass('activate-course-fne');
            PxPage.SetFneTitle(title);

            $('#FindSchoolPopup').hide();

            
            $("#schoolsearchbyzip").prop('checked', true);
            $("#college").prop('checked', true);
            $('#Country').trigger('change');
            $("#schoolsearchbyzip").trigger('click');


            if (selectedDomain && selectedDomain.length > 0) {
                $('select#PossibleDomains').val(selectedDomain);
                $('#SelectedDerivativeDomain').val($('select#PossibleDomains option:selected').text());
            }
        },
        InitBindings: function (commonScriptPath) {
            var initializeSearch = function () {
                $('#FindSchools').val('Find Schools');
                $('select#school_search_result').html('');
                $('select#school_search_result').hide();

                var icon = $('#findSchoolIcon');
                if (icon.hasClass('find-icon') == false)
                    icon.addClass('find-icon');
            };
            
            $('#fne-unblock-action').click(function () {
                var path = _static.fn.getCourseLocation();
                if (path.length > 0) {
                    window.location = path;
                }
            });
            
            $('#FindSchool').click(function () {
                $('#FindSchoolPopup').show();
                $('#SelectedSchool').hide();
            });

            $('#Country').change(function () {
                $('select#State').html('');
                var country = $("select#Country option:selected").attr("value");
                $.getJSON(commonScriptPath + country + ".js", function (results) {
                    var items = [];

                    $.each(results, function(key, val) {
                        items.push('<option value="' + key + '">' + val + '</option>');
                    });

                    $('select#State').html(items.join(''));
                });
            });

            $('#schoolsearchbycity').click(function () {
                $('#schoolbox').show();
                $('#zipbox').hide();
                initializeSearch();
            });

            $('#schoolsearchbyzip').click(function () {
                $('#schoolbox').hide();
                $('#zipbox').show();
                initializeSearch();
            });

            $('#college, #highschool').click(function () {
                initializeSearch();
            });

            $('select#PossibleDomains').change(function () {
                $('#SelectedDerivativeDomain').val($('select#PossibleDomains option:selected').text());
            });

            $('select#PossibleDomains').click(function () {
                $('#SelectedDerivativeDomain').val($('select#PossibleDomains option:selected').text());
            });

            $('#CloseFindPopupSchool').click(function (event) {
                initializeSearch();

                $('#FindSchoolPopup').hide();
                $('#SelectedSchool').show();
            });


            $('#FindSchools').click(function () {
                var button = $(this);
                if (button.val() == "Find Schools") {
                    var resultList = $('select#school_search_result');
                    PxPage.Loading('schoolresult');
                    resultList.html('');

                    var searchBy = 'Zip';
                    var city = '';
                    var regionCode = '';
                    var country = '';
                    var instituteType = '1';
                    var zipCode = '';

                    if ($('#schoolsearchbycity:checked').val() == 'City') { searchBy = 'City'; }
                    if ($('#highschool:checked').val() == '2') { instituteType = '2'; }
                    if (searchBy == 'City') {
                        city = $('#searchcity').val();
                        regionCode = $('select#State').val();
                        country = $('select#Country').val();
                    } else {
                        zipCode = $('#ZipCode').val();
                    }

                    var url = PxPage.Routes.search_onyx_school + "?searchType=" + searchBy + "&city=" + city + "&regionCode=" + regionCode
                                  + "&countryCode=" + country + "&instituteType=" + instituteType + "&zipCode=" + zipCode;
                    $.getJSON(url, function (results) {
                        var items = [];

                        $.each(results, function(key, val) {
                            items.push('<option value="' + key + '">' + val + '</option>');
                        });

                        resultList.html(items.join(''));
                        if (items.length > 0) {
                            $('#findSchoolIcon').removeClass('find-icon');
                            $('#FindSchools').val('Choose');
                            $('select#school_search_result option:first-child').attr("selected", "selected");
                            resultList.show();
                        }

                        PxPage.Loaded('schoolresult');
                    });
                }
                else {
                    var selectedItem = $('select#school_search_result').find(":selected");
                    var insertItemInto = $('select#PossibleDomains');
                    insertItemInto.append('<option value="' + selectedItem.val() + '" selected="selected" >' + selectedItem.text() + '</option>');
                    insertItemInto.trigger('change');
                    initializeSearch();
                    $('#CloseFindPopupSchool').trigger('click');
                }
            });

            $('.create-course-title').keyup(function (event) {
                if (_static.fn.ignoreKey(event.which))
                    return;
                
                var productName = $('#ProductName').val();
                var sectionNumber = $('#SectionNumber').val();
                var courseNumber = $('#SectionNumber').val();
                var courseSection = '';
                var initVal = $(this).val();
                var courseName = $(this).val().replace('&#', '');

                if ((sectionNumber.length > 0) && (courseNumber.length > 0)) {
                    courseSection = courseNumber + '-' + sectionNumber + " ";
                } else {
                    courseSection = courseNumber + sectionNumber + ' ';
                }

                if (initVal != courseName) {
                    $(this).val(courseName);
                }

                $('#CourseProductName').val(courseSection + courseName + ', ' + productName);
            });

            $('.create-course-number').keyup(function (event) {
                if (_static.fn.ignoreKey(event.which))
                    return;

                var productName = $('#ProductName').val();
                var courseName = $('#Title').val();
                var sectionNumber = $('#SectionNumber').val();
                var courseNumber = $('#CourseNumber').val();
                var courseSection = '';
                var initVal = $(this).val();

                var courseNumber = $(this).val().replace('&#', '');



                if (initVal != courseNumber) {
                    $(this).val(courseNumber);
                }

                if ((sectionNumber.length > 0) && (courseNumber.length > 0)) {
                    courseSection = courseNumber + '-' + sectionNumber + " ";
                } else {
                    courseSection = courseNumber + sectionNumber + ' ';
                }

                $('#CourseProductName').val(courseSection + courseName + ', ' + productName);
            });

            $('.create-section-number').keyup(function (event) {
                if (_static.fn.ignoreKey(event.which))
                    return;

                var productName = $('#ProductName').val();
                var courseName = $('#Title').val();
                var sectionNumber = $('#SectionNumber').val();
                var courseNumber = $('#CourseNumber').val();
                var courseSection = '';
                var initVal = $(this).val();

                var sectionNumber = $(this).val().replace('&#', '');



                if (initVal != sectionNumber) {
                    $(this).val(sectionNumber);
                }

                if ((sectionNumber.length > 0) && (courseNumber.length > 0)) {
                    courseSection = courseNumber + '-' + sectionNumber + " ";
                } else {
                    courseSection = courseNumber + sectionNumber + ' ';
                }

                $('#CourseProductName').val(courseSection + courseName + ', ' + productName);
            });

        }
    };
}(jQuery);
