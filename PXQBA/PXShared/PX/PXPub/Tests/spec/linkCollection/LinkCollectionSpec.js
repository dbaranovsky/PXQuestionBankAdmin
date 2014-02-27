/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/blanket/blanket.min.js" data-cover-adapter="../../lib/blanket/jasmine-blanket.js" />

/// <reference path="../../../Tests/lib/jasmine-jquery.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/other/jquery.tmpl.js" />
/// <reference path="../../../Scripts/jquery/jquery.blockUI.js" />
/// <reference path="../../../Scripts/jquery/jquery.utils.js" />
/// <reference path="../../../Scripts/jquery/jquery-ui-1.10.3.custom.min.js" />
/// <reference path="../../../Scripts/jquery/jquery.validate.js" />
/// <reference path="../../../Tests/lib/mock-ajax.js" />
/// <reference path="../../../Tests/lib/viewrender.js" />

/// <reference path="../../../Scripts/Common/PxPage.js" />
/// <reference path="../../../Scripts/Common/PxPage.LargeFNE.js" />
/// <reference path="../../../Scripts/Common/PxCommon.js" />
/// <reference path="../../../Scripts\Assignment\Assignment.js" />

/// <reference path="../../../Scripts/LinkCollection/LinkCollection.js" />

describe('LinkCollection', function () {
    var fixture = '';

    beforeEach(function () {
        helper.GetBasicInfoView();
    });

    afterEach(function () {
        $('body').children('div').not('#HTMLReporter').remove();
    });

    it("can hide the add new url popup", function () {
        $('.savecancelbtns [value="Save"]').bind('click', function () {
            PxLinkCollection.CloseAddResourse();
        });        
        PxLinkCollection.ShowLinkOpenPopupAsDialog({ target: { rel: 'divLinkWin' } });

        $('.savecancelbtns [value="Save"]').first().click();

        expect($('.divPopupWin:visible').length == 0).toBeTruthy();
    });

    it("can show the add new url popup", function () {
        PxLinkCollection.BindControls();
        $('.divPopupWin:visible').hide();

        $('.linkOpenPopup').click();

        expect($('.divPopupWin:visible').length == 0).toBeFalsy();
    });

    var helper = {
        GetBasicInfoView: function () {
            var data = {
                viewPath: "LinkCollection",
                viewModel: JSON.stringify({
                    Title: "Test Link Collection",
                    CourseInfo: {
                        CourseType: "faceplate"
                    }
                }),
                viewModelType: "Bfw.PX.PXPub.Models.LinkCollection"
            };

            var view = PxViewRender.RenderView('PXPub', 'EditorTemplates', data);

            jasmine.Fixtures.prototype.addToContainer_(view);
        }
    };
});