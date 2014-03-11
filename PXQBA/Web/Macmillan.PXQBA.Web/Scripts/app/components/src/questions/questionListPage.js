/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

	getInitialState: function() {
		return { data:[] };
	},

	loadQuestionFromServer: function() {
		 $.ajax({
		 	url: window.actions.questionList.getQuestionListUrl,
		 	dataType: 'json',
		 	type: 'POST'
		 	})
		 .done(
		 	function(data) {
		 		this.setState({ data: data });
		 		}.bind(this))
		 .fail(
		 	function(xhr, status, error) {
		 	console.error("loadQuestionFromServer:", error);
		 	}.bind(this));

 
	},

	componentWillMount: function() {
		this.loadQuestionFromServer();
	},

	render: function() {
		return (
			<div className="questionBox">
			<div> 
			     <QuestionFilter/>
			</div>
				<QuestionList data={this.state.data}/>
			</div>
			);
	}
});
React.renderComponent(
        <QuestionListPage> </QuestionListPage>,
        $('#question-container')[0]);