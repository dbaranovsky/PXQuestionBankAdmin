window.helper = {
    confirmDialog: function (titleText, alertText, okText, cancelText, okCallback, cancelCallback, elementToDelete, optionText, optionCallback) {
        var iframe;
        var editorName = "confirm-dialogbox";
        var div;
        var height = "100";

        if ($('#confirm-dialogbox').length > 0) {
            div = $('#confirm-dialogbox');
        } else {
            div = $(document.createElement('div')).attr("id", editorName).attr("class", "confirm-dialogbox-message");
            div.html(alertText);

            if (elementToDelete != 'undefined') {
                iframe = $(elementToDelete).closest('form');
                if (iframe == 'undefined' || iframe.length == 0) {
                    iframe = $(elementToDelete).closest('iframe');
                }
                iframe.append(div);
            }
        }

        var buttons = {};
        buttons["okButton"] = {
            text: okText,
            click: function () {
                if (okCallback) {
                    okCallback(elementToDelete);
                }
                $(this).dialog("close").remove();
            }
        };

        if (optionText != undefined && optionText !== "") {
            buttons["optionButton"] = {
                text: optionText,
                click: function () {
                    if (optionCallback) {
                        optionCallback();
                    }
                    $(this).dialog("close").remove();
                }
            };
            height = 200;
        }

        buttons["cancelButton"] = {
            text: cancelText,
            click: function () {
                if (cancelCallback) {
                    cancelCallback();
                }
                $(this).dialog("close").remove();
            }
        };

        div.dialog({
            resizable: false,
            dialogClass: 'confirm-dialogbox',
            height: height,
            modal: true,
            /*title: titleText,*/
            open: function (event, ui) {
                $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
                if (cancelText.length > 0) {/* Focus to cancel button */
                    $(this).closest('.ui-dialog').find('.ui-dialog-buttonpane button:contains(' + cancelText +')').focus();
                }
            },
            buttons: buttons,
            create: function (event, ui) {
                $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar").css("display", "none");

                $(this).parents(".ui-dialog").css("padding", 10);
                $(this).parents(".ui-dialog").css("border-radius", "10px");
                $(this).parents(".ui-dialog:first").find(".ui-dialog-content").css("padding", 0);
            }
        });
    },

    confirmMessageDialog: function (alertText, saveElement) {
        var iframe;
        var editorName = "confirm-dialogbox";
        var div;
        if ($('#confirm-dialogbox').length > 0) {
            div = $('#confirm-dialogbox');
        } else {
            div = $(document.createElement('div')).attr("id", editorName).attr("class", "confirm-dialogbox-message");
            div.html(alertText);

            if (saveElement != undefined || saveElement != null) {
                iframe = $(saveElement).closest('form');
                if (iframe == undefined || iframe.length == 0) {
                    iframe = $(saveElement).closest('iframe');
                }
                iframe.append(div);
            }
            else {
                saveElement = $('#question-editor');
            }
        }

        var timer = $.timer(function () {
            div.dialog().remove();
        });
        /*timer.set({ time: 300 });*/
        timer.once(3000);

        div.dialog({
            resizable: false,
            dialogClass: 'confirm-dialogbox',
            height: 100,
            modal: true,
            open: function (event, ui) {
                $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
            },
            buttons: [{
                text: "OK",
                click: function () {
                    timer.stop();
                    $(this).dialog("close").remove();
                }
            }],
            close: function () {
                $(this).remove();
            },
            create: function (event, ui) {
                $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar").css("display", "none");

                $(this).parents(".ui-dialog").css("padding", 15);
                $(this).parents(".ui-dialog").css("border-radius", "10px");
                $(this).parents(".ui-dialog:first").find(".ui-dialog-content").css("padding", 0);
            }
        });
    },

    saveDialogOpen: function () {
        var iframe;
        var editorName = "save-dialogbox";
        var div;
        var saveElement = $('#question-editor');

        if ($('#save-dialogbox').length > 0) {
            div = $('#save-dialogbox');
        } else {
            div = $(document.createElement('div')).attr("id", editorName).attr("class", "save-dialogbox-message");
            div.html("Saving...");
        }

        div.dialog({
            resizable: false,
            dialogClass: 'save-dialogbox',
            height: 100,
            modal: true,
            open: function (event, ui) {
                $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
            },
            buttons: [{
                text: "OK",
                click: function () {
                    timer.stop();
                    $(this).dialog("close").remove();
                }
            }],
            close: function () {
                $(this).remove();
            },
            create: function (event, ui) {
                $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar").css("display", "none");

                $(this).parents(".ui-dialog").css("padding", 15);
                $(this).parents(".ui-dialog").css("border-radius", "10px");
                $(this).parents(".ui-dialog:first").find(".ui-dialog-content").css("padding", 0);
            }
        });

        div.dialog("widget").find(".ui-dialog-buttonpane").hide();
    },

    saveDialogClose: function () {
        var saveDialog = $('#save-dialogbox');
        if (saveDialog) {
            saveDialog.dialog("close").remove();
        }
    },

    htmlEncode: function (value) {
        //create a in-memory div, set it's inner text(which jQuery automatically encodes)
        //then grab the encoded contents back out.  The div never exists on the page.
        value = value.replace(/&/g, '&amp;');
        return $('<div/>').text(value).html();
    },

    htmlDecode: function (value) {
        var decodeValue = $('<div/>').html(value).text();
        decodeValue = decodeValue.replace(/&amp;/g, '&');
        return decodeValue;
    }
};


