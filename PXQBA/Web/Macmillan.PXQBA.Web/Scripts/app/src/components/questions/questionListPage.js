/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

    getInitialState: function() {
      return { 
               loading: false, 
               editor: {
                        show: false,
                        template: null, 
                        isNew: false
                       }
             };
    },

    renderLoader: function() {
        if((this.state.loading)) {
            return (<Loader />);
        }
        return null;
    },

    nextStepHandler: function(){
        this.setState({ loading:true} );
        questionDataManager.getNewQuestionTemplate().done(this.loadTemplateComplete.bind(this, true));
    },

    renderQuestionEditorDialog: function()
    {
      if(this.state.editor.show) {          
            return (<QuestionEditorDialog closeDialogHandler={this.closeDialogHandler}
                                          isNew={this.state.isNew}
                                          showOnCreate={true}
                                          question={this.state.editor.template}/>);
          }
      return null;
    },

    copyQuestionHandler: function(questionId) {
        this.setState({ loading:true} );
        questionDataManager.getDuplicateQuestionTemplate(questionId).done(this.loadTemplateComplete.bind(this, false));
    },

    loadTemplateComplete: function(isNew, template) {
        this.setState({
                 loading: false,
                 editor: {
                    template: template,
                    show: true,
                    isNew: isNew }
                    });
    },

    closeDialogHandler: function() {
        this.showEditor(false);
    },

    showEditor: function(showEditor) {
      this.setState({
                editor: {
                    template: this.state.editor.template,
                    show: showEditor,
                    isNew: this.state.editor.isNew }
                });
    },
     
    render: function() {
       return (
            <div className="QuestionListPage">
             {this.renderLoader()}
                <div className="add-question-action">
                    <button className="btn btn-primary " data-toggle="modal" data-target="#addQuestionModal">
                    Add Question
                    </button>
                </div>
                <div>
                  <QuestionTabs response={this.props.response} handlers={{copyQuestionHandler: this.copyQuestionHandler}}/>
                </div>
                <AddQuestionDialog nextStepHandler={this.nextStepHandler}/>
                {this.renderQuestionEditorDialog()}
            </div>
            );
    }
});

