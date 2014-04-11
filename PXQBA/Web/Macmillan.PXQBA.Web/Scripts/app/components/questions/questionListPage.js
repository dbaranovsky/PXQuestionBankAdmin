﻿/**
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
                        caption: window.enums.dialogCaptions.newQuestion
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
                                          isNew:this.state.editor.isNew,
                                          showOnCreate:true,
                                          question:this.state.editor.template,
                                          caption:this.state.editor.caption }));
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

    editQuestionHandler: function(questionId) {
        this.setState({
           loading: true,
           editorCaption: window.enums.dialogCaptions.editQuestion
        });
       questionDataManager.getQuestion(questionId).done(this.loadTemplateComplete.bind(this, false));
    },
              
    loadTemplateComplete: function(isNew, template) { 
        
        this.setState({
                 loading: false,
                 editor: {
                    template: template,
                    step: this.editorsSteps.step2,
                    isNew: isNew,
                    caption: isNew? window.enums.dialogCaptions.newQuestion : this.state.editorCaption}
                    });
    },

    closeDialogHandler: function() {
        this.showEditor(this.editorsSteps.none);
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

    loadMetadataComplete: function(metadata) {
        this.setState({
              loading: false,
               editor: {
                   metadata: metadata,
                   template: this.state.editor.template,
                   step: this.editorsSteps.step1,
                   isNew: this.state.editor.isNew }
        });
    },


    closeNoteDialogHandler: function(){
      this.setState({
            showNoteEditDialog: false,
            questionIdForNotes: 0 
       });
    },

    renderNotesDialog: function()
    {
      if(this.state.showNoteEditDialog) {
        return (EditQuestionNotesDialog( {closeDialogHandler:this.closeNoteDialogHandler, questionId:this.state.questionIdForNotes}));
      }
      return null;
    },

    editNotesHandler: function(qId) {
       this.setState({
            showNoteEditDialog: true,
            questionIdForNotes: qId 
       });
    },

    render: function() {
       return (
            React.DOM.div( {className:"QuestionListPage"}, 
             this.renderLoader(),
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
                                                                editNotesHandler: this.editNotesHandler
                                                                }})
                ),
                this.renderQuestionEditorDialog(),
                this.renderNotesDialog()
            )
            );
    }
});

