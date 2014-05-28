var routsManager = (function () {
    var self = {};

    self.query = new RouteItem('filter', filterHashParameterHelper.emptyValue);
    self.page = new RouteItem('page', '1');
    self.columns = new RouteItem('columns', window.consts.grid.initialFieldSet);
    self.order = new RouteItem('order', 'none');

    self.setState = function (filter, page, columns, orderType, orderField) {
        self.query.setValue(filter);
        self.page.setValue(page);
        self.columns.setValue(columns);
        self.order.setValue(orderType + '/' + orderField);
    },

    self.setHashSilently = function(hash) {
        hasher.changed.active = false;  
        hasher.setHash(hash);  
        hasher.changed.active = true;  
    },

    self.buildHash = function () {
        return '[page]/[columns]/[filer]/[order]'
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
        var isFiltrationChenged = filterHashParameterHelper.isFiltrationChenged(field, valuesArray, self.query.getValue(), false);
        self.query.setValue(filterHashParameterHelper.addFiltration(field, valuesArray, self.query.getValue()));
        
        if (isFiltrationChenged) {
            self.page.setValue(1);
        }

        hasher.setHash(self.buildHash());
    };

    self.deleteFiltration = function (field) {
        var isFiltrationChenged = filterHashParameterHelper.isFiltrationChenged(field, null, self.query.getValue(), false);
        self.query.setValue(filterHashParameterHelper.deleteFiltration(field, self.query.getValue()));
        
        if (isFiltrationChenged) {
            self.page.setValue(1);
        }

        hasher.setHash(self.buildHash());
    };


    self.copyAndApplyState = function (filter, page, orderType, orderField) {
        self.copyFiltration(filter);
        self.copyPage(page);
        self.copyOrder(orderType, orderField);
        self.setHashSilently(self.buildHash());
    },

    self.copyFiltration = function (filtersObjects) {
        self.query.setValue(filterHashParameterHelper.emptyValue);
        
        for (var i = 0; i < filtersObjects.length; i++) {
            self.query.setValue(
                filterHashParameterHelper.addFiltration(
                filtersObjects[i].field, filtersObjects[i].values, self.query.getValue()));
        }
    };

    self.copyPage = function (page) {
        self.page.setValue(page);
    };

    self.copyOrder = function (orderType, fieldName) {
        self.order.setValue(orderType);
        if (fieldName != undefined) {
            self.order.setValue(self.order.getValue() + '/' + fieldName);
        }
    };

    return self;
}());

 