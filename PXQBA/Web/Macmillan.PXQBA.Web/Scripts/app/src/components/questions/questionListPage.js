/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

	render: function() {
		return (
			<div className="questionBox">
			<div> 
			     <QuestionFilter/>
			</div>
				<QuestionList data={this.props.data}/>
			</div>
			);
	}
});

