/**
* @jsx React.DOM
*/ 
crossroads.addRoute('/filter/:{?query}:', function(query) {
	var questionsData = questionDataManager.getQuestionsByQuery(query);
    questionsData.done(function(data) {
        React.renderComponent(
            QuestionListPage({ data: data }, " "),
            $('#question-container')[0]);
    }).fail(function() {
            console.error("getQuestionsBy:", error);
    });
});

crossroads.addRoute('', function() {
	React.renderComponent(
        <QuestionListPage data={[]}> </QuestionListPage>,
        $('#question-container')[0]);
});

//setup hasher
function parseHash(newHash, oldHash) {
    crossroads.parse(newHash);
}
hasher.initialized.add(parseHash); //parse initial hash
hasher.changed.add(parseHash); //parse hash changes
hasher.init(); //start listening for history change
