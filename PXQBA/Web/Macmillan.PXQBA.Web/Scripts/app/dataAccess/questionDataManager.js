var questionDataManager = (function() {
    var self = {};


    self.processDataResponse = function (response) {
        //save in cache here
    };

    self.loadFromCache = function (query, page, orderType, orderField) {
        // load from cache here
        return null;
    };

    self.getQuestionsByQuery = function (query, page, orderType, orderField) {
        
        var cacheResult = self.loadFromCache(query, page, orderType, orderField);

        if (cacheResult != null) {
            return $.Deferred(function() {
                this.resolve(cacheResult);
            });
        }

        var request = {
            query: query,
            pageNumber: page,
            orderType: orderType, 
            orderField: orderField
        };

        return $.ajax({
            url: window.actions.questionList.getQuestionListUrl,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            self.processDataResponse(response);
        });
    };
    
    return self;
}());