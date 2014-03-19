/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({
        componentDidMount: function() {
            $(this.getDOMNode()).html(this.props.preview);
        },

        componentDidUpdate: function () {
        	$(this.getDOMNode()).html(this.props.preview);
    	},

        render: function() {
            return ( 
                <div className="preview-collapsed question-preview">
                </div>
            );
        }
});