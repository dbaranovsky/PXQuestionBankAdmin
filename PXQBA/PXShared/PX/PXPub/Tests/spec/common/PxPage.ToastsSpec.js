/// <reference path="../../../Scripts/jquery/jquery-1.6.4.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" /> 
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.6.4.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.8.18.custom.min.js" />
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxCommon.js" />
/// <reference path="../../../Scripts/Common/PxPage.Toasts.js" />

describe("PxPage.Toasts Tests:", function () {
    it("can set and get application name", function () {
        var test = "Bla bla";
        window.PxPage.Toasts.AppName(test);

        expect(window.PxPage.Toasts.AppName()).toBe(test);
    });
    it("can overwrites operation message", function() {
        var test = "this is a test message";
        var cat = "NewItemName";

        expect(window.PxPage.Toasts.Message(cat)).toEqual(window.PxPage.Toasts.Operation.NewItemName);

        window.PxPage.Toasts.Message(cat, test);

        expect(window.PxPage.Toasts.Message(cat)).not.toEqual(window.PxPage.Toasts.Operation.NewItemName);

        expect(window.PxPage.Toasts.Message(cat)).toEqual(test);
    });
    it("can set and get operation message", function() {
        var test = "this is a test message";
        var cat = "NewCat";

        expect(window.PxPage.Toasts.Message(cat)).toBeUndefined();

        window.PxPage.Toasts.Message(cat, test);
        
        expect(window.PxPage.Toasts.Message(cat)).toEqual(test);
    });
});