/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',
		render: function() {
		return ( 
			React.DOM.div( {className:"preview-collapsed question-preview"}, 
		  React.DOM.div( {dangerouslySetInnerHTML:{__html: this.props.preview}} )
			)

			);
		}
});