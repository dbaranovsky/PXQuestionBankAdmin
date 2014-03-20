/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

        loadPreview: function (container) {
            container.html(this.props.preview);
        },

        componentDidMount: function() {
            this.loadPreview($(this.getDOMNode()))
        },

        componentDidUpdate: function () {
            this.loadPreview($(this.getDOMNode()))
    	},

        render: function() {
            return ( 
                <div className="preview-collapsed question-preview">
                </div>
            );
        }
});