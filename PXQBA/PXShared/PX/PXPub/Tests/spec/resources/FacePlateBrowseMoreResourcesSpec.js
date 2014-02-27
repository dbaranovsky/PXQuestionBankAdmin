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
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

/// <reference path="../../../Scripts/FacePlate/FaceplateBrowseMoreResources.js" />  

var data = {
    viewPath: "Index",
    viewModelType: "System.Collections.Generic.Dictionary`2[System.String, System.String]"
};

var html = PxViewRender.RenderView('PXPub', 'BrowseMoreResourcesWidget', data);

describe('FacePlateBrowseMoreResources', function () {
    
    beforeEach(function () {
        
        jasmine.Ajax.useMock();
        setFixtures(html);
        
        //PxPage setup
        window.PxPage = {};
        appendSetFixtures("<div id='main'></div>");
        appendSetFixtures("<div class='nav-category active'></div>");
        appendSetFixtures("<ul class='faux-tree'><li class='faux-tree-node' data-ft-id='current open chapter' data-ft-state='open'></li></ul>");

        //$(".nav-category.active").data("resourcesChapterId", currentChapter);

        window.PxPage = {
            switchboard: $("#main"),
            Loading: function (args) { },
            Loaded: function (args) { },
            Routes: {
                resource_type_list: "resource_type_list",
                resource_results: "resource_results",
                resource_removed: "resource_removed",
                resource_mine: "resource_mine",
                resource_facets: "resource_facets",
                resource_facets_chapter: "resource_facets_chapter",
                resource_facets_type: "resource_facets_type"
            }
        };
        spyOn(PxPage, "Loading");
        spyOn(PxPage, "Loaded");
        window.set_cookie = jasmine.createSpy("set_cookie");
        $(PxPage.switchboard).unbind("getActiveChapterId").bind("getActiveChapterId", function () {
            return "current open chapter";
        });

        $('.faceplate-browse-resources').FacePlateBrowseMoreResources();
    });

    afterEach(function() {
        //TODO:
    });

    describe('can initalize  ', function() {
        it('event handlers', function () {
            //expect(false).toBe(true);
        });
        it('window resize events', function () {
            //expect(false).toBe(true);
        });
     
    });
    describe('can open the Resources Window', function () {
        it('in default mode', function () {
            expect($('.faceplate-browse-resources').is(":visible")).toBe(true);

            $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow");

            runs(function () {
                expect($('#browseResultsPanel').hasClass("open")).toBe(true);
                expect($('#browseResultsPanel').is(":visible")).toBe(true);
            });
        });
        it("in none mode", function() {
            // display the label
            var message = "this is a blank dialog with coming soon... message";
            $.fn.FacePlateBrowseMoreResources("showMoreResourcesWindow", "none", message);

            expect($("#browseResultsPanel #browseResults").html()).toBe(message);
        });
        it ("in quiz mode", function() {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "quiz");
                expect($('#browseResultsPanel').hasClass("quiz-window-resources"));
            });
        });
        it("in start mode", function () {
            runs(function() {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "start");
                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "start mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toBe("resource_type_list");
                request.response(responseSuccess);
                
            });
            runs(function() {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("start mode response");
            }); 
        });
        it('in chapter mode', function () {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "chapter");
                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "chapter mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toBe("resource_facets_chapter");
                request.response(responseSuccess);

            });
            runs(function () {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("chapter mode response");
            });
        });
        it('opening a specific chapter', function () {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "chapter", "target chapter");
                
                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "chapter mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toContain("resource_results");

                var data = QueryStringToHash(request.url.split("?")[1]);

                expect(data).toEqual({
                    ExactPhrase: "target+chapter",
                    MetaIncludeFields: "meta-topics/meta-topic",
                    Start: "0",
                    Rows: "100",
                    chapterId: "current+open+chapter",
                    ebookOnly: 'false',
                    fromLearningCurve: "false"
                });
                request.response(responseSuccess);

            });
            runs(function () {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("chapter mode response");
            });
        });
        it('in type mode', function () {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "type");
                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "type mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toBe("resource_facets_type");
                request.response(responseSuccess);
            });
            runs(function () {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("type mode response");
            });
        });
        it('opening a specific type', function () {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "type", "target type");

                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "type mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toContain("resource_results");

                var data = QueryStringToHash(request.url.split("?")[1]);

                expect(data).toEqual({
                    ExactPhrase: "target+type",
                    MetaIncludeFields: "meta-content-type",
                    Rows: "100",
                    Start: "0",
                    chapterId: "current+open+chapter",
                    ebookOnly: "false",
                    fromLearningCurve: "false"
                });
                request.response(responseSuccess);

            });
            runs(function () {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("type mode response");
            });
        });
        it('in instructor content mode', function () {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "myresources");
                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "myresources mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toBe("resource_mine");
                request.response(responseSuccess);
            });
            runs(function () {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("myresources mode response");
            });
        });
        it('in removed content mode', function () {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "removed");
                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "removed mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toBe("resource_removed");
                request.response(responseSuccess);
            });
            runs(function () {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("removed mode response");
            });
        });
        it('in disable editing mode (student mode)', function () {
            $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "disableediting");
            expect($('.faceplate-browse-resources').hasClass('disableediting'));
        });
        it('in learningCurve mode', function () {
            runs(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "learningCurve");
                expect(PxPage.Loading).toHaveBeenCalled();

                var responseSuccess = {
                    status: 200,
                    responseText: "type mode response"
                };
                var request = mostRecentAjaxRequest();
                expect(request.url).toBe("resource_facets_type");
                request.response(responseSuccess);
            });
            runs(function () {
                expect(PxPage.Loaded).toHaveBeenCalled();
                expect($("#browseResults").text()).toEqual("type mode response");
            });
        });


    });
    describe("can display resources with include tags based on the current chapter", function() {
        it('can set the _currentChapterId cookie', function() {
            appendSetFixtures("<input id='CourseId' type='hidden' value='course_1' />");
            $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesWindow", "chapter", "target chapter");
            
            expect(window.set_cookie).toHaveBeenCalledWith("course_1_currentChapterId", "current open chapter", '0', '/');
        });
    });
    describe("can search for content", function() {
        it('from the course title box', function() {

        });
        it('from the resources title box', function() {

        });
    });
    
    var QueryStringToHash = function QueryStringToHash(query) {
        var query_string = {};
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            pair[0] = decodeURIComponent(pair[0]);
            pair[1] = decodeURIComponent(pair[1]);
            // If first entry with this name
            if (typeof query_string[pair[0]] === "undefined") {
                query_string[pair[0]] = pair[1];
                // If second entry with this name
            } else if (typeof query_string[pair[0]] === "string") {
                var arr = [query_string[pair[0]], pair[1]];
                query_string[pair[0]] = arr;
                // If third or later entry with this name
            } else {
                query_string[pair[0]].push(pair[1]);
            }
        }
        return query_string;
    };
});