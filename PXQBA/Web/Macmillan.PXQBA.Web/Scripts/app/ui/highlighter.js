var highlighter = (function () {

    self.target = null;
    self.filter = null;
    self.text = null;

    self.getItems = function () {
        debugger;
        if (self.filter == null) {
            return self.target;
        }
        return self.target.find(filter);
    };

    self.highlight = function (text) {
        self.target.removeHighlight();
        self.text = text;
        self.highlightWords(self.getItems(), text);
    };

    self.unhighlight = function() {
        self.target.removeHighlight();
    };

    self.refresh = function() {
        self.target.removeHighlight();
        self.highlightWords(self.getItems(), text);
    };

    self.highlightWords = function(items, text) {
        var words = text.split(' ');
        for (var i = 0 ; i<words.length; i++) {
            if (words[i] != "") {
                items.highlight(words[i]);
            }
        }
    };

    return self;
}());