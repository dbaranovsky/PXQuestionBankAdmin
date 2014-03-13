/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',
	render: function() {
		return ( 
			React.DOM.div( {className:"preview-collapsed question-preview"}, 
			"Test render:  ",  this.props.preview
			)
			);
		}
});