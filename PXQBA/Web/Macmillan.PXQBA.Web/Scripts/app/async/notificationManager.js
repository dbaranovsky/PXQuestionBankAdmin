var notificationManager = (function () {
    self.delay = 3000;
    
    self.htmlContainer = null;

    self.showDanger = function (text) {
        var notifyOptions = {
            message: { text: text },
            type: 'danger',
            fadeOut: { enabled: true, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showSuccess = function (text) {
        var notifyOptions = {
            message: { text: text },
            type: 'success',
            fadeOut: { enabled: true, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showWarning = function (text) {
        var notifyOptions = {
            message: { text: text },
            type: 'warning',
            fadeOut: { enabled: true, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showSuccessHtml = function (html) {
        var notifyOptions = {
            message: { html: html },
            type: 'success',
            fadeOut: { enabled: true, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showWarningHtml = function (html) {
        var notifyOptions = {
            message: { html: html },
            type: 'warning',
            fadeOut: { enabled: true, delay: self.delay }
        };
        $('.top-center').notify(notifyOptions).show();
    };

    return self;
}());




 
 