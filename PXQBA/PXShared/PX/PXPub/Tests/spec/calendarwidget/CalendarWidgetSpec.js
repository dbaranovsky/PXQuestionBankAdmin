/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" data-cover-adapter="../../lib/blanket/jasmine-blanket.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Scripts/jquery/jquery.fauxtree.js"/>
/// <reference path="../../../Scripts/jquery/jquery.qtip.min.js"/>
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

/// <reference path="../../../Scripts/fullcalendar/fullcalendar.js />
/// <reference path="../../../Scripts/other/date.js" />
/// <reference path="../../../Scripts/Common/dateFormat.js" />
/// <reference path="../../../Scripts/Handlebar/Handlebar.js" />

/// <reference path="../../../Scripts/calendarwidget/calendarwidget.js" />

describe("CalendarWidget", function () {

    var _,
        noOp = function () {};

    beforeEach(function () {
        _ = CalendarWidget.privateTest();
        PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
        PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
        PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
        PxPage.log = jasmine.createSpy('logger');
        CalendarWidget.privateTest().fn.updateCalendarDate = jasmine.createSpy('updateCalendarDate');
        _.fn.updateCalendarDate = noOp;
        _.fn.scrollCalendar = noOp;
    });

    afterEach(function () {
        $('body').children('div').not('#HTMLReporter').remove();
    });
    
    describe("List View", function () {
        var fixture = '';

        beforeEach(function () {
            if (fixture == '') {
                fixture = helper.GetAgendaView();
            }

            jasmine.Fixtures.prototype.addToContainer_(fixture);
            var fixtureMain = "<div id='main'></div>";
            jasmine.Fixtures.prototype.addToContainer_(fixtureMain);
            helper.AdjustAgendaCss();
        });

        it("can remove assignment from start page", function () {
            CalendarWidget.Init("agenda", 0, '');

            window.JSON.parse = jasmine.createSpy("JSON Parse Spy");

            jasmine.Ajax.useMock();

            CalendarWidget.Remove("1", "agenda", "<div></div>", 0);

            var request = mostRecentAjaxRequest();

            expect(request.params.indexOf('"StartDate":"01/01/0001"') > 0).toBeTruthy();
            expect(request.params.indexOf('"EndDate":"01/01/0001"') > 0).toBeTruthy();
        });

        it("can remove assignment from home page", function () {
            var fauxTree = "<div class='faux-tree'></div>";
            jasmine.Fixtures.prototype.addToContainer_(fauxTree);

            CalendarWidget.Init("agenda", 0, '');

            $(PxPage.switchboard).bind('contentunassigned', function (obj, args) {
                expect(args.date).toEqual("01/01/0001");
                expect(args.startdate).toEqual("01/01/0001");
            });

            jasmine.Ajax.useMock();

            CalendarWidget.Remove("1", "agenda", "<div></div>", 0);
        });

        it("display visible month range on init", function () {
            CalendarWidget.Init("agenda", 0, '');
            var range = helper.GetVisibleWeeks();
            var expected = helper.GetExpectedMonthRange(range);
            expect($('#currentSelection').html()).toBe(expected);
        });

        it("display visible month range on scroll", function () {
            CalendarWidget.Init("agenda", 0, '');

            var rangeBeforeScroll = helper.GetVisibleWeeks();
            var expectedBeforeScroll = helper.GetExpectedMonthRange(rangeBeforeScroll);

            $('.agenda-main').scrollTop(200);
            $('.agenda-main').scroll(); //scrollTop doesn't trigger event processing

            var range = helper.GetVisibleWeeks();
            var expected = helper.GetExpectedMonthRange(range);
            
            expect(expectedBeforeScroll === expected).toBeFalsy(); //make sure we actually scrolled to something new
            expect($('#currentSelection').html()).toBe(expected);
        });

    });

    describe("Calendar View", function () {

        var fixture = '',
            _ = CalendarWidget.privateTest();

        beforeEach(function () {

            if (fixture == '') {
                fixture = helper.GetCalendarView();
            }

            jasmine.Fixtures.prototype.addToContainer_(fixture);

            var fixtureMain = "<div id='main'></div>";
            jasmine.Fixtures.prototype.addToContainer_(fixtureMain);


            // for date formatting
            var timezoneOffset = '<input type="hidden" id="TimeZoneStandardOffset" value="-300" />';
            jasmine.Fixtures.prototype.addToContainer_(timezoneOffset);

            CalendarWidget.Init('month', 0, 'instructor');            

        });

        it("can remove assignment from start page", function () {

            spyOn($, 'ajax');

            CalendarWidget.Init("month", 0, '');

            window.JSON.parse = jasmine.createSpy("JSON Parse Spy");

            jasmine.Ajax.useMock();

            CalendarWidget.Remove("1", "month", "<div></div>", 0);

            var request = $.ajax.mostRecentCall.args[0].data;

            expect(request.indexOf('"StartDate":"01/01/0001"') > 0).toBeTruthy();
            expect(request.indexOf('"EndDate":"01/01/0001"') > 0).toBeTruthy();

        });

        it("can remove assignment from home page", function () {

            spyOn($, 'ajax');

            var fauxTree = "<div class='faux-tree'></div>";
            jasmine.Fixtures.prototype.addToContainer_(fauxTree);

            CalendarWidget.Init("month", 0, '');

            $(PxPage.switchboard).bind('contentunassigned', function (obj, args) {
                expect(args.date).toEqual("01/01/0001");
                expect(args.startdate).toEqual("01/01/0001");
            });

            jasmine.Ajax.useMock();

            CalendarWidget.Remove("1", "month", "<div></div>", 0);
        });


        describe('student', function () {

             it('will not allow drag and drop', function () {
                
                spyOn(_.fn, 'getAssignments');

                spyOn($, 'ajax').andCallFake(function () {
                    spyOn($.fn, 'draggable');
                    _.fn.buildAssignments(helper.events);
                    _.fn.renderAssignments();
                    waits(300);
                    runs(function () {
                        expect($.fn.draggable).not.toHaveBeenCalled();
                    });
                });

                CalendarWidget.Init('month', 0, 'student');

            });

        });

        describe('instructor', function () {

            beforeEach(function () {
                window.PxPage.LargeFNE = {
                    OnChangeSettingsList: function () {}
                };
            });

            it('will update an event with a new date', function () {

                //PxPage = {};
                PxPage.switchboard = document.documentElement;
                $(PxPage.switchboard).data('events', { contentassigned: [] })

                spyOn($, 'ajax').andCallFake(function () {
                    _.fn.buildAssignments(helper.events);
                    _.fn.renderAssignments();
                });

                CalendarWidget.Init('month', 0, 'student');

                $(PxPage.switchboard).bind('contentassigned.test', function (event, args) {
                    var oldEvent = $(_.sel.calendar).fullCalendar('clientEvents')[0];
                    oldEvent.start = args.date;
                    $(_.sel.calendar).fullCalendar('updateEvent', oldEvent);
                    var newDate = $.fullCalendar.formatDate($(_.sel.calendar).fullCalendar('clientEvents')[0].start, 'MM/dd/yyyy HH:mm:ss');
                    expect(newDate).toEqual(args.date);
                });

                _.fn.MoveAssignment({
                    start: new Date()
                });

            });

            it('will make a request for changed group (entity) id', function () {

                spyOn(_.fn, 'getAssignments');

                window.PxPage.LargeFNE = {
                    OnChangeSettingsList: noOp
                };

                $(_.sel.groupMenu).append('<option value="TEST" selected></option>');

                spyOn(_.fn, 'renderCalendar').andCallThrough();

                _.fn.ChangeGroup();
                expect(_.fn.renderCalendar).toHaveBeenCalled();

                spyOn($, 'ajax').andCallFake(function (response) {
                    expect(response.data.entityId).toEqual($(_.sel.groupMenu).children().eq(0).val());
                });

                CalendarWidget.Init('month', 0, 'instructor');

            });

            it('will render different, adjusted dates', function () {

                var switched = false;

                spyOn($, 'ajax').andCallFake(function () {
                    _.fn.buildAssignments(switched ? helper.adjustedEvents : helper.events);
                    _.fn.renderAssignments();
                });

                CalendarWidget.Init('month', 0, 'instructor');

                _.fn.destroyCalendar();
                expect(_.defaults.assignments.length).toBeFalsy();
                switched = true;

                CalendarWidget.Init('month', 0, 'instructor');

                expect($(_.sel.calendar).fullCalendar('clientEvents').length).toEqual(helper.adjustedEvents.length);

            });

        });

    });

    var helper = {

        // mock events for fullcalendar
        events: [{"adjustedGroups":"","editLink":"<a href='#state\/item\/stoneecon2_8_0?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"155511","id":null,"itemid":"stoneecon2_8_0","openLink":"<a href='#state\/item\/stoneecon2_8_0?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(1385874001000)\/","points":0,"start":"\/Date(1385874001000)\/","title":"CHAPTER INTRODUCTION","type":"Assignment"},{"adjustedGroups":"Brad Candullo","editLink":"<a href='#state\/item\/a7e0b3d305464392a7538a0059ba32d9?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"169481","id":null,"itemid":"a7e0b3d305464392a7538a0059ba32d9","openLink":"<a href='#state\/item\/a7e0b3d305464392a7538a0059ba32d9?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(-62135578800000)\/","points":0,"start":"\/Date(1386046801000)\/","title":"Untitled Quiz","type":"Assessment"},{"adjustedGroups":"","editLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"155511","id":null,"itemid":"ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000","openLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(1388552401000)\/","points":0,"start":"\/Date(1388552401000)\/","title":"Chapter 16 Homework Quiz","type":"Assessment"},{"adjustedGroups":"Brad Candullo","editLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"169481","id":null,"itemid":"ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000","openLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(1388552401000)\/","points":0,"start":"\/Date(1389934801000)\/","title":"Chapter 16 Homework Quiz","type":"Assessment"},{"adjustedGroups":"","editLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_A21247E9C346D79C900947659F9E0000?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"155511","id":null,"itemid":"ANGEL_econportal__stoneecon2__master_A21247E9C346D79C900947659F9E0000","openLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_A21247E9C346D79C900947659F9E0000?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(1394769601000)\/","points":0,"start":"\/Date(1394769601000)\/","title":"Chapter 11 Homework Quiz","type":"Assessment"}]
        , adjustedEvents: [{"adjustedGroups":"","editLink":"<a href='#state\/item\/stoneecon2_8_0?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"155511","id":null,"itemid":"stoneecon2_8_0","openLink":"<a href='#state\/item\/stoneecon2_8_0?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(1385874001000)\/","points":0,"start":"\/Date(1385874001000)\/","title":"CHAPTER INTRODUCTION","type":"Assignment"},{"adjustedGroups":"","editLink":"<a href='#state\/item\/a7e0b3d305464392a7538a0059ba32d9?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"169481","id":null,"itemid":"a7e0b3d305464392a7538a0059ba32d9","openLink":"<a href='#state\/item\/a7e0b3d305464392a7538a0059ba32d9?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(-62135578800000)\/","points":0,"start":"\/Date(1386046801000)\/","title":"Untitled Quiz","type":"Assessment"},{"adjustedGroups":"","editLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"169481","id":null,"itemid":"ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000","openLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_0C2F3653491D018FFB55BC9FA3B00000?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(-62135578800000)\/","points":0,"start":"\/Date(1389848401000)\/","title":"Chapter 16 Homework Quiz","type":"Assessment"},{"adjustedGroups":"","editLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_A21247E9C346D79C900947659F9E0000?mode=Assign&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >edit<\/a>","entityid":"155511","id":null,"itemid":"ANGEL_econportal__stoneecon2__master_A21247E9C346D79C900947659F9E0000","openLink":"<a href='#state\/item\/ANGEL_econportal__stoneecon2__master_A21247E9C346D79C900947659F9E0000?mode=Preview&includeDiscussion=False&renderFNE=True&isBeingEdited=False' class='faceplatefne-assignmentwidget' >open<\/a>","originalstart":"\/Date(1394769601000)\/","points":0,"start":"\/Date(1394769601000)\/","title":"Chapter 11 Homework Quiz","type":"Assessment"}]
        , GetAgendaView: function () {
            var viewData = {
                ViewType: {
                    dataType: "System.String",
                    dataValue: "Agenda"
                },
                timeZone: {
                    dataType: "System.String",
                    dataValue: "EST"
                }
            };

            var data = {
                Serializer: 1,
                viewModel: JSON.stringify({
                    Groups: helper.GetGroups()
                }),
                viewModelType: "Bfw.PX.PXPub.Models.AssignmentWidget",
                viewPath: "AgendaFullView",
                viewData: JSON.stringify(viewData)
            };

            var view = "";//"<div class='calendar-navigate-back'>Prev</div><div class='calendar-navigate-next'>Next</div>";
            view += PxViewRender.RenderView('PXPub', 'CalendarWidget', data);

            return view;
        },
        GetCalendarView: function () {
            var data = {
                Serializer: 1,
                viewModel: JSON.stringify({

                }),
                viewModelType: "Bfw.PX.PXPub.Models.AssignmentWidget",
                viewPath: "MonthFullView"
            };

            return PxViewRender.RenderView('PXPub', 'CalendarWidget', data);
        },
        GetGroups: function () {
            var groups = [];

            groups.push({
                Type: "January 2013",
                Title: "week of january (this week)",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "February 2013",
                Title: "week of february",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "March 2013",
                Title: "week of march",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "April 2013",
                Title: "week of april",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "May 2013",
                Title: "week of may",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "June 2013",
                Title: "week of june",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "July 2013",
                Title: "week of july",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "August 2013",
                Title: "week of august",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "September 2013",
                Title: "week of september",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "October 2013",
                Title: "week of october",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "November 2013",
                Title: "week of november",
                Assignments: helper.GetAssignments()
            });

            groups.push({
                Type: "December 2013",
                Title: "week of december",
                Assignments: helper.GetAssignments()
            });

            return groups;
        },
        GetAssignments: function () {
            var assignments = [];

            assignments.push({
                Id: "1",
                Title: "Assignment #1",
                DueDate: new Date()
            });
            assignments.push({
                Id: "2",
                Title: "Assignment #2",
                DueDate: new Date()
            });

            return assignments;
        },
        GetVisibleWeeks: function () {
            var range = [];
            var container = $('.agenda-main');
            var offsetMax = container.height() + container.offset().top;
            var offsetMin = container.offset().top;
            $('.title.daylabel').each(function () {
                if ($(this).offset().top < offsetMax && $(this).offset().top > offsetMin ) {
                    range.push($(this).attr('monthYear'));
                }
            });
            return range;
        },

        GetExpectedMonthRange: function (range) {
            return range[0] == range[range.length - 1] ? range[0] : range[0] + " - " + range[range.length - 1];
        },
        AdjustAgendaCss: function () {
            // agenda container is scrollable and stretches to the window
            // on the real view its wrapped into a fne container that has inline height set
            $('.agenda-main').css("position", "absolute");
            $('.agenda-main').css("top", "300px");
            $('.agenda-main').css("min-width", "800px");
            $('.agenda-main').css("overflow-y", "auto");
            var h = $(window).height() - 300;
            h = (h < 100) ? 100 : h; // show at least one month
            $('.agenda-main').css('height', h + 'px');
        }
    };
});