var questionContainer = $('#question-container')[0];

crossroads.addRoute('/page/{page}/columns/{columns}/filter/{query}/order/{orderType}/:orderField:',
    function (page, columns, query, orderType, orderField) {
        console.log('route callback executed');
        routsManager.setState(query, page, columns, orderType, orderField);
        asyncManager.startWait(questionContainer);
        var questionsData = questionDataManager.getQuestionsByQuery(filterHashParameterHelper.parse(query),
                                                                    page,
                                                                    columnHashParameterHelper.parse(columns),
                                                                    orderType,
                                                                    orderField);
        questionsData.done(function (response) {
            asyncManager.page = React.renderComponent(
                QuestionListPage({
                    response: response
                }, " "),
                questionContainer);
             })
            .fail(function(error) {
                console.error("getQuestionsBy:", error);
            })
            .always(function() {
                asyncManager.stopWait();
            });
    });

crossroads.addRoute('', function () {
    asyncManager.startWait(questionContainer);
    hasher.setHash(window.routsManager.buildHash());
});

//setup hasher
function parseHash(newHash, oldHash) {
    crossroads.parse(newHash);
}
hasher.initialized.add(parseHash); //parse initial hash
hasher.changed.add(parseHash); //parse hash changes
hasher.init(); //start listening for history change

