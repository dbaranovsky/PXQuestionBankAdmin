/**
* @jsx React.DOM
*/

var QuestionBox = React.createClass({

	getInitialState: function() {
		return { data:[] };
	},

	loadQuestionFromServer: function() {
		 $.ajax({
		 	url: this.props.url,
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
				<QuestionList data={this.state.data}/> 
			</div>
			);
	}
});