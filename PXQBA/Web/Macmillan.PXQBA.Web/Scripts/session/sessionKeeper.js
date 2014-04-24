var sessionKeeper = (function () {
    var self = {};

    self.keepSessionAliveUrl = null;
    self.interval = null;
    
    self.startSessionKeeper = function (keepSessionAliveUrl, interval) {
        self.keepSessionAliveUrl = keepSessionAliveUrl;
        self.interval = interval;
        self.keepSessionAliveLoop();
    };

    self.keepSessionAliveLoop = function () {
        setTimeout(function () { self.keepSessionAlive(); },
            self.interval);
    };

    self.keepSessionAlive = function () {
            $.ajax({
                type: "POST",
                url: self.keepSessionAliveUrl,
                success: function () { }
            });

        self.keepSessionAliveLoop();
    };
    
    return self;
}());