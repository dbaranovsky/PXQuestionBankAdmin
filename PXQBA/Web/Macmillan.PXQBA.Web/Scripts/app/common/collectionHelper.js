var collectionHelper = (function () {
 
    self.changeCollection = function(item, collection, isInsert) {
        var index = $.inArray(item, collection);
        if (isInsert) {
            if (index == -1) {
                collection.push(item);
            }
        } else {
            if (index != -1) {
                collection.splice(index, 1);
            }
        }
        return collection;
    };

    self.isItemInCollection = function(item, collection) {
        var index = $.inArray(item, collection);
        if (index == -1) {
            return false;
        }
        return true;
    };

    return self;
}());