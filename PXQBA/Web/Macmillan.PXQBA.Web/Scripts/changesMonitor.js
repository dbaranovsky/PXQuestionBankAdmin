
function monitorChanges(item, stopWatching) {
    if (!item) {
        return;
    }

    if (!(Object.prototype.toString.call(window.monitoringItems) === '[object Array]')) {
        window.monitoringItems = [];
    }

    if (stopWatching) {
        var index = -1;
        $(window.monitoringItems).each(function (i) {
            if (this['item'] == item) {
                index = i;
            }
        });

        if (index >= 0) {
            window.monitoringItems.splice(index, 1);
        }
        return;
    }

    var monitorable = {'item':item,'hasChanges':false};
    window.monitoringItems.push(monitorable);

    $(item).one('keypress', function() {
        monitorable['hasChanges'] = true;
    });

     $(item).on('change', 'input', function(){
        monitorable['hasChanges'] = true;

     });
     $(item).on('change', 'textarea', function(){
        monitorable['hasChanges'] = true;
        
     });

      $(item).on('click', '.chosen-container', function(){
        monitorable['hasChanges'] = true;
        
     });


}

function resetMonitoring(item) {
    if (!item) {
        return;
    }

    if (!(Object.prototype.toString.call(window.monitoringItems) === '[object Array]')) {
        window.monitoringItems = [];
    }
    var index = -1;
    $(window.monitoringItems).each(function (i) {
        if (this['item']==item) {
            index = i;
            return;
        }
    });

    if (index >= 0) {
        var monitorable = window.monitoringItems[index];
        monitorable['hasChanges'] = false;
        $(monitorable['item']).one('keypress', function () {
            monitorable['hasChanges'] = true;
        });
    }
}

function hasUnsavedData() {
    if (!window.monitoringItems || window.monitoringItems.length == 0) {
        return false;
    }

    var hasChanges = false;
    $(window.monitoringItems).each(function () {
        if (this['hasChanges']) {
            hasChanges = true;
            return;
        }
    });

    return hasChanges;
}

$( document ).ready(function(){
    $(window).bind('beforeunload', function () {
        if (hasUnsavedData()) {
            return "You have unsaved changes on this page. Do you want to leave this page and discard your changes or stay on this page?";
        }
    });

});

 