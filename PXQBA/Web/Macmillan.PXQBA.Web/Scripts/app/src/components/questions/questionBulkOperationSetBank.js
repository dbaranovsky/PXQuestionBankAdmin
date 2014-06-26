/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationSetBank = React.createClass({

    cancelHandler: function() {
    },

    selectHandler: function(value) {
       questionDataManager.bulk.updateMetadataField(this.props.selectedQuestions, window.consts.questionBankName, value); 
    },

    render: function() {
        return ( 
            <div> 
            <SingleSelectButton caption='Change Bank to..'  cancelHandler={this.cancelHandler} selectHandler={this.selectHandler} values={this.props.availableStatuses} />
            </div>
            );
        }
});
 