/**
* @jsx React.DOM
*/ 

var QuestionCell = React.createClass({displayName: 'QuestionCell',
    render: function() {
        return ( 
                React.DOM.td(null, 
                    this.props.value
                )
            );
        }
});