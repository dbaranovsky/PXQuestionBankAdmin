var CalendarWidget = function ($) {

    var startingMonthOffset = 3, // past months to display on init
        startingMonth = new Date().getMonth() - startingMonthOffset, // month to start on render
        rows = 30, // amount of calendar rows on init
        rowHeight = 100, // how tall each calendar row should be
        calendarHeight = rows * rowHeight // total height of calendar on init    

    // static data
    var _static = {

        defaults: {
            assignments: [],
            currentSeq: null,
            calendarDay: null,
            loadedDates: null,
            type: null,
            dueLater: 0,
            viewType: null,
            tipDisplayed: false,
            scrollAfterLoad: 0,
            rendered: false, // false only when calendar was not rendered
            isLoading: false,
            scrollOffset: 0,
            isPast: false, // for loading direction
            isDragging: false
        },

        settings: {

            rowsToAdd: 6, // server reqs into the future or past will add n rows
            nongradeablecolor: '#CCEEFF',
            nongradeablebordercolor: '#74BBE2',
            pastduecolor: '#D3D3D3',
            duesooncolor: '#FFA500',
            duelatercolor: '#A9A9A9',
            scollTopAnimateDuration: 100,
            qtip: {
                events: {
                    hide: function (event) {
                        event.preventDefault();
                    }
                },
                show: {
                    ready: true,
                    solo: true,
                    effect: false
                },
                position: {
                    my: 'bottom left',
                    at: 'top right'
                },
                style: {
                    classes: 'tip-calendar'
                },
                template: $('#tip-template').html()
            }

        },

        sel: {
            backButton: '.calendar-navigate-back',
            nextButton: '.calendar-navigate-next',
            currentSelection: '#currentSelection',
            currentMonthCell: '.current-view',
            id: 'id',
            title: '.title',
            todaySelector: '#todaySelector',

            weeks: '[id^="week_of"]',
            months: 'td[monthYear]',
            seqs: 'div[sequence=',

            tooltip: '.tooltip',
            tooltipLink: '.tooltipLink',
            editLink: '.editLink',
            due: 'due',
            points: 'points',
            a: 'a',
            href: 'href',
            className: 'class',
            type: "type",

            agendaContentWrapper: "#agenda-content-wrapper",
            sequence: 'sequence',
            agendaMain: '.agenda-main',
            monthYear: 'monthyear',
            qtip: '.qtip',

            newMonthYear: '.newMonthYear',
            calendar: '#calendar',
            showMore: '#showMore',
            fcevent: '.fc-event',
            fceventinner: '.fc-event-inner',
            fcborder: ".fc-border-separate thead",
            fccurrentDay: '.fc-today',

            groupMenu: '#ddlSettingsList'
        },

        // private functions
        fn: {

            BindControls: function (type) {

                _static.defaults.type = type;
                _static.defaults.calendarDay = null;
                _static.defaults.loadedDates = [];

                $(document).bind('click', function () {
                    $(_static.sel.qtip).qtip('destroy');
                });

                if (_static.defaults.type === 'agenda') {
                    _static.fn.bindAgendaControls();
                }
                else {
                    _static.fn.bindCalendarControls();
                }

                // pubsub events
                $(PxPage.switchboard)
                    .unbind('calendar.loaded', _static.fn.calendarLoaded)
                    .unbind('calendar.loadFutureDates', _static.fn.loadFutureDates)
                    .unbind('calendar.loadPastDates', _static.fn.loadPastDates)
                    .bind('calendar.loaded', _static.fn.calendarLoaded)
                    .bind('calendar.loadFutureDates', _static.fn.loadFutureDates)
                    .bind('calendar.loadPastDates', _static.fn.loadPastDates)
                    .bind('fneclosed.calendar', _static.fn.destroyCalendar);
                    
            },

            bindAgendaControls: function () {

                _static.fn.ListToolTip();
                _static.fn.OnScrollList();

                $(_static.sel.agendaMain).unbind('scroll').bind('scroll', _static.fn.OnScrollList);

                $(_static.sel.todaySelector).unbind('click').bind('click', function (event) {
                    _static.fn.ScrollList(0);
                    event.preventDefault();
                });
                $(_static.sel.backButton).unbind('click').bind('click', function () {
                    _static.fn.ScrollList(-1);
                });
                $(_static.sel.nextButton).unbind('click').bind('click', function () {
                    _static.fn.ScrollList(1);
                });

            },

            /***************************************
            * calendar view functions
            ***************************************/

            destroyCalendar: function () {
                PxPage.log('calendar : destroy');
                $(PxPage.switchboard).unbind('fneclosed.calendar');
                _static.defaults.assignments = []; // reset all our event data
            },

            bindCalendarControls: function () {

                _static.defaults.rendered = false;
                _static.fn.initCalendar();

                $(_static.sel.todaySelector).unbind('click').bind('click', function (event) {
                    _static.fn.scrollCalendar(0);
                    event.preventDefault();
                });
                $(_static.sel.backButton).unbind('click').bind('click', function () {
                    _static.fn.scrollCalendar(-1);
                });
                $(_static.sel.nextButton).unbind('click').bind('click', function () {
                    _static.fn.scrollCalendar(1);
                });

            },

            scrollCalendar: function (direction) {
                
                PxPage.log('calendar : scroll : ' + direction);

                var target,
                    targetTop,
                    curScrollPosition = $(_static.sel.agendaMain).scrollTop();

                // navigate to todays sequence div
                if (_static.defaults.currentSeq == null || direction == 0) {
                    target = $(_static.sel.fccurrentDay).parent().prevAll().find(_static.sel.newMonthYear).sort(function (a, b) {
                        
                        return $(a).attr('sequence') * 1 < $(b).attr('sequence') * 1;
                    }).first();
                }
                else {
                    var newSequence = parseInt(_static.defaults.currentSeq, 10) + direction;
                    target = $(_static.sel.seqs + newSequence + ']');
                }

                targetTop = target.offset().top - $(_static.sel.agendaMain).offset().top + curScrollPosition;

                $(_static.sel.agendaMain).animate({
                    'scrollTop': targetTop,
                }, _static.settings.scollTopAnimateDuration, _static.fn.updateCalendarDate);

                // set up today
                if (direction === 0) {
                    _static.defaults.currentSeq = target.attr(_static.sel.sequence);
                    $(_static.sel.currentSelection).text(target.attr(_static.sel.monthYear));
                }

            },

            /*
            * load events
            */

            calendarLoaded: function (event, args) {
                PxPage.log('calendar : date load success');
                _static.fn.renderAssignments();
                PxPage.Loaded("fne-content");
                window.setTimeout(function () {
                    _static.defaults.isLoading = false;
                }, 1200);
            },

            // prepares ui for load
            loadPrepare: function (isPast) {
                _static.fn.unbindScroll();
                _static.defaults.isLoading = true;
                _static.defaults.isPast = isPast || false;
                PxPage.Loading("fne-content");
            },

            loadPastDates: function () {

                PxPage.log('calendar : loading past dates...');

                _static.fn.loadPrepare(true);

                // set starting month
                startingMonth -= 3;

                // stops trimming of inverse direction
                rows = rows += _static.settings.rowsToAdd*3;
                calendarHeight = rows * rowHeight;

                _static.defaults.scrollAfterLoad = 600;

                _static.fn.renderCalendar();

            },

            // renders next dates and calls to server
            loadFutureDates: function () {

                PxPage.log('calendar : loading future dates...');

                _static.fn.loadPrepare();

                // calculation for adding more dates (rows) to calendar
                rows = rows += _static.settings.rowsToAdd*3;
                calendarHeight = rows * rowHeight;

                _static.fn.renderCalendar();

            },

            /*
            * scroll listeners
            */

            bindCalendarScroll: function () {

                var pageTop = _static.defaults.scrollOffset + rowHeight + 100, // cached top of page value
                    x, y;

                $(_static.sel.agendaMain)
                    .unbind('scroll')
                    .bind('scroll', function () {

                        x = $(_static.sel.agendaMain).scrollTop();
                        y = $(_static.sel.calendar).height() - $(_static.sel.agendaMain).height();

                        //console.log(pageTop, '>', x); // DEBUG

                        if((x + (rowHeight * _static.settings.rowsToAdd)) > y) {
                            // load future dates if we are near document end
                            _static.defaults.scrollAfterLoad = x;
                            $(PxPage.switchboard).trigger('calendar.loadFutureDates');
                        }
                        else if (pageTop > x) {
                            // at the top of the page, load past dates
                            $(PxPage.switchboard).trigger('calendar.loadPastDates');
                        }
                        
                        _static.fn.updateCalendarDate();

                        // hide tip on scroll
                        if (_static.settings.tipDisplayed) {
                            $(_static.sel.qtip).qtip('destroy');
                            _static.settings.tipDisplayed = false;
                        }
                        
                        if (_static.defaults.isDragging) {
                            //update FullCalendar grid  tracking
                            $(_static.sel.calendar).fullCalendar("updateCoordinates");
                        }
                    });

            },

            unbindScroll: function () {
                $(_static.sel.agendaMain).unbind('scroll');
            },

            /*
            * calendar callbacks
            */

            // date contains the drop m/d/y
            // event contains the correct start time
            drop: function (date, allDay, jsEvent, ui) {
                var event = $(_static.sel.calendar).fullCalendar('clientEvents', ui.helper[0].id),
                    _date = new Date();
                    _date.setTime( new Date(event[0].start).getTime() );
                    _date.setMonth(date.getMonth());
                    _date.setDate(date.getDate());
                    _date.setFullYear(date.getFullYear());
                _static.defaults.isDragging = false;
                if (event != null && event.length) {
                    var item = {
                        itemid: event[0].itemid,
                        points: event[0].points,
                        start: new Date(_date),
                        entityid: event[0].entityid,
                        id: event[0].id
                    };
                    _static.fn.MoveAssignment(item);
                    event.color = _static.fn.SetCalendaritemColor(item);
                }
            },

            // displays an event tool tip using qtip2
            // http://qtip2.com/api
            eventClick: function (calEvent) {
                var isInstructorView = (_static.defaults.viewType === 'instructor') ? true : false;
                //only allow removing items when the class is selected and item is not an adjustment
                var isRemoveAllowed = $('#CourseId').val() == calEvent.entityid && _static.fn.getGroupId() == "";

                $(_static.sel.qtip).qtip('destroy');

                // IIFE to compile and render qtip template
                var renderTemplate = (function () {
                    var template = Handlebars.compile(_static.settings.qtip.template);

                    var context = {
                            description: calEvent.description,
                            groups: calEvent.adjustedGroups,
                            exception: {
                                has: !!calEvent.adjustedGroups.length,
                                date: $.fullCalendar.formatDate(calEvent.start, 'ddd, MMMM dd'),
                                time: $.fullCalendar.formatDate(calEvent.start, 'hh:mmtt')
                            },
                            original: {
                                label: 'Original ',
                                date: $.fullCalendar.formatDate(calEvent.originalstart || calEvent.start, 'ddd, MMMM dd'),
                                time: $.fullCalendar.formatDate(calEvent.originalstart || calEvent.start, 'hh:mmtt')
                            },
                            points: calEvent.points,
                            link: {
                                edit: isInstructorView && calEvent.editLink,
                                open: calEvent.openLink,
                                clear: (isInstructorView && isRemoveAllowed
                                        ? "<a href='javascript:;' onclick=CalendarWidget.Remove('" + calEvent.itemid + "','" + calEvent.entityid + "','" 
                                        + _static.defaults.type + "','" + _static.sel.agendaContentWrapper + "','"
                                        + calEvent.points + "')>clear due date</a>" : "")
                            }
                        };

                    return template(context);

                }());

                // set it as object for fullCalendar
                var obj = {
                    content: {
                        text: renderTemplate
                    }
                };

                // extend base settings with it
                $(this).qtip( $.extend(obj, _static.settings.qtip) );

                _static.settings.tipDisplayed = true;
            },

            /*
            * core methods
            */

            updateCalendarDate: function () {

                var nextMonth;

                $(_static.sel.agendaMain).find('.active').removeClass('active');

                $(_static.sel.newMonthYear).each(function (index, element) {

                    var diff = $(this).offset().top - $(_static.sel.agendaMain).offset().top;

                    if (diff >= 0) { // future
                        nextMonth = $(element);
                        return false;
                    }
                    else if (diff > 0 && diff >= rowHeight) { // past
                        nextMonth = $(_static.sel.newMonthYear).eq(0);
                        return false;
                    }

                })

                if (nextMonth.length) {
                    _static.defaults.currentSeq = nextMonth.attr(_static.sel.sequence);
                    $(_static.sel.currentSelection).text(nextMonth.attr(_static.sel.monthYear));
                    nextMonth.addClass('active');
                }

            },
            
            // interfaces with fullCalendar api to render calendar
            // http://arshaw.com/fullcalendar/docs
            // @param {function} cb : called after calendar is rendered (estimated - fullCalendar 1.x has no render callback)
            renderCalendar: function (cb) {
                PxPage.Loading("fne-content");
                PxPage.log('calendar : render');

                var newScrollTop = _static.defaults.scrollAfterLoad || $(_static.sel.agendaMain).scrollTop();

              
                if (_static.defaults.isDragging) {
                    $(document).trigger("mouseup");//this is a hack to cancel dragging when we're about to re-render the calendar
                }
                //Make sure draggable has been initialized before destroying it.
                $(_static.sel.calendar).find(".fc-event.ui-draggable").draggable("destroy");
                
                // kill current calendar and init a new with updated row and starting point (startingMonth)
                $(_static.sel.calendar)
                    .fullCalendar('destroy')
                    .fullCalendar({
                        header: false,
                        editable: false,
                        month: startingMonth,
                        rows: rows,
                        height: calendarHeight,
                        eventClick: _static.fn.eventClick,
                        droppable: true,
                        drop: _static.fn.drop,
                        events: _static.defaults.assignments
                    });


                
                
                //if we are currently dragging the node, trigger trag start on the calendar
                $(_static.sel.agendaMain).scrollTop(newScrollTop);

                if (typeof cb === 'function') {
                    cb.call();
                }

                _static.fn.getAssignments();
                // scroll to today on first load
                if (!_static.defaults.rendered) {
                    _static.fn.scrollCalendar(0);
                    _static.defaults.rendered = true;
                    // scroll top animation will trigger scoll event
                    // attached event handler only after animation has been done
                    window.setTimeout(_static.fn.bindCalendarScroll, _static.settings.scollTopAnimateDuration);
                } else {
                    _static.fn.bindCalendarScroll();
                }

                PxPage.Loaded("fne-content");
            },

            // sets color of non-graded assignments only
            SetCalendaritemColor: function (event) {
                if (event.points == '' || event.points == 0) {
                    event.backgroundColor = _static.settings.nongradeablecolor;
                    event.borderColor = _static.settings.nongradeablebordercolor;
                }
            },

            MoveAssignment: function (item) {
                PxPage.Loading("fne-content");

                var entityId = _static.fn.getGroupId() == "" ? item.entityid : _static.fn.getGroupId();
                
                //update internal calendar item data with new date
                var existingItem = $.grep(_static.defaults.assignments, function(elem, i) {
                    return elem.id == item.id;
                });
                if (existingItem.length) {
                    existingItem[0].start = item.start;
                    
                    if (item.entityid == $('#CourseId').val()) {
                        //if we're moving a non-adjusted item, set the originalstart date as well (for tooltip)
                        existingItem[0].originalstart = item.start;
                    }
                }
               
                _static.fn.MergeItemsWithSameDate(existingItem);
                
                //update the server
                if ($(PxPage.switchboard).data('events').contentassigned) {
                    var args = new Object();
                    args.entityId = entityId;
                    args.id = item.itemid;
                    args.date = $.fullCalendar.formatDate(item.start, "MM/dd/yyyy HH:mm:ss");

                    $(PxPage.switchboard).trigger("contentassigned", args);
                }
                else {
                    var args = {
                        itemId: item.itemid,
                        assignedItem: {
                            entityId: entityId,
                            id: item.itemid,
                            DueDate: dateFormat.dateConvertToUtc($.fullCalendar.formatDate(item.start, "MM/dd/yyyy HH:mm:ss")),
                            EndDate: dateFormat.dateConvertToUtc($.fullCalendar.formatDate(item.start, "MM/dd/yyyy HH:mm:ss"))
                        },
                        operation: "DatesAssigned"
                    };

                    _static.fn.ItemOperation(args);
                }

                PxPage.Loaded("fne-content");
                //force assignments to re-render to position them correctly
                _static.fn.renderAssignments(true);
            },

            initCalendar: function () {

                // first view call
                _static.fn.renderCalendar(function () {
                    _static.defaults.scrollOffset = $(_static.sel.agendaMain).offset().top;
                });

            },

            // formats the date using fullCalendar
            // @param {array} arr : example ['January', 2014, 1]
            getDateFormat: function (arr) {
                var d = new Date(arr[1], monthByName(arr[0]), arr[2] || 1); // y m d
                return $.fullCalendar.formatDate(d, 'MM/dd/yyyy');
            },

            // gets the attribute containing the month name and year
            // @param {string} el : a jquery object
            getMonthYearArray: function (el) {
                return el.attr('monthyear').split(' ');
            },

            // returns a range of dates that were scrolled
            // or all intial dates in view
            getDatesToLoad: function () {

                var i = parseInt(_static.defaults.currentSeq, 10),
                    newMonthElements = $(_static.sel.newMonthYear),
                    fromDate = _static.fn.getMonthYearArray( newMonthElements.eq(_static.defaults.currentSeq - 1) ), // ex: January 2014
                    toDate = _static.fn.getMonthYearArray( newMonthElements.eq(newMonthElements.length - 2) );
                
                // for initial loading of many dates
                if (!_static.defaults.rendered || _static.defaults.forceAllDates) {
                    fromDate = _static.fn.getMonthYearArray( newMonthElements.eq(1) ), // ex: January 2014
                    toDate = _static.fn.getMonthYearArray( newMonthElements.eq(newMonthElements.length - 1) );
                }
                else if (_static.defaults.isPast) {
                    fromDate = _static.fn.getMonthYearArray( newMonthElements.eq(0) ),
                    toDate = _static.fn.getMonthYearArray( newMonthElements.eq(i) );
                }

                return {
                    firstDay: _static.fn.getDateFormat(fromDate),
                    lastDay: _static.fn.getDateFormat(toDate)
                };

            },

            getGroupId: function () {
                var entityId = $(_static.sel.groupMenu).val();

                if (entityId == null || entityId == "EntireClass" || entityId == "AddIndividual" || entityId == "ManageGroups") {
                    entityId = "";
                }

                return entityId;
            },

            // loads assignments from server
            getAssignments: function () {

                var dayFound = false,
                    dates = _static.fn.getDatesToLoad();

                // check if events for date are already loaded
                $.each(_static.defaults.loadedDates, function (index, curDay) {
                    if (curDay == dates.lastDay) {
                        dayFound = true;
                    }
                });

                if (!dayFound || _static.defaults.isLoading) {

                    _static.defaults.loadedDates.push(dates.lastDay);

                    var dPlusMonthDay = new Date(dates.lastDay);
                        dPlusMonthDay.setMonth(dPlusMonthDay.getMonth() + 1);
                    var plusMonthDay = $.fullCalendar.formatDate(dPlusMonthDay, 'MM/dd/yyyy');

                    _static.defaults.loadedDates.push(plusMonthDay);

                    PxPage.log('calendar : load assignments from : ' + dates.firstDay + ' to : ' + dates.lastDay);

                    $.ajax({
                        url: PxPage.Routes.Calendar_GetAssignments + "?r=" + Math.random(),
                        dataType: 'json',
                        cache: false,
                        data: {
                            entityId: _static.fn.getGroupId(),
                            firstDay: dates.firstDay,
                            lastDay: dates.lastDay
                        },
                        success: function (response) {
                            _static.fn.buildAssignments(response);
                        },
                        error: function (err) {
                            PxPage.log('calendar : fetch error');
                            _static.defaults.loadedDates.splice($.inArray(dates.lastDay, _static.defaults.loadedDates), 1);
                            PxPage.Toasts.Error("An exception occurred while retrieving assignments!");
                        },
                        complete: function () {
                            $(PxPage.switchboard).trigger('calendar.loaded');
                        }
                    });

                }
                else {
                    $(PxPage.switchboard).trigger('calendar.loaded');
                }

            },

            // builds assignments data after server request
            buildAssignments: function (data) {

                if (!data) {
                    return false;
                }

                var i, len = data.length;

                for (i = 0; i < len; i ++) {

                    var start = dateFormat.dateConvertFromCourseTimeZone(dateFormat.jsonDate(data[i].start)),
                        originalstart = dateFormat.dateConvertFromCourseTimeZone(dateFormat.jsonDate(data[i].originalstart)),
                        adjustedGroups = data[i].adjustedGroups;

                    var newEvent = {
                        id: data[i].itemid + "_" + data[i].entityid,
                        itemid: data[i].itemid,
                        entityid: data[i].entityid,
                        title: truncate(data[i].title, 35),
                        start: start,
                        description: data[i].title,
                        points: data[i].points,
                        editLink: data[i].editLink,
                        openLink: data[i].openLink,
                        type: data[i].type,
                        originalstart: originalstart,
                        adjustedGroups: adjustedGroups,
                        className: (adjustedGroups.length) && 'altered-event'
                    };

                    // check if assignment is in existing assignments
                    if ($(_static.sel.calendar).fullCalendar('clientEvents', newEvent.id).length === 0) {
                        _static.defaults.assignments.push(newEvent);
                    }

                }

            },

            // renders all assignments to calendar
            renderAssignments: function (forceRender) {

                var i, len = _static.defaults.assignments.length;

                for (i = 0; i < len; i ++) {

                    var newEvent = _static.defaults.assignments[i];
                    _static.fn.SetCalendaritemColor(newEvent);

                    if ($(_static.sel.calendar).fullCalendar('clientEvents', newEvent.id).length === 0) {
                        $(_static.sel.calendar).fullCalendar('renderEvent', newEvent);
                    }

                }
                if (forceRender) {
                    $(_static.sel.calendar).fullCalendar('rerenderEvents');
                }
                var isInstructorView = (_static.defaults.viewType === 'instructor') ? true : false;
                if (isInstructorView) {
                    $(_static.sel.calendar).find(".fc-event").draggable({
                        zIndex: 9,
                        snap: ".fc-widget-content .fc-day-content",
                        snapModeType: "inner",
                        snapTolerance: 40,
                        appendTo: "body",
                        containment: "#calendar.fc",
                        delay: 50,
                        start: function(event, ui) {
                            _static.defaults.isDragging = true;
                        }
                    });
                }
            },

            /***************************************
            * agenda (list) view functions
            ***************************************/

            ListToolTip: function () {

                $(_static.sel.tooltip).each(function () {
                    var id = $(this).attr(_static.sel.id);
                    var isInstructorView = _static.defaults.viewType == "instructor";

                    var date = new Date($(this).attr(_static.sel.due)).format('ddd, mmmm d');
                    var time = new Date($(this).attr(_static.sel.due)).format('hh:MMtt');
                    var points = $(this).attr(_static.sel.points);

                    var className = '';
                    var link = '';
                    var editLinkClassName = '';
                    var editlink = '';

                    $(_static.sel.tooltipLink).each(function () {
                        if ($(this).attr(_static.sel.id) == id) {
                            link = $(this).find('a').attr(_static.sel.href);
                            className = $(this).find(_static.sel.a).attr(_static.sel.className);
                            return false;
                        }
                    });

                    $(_static.sel.editLink).each(function () {
                        if ($(this).attr(_static.sel.id) == id) {
                            editlink = $(this).find('a').attr(_static.sel.href);
                            editLinkClassName = $(this).find(_static.sel.a).attr(_static.sel.className);
                            return false;
                        }
                    });


                    $(this).bind('click', function () {

                        //only allow removing items when the class is selected and item is not an adjustment
                        var isRemoveAllowed = _static.fn.getGroupId() == "";

                        var self = $(this),
                            obj = {
                            content: {
                                text: '<strong class="tip-title">' + self.text() + '</strong><p>'
                                    + date + ' at ' + time
                                    + '<br />' + points + ' points</p>'
                                    + '<a class=' + className + ' href=' + link + '>open</a>'
                                    + '<a class=' + editLinkClassName
                                    + (isInstructorView ? "' href='" + editlink + "'>edit</a>&nbsp;"+ ''
                                    + (isRemoveAllowed ? "<a class='clear-date' href='#' onclick=CalendarWidget.Remove('" + id + "','','" 
                                    + _static.defaults.type + "','" + _static.sel.agendaContentWrapper + "','" + points + "')>clear due date</a>" : ""):"")
                            }
                        };

                        self.qtip('destroy');
                        self.qtip( $.extend(obj, _static.settings.qtip) );

                        _static.settings.tipDisplayed = true;

                    });

                });

            },

            GetLastVisibleMonth: function () {
                var container = $(_static.sel.agendaMain);
                var fromTop = $(container).height() + $(container).offset().top;
                var els = $(_static.sel.months);
                var len = els.length;
                var i;

                for (i = len - 1 ; i >= 0 ; i--) {
                    if ($(els[i]).offset().top < fromTop) {
                        return $(els[i]);
                    }
                }
            },

            GetHeadline: function (m1, m2) {
                var monthyear = [];
                var headline = '';
                if (m1 !== null) {
                    monthyear[0] = m1.attr(_static.sel.monthYear);
                }

                if (m2 !== null) {
                    monthyear[1] = m2.attr(_static.sel.monthYear);
                }

                if (monthyear.length > 0) {
                    headline = monthyear[0];
                    if (monthyear.length > 1) {
                        if (monthyear[0] != monthyear[1]) {
                            headline += ' - ' + monthyear[1];
                        }
                    }
                }
                return headline;
            },

            /*
            * core methods
            */

            OnScrollList: function () {
                var target = null;

                $(_static.sel.months).each(function () {
                    if ($(this).offset().top >= $(_static.sel.agendaMain).offset().top) {
                        target = $(this);
                        
                        _static.defaults.currentSeq = target.attr(_static.sel.sequence) || _static.defaults.currentSeq;
                        var lastOnPage = _static.fn.GetLastVisibleMonth();
                        var headline = _static.fn.GetHeadline(target, lastOnPage);
                        $(_static.sel.currentSelection).html(headline);

                        return false;
                    }
                });

                // hide tip on scroll
                if (_static.settings.tipDisplayed) {
                    $(_static.sel.qtip).qtip('destroy');
                    _static.settings.tipDisplayed = false;
                }

            },

            ScrollList: function (direction) {
                var target;

                if (_static.defaults.currentSeq == null || direction == 0) {
                    target = $(_static.sel.weeks).first();
                }
                else {
                    $(_static.sel.weeks).each(function () {
                        if (Number($(this).attr(_static.sel.sequence)) == Number(_static.defaults.currentSeq) + Number(direction)) {
                            target = $(this);
                            return false;
                        }
                    });
                }

                if (target != null) {
                    var curScrollPosition = $(_static.sel.agendaMain).scrollTop();
                    var target_top = $(target).offset().top - $(_static.sel.agendaMain).offset().top + curScrollPosition - 10;

                    $(_static.sel.agendaMain).animate({
                        scrollTop: target_top
                    });


                    if (direction == 0) {
                        _static.defaults.currentSeq = target.attr(_static.sel.sequence);
                        var lastOnPage = _static.fn.GetLastVisibleMonth();
                        var headline = _static.fn.GetHeadline(target, lastOnPage);
                        $(_static.sel.currentSelection).html(headline);
                    }
                }

            },

            ChangeGroup: function (event) {
                _static.defaults.assignments = [];
                _static.defaults.loadedDates = [];
                _static.defaults.rendered = true;

                PxPage.LargeFNE.OnChangeSettingsList(event);

                if (_static.defaults.type === 'agenda') {
                    //TODO: render agenda for this group
                    _static.fn.UpdateView(_static.defaults.type, _static.sel.agendaContentWrapper);
                }
                else {
                    $(_static.sel.calendar).fullCalendar('removeEvents');
                    _static.defaults.forceAllDates = true;
                    _static.defaults.isPast = false;
                    _static.defaults.scrollAfterLoad = 0;
                    _static.fn.renderCalendar(_static.fn.getAssignments);
                }
            },

            // shows a dialog if assignment has points
            Remove: function (itemId, entityId, type, div, points) {
                if (points > 0) {
                    $("#dialog-confirm").dialog({
                        resizable: false,
                        height: 200,
                        width: 490,
                        modal: true,
                        dialogClass: 'gradebook-confirm',
                        buttons:
                            [{
                                text: "Keep in gradebook",
                                'class': 'keep-gradebook',
                                click: function () {
                                    $(this).dialog("close");
                                    _static.fn.RemoveAssignment(itemId, entityId, type, div, points);
                                }
                            },
                            {
                                text: "Remove from gradebook",
                                'class': 'remove-gradebook',
                                click: function () {
                                    $(this).dialog("close");
                                    _static.fn.RemoveAssignment(itemId, entityId, type, div, 0);
                                }
                            },
                            {
                                text: "Cancel",
                                'class': 'cancel',
                                click: function () {
                                    $(this).dialog("close");
                                }
                            }]
                    });
                }
                else {
                    _static.fn.RemoveAssignment(itemId, entityId, type, div, 0);
                }

                return false;
            },

            RemoveAssignment: function (itemId, entityId, type, div, points) {

                PxPage.Loading('fne-content');

                if (points == null) {
                    points = 0;
                }

                var callback = function () {
                    $.each(_static.defaults.assignments, function (index) {
                        if (this.id == itemId + "_" + entityId) {
                            _static.defaults.assignments.splice(index, 1);
                        }
                    });
                    PxPage.Loaded('fne-content');
                    return _static.fn.UpdateView(type, div);
                };

                if ($('.faux-tree').length > 0 && $(PxPage.switchboard).data('events').contentunassigned) {
                    var args = {
                        id: itemId,
                        points: points,
                        keepInGradebook: points == 0 ? false : true,
                        date: '01/01/0001',
                        startdate: '01/01/0001',
                        callback: callback
                    };

                    $(PxPage.switchboard).trigger("contentunassigned", args);                   
                }
                else {
                    var args = {
                        itemId: itemId,
                        assignedItem: {
                            id: itemId,
                            StartDate: '01/01/0001',
                            EndDate: '01/01/0001',
                            MaxPoints: points
                        },
                        operation: "DatesUnAssigned",
                        keepInGradebook: points == 0 ? false : true,
                        callback: callback
                    };

                    _static.fn.ItemOperation(args);
                }
            },

            UpdateView: function (type, div) {
                var groupId = _static.fn.getGroupId();
                $.ajax({
                    url: type == "agenda" ? PxPage.Routes.Calendar_ListView : PxPage.Routes.Calendar_MonthView,
                    type: 'GET',
                    data: { entityId: groupId },
                    success: function (result) {
                        $(div).html(result);
                        //preserve the current group selection
                        PxPage.LargeFNE.GetSettingsList(groupId);
                        
                        $.when(
                            _static.fn.UpdateAssignmentWidget()
                        ).
                        done(
                            PxPage.Loaded("fne-content")
                        );
                    },
                    error: function () {
                        PxPage.Loaded("fne-content");
                    }
                })
            },

            UpdateAssignmentWidget: function () {
                $.ajax({
                    url: PxPage.Routes.view_assignment_widget,
                    type: "GET",
                    success: function (widget) {
                        $(".assignment-widget").parent().html(widget);
                    }
                });
            },

            ItemOperation: function (args) {
                var data = {
                    itemId: args.itemId,
                    entityId: args.entityId,
                    targetId: args.targetId,
                    assignedItem: args.assignedItem,
                    operation: args.operation,
                    keepInGradebook: args.keepInGradebook
                };

                url = PxPage.Routes.launchpad_item_operation;
                dataType = "json";

                $.ajax({
                    url: url,
                    type: "POST",
                    data: JSON.stringify(data),
                    dataType: dataType,
                    contentType: "application/json; charset=utf-8",
                    success: function () {
                        if (args.callback != null) {
                            args.callback();
                        }
                    }
                });
            },

            MergeItemsWithSameDate: function (item) {
                $.each($(_static.sel.calendar).fullCalendar('clientEvents'), function () {
                    if ((this.start - item.start == 0) && this.itemid == item.itemid && this.entityid != item.entityid) {                        
                        if (item.entityid == $('#CourseId').val()) {
                            $(_static.sel.calendar).fullCalendar('removeEvents', this.id);
                        }
                        else {
                            $(_static.sel.calendar).fullCalendar('removeEvents', item.id);
                        }
                    }
                });
            }
        }
    };

    // static functions
    return {
        Init: function (type, dueLater, viewType) {
            _static.defaults.dueLater = dueLater;
            _static.defaults.viewType = viewType;
            _static.defaults.isDragging = false;
            _static.fn.BindControls(type);
            _static.defaults.scrollOffset = $(_static.sel.agendaMain).offset().top;
            //window._static = _static; // DEBUG
        },
        ChangeGroup: function (event) {
            _static.fn.ChangeGroup(event);
        },
        Remove: function (itemId, entityid, type, div, points) {
            _static.fn.Remove(itemId, entityid, type, div, points);
        },
        bindControls: function (type) {
            _static.fn.BindControls(type);
        },
        privateTest: function () {
            return _static;
        }
    };

}(jQuery)

function weeksInMonths(from, to) {
    var mil = from - to;
    return Math.floor(Math.abs(mil) / (1000 * 7 * 24 * 60 * 60)) + 1;
}

function monthByName(name) {
    var result = '';
    var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

    $.each(months, function (index, month) {
        if (month == name) {
            result = index;
        }
    });

    return result;
}

function truncate(stringToTruncate, maxSymbols) {

    var result = stringToTruncate;

    if (stringToTruncate.length > maxSymbols) {
        var lastIndex = stringToTruncate.indexOf(" ");

        stringToTruncate = stringToTruncate.substring(0, maxSymbols);

        if (lastIndex == -1) {
            result = stringToTruncate;
        }
        else {
            lastIndex = stringToTruncate.lastIndexOf(" ", maxSymbols - 1);

            if (lastIndex != -1) {
                stringToTruncate = stringToTruncate.substring(0, lastIndex);
            }

            result = stringToTruncate;
        }

        result = result + "...";
    }

    return result;
}
