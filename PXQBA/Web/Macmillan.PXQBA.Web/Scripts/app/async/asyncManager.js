var asyncManager = (function () {

    self.page = null;

    self.startWait = function(loaderContainer) {
        if (self.page != null) {
            self.page.setState({ loading: true });
        } else {
            if (loaderContainer != null) {
                React.renderComponent(Loader({}, " "), loaderContainer);
            }
        }
    };

    self.stopWait = function() {
        if (self.page != null) {
            self.page.setState({ loading: false });
        }
    };

    return self;
}());