var routsManager = (function () {
    var self = {};
     
    self.query = 'filter/query';
    self.page = 'page/1';
    self.columnsKey = 'columns/';
    self.columnsValue = 'bank+seq+dlap_title+dlap_q_type';
    self.order = 'order/none';

    self.buildColumns = function() {
        return self.columnsKey + self.columnsValue;
    };

    self.buildHash = function () {
        return '[filer]/[page]/[columns]/[order]'
            .replace('[filer]', self.query)
            .replace('[page]', self.page)
            .replace('[columns]', self.buildColumns())
            .replace('[order]', self.order);
    };

    self.setPage = function (page) {
        self.page = 'page/' + page;
        hasher.setHash(self.buildHash());
    };

    self.setOrder = function(orderType, fieldName) {
        self.order = 'order/' + orderType;
        if (fieldName != undefined) {
            self.order = self.order + '/' + fieldName;
        }
        hasher.setHash(self.buildHash());
    };

    self.addField = function (field) {
        self.columnsValue = columnHashParameterHelper.addField(field, self.columnsValue);
        hasher.setHash(self.buildHash());
    };

    self.deleteField = function (field) {
        if (!columnHashParameterHelper.isLast(self.columnsValue)) {
            self.columnsValue = columnHashParameterHelper.deleteField(field, self.columnsValue);
        }
        hasher.setHash(self.buildHash());
    };
    
    return self;
}());

 