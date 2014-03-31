var columnHashParameterHelper = (function () {
    var separator = '+';
    var self = {};

    self.parse = function(columnUrlHash) {
        return columnUrlHash.split(separator);
    };

    self.toUrlHash = function(columns) {
        return columns.join(separator);
    };

    self.addToArray = function (columns, field) {
        if ($.inArray(field, columns) == -1) {
            columns.push(field);
        }
        return columns;
    };

    self.deleteFromArray = function (columns, field) {
        var index = $.inArray(field, columns);
        if (index != -1) {
            columns.splice(index, 1);
        }
        return columns;
    };

    self.addField = function (field, columnsUrlParameters) {
        var columns = self.parse(columnsUrlParameters);
        columns = self.addToArray(columns, field);
        return self.toUrlHash(columns);
    };

    self.deleteField = function (field, columnsUrlParameters) {
        var columns = self.parse(columnsUrlParameters);
        columns = self.deleteFromArray(columns, field);
        return self.toUrlHash(columns);
    };

    self.isLast = function(columnsUrlParameters) {
        var columns = self.parse(columnsUrlParameters);
        return columns.length < 2;
    };

    return self;
}());