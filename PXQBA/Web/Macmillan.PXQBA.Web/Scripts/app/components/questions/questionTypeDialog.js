/**
* @jsx React.DOM
*/

var QuestionTypeDialog = React.createClass({displayName: 'QuestionTypeDialog',

    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
    },

   
    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };

        var nextStepHandler = this.props.nextStepHandler;
        var questionTypes = this.props.questionTypes;
        var renderBody = function(){

            return (AddQuestionBox( {nextStepHandler:nextStepHandler, questionTypes:questionTypes}));
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
                        QuestionTypeList( {questionTypes:this.props.questionTypes}),
                            React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:this.props.nextStepHandler}, "Next"),
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Close")
                            )
                   )
               );
    }
});