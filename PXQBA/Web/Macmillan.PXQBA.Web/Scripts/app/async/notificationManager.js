var notificationManager = (function () {
    self.delay = 3000;
    
    self.htmlContainer = null;

    self.showDanger = function (text, isFadingOut) {
        var enableDelay = isFadingOut == undefined ? true : isFadingOut;
        var notifyOptions = {
            message: { text: text },
            type: 'danger',
            fadeOut: { enabled: enableDelay, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showSuccess = function (text, isFadingOut) {
        var enableDelay = isFadingOut == undefined ? true : isFadingOut;
        var notifyOptions = {
            message: { text: text },
            type: 'success',
            fadeOut: { enabled: enableDelay, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showWarning = function (text, isFadingOut) {
        var enableDelay = isFadingOut == undefined ? true : isFadingOut;
        var notifyOptions = {
            message: { text: text },
            type: 'warning',
            fadeOut: { enabled: enableDelay, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showSuccessHtml = function (html, isFadingOut) {
        var enableDelay = isFadingOut == undefined ? true : isFadingOut;
        var notifyOptions = {
            message: { html: html },
            type: 'success',
            fadeOut: { enabled: enableDelay, delay: self.delay }
        };
        $(self.htmlContainer).notify(notifyOptions).show();
    };

    self.showWarningHtml = function (html, isFadingOut) {
        var enableDelay = isFadingOut == undefined ? true : isFadingOut;
        var notifyOptions = {
            message: { html: html },
            type: 'warning',
            fadeOut: { enabled: enableDelay, delay: self.delay }
        };
        $('.top-center').notify(notifyOptions).show();
    };

    return self;
}());




 
 