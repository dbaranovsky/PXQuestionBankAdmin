/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarImport = React.createClass({

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
                 <table className="bulk-operation-bar-table">
                          <tr>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                 <span> {this.props.message}  </span>
                               </div>
                            </td>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                <button type="button" className="btn btn-primary"  onClick={this.importHandler}>
                                    Import questions to...
                                </button>
                              </div>
                            </td>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                 <button type="button" className="btn btn-default" onClick={this.backHandler}>
                                     Back
                                  </button>
                              </div>
                            </td>
                          </tr>
                        </table>
            );
        }
});