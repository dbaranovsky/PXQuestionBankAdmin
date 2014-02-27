// Defines a singleton object that coordinates all client-side behavior of the AssignmentCenter page.
var PxActivitiesWidget = function ($) {
    // static data
    var _static = {
        pluginName: "ActivitiesWidget",
        dataKey: "ActivitiesWidget",
        bindSuffix: ".ActivitiesWidget",
        dataAttrPrefix: "data-aw-",
        defaults: {},
        settings: {},
        modes: {},
        // commonly used CSS classes
        css: {},
        cached: {
            courseTitle: '',
        },
        // commonly used jQuery selectors
        sel: {
            reportLink: ".reportLink",
            assignLink: ".assignLink",
            activityNameLink: ".activityNameLink",
            btnActivateCourse: '.course-directions-widget .activation-link',
            unassignButtonLC: ".unassignButtonLC",
            sortCheckBox: "#sortCheckbox",
            AssignmentWidget: ".widgetItem PX_ActivitiesWidget",
            ClassActiviyReport: ".Class-Activity-Report",
            StudentActivityReport: ".Student-Activity-Report",
            learningCurveItems: '.activitiesWidgetBody .itemRow'
        },
        cssclass : {
            btnActivateCourse: 'btnActivate',
            btnSubmitActiviteCourse: 'activate-submit',
            btnCloseFne: 'fne-unblock-action',
            btnUnassignLC: 'unassignButtonLC',
            checkBoxSort: 'sortCheckbox',
            linkActivateCourse: 'activation-link',
            linkActivityName: 'activityNameLink',
            linkAssign: 'assignLink',
            linkClassActiviyReport: 'Class-Activity-Report',
            linkStudentActivityReport: 'Student-Activity-Report',
            linkReport: 'reportLink',
            linkCreateCourse: 'createCourseLink'
        },

        fn: {
            toHomePage: function() {
                PxPage.UnBlockAction();
            },
            onHashInit: function(newHash) {
                if (newHash) {
                    _static.fn.onHashChange(newHash);
                } else {
                    window.location.hash = '#state';
                }
            },
            onHashChange: function(newHash, oldHash) {
                PxPage.log('new hash: ' + newHash + ', old hash: ' + oldHash);
                //Check hash based on cases
                switch(newHash) {
                    case '':
                    case 'report':
                        return true;
                    case 'state':
                        _static.fn.toHomePage();
                        return true;
                    default:
                        break;
                }
                
                //Check hash based on regular expression
                var activityRegex = /activity_(.*)/;
                var reportRegex = /report_(.*)/;
                var itemId;
                if (newHash.indexOf('activity_') !== -1) {
                    newHash.search(activityRegex);
                    itemId = RegExp.$1;
                    _static.fn.loadActivitiesWidgetContent(itemId, 'activity');
                    
                } else if (newHash.indexOf('report_') !== -1) {
                    newHash.search(reportRegex);
                    itemId = RegExp.$1;
                    _static.fn.loadActivitiesWidgetContent(itemId, 'report');
                }
                return true;

            },
            setSampleReportLink: function () {
                var studentReportLink = !!$('.Student-Activity-Report').length? $('.Student-Activity-Report') : $($('.course-directions-widget .bottom a')[1]);
                var classReportLink = !!$('.Class-Activity-Report').length ? $('.Class-Activity-Report') : $($('.course-directions-widget .bottom a')[0]);;
                
                classReportLink.removeClass('Class-Activity-Report').addClass('Class-Activity-Report').removeClass('fne-link').addClass('fne-link');
                studentReportLink.removeClass('Student-Activity-Report').addClass('Student-Activity-Report').removeClass('fne-link').addClass('fne-link');
                
                if (!classReportLink.length || !studentReportLink.length)
                    return;
                var lcReportLinks = $('.activitiesWidgetBody.instructor .activityNameLink ');
                if (!lcReportLinks.length)
                    return;
                var lcReportLink = lcReportLinks.first().attr('url').split('&')[0];
                if (!lcReportLink || lcReportLink == '')
                    return;
                classReportLink.attr('href', lcReportLink + '&show_report=true&sample_report_popUp=true&lc_type=standalone&platform=px');
                studentReportLink.attr('href', lcReportLink + '&show_report=true&report_type=showAll&sample_report_popUp=true&disable_links=true&lc_type=standalone&platform=px');

            },
            updateActivity: function () {
                var itemId = $('.selected').attr('data-aw-id');
                if ($.fn.block) {
                    $('.selected .colIndex2').block();
                    $('.selected .colIndex2 #loadingBlock').remove();
                }

                $.post(PxPage.Routes.ActivitiesWidget_UpdateDueDateColumn, { itemId: itemId }, function (response) {
                    $('.selected .colIndex2').replaceWith(response);
                    $('.selected').removeClass('selected');

                });

            },
            loadActivitiesWidgetContent: function (itemId, mode) {
                if (!itemId)
                    return;
                var row = $('.itemRow[data-aw-id="' + itemId + '"]');
                var cell = mode === 'activity' ? row.find('.colIndex1 span') : row.find('.colIndex3 span');
                var url = cell.attr('url');
                var title = cell.html();
                $('.selected').removeClass('selected');
                row.addClass('selected');
                
                PxPage.openContent({ url: url, title: title, useIFrame: true, useFne: true, winHistory: false });
            },

            AssignItem: function (assignDate, currentNode) {
                var parent = currentNode.closest("tr"),
                    itemId = parent.attr("data-aw-id"),
                    dateSplitted = (assignDate).split("/"),
                    persistSorting = currentNode.closest(".PX_ActivitiesWidget").children("#tableHeader").children("#sortcontainer").children("#sortCheckbox").is(":checked")

                PxPage.Loading('#PX_ActivitiesWidget');

                $.ajax({
                    url: PxPage.Routes.ActivitiesWidget_Assign,
                    data: {
                        itemId: itemId,
                        dueYear: dateSplitted[2],
                        dueMonth: dateSplitted[0],
                        dueDay: dateSplitted[1],
                        dueHour: "11",
                        dueMinute: "59",
                        dueAmpm: "PM",
                        behavior: "assign",
                        isImportant: "false",
                        completionTrigger: '1',
                        IncludeGbbScoreTrigger: "0",
                        gradebookCategory: "2",
                        CalculationTypeTrigger: "0",
                        syllabusFilter: "LearningCurveItems",
                        points: "100",
                        rubricId: "",
                        isGradeable: true,
                        IsMarkAsCompleteChecked: false,
                        isAllowLateSubmission: false,
                        isSendReminder: false,
                        reminderDurationCount: 0,
                        reminderDurationType: "",
                        reminderSubject: "",
                        reminderBody: "",
                        isHighlightLateSubmission: false,
                        isAllowLateGracePeriod: false,
                        lateGraceDuration: 0,
                        lateGraceDurationType: "",
                        isAllowExtraCredit: false,
                        groupId: "",
                        persistSorting: persistSorting
                    },
                    type: "POST",
                    success: function (response) {
                        $(".widgetItem.PX_ActivitiesWidget").children(".widgetContents").children(".widgetBody").empty()
                        $(".widgetItem.PX_ActivitiesWidget").children(".widgetContents").children(".widgetBody").html(response)
                        PxPage.Loaded('#PX_ActivitiesWidget');

                    }
                });

            },

            onUnassignedClick: function (event) {


                var id = $(event.target).attr("id");
                $.ajax({
                    url: PxPage.Routes.unassign_item_date,
                    data: {
                        itemId: id
                    },
                    type: "POST",
                    success: function () {

                        var currrentNode = $("tr");
                        var node = currrentNode.filter("[data-aw-id=" + id + "]");
                        node.removeClass("assigned");

                        var titleLink = $(node.find("td").find("span.titleSpan"));
                        titleLink.removeClass("assignedTextChange");
                        titleLink.addClass("noDecoration");

                        var reportLink = $(node.find("td").find("span.reportSpan"));
                        reportLink.removeClass("assignedTextChange");

                        var dueLink = $(node.find("td").find("span.assignStyles"));
                        dueLink.closest(".colIndex2").removeClass("assigned");
                        dueLink.removeClass("assignedTextChange");
                        dueLink.text("Assign");

                        $(".ui-icon-closethick").click();
                    }
                });


            },

            onDateAssignedClick: function (selected, data) {
                var currentNode = data.currentNode,
                    selectedDate = data.StartDate == '01/01/0001' ? '01/01/9999' : data.StartDate;

                _static.fn.AssignItem(selectedDate, currentNode);

            },

            sortByDueDate: function () {
                $.ajax({
                    url: PxPage.Routes.sort_activities_widget,
                    data: {
                        sort: true
                    },
                    type: "POST",
                    success: function (response) {

                        PxPage.Loading();
                        $(".widgetBody.PX_ActivitiesWidget").html(response);
                        PxPage.Loaded();
                    }
                });
            },

            unsortActivity: function () {
                $.ajax({
                    url: PxPage.Routes.unsorted_activities_widget,
                    type: "POST",
                    success: function (response) {

                        PxPage.Loading();
                        $(".widgetBody.PX_ActivitiesWidget").html(response);
                        PxPage.Loaded();
                    }
                });
            },

            onAssignClick: function (event) {
                var startDate = "",
                    endDate = "",
                    mode = "single",
                    currentNode = $(event.target),
                    unassignLC = false;
                var parent = currentNode.closest("tr");
                var dateAssigned = parent.children(".colIndex2").text().replace("Due", "");
                dateAssigned = $.trim(dateAssigned);
                if (dateAssigned != "") {
                    var regex = new RegExp(/^(1[0-2]|0?[1-9])\/(3[01]|[12][0-9]|0?[1-9])\/(?:[0-9]{2})?[0-9]{2}$/);
                    if (dateAssigned.match(regex)) {
                        var d = Date.parse(dateAssigned);
                        startDate = d;
                    }

                }

                if (parent.children(".colIndex2").hasClass("assigned")) {

                    unassignLC = true;
                }
                PxPage.ShowDatePicker({
                    calendarMode: mode,
                    oldStartDate: startDate,
                    oldDueDate: endDate,
                    customValues: { currentNode: currentNode, id: parent.attr("data-aw-id"), unassignLC: unassignLC, LC: true },
                    customTitle: "Click on a date to set the due date.",
                    callback: _static.fn.onDateAssignedClick,
                    autoCloseAfterAssign: true,
                    instructions: false

                });
            },
            onClickSort: function () {
                if ($(_static.sel.sortCheckBox).is(":checked")) {
                    if ($(".assigned").length > 0) {
                        _static.fn.sortByDueDate();

                    } else {
                        alert("There are no activities to sort.");
                        $(_static.sel.sortCheckBox).prop("checked", false);
                        return;
                    }
                } else {

                    _static.fn.unsortActivity();
                }

            },
            onClickEventDelegation: function (event) {
                var classList = new Array();
                if (event.target.id && event.target.id != '') {
                    classList.push(event.target.id);
                }
                classList = classList.concat(event.target.className.split(/\s+/));
                
                var hasHandler = false;

                for (var i = 0; (i < classList.length && !hasHandler) ; i++) {
                    hasHandler = true;
                    switch (classList[i]) {
                        case _static.cssclass.linkCreateCourse:
                            window.location.hash = '#createCourse';
                            break;
                        case _static.cssclass.btnActivateCourse:
                            $(_static.sel.btnActivateCourse).hide();
                            $('.homepage-course-info .course-title-lable').html(_static.cached.courseTitle);
                            hasHandler = false;
                            break;
                        case _static.cssclass.btnSubmitActiviteCourse:
                            _static.cached.courseTitle = $('.create-course-title').val();
                            hasHandler = false;
                            break;
                        case _static.cssclass.btnUnassignLC:
                            _static.fn.onUnassignedClick(event);
                            break;
                        case _static.cssclass.checkBoxSort:
                            _static.fn.onClickSort();
                            break;

                        case _static.cssclass.linkAssign:
                            _static.fn.onAssignClick(event);
                            break;
                        case _static.cssclass.linkActivityName:
                                var $this = $(event.target),
                                parentTr = $this.closest('.itemRow'),
                                activityId = parentTr.attr('data-aw-id');
                                window.location.hash = '#activity_' + activityId;
                            break;
                        case _static.cssclass.linkReport:
                            var $this = $(event.target),
                                parentTr = $this.closest('.itemRow'),
                                activityId = parentTr.attr('data-aw-id');
                            window.location.hash = '#report_' + activityId;
                            break;
                        case _static.cssclass.linkStudentActivityReport:
                        case _static.cssclass.linkClassActiviyReport:
                            window.location.hash = '#report';
                            hasHandler = false;
                            break;
                        case _static.cssclass.linkActivateCourse:
                            window.location.hash = '#activate';
                            hasHandler = false;
                            break;
                        case _static.cssclass.btnCloseFne:
                            _static.fn.updateActivity();
                            $('#fne-content').empty();
                            break;
                        case 'fne-link':
                            break;
                        default:
                            hasHandler = false;
                            break;

                    }

                }
                if(hasHandler)
                    event.preventDefault();
            }


        }
    };
    // static functions
    return {
        Init: function () {
            if (self == top) {
                $('.product-type-learningcurve').first().unbind('click');
                $('.product-type-learningcurve').first().bind('click', _static.fn.onClickEventDelegation);
                _static.fn.setSampleReportLink();

                PxActivitiesWidget.InitHasher();
            }
            return false;
        },
        InitHasher: function () {
            HashHistory.InitializeWithArgs({ defaultHash: '', onHasherChanged: _static.fn.onHashChange, onHasherInit: _static.fn.onHashInit });
            //_static.fn.onHashChange();
            //window.onhashchange = _static.fn.onHashChange;
        }
    };
} (jQuery)