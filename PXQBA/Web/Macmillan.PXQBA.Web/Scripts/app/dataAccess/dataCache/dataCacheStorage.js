function DataCacheStorage() {
    var self = {};

    self.query = '';
    self.orderType = '';
    self.orderField = '';

    self.allAvailableColumns = [];
    self.data = [];
    
    self.toСache = function (response) {
        self.availableFields = response.allAvailableColumns;
        self.data.push({ data: response.questionList, page: response.pageNumber, timeStamp: new Date() });
    };

    self.needClear = function (query, orderType, orderField) {
        if ((self.query != query) ||
            (self.orderField != orderField) ||
            (self.orderType != orderType)) {
            return true;
        }
        return false;
    };

    self.loadFromCache = function (query, page, orderType, orderField) {
        if (self.needClear(query, orderType, orderField)) {
            self.clearCache();
            self.query = query;
            self.orderType = orderType;
            self.orderField = orderField;
            return null;
        }
        
        //load here
        // mapping columns here
        return null;
    };

    self.clearCache = function() {
        self.data = [];
        self.allAvailableColumns = [];
    };
    
    return self;

}

