/**
* @jsx React.DOM
*/ 
crossroads.addRoute('/filter/{query}/page/{page}', function (query, page) {
    var questionsData = questionDataManager.getQuestionsByQuery(query, page);
    questionsData.done(function (response) {
        React.renderComponent(
            QuestionListPage({ data: response.data, currentPage: response.pageNumber, totalPages: response.totalPages }, " "),
            $('#question-container')[0]);
    }).fail(function () {
        console.error("getQuestionsBy:", error);
    });
});

crossroads.addRoute('', function() {
	React.renderComponent(
        QuestionListPage({ data: [], currentPage: 1, totalPages: 1 }, " "),
        $('#question-container')[0]);
});

//setup hasher
function parseHash(newHash, oldHash) {
    crossroads.parse(newHash);
}
hasher.initialized.add(parseHash); //parse initial hash
hasher.changed.add(parseHash); //parse hash changes
hasher.init(); //start listening for history change
