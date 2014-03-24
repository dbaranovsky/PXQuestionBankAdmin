crossroads.addRoute('/filter/{query}/page/{page}/columns/{columns}/order/{orderType}/:orderField:',
    function (query, page, columns, orderType, orderField) {
        var questionsData = questionDataManager.getQuestionsByQuery(query,
                                                                    page,
                                                                    columnHashParameterHelper.parse(columns),
                                                                    orderType,
                                                                    orderField);
        questionsData.done(function (response) {
                    React.renderComponent(
                        QuestionListPage({
                            data: response.questionList,
                            currentPage: response.pageNumber,
                            totalPages: response.totalPages,
                            order: response.order,
                            columns: response.columns
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
