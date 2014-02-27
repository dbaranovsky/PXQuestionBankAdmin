
describe('Test Html Sanitizer: ', function() {
    
    beforeEach(function() {
        
    });
    
    it("Expect success", function () {
        var expectedResult = '<img>';
        var actualResult = html_sanitize(expectedResult);
        expect(actualResult).toEqual(expectedResult);

    });
    describe('Test html entity', function () {
        it("If html that vaild entity , expect success", function () {
            var expectedResult = '<div>&nbsp;</div>';
            var actualResult = html_sanitize(expectedResult);
            expect(actualResult).toEqual(expectedResult);

        });

    });
    describe('Test html tag', function () {
        it("If html that has no end tag , expect success", function () {
            var expectedResult = '<div><img><div></div></div>';
            var actualResult = html_sanitize(expectedResult);
            expect(actualResult).toEqual(expectedResult);

        });
        it("If contains unsupported html tag , expect to throw exception", function() {
            var expectedResult = '<frame></frame>';
            var actualResult = function() { html_sanitize(expectedResult); };
            expect(actualResult).toThrow("Cannot contain unsupported html tag");

        });
    });
    describe('Test attribute', function () {
        it("With valid attributes, Expect success", function () {
            var expectedResult = '<img style="display:none" src="http://ebooks.bfwpub.com/cowentabarrokecon2/pics/ch13_varrow.jpg">';
            var actualResult = html_sanitize(expectedResult);
            expect(actualResult).toEqual(expectedResult);

        });
        it("If contains id attribute, expect to throw exception", function () {
            var expectedResult = '<img id="test"></img>';
            var actualResult = function () { html_sanitize(expectedResult); };
            expect(actualResult).toThrow("Cannot contain unsupported html attribute");

        });
        it("If contains script type attribute, expect to throw exception", function () {
            var expectedResult = '<img onClick="alert();"></img>';
            var actualResult = function () { html_sanitize(expectedResult); };
            expect(actualResult).toThrow("Cannot contain unsupported html attribute");

        });
        it("If contains class attribute, expect to throw exception", function () {
            var expectedResult = '<img class="test"></img>';
            var actualResult = function () { html_sanitize(expectedResult); };
            expect(actualResult).toThrow("Cannot contain unsupported html attribute");

        });
        it("1, If contains unsupported html attribute, expect to throw exception", function() {
            var expectedResult = '<img disable="disabled"></img>';
            var actualResult = function() { html_sanitize(expectedResult); };
            expect(actualResult).toThrow("Cannot contain unsupported html attribute");

        });

        it("2, If contains unsupported html attribute, expect to throw exception", function() {
            var expectedResult = '<img style="display:none;" disabled="disabled"></img>';
            var actualResult = function() { html_sanitize(expectedResult); };
            expect(actualResult).toThrow("Cannot contain unsupported html attribute");

        });
    });
});