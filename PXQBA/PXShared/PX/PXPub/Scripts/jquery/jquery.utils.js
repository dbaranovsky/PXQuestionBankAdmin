
(function ($) {

    // Some jQuery utility functions
    // A 'live' version of making an element draggable
    jQuery.fn.liveDraggable = function (opts) {
        this.bind("mouseover", function () {
            if (!$(this).data("init")) {
                // Add the new draggable, after removing any existing one.
                $(this).data("init", true).draggable('destroy').draggable(opts);
            }
        });
    };

    // Bind event e to function f, but unbind it first, so this call is idempotent
    jQuery.fn.rebind = function (e, f) { return this.unbind(e, f).bind(e, f); };


} (jQuery));

(function ($) {

    //Returns the string of text that the user has highlighted in a cross-browser way
    var _getRangeString = function () {
        var userSelection = null;
        var range = null;
        var text = "";

        if (window.getSelection) {
            userSelection = window.getSelection();
        }
        else if (document.selection) { // should come last; Opera!
            userSelection = document.selection.createRange();
        }

        if (userSelection.getRangeAt) {
            range = userSelection.getRangeAt(0);
            if (range) {
                userSelection.removeAllRanges();
                userSelection.addRange(range);
                text = range.toString();
                range.detach();
            }
        }
        else if (document.all) {
            //MSIE
            text = document.selection.createRange().text;
            document.selection.empty();
        }
        else { // Safari!
            range = document.createRange();
            range.setStart(userSelection.anchorNode, userSelection.anchorOffset);
            range.setEnd(userSelection.focusNode, userSelection.focusOffset);
            text = range.text;
            range.collapse(false);
        }

        return text;
    }

    window.PxCommon = {
        UserSelectedText: _getRangeString
    };
} (jQuery));