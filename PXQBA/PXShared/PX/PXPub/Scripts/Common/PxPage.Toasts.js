PxPage.Toasts = (function ($) {
    var _static = {
        defaults: {},
        css: {},
        sel: {},
        appName: "Launchpad",
        messages: {},
        operations: {
            Remove: "{0} has been removed.",
            RemoveOnUnassign: "{0} has been unassigned.",
            NewItemTo: "{0} has been added to {1}.",
            NewItemName: "{0} has been added.",
            Move: "{0} has been moved.",
            VisibleToStudents: "{0} has been made {1} to students.",
            InvisibleToStudents: "{0} has been made {1} to students.",
            Default: "Item operation {0} successful.",
        },

        fn: {
            setAppName: function(name) {
                _static.appName = name;
            },
            getAppName: function() {
                return _static.appName;
            },
            setMessage: function (cat, msg) {
                _static.messages[cat] = msg;
            },
            getMessage: function (cat) {
                var msg = null;
                $.each(_static.operations, function(ind, val) {
                    if (ind === cat) {
                        msg = _static.messages[ind];
                        if (!msg) {
                            msg = val;
                        }
                    }
                });
                return (msg)? msg: _static.messages[cat];
            }
        }
    };

    return {
        AppName: function(name) {
            return name == null ? _static.fn.getAppName() : _static.fn.setAppName(name);
        },

        Message: function(cat, msg) {
            return msg == null ? _static.fn.getMessage(cat) : _static.fn.setMessage(cat, msg);
        },

        Operation: _static.operations,
        
        Warning: function(text) {
            return toastr.warning(text);
        },

        PersistantWarning: function(text, callback) {
            var options = {
                positionClass: 'toast-top-full-width',
                timeOut: 0, // Set timeOut to 0 to make it sticky
                extendedTimeOut: 0,
                onclick: callback
            };
            return toastr.warning(text, "", options);
        },

        Success: function(text) {
            return toastr.success(text);
        },

        Info: function(text) {
            return toastr.info(text);
        },

        Error: function(text) {
            return toastr.error(text);
        },

        Clear: function(element) {
            return toastr.clear(element);
        }
    
    };
})(jQuery);