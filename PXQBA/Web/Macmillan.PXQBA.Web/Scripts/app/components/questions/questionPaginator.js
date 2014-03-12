/**
* @jsx React.DOM
*/ 

var QuestionPaginator = React.createClass({displayName: 'QuestionPaginator',

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
			   React.DOM.div( {className:"questionPaginator", id:"question-paginator"} 
			   )
			);
		}
});