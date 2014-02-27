var DomainDialog = function ($) {
    return {
        BindControls: function () {
            $('.divPopupClose, .divPopupWin input[name="cancel"]').bind('click', function () { $('body').unblock(); return false; });

        },

        ShowDialog: function () {

            var options = { modal: true, draggable: false, closeOnEscape: true, width: '300px', height: '250px', resizable: false, autoOpen: true };
            var tag = $("#domain-selection-dialog"); //hold the dialog content

            tag.dialog({ modal: options.modal, title: options.title, draggable: options.draggable, closeOnEscape: options.closeOnEscape, width: options.width, resizable: options.resizable, autoOpen: options.autoOpen, close: function () {
                $("#uploadTitle").val("");
            }
            }).dialog('open');

            $(".ui-dialog-title").html("Choose your university");
            $(".divPopupTitle").hide();

        },

        Init: function () {
            DomainDialog.BindControls();
            DomainDialog.ShowDialog();
        }

    }
} (jQuery);