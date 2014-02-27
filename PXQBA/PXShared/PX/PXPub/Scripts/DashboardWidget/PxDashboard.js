// Defines a singleton object that coordinates all client-side behavior of the AssignmentCenter page.
(function ($) {
    // static plugin values
    var _static = {
        pluginName: "PxDashboardWidget",
        dataKey: "PxDashboardWidget",
        bindSuffix: ".DashboardWidget",
        dataAttrPrefix: "data-dw-",
        defaults: {},
        sel: {
            createNewCourse: ".PX_DashboardWidget .show-creation-button",
            pxDashboardWidget: ".PX_DashboardWidget",
            pxDeleteCourseDialog: "#PX_DeleteCourseDialog",
            pxDeactivateCourseDialog: "#PX_DeactivateCourseDialog",
            pxActivateCourseDialog: "#PX_ActivateCourseDialog",
            pxEditCourseDialog: "#PX_EditCourseDialog",
            pxCreateCourseOptionDialog: "#PX_CreateCourseOptionDialog",
            dialogActivateDashboardCourse: "#ActivateDashBoardCourse",
            dialogDeactivateDashboardCourse: "#DeactivateDashBoardCourse",
            dialogDeleteDashboardCourse: "#DeleteDashBoardCourse",
            createBranchConfirm: "a.create-another-branch-link",
            courseTitle: "#cover #course-title",
            courseNumber: "#cover #course-number",
            cover: '#dashboard-course-creation-screen #cover',
            sectionNumber: "#cover #section-number",
            instructorName: "#cover #instructor-name",
            schoolName: "#cover #school-name",
            academicTerm: "#academicTerm",
            timeZone: "#cover #courseTimeZone",
            parentCourseId: "#cover #parent-course-id",
            dashboardCourseItem: '#dashboard-course-item',
            schoolBox: '#cover #schoolbox',
            schoolSearchResult: '#cover select#school_search_result',
            zipBox: '#cover #zipbox',
            selectedSchool: '#cover #SelectedSchool',
            userSelectedSchool: '#cover select#UserSelectedSchool',
            userSelectedSchoolSelection: "#cover select#UserSelectedSchool option:selected",
            findSchool: '#cover #FindSchool',
            findSchools: '#cover #FindSchools',
            findSchoolPopup: '#cover #FindSchoolPopup',
            findSchoolError: '#cover #FindSchoolError',
            findSchoolIcon: '#cover #findSchoolIcon',
            closeFindPopupSchool: '#cover #CloseFindPopupSchool',
            country: '#cover #Country',
            countrySelected: "#cover select#Country option:selected",
            state: '#cover select#State',
            schoolSearchByCity: '#cover #schoolsearchbycity',
            schoolSearchByZip: '#cover #schoolsearchbyzip',
            semester: "#cover #semester",
            selectedTimeZone: '#courseTimeZone option:selected',
            timeZoneOption: '#courseTimeZone option',
            rosterInformationDialog: "#roster-information-dialog",
            dialogTitle: "#cover #title-header h1", // dialog title header
            LmsIdRequired: "input[name='LmsIdRequired']"
        },
        fn: {

            //#region Activate Dashboard Course

            ActivateDashboardCourse: function (courseId, source) {
                var markup = _static.fn.UpdateCourseInfo(source, _static.sel.dialogActivateDashboardCourse, true).clone();

                _static.fn.SetServerDateTime($(markup).find('.activateddate'), courseId);

                var courseIdFromMarkup = markup.find('.course-info').attr('dw-courseId');

                var args = {
                    courseId: courseIdFromMarkup
                    , markup: markup
                    , cloned: true
                    , sourceClicked: source
                    , height: "auto", width: 700, buttonOkName: "Activate", buttonCancelName: "Cancel"
                    , buttonOkClass: "primary-green", buttonCancelClass: "branch-confirmation"
                    , buttonPaneClass: "pxactivatecourse-dialog-buttonpane", buttonSetClass: "pxactivatecourse-dialog-buttonset"
                    , callback: _static.fn.ActivateCourse
                };
                $(document).off('click', '#ActivateDashBoardCourse .editactivatecourse').on('click', '#ActivateDashBoardCourse .editactivatecourse', function(event){
                    event.preventDefault();
                    _static.fn.EditCourseLink(courseIdFromMarkup, this);
                });

                _static.fn.GenericDashboardDialog(args);

                return true;
            },

            ActivateCourse: function (courseId, sourceClicked, dialogBox) {
                PxPage.Loading('ui-dialog .dashboard-course-item', true, true);

                $.ajax({
                    url: PxPage.Routes.activate_dashboard_course,
                    data: {
                        'courseId': courseId
                    },
                    type: "POST",
                    success: function (result) {
                        if (result.status === "True") {
                            dialogBox.find(".pre-activation").addClass("disable");
                            dialogBox.find(".post-activation").removeClass("disable").addClass("enable");

                            _static.fn.SetServerDateTime(dialogBox.find('.activateddate'), result.courseid);
                            dialogBox.find(".student-url-display").html(result.url);
                            dialogBox.find("a.student-url-display").attr("href", result.url);
                            dialogBox.find(".instructor-email").html(result.instructoremail);
                            dialogBox.find(".coursetitle").html(dialogBox.find(".title").html());
                            dialogBox.parent().find('.primary-green').addClass('disable');
                            dialogBox.parent().find('.branch-confirmation').toggleClass('branch-confirmation primary-green').find('span').text('Done');
                            
                            if ($(sourceClicked).hasClass('activate-from-course-page') === false) {
                                var parentInstance = $(sourceClicked).closest('tr');
                                parentInstance.attr('data-dw-start-date', result.activationDate);
                                parentInstance.removeClass("Activate").addClass("Deactivate");
                                $(sourceClicked).removeClass("activate-dashboard-course").addClass("deactivate-dashboard-course").html("Deactivate");
                                $(sourceClicked).closest(".entityidofcourse").find(".status-cell .right").html("Active");
                                $(sourceClicked).unbind("click");
                                $(sourceClicked).bind("click", function () {
                                    var id = $(this).attr("data-dw-id");
                                    _static.fn.DeactivateDashboardCourse(id, sourceClicked);
                                });
                            }

                            var editorName = $(dialogBox).closest('.ui-dialog');
                            PxPage.ResizeDialogBox(editorName);

                            dialogBox.dialog("option", "position", { my: "center", at: "center", of: window });

                            if ($(sourceClicked).hasClass('activate-from-course-page')) {
                                dialogBox.dialog({
                                    buttons: [{
                                        text: "Done",
                                        click: function () {
                                            window.location.reload(true);
                                        }
                                    }]
                                });
                            }
                        }
                        else {
                            PxPage.Toasts.Error("Error encountered while this operation");
                        }
                        PxPage.Loaded('ui-dialog .dashboard-course-item', true, true);
                    }
                });
            },

            EditCourseLink: function (courseId, source) {
                href = PxPage.Routes.InstructorConsole_FullViewRaw.replace(/~~courseId~~/gi, courseId) + "?view=Dashboard";
                PxPage.LargeFNE.OpenFNELink(href, "Instructor Console", false, _static.fn.UpdateEditCourseLink);
                $('#fne-unblock-action-home').html('<span class="doneEditing-btn-icon"></span> Done');
                $('.dashboard-course-item.ui-dialog-content').dialog("close");

                $(PxPage.switchboard).unbind("InstructorDashboard.CourseSaved");
                $(PxPage.switchboard).bind("InstructorDashboard.CourseSaved", _static.fn.UpdateCourseDataFromLaunchPad);
            },

            UpdateEditCourseLink: function () {
                $('div.settingsSubmit-wrapper a').attr('href', '#');
                $(document).off('click', 'div.settingsSubmit-wrapper a').on('click', 'div.settingsSubmit-wrapper a', function (event) { event.preventDefault(); PxPage.UnBlock; });
            },

            UpdateCourseDataFromLaunchPad: function (event, data) {
                var result = JSON.parse(data);
                var courseId = '.entityidofcourse[data-dw-id*="' + result.Id + '"]';
                var course = $(courseId);
                var title = _static.fn.ConcatenateCourseTitle(result.CourseNumber, result.SectionNumber, result.Title);

                course.attr("data-dw-course-section", result.SectionNumber);
                course.attr("data-dw-course-number", result.CourseNumber);
                course.attr("data-dw-academic-term", result.AcademicTerm);
                course.attr("data-dw-lms-id-required", result.lmsIdRequired);  
                course.find('.course-title').html(title);
                course.find('.course-title-original').val(result.Title);
                course.find('.professor-name').html(result.CourseUserName);
                course.find('.semester').html(result.AcademicTermText);
            },

            //#endregion Activate Dashboard Course

            //#region Deactivate Dashboard Course

            DeactivateDashboardCourse: function (courseId, source) {
                var args = {
                    courseId: courseId
                    , markup: _static.fn.UpdateCourseInfo(source, _static.sel.dialogDeactivateDashboardCourse).clone()
                    , cloned: true
                    , sourceClicked: source
                    , height: "auto", width: 700, buttonOkName: "Deactivate", buttonCancelName: "Cancel"
                    , buttonOkClass: "primary-green", buttonCancelClass: "branch-confirmation"
                    , buttonPaneClass: "pxdeactivatecourse-dialog-buttonpane", buttonSetClass: "pxdeactivatecourse-dialog-buttonset"
                    , callback: _static.fn.DeactivateCourse
                };
                _static.fn.GenericDashboardDialog(args);
                return true;
            },

            DeactivateCourse: function (courseId, sourceClicked, dialogBox) {
                PxPage.Loading('ui-dialog .dashboard-course-item', true, true);
                $.ajax({
                    url: PxPage.Routes.deactivate_dashboard_course,
                    data: {
                        'courseId': courseId
                    },
                    type: "POST",
                    success: function (result) {
                        if (result.status === "True") {
                            var parentInstance = $(sourceClicked).closest('tr');
                            parentInstance.attr('data-dw-start-date', '');
                            parentInstance.removeClass("Deactivate").addClass("Activate");
                            $(sourceClicked).removeClass("deactivate-dashboard-course").addClass("activate-dashboard-course").html("Activate");
                            $(sourceClicked).closest(".entityidofcourse").find(".status-cell .right").html("Inactive");

                            $(sourceClicked).unbind().bind("click", function () {
                                var id = $(this).attr("data-dw-id");
                                _static.fn.ActivateDashboardCourse(id, sourceClicked);
                            });

                            dialogBox.filter('.ui-dialog-content').dialog("close");
                        }
                        else {
                            PxPage.Toasts.Error("Error encountered while this operation");
                        }
                        PxPage.Loaded('ui-dialog .dashboard-course-item', true, true);
                    }
                });

            },

            //#endregion Deactivate Dashboard Course

            //#region Create Course

            CreateCourse: function () {
                var courseTitle = $(_static.sel.courseTitle).val(),
                    courseNumber = $(_static.sel.courseNumber).val(),
                    sectionNumber = $(_static.sel.sectionNumber).val(),
                    instructorName = $(_static.sel.instructorName).val(),
                    school = $(_static.sel.schoolName).val(),
                    userSelectedSchoolId = $(_static.sel.userSelectedSchoolSelection).val(),
                    academicTerm = $(_static.sel.academicTerm).val(),
                    timezone = $(_static.sel.selectedTimeZone).val(),
                    parentCourseId = $(_static.sel.parentCourseId).val(),
                    lmsIdRequired = $(_static.sel.LmsIdRequired)[0].checked ? false : true

                if (_static.fn.CheckCreateCourseError() === false) {
                    return false;
                }

                PxPage.Loading('ui-dialog .dashboard-course-item', true, true);
                $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.create_course_from_dashboard,
                    data: {
                        selectedDomainId: userSelectedSchoolId,
                        courseTitle: courseTitle,
                        courseNumber: courseNumber,
                        sectionNumber: sectionNumber,
                        instructorName: instructorName,
                        school: school,
                        academicTerm: academicTerm,
                        timezone: timezone,
                        parentCourseId: parentCourseId,
                        creationMode: api.CourseInformation.CreationType,
                        lmsIdRequired: lmsIdRequired
                    },
                    success: function (response) {
                        if (response !== null) {
                            if (api.CourseInformation.CreationType === "dropdown") {
                                window.location.replace(response);
                                return;
                            }

                            var template = $(".entityidofcourse.template").clone(true);
                            template.removeClass("template");

                            var baseId = $('tr.child-course[data-dw-id=' + parentCourseId + ']').attr('data-base-id');

                            if (api.CourseInformation.CreationType === "branch" || (baseId && api.CourseInformation.CreationType === "copy")) {
                                var nextCourse = $(".dashboardgrid").find('[data-dw-id="' + parentCourseId + '"]').next();

                                template.addClass("child-course");
                                template.attr("data-current-level", "1");                                
                                template.attr("data-base-id", parentCourseId);
                                template.removeClass("parent-course");

                                $(template[0]).find(".delete-button.delete-button-visbility").removeClass("delete-button-visbility");
                                $(template[0]).find(".create-another-branch").remove();
                                $("#isBranchCreated").val("true");
                            }

                            var title = '';
                            if (response.CourseNumber !== undefined && response.CourseNumber.trim().length > 0) {
                                title = response.CourseNumber.trim() + ' - ';
                            }

                            if (response.SectionNumber !== undefined && response.SectionNumber.trim().length > 0) {
                                title += response.SectionNumber.trim() + ' - ';
                            }
                            title += response.Title;
                            
                            $(template).addClass("Activate")
                                       .attr("data-dw-id", response.Id)
                                       .attr("data-dw-course-section", sectionNumber)
                                       .attr("data-dw-course-number", courseNumber)
                                       .attr("data-dw-academic-term", response.AcademicTerm)
                                       .attr("data-dw-timezone", response.CourseTimeZone)
                                       .attr("data-dw-lms-id-required", response.LmsIdRequired);

                            $(template[0]).find(".course-title").text(title).attr("href", $(template[0]).find(".show-url.course-url").text() + response.Id);
                            $(template[0]).find(".course-title-original").val(response.Title);
                            $(template[0]).find(".course-url").text($(template[0]).find(".show-url.course-url").text() + response.Id);
                            $(template[0]).find(".professor-name").text(response.InstructorName);
                            $(template[0]).find(".domain-name").text(response.Domain.Name);
                            $(template[0]).find(".class-id").text(response.Id);
                            $(template[0]).find(".delete-dashboard-course").attr("data-dw-id", response.Id);

                            $(_static.sel.academicTerm + " option").each(function () {
                                if ($(this).val() === response.AcademicTerm) {
                                    $(template[0]).find(".semester").text($(this).text());
                                    return false;
                                }
                            });

                            $(template[0]).find(".enrollment-count-cell .right").text("0 Students");
                            $(template[0]).find(".status-cell .right ").text("Inactive");

                            var activateLink = $(template[0]).find(".title-cell .left .course-actions .activate-button .deactivate-dashboard-course");
                            activateLink.removeClass("deactivate-dashboard-course").addClass("activate-dashboard-course").text("Activate");

                            if (api.CourseInformation.CreationType === "new" || (baseId == undefined && api.CourseInformation.CreationType === "copy")) {
                                $("#PX_DashboardWidget .dashboardgrid tbody").prepend(template[0]);
                            } else {
                                if (!baseId) {
                                    $(api.CourseInformation.Course).after(template[0]);
                                }
                                else {
                                    $('tr[data-dw-id=' + baseId +']:visible').after(template[0]);
                                }
                            }

                            var newCourse = $('tr.entityidofcourse[data-dw-id=' + response.Id + ']');
                            newCourse.attr('style', 'background-color: rgb(245, 255, 159) !important');

                            $('.dashboard-course-item.ui-dialog-content').dialog("close");
                            $('#courseoptionno').trigger('click');
                            $('body').animate({                                
                                scrollTop: newCourse.offset().top
                            }, {
                                duration: 2000,
                                complete: function () {
                                    newCourse.removeAttr('style');
                                }
                            });

                            PxPage.Loaded('ui-dialog .dashboard-course-item', true, true);
                        }
                    },
                    error: function (req, status, error) {
                        PxPage.Toasts.Error(error);
                        PxPage.Loaded('ui-dialog .dashboard-course-item', true, true);
                    }
                });
            },

            CreateCourseOption: function () {
                var args = {
                    courseId: ""
                    , markup: $("#CreateCourseOption")
                    , width: 550, buttonOkName: "Next", buttonCancelName: "Cancel"
                    , buttonOkClass: "courseoption-dialog-next", buttonCancelClass: "courseoption-dialog-cancel branch-confirmation"
                    , buttonPaneClass: "courseoption-dialog-buttonpane", buttonSetClass: "courseoption-dialog-buttonset"
                    , callback: _static.fn.ShowCourseCreationScreen
                    , disableParentScollbar: true
                };
                _static.fn.GenericDashboardDialog(args);
                return true;
            },

            ShowCourseCreationScreen: function (courseId) {
                var args = {
                    element: _static.sel.pxActivateCourseDialog
                , markup: $("#dashboard-course-creation-screen")
                    , height: "auto", width: "auto", buttonOkName: "Create", buttonCancelName: "Cancel",
                    buttonOkClass: "primary-green",
                    buttonCancelClass: "branch-confirmation",
                    buttonPaneClass: "pxactivatecourse-dialog-buttonpane",
                    buttonSetClass: "pxactivatecourse-dialog-buttonset",
                    callback: _static.fn.CreateCourse
                };
                _static.fn.SetupCourseCreation();
                _static.fn.GenericDashboardDialog(args);

                return true;
            },

            SetupCourseCreation: function (course) {
                // change dialog title
                $(_static.sel.dialogTitle).text("Create Course");

                currentCourse = $(api.CourseInformation.Course[0]);

                var courseTitle, courseNumber, sectionNumber, instructorName, school, schoolId, academicTerm, courseTimezone, parentCourseId;
                if (currentCourse.length <= 0) {
                    $.ajax({
                        type: 'POST',
                        async: false,
                        url: PxPage.Routes.get_product_course,
                        success: function (response) {
                            _static.fn.PopulateDialogData(null, response);
                        },
                        error: function (req, status, error) { }
                    });
                } else {
                    _static.fn.PopulateDialogData(currentCourse);
                }
            },

            //#endregion Create Course

            //#region Copy Dashboard Course

            CreateCourseCopyGrid: function (courseId) {
                if ($(".course-copy").length > 0) {
                    return false;
                }

                var dashboard = $("table.dashboardgrid").clone();
                var courseCopyDiv = $(document.createElement('div')).attr("class", "course-copy");
                var whichcourse = $(document.createElement('p')).attr("id", "which-course-text");
                whichcourse.text("Which course?");
                courseCopyDiv.append(whichcourse);
                courseCopyDiv.append(dashboard);

                $(dashboard.find("tr")).each(function () {
                    var currentRow = $(this);
                    var semester = currentRow.find(".title-cell .semester");
                    var statusCell = currentRow.find(".status-cell .right");
                    statusCell.replaceWith(semester);

                    currentRow.find(".enrollment-count-cell").remove();
                    currentRow.find(".course-info").hide();
                    currentRow.find(".course-actions").remove();
                    currentRow.find(".course-title").removeAttr("href");

                    if (currentRow.hasClass("inner-child-course")) {
                        currentRow.removeClass("inner-child-course");
                        currentRow.addClass("child-course");
                    }
                });

                $("#CreateCourseOption").append(courseCopyDiv);
                
                $(document).off('click', '.course-copy .dashboardgrid .entityidofcourse').on('click', '.course-copy .dashboardgrid .entityidofcourse',
                    function () {
                        api.CourseInformation.Course = $(this).closest(".entityidofcourse");
                        api.CourseInformation.CreationType = "copy";
                        $(".selected").removeClass("selected");

                        $(this).addClass("selected");
                });

                $(".course-copy .dashboardgrid tr:first-child").click();
            },

            RemoveCourseCopyGrid: function (courseId) {
                $(".course-copy").remove();
            },

            //#endregion Copy Dashboard Course

            //#region Delete Dashboard Course

            DeleteDashboardCourse: function (courseId, source) {
                var markup = _static.fn.UpdateCourseInfo(source, _static.sel.dialogDeleteDashboardCourse).clone();
                var year = $(markup).find('.activateddate').text();
                if (year !== undefined && year.length > 8 && year.substr(year.length - 4, 4) === '9999') {
                    _static.fn.SetServerDateTime($(markup).find('.activateddate'));
                }

                var args = {
                    courseId: courseId
                    , markup: markup
                    , cloned: true
                    , sourceClicked: source
                    , height: "auto", width: 700, buttonOkName: "Delete", buttonCancelName: "Cancel"
                    , buttonOkClass: "pxdeletecourse-dialog-delete", buttonCancelClass: "branch-confirmation"
                    , buttonPaneClass: "pxdeletecourse-dialog-buttonpane", buttonSetClass: "pxdeletecourse-dialog-buttonset"
                    , callback: _static.fn.DeleteCourse
                };
                _static.fn.GenericDashboardDialog(args);
                return true;
            },

            DeleteCourse: function (courseId, sourceClicked, dialogBox) {
                var courseIdList = [];
                var coursesToDelete = [];
                var courseDeleted = $(sourceClicked).closest("tr");

                if (courseDeleted.attr("data-current-level") === "0") {
                    var dashboardtable = $(".dashboardgrid tr");
                    var indexofElement = $(dashboardtable).index(courseDeleted);

                    courseIdList += courseDeleted.attr("data-dw-id");
                    coursesToDelete.push(courseDeleted);

                    dashboardtable = $(".dashboardgrid tr").slice(indexofElement);
                    dashboardtable.each(function () {
                        var nextElement = $(this).next();
                        if (nextElement.attr("data-current-level") === "1") {
                            courseIdList += "," + nextElement.attr("data-dw-id");
                            coursesToDelete.push(nextElement);
                        } else {
                            return false;
                        }
                    });

                } else {
                    if ($(sourceClicked).attr("data-dw-id") === undefined)
                        sourceClicked = courseDeleted;
                    courseIdList += $(sourceClicked).attr("data-dw-id");
                    coursesToDelete.push(sourceClicked);
                    $(sourceClicked).remove();
                }

                PxPage.Loading('ui-dialog .dashboard-course-item', true, true);

                $.ajax({
                    url: PxPage.Routes.delete_course,
                    data: {
                        'coursesToDelete': courseIdList
                    },
                    type: "POST",
                    success: function (response) {
                        if (response === "True") {

                            $(coursesToDelete).each(function (index, value) {

                                $(".dashboardgrid").find('[data-dw-id="' + $(coursesToDelete[index]).attr("data-dw-id") + '"]').remove();
                            });

                            if ($(".dashboardgrid").find('[data-current-level=1]').length === 0) {
                                $("#isBranchCreated").val("false");
                            }
                            if (dialogBox != undefined || dialogBox != null) {
                                dialogBox.filter('.ui-dialog-content').dialog("close");
                            }

                            PxPage.Loaded('ui-dialog .dashboard-course-item', true, true);
                        }
                    }
                });
            },

            //#endregion Delete Dashboard Course 

            //#region Edit Dashboard Course

            EditDashboardCourse: function (courseId, source) {
                var args = {
                    element: _static.sel.pxEditCourseDialog
                    , markup: $("#dashboard-course-creation-screen")
                    , height: "auto", width: "auto", buttonOkName: "Save", buttonCancelName: "Cancel",
                    buttonOkClass: "primary-green",
                    buttonCancelClass: "branch-confirmation",
                    buttonPaneClass: "pxactivatecourse-dialog-buttonpane",
                    buttonSetClass: "pxactivatecourse-dialog-buttonset",
                    callback: _static.fn.EditCourse
                };

                // change dialog title
                $(_static.sel.dialogTitle).text("Edit course details");

                _static.fn.PopulateDialogData($(api.CourseInformation.Course[0]));

                _static.fn.GenericDashboardDialog(args);
                return true;
            },

            EditCourse: function (courseId, sourceClicked, dialogBox) {
                if (_static.fn.CheckCreateCourseError() === false)
                    return false;

                PxPage.Loading('ui-dialog .dashboard-course-item', true, true);

                courseId = $(api.CourseInformation.Course).attr("data-dw-id");
                var courseTitle = dialogBox.find("#course-title").val();
                var courseNumber = dialogBox.find("#course-number").val();
                var sectionNumber = dialogBox.find("#section-number").val();
                var instructorName = dialogBox.find("#instructor-name").val();
                var schoolName = dialogBox.find("#school-name").val();
                var academicTerm = dialogBox.find("#academicTerm option:selected").val();
                var courseTimeZone = dialogBox.find("#courseTimeZone option:selected").val();
                var LmsIdRequired = $(_static.sel.LmsIdRequired + ":checked").val();

                $.ajax({
                    url: PxPage.Routes.edit_dashboard_course,
                    data: {
                        'courseId': courseId,
                        'courseTitle': courseTitle,
                        'courseNumber': courseNumber,
                        'sectionNumber': sectionNumber,
                        'instructorName': instructorName,
                        'schoolName': schoolName,
                        'academicTerm': academicTerm,
                        'courseTimeZone': courseTimeZone,
                        'LmsIdRequired': LmsIdRequired
                    },
                    type: "POST",
                    success: function (response) {
                        if (response !== null) {
                            var title = _static.fn.ConcatenateCourseTitle(response.CourseNumber, response.SectionNumber, response.Title);
                            $(api.CourseInformation.Course).find(".course-title").text(title);
                            $(api.CourseInformation.Course).find(".course-title-original").val(response.Title);
                            $(api.CourseInformation.Course).find(".professor-name").text(response.InstructorName);
                            $(api.CourseInformation.Course).find(".domain-name").text(response.Domain.Name);
                            $(api.CourseInformation.Course).find(".class-id").text(response.Id);
                            $(_static.sel.academicTerm + " option").each(function () {
                                if ($(this).val() === response.AcademicTerm) {
                                    $(api.CourseInformation.Course).find(".semester").text($(this).text());
                                    return false;
                                }
                            });
                            
                            _static.fn.SetDwServerDateTime("data-dw-start-date", (response.Id));
                            $(api.CourseInformation.Course).attr("data-dw-course-number", response.CourseNumber);
                            $(api.CourseInformation.Course).attr("data-dw-course-section", response.SectionNumber);
                            $(api.CourseInformation.Course).attr("data-dw-academic-term", response.AcademicTerm);
                            $(api.CourseInformation.Course).attr("data-dw-timezone", response.CourseTimeZone);
                            $(api.CourseInformation.Course).attr("data-dw-lms-id-required", response.LmsIdRequired);
                            $(".dashboard-course-item.ui-dialog-content").dialog("close");
                        }
                        else {
                            PxPage.Toasts.Error("Error encountered while this operation");
                        }

                        PxPage.Loaded('ui-dialog .dashboard-course-item', true, true);
                    }
                });
            },

            //#endregion Edit Dashboard Course

            //#region Update Dashboard Course

            UpdateCourseInfo: function (source, coursedialog, activationScreen) {
                var data = _static.fn.RetrieveCourseData(source, false);

                if (data !== undefined || data !== null) {
                    var courseInfo = $(coursedialog).find('.course-info');
                    courseInfo.attr('dw-courseid', data.courseId);
                    courseInfo.find(".title").html(data.courseTitle);
                    courseInfo.find(".coursenumber").html(data.courseNumber);
                    courseInfo.find(".sectionnumber").html(data.sectionNumber);
                    courseInfo.find(".schoolname").html(data.school);
                    courseInfo.find(".activateddate").html(data.activatedDate);

                    $(_static.sel.academicTerm + " option").each(function () {
                        if ($(this).val() === data.academicTerm) {
                            courseInfo.find(".academicterm").html($(this).text());
                            return false;
                        }
                    });

                    // data.courseTimezone is id 
                    $(_static.sel.timeZoneOption).each(function () {
                        if ($(this).val() === data.courseTimezone) {
                            courseInfo.find(".coursetimezone").html($(this).text());
                            return false;
                        }
                    });
                }

                if (activationScreen === true) {
                    $(coursedialog).find(".pre-activation").removeClass("disable").addClass("enable");
                    $(coursedialog).find(".post-activation").removeClass("enable").addClass("disable");

                }
                return $(coursedialog);
            },

            PopulateDialogData: function (course, response) {
                var data;

                if (response !== undefined) {
                    data = _static.fn.RetrieveCourseData(response, true);
                }
                else {
                    data = _static.fn.RetrieveCourseData(course, false);
                }

                if (data !== undefined || data !== null) {
                    $(_static.sel.courseTitle).val(data.courseTitle);
                    $(_static.sel.courseNumber).val(data.courseNumber);
                    $(_static.sel.sectionNumber).val(data.sectionNumber);
                    $(_static.sel.instructorName).val(data.instructorName);
                    $(_static.sel.schoolName).val(data.school);
                    $(_static.sel.academicTerm + " option").each(function () {
                        this.selected = $(this).val() === data.academicTerm;
                    });
                    $(_static.sel.timeZoneOption).each(function () {
                        this.selected = $(this).val() === data.courseTimezone;
                    });
                    $(_static.sel.parentCourseId).val(data.courseId);
                    $(_static.sel.LmsIdRequired + "[value=" + data.lmsIdRequired.toString().toLowerCase() + "]").prop('checked', true);
                }

                _static.fn.EnableSchoolSearchAutocomplete();

                _static.fn.ShowTimeZoneError(false);

                if ($(_static.sel.selectedTimeZone).length === 0) {
                    $(_static.sel.timeZone).prepend("<option value='' selected='selected'></option>");
                }
            },

            RetrieveCourseData: function (course, isReturnedData) {
                var courseTitle, courseNumber, sectionNumber, instructorName, school, schoolId,
                    academicTerm, courseTimezone, activatedDate, courseId, lmsIdRequired;
                if (typeof String.prototype.trim !== 'function') {
                    String.prototype.trim = function () {
                        return this.replace(/^\s+|\s+$/g, '');
                    }
                }
                if (isReturnedData) {
                    if (course.Title !== null) {
                        courseTitle = course.Title;
                    }
                    courseNumber = course.CourseNumber;
                    sectionNumber = course.SectionNumber;
                    instructorName = course.InstructorName;
                    if (course.Domain.Name === undefined || course.Domain.Name.toLowerCase() === "pxgeneric") {
                        school = "";
                    } else {
                        school = course.Domain.Name;
                    }
                    schoolId = course.Domain.Id;
                    academicTerm = course.AcademicTerm;
                    courseTimezone = course.CourseTimeZone;
                    activatedDate = course.ActivatedDate;
                    courseId = $(".dashboardgrid").attr("data-dw-dashboard-id");
                    lmsIdRequired = course.LmsIdRequired;   
                }
                else {
                    if (course.length === undefined) {
                        course = $(course).closest("tr");
                    }
                    if (course.length > 0) {
                        courseId = course.attr("data-dw-id");
                        courseTitle = course.find(".course-title-original").val();
                        if (courseTitle === undefined) {
                            courseTitle = course.find(".course-title").text();
                        }
                        courseNumber = course.attr("data-dw-course-number");
                        sectionNumber = course.attr("data-dw-course-section");
                        instructorName = course.find(".professor-name").text();
                        var academicTerm = course.attr("data-dw-academic-term");
                        $(_static.sel.academicTerm + " option").each(function () {
                            if ($(this).text() === academicTerm) {
                                academicTerm = $(this).val();
                                return;
                            }
                        });
                        school = course.find(".course-info .domain-name").text();
                        if (school === "") {
                            school = course.attr("data-dw-school-name");
                        }
                        schoolId = course.find(".domain-name").attr("data-dw-domain-id");
                        courseTimezone = course.attr("data-dw-timezone");
                        activatedDate = course.attr('data-dw-start-date');
                        lmsIdRequired = course.attr("data-dw-lms-id-required");  
                    }
                }

                var arrJson = {
                    "courseId": (courseId !== undefined) ? courseId.trim() : courseId,
                    "courseTitle": (courseTitle !== undefined) ? courseTitle.trim() : courseTitle,
                    "courseNumber": (courseNumber !== undefined) ? courseNumber.trim() : courseNumber,
                    "sectionNumber": (sectionNumber !== undefined) ? sectionNumber.trim() : sectionNumber,
                    "instructorName": (instructorName !== undefined) ? instructorName.trim() : instructorName,
                    "academicTerm": (academicTerm !== undefined) ? academicTerm.trim() : academicTerm,
                    "school": (school !== undefined) ? school.trim() : school,
                    "schoolId": (schoolId !== undefined) ? schoolId.trim() : schoolId,
                    "courseTimezone": (courseTimezone !== undefined) ? courseTimezone.trim() : courseTimezone,
                    "activatedDate": activatedDate,
                    "lmsIdRequired": lmsIdRequired
                };

                return arrJson;
            },

            //#endregion Update Dashboard Course

            //#region Branch

            ShowBranchInstruction: function () {
                var createBranchDialog = $("#create-branch-confirmation");

                var args = {
                    markup: createBranchDialog,
                    width: "auto",
                    buttonOkName: "Yes, I\'d like to branch this course",
                    buttonOkClass: "primary-green",
                    buttonCancelName: "Cancel",
                    buttonCancelClass: "branch-confirmation",
                    buttonPaneClass: "pxactivatecourse-dialog-buttonpane",
                    buttonSetClass: "pxactivatecourse-dialog-buttonset",
                    callback: _static.fn.ShowCourseCreationScreen
                };

                _static.fn.GenericDashboardDialog(args);
                return true;
            },

            //#endregion Branch

            //#region Dialogs

            //#region Roster

            OpenDialogForRoster: function (response) {
                $(_static.sel.rosterInformationDialog).html(response);
                $(_static.sel.rosterInformationDialog).dialog("open");
            },

            RosterDialogInitializer: function () {
                $(_static.sel.rosterInformationDialog).dialog({
                    autoOpen: false,
                    height: 750,
                    minWidth: 900,
                    resizable: false,
                    dialogClass: 'roster-information-dialog',
                    modal: true
                });
            },

            //#endregion Roster

            GenericDashboardDialog: function (args) {
                var newDiv = $("static");
                var markup = $(args.markup);
                var editorName = "dashboard-course-item";
                var sourceClicked = args.sourceClicked;
                var markuptogether = "";

                if (newDiv.length > 0) {
                    newDiv.empty();
                }
                else {
                    newDiv = $(document.createElement('div')).attr("id", editorName).attr("class", editorName);
                }

                if (markup.length > 1) {
                    for (var index = 0; index < markup.length; index++) {

                        markuptogether += markup[index].outerHTML;
                    }
                    if (markuptogether !== null) {
                        markup = markuptogether;
                    }
                } else {
                    markup.show();
                }
                if (markup !== null) {
                    newDiv.html(markup);
                    newDiv.dialog({
                        autoOpen: true,
                        height: args.height,
                        width: args.width,
                        resizable: false,
                        dialogClass: editorName,
                        modal: true,
                        buttons: [{
                            text: args.buttonOkName,
                            'class': args.buttonOkClass,
                            click: function () {
                                if (args.callback !== null) {
                                    var returnValue = args.callback(args.courseId, sourceClicked, $(this));
                                    if (returnValue === true) {
                                        $(this).dialog("close");
                                    }
                                }
                            }
                        }, {
                            text: args.buttonCancelName,
                            'class': args.buttonCancelClass,
                            click: function () {
                                $(this).dialog("close");
                            }
                        }],
                        close: function () {
                            if (args.cloned === true) {
                                $(this).remove();
                            }
                            if (args.redirectUrl !== null
                                && args.redirectUrl !== undefined
                                && args.redirectUrl !== '') {
                                window.location.reload(true);
                            }
                            if (args.disableParentScollbar) {
                                $('html').css('overflow', 'auto');
                            }
                        },
                        create: function (event, ui) {
                            var dialog = $(this).parent();
                            dialog.find(".ui-dialog-titlebar").css("display", "none");
                            dialog.css("padding", 0);
                            dialog.css("border-radius", "10px");
                            dialog.find(".ui-dialog-content").css("padding", 20);
                            dialog.find(".ui-dialog-buttonpane").addClass(args.buttonPaneClass);
                            dialog.find(".ui-dialog-buttonpane .ui-dialog-buttonset").addClass(args.buttonSetClass)
                            dialog.css("position", "fixed");
                            dialog.css("top", ($(window).height() / 2 - (dialog.height() / 2)));
                            if (args.disableParentScollbar) {
                                $('html').css('overflow', 'hidden');
                            }
                        }

                    });

                    PxPage.Update();
                } else {

                    PxPage.Toasts.Error("No markup found");

                }
            },

            GetCourseCreationDialog: function () {
                PxPage.Loading();
                api.CourseInformation.CreationType = "dropdown";
                $.ajax({
                    url: PxPage.Routes.show_create_course_dialog,
                    type: "POST",
                    success: function (result) {

                        var args = {
                            markup: result,
                            height: "auto",
                            width: "auto",
                            buttonOkName: "Create",
                            buttonCancelName: "Cancel",
                            buttonOkClass: "primary-green",
                            buttonCancelClass: "branch-confirmation",
                            buttonPaneClass: "pxactivatecourse-dialog-buttonpane",
                            buttonSetClass: "pxactivatecourse-dialog-buttonset",
                            callback: _static.fn.CreateCourse
                        };
                        _static.fn.GenericDashboardDialog(args);

                        var schoolName = $("#school-list").val();
                        schoolName = JSON.parse(schoolName);
                        $("#school-name").autocomplete({
                            source: schoolName,
                            minLength: 0
                        });


                        $(_static.sel.schoolName).unbind("autocompleteopen");
                        $(_static.sel.schoolName).bind('focus', function () {

                            $(this).autocomplete("search");
                        });
                        $(_static.sel.schoolName).bind("autocompleteopen", function () {
                            var element = $('.ui-autocomplete');
                            var findschoolelement = $('<li class="ui-menu-item find-school" role="menuitem"><a class="ui-corner-all" href="#" id="ui-active-menuitem">Find your school</a></li>');
                            element.append('<li class="ui-menu-item" role="menuitem"><hr /></li>');
                            element.append(findschoolelement);
                            $('.find-school').off('click');
                            $('.find-school').on('click', function (event) {
                                event.preventDefault();
                                _static.fn.FindSchoolAction();
                                $(_static.sel.schoolName).autocomplete("close");
                                return false;
                            });
                        });

                        $("#school-name").unbind("autocompleteselect");
                        $("#school-name").bind("autocompleteselect", function (event, ui) {
                            event.preventDefault();
                            $(this).val(ui.item.label);
                            $(this).attr("style", "border:default");
                            _static.fn.InitializeSelectedSchoolSearch(ui.item.label, ui.item.value);
                        });

                        $("#school-name").unbind("autocompletefocus");
                        $("#school-name").bind("autocompletefocus", function (event, ui) {
                            event.preventDefault();
                            $(this).val(ui.item.label);
                            $(this).attr("style", "border:default");
                            _static.fn.InitializeSelectedSchoolSearch(ui.item.label, ui.item.value);
                        });
                        PxPage.Loaded();
                    }
                });

            },

            //#endregion Dialogs

            //#region Generic

            ConcatenateCourseTitle: function (courseNumber, sectionNumber, title) {
                var concatTitle = '';
                if (courseNumber !== undefined && courseNumber.trim().length > 0) concatTitle = courseNumber.trim() + ' - ';
                if (sectionNumber !== undefined && sectionNumber.trim().length > 0) concatTitle += sectionNumber.trim() + ' - ';
                concatTitle += title;
                return concatTitle;
            },

            GetAcademicTermsByDomain: function () {
                var domainid = $(_static.sel.schoolName).val();
                var selAcademicTerm = $(_static.sel.academicTerm).val();
                $.getJSON(PxPage.Routes.GetAcademicTerms + "?domainname=" + domainid, function (results) {
                    var html = "";
                    for (k = 0; k < results.length; k++) {
                        var selected = "";
                        if (selAcademicTerm === results[k].Id) {
                            selected = "selected='selected'";
                        }
                        html += ("<option value='" + results[k].Id + "'" + selected + ">" + results[k].Name + "</option>");
                    }
                    $('select#academicTerm').html(html).removeAttr("disabled");
                });
            },

            SetDwServerDateTime: function (attribute, courseId) {
                $.ajax({
                    url: PxPage.Routes.get_current_date_time,
                    data: {
                        'dashboardCourseId': courseId
                    }
                }).done(function (data) {
                    $(api.CourseInformation.Course).attr(attribute, data);
                });
            },


            SetServerDateTime: function (startDate, courseId) {
                $.ajax({
                    url: PxPage.Routes.get_current_date_time,
                    data: {
                        'dashboardCourseId': courseId
                    },
                    success: function (data) {
                        startDate.text(data);
                    },
                    error: function () {
                        startDate.text($('#startdate').val());
                    }
                });
            },

            //#region Show/Hide URL

            ShowUrl: function (target) {
                var targetRow = $(target).closest(".entityidofcourse").children(".title-cell");
                if (targetRow.children("span.show-url").css("display") === "none") {
                    targetRow.children("span.show-url").css("display", "block");
                    targetRow.children("a.show-url-hover").text("Hide Url");
                } else {
                    targetRow.children("span.show-url").css("display", "none");
                    targetRow.children("a.show-url-hover").text("Show Url");
                }
            },

            ShowUrlHoverText: function (target) {

                $('a.show-url-hover,a.show-url-delete,a.show-url-view-roster,a.show-url-view-edit').css('display', 'none');
                var targetRow = $(target).children(".title-cell").children("a.show-url-hover");
                var deleteRow = $(target).children(".enrollment-count-cell").find("a.show-url-delete");
                var rosterLink = $(target).children(".enrollment-count-cell").find("a.show-url-view-roster");
                var editLink = $(target).children(".enrollment-count-cell").find("a.show-url-view-edit");
                targetRow.css("display", "block");
                deleteRow.css("display", "block");
                rosterLink.css("display", "block");
                editLink.css("display", "block");
            },

            HideUrlHoverText: function (target) {
                $('a.show-url-hover,a.show-url-delete,a.show-url-view-roster,a.show-url-view-edit').css('display', 'none');
            },

            //#endregion Show/Hide URL            

            ToggleInstruction: function (toggleElement) {
                $(toggleElement).toggleClass('display-branch-none');
                var editorName;
                editorName = $('#create-branch-confirmation').closest(_static.sel.dashboardCourseItem).parent();
                PxPage.ResizeDialogBox(editorName);
            },

            //#endregion Generic        

            //#region Roster

            ShowRoster: function (event) {
                PxPage.Loading('fne-content');
                var target = $(event.target);
                var courseId = $(target).closest('tr').attr("data-dw-id");
                var courseText = $(target).closest('tr').find('.course-title').text();
                $.get(PxPage.Routes.show_roster_info, { courseIdForRoster: courseId }, function (response) {
                    _static.fn.RemovePreviousRosterData();
                    _static.fn.OpenDialogForRoster(response);
                    $('.course-roster-title-text').text(courseText);
                    PxPage.Loaded('fne-content');
                });
                event.stopPropagation();
                event.preventDefault();
            },

            RemovePreviousRosterData: function () {
                $('#studentsRosterInformation').remove();
            },

            //#endregion Roster

            //#region School Search

            AdjustCourseCreateScreen: function () {
                var editorName = $(_static.sel.cover).closest(_static.sel.dashboardCourseItem).parent();
                PxPage.ResizeDialogBox(editorName);
            },

            EnableSchoolSearchAutocomplete: function () {
                var schoolName = $("#school-list").val();
                if (!schoolName)
                    return;
                schoolName = JSON.parse(schoolName);
                $("#school-name").autocomplete({
                    source: schoolName,
                    minLength: 0,
                    create: function() {
                        $(this).autocomplete('widget').addClass('schoolNames');
                        //<!-- School list autocomplete selection -->
                        $(_static.sel.schoolName).unbind("autocompleteopen");
                        $(_static.sel.schoolName).bind('focus', function () {

                            $(this).autocomplete("search");
                        });

                        $("#school-name").unbind("autocompleteselect");
                        $("#school-name").bind("autocompleteselect", function (event, ui) {
                            event.preventDefault();
                            $(this).val(ui.item.label);
                            $(this).attr("style", "border:default");
                            _static.fn.InitializeSelectedSchoolSearch(ui.item.label, ui.item.value);
                        });

                        $("#school-name").unbind("autocompletefocus");
                        $("#school-name").bind("autocompletefocus", function (event, ui) {
                            event.preventDefault();
                            $(this).val(ui.item.label);
                            $(this).attr("style", "border:default");
                            _static.fn.InitializeSelectedSchoolSearch(ui.item.label, ui.item.value);
                        });
                        //<!-- End of School list autocomplete selection -->
                    }
                });
                
            },

            FindSchoolAction: function () {
                $(_static.sel.findSchoolPopup).show();
                $(_static.sel.selectedSchool).hide();
                _static.fn.AdjustCourseCreateScreen();
            },

            InitializeSelectedSchoolSearch: function (school, id) {
                $(_static.sel.userSelectedSchool).html('');
                if (school !== 'undefined' && id !== 'undefined') {
                    $(_static.sel.userSelectedSchool).html('<option value="' + id + '" selected="selected">' + school + '</option>');
                    $(_static.sel.schoolName).style = "border:default";
                    $(_static.sel.findSchoolError).removeClass('enable').addClass('disable');
                    $(_static.sel.findSchool).removeClass('enable').addClass('disable');
                }
            },

            InitializeSchoolSearch: function () {
                $(_static.sel.findSchools).val('Find Schools');
                $(_static.sel.schoolSearchResult).html('');
                $(_static.sel.schoolSearchResult).hide();
                var icon = $(_static.sel.findSchoolIcon);
                if (icon.hasClass('find-icon') === false)
                    icon.addClass('find-icon');
                _static.fn.AdjustCourseCreateScreen();
            },

            //#endregion School Search

            //#region Validation

            CheckCreateCourseError: function () {
                var flag = true;
                var courseTitle = $(_static.sel.courseTitle).val(),
                    school = $(_static.sel.schoolName).val(),
                    timeZone = $(_static.sel.selectedTimeZone).text(),
                    userSelectedSchool = $(_static.sel.userSelectedSchoolSelection).text(),
                    academicTerm = $(_static.sel.academicTerm).val();

                if (courseTitle.trim() === "") {
                    $(_static.sel.courseTitle).css("border", "2px red solid");
                    flag = false;
                } else if (courseTitle.length > 0) {
                    $(_static.sel.courseTitle).attr("style", "border:default");
                }

                if (academicTerm === null) {
                    _static.fn.ShowAcademicTermError(true);
                    flag = false;
                } else {
                    _static.fn.ShowAcademicTermError(false);
                }
                if (typeof userSelectedSchool !== undefined
                    && (!userSelectedSchool || school.trim() !== userSelectedSchool.trim())) {
                    var arrSchool = JSON.parse($("#school-list").val());
                    $.each(arrSchool, function (i, field) {
                        if (field.label === school) {
                            userSelectedSchool = school;
                            return;
                        }
                    });
                }
                if (school.trim() === "" || userSelectedSchool === undefined || school.trim() !== userSelectedSchool.trim()) {
                    _static.fn.ShowSchoolNameError(true);
                    flag = false;
                } else if (courseTitle.length > 0) {
                    _static.fn.ShowSchoolNameError(false);
                }

                if (timeZone === undefined || timeZone === null || timeZone.trim().length === 0) {
                    _static.fn.ShowTimeZoneError(true);
                    flag = false;
                } else {
                    _static.fn.ShowTimeZoneError(false);
                }

                if (flag === false) {
                    $("#error-message").show();
                } else {
                    $("#error-message").hide();
                }

                return flag;
            },

            ShowAcademicTermError: function (error) {
                if (error === true) {
                    $(_static.sel.academicTerm).css("border", "2px red solid");
                } else {
                    $(_static.sel.academicTerm).attr("style", "border:default");
                }
            },

            ShowSchoolNameError: function (error) {
                if (error === true) {
                    $(_static.sel.schoolName).css("border", "2px red solid");
                    $(_static.sel.findSchoolError).removeClass('disable').addClass('enable');
                } else {
                    $(_static.sel.schoolName).attr("style", "border:default");
                    $(_static.sel.findSchoolError).removeClass('enable').addClass('disable');
                }
            },

            ShowTimeZoneError: function (error) {
                if (error === true) {
                    $(_static.sel.timeZone).css("border", "2px red solid");
                } else {
                    $(_static.sel.timeZone).attr("style", "border:default");
                }
            }

            //#endregion Validation
        }
    };

    // The public interface for interacting with this plugin.
    var api = {
        // The init method sets up the data for the plugin using the given
        // option values to override the defaults. 
        CourseInformation: {},
        init: function (options) {

            return this.each(function () {
                if ($(".product-type-learningcurve").length > 0) {
                    
                    $("table .entityidofcourse").unbind().bind({
                        mouseenter: function () {
                            _static.fn.ShowUrlHoverText(this);
                        },
                        mouseleave: function ()
                        { _static.fn.HideUrlHoverText(this); }
                    });
                    
                    $("a.show-url-hover").unbind().bind("click", function () {
                        _static.fn.ShowUrl(this);
                    });
                    $("a.show-url-delete").unbind().bind("click", function (event) {
                        event.preventDefault();

                        //Get course title and id;
                        var courseTitle = $(this).closest(".entityidofcourse").find('.course-title').html();
                        var courseId = $(this).closest(".entityidofcourse").attr("data-dw-id");
                        var courseTd = $(this);
                        //Create confirmation div
                        var conifrmationDiv = $('<div style="line-height: 1.5em" ><small>Are you sure you want to delete this course? <br /> <b>' +
                                                courseTitle +
                                                '</b><br />Any assignments you have created will be lost.</small></div>');
                        conifrmationDiv.dialog({
                            width: 300,
                            resizable: false,
                            title: 'Delete Course',
                            buttons: [{
                                text: 'Confirm Delete',
                                'class': 'red-background-button',
                                click: function () {
                                    _static.fn.DeleteCourse(courseId, courseTd);
                                    $(this).dialog('close');
                                }
                            },
                            {
                                text: 'Cancel',
                                'class': 'courseoption-dialog-cancel',
                                click: function () {
                                    $(this).dialog('close');
                                }
                            }]
                        });
                    });

                    _static.fn.RosterDialogInitializer();

                    $("a.show-url-view-roster").unbind().bind("click", function (event) {
                        event.preventDefault();
                        _static.fn.ShowRoster(event);
                    });
                    
                    var editCourseLinks = $('.edit-course-link');
                    if (editCourseLinks.length > 0) {
                        $(editCourseLinks).each(function (i, v) {
                            var courseId = $(v).closest('tr').attr('data-dw-id');
                            var urlQuery = "";
                            if ($(v).hasClass('s-en')) {
                                urlQuery = courseId + '?id=' + courseId + '&hasEnrollment=true&activateCourse=false';
                            } else {
                                urlQuery = courseId + '?id=' + courseId + '&hasEnrollment=false&activateCourse=false';
                            }
                            $(v).attr('href', PxPage.Routes.show_course_activation + '/' + urlQuery);
                            $(v).addClass('fne-link');
                        });
                    }
                }



                $(document).off("click", 'div#create-course').on("click", 'div#create-course', function (event) {
                    event.preventDefault();
                    _static.fn.CreateCourseOption(this);
                    api.CourseInformation.Course = $(this).closest(".entityidofcourse");
                    api.CourseInformation.CreationType = "new";
                });

                $(document).off('click', '#CreateCourseOption #courseoptionyes').on('click', '#CreateCourseOption #courseoptionyes', function (event) {
                    //event.preventDefault(true);
                    _static.fn.CreateCourseCopyGrid();
                    var editorName = $("#CreateCourseOption").closest(_static.sel.dashboardCourseItem).parent();
                    PxPage.ResizeDialogBox(editorName);
                });

                $(document).off('click', '#CreateCourseOption #courseoptionno').on('click', '#CreateCourseOption #courseoptionno', function (event) {
                    //If select option no, clear course information.
                    if (api.CourseInformation && api.CourseInformation.Course)
                        api.CourseInformation.Course = [];
                    
                    _static.fn.RemoveCourseCopyGrid();
                    var editorName = $("#CreateCourseOption").closest(_static.sel.dashboardCourseItem).parent();
                    PxPage.ResizeDialogBox(editorName);
                });

                $('a.activate-dashboard-course').unbind().bind('click', function (event) {
                    event.preventDefault();
                    var id = $(this).attr("data-dw-id");
                    _static.fn.ActivateDashboardCourse(id, this);
                });

                $('a.deactivate-dashboard-course').unbind().bind('click', function (event) {
                    event.preventDefault();
                    var id = $(this).attr("data-dw-id");
                    _static.fn.DeactivateDashboardCourse(id, this);
                });

                $(document).off('click', 'a.edit-dashboard-course').on('click', 'a.edit-dashboard-course', function (event) {
                    event.preventDefault();
                    var id = $(this).attr("data-dw-id");

                    api.CourseInformation.Course = $(this).closest(".entityidofcourse");

                    _static.fn.EditDashboardCourse(id, this);
                });

                $(document).off('click', 'a.delete-dashboard-course').on('click', 'a.delete-dashboard-course', function (event) {
                    event.preventDefault();
                    var id = $(this).attr("data-dw-id");
                    _static.fn.DeleteDashboardCourse(id, this);
                });

                $(PxPage.switchboard).bind("PxDashboard.CreationDialog", _static.fn.GetCourseCreationDialog);

                $(document).off('click', '#lnkActivateCourse').on('click', '#lnkActivateCourse', function (event) {
                    event.preventDefault();
                    var id = $(this).attr("data-dw-id");
                    _static.fn.ActivateDashboardCourse(id, this);
                });

                $(_static.sel.schoolName).off().on("blur", function () {
                    _static.fn.GetAcademicTermsByDomain();
                });

                $('body').off('click', _static.sel.createBranchConfirm).on('click', _static.sel.createBranchConfirm, function (event) {
                    event.preventDefault();
                    api.CourseInformation.Course = $(this).closest(".entityidofcourse");
                    api.CourseInformation.CreationType = "branch";
                    if ($("#isBranchCreated").val().toLowerCase() === "true") {
                        _static.fn.ShowCourseCreationScreen();
                    } else {
                        _static.fn.ShowBranchInstruction();
                    }
                });
                $(document).off('click', '.instruction-1').on('click', '.instruction-1', function (event) {
                    event.preventDefault();
                    _static.fn.ToggleInstruction('#branch-instruction-1');
                });

                $(document).off('click', '.instruction-2').on('click', '.instruction-2', function (event) {
                    event.preventDefault();
                    _static.fn.ToggleInstruction('#branch-instruction-2');
                });
            });
        },

        // the destroy method cleans up any plugin related data for the instance
        // on which it is called.
        destroy: function () {
            return this.each(function () {
                $(this).unbind(_static.bindSuffix);
            });
        },
        // opens the dialog in the given mode
        //
        // args {
        //    mode: determines if the dialog is opened in "add new content" or "create content" mode
        //    onDialogOpen: callback function for when dialog is open
        //    onDialogClosed: callback function for when the dialog is closed
        // }
        open: function (args) {

        },
        // closes the dialog if it is open
        close: function () {

        }

    };

    // Associate the plugin with jQuery
    $.fn.PxDashboardWidget = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        }
        else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };
}(jQuery));