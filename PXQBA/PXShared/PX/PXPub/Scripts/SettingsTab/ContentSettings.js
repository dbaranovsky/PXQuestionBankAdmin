(function ($) {

    var _static = {
        pluginName: "ContentSettings",
        defaults: {},
        settings: {},
        fn: {
            collape: function (event) {
                var checked = $(this).parents(".sectionparent").find(".sectionCheckBox").is(':checked');
                if (checked == false) {
                    $(this).parents(".sectionparent").find(".sectionbody").hide();
                    $(this).siblings(".open").show();
                    $(this).hide();
                }
            },

            open: function (event) {
                $(this).parents(".sectionparent").find(".sectionbody").show();
                $(this).siblings(".close").show();
                $(this).hide();
            },

            toggleVisibility: function () {
                $(this).siblings(".visibility").prop("checked", false);
                if ($(this).val() == "hidefromstudent") {
                    $(".restricted").hide();
                    $(".spanRestricted").hide();
                    $(".restricted-calendar").hide();
                    $(".restricted").prop("checked",false);
                } else {
                    $(".restricted").show();
                    $(".spanRestricted").show();
                    $(".restrictedbydate").show();
                }

                if ($("#restrictedbydate").prop("checked")) {
                    $(".restricted-calendar").show();
                } else {
                    $(".restricted-calendar").hide();
                }
            },

            clearDateFields: function () {
                $("#settingsAssignDueDate").val('');
                $("#settingsAssignTime").val('');
                $('#liDueDate .placeholderWrap').removeClass('placeholder-changed');
                $('.btnSaveChanges').removeAttr('disabled');
                $('.btnSaveChanges').css('background-color', '#0074BB');
                return false;
            },

            toggleRestricted: function () {
                if ($(this).attr('id') == 'restrictedbydate') {
                    $(".restricted-calendar").show();
                    $(".li-cal-box").show();

                    if ($(".isCalendarInitialized").val() != "true") {
                        $(".isCalendarInitialized").val("true");
                    }
                }
                else {
                    $(".restricted-calendar").hide();
                    $(".li-cal-box").hide();
                }
            },

            SetupfancyTime: function () {
                if (!($(".settingsTabGroupLink").length > 0 && $(".settingsTabGroupLink").closest('li').hasClass('active'))) {
                    $('#settingsAssignDueDate').focusout(function () {
                        _static.fn.parseDateTime();
                        _static.fn.setDueDate();
                    });

                    $('#settingsAssignTime').focusout(function () {
                        _static.fn.parseDateTime();
                        _static.fn.setDueDate();
                    });

                    if ($("#settingsAssignTime").length > 0) {
                        $("#settingsAssignTime").ptTimeSelect();
                        _static.fn.parseDateTime(true);
                        _static.fn.setDueDate();
                    }
                }
            },

            parseDateTime: function (isInit) {

                if (isInit == true) {
                    if ($.trim($('#settingsAssignDueDate').val()) == "") {
                        return;
                    }
                }

                $("#fsDateTime").show();

                var dateval = $.trim(_static.fn.getDate($('#settingsAssignDueDate').val()));
                $('#settingsAssignDueDate').val(dateval);
                if (dateval == 'Invalid date.') {
                    $('#cal-box #assignment-calendar').DatePickerSetDate('');
                    ContentWidget.AssignmentDateSelected('');
                    return;
                }

                if ($.trim($('#settingsAssignTime').val()).length < 1) {
                    $('#settingsAssignTime').val('11:59 PM');
                } else {
                    var time = _static.fn.getTime(dateval + " " + $.trim($('#settingsAssignTime').val()));
                    $('#settingsAssignTime').val(time);
                }


                $('#cal-box #assignment-calendar').DatePickerSetDate(dateval, true);
                ContentWidget.AssignmentDateSelected(dateval);
            },

            getTime: function (d) {
                var input = d;
                if (/^\d+$/.test(input)) {
                    input = input.toNumber();
                }
                var text, dt = Date.parse(input);
                if (dt == null) {
                    text = 'Invalid time.';
                } else {
                    text = dt.format('hh:MM TT');
                }
                return text;
            },

            getDate: function (d) {
                var input = d;
                if (/^\d+$/.test(input)) {
                    input = val.toNumber();
                }
                var text, date = Date.parse(input);
                if (date == null) {
                    // text = 'Invalid date.';
                    $(".invaliddate").show();
                } else {
                    text = date.format('mm/dd/yyyy');
                    $(".invaliddate").hide();
                }
                return text;
            },

            setDueDate: function () {
                if ($("#settingsAssignDueDate").length) {
                    var dueDate = $("#settingsAssignDueDate").val();
                    var dueTime = $('#settingsAssignTime').val();

                    var fulldatetime = dueDate + " " + dueTime;
                    var utcDate = dateFormat.dateConvertToUtc(dueDate, dueTime);
                    if (utcDate != null) {
                        dueDate = utcDate.format("mm/dd/yyyy");
                        fulldatetime = utcDate.format("mm/dd/yyyy HH:MM:ss");
                    }

                    $("#DueDate").val(fulldatetime);
                }
            },

            ValidateVisibility: function () {
                if ($("#restrictedbydate").is(':checked') == true) {
                    var today = new Date();
                    var currentTime = new Date().getTime();
                    var settingsAssignDueDate = $.trim($("#settingsAssignDueDate").val());
                    var settingsAssignDueTime = $.trim($("#settingsAssignTime").val());
                    var date = Date.parse(settingsAssignDueDate + " " + settingsAssignDueTime);

                    if (date == null || settingsAssignDueDate == "") {
                        PxPage.Toasts.Error("Invalid restriction date");
                        return false;
                    }

                    if (date < today) {
                        PxPage.Toasts.Error("Restriction date can not be prior to today's date");
                        return false;
                    }
                }

                return true;
            },

            ValidateLearningCurveSettings: function () {
                var targetScore = $('#LearningCurveTargetScore');
                if (targetScore != null && targetScore.length > 0) {
                    var autoCalculate = $('#AutoTargetScore');
                    if (autoCalculate[0] && !autoCalculate[0].checked) {
                        var res = parseInt($('#LearningCurveTargetScore').val());
                        if (isNaN(res)) {
                            PxPage.Toasts.Error("Please enter a valid score.");
                            return false;
                        }
                        if ($.trim(res) == "") {
                            PxPage.Toasts.Warning("Target Score can not be empty");
                        }
                    }
                }
                return true;
            },

            toggleSection: function () {

                var checked = $(this).parents(".sectionparent").find(".sectionCheckBox").is(':checked');
                if (checked == true) {
                    $(this).siblings(".widgetExpand").click();
                }
                else {

                    if ($(this).siblings(".widgetCollapse").css("display") == "none") {
                        $(this).siblings(".widgetExpand").click();
                    }
                    else {
                        $(this).siblings(".widgetCollapse").click();
                    }
                }
            },

            saveContentSettings: function (event) {
                if (_static.fn.ValidateVisibility() == false) {
                    return false;
                }
                
                var convertedDateTime = dateFormat.dateConvertToUtc($('#settingsAssignDueDate').val(), $('#settingsAssignTime').val());
                $('#DueDate').val(convertedDateTime.toUTCString().replace('UTC', 'GMT'));

                $("#savecontentsettings").hide();
                PxPage.Loading();

                var setting = $("#contentwrapper #assessment-settings-form");
                if (setting.length > 0) {
                    var data = setting.serialize();
                    $.post(setting.attr("action"), data, function (response) {
                        if (response.Result == "SUCCESS") {
                            PxPage.Loaded();
                            $('#isShowOnSuccessMessage').val('false');
                            $("#form0").submit();
                        }
                        PxPage.Toasts.Success(response.ReturnMessage);
                    });
                } else {
                    $("#form0").submit();
                    PxPage.Loaded();
                }

                $("#savecontentsettings").show();

                var contentItemId = $(".item-id").val();
                var isHiddenFromStudent = $("#hidefromstudent").is(':checked');
                var restrictedbydate = $("#restrictedbydate").is(':checked');                

                var data = {
                    id: contentItemId,
                    hiddenFromStudent: isHiddenFromStudent,
                    restrictedbydate: restrictedbydate                    
                };
                $(PxPage.switchboard).bind("contentSettingsSaved", function () {
                    $(PxPage.switchboard).trigger("settingsUpdate", [data]);
                    $(PxPage.switchboard).unbind("contentSettingsSaved");
                });

            }
        }
    };

    api = {
        init: function () {
            var convertedDateTime = dateFormat.dateConvertFromCourseTimeZone($('#settingsAssignDueDate').val() + ' ' + $('#settingsAssignTime').val());
            $('#settingsAssignDueDate').val(dateFormat(convertedDateTime, 'mm/dd/yyyy'));
            $('#settingsAssignTime').val(dateFormat(convertedDateTime, 'hh:MM TT'));

            $(document).off("click", ".open").on("click", ".open", _static.fn.open);
            $(document).off("click", ".close").on("click", ".close", _static.fn.collape);

            $(document).off("click", "#savecontentsettings").on("click", "#savecontentsettings", _static.fn.saveContentSettings);

            $(document).off("click", "#clearDateField").on("click", "#clearDateField", _static.fn.clearDateFields);

            $(document).off("click", "#visibleforstudent").on("click", "#visibleforstudent", _static.fn.toggleRestricted);
            $(document).off("click", "#hidefromstudent").on("click", "#hidefromstudent", _static.fn.toggleRestricted);
            $(document).off("click", "#restrictedbydate").on("click", "#restrictedbydate", _static.fn.toggleRestricted);

            $(document).off("click", ".sectiontitle").on("click", ".sectiontitle", _static.fn.toggleSection);

            $("#assessment-settings-save").hide();

            var section = $('ul li.sectionparent');
            $('ul li.sectionparent').each(function () {
                var node = $(this);
                var state = node.attr("default-state");
                if (state == "open") {
                    node.find(".sectionbody").show();
                    node.find(".open").hide();
                    node.find(".close").show();
                }
                else if (state == "hide") {
                    node.hide();
                }
                else {
                    node.find(".sectionbody").hide();
                    node.find(".open").show();
                    node.find(".close").hide();
                }
            });

            if ($("#restrictedbydate").is(":checked")) {
                $(".restricted-calendar").show();
                $(".restricted-calendar .li-cal-box").show();
                //ContentWidget.InitAssign("contentcreate");
                $(".isCalendarInitialized").val("true")
            } else {
                $(".restricted-calendar").hide();
                $(".restricted-calendar .li-cal-box").hide();
            }

            if ($("#restrictedaccess").prop("checked")) {
                $(".restricted").show();
                $(".spanRestricted").show();

                if ($("#restrictedbydate").prop("checked")) {
                    $(".restricted-calendar").show();
                    $(".isCalendarInitialized").val("true");
                } else {
                    $(".restricted-calendar").hide();
                }
            }

            PxPage.Loaded();
            _static.fn.SetupfancyTime();
            $(".calendar_toggle").unbind('click').click("click", function () {
                $(".li-cal-box").toggle();
            });

            $('#fne-window').removeClass('require-confirm');
            //$(".calendar_toggle").click();
        }
    },

    $.fn.ContentSettings = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };
})(jQuery);