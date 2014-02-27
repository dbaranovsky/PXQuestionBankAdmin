/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" /> 
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />

describe("PxPage.LargeFNE Tests:", function () {
    var fixture = '';

    beforeEach(function () {
        jasmine.Fixtures.prototype.addToContainer_(fixture);

        var fixtureMain = "<div id='main'></div>";
        jasmine.Fixtures.prototype.addToContainer_(fixtureMain);
        PxPage.Toasts = jasmine.createSpyObj('PxPage.Toasts', ['Error', 'Success', 'Info']);


        PxPage.FrameAPI = jasmine.createSpy("PxPage.FrameAPI Spy");
        PxPage.LargeFNE.Init();
    });

    afterEach(function () {
        $('body').children('div').not('#HTMLReporter').remove();
    });

    it("can display confirmation popup for unsubmitted changes", function () {
        fixture = "<div id='fne-window'><div id='fne-header'><div class='fne-hidden'><input type='button' id='fne-done' />"
        fixture += "</div></div><div class='show-quiz'></div></div>";
        jasmine.Fixtures.prototype.addToContainer_(fixture);

        var result = false;
        
        spyOn(window, 'confirm').andCallFake(function () {
            result = true;
            return result;
        });

        $(PxPage.switchboard).trigger("fnedonemode");
        $(PxPage.switchboard).trigger("validateNavigateAway");

        expect(result).toBeTruthy();
    });

    it('when event "fnedonemode" is triggered, should not add "require-confirm-custom" class to fne window in default', function () {
        fixture ='"<div id="main"></div><div id="fne-window"></div>';
        setFixtures(fixture);

        $(PxPage.switchboard).trigger("fnedonemode");
        expect($('#fne-window').hasClass('require-confirm-custom')).toBeFalsy();
    });
    var helper = {

    };
});