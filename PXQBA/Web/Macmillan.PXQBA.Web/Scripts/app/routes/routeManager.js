var routsManager = (function () {
    var self = {};

    self.query = new RouteItem('filter', filterHashParameterHelper.emptyValue);
    self.page = new RouteItem('page', '1');
    self.columns = new RouteItem('columns', window.consts.grid.initialFieldSet);
    self.order = new RouteItem('order', 'none');
    self.needRerender = new RouteItem('needRerender', 'true');

    self.setState = function (filter, page, columns, orderType, orderField) {
        self.query.setValue(filter);
        self.page.setValue(page);
        self.columns.setValue(columns);
        self.order.setValue(orderType);
        if (orderField != null) {
            self.order.setValue(self.order.getValue() + '/' + orderField);
        }
    },

    self.setHashSilently = function(hash) {
        hasher.changed.active = false;  
        hasher.setHash(hash);  
        hasher.changed.active = true;  
    },

    self.buildHash = function () {
        return '[page]/[columns]/[filer]/[needRerender]/[order]'
            .replace('[filer]', self.query.getRoute())
            .replace('[page]', self.page.getRoute())
            .replace('[columns]', self.columns.getRoute())
            .replace('[order]', self.order.getRoute())
            .replace('[needRerender]', self.needRerender.getRoute());
    };

    self.setPage = function (page) {
        self.page.setValue(page);
        hasher.setHash(self.buildHash());
    };

    self.setOrder = function (orderType, fieldName) {
        self.order.setValue(orderType);
        if (fieldName != null) {
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
       
        if ((self.needResetFiltration(field))&&(isFiltrationChenged)) {
            self.resetParameters();
            self.query.setValue(filterHashParameterHelper.addFiltration(field, valuesArray, self.query.getValue()));
            self.query.setValue(filterHashParameterHelper.addFiltration(window.consts.containsTextName, [], self.query.getValue()));
        } else {
            self.query.setValue(filterHashParameterHelper.addFiltration(field, valuesArray, self.query.getValue()));
        }
        
        
        if (isFiltrationChenged) {
            self.page.setValue(1);
        } else {
            self.needRerender.setValue('false');
        }

        hasher.setHash(self.buildHash());
        self.needRerender.setValue('true');
    };

    self.resetParameters = function() {
        self.query = new RouteItem('filter', filterHashParameterHelper.emptyValue);
        self.page = new RouteItem('page', '1');
        self.columns = new RouteItem('columns', window.consts.grid.initialFieldSet);
        self.order = new RouteItem('order', 'none');
    };

    self.needResetFiltration = function (field) {
        if (field == window.consts.questionCourseName) {
            return true;
        }
        return false;
    };

    self.deleteFiltration = function (field) {
        var isFiltrationChenged = filterHashParameterHelper.isFiltrationChenged(field, null, self.query.getValue(), true);
        self.query.setValue(filterHashParameterHelper.deleteFiltration(field, self.query.getValue()));
        
        if (isFiltrationChenged) {
            self.page.setValue(1);
        } else {
            self.needRerender.setValue('false');
        }

        hasher.setHash(self.buildHash());
        self.needRerender.setValue('true');
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
        if (fieldName != null) {
            self.order.setValue(self.order.getValue() + '/' + fieldName);
        }
    };

    return self;
}());

 