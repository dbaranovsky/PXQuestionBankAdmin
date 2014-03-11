var route1 = crossroads.addRoute('/news/{id}', function(id) {
    console.log(id);
});
//crossroads.parse('/news/123');
crossroads.routed.add(console.log, console);
