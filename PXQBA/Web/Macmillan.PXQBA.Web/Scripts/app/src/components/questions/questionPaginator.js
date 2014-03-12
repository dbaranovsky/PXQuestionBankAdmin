/**
* @jsx React.DOM
*/ 

var QuestionPaginator = React.createClass({

	componentDidMount: function() {
         var options = {
            currentPage: 1,
            totalPages: 10,
            onPageChanged: function(event, oldPage, newPage) {
            	//ToDo: Implement 
            }
        }

        $('#question-paginator').bootstrapPaginator(options);
	},

	render: function() {
		return ( 
			   <div className="questionPaginator" id="question-paginator"> 
			   </div>
			);
		}
});