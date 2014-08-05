/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationSetChapter = React.createClass({displayName: 'QuestionBulkOperationSetChapter',

    cancelHandler: function() {
    },

    selectHandler: function(value) {
        debugger;
       questionDataManager.bulk.updateMetadataField(this.props.currentCourseId, this.props.selectedQuestions, window.consts.questionChapterName, value); 
    },

    render: function() {
        return ( 
            React.DOM.div(null,  
            SingleSelectButton( {caption:"Change Chapter to..",  cancelHandler:this.cancelHandler, selectHandler:this.selectHandler, values:this.props.availableStatuses} )
            )
            );
        }
});
 