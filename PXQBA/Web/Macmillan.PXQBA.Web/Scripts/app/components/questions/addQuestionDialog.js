/**
* @jsx React.DOM
*/

var AddQuestionDialog = React.createClass({displayName: 'AddQuestionDialog',

   
    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };

        var nextStepHandler = this.props.nextStepHandler;
        var renderBody = function(){

            return (AddQuestionBox( {nextStepHandler:nextStepHandler}));
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (ModalDialog( {renderHeaderText:renderHeaderText, renderBody:renderBody, renderFooterButtons:renderFooterButtons, dialogId:"addQuestionModal"})
                );
    }
});

var AddQuestionBox = React.createClass({displayName: 'AddQuestionBox',

    render: function() {

            return (React.DOM.div(null, 
                        QuestionTypeList( {questionTypes:filterDataManager.getQuestionTypeList()}),
                        
                        
                            React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:this.props.nextStepHandler}, "Next"),
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Close")
                            )
                    

                   )
               );
        
      
       
    }
});