filterHashParameterHelper = (function () {
    self.emptyValue = '_';
    var emptyValue = self.emptyValue;
    var fieldsSeparator = '+';
    var valuesSeparator = '|';
    var fieldValueSeparator = ':';

    self.parse = function (filterUrlHash) {
        if (filterUrlHash == emptyValue) {
            return null;
        }
        var fieldFilters = filterUrlHash.split(fieldsSeparator);
        var filterItems = [];
        for (var i = 0; i < fieldFilters.length; i++) {
            filterItems.push(new FilterItem(fieldFilters[i], valuesSeparator, fieldValueSeparator));
        }
        return filterItems;
    };

    self.toUrlHash = function(filterItems) {
        var stringItems = [];
        for (var i = 0; i < filterItems.length; i++) {
            stringItems.push(filterItems[i].toUrlString());
        }

        return stringItems.join(fieldsSeparator);
    };

    self.addFiltrationToArray = function (filterItem, filtrationArray) {
        for (var i = 0; i < filtrationArray.length; i++) {
            if (filtrationArray[i].field == filterItem.field) {
                filtrationArray[i] = filterItem;
                return;
            }
        }
        
        filtrationArray.push(filterItem);
    };

    self.deleteFiltrationFromArray = function (field, filtrationArray) {
        var fieldIndex = -1;
        for (var i = 0; i < filtrationArray.length; i++) {
            if (filtrationArray[i].field == field) {
                fieldIndex = i;
                break;
            }
        }
        if (fieldIndex != -1) {
            filtrationArray.splice(fieldIndex, 1);
        }
    };

    self.addFiltration = function (field, valuesArray, filtrationUrlParameters) {
        var filterItems = self.parse(filtrationUrlParameters);
        if (filterItems == null) {
            filterItems = [];
        }
        var filterItem = new FilterItem(field + fieldValueSeparator, valuesSeparator, fieldValueSeparator);
        filterItem.values = self.removeDangerousCharacters(valuesArray);
        addFiltrationToArray(filterItem, filterItems);
        return self.toUrlHash(filterItems);
    };

    self.deleteFiltration = function(field, filtrationUrlParameters) {
        var filterItems = self.parse(filtrationUrlParameters);
        if (filterItems == null) {
            return self.emptyValue;
        }
        self.deleteFiltrationFromArray(field, filterItems);
        if (filterItems.length == 0) {
            return self.emptyValue;
        }
        return self.toUrlHash(filterItems);
    };

    self.removeDangerousCharacters = function(values) {
        if (values == null) {
            return values;
        }
        
        for (var i = 0; i < values.length; i++) {
            values[i] = self.replaceSeporators(values[i]);
        }

        return values;
    };

    self.replaceSeporators = function (value) {
        return value.replace(/\+/g, '').replace(/:/g, '').replace(/\|/g, '');
    };

    return self;
}());

