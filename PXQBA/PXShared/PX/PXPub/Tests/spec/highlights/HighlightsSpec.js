xdescribe('PxComments', function() {
    beforeEach(function () {
        loadFixtures('highlights.html');
        
        jasmine.Ajax.useMock();

    });

    afterEach(function() {
        //TODO:
    });

    describe('can get note data from comment block', function() {
        it('when the highlight block is inactive', function () {
            var data = PxComments.GetNoteDataFromBlock('#highlight-block-0b071e49-3f04-49a4-a949-23ebe630342f');
            expect(data.highlightId).toEqual('0b071e49-3f04-49a4-a949-23ebe630342f');
            expect(data.highlightType).toEqual('1');
            expect(data.highlightBlockId).toEqual('highlight-block-0b071e49-3f04-49a4-a949-23ebe630342f');
        });
        it('when the highlight block is brand new', function () {
            var data = PxComments.GetNoteDataFromBlock('#highlight-block-0');
            expect(data.highlightId).toEqual('0');
            expect(data.highlightType).toEqual('1');
            expect(data.highlightBlockId).toEqual('highlight-block-0');
        });
        it('when the highlight block is active', function () {
            var data = PxComments.GetNoteDataFromBlock('#highlight-block-0b071e49-3f04-49a4-a949-23ebe630342f');
            expect(data.highlightId).toEqual('0b071e49-3f04-49a4-a949-23ebe630342f');
            expect(data.highlightType).toEqual('1');
            expect(data.highlightBlockId).toEqual('highlight-block-0b071e49-3f04-49a4-a949-23ebe630342f');
        });
    });
    describe('saving notes', function () {

        
        it('can call add note when the note is new', function () {
            spyOn(PxComments, 'AddNote');
            spyOn(PxComments, 'GetNoteDataFromBlock').andReturn({
                noteId: ''
            })
            PxComments.SaveNoteBlock('');
            expect(PxComments.AddNote).toHaveBeenCalled();
        });

        it('can call update note when the note already exists', function() {
            spyOn(PxComments, 'UpdateNote');
            spyOn(PxComments, 'GetNoteDataFromBlock').andReturn({
                noteId: 'blockid'
            })
            PxComments.SaveNoteBlock('');
            expect(PxComments.UpdateNote).toHaveBeenCalled();
        });

        it('can get highlight count', function() {
            //TODO:
        });
    });
});