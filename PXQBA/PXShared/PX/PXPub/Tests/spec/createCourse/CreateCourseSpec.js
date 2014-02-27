describe('Create course Test: ', function () {
    beforeEach(function () {
        setFixtures('<div id="fne-window"><div id="fne-content"></div><div id="fne-unblock-action"></div></div>');
        BaseTestHelper.spy.spyOnPxPage();
        var model = {
            viewModel: JSON.stringify({ "Id": "167802", "Title": "DEMO-1/10/2014", "CourseUserName": "Anthony Cheung", "CourseTimeZone": "Eastern Standard Time", "CourseProductName": "[CourseNumber]-[SectionNumber] [Title],Hacker, Writer\u0027s Reference 7e LearningCurve", "TextEditorConfiguration": { "EditorOptions": null }, "CourseNumber": "", "SectionNumber": "", "DerivedCourseId": "87259", "IsDashboardActive": true, "IsSandboxCourse": false, "CurrentUserName": null, "SchoolName": null, "ActivatedDate": "1/1/0001 12:00:00 AM", "PossibleDerivativeDomains": null, "SelectedDerivativeDomain": null, "DerivativeDomainId": "109883", "ProductName": "Hacker, Writer\u0027s Reference 7e LearningCurve", "IsActivated": false, "ParentId": null, "PassingScore": 0, "AssignTabSettings": { "ShowMakeGradeable": false, "ShowAllowLateSubmissions": false, "ShowScheduleReminder": false, "ShowSendReminder": false, "ShowSubContentCreation": false, "ShowAssignedSameDay": false, "ShowGradebookCategory": false, "ShowPointsPossible": false, "ShowIncludeScore": false, "ShowCalculationType": false, "ShowMarkAsComplete": false, "ShowCompletionCategory": false }, "ProductCourseId": "87259", "RubricTypes": [], "InstructorName": "Anthony Cheung", "IsProductCourse": false, "Units": [], "UserAccess": 0, "UserAccessType": 0, "DownloadOnlyDocumentTypes": "", "IsAllowExtraCredit": true, "SelectedLessonId": null, "SelectedContentId": null, "FilterSection": null, "AssessmentConfiguration": { "Rubrics": { "ShowPreviewOnLeft": false, "ShowPreviewOnRight": false, "ShowEditOnLeft": false, "ShowDeleteOnLeft": false, "ShowEditOnRight": false, "ShowViewAlignments": false, "ShowLeftColumn": false, "ShowRubricAlignments": false, "ShowAssignmentAlignments": false, "ShowRightColumn": false, "ShowInAssessment": false }, "Objectives": { "ShowEditOnLeft": false, "ShowEditOnRight": false, "ShowLeftColumn": false, "ShowRightColumn": false, "ShowViewAlignments": false, "ShowObjectiveAlignments": false, "ShowAssignmentAlignments": false, "ShowInAssessment": false }, "Reports": { "ShowInAssessment": false } }, "AcademicTerm": "9bf5dc3b-3514-4989-9ed4-767175e94464", "PossibleAcademicTerms": [{ "Name": "Fall 2012", "Id": "2349a155-7a5c-4788-880f-f0b6fcb1ff8f2", "StartDate": "11/22/2009", "EndDate": "11/22/2009" }, { "Name": "Winter 2013", "Id": "9bf5dc3b-3514-4989-9ed4-767175e94464", "StartDate": "11/22/2009", "EndDate": "11/22/2009" }, { "Name": "Spring 2013", "Id": "b8979c0d-31c2-4975-ade6-fb65e38f815c", "StartDate": "11/22/2009", "EndDate": "11/22/2009" }, { "Name": "Fall 2013", "Id": "fe26c0be-ff55-451c-896a-5be0f2539a4d", "StartDate": "11/22/2009", "EndDate": "11/22/2009" }, { "Name": "Summer 2013", "Id": "38247af2-3a41-4b27-b69d-2c000de68dd7", "StartDate": "11/22/2009", "EndDate": "11/22/2009" }], "PossibleDomains": [{ "Id": "", "Name": "--Select your school--" }, { "Id": "113498", "Name": "Yale University" }], "PossibleOwners": null, "HasUserMaterials": false, "CompletedItems": [], "Children": [], "NoDueDate": [], "EndOfClass": [], "PassedDueDate": [], "DueNextMonth": [], "DueTwoWeeks": [], "DueThisWeek": [], "CourseType": 3, "CourseSectionType": "learningcurve", "CourseOwner": "125673", "CourseTemplate": "", "DashboardCourseId": "", "CourseHomePage": "PX_LC_HOME_2COLUMN", "StudentStartPage": "", "InstructorStartPage": "", "Theme": "", "WelcomeMessage": "\u0026lt;h1\u0026gt;Welcome\u0026lt;/h1\u0026gt;\n                        \u0026lt;p\u0026gt;This Bedford e-Portfolio gives you a single place to collect and reflect on your work. You can compose directly in your e-Portfolio or uplaoad word documents, video, audio, or images that best represent who you are and what you have created. Just click on the e-Portfolio tab at the top of the page and get started!\u0026amp;nbsp;\u0026lt;/p\u0026gt;\n                        \u0026lt;p\u0026gt;\u0026amp;nbsp;\u0026lt;/p\u0026gt;\n                        \u0026lt;div class=\u0026quot;eportfolio_PortfolioTeaching\u0026quot; style=\u0026quot;float: left;\u0026quot;\u0026gt;\u0026lt;img style=\u0026quot;width: 100%; height: 100%;\u0026quot; src=\u0026quot;/Content/images/Universe-9e-cover.jpg\u0026quot; alt=\u0026quot;\u0026quot; /\u0026gt;\u0026lt;/div\u0026gt;\n                        \u0026lt;ul class=\u0026quot;welcome-links\u0026quot;\u0026gt;\n                        \u0026lt;li\u0026gt;\u0026lt;a href=\u0026quot;#\u0026quot;\u0026gt;Finding and developing your materials\u0026lt;/a\u0026gt;\u0026lt;/li\u0026gt;\n                        \u0026lt;li\u0026gt;\u0026lt;a href=\u0026quot;#\u0026quot;\u0026gt;Creating your Structure\u0026lt;/a\u0026gt;\u0026lt;/li\u0026gt;\n                        \u0026lt;li\u0026gt;\u0026lt;a href=\u0026quot;#\u0026quot;\u0026gt;Choosing a style and design\u0026lt;/a\u0026gt;\u0026lt;/li\u0026gt;\n                        \u0026lt;/ul\u0026gt;\n                    ", "BannerImage": "", "AllowedThemes": "", "AllowCommentSharing": true, "AllowSiteSearch": false, "QuestionCardLayout": "", "QuestionFilter": { "FilterMetadata": null }, "HideStudentViewLink": false, "CourseCreationDate": "1/10/2014 3:23:55 PM", "PublishedStatus": "Unpublished", "SearchEngineIndex": false, "FacebookIntegration": false, "SocialCommentingIntegration": false, "SocialCommentingAllowedTypes": [""], "AllowActivation": true, "IsLoadStartOnInit": false, "OfficeHours": "", "ContactInformation": [], "Syllabus": "", "SyllabusType": "Url", "UseWeightedCategories": false, "DashboardSettings": { "IsInstructorDashboardOn": true, "IsProgramDashboardOn": true, "DashboardHomePageStart": "PX_DASHBOARD_1COLUMN", "ProgramDashboardHomePageStart": "PX_PROGRAM_DASHBOARD_1COLUMN" }, "AllowTrialAccess": false, "SyllabusUrl": "", "SyllabusFileName": "", "GenericCourseId": "", "GenericCourseSupported": true, "CourseSubType": "regular", "EnrollmentSwitchSupported": true, "DomainSwitchSupported": true, "LmsIdRequired": false, "LmsIdLabel": "", "LmsIdPrompt": "", "bfw_tab_settings": { "view_tab": { "show_policies": false, "show_assignment_details": false, "show_rubrics": false, "show_learning_objectives": false } }, "GradeBookCategoryList": null }),
            viewPath: 'CreateCourse',
            viewModelType: 'Bfw.PX.PXPub.Models.Course'
        };
        BaseTestHelper.api.setFixtureFromCache(model, 'Course', 'target', "#fne-content");
    });

    afterEach(function () {
    });
    describe('Initailization', function() {
        
        it('expect fne-window has class "activate-course-fne"', function() {
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle');
            expect($('#fne-window').hasClass('activate-course-fne')).toBeTruthy();
        });
        
        it('expect #FindSchoolPopup to be hidden', function () {
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle');
            expect($('#FindSchoolPopup').is(':visible')).toBeFalsy();
        });
        
        it('expect #schoolsearchbyzip, #college are checked', function () {
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle');
            expect($('#schoolsearchbyzip').prop('checked')).toBeTruthy();
            expect($('#college').prop('checked')).toBeTruthy();

        });

        it('when title is passesd, expect fne window has title', function() {
            var expectedTitle;
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            PxPage.SetFneTitle.andCallFake(function(title) {
                expectedTitle = title;
            });
            CreateCourse.Init('testTitle');
            expect(expectedTitle).toEqual('testTitle');
        });
        it('when domain empty is passesd, expect drop down list is set to "--Select your school-- "', function() {
            var expectedTitle;
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            PxPage.SetFneTitle.andCallFake(function(title) {
                expectedTitle = title;
            });
            CreateCourse.Init('testTitle', '');
            expect($('#PossibleDomains option:selected').html()).toEqual("--Select your school--");
        });

        it('when domain 113498 is passesd, expect drop down list is set to "Yale University"', function() {
            var expectedTitle;
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            PxPage.SetFneTitle.andCallFake(function(title) {
                expectedTitle = title;
            });
            CreateCourse.Init('testTitle', '113498');
            expect($('#PossibleDomains option:selected').html()).toEqual("Yale University");
        });
        
        it('expect #Country will trigger change event', function () {
            var elementIds = new Array();
            var events = new Array();
            spyOn($.fn, 'trigger').andCallFake(function(e) {
                elementIds.push($(this).prop('id'));
                events.push(e);
            });
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            var index = $.inArray("Country", elementIds);
            expect(index).toNotEqual(-1);
            expect(events[index]).toEqual('change');
        });
        
        it('expect #schoolsearchbyzip will trigger click event', function () {
            var elementIds = new Array();
            var events = new Array();
            spyOn($.fn, 'trigger').andCallFake(function (e) {
                elementIds.push($(this).prop('id'));
                events.push(e);
            });
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            var index = $.inArray("schoolsearchbyzip", elementIds);
            expect(index).toNotEqual(-1);
            expect(events[index]).toEqual('click');
        });
    });

    describe('Initailization', function () {
        it('expect #fne-unblock-action bind to click event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#fne-unblock-action')[0], "events").click).toNotEqual(null);
        });
        
        it('when #fne-unblock-action is clicked, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            $('#fne-content').append($('<input id="courseLocation" value="' + window.location.href + '#test" />'));

            $('#fne-unblock-action').trigger('click');

            expect(window.location.href.indexOf('#test')).toNotEqual(-1);
        });
        it('expect #FindSchool bind to click event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#FindSchool')[0], "events").click).toNotEqual(null);
        });
        
        it('when #FindSchool is clicked, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            $('#FindSchool').trigger('click');

            expect($('#FindSchoolPopup').is(':visible')).toBeTruthy();
            expect($('#SelectedSchool').is(':visible')).toBeFalsy();

        });
        
        it('expect #Country bind to change event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#Country')[0], "events").change).toNotEqual(null);
        });
        
        it('when #Country is changed, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            spyOn($, 'getJSON').andCallFake(function (url, c) {
                c({test:'test'});
            });

            CreateCourse.Init('testTitle', '113498');

            expect($.getJSON).toHaveBeenCalled();
            expect($('select#State option').length).toEqual(1);
        });
        
        it('expect #schoolsearchbycity bind to click event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#schoolsearchbycity')[0], "events").click).toNotEqual(null);
        });
        
        it('when #schoolsearchbycity is clicked, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            $('#FindSchool').click();
            $('#schoolsearchbycity').trigger('click');

            expect($('#schoolbox').is(':visible')).toBeTruthy();
            expect($('#zipbox').is(':visible')).toBeFalsy();
        });
        
        it('expect #schoolsearchbyzip bind to click event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#schoolsearchbyzip')[0], "events").click).toNotEqual(null);
        });
        
        it('when #schoolsearchbyzip is clicked, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            $('#FindSchool').click();
            $('#schoolsearchbyzip').trigger('click');
            expect($('#zipbox').is(':visible')).toBeTruthy();
            expect($('#schoolbox').is(':visible')).toBeFalsy();
        });
        
        it('expect #college bind to click event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#schoolsearchbyzip')[0], "events").click).toNotEqual(null);
        });
        it('expect select#PossibleDomains bind to click & change event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('select#PossibleDomains')[0], "events").click).toNotEqual(null);
            expect($._data($('select#PossibleDomains')[0], "events").change).toNotEqual(null);

        });
        
        it('when select#PossibleDomains is changed, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('select#PossibleDomains').val('113498');
            $('select#PossibleDomains').trigger('change');
            expect($('#SelectedDerivativeDomain').val()).toEqual("Yale University");

        });
        
        it('when select#PossibleDomains is clicked, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('select#PossibleDomains').val('113498');
            $('select#PossibleDomains').trigger('click');
            expect($('#SelectedDerivativeDomain').val()).toEqual("Yale University");

        });
        
        it('expect #CloseFindPopupSchool bind to click event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#CloseFindPopupSchool')[0], "events").click).toNotEqual(null);
        });
        
        it('when #CloseFindPopupSchool is clicked, expect event is triggered', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('#CloseFindPopupSchool').trigger('click');
            expect($('#SelectedSchool').is(':visible')).toBeTruthy();
            expect($('#FindSchoolPopup').is(':visible')).toBeFalsy();

        });
        
        it('expect #FindSchools bind to click event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('#FindSchools')[0], "events").click).toNotEqual(null);
        });

        it('when #FindSchools is clicked, expect event is triggered', function() {

            spyOn($, 'getJSON').andCallFake(function(url, c) {
                c({ test: 'test' });
            });

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('#FindSchools').trigger('click');

            expect($.getJSON).toHaveBeenCalled();
            expect($('select#school_search_result option').length).toEqual(1);
        });
        
        it('when #FindSchools is clicked and #FindSchools value is not "Find Schools", expect event is triggered', function () {

            spyOn($, 'getJSON').andCallFake(function (url, c) {
                c({ test: 'test' });
            });

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('#FindSchools').val('test');
            $('#FindSchools').trigger('click');

            expect($.getJSON).toHaveBeenCalled();
            expect($('#FindSchools').val()).toEqual('Find Schools');
        });
        
        it('expect .create-course-title bind to keyup event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('.create-course-title')[0], "events").keyup).toNotEqual(null);
        });
        
        it('when .create-course-title is typed, expect event is triggered', function () {
            $('#CourseProductName').val('');
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('#ProductName').val('product');
            $('.create-course-title ').trigger('keyup');

            expect($('#CourseProductName').val()).toNotEqual('');
        });
        it('expect .create-course-number bind to keyup event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('.create-course-number')[0], "events").keyup).toNotEqual(null);
        });
        
        it('when .create-course-number is typed, expect event is triggered', function () {
            $('#CourseProductName').val('');
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('#ProductName').val('product');
            $('.create-course-number').trigger('keyup');
            expect($('#CourseProductName').val()).toNotEqual('');
        });
        it('expect .create-section-number bind to keyup event', function () {

            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '113498');
            expect($._data($('.create-section-number')[0], "events").keyup).toNotEqual(null);
        });
        it('when .create-section-number is typed and key is ignored, expect event is triggered but nothing should happen', function () {
            $('#CourseProductName').val('');
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('#ProductName').val('product');
            var simulateEnterEvent = jQuery.Event("keyup");
            simulateEnterEvent.which = 8;
            simulateEnterEvent.keyCode = 8;

            $('.create-section-number ').trigger(simulateEnterEvent);

            expect($('#CourseProductName').val()).toEqual('');
        });

        it('when .create-section-number is typed, expect event is triggered', function () {
            $('#CourseProductName').val('');
            PxPage.SetFneTitle = jasmine.createSpy("PxPage.SetFneTitle Spy");
            CreateCourse.Init('testTitle', '');
            $('#ProductName').val('product');
            $('.create-section-number ').trigger('keyup');

            expect($('#CourseProductName').val()).toNotEqual('');
        });
    });
});
