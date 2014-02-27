var EditUnitInfo = function ($) {
    // static data
    var _static = {
        defaults: {
        },
        settings: {
        },
        sel: {
            unitImage: '.unitImage',
            thumbnail: '#thumbnail',
            titlebar: ".ui-dialog-titlebar.ui-widget-header.ui-corner-all.ui-helper-clearfix"
        },
        // private functions
        fn: {
            BindControls: function () {
                $(_static.sel.titlebar).addClass('editContentTitleBar');

                $(_static.sel.unitImage).bind('click', function () {
                    tinyMCE.activeEditor.IsExternalCall = true;
                    tinyMCE.activeEditor.ExternalPlaceholder = $(_static.sel.unitImage);
                    $(tinyMCE.activeEditor.buttons.image).click();
                });
            }
        }
    };
    // static functions
    return {
        Init: function () {
            _static.fn.BindControls();
        },
        UpdateThumbnail: function () {
            $(_static.sel.thumbnail).val($(_static.sel.unitImage).attr('src'));
        }
    };
} (jQuery)

