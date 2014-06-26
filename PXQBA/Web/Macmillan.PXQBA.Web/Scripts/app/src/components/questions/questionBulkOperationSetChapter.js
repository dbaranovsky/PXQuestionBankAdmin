/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationSetChapter = React.createClass({

    cancelHandler: function() {
    },

    selectHandler: function(value) {
       questionDataManager.bulk.updateMetadataField(this.props.selectedQuestions, window.consts.questionChapterName, value); 
    },

    render: function() {
        return ( 
            <div> 
            <SingleSelectButton caption='Change Chapter to..'  cancelHandler={this.cancelHandler} selectHandler={this.selectHandler} values={this.props.availableStatuses} />
            </div>
            );
        }
});
 