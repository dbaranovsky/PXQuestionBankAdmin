/**
* @jsx React.DOM
*/ 

var QuestionPaginator = React.createClass({displayName: 'QuestionPaginator',
	render: function() {
		return ( 
			   React.DOM.div( {className:"questionPaginator"},  
			   		React.DOM.span(null, "Paging")
			   )
			);
		}
});