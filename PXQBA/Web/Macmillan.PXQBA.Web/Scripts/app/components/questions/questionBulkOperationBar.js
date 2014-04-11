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
                          React.DOM.div( {className:"bulk-operation-item"}, 
                               React.DOM.span(null,  " ", this.getSelectedQuestionCount(), " questions selected")
                          ),
                          React.DOM.div( {className:"bulk-operation-item"}
                              
                          ),
                          React.DOM.div( {className:"deselect-button", onClick:this.deselectsAllHandler}, 
                                React.DOM.span(null ,  " X " )
                          )
                    )
                  )
            );
        }
});