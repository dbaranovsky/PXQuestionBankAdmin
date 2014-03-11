var questionDataManager = (function() {
    var self = {};
    self.data = [];
    self.getQuestionsByQuery = function (query) {
        if (self.data.length > 0) {
            return $.Deferred(function() {
                this.resolve(self.data);
            });
        }
        
        return $.ajax({
            url: window.actions.questionList.getQuestionListUrl,
            dataType: 'json',
            type: 'POST'
        }).done(function(data) {
            self.data = data;
        });

    };
    return self;
}());