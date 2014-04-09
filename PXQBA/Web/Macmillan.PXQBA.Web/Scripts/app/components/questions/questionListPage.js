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
                        isNew: false
                       }
             };
    },

    renderLoader: function() {
        if((this.state.loading)) {
            return (Loader(null ));
        }
        return null;
    },

    nextStepHandler: function(questionType){
        questionDataManager.getNewQuestionTemplate(questionType).done(this.loadTemplateComplete.bind(this, true));
    },

    renderQuestionEditorDialog: function() {
      
        switch (this.state.editor.step) {
          case this.editorsSteps.step1:
           return ( QuestionTypeDialog( 
                              {nextStepHandler:this.nextStepHandler, 
                              showOnCreate:true, 
                              questionTypes:this.state.editor.questionTypes, 
                              closeDialogHandler:  this.closeDialogHandler}));
          case this.editorsSteps.step2:
            return (QuestionEditorDialog( {closeDialogHandler:this.closeDialogHandler,
                                          isNew:this.state.editor.isNew,
                                          showOnCreate:true,
                                          question:this.state.editor.template}));
          default:
            return null;
        }
    },

    copyQuestionHandler: function(questionId) {
        this.setState({ loading:true} );
        questionDataManager.getDuplicateQuestionTemplate(questionId).done(this.loadTemplateComplete.bind(this, false));
    },

    editQuestionHandler: function(questionId) {
      this.setState({ loading:true} );
      questionDataManager.getQuestion(questionId).done(this.loadTemplateComplete.bind(this, false));
    },

    loadTemplateComplete: function(isNew, template) {
       if(isNew){
        template.type = this.state.questionType;
       }
             
    loadTemplateComplete: function(isNew, template) { 
        this.setState({
                 loading: false,
                 editor: {
                    template: template,
                    step: this.editorsSteps.step2,
                    isNew: isNew }
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
      questionFilterDataManager.getQuestionTypeList().done(this.loadQuestionTypesComplete.bind(this))

    },

    loadQuestionTypesComplete: function(questionTypes) {
        this.setState({
              loading: false,
               editor: {
                   questionTypes: questionTypes,
                   template: this.state.editor.template,
                   step: this.editorsSteps.step1,
                   isNew: this.state.editor.isNew }
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
                                                                editQuestionHandler: this.editQuestionHandler
                                                                }})
                ),
                this.renderQuestionEditorDialog()
            )
            );
    }
});