/**
* jquery.timer.js
*
* Copyright (c) 2011 Jason Chavannes <jason.chavannes@gmail.com>
*
* http://jchavannes.com/jquery-timer
*
* Permission is hereby granted, free of charge, to any person
* obtaining a copy of this software and associated documentation
* files (the "Software"), to deal in the Software without
* restriction, including without limitation the rights to use, copy,
* modify, merge, publish, distribute, sublicense, and/or sell copies
* of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
* NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
* BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
* ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

; (function ($) {
    $.timer = function (func, time, autostart) {
        this.set = function (func, time, autostart) {
            this.init = true;
            if (typeof func == 'object') {
                var paramList = ['autostart', 'time'];
                for (var arg in paramList) { if (func[paramList[arg]] != undefined) { eval(paramList[arg] + " = func[paramList[arg]]"); } };
                func = func.action;
            }
            if (typeof func == 'function') { this.action = func; }
            if (!isNaN(time)) { this.intervalTime = time; }
            if (autostart && !this.isActive) {
                this.isActive = true;
                this.setTimer();
            }
            return this;
        };
        this.once = function (time) {
            var timer = this;
            if (isNaN(time)) { time = 0; }
            window.setTimeout(function () { timer.action(); }, time);
            return this;
        };
        this.play = function (reset) {
            if (!this.isActive) {
                if (reset) { this.setTimer(); }
                else { this.setTimer(this.remaining); }
                this.isActive = true;
            }
            return this;
        };
        this.pause = function () {
            if (this.isActive) {
                this.isActive = false;
                this.remaining -= new Date() - this.last;
                this.clearTimer();
            }
            return this;
        };
        this.stop = function () {
            this.isActive = false;
            this.remaining = this.intervalTime;
            this.clearTimer();
            return this;
        };
        this.toggle = function (reset) {
            if (this.isActive) { this.pause(); }
            else if (reset) { this.play(true); }
            else { this.play(); }
            return this;
        };
        this.reset = function () {
            this.isActive = false;
            this.play(true);
            return this;
        };
        this.clearTimer = function () {
            window.clearTimeout(this.timeoutObject);
        };
        this.setTimer = function (time) {
            var timer = this;
            if (typeof this.action != 'function') { return; }
            if (isNaN(time)) { time = this.intervalTime; }
            this.remaining = time;
            this.last = new Date();
            this.clearTimer();
            this.timeoutObject = window.setTimeout(function () { timer.go(); }, time);
        };
        this.go = function () {
            if (this.isActive) {
                this.action();
                this.setTimer();
            }
        };

        if (this.init) {
            return new $.timer(func, time, autostart);
        } else {
            this.set(func, time, autostart);
            return this;
        }
    };
})(jQuery);