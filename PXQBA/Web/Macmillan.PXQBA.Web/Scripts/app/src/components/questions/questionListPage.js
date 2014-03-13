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
					<QuestionPaginator metadata={{
						    currentPage: this.props.currentPage,
						    totalPages: this.props.totalPages}} />
				</div> 
			</div> 
			);
	}
});

