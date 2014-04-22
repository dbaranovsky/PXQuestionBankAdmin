var routsManager = (function () {
    var self = {};
    
    //self.query = new RouteItem('filter', 'bank:value|value2+dlap_title:some value'); 
    self.query = new RouteItem('filter', filterHashParameterHelper.emptyValue);
    self.page = new RouteItem('page', '1');
    self.columns = new RouteItem('columns', 'bank+seq+dlap_title+dlap_q_type+dlap_q_status');
    self.order = new RouteItem('order', 'none');

    self.setState = function (filter, page, columns, orderType, orderField) {
        self.query.setValue(filter);
        self.page.setValue(page);
        self.columns.setValue(columns);
        self.order.setValue(orderType + '/' + orderField);
    },

    self.buildHash = function () {
        return '[filer]/[page]/[columns]/[order]'
            .replace('[filer]', self.query.getRoute())
            .replace('[page]', self.page.getRoute())
            .replace('[columns]', self.columns.getRoute())
            .replace('[order]', self.order.getRoute());
    };

    self.setPage = function (page) {
        self.page.setValue(page);
        hasher.setHash(self.buildHash());
    };

    self.setOrder = function (orderType, fieldName) {
        self.order.setValue(orderType);
        self.order.setValue(orderType);
        if (fieldName != undefined) {
            self.order.setValue(self.order.getValue() + '/' + fieldName);
        }
        hasher.setHash(self.buildHash());
    };

    self.addField = function (field) {
        self.columns.setValue(columnHashParameterHelper.addField(field, self.columns.getValue()));
        hasher.setHash(self.buildHash());
    };

    self.deleteField = function (field) {
        if (!columnHashParameterHelper.isLast(self.columns.getValue())) {
            self.columns.setValue(columnHashParameterHelper.deleteField(field, self.columns.getValue()));
        }
        hasher.setHash(self.buildHash());
    };

    self.addFiltration = function (field, valuesArray) {
        self.query.setValue(filterHashParameterHelper.addFiltration(field, valuesArray, self.query.getValue()));
        hasher.setHash(self.buildHash());
    };

    self.deleteFiltration = function (field) {
        self.query.setValue(filterHashParameterHelper.deleteFiltration(field, self.query.getValue()));
        hasher.setHash(self.buildHash());
    };

    return self;
}());

 