function RouteItem(key, value) {
    if (!(this instanceof RouteItem)) {
        return new RouteItem(key, value);
    }

    this.key=key;
    this.value=value;
};

RouteItem.prototype.getRoute = function () {
    return this.key + '/' + this.value;
};

RouteItem.prototype.setValue = function(value) {
    this.value=value;
};

RouteItem.prototype.getValue = function() {
    return this.value;
};
