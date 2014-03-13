var questionDataManager = (function() {
    var self = {};
    self.dataCache = [];
    self.data = [];

    self.processDataResponse = function (response) {
        self.dataCache = response.data;
    };

    self.getQuestionsByQuery = function (query, page) {

        var data = {
            query: query,
            pageNumber: page,
            pageSize: window.actions.questionList.pageSize
        };

        return $.ajax({
            url: window.actions.questionList.getQuestionListUrl,
            data: data,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            self.processDataResponse(response);
        });
    };
    
    return self;
}());