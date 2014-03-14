/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',
        componentDidMount: function() {
            $(this.getDOMNode()).html(this.props.preview);
        },
        render: function() {
            return ( 
                React.DOM.div( {className:"preview-collapsed question-preview"}
                )
            );
        }
});