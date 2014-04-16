/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationSetStatus = React.createClass({displayName: 'QuestionBulkOperationSetStatus',

    cancelHandler: function() {
       alert('cancel');
    },

    selectHandler: function(value) {
        debugger;
        questionDataManager.bulk.setStatus(this.props.selectedQuestions, value)
    },

    render: function() {
        return ( 
            React.DOM.div(null,  
            SingleSelectButton( {caption:"Set status to..",  cancelHandler:this.cancelHandler, selectHandler:this.selectHandler, values:this.props.availableStatuses} )
            )
            );
        }
});