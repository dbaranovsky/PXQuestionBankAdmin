//var isInit = false;
var PxAssignmentWidget = function ($) {
    return {
        Init: function (isStart) {
            $('.collapseIcon').css('cursor', 'pointer');

            if (isStart) {
                $('.widgetItem[templateid="PX_AssignmentWidget"]').find('.widgetHeaderText').each(function () {
                    if ($(this).find('.openCalendarMonth').length == 0) {
                        $(this).append('<a href="#state/calendar/month"><div style="float:right" class="openCalendarMonth"></div></a>');
                    }
                })
            }

            $('.collapseIcon, .tblAssignmentCollapsed').click(function () {
                var group = $(this).closest('.group');
                var collapseIcon = $(group).find('.collapseIcon');
                var col = $(collapseIcon).attr('col');

                if (isStart) {
                    if (col == 'yes') {
                        $(group).next().find('.tblAssignmentFull').show();
                        $(group).next().find('.tblAssignmentCollapsed').hide();
                        $(group).find('.group-title').removeClass('Collapse');
                        $(group).find('.group-title').addClass('noCollapse');
                        collapseIcon.attr('col', 'no');
                    }
                    else {
                        $(group).next().find('.tblAssignmentFull').hide();
                        $(group).next().find('.tblAssignmentCollapsed').show();
                        $(group).find('.group-title').removeClass('noCollapse');
                        $(group).find('.group-title').addClass('Collapse');
                        collapseIcon.attr('col', 'yes');
                    }
                }
                else {
                    if (col == 'yes') {
                        $(group).find('.tblAssignmentFull').show();
                        $(group).find('.tblAssignmentCollapsed').hide();
                        $(collapseIcon).attr('col', 'no');
                    }
                    else {
                        $(group).find('.tblAssignmentFull').hide();
                        $(group).find('.tblAssignmentCollapsed').show();
                        $(collapseIcon).attr('col', 'yes');
                    }
                }
            });

            //If the "Upcoming Assignments" is not a widget and it is not on home page, hide the "View All" link.
            //This will hide the "View All" link in the Assignment Center
            var widgetBodyID = $('.assignment-widget').closest('.widgetBody').attr('id');
            if (widgetBodyID == '' || widgetBodyID == null || widgetBodyID == 'undefined') {
                $('.assignmentViewAll').hide();
            }

        },

        InitCalendar: function () {
            var currentDate = $('#schedule #currentDate').val();
            var today = [], thisWeek = [], nextWeek = [], totalArray = [], cssClasses = [];

            if ($('#schedule #today').val() != '') {
                today = $('#schedule #today').val().split(',');
                for (var i = 0; i < today.length; i++) {
                    var tempDate = { date: today[i], cssClass: 'schedule-today' };
                    totalArray.push(today[i]);
                    cssClasses.push(tempDate);
                }
            }

            if ($('#schedule #thisWeek').val() != '') {
                thisWeek = $('#schedule #thisWeek').val().split(',');
                for (var i = 0; i < thisWeek.length; i++) {
                    var tempDate = { date: thisWeek[i], cssClass: 'schedule-this-week' };
                    totalArray.push(thisWeek[i]);
                    cssClasses.push(tempDate);
                }
            }

            if ($('#schedule #nextWeek').val() != '') {
                nextWeek = $('#schedule #nextWeek').val().split(',');
                for (var i = 0; i < nextWeek.length; i++) {
                    var tempDate = { date: nextWeek[i], cssClass: 'schedule-next-week' };
                    totalArray.push(nextWeek[i]);
                    cssClasses.push(tempDate);
                }
            }

            $('#assignWidgetCalendar').DatePicker({
                flat: true,
                date: '',
                current: currentDate,
                calendars: 1,
                mode: 'multiple',
                starts: 0,
                onRender: function (date) {
                    return {
                        disabled: true
                    };
                }
            }).DatePickerSetDate(totalArray, true, cssClasses);
        },

        BindEditModalControls: function () {

            $(PxPage.switchboard).unbind("validateModalDialog", PxAssignmentWidget.ValidateModalDialog);
            $(PxPage.switchboard).bind("validateModalDialog", PxAssignmentWidget.ValidateModalDialog);
            
            $(document).off('keydown', '.AssignmentName').on('keydown', '.AssignmentName', function () {

                var assignmentID = $(this).closest('.AssignmentContainer').find('.AssignmentID').val();
                var removedAssignments = $(this).closest('.customEditWidget').find('.removedAssignments');
                if (assignmentID != '') {
                    var assignmentList = $(removedAssignments).val() + ',' + assignmentID;
                    $(removedAssignments).val(assignmentList);
                }
                $(this).closest('.AssignmentContainer').find('.AssignmentID').val('');
                var assignmentName = $(this);
                PxAssignmentWidget.Validation(assignmentName);
            });

            $(document).off('click', '.AssignmentName').on('click', '.AssignmentName', function () {
                var bool = $(this).hasClass('watermarkAssignment');
                if (bool) {
                    $(this).val('');
                    $(this).removeClass('watermarkAssignment');
                }
            });

            $(document).off('focus', '.AssignmentName').on('focus', '.AssignmentName', function () {
                $(this).autocomplete({

                    source: function (request, response) {
                        $.ajax({
                            url: PxPage.Routes.search_assignments,
                            type: 'POST',
                            dataType: 'json',
                            data: { searchText: request.term },
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        value: item.Title,
                                        id: item.Id
                                    }
                                }))
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                alert(textStatus);
                            }
                        });
                    }

                , select: function (event, ui) {
                    $(this).val(ui.item.label);
                    $(this).closest('.AssignmentContainer').find('.AssignmentID').val(ui.item.id);
                    var assignmentName = $(this);
                    PxAssignmentWidget.Validation(assignmentName);
                    return false;
                }

                , change: function (event, ui) {
                }
                });
            });

            $('.addAssignment').click(function () {
                var assignmentContainer = [
                '<div class="AssignmentContainer">',
                '<div style="float:left;"><input type="text" class="AssignmentName watermarkAssignment" value="Start typing an existing assignment name..."/></div>',
                '<div class="errorAssignment">*</div>',
                '<a class="removeAssignment" title="Remove from important assignments"></a>',
                '<input type="hidden" class="AssignmentID" />',
                '</div>'].join('\n');
                $(this).siblings('.AssignmentGroup').append(assignmentContainer);
            });

            $(document).off('click', '.removeAssignment').on('click', '.removeAssignment', function () {
                var assignmentID = $(this).closest('.AssignmentContainer').find('.AssignmentID').val();
                var removedAssignments = $(this).closest('.customEditWidget').find('.removedAssignments');
                if (assignmentID != '') {
                    var assignmentList = $(removedAssignments).val() + ',' + assignmentID;
                    $(removedAssignments).val(assignmentList);
                }
                $(this).closest('.AssignmentContainer').remove();
                PxAssignmentWidget.ValidateForm();
            });

            $('.btnSaveAssignment').click(function () {

                if ($('.errorSummary').is(':visible')) {
                    alert('Please choose correct assignments');
                    return false;
                }

                var importantAssignments = [];
                $('.AssignmentContainer').each(function (index) {
                    importantAssignments.push($(this).find('.AssignmentID').val());
                });
                var removedAssignments = $(this).closest('.customEditWidget').find('.removedAssignments').val().split(',');
                removedAssignments.reverse().pop();

                //ajax call to mark the Assignments as important
                var bodyPostContent = $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.save_important_assignments,
                    dataType: "json",
                    traditional: true,
                    data: { importantAssignments: importantAssignments, removedAssignments: removedAssignments },
                    success: function (response) {
                        alert(response.message);
                    },
                    error: function (req, status, error) {
                        alert('ERROR_SAVE_IMPORTANT_ASSIGNMENTS');
                    }
                });

            });




        },

        Validation: function (assignmentName) {
            var itemid = $(assignmentName).closest('.AssignmentContainer').find('.AssignmentID').val();

            if (itemid == null || itemid == '') {
                $(assignmentName).closest('.AssignmentContainer').find('.errorAssignment').show();
            }
            else {
                $(assignmentName).closest('.AssignmentContainer').find('.errorAssignment').hide();
            }
            //set the error summary
            $('.errorSummary').hide();
            $('.AssignmentContainer').each(function () {
                var assignmentID = $(this).find('.AssignmentID').val();
                if (assignmentID == null || assignmentID == '') {
                    $('.errorSummary').show();
                    return false;
                }
            });
        },

        ValidateForm: function () {
            $('.ui-dialog').find('.errorSummary').hide();
            $('.ui-dialog').find('.AssignmentContainer').each(function () {
                var assignmentID = $(this).find('.AssignmentID').val();
                if (assignmentID == null || assignmentID == '') {
                    $('.ui-dialog').find('.errorSummary').show();
                    $(this).find('.AssignmentID').show();
                }
                else {
                    $(this).find('.AssignmentID').hide();
                }
            });
        },

        ValidateModalDialog: function () {

            //don't validate the form if the are no assignments to validate (just mark the valid status to false)
            var assignmentcount = $('.ui-dialog').find('.assignmentcount').val();
            if (assignmentcount == '0') {
                $('.ui-dialog').find('.isFormValid').val('false');
                return;
            }

            //validate the form
            PxAssignmentWidget.ValidateForm();

            if ($('.ui-dialog').find('.errorSummary').is(':visible')) {
                alert('Please choose correct assignments');
                $('.ui-dialog').find('.isFormValid').val('false');
            }
            else {
                $('.ui-dialog').find('.isFormValid').val('true');
                var importantAssignmentList = [];
                $('.ui-dialog').find('.AssignmentContainer').each(function (index) {
                    importantAssignmentList.push($(this).find('.AssignmentID').val());
                });
                var importantAssignments = importantAssignmentList.join(',');
                $('.ui-dialog').find('.importantAssignments').val($.trim(importantAssignments));

                var removedAssignmentList = $('.ui-dialog').find('.removedAssignments').val().split(',');
                removedAssignmentList.reverse().pop();
                var removedAssignments = removedAssignmentList.join(',');
                $('.ui-dialog').find('.removedAssignments').val($.trim(removedAssignments));
            }
        }
    };
} (jQuery);