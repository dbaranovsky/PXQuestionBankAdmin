/**
* @jsx React.DOM
*/

var QuestionGrid = React.createClass({
  
  customPaginatorClickHandle: function(page){
      routsManager.setPage(page);
  },

  getCapabilities: function(){
    return {
            canDuplicateQuestion: this.props.response.canDuplicateQuestion,
            canUnflagQuestion: this.props.response.canUnflagQuestion,
            canFlagQuestion: this.props.response.canFlagQuestion,
            canAddNotesQuestion: this.props.response.canAddNotesQuestion,
            canViewHistory: this.props.response.canViewHistory,
            canShareQuestion: this.props.response.canShareQuestion,
            canPublishDraft: this.props.response.canPublishDraft,
            canPreviewQuestion: this.props.response.canPreviewQuestion,
            canChangeFromAvailibleToInProgress: this.props.response.canChangeFromAvailibleToInProgress,
            canChangeFromAvailibleToDeleted: this.props.response.canChangeFromAvailibleToDeleted,
            canChangeFromDeletedToInProgress: this.props.response.canChangeFromDeletedToInProgress,
            canChangeFromDeletedToAvailible: this.props.response.canChangeFromDeletedToAvailible,
            canChangeFromInProgressToAvailible: this.props.response.canChangeFromInProgressToAvailible,
            canChangeFromInProgressToDeleted: this.props.response.canChangeFromInProgressToDeleted
          };
  },

  getOptions: function() {
    if(this.props.mode==window.enums.questionGridModesType.importQuestions) {
        return {
            bulkOperationBarType: window.enums.bulkOperationBarType.importQuestions,
            inlineMenuEnabled: false,
            selectQuestionByRow: true,
        };
    }

    return {
        bulkOperationBarType: window.enums.bulkOperationBarType.standart,
        inlineMenuEnabled: true,
        selectQuestionByRow: false,
    };
  },
     
  render: function() { 
        if (this.props.response.canViewQuestionList){
             return (
              <div className="questionGrid">
                <div className="question-grid-item"> 
                     <QuestionFilter filter={this.props.response.filter} allAvailableColumns={this.props.response.allAvailableColumns}/>
                </div>
                <div className="question-grid-item"> 
                    <QuestionList data={this.props.response.questionList} 
                                        filter={this.props.response.filter}
                                        order={this.props.response.order} 
                                        columns={this.props.response.columns}
                                        questionCardTemplate = {this.props.response.questionCardLayout}
                                        allAvailableColumns={this.props.response.allAvailableColumns}
                                        handlers={this.props.handlers}
                                        currentPage={this.props.response.pageNumber}
                                        capabilities = {this.getCapabilities()}
                                        options = {this.getOptions()}
                                        />
                </div> 
                <div className="question-grid-item"> 
                    <Paginator metadata={{
                            currentPage: this.props.response.pageNumber,
                            totalPages: this.props.response.totalPages}} 
                            customPaginatorClickHandle={this.customPaginatorClickHandle}/>
                </div> 
                 
            </div> 

            );
          
        }
        
         return (<b>You have no access to view question list</b>);
    }
});
