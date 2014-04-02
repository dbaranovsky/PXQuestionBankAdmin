crossroads.addRoute('/filter/{query}/page/{page}/columns/{columns}/order/{orderType}/:orderField:',
    function (query, page, columns, orderType, orderField) {
        console.log('route callback executed');
        asyncManager.startWait();
        var questionsData = questionDataManager.getQuestionsByQuery(query,
                                                                    page,
                                                                    columnHashParameterHelper.parse(columns),
                                                                    orderType,
                                                                    orderField);
        questionsData.done(function(response) {
            asyncManager.questionListPage = React.renderComponent(
                QuestionListPage({
                    response: response
                }, " "),
                $('#question-container')[0]);
             })
            .fail(function(error) {
                console.error("getQuestionsBy:", error);
            })
            .always(function() {
                asyncManager.stopWait();
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

