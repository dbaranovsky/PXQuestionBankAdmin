/**
* @jsx React.DOM
*/

var AddQuestionDialog = React.createClass({displayName: 'AddQuestionDialog',

    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };
        var renderBody = function(){
            return (QuestionTypeList( {questionTypes:filterDataManager.getQuestionTypeList()}));
        };
        var renderFooterButtons = function(){
            return (React.DOM.div(null, 
                        React.DOM.button( {type:"button", className:"btn btn-primary"}, "Save changes"),
                        React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Close")
                    ));
        };
        return (ModalDialog( {renderHeaderText:renderHeaderText, renderBody:renderBody, renderFooterButtons:renderFooterButtons, dialogId:"addQuestionModal"})
                );
    }
});
