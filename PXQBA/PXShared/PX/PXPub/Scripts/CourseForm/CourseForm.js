var CourseForm = function ($) {
    // static data
    var _static = {
        defaults: {
            contactInfoCount: 0,
            courseInfoWidgetId: "PX_Course_Information",
            timezone: "",
            reloadPage: false,
            isLmsEnabled: false
        },
        sel: {
            addContact: "#addContact",
            dynamicElementTarget: "#dynamicElementTarget",
            courseInformationForm: "#courseInformationForm",
            launchpadForm: "#launchpadForm",
            submitForm: "#submitForm",
            submitlaunchpadForm: "#submitlaunchPadForm",
            fileUploadForm: "#fileUploadForm",
            UploadResults: "#UploadResults",
            RefUrlFilePath: "#RefUrlFilePath",
            RefUrlFileName: "#RefUrlFileName",
            isUploadValid: "#isUploadValid",
            rdlink: ".rdlink",
            rdupload: ".rdupload",
            syllabusblock: "#syllabusblock",
            uploadblock: "#uploadblock",
            aboutWidget: "#PX_Course_Information",
            consoleWrapper: '#instructor-console-wrapper',
            placeholder: 'placeholder',
            inputPlaceholder: 'input[placeholder]',
            officeHours: '.officeHours',
            contactInfoType: '.contactInfoType',
            contactInfoValue: '.contactInfoValue',
            contactInfoInput: '.contactInfoInput',
            syllabusURL: '#SyllabusURL',
            courseTimeZone: '#CourseTimeZone',
            uploadSubmit: '#uploadSubmit',
            errorMessage: '.errorMessage',
            courseActivationWidget: "#PX_Course_Activation_Widget",
            lmsWhatsThisBlock: "#lmsWhatsThisBlock",
            lmsWhatsThisShow: "#lmsWhatsThisShow",
            lmsWhatsThisHide: "#lmsWhatsThisHide",
            LmsIdRequiredFalse: "#LmsIdRequiredFalse",
            LmsIdRequiredTrue: "#LmsIdRequiredTrue",
            lmsIdRequiredPrompt: "#lmsIdRequiredPrompt"
        },
        fn: {
            GetFormData: function () {
                var formData = {};
                var temp = $(_static.sel.courseInformationForm).serializeArray();
                formData = "{";

                $.each(temp, function (key, d) {
                    formData += '"' + d.name + '" : "' + d.value + '",'
                });

                formData = formData + ' "AcademicTermText" : "' + $(_static.sel.courseInformationForm).find('#AcademicTerm option:selected').text() + '"';
                formData = formData + "}";
                return formData;
            },
            ShowLmsWhatsThis: function (show) {
                if (show) {
                    $(_static.sel.lmsWhatsThisBlock).show();
                    $(_static.sel.lmsWhatsThisShow).hide();
                    $(_static.sel.lmsWhatsThisHide).show();
                } else {
                    $(_static.sel.lmsWhatsThisBlock).hide();
                    $(_static.sel.lmsWhatsThisShow).show();
                    $(_static.sel.lmsWhatsThisHide).hide();
                }
            },
            ShowLmsIdRequired: function (show) {
                if (show) {
                    $(_static.sel.lmsIdRequiredPrompt).show();
                } else {
                    $(_static.sel.lmsIdRequiredPrompt).hide();
                }
            }
        }
    };
    // static functions
    return {
        Init: function (contactInfoCount) {
            _static.defaults.timezone = $(_static.sel.courseTimeZone).val();
            _static.defaults.contactInfoCount = contactInfoCount;
            CourseForm.BindControls();
            _static.fn.ShowLmsWhatsThis(false);
        },
        BindControls: function () {

            var options = {
                type: "post",
                beforeSubmit: ShowRequest,
                dataType: 'json',
                success: SubmitSuccesful,
                error: AjaxError
            };

            $(_static.sel.uploadSubmit).bind('click', function () {
                $(_static.sel.fileUploadForm).ajaxSubmit(options);
            });

            SetPlaceHolder();

            $(_static.sel.contactInfoType).bind('change', function () {
                SetPlaceHolder();
            });

            $(_static.sel.submitForm).bind('click', function () {
                var isValid = true;

                $($('input:visible[type="text"]')).each(function () {
                    if ($(this).val().match(/<(\w+)((?:\s+\w+(?:\s*=\s*(?:(?:"[^"]*")|(?:'[^']*')|[^>\s]+))?)*)\s*(\/?)>/)) {                        
                        var text = $('label[for="' + $(this).attr('id') + '"]').text();
                        PxPage.Toasts.Error(text + ' should not contain any html tags!');
                        isValid = false;
                    }
                });

                if (!isValid) {
                    return false;
                }

                $.blockUI();

                $(_static.sel.errorMessage).text('');

                if (_static.defaults.timezone != $(_static.sel.courseTimeZone).val()) {
                    _static.defaults.reloadPage = true;
                }

                var formData = {};

                if ($(_static.sel.courseInformationForm).find(_static.sel.isUploadValid).length == 0) {
                    $(_static.sel.courseInformationForm).append(
                        $("<input type='hidden' />").attr({
                            id: _static.sel.isUploadValid.replace('#', ''),
                            name: _static.sel.isUploadValid.replace('#', ''),
                            value: $(_static.sel.isUploadValid).val()
                        })
                    );
                }

                if ($(_static.sel.courseInformationForm).find(_static.sel.RefUrlFilePath).length == 0) {
                    $(_static.sel.courseInformationForm).append(
                        $("<input type='hidden' />").attr({
                            id: _static.sel.RefUrlFilePath.replace('#', ''),
                            name: _static.sel.RefUrlFilePath.replace('#', ''),
                            value: $(_static.sel.RefUrlFilePath).val()
                        })
                    );
                }

                $.ajax({
                    url: PxPage.Routes.InstructorConsole_UpdateCourse,
                    data: $(_static.sel.courseInformationForm).serialize(),
                    type: 'POST',
                    success: function (data) {
                        $.unblockUI();
                        formData = _static.fn.GetFormData();
                        if (data.Error != undefined && eval(data.Error).length > 0) {
                            $('[id^="error"]').html('');

                            var errors = eval(data.Error);

                            for (var i = 0; i < errors.length; i++) {
                                $("#error" + errors[i].id).html(errors[i].messages.toString());
                            }

                            return;
                        }
                        else if (data.Error != undefined && data.Error.id == "url") {
                            $('#urlError').html(data.Error.Message);

                            return;
                        }
                        else {
                            $.ajax({
                                url: PxPage.Routes.about_widget_refresh,
                                traditional: true,
                                data: { id: _static.defaults.courseInfoWidgetId },
                                type: 'GET',
                                success: function (result) {
                                    $(_static.sel.aboutWidget).html(result);
                                    PxInstructorConsoleWidget.BindEditButton();
                                    $(PxPage.switchboard).trigger("InstructorDashboard.CourseSaved", formData);

                                    CourseForm.RefreshCourseActivation();

                                    if (_static.defaults.reloadPage) {
                                        location.reload();
                                    }
                                }
                            });

                        }

                        if (!_static.defaults.isLmsEnabled && $('#LmsIdRequiredTrue').is(':checked')) {
                            PxPage.Toasts.Success("Your course is now set up to prompt students to enter their Campus LMS ID.");
                        }
                    },

                    error: function () {
                        $.unblockUI();
                        PxPage.Toasts.Error('An exeption occurred while updating settings.');
                    }
                });
            });

            $(_static.sel.submitlaunchpadForm).bind('click', function () {
                $.blockUI();

                $.ajax({
                    url: PxPage.Routes.InstructorConsole_UpdateLaunchPadSettings,
                    data: $(_static.sel.launchpadForm).serialize(),
                    type: 'POST',
                    success: function (data) {
                        $.unblockUI();

                        if (data.Error != undefined && eval(data.Error).length > 0) {
                            $('[id^="error"]').html('');

                            var errors = eval(data.Error);

                            for (var i = 0; i < errors.length; i++) {
                                $("#error" + errors[i].id).html(errors[i].messages.toString());
                            }

                            return;
                        }
                        else {
                            $(_static.sel.consoleWrapper).html(data);

                            $.ajax({
                                url: PxPage.Routes.about_widget_refresh,
                                traditional: true,
                                data: { id: _static.defaults.courseInfoWidgetId },
                                type: 'GET',
                                success: function (result) {
                                    $(_static.sel.aboutWidget).html(result);
                                }
                            });
                        }
                    },

                    error: function () {
                        $.unblockUI();
                        PxPage.Toasts.Error('An exeption occurred while updating settings.');
                    }
                });
            });

            $(_static.sel.addContact).bind('click', function () {
                _static.defaults.contactInfoCount++;

                var newContact = getContactHtml(_static.defaults.contactInfoCount);
                $(_static.sel.dynamicElementTarget).append(newContact);

                $(_static.sel.contactInfoType).bind('change', function () {
                    SetPlaceHolder();
                });

                SetPlaceHolder();
            });

            $(_static.sel.rdlink).bind('click', function () {
                $(_static.sel.syllabusblock).css('display', 'block');
                $(_static.sel.uploadblock).css('display', 'none');
            });

            $(_static.sel.rdupload).bind('click', function () {
                $(_static.sel.syllabusblock).css('display', 'none');
                $(_static.sel.uploadblock).css('display', 'block');
            });

            $(_static.sel.lmsWhatsThisShow).bind('click', function () {
                _static.fn.ShowLmsWhatsThis(true);
            });
            $(_static.sel.lmsWhatsThisHide).bind('click', function () {
                _static.fn.ShowLmsWhatsThis(false);
            });

            $(_static.sel.LmsIdRequiredFalse).bind('click', function () {
                _static.fn.ShowLmsIdRequired(false);
            });
            $(_static.sel.LmsIdRequiredTrue).bind('click', function () {
                _static.fn.ShowLmsIdRequired(true);
            });

            function ShowRequest(formData, jqForm, options) {
                if (formData[0].value == null || formData[0].value == undefined || formData[0].value == "") {
                    $(_static.sel.UploadResults).html('Please select a file!');

                    return false;
                }

                var queryString = $.param(formData);
                var message = queryString.replace("postedFile=", "Uploading file ");
                $(_static.sel.UploadResults).html(message);

                return true;
            }

            function SubmitSuccesful(result, statusText) {
                result = eval(result);

                $(_static.sel.UploadResults).html(result.resultMessage.UploadMessage);
                $(_static.sel.RefUrlFilePath).val(result.resultMessage.UploadPath);
                $(_static.sel.RefUrlFileName).val(result.resultMessage.UploadFileName);
                $(_static.sel.isUploadValid).val('true');
            }

            function AjaxError(responseText, statusText) {
                PxPage.Toasts.Error("An AJAX error occurred." + responseText);
            }

            function SetPlaceHolder() {
                $(_static.sel.contactInfoInput).each(function () {
                    var placeholderText = "";

                    switch ($(this).parents(_static.sel.contactInfoValue).find('option:selected').text()) {
                        case "Email":
                            placeholderText = "Enter email address";
                            break;
                        case "Phone":
                            placeholderText = "Enter phone number";
                            break;
                        case "Fax":
                            placeholderText = "Enter fax number";
                            break;
                        case "Other":
                            placeholderText = "Enter other contact information";
                            break;
                    }

                    $(this).attr(_static.sel.placeholder, placeholderText);
                });

                $(_static.sel.syllabusURL).attr(_static.sel.placeholder, "Enter syllabus URL");
                $(_static.sel.officeHours).attr(_static.sel.placeholder, "Enter office hours");

                $(_static.sel.inputPlaceholder).placeholder();
            }

            _static.defaults.isLmsEnabled = $('#LmsIdRequiredTrue').is(':checked');
        },

        RefreshCourseActivation: function () {
            var elem = $(_static.sel.courseActivationWidget).find(".homepageheader tr");
            if (elem === undefined || elem.length === undefined) {
                return;
            }

            $.ajax({
                url: PxPage.Routes.course_activation_widget_refresh,
                traditional: true,
                type: 'GET',
                success: function (response) {
                    $(elem).find(".course-title").html(response.Title);

                    if ($(elem).attr("data-dw-id") !== undefined)
                        $(elem).attr("data-dw-id", response.Id);
                    if ($(elem).attr("data-dw-course-section") !== undefined)
                        $(elem).attr("data-dw-course-section", response.SectionNumber);
                    if ($(elem).attr("data-dw-course-number") !== undefined)
                        $(elem).attr("data-dw-course-number", response.CourseNumber);
                    if ($(elem).attr("data-dw-academic-term") !== undefined)
                        $(elem).attr("data-dw-academic-term", response.AcademicTerm);
                    if ($(elem).attr("data-dw-timezone") !== undefined)
                        $(elem).attr("data-dw-timezone", response.CourseTimeZone);
                }
            });
        }
    };
}(jQuery);

function getContactHtml(num) {
    var result = '<div id="contact_' + num + '" class="contactInfoValue" ><select class="contactInfoType" name="contactInfoType_' + num + '" id="contactInfoType_' + num + '"><option>Email</option><option>Phone</option><option>Fax</option><option>Other</option></select>';
    result += '&nbsp;<input id="contactInfoValue_' + num + '" name="contactInfoValue_' + num + '" type="text" class="contactInfoInput" size="50"/>&nbsp;<a href="#" class="removeField" onclick=removeField("#contact_' + num + '")></a>'
    result += '<span id="errorcontactInfoValue_' + num + '" class="errorMessage"></span></div>';

    return result;
}

function removeField(field) {
    $(field).remove();
}
