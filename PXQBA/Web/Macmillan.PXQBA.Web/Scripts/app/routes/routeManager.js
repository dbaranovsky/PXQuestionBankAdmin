var routsManager = (function () {
    var self = {};
     
    self.query = 'filter/query';
    self.page = 'page/1';
    self.order = 'order/none';

    self.buildHash = function() {
        return '[filer]/[page]/[order]'
            .replace('[filer]', self.query)
            .replace('[page]', self.page)
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

    return self;
}());

 