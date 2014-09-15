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


    self.isFiltrationChenged = function(field, valuesArray, filtrationUrlParameters, isDelete) {
        var filterItems = self.parse(filtrationUrlParameters);
        if (filterItems == null) {
            filterItems = [];
        }
        var filterItem = new FilterItem(field + fieldValueSeparator, valuesSeparator, fieldValueSeparator);
        filterItem.values = self.removeDangerousCharacters(valuesArray);
        
        var currentFilterItem = self.getFiltration(filterItem.field, filterItems);
        var currentValues = [];
        if (currentFilterItem != null) {
            currentValues = currentFilterItem.values;
        }
        
        if (currentValues.length==0) {
            if (isDelete) {
                return false;
            } else {
                if (currentFilterItem == null) {
                    return false;
                }
            }
            
            if (filterItem.values == null) {
                return false;
            }
            
            if (filterItem.values.length==0) {
                return false;
            }
        }

        return true;
    };
    

    self.getFiltration = function(fieldName, filtrationArray) {
        for (var i = 0; i < filtrationArray.length; i++) {
            if (filtrationArray[i].field == fieldName) {
                return filtrationArray[i];
            }
        }

        return null;
    };

    self.addFiltrationToArray = function (filterItem, filtrationArray) {
        var currentFiltration = self.getFiltration(filterItem.field, filtrationArray);
        if (currentFiltration != null) {
            currentFiltration.values = filterItem.values;
        } else {
            filtrationArray.push(filterItem);
        }
        
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

