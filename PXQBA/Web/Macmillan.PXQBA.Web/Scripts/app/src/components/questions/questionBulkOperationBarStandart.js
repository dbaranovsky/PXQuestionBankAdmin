/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarStandart = React.createClass({

    deselectsAllHandler: function() {
        this.props.deselectsAllHandler();
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

    renderRemoveButton: function(){

      if (this.props.isShared){
          return(<button type="button" className="btn btn-default" onClick={this.bulkRemoveFromTitle}>Remove from this title</button>);
      }
    
      return null;
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
                               <div className="bulk-operation-item" data-toggle="tooltip" title="Change all selected questions">
                                <QuestionBulkOperationSetStatus availableStatuses={this.getAvailableStatuses()} 
                                                              selectedQuestions={this.props.selectedQuestions}/> 
                               </div>
                            </td>
                            <td className="bulk-operation-cell">
                               <div className="bulk-operation-item" data-toggle="tooltip" title="Change all selected questions">
                                <QuestionBulkOperationSetBank availableStatuses={this.getAvailableBanks()} 
                                                              selectedQuestions={this.props.selectedQuestions}/> 
                               </div>
                            </td>
                            <td className="bulk-operation-cell">
                               <div className="bulk-operation-item" data-toggle="tooltip" title="Change all selected questions">
                                <QuestionBulkOperationSetChapter availableStatuses={this.getAvailableChapters()} 
                                                              selectedQuestions={this.props.selectedQuestions}/> 
                               </div>
                            </td>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                <button type="button" className="btn btn-default" disabled={!this.props.capabilities.canShareQuestion} onClick={this.bulkShareToTitle}>Share with another title</button>
                              </div>
                            </td>
                            <td className="bulk-operation-cell">
                               <div className="bulk-operation-item">
                                  {this.renderRemoveButton()}
                              </div>
                            </td>
                            <td className="bulk-operation-cell">
                               <div className="deselect-button" onClick={this.deselectsAllHandler} data-toggle="tooltip" title="Deselect all">
                                 <span > X </span>
                               </div>
                            </td>
                          </tr>
                        </table>
            );
        }
});