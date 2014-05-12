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


    getAvailableStatuses: function() {
      var availableStatuses = [];
        for(var i=0; i<this.props.columns.length; i++) {
            if(this.props.columns[i].metadataName==window.consts.questionStatusName) {
                availableStatuses=this.props.columns[i].editorDescriptor.availableChoice;
                break;
            }
        }
        return availableStatuses;
    },


    bulkRemoveFromTitle: function(){
       questionDataManager.bulk.removeTitle(this.props.selectedQuestions);
    },

    bulkShareToTitle: function(){
      this.props.bulkShareHandler(this.props.selectedQuestions);
    },

    getTextMessage: function() {
        var count = this.getSelectedQuestionCount();
        if(count==1) {
          return "1 question selected";
        }
        else {
          return "Bulk action ( " + count + " questions selected ):";
        }
    },

    render: function() {
        return ( 
                  React.DOM.tr(null, 
                    React.DOM.td( {colSpan:this.props.colSpan, className:"bulk-operation-bar"}, 
                        React.DOM.table( {className:"bulk-operation-bar-table"}, 
                          React.DOM.tr(null, 
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                              React.DOM.div( {className:"bulk-operation-item"}, 
                                 React.DOM.span(null,  " ", this.getTextMessage(),  "  "  )
                               )
                            ),
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                               React.DOM.div( {className:"bulk-operation-item", 'data-toggle':"tooltip", title:"Change all selected questions"}, 
                                QuestionBulkOperationSetStatus( {availableStatuses:this.getAvailableStatuses(), 
                                                              selectedQuestions:this.props.selectedQuestions}) 
                               )
                            ),
                            React.DOM.td( {className:"bulk-operation-sharing"}, 
                              React.DOM.button( {type:"button", className:"btn btn-default", onClick:this.bulkShareToTitle}, "Share with another title"),
                              React.DOM.button( {type:"button", className:"btn btn-default", onClick:this.bulkRemoveFromTitle}, "Remove from this title")
                            ),
                            React.DOM.td(null, 
                               React.DOM.div( {className:"deselect-button", onClick:this.deselectsAllHandler, 'data-toggle':"tooltip", title:"Deselect all"}, 
                                 React.DOM.span(null ,  " X " )
                               )
                            )
                          )
                        )
                    )
                  )
            );
        }
});