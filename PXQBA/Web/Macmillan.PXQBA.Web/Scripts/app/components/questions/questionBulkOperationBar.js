/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBar = React.createClass({displayName: 'QuestionBulkOperationBar',

    deselectsAllHandler: function() {
        this.props.deselectsAllHandler();
    },

    getSelectedQuestionCount: function() {
        return this.props.selectedQuestions.length;
    },

    render: function() {
        return ( 
                  React.DOM.tr(null, 
                    React.DOM.td( {colSpan:this.props.colSpan, className:"bulk-operation-bar"}, 

                          React.DOM.span(null,  " ", this.getSelectedQuestionCount(), " questions selectd " ),
                          React.DOM.span( {className:"deselect-button", onClick:this.deselectsAllHandler},  " X " )
                    )
                  )
            );
        }
});