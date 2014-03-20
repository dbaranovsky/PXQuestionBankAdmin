/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',

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
                React.DOM.div( {className:"preview-collapsed question-preview"}
                )
            );
        }
});