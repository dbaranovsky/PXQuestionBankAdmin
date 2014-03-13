/**
* @jsx React.DOM
*/

var QuestionFilter = React.createClass({
	render: function() {
		return (
			<div className="question-filter">
				 <div> 
				 	<span>
				 		 <strong> Filter: </strong> 
				    </span>
				    <a href="#/filter/query/page/1"> Favourite filter</a>
				</div>
			</div>
			);
		}
});