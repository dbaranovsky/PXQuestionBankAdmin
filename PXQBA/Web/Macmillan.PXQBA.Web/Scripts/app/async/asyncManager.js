var asyncManager = (function () {

    self.questionListPage = null;

    self.startWait = function(loaderContainer) {
        if (questionListPage != null) {
            questionListPage.setState({ loading: true });
        } else {
            if (loaderContainer != null) {
                React.renderComponent(Loader({}, " "), loaderContainer);
            }
        }
    };

    self.stopWait = function() {
        if (questionListPage != null) {
            questionListPage.setState({ loading: false });
        }
    };

    return self;
}());