/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

    getInitialState: function() {
      return { loading: false, showEditor: false};
    },

    renderLoader: function() {
        if((this.state.loading)) {
            return (<Loader />);
        }
        return null;
    },


    nextStepHandler: function(){
         this.setState({showEditor: true});
    },

    renderQuestionEditorDialog: function()
    {
      if(this.state.showEditor)
          {          

            return (<QuestionEditorDialog closeDialogHandler={this.closeDialogHandler} isNew={true} showOnCreate={true} question={this.state.template}/>);
          }
      return null;
    },



    closeDialogHandler: function()
    {
         this.setState({showEditor: false});
    },

    loadTemplate: function(data){
      this.setState({template: data});
    },

    componentDidMount: function()
    {
         questionDataManager.getNewQuestionTemplate().done(this.loadTemplate);
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
                  <QuestionTabs response={this.props.response} />
                </div>
                <AddQuestionDialog nextStepHandler={this.nextStepHandler}/>
                {this.renderQuestionEditorDialog()}
            </div>
            );
    }
});

