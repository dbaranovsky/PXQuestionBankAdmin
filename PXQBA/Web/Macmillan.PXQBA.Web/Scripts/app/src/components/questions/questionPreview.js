/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({
		render: function() {
		return ( 
			<div className="preview-collapsed question-preview">
		  <div dangerouslySetInnerHTML={{__html: this.props.preview}} />
			</div>

			);
		}
});