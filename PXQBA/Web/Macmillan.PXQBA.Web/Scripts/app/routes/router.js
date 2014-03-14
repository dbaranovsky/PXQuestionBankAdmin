/**
* @jsx React.DOM
*/ 
crossroads.addRoute('/filter/{query}/page/{page}', function (query, page) {
    var questionsData = questionDataManager.getQuestionsByQuery(query, page);
    questionsData.done(function (response) {
        React.renderComponent(
            QuestionListPage({ data: response.questionList, currentPage: response.pageNumber, totalPages: response.totalPages }, " "),
            $('#question-container')[0]);
    }).fail(function () {
        console.error("getQuestionsBy:", error);
    });
});

crossroads.addRoute('', function() {
    hasher.setHash('filter/query/page/1');
});

//setup hasher
function parseHash(newHash, oldHash) {
    crossroads.parse(newHash);
}
hasher.initialized.add(parseHash); //parse initial hash
hasher.changed.add(parseHash); //parse hash changes
hasher.init(); //start listening for history change
