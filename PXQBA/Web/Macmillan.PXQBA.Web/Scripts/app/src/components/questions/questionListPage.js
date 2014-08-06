/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

    editorsSteps: {
            none: 'none',
            step1: 'step1',
            step2: 'step2',
    },


    getInitialState: function() {
      return { 
               loading: false, 
               editor: {
                        step: this.editorsSteps.none,
                        template: null, 
                        isNew: false,
                        caption: window.enums.dialogCaptions.newQuestion,
                        viewHistoryMode: false
                       }
             };
    },

    renderLoader: function() {
        if((this.state.loading)) {
            return (<Loader />);
        }
        return null;
    },

    nextStepHandler: function(question){
        questionDataManager.getNewQuestionTemplate(this.props.response.productCourseId, question)
                            .done(this.loadTemplateComplete.bind(this, true))
                            .fail(this.resetState);
    },

    renderQuestionEditorDialog: function() {
      
        switch (this.state.editor.step) {
          case this.editorsSteps.step1:
           return ( <QuestionTypeDialog 
                              nextStepHandler={this.nextStepHandler} 
                              showOnCreate={true} 
                              metadata={this.state.editor.metadata} 
                              closeDialogHandler = {this.closeDialogHandler}/>);
          case this.editorsSteps.step2:

            return (<QuestionEditorDialog 
                                          currentCourseId={this.props.response.productCourseId}
                                          closeDialogHandler={this.closeDialogHandler}
                                          editSourceQuestionHandler={this.editSourceQuestionHandler}
                                          isNew={this.state.editor.isNew}
                                          showOnCreate={true}
                                          question={this.state.editor.template}
                                          caption={this.state.editor.caption }
                                          metadata={this.state.editor.metadata}
                                          viewHistoryMode={this.state.editor.viewHistoryMode}
                                          isEditedInPlace = {this.state.editor.isEditedInPlace}
                                          handlers = {{
                                            createDraftHandler: this.createDraftHandler,
                                            createQuestionFromVersionHandler: this.createQuestionFromVersionHandler,
                                            reloadEditor: this.reloadQuestion
                                          }} />);
                                          
          default:
            return null;
        }
    },

    resetState: function(e){
         this.closeDialogHandler();
    },

    copyQuestionHandler: function(questionId) {
        this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.duplicateQuestion,
           viewHistoryMode: false
        });
        questionDataManager.getDuplicateQuestionTemplate(this.props.response.productCourseId, questionId).done(this.loadTemplateComplete.bind(this, false));
    },

    editQuestionHandler: function(questionId, viewHistoryMode, isEditedInPlace) {

        this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.editQuestion,
           sourceQuestionId: questionId,
           viewHistoryMode: viewHistoryMode === undefined? false : viewHistoryMode,
           isEditedInPlace: isEditedInPlace === undefined? false : isEditedInPlace
        });
       questionDataManager.getQuestion(this.props.response.productCourseId, questionId).done(this.loadTemplateComplete.bind(this, false));
    },

    publishDraftHandler: function(questionId) {

         questionDataManager.publishDraftToOriginal(this.props.response.productCourseId, questionId);
    },
       
    //from version 
    createQuestionFromVersionHandler: function(questionId, version) {
      this.showEditor(this.editorsSteps.none);
      this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.duplicateQuestion
        });

       questionDataManager.getDuplicateQuestionTemplate(this.props.response.productCourseId, questionId, version).done(this.loadTemplateComplete.bind(this, false));
    },

    //from version
    createDraftHandler: function(questionId, version, status) {
      this.showEditor(this.editorsSteps.none);
      this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.editQuestion,
           viewHistoryMode: false
        });

        questionDataManager.createDraft(this.props.response.productCourseId, questionId, version, status).done(this.loadTemplateComplete.bind(this, false));
     
    },

    reloadQuestion: function(){
       this.showEditor(this.editorsSteps.none);
       this.editQuestionHandler(this.state.sourceQuestionId, this.state.viewHistoryMode, this.state.isEditedInPlace);
    },

    loadTemplateComplete: function(isNew, template) { 
        if(!isNew){
          questionDataManager.getMetadataFields(this.props.response.productCourseId).done(this.loadMetadataForEditingComplete.bind(this, template));
          return;
        }

        this.setState({
                 loading: false,
                 editor: {
                    template: template,
                    step: this.editorsSteps.step2,
                    isNew: isNew,
                    metadata: this.state.editor.metadata,
                    caption: isNew? window.enums.dialogCaptions.newQuestion : this.state.editorCaption},
                    sourceQuestionId: this.state.sourceQuestionId,
                    viewHistoryMode: false,
                    isEditedInPlace: this.state.isEditedInPlace
                    });
    },

    closeDialogHandler: function() {
        this.showEditor(this.editorsSteps.none);
    },

    editSourceQuestionHandler: function(questionId){
       this.showEditor(this.editorsSteps.none);
       this.editQuestionHandler(questionId);
    },

  
    showEditor: function(step) {
      this.setState({
                editor: {
                    template: this.state.editor.template,
                    step: step,
                    isNew: this.state.editor.isNew }
                });
    },
     

    initialCreateNewQuestion: function() {
      this.setState({ loading:true} );
      questionDataManager.getMetadataFields(this.props.response.productCourseId).done(this.loadMetadataComplete.bind(this))

    },

    loadMetadataForEditingComplete: function(template, metadata){
         this.setState({
              loading: false,
               editor: {
                   metadata: metadata,
                   template: template,
                   isNew: false,
                   step: this.editorsSteps.step2,
                   caption: this.state.editorCaption,
                   viewHistoryMode: this.state.viewHistoryMode,
                   isEditedInPlace: this.state.isEditedInPlace
                   }
        });
    },

    loadMetadataComplete: function(metadata) {
        this.setState({
              loading: false,
               editor: {
                   metadata: metadata,
                   template: this.state.editor.template,
                   step: this.editorsSteps.step1,
                   isNew: this.state.editor.isNew,
                   viewHistoryMode: this.state.viewHistoryMode,
                   isEditedInPlace: this.state.isEditedInPlace }
        });
    },


 

    renderNotesDialog: function(){
      if(this.state.showNoteEditDialog) {
        return (<EditQuestionNotesDialog closeDialogHandler={this.closeNoteDialogHandler}
                                         questionId={this.state.questionIdForNotes} 
                                         canDelete={this.props.response.canRemoveNotesQuestion}
                                         canAddNote={this.props.response.canAddNotesQuestion}/>);
      }
      return null;
    },

    renderShareDialog: function() {
      if(this.state.showShareDialog) {
        return (<QuestionShareDialog currentCourseId={this.props.response.productCourseId} showOnCreate={true} closeDialogHandler={this.closeShareDialogHandler} questionIds={this.state.questionIds} currentTitle={this.props.response.productTitle}/>);
      }
      return null;
    },

    renderNotificationDialog: function(){
      if(this.state.showNotificationDialog){
        return (<NotificationDialog  closeDialog={this.closeNotificationDialog} proceedHandler = {this.state.proceed} notification={this.state.notification} isCustomCloseHandle={false}/>);
      }
    },

   closeNoteDialogHandler: function(notes){
    var questionId = this.state.questionIdForNotes;
      this.setState({
            showNoteEditDialog: false,
            questionIdForNotes: 0 
       });
    },

    closeShareDialogHandler: function(){
       this.setState({
        showShareDialog: false,
        questionId: 0 
      });
    },

    closeNotificationDialog: function(){
      this.setState({showNotificationDialog: false});
    },

    editNotesHandler: function(qId) {
       this.setState({
            showNoteEditDialog: true,
            questionIdForNotes: qId 
       });
    },

    shareHandler: function(questionIds){
      this.setState({
        showShareDialog: true,
        questionIds: questionIds
      });
    },

    showNotification: function(notification, proceed){
      this.setState({
          showNotificationDialog: true,
          notification: notification,
          proceed: proceed
      });
    },

    renderAddQuestionButton: function() {
      if(this.props.mode==window.enums.questionGridModesType.importQuestions) {
        return null;
      }

      return (<button className="btn btn-primary " disabled={!this.props.response.canCreateQuestion} onClick={this.initialCreateNewQuestion}>
                    Add Question
              </button>);
    },

    render: function() {
       return (
            <div className="QuestionListPage">
             {this.renderLoader()}
                <div>
                  <a href={window.actions.questionTitle.titleListUrl}>
                     <span className="mars-font">&#59442;&#59442; </span>
                     <span> Back to the titles list</span>
                  </a>
                </div>
                <div className="add-question-action">
                     {this.renderAddQuestionButton()}
                </div>
                <div>
                  <QuestionTabs 
                       mode={this.props.mode}
                       response={this.props.response} 
                       handlers={{
                               copyQuestionHandler: this.copyQuestionHandler,
                               editQuestionHandler: this.editQuestionHandler,
                               closeNoteDialogHandler: this.closeNoteDialogHandler,
                               editNotesHandler: this.editNotesHandler,
                               shareHandler: this.shareHandler,
                               publishDraftHandler: this.publishDraftHandler,
                               createDraftHandler: this.createDraftHandler,
                               showNotification: this.showNotification
                        }}/>
                </div>
                {this.renderQuestionEditorDialog()}
                {this.renderNotesDialog()}
                {this.renderShareDialog()}
                {this.renderNotificationDialog()}
                 
            </div>
            );          
    }
});

