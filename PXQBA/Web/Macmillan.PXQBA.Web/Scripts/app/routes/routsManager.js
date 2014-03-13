var routsManager = (function () {
    var self = {};
     
    // Stub
    self.query = 'filter/query';

    self.setPage= function (page) {
        hasher.setHash(this.query +'/page/' + page);
    };

    return self;
}());