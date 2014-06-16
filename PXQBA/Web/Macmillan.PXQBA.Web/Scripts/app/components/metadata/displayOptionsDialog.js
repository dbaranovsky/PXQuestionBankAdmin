/**
* @jsx React.DOM
*/

var DisplayOptionsDialog = React.createClass({displayName: 'DisplayOptionsDialog',

    getInitialState: function() {
        return { displayOptions: this.props.value };
    },

    editInternalFieldHandler: function() {
       this.props.updateHandler(this.props.itemIndex, "displayOptions",  this.state.displayOptions)
    },

    onChangeHandler: function(fieldName, value) {
        var displayOptions = this.state.displayOptions;
        displayOptions[fieldName] = value
        this.setState( {displayOptions: displayOptions} );
    },

    onClickTooltipHandler: function(imgName) {
        this.refs.modalDialog.refs.cancelButton.getDOMNode().click();
        this.props.showDisplayImageDialogHandler(window.content.img.displayOptionsHelp[imgName]);
    },
 

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Display options";
        };
      
        var renderBody = function(){
             return (React.DOM.div(null, 
                        React.DOM.div(null, 
                         React.DOM.div(null,  " ", React.DOM.b(null,  " Question Picker (Question in Question Banks)"),
                                  ToltipElement( {tooltipText:"Click for details", onClickHandler:self.onClickTooltipHandler.bind(null, "questionsInQuestionBanksUrl")})
                         ),
                       
                         CheckBoxEditor( {value:self.state.displayOptions.displayInBanks, 
                            label:"Display this field when listing questions", 
                            onChangeHandler:self.onChangeHandler.bind(null, "displayInBanks")}),

                         CheckBoxEditor( {value:self.state.displayOptions.showFilterInBanks, 
                            label:"Show filter for this field",
                            onChangeHandler:self.onChangeHandler.bind(null, "showFilterInBanks")}),

                         CheckBoxEditor( 
                            {value:self.state.displayOptions.matchInBanks, 
                            label:"Search result match this field", 
                            onChangeHandler:self.onChangeHandler.bind(null, "matchInBanks")}),

                         React.DOM.div(null,  " ", React.DOM.b(null,  " Question Picker (Question in Current Quiz)"),
                                  ToltipElement( {tooltipText:"Click for details", onClickHandler:self.onClickTooltipHandler.bind(null, "questionsInCurrentQuizUrl")})
                         ),

                         CheckBoxEditor( {value:self.state.displayOptions.displayInCurrentQuiz, 
                            label:"Display this field when listing questions", 
                            onChangeHandler:self.onChangeHandler.bind(null, "displayInCurrentQuiz")}),

                         React.DOM.div(null,  " ", React.DOM.b(null,  " Quiz Instractor View"),
                                  ToltipElement( {tooltipText:"Click for details", onClickHandler:self.onClickTooltipHandler.bind(null, "quizInstructorViewUrl")}) 
                         ),
                         
                         CheckBoxEditor( {value:self.state.displayOptions.displayInInstructorQuiz, 
                            label:"Display this field when listing questions", 
                            onChangeHandler:self.onChangeHandler.bind(null, "displayInInstructorQuiz")}),

                         React.DOM.div(null,  " ", React.DOM.b(null,  " Recourse Panel"),
                                  ToltipElement( {tooltipText:"Click for details", onClickHandler:self.onClickTooltipHandler.bind(null, "resourcePanelUrl")}) 
                         ),

                         CheckBoxEditor( {value:self.state.displayOptions.displayInResources,
                             label:"Display this field when listing questions", 
                             onChangeHandler:self.onChangeHandler.bind(null, "displayInResources")}),

                         CheckBoxEditor( {value:self.state.displayOptions.showFilterInResources, 
                            label:"Show filter for this field", 
                            onChangeHandler:self.onChangeHandler.bind(null, "showFilterInResources")}),

                         CheckBoxEditor( {value:self.state.displayOptions.matchInResources, 
                            label:"Search result match this field", 
                            onChangeHandler:self.onChangeHandler.bind(null, "matchInResources")})

                         
                        ),
                         React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {ref:"cancelButton", type:"button", className:"btn btn-default", 'data-dismiss':"modal", onClick:self.props.closeDialogHandler}, "Cancel"),
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal", onClick:self.editInternalFieldHandler}, "Save")
                         )
                    )
            );
        };

        return (ModalDialog( {ref:"modalDialog",
                             showOnCreate:true,
                             renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             closeDialogHandler:  this.props.closeDialogHandler,
                             dialogId:"displayOptionsDialog"}));
    }
});




