/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({displayName: 'QuestionListPage',

	render: function() {
		return (
			React.DOM.div( {className:"questionBox"}, 
				React.DOM.div(null,  
				     QuestionFilter(null)
				),
				React.DOM.div(null,  
					QuestionList( {data:this.props.data})
				), 
				React.DOM.div(null,  
					QuestionPaginator(null)
				) 
			) 
			);
	}
});

