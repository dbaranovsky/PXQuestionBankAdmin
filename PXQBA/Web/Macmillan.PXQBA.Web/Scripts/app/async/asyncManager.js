var asyncManager = (function () {

    self.questionListPage = null;

    self.startWait = function() {
        if (questionListPage != null) {
            questionListPage.setState({ loading: true });
        }
    };

    self.stopWait = function() {
        if (questionListPage != null) {
            questionListPage.setState({ loading: false });
        }
    };

    return self;
}());