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
				<div> 
					<QuestionList data={this.props.data}/>
				</div> 
				<div> 
					<QuestionPaginator/>
				</div> 
			</div> 
			);
	}
});

