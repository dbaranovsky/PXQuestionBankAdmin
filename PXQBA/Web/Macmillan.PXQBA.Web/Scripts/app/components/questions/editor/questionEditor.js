/**
* @jsx React.DOM
*/
var QuestionEditor = React.createClass({displayName: 'QuestionEditor',

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
            React.DOM.div(null, 
                      React.DOM.div( {className:"header-buttons"}, 
                         React.DOM.button( {className:"btn btn-primary run-question", 'data-toggle':"modal", onClick:this.runQuestion}, 
                             React.DOM.span( {className:"glyphicon glyphicon-play"}), " Run Question"
                        ),
                        React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal", onClick:this.props.closeDialog}, 
                             "Cancel"
                        ),
                         React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", onClick:this.saveQuestion} , 
                             "Save"
                        )
                      ),
                
                React.DOM.div(null, 
                  QuestionEditorTabs( {question:this.state.question, editHandler:this.editHandler} )
                )
         ));
    }
});