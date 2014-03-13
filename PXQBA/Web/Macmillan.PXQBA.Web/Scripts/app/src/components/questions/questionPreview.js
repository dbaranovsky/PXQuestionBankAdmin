/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({
	render: function() {
		return ( 
			<div className="preview-collapsed question-preview">
			Test render:  {this.props.preview}
			</div>
			);
		}
});