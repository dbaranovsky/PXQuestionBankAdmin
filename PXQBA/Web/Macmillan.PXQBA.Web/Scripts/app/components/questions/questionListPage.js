/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({displayName: 'QuestionListPage',

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
			React.DOM.div( {className:"questionBox"}, 
			React.DOM.div(null,  
			     QuestionFilter(null)
			),
				QuestionList( {data:this.state.data})
			)
			);
	}
});
React.renderComponent(
        QuestionListPage(null,  " " ),
        $('#question-container')[0]);