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

    self.addFiltration = function (field, valuesArray, filtrationUrlParameters) {
        var filterItems = self.parse(filtrationUrlParameters);
        var filterItem = new FilterItem(field + fieldValueSeparator, valuesSeparator, fieldValueSeparator);
        filterItem.values = valuesArray;
        addFiltrationToArray(filterItem, filterItems);
        return self.toUrlHash(filterItems);
    };

    return self;
}());

