/// <reference path="../../jquery/jquery-1.6.4.js" />
/// <reference path="../../HighlightWidget/highlighter.js" />

describe("Highlighter", function () {
    beforeEach(function () {
        if ($("body").length === 0) {
            $(document).append("body");
        }

        $("body").append('<div id="html-example-root" />');
    });

    afterEach(function () {
        $("div#html-example-root").remove();
    });

    it("can highlight", function () {

        //arrange
        var loaded = false;
        var contentWindow;
        var contentDocument;
        var tragetFrame;

        // this will run asynchronously to load the document into the iframe
        runs(function () {
            var frame = $('<iframe id="hframe" />');
            frame.attr("src", "./html/Highlighter/ExampleDocument.html");
            frame.load(function () {                
                targetFrame = frame[0];
                contentWindow = targetFrame.contentWindow;
                contentDocument = targetFrame.contentDocument;
                contentWindow.document = contentDocument;

                // this will clue jasmine into the fact that the waitFor can terminate early
                loaded = true;
            });
            $("div#html-example-root").append(frame);
        });

        // this will wait up to 10 seconds, or continue if loaded returns true
        waitsFor(function () {
            return loaded;
        }, "iframe did not load", 10000);

        // It may not seem obvious, but even though the following code does not need to
        // be asynchronous, the way jasmine works is kind of like a queue. 
        // 
        // First we ran a task, then we told it to wait up to 10 seconds, then we are now
        // telling it to run another task.
        //
        // If we had not wrapped this code in "runs" then it would execute immediately 
        // (i.e. before waitFor is done) and we'd be seeing a failing test.
        runs(function () {            
            //act
            window.Highlighter.Highlight(
                [{
                    id: 1234,
                    rangeDetail: {
                        start: '/div[1]/text()[1]',
                        startOffset: 2,
                        end: '/div[1]/p[1]/text()[1]',
                        endOffset: 4
                    },
                    css: { color: "red" }
                }],
                targetFrame
            );

            //assert            
            var highlight = $(contentDocument).find("#highlight-1234");
            expect(highlight.length).toEqual(1);
        });
    });
});