/**
* @jsx React.DOM
*/
var QuestionEditor = React.createClass({

   getInitialState: function() {

      return { question: this.props.question };
    },


    saveQuestion: function(){
       // if(this.props.isNew)
       // {
       // }
        var finishSaving = this.props.finishSaving;
        questionDataManager.updateQuestion(this.state.question).done(finishSaving);

    },

     runQuestion: function(){
      window.open(this.props.question.actionPlayerUrl, '_blank', 'location=yes,height=600,width=600,scrollbars=yes,status=yes');
    },

    editHandler: function(editedQuestion){
      this.setState({question: editedQuestion});
    },

    componentDidMount: function(){
       
    },

    render: function() {
        return (
            <div>
                      <div className="header-buttons">
                         <button className="btn btn-primary run-question" data-toggle="modal" onClick={this.runQuestion}>
                             <span className="glyphicon glyphicon-play"></span> Run Question
                        </button>
                        <button className="btn btn-default" data-toggle="modal" onClick={this.props.closeDialog}>
                             Cancel
                        </button>
                         <button className="btn btn-primary " data-toggle="modal" onClick={this.saveQuestion} >
                             Save
                        </button>
                      </div>
                
                <div>
                  <QuestionEditorTabs question={this.state.question} editHandler={this.editHandler} />
                </div>
         </div>);
    }
});