$.widget("px.quizDialog", {
    options: {
        width: 400,
        height: 180,
        resizable: false,
        draggable: false,
        closeOnEscape: false,
        title: "Confirmation",
        style: "font-size:14px;padding-left:15px;",
        message: "Please select the options:",
        buttons: {}
    },
    
    _setOption: function (key, value) {
        this.options[key] = value;
    },
    
    _create: function () {
        $(this.element).attr("style", this.options.style);
        $(this.element).text(this.options.message);
        
        $(this.element).dialog({
            width: this.options.width,
            height: this.options.height,
            modal: true,
            resizable: this.options.resizable,
            draggable: this.options.draggable,
            closeOnEscape: this.options.closeOnEscape,
            title: this.options.title,
            buttons: this.options.buttons,
            create: function () {
                $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").css("display", "none");
            }
        });
    },
    
    addbutton: function (name, callback, e) {
        var buttons = this.element.dialog("option", "buttons");
        
        buttons[name] = function () {
            if ($.isFunction(callback)) {
                callback(e);
            } 
            $(this).remove();
        };
        
        this.element.dialog("option", "buttons", buttons);
    }
});