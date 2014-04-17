function FilterItem(stringValue, valuesSeparator, fieldValueSeparator) {
    if (!(this instanceof FilterItem)) {
        return new FilterItem(stringValue, valuesSeparator, fieldValueSeparator);
    }

    this.valuesSeparator = valuesSeparator;
    this.fieldValueSeparator = fieldValueSeparator;

    this.parseItem(stringValue, valuesSeparator, fieldValueSeparator);
};

FilterItem.prototype.toUrlString = function () {
    return this.field + this.fieldValueSeparator + this.values.join(this.valuesSeparator);
};

FilterItem.prototype.parseItem = function (stringValue, valuesSeparator, fieldValueSeparator) {
    var splitedObject = stringValue.split(fieldValueSeparator);
    this.field = splitedObject[0];
    this.parseValues(splitedObject[1], valuesSeparator);
};

FilterItem.prototype.parseValues = function (stringValues, valuesSeparator) {
    var values = stringValues.split(valuesSeparator);
    this.values = values;
};