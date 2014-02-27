(function ($) {

    var _static = {
        pluginName: "AssignmentSettings",
        defaults: {
            context: '#assignment-settings'
        },
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
                    $(".restricted").prop("checked", false);
                } else {
                    $(".restricted").show();
                    $(".spanRestricted").show();
                }

                if ($("#restrictedbydate").prop("checked")) {
                    $(".restricted-calendar").show();
                } else {
                    $(".restricted-calendar").hide();
                }
            },

            clearDateFields: function (event) {
                var context = $(event.target).closest(_static.defaults.context);
                var dt = new Date();
                context.find("#settingsAssignDueDate").val('');
                context.find("#settingsAssignTime").val('');
                context.find("#DueDate").val("");
                context.find('#assignment-calendar').DatePickerClear();
                context.find('.invaliddate').show();
                context.find('.invalidtime').show();
                context.find('#fsDateTime').hide();
                context.find('#liDueDate .placeholderWrap').removeClass('placeholder-changed');
                context.find('.btnSaveChanges').attr('disabled', 'disabled');
                context.find('.btnAssign').attr('disabled', 'disabled');
            },

            toggleRestricted: function () {
                $(this).siblings(".restricted").prop("checked", false);

                if ($(this).val() == "restrictedbydate") {
                    $(".restricted-calendar").show();
                    if ($(".isCalendarInitialized").val() != "true") {
                        //ContentWidget.InitAssign("contentcreate");
                        $(".isCalendarInitialized").val("true");
                    }
                } else {
                    $(".restricted-calendar").hide();
                }
            },

            SetupfancyTime: function () {

                $(document).off('focusout', '#settingsAssignDueDate').on("focusout", '#settingsAssignDueDate', function () {
                    _static.fn.parseDateTime();
                    _static.fn.setDueDate();
                });

                $(document).off('focusout', '#settingsAssignTime').on("focusout", '#settingsAssignTime', function () {
                    _static.fn.parseDateTime();
                    _static.fn.setDueDate();
                });

                $(document).off('change', '#settingsAssignTime').on("change", '#settingsAssignTime', function () {
                    _static.fn.parseDateTime();
                    _static.fn.setDueDate();
                });

                if ($("#settingsAssignTime").length > 0) {
                    $("#settingsAssignTime").ptTimeSelect();


                    if ($('#facePlateAssignDueDate').parent().hasClass('placeholder-focus') && !$('#facePlateAssignDueDate').parent().hasClass('placeholder-changed')) {
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

                if (dateval == null || dateval === undefined || dateval.trim() === "") {
                    $("#clearDateField").click();
                    return;
                }

                if ($.trim($('#settingsAssignTime').val()).length < 1) {
                    $('#settingsAssignTime').val('11:59 PM');
                } else {
                    var time = _static.fn.getTime(dateval + " " + $.trim($('#settingsAssignTime').val()));
                    $('#settingsAssignTime').val(time);
                }

                var localDateTime = dateval + " " + $.trim($('#settingsAssignTime').val());

                if ($("#dueHour").length > 0) {
                    $("#dueHour").val(ContentWidget.getHours(localDateTime));
                }

                if ($("#dueMinute").length > 0) {
                    $("#dueMinute").val(ContentWidget.getMinutes(localDateTime));
                }

                if ($("#dueAmpm").length > 0) {
                    $("#dueAmpm").val(ContentWidget.getAMorPM(localDateTime));
                }

                if (dateval != '') {
                    $('#cal-box #assignment-calendar').DatePickerSetDate(dateval, true);
                    ContentWidget.AssignmentDateSelected(dateval);
                }
            },

            getDate: function (d) {
                var input = d;
                if (/^\d+$/.test(input)) {
                    input = val.toNumber();
                }
                var text, date = Date.parse(input);
                if (date != null) {
                    text = date.format('mm/dd/yyyy');
                }
                return text;
            },

            setDueDate: function () {
                if ($("#settingsAssignDueDate").length) {
                    var dueDate = $("#settingsAssignDueDate").val();
                    var dueTime = $('#settingsAssignTime').val();
                    var fulldatetime = dueDate + " " + dueTime;
                    fulldatetime = jQuery.trim(fulldatetime);
                    
                    if (fulldatetime != "") {
                        var hours = _static.fn.getHours(fulldatetime);
                        $("#dueHour").closest().val(_static.fn.getHours(fulldatetime) - 1);
                        $("#dueMinute").closest().val(_static.fn.getMinutes(fulldatetime) - 1);
                        $("#dueAmpm").closest().val(_static.fn.getAMorPM(fulldatetime));
                        var date = Date.parse(fulldatetime);
                        var year = 1;
                        if (date != null && !isNaN(date)) {
                            year = Date.parse(fulldatetime).getFullYear();
                            $("#dueYear").closest().val(year);

                            ContentWidget.AssignmentDateSelected(dueDate);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            },

            toggleSection: function () {

                var checked = $(this).parents(".sectionparent").find(".sectionCheckBox").is(':checked');
                if (checked == true) {
                    $(this).siblings(".widgetExpand").click();
                } else {

                    if ($(this).siblings(".widgetCollapse").css("display") == "none") {
                        $(this).siblings(".widgetExpand").click();
                    } else {
                        $(this).siblings(".widgetCollapse").click();
                    }
                }
            },

            sectionCheckBox: function () {
                if ($(this).is(':checked')) {
                    $(this).parents(".sectionparent").find(".widgetExpand").click();
                }
            },

            getTime: function (d) {
                var input = d;
                if (/^\d+$/.test(input)) {
                    input = input.toNumber();
                }
                var text, dt = new Date.parse(input);
                if (dt != null) {
                    try{
                        text = dt.format('hh:MM TT');
                    } catch (e) { }
                }
                return text;
            },

            getAMorPM: function (d) {
                var input = d;
                if (/^\d+$/.test(input)) {
                    input = input.toNumber();
                }
                var text, dt = Date.parse(input);
                if (dt == null) {
                    text = 'PM';
                } else {
                    text = dt.format('TT');
                }
                return text;
            },

            getHours: function (d) {
                var input = d;
                if (/^\d+$/.test(input)) {
                    input = input.toNumber();
                }
                var text, dt = Date.parse(input);
                if (dt == null) {
                    text = '11';
                } else {
                    text = dt.format('hh');
                }
                return text;
            },

            getMinutes: function (d) {
                var input = d;
                if (/^\d+$/.test(input)) {
                    input = input.toNumber();
                }
                var text, dt = Date.parse(input);
                if (dt == null) {
                    text = '59';
                } else {
                    text = dt.format('MM');
                }
                return text;
            },

            BindControls: function () {
                $(document).off('click', '#btnAddCancel').on('click', '#btnAddCancel', function () {
                    PxSettingsTab.CloseDialog('btnAddCancel');
                    PxSettingsTab.UpdateQuizSettings("EntireClass");
                    $('#errorMessage').text("");
                });

                $(document).off('click', '#btnTemplateCancel').on('click', '#btnTemplateCancel', function () {
                    PxSettingsTab.CloseDialog('cancel-template');
                });
                $(document).off('click', '#btnCancel').on('click', '#btnCancel', function () {
                    $('#fne-window').unblock();
                });
            },

            saveContentSettings: function (event) {
                _static.fn.setDueDate();
                PxPage.Loading();

                var setting = $("#contentwrapper #assessment-settings-form");
                if (setting.length > 0) {
                    var data = setting.serialize();
                    $.post(setting.attr("action"), data, function (response) {
                        if (response.Result == "SUCCESS") {
                            $("#form0").submit();
                            PxPage.Loaded();
                        }
                        PxPage.Toasts.Success(response.ReturnMessage);
                    });
                } else {
                    $("#form0").submit();

                    var hiddenFromStudent = $('#selgradebookweights option:selected');
                    jQuery(PxPage.switchboard).trigger('setvisibility', [itemId, hiddenFromStudent]);
                }
            },

            submitAssignment: function (itemId, onSuccess, contentWrapper, area) {
                if ($("#settingsAssignDueDate").length) {
                    var dueDate = Date.parse($("#settingsAssignDueDate").val());
                    var dueTime = $('#settingsAssignTime').val();
                    if (isNaN(dueDate)
                        || !dueTime.match(/^(?:0?\d|1[012]):[0-5]\d [aApP][mM]/)) {
                        return;
                    }
                }

                if ($("#settingsAssignDueDate").length) {
                    var dueDate = Date.parse($("#settingsAssignDueDate").val());
                    var dueTime = $('#settingsAssignTime').val();
                    var fulldatetime = dateFormat(dueDate, 'mm/dd/yyyy') + " " + dueTime;

                    ContentWidget.IsAssignDateValid(jQuery.trim(fulldatetime), function () { ContentWidget.ContentAssigned('assign', itemId, onSuccess, contentWrapper, area); });
                }
                else {
                    ContentWidget.ContentAssigned('assign', itemId, onSuccess, contentWrapper, area);
                }
            }
        }
    };

    window.AssignmentSettings = {
        //Initializes a page the uses the AssignmentCalendar control.  Context is needed in case two AssignmentCalendar controls
        //are being displayed at the same time. (i.e Eportfolio Reflection Creation dialog and Assignment tab.)
        init: function (contextClass) {

            var assignmentContext = '';
            if (contextClass == null) {
                assignmentContext = _static.defaults.context;
            } else {
                assignmentContext = '.' + contextClass;
            }

            PxPage.Loading();
            $(document).off("click", assignmentContext + " .open").on("click", assignmentContext + " .open", _static.fn.open);
            $(document).off("click", assignmentContext + " .close").on("click", assignmentContext + " .close", _static.fn.collape);
            $(document).off("click", assignmentContext + " .visibility").on("click", assignmentContext + " .visibility", _static.fn.toggleVisibility);
            $(assignmentContext + " .restricted").hide();
            $(assignmentContext + " .spanRestricted").hide();

            $(document).off("click", assignmentContext + " #savecontentsettings").on("click", assignmentContext + " #savecontentsettings", _static.fn.saveContentSettings);
            $(assignmentContext + " #assessment-settings-save").hide();

            $(document).off("click", assignmentContext + " #clearDateField").on("click", assignmentContext + " #clearDateField", _static.fn.clearDateFields);

            $(document).off("click", assignmentContext + " #restrictedbydate").on("click", assignmentContext + " #restrictedbydate", _static.fn.toggleRestricted);

            $(document).off("click", assignmentContext + " .sectionCheckBox").on("click", assignmentContext + " .sectionCheckBox", _static.fn.sectionCheckBox);
            // Place holder jquery call for due date 
            $(assignmentContext + " #settingsAssignDueDate").placeholder();

            var section = $('ul li.sectionparent');
            $(assignmentContext + ' ul li.sectionparent').each(function () {
                var node = $(this);
                var state = node.attr("default-state");
                if (state == "open") {
                    node.find(".sectionbody").show();
                    node.find(".open").hide();
                    node.find(".close").show();
                } else if (state == "hide") {
                    node.hide();
                } else {
                    node.find(".sectionbody").hide();
                    node.find(".open").show();
                    node.find(".close").hide();
                }
            });

            if ($(assignmentContext + " #restrictedaccess").prop("checked")) {
                $(".restricted").show();
                $(".spanRestricted").show();

                if ($("#restrictedbydate").prop("checked")) {
                    $(".spanRestricted").show();
                    $(".restricted-calendar").show();
                    //ContentWidget.InitAssign("contentcreate");
                    $(".isCalendarInitialized").val("true");
                }
                else {
                    $(".restricted-calendar").hide();
                }
            }

            var itemId = $(assignmentContext + " #AssignmentTabItemId").val();

            //assign button
            $(document).off('click', assignmentContext + ' .btnAssign').on("click", assignmentContext + ' .btnAssign', function () {

                var areaToBlock = "";
                if ($(".product-type-lms-faceplate").length + $(".product-type-faceplate").length) {
                    areaToBlock = "content";
                }
                var contentWrapper = $(this).closest('.contentwrapper');

                var onSuccess = function () {
                };

                //TODO: Remove this once we figure out how to save an assignment without a due date. 
                _static.fn.submitAssignment(itemId, onSuccess, contentWrapper, areaToBlock);
            });

            //unassign button
            $(document).off('click', assignmentContext + ' .btnUnassign').on("click", assignmentContext + ' .btnUnassign', function () {
                var tag = $("<div id='unassignedItemAlert' style='display:none'><span>Do you want to unassign this item?</span></div>");

                var onSuccess = function () {
                    tag.dialog("close");

                };
                PxPage.Loading();
                var contentWrapper = $(this).closest('.contentwrapper');

                if ($(".product-type-lms-faceplate").length + $(".product-type-faceplate").length) {
                    ContentWidget.ContentAssigned('unassign', itemId, false, contentWrapper);
                } else {

                    tag.dialog({
                        width: 300,
                        modal: true,
                        draggable: false,
                        closeOnEscape: false,
                        resizable: false,
                        buttons: { "Yes": function () {
                            ContentWidget.ContentAssigned('unassign', itemId, false, contentWrapper);
                            $(this).dialog("close");
                        },
                            "No": function () {
                                $(this).dialog("close");
                            }
                        },
                        close: function () {
                            $(this).dialog('destroy').empty().detach();
                            PxPage.Loaded();
                        }
                    }).dialog('open');
                    $(".ui-dialog-titlebar").hide();
                }
            });

            //save changes button
            $(document).off('click', assignmentContext + ' .btnSaveChanges').on("click", assignmentContext + ' .btnSaveChanges', function () {

                var onSuccess = function () {
                    //alert("Your changes have been saved.");
                    PxPage.Loaded();
                    PxPage.Loaded("content");
                }

                var area = "";
                if ($(".product-type-lms-faceplate").length + $(".product-type-faceplate").length) {
                    area = "content";
                }
                var contentWrapper = $(this).closest('.contentwrapper');

                _static.fn.submitAssignment(itemId, onSuccess, contentWrapper, area);
            });

            _static.fn.BindControls();

            $(assignmentContext + ' #assessment-settings-save').hide();
            PxPage.Loaded();
            _static.fn.SetupfancyTime();
            $('#fne-window').removeClass('require-confirm');
        }
    };

    //        $.fn.AssignmentSettings = function (method) {
    //            if (api[method]) {
    //                return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
    //            } else if (typeof method === "object" || !method) {
    //                return api.init.apply(this, arguments);
    //            } else {
    //                $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
    //            }
    //        };
})(jQuery);