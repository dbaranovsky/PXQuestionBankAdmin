var questionDataManager = (function() {
    var self = {};

    self.cache = {};

    self.processDataResponse = function (response) {
        //save in cache here
    };

    self.loadFromCache = function (query, page, orderType, orderField) {
        // load from cache here
        return null;
    };

    self.getQuestionsByQuery = function (query, page, columns, orderType, orderField) {
        
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
            orderField: orderField,
            columns: columns
        };

        return $.ajax({
            url: window.actions.questionList.getQuestionListUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            self.processDataResponse(response);
        });
    };


    self.saveQuestionData = function(questionId, fieldName, fieldValue) {
        var request = {
            questionId: questionId,
            fieldName: fieldName,
            fieldValue: fieldValue
        };

        return $.ajax({
            url: window.actions.questionList.editQuestionFieldUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            console.log('Edited complete');
            crossroads.resetState();
            crossroads.parse(window.routsManager.buildHash());
            console.log('Refrash complite');
        });
    };

    return self;
}());