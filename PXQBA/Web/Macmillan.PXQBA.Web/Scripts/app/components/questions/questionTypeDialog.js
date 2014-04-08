/**
* @jsx React.DOM
*/

var QuestionTypeDialog = React.createClass({displayName: 'QuestionTypeDialog',

    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
        var closeDialogHandler = this.props.closeDialogHandler;
         $(this.getDOMNode()).on('hidden.bs.modal', function () {
            closeDialogHandler();
        })
    },

   
    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };

        var nextStepHandler = this.props.nextStepHandler;
        var questionTypes = this.props.questionTypes;
        var renderBody = function(){

            return (React.DOM.div(null, 

                    React.DOM.ul( {className:"nav nav-tabs"}, 
                      React.DOM.li( {className:"active"},  
                         React.DOM.a( {href:"#newQuestion", 'data-toggle':"tab"}, "New question")
                      )
                                                
                    ),   
                  
                      React.DOM.div( {className:"tab-pane active", id:"newQuestion"}, 
                         React.DOM.div( {className:"tab-body"},        
                            AddQuestionBox( {nextStepHandler:nextStepHandler, questionTypes:questionTypes})
                          )
                      )
                    
                

                )

);
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (ModalDialog( {renderHeaderText:renderHeaderText, renderBody:renderBody, renderFooterButtons:renderFooterButtons, dialogId:"addQuestionModal"})
                );
    }
});

var AddQuestionBox = React.createClass({displayName: 'AddQuestionBox',


    getInitialState: function() {
      return { 
               questionType: 0
             };
    },
   
   nextStepHandler: function(){
    //  this.props.nextStepHandler(this.state.questionType);
    this.props.nextStepHandler();
   },

   changeQuestionType: function(questionType){
   // this.setState({questionType: questionType});
   },
    render: function() {
            return (React.DOM.div(null, 
                        QuestionTypeList( {questionTypes:this.props.questionTypes, changeQuestionType:this.changeQuestionType, changeHandler:this.changeQuestionType}),
                            React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:this.props.nextStepHandler}, "Next"),
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Close")
                            )
                   )
               );
    }
});