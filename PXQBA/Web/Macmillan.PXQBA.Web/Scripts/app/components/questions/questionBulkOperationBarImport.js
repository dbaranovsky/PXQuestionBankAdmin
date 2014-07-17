/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarImport = React.createClass({displayName: 'QuestionBulkOperationBarImport',

    backHandler: function() {
       window.location = window.actions.importActions.formTitleStep1Url;
    },

    importHandler: function() {
       var selectedQuestions = this.props.selectedQuestions;
       if(selectedQuestions.length>0) {
           importDataManager.saveQuestionsForImport(selectedQuestions).done(this.saveQuestionsForImportDoneHandler);
       }
       else {
           notificationManager.showWarning("You should select questions to continue.");
       }
    },

    saveQuestionsForImportDoneHandler: function (response) {
       window.location = window.actions.importActions.fromTitleStep3Url;
    },

    render: function() {
        return ( 
                 React.DOM.table( {className:"bulk-operation-bar-table"}, 
                          React.DOM.tr(null, 
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                              React.DOM.div( {className:"bulk-operation-item"}, 
                                 React.DOM.span(null,  " ", this.props.message,  "  "  )
                               )
                            ),
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                              React.DOM.div( {className:"bulk-operation-item"}, 
                                React.DOM.button( {type:"button", className:"btn btn-primary",  onClick:this.importHandler}, 
                                    "Import questions to..."
                                )
                              )
                            ),
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                              React.DOM.div( {className:"bulk-operation-item"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", onClick:this.backHandler}, 
                                     "Back"
                                  )
                              )
                            )
                          )
                        )
            );
        }
});