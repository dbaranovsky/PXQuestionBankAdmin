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
        return this.getAvailableValues(window.consts.questionStatusName);
    },

    getAvailableChapters: function() {
        return this.getAvailableValues(window.consts.questionChapterName);
    },

    getAvailableBanks: function() {
        return this.getAvailableValues(window.consts.questionBankName);
    },

    getAvailableValues: function(metadataName) {
      var availableValues = [];
        for(var i=0; i<this.props.columns.length; i++) {
            if(this.props.columns[i].metadataName==metadataName) {
                availableValues=this.props.columns[i].editorDescriptor.availableChoice;
                break;
            }
        }
        return availableValues;
    },

    bulkRemoveFromTitle: function(){
      var self = this;
       questionDataManager.bulk.removeTitle(this.props.selectedQuestions).done(function(){self.deselectsAllHandler();});
    },

    bulkShareToTitle: function(){
      this.props.bulkShareHandler(this.props.selectedQuestions);
    },

    getTextMessage: function() {
        var count = this.getSelectedQuestionCount();
        if(count==1) {
          return "1 question selected:";
        }
        else {
          return "Bulk action ( " + count + " questions selected ):";
        }
    },

    renderRemoveButton: function(){

      if (this.props.isShared){
          return(React.DOM.button( {type:"button", className:"btn btn-default", onClick:this.bulkRemoveFromTitle}, "Remove from this title"));
      }
    
      return null;
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
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                               React.DOM.div( {className:"bulk-operation-item", 'data-toggle':"tooltip", title:"Change all selected questions"}, 
                                QuestionBulkOperationSetBank( {availableStatuses:this.getAvailableBanks(), 
                                                              selectedQuestions:this.props.selectedQuestions}) 
                               )
                            ),
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                               React.DOM.div( {className:"bulk-operation-item", 'data-toggle':"tooltip", title:"Change all selected questions"}, 
                                QuestionBulkOperationSetChapter( {availableStatuses:this.getAvailableChapters(), 
                                                              selectedQuestions:this.props.selectedQuestions}) 
                               )
                            ),
                            React.DOM.td( {className:"bulk-operation-sharing"}, 
                              React.DOM.button( {type:"button", className:"btn btn-default", disabled:!this.props.canShareQuestion, onClick:this.bulkShareToTitle}, "Share with another title"),
                              this.renderRemoveButton()
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