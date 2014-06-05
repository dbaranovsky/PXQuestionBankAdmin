/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({displayName: 'QuestionListPage',

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
            return (Loader(null ));
        }
        return null;
    },

    nextStepHandler: function(question){
        questionDataManager.getNewQuestionTemplate(question)
                            .done(this.loadTemplateComplete.bind(this, true))
                            .fail(this.resetState);
    },

    renderQuestionEditorDialog: function() {
      
        switch (this.state.editor.step) {
          case this.editorsSteps.step1:
           return ( QuestionTypeDialog( 
                              {nextStepHandler:this.nextStepHandler, 
                              showOnCreate:true, 
                              metadata:this.state.editor.metadata, 
                              closeDialogHandler:  this.closeDialogHandler}));
          case this.editorsSteps.step2:

            return (QuestionEditorDialog( {closeDialogHandler:this.closeDialogHandler,
                                          editSourceQuestionHandler:this.editSourceQuestionHandler,
                                          isNew:this.state.editor.isNew,
                                          showOnCreate:true,
                                          question:this.state.editor.template,
                                          caption:this.state.editor.caption, 
                                          metadata:this.state.editor.metadata,
                                          viewHistoryMode:this.state.editor.viewHistoryMode,
                                          handlers:  {
                                            createDraftHandler: this.createDraftHandler,
                                            createQuestionFromVersionHandler: this.createQuestionFromVersionHandler
                                          }}
                                           ));
                                          
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
           editorCaption: window.enums.dialogCaptions.duplicateQuestion
        });
        questionDataManager.getDuplicateQuestionTemplate(questionId).done(this.loadTemplateComplete.bind(this, false));
    },

    editQuestionHandler: function(questionId, viewHistoryMode) {

        this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.editQuestion,
           sourceQuestionId: questionId,
           viewHistoryMode: viewHistoryMode === undefined? false : viewHistoryMode
        });
       questionDataManager.getQuestion(questionId).done(this.loadTemplateComplete.bind(this, false));
    },

    publishDraftHandler: function(questionId) {
      if(confirm("You are about to publish a draft. The edits reflected in this draft will replace all content in the original question. Click Proceed to continue with the publish process")){
         questionDataManager.publishDraftToOriginal(questionId);
      }
    },
       
    //from version 
    createQuestionFromVersionHandler: function(questionId, version) {
      this.showEditor(this.editorsSteps.none);
      this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.duplicateQuestion
        });

       questionDataManager.getDuplicateQuestionTemplate(questionId, version).done(this.loadTemplateComplete.bind(this, false));
    },

    //from version
    createDraftHandler: function(questionId, version) {
      this.showEditor(this.editorsSteps.none);
      this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.duplicateQuestion
        });
       questionDataManager.createDraft(questionId, version).done(this.loadTemplateComplete.bind(this, false));
    },

    loadTemplateComplete: function(isNew, template) { 
        if(!isNew){
          questionDataManager.getMetadataFields().done(this.loadMetadataForEditingComplete.bind(this, template));
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
                    viewHistoryMode: this.state.viewHistoryMode
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
      questionDataManager.getMetadataFields().done(this.loadMetadataComplete.bind(this))

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
                   viewHistoryMode: this.state.viewHistoryMode
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
                   viewHistoryMode: this.state.viewHistoryMode }
        });
    },


    closeNoteDialogHandler: function(){
      this.setState({
            showNoteEditDialog: false,
            questionIdForNotes: 0 
       });
    },

    renderNotesDialog: function(){
      if(this.state.showNoteEditDialog) {
        return (EditQuestionNotesDialog( {closeDialogHandler:this.closeNoteDialogHandler, questionId:this.state.questionIdForNotes}));
      }
      return null;
    },

    renderShareDialog: function() {
      if(this.state.showShareDialog) {
        return (QuestionShareDialog( {showOnCreate:true, closeDialogHandler:this.closeShareDialogHandler, questionIds:this.state.questionIds, currentTitle:this.props.response.productTitle}));
      }
      return null;
    },

    closeShareDialogHandler: function(){
       this.setState({
        showShareDialog: false,
        questionId: 0 
      });
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

    render: function() {
       return (
            React.DOM.div( {className:"QuestionListPage"}, 
             this.renderLoader(),
                React.DOM.div(null, 
                  React.DOM.a( {href:window.actions.questionTitle.titleListUrl},   "  << Back to the titles list " )
                ),
                React.DOM.div( {className:"add-question-action"}, 
                    React.DOM.button( {className:"btn btn-primary ",  onClick:this.initialCreateNewQuestion}, 
                    "Add Question"
                    )
                ),
                React.DOM.div(null, 
                  QuestionTabs( {response:this.props.response, handlers:{
                                                                copyQuestionHandler: this.copyQuestionHandler,
                                                                editQuestionHandler: this.editQuestionHandler,
                                                                closeNoteDialogHandler: this.closeNoteDialogHandler,
                                                                editNotesHandler: this.editNotesHandler,
                                                                shareHandler: this.shareHandler,
                                                                publishDraftHandler: this.publishDraftHandler,
                                                                createDraftHandler: this.createDraftHandler
                                                                }})
                ),
                this.renderQuestionEditorDialog(),
                this.renderNotesDialog(),
                this.renderShareDialog()
                 
            )
            );          
    }
});

