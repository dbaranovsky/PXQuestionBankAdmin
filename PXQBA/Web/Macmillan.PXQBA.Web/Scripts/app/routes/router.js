crossroads.addRoute('/filter/{query}/page/{page}/order/{orderType}/:orderField:', function (query, page, orderType, orderField) {
    var questionsData = questionDataManager.getQuestionsByQuery(query, page, orderType, orderField);
    questionsData.done(function (response) {
        React.renderComponent(
            QuestionListPage({
                data: response.questionList,
                currentPage: response.pageNumber,
                totalPages: response.totalPages,
                order: response.order
            }, " "),
            $('#question-container')[0]);
    }).fail(function () {
        console.error("getQuestionsBy:", error);
    });
});

crossroads.addRoute('', function() {
    hasher.setHash(window.routsManager.buildHash());
});

//setup hasher
function parseHash(newHash, oldHash) {
    crossroads.parse(newHash);
}
hasher.initialized.add(parseHash); //parse initial hash
hasher.changed.add(parseHash); //parse hash changes
hasher.init(); //start listening for history change
