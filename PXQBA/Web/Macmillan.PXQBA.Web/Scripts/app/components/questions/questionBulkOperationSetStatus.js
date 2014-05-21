/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationSetStatus = React.createClass({displayName: 'QuestionBulkOperationSetStatus',

    cancelHandler: function() {
    },

    selectHandler: function(value) {
        questionDataManager.bulk.updateMetadataField(this.props.selectedQuestions, "questionstatus", value);
    },

    render: function() {
        return ( 
            React.DOM.div(null,  
            SingleSelectButton( {caption:"Set status to..",  cancelHandler:this.cancelHandler, selectHandler:this.selectHandler, values:this.props.availableStatuses} )
            )
            );
        }
});
