/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBar = React.createClass({

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

    render: function() {
        return ( 
                  <tr>
                    <td colSpan={this.props.colSpan} className="bulk-operation-bar">
                        <table className="bulk-operation-bar-table">
                          <tr>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                 <span> {this.getSelectedQuestionCount()} questions selected</span>
                               </div>
                            </td>
                            <td className="bulk-operation-cell">
                               <div className="bulk-operation-item" data-toggle="tooltip" title="Change all selected questions">
                                <QuestionBulkOperationSetStatus availableStatuses={this.getAvailableStatuses()} 
                                                              selectedQuestions={this.props.selectedQuestions}/> 
                               </div>
                            </td>
                            <td className="bulk-operation-sharing">
                              <button type="button" className="btn btn-default" onClick={this.bulkShareToTitle}>Share with another title</button>
                              <button type="button" className="btn btn-default" onClick={this.bulkRemoveFromTitle}>Remove from this title</button>
                            </td>
                            <td>
                               <div className="deselect-button" onClick={this.deselectsAllHandler} data-toggle="tooltip" title="Deselect all">
                                 <span > X </span>
                               </div>
                            </td>
                          </tr>
                        </table>
                    </td>
                  </tr>
            );
        }
});