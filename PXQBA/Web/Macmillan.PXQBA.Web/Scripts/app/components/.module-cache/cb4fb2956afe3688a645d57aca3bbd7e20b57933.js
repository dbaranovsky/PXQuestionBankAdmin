/**
* @jsx React.DOM
*/

var QuestionFilter = React.createClass({displayName: 'QuestionFilter',
	render: function() {
		return (
			React.DOM.div( {className:"question-filter"}, 
				 React.DOM.div(null,  
				 	React.DOM.span(null, 
				 		 React.DOM.strong(null,  " Filter: " ) 
				    ),
				    React.DOM.span(null,  " Favourite filter")
				)
			)
			);
		}
});