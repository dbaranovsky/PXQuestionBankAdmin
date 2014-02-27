describe("LaunchPad tinyMce Plugin Tests:", function() {
    beforeEach(function () {
        setFixtures('<div id="pxpage-toasts-error"></div>');
        BaseTestHelper.spy.spyOnPxPage(true);

    });
    
    it('If upload file size is larger than 25mb, expect error message', function () {
        OnUploadComplete('{"Status":"error","ErrorMessage":"Upload cannot be completed, file size cannot exceed 25 Megabytes","ParentId":null,"ResourceId":null,"OnSuccessActionUrl":null,"FileName":null}');
        expect($('#pxpage-toasts-error').html()).toEqual('Upload cannot be completed, file size cannot exceed 25 Megabytes');
    });
});