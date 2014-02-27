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

/// <reference path="../../../Scripts/rssfeed/rssfeed.js" />

describe("RssFeed", function () {
    var fixture = '';

    beforeEach(function () {
        if (fixture == '') {
            fixture = helper.GetCompactSummaryFPStartView();
        }

        jasmine.Fixtures.prototype.addToContainer_(fixture);

        PxPage.Toasts = jasmine.createSpy("PxPage.Toasts Spy");
        PxPage.Toasts.Success = jasmine.createSpy("PxPage.Toasts.Success Spy");
        PxPage.Toasts.Error = jasmine.createSpy("PxPage.Toasts.Error Spy");
    });

    afterEach(function () {
        $('body').children('div').not('#HTMLReporter').remove();
    });

    it("can populate widget with rss data", function () {
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success("dummyxml");
        });

        var widget = $(".customRSSWidgetMaindiv.customRSSWidgetOuterDiv");
        priorHtml = widget.html();

        PxRssArticle.GetRssFeeds(widget);

        expect(widget.html()).toBe(priorHtml + "dummyxml");
    });

    it("can populate widget with no more articles message", function () {
        spyOn($, 'ajax').andCallFake(function (params) {
            params.success({ Message: "NoMoreRSSFeeds" });
        });

        var widget = $(".customRSSWidgetMaindiv.customRSSWidgetOuterDiv");

        PxRssArticle.GetRssFeeds(widget);
        
        expect(widget.html().indexOf("There are no more articles in this feed...") > -1).toBe(true);
    });

    var helper = {
        GetCompactSummaryFPStartView: function () {
            var data = {
                viewModel: JSON.stringify({
                    
                }),
                viewModelType: "Bfw.PX.PXPub.Models.RSSFeedWidget",
                viewPath: "CompactSummaryFPStart"
            };

            return PxViewRender.RenderView('PXPub', 'RSSFeedWidget', data);
        }
    };
});