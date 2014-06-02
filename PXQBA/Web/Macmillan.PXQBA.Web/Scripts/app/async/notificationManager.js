var notificationManager = (function () {

    self.htmlContainer = null;

    self.showDanger = function (text) {
        var notifyOptions = {
            message: { text: text },
            type: 'danger',
            fadeOut: { enabled: true, delay: 3000 }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showSuccess = function (text) {
        var notifyOptions = {
            message: { text: text },
            type: 'success',
            fadeOut: { enabled: true, delay: 3000 }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showWarning = function (text) {
        var notifyOptions = {
            message: { text: text },
            type: 'warning',
            fadeOut: { enabled: true, delay: 3000 }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    return self;
}());




 
 