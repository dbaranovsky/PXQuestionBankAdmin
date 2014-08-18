/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationSetBank = React.createClass({displayName: 'QuestionBulkOperationSetBank',

    cancelHandler: function() {
    },

    selectHandler: function(value) {
       questionDataManager.bulk.updateMetadataField(this.props.currentCourseId,this.props.selectedQuestions, window.consts.questionBankName, value); 
    },

    render: function() {
        return ( 
            React.DOM.div(null, 
            SingleSelectButton({caption: "Change Bank to..", cancelHandler: this.cancelHandler, selectHandler: this.selectHandler, values: this.props.availableStatuses})
            )
            );
        }
});
 