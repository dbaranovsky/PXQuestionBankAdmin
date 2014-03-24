/**
* @jsx React.DOM
*/ 

var QuestionListColumnSelector = React.createClass({displayName: 'QuestionListColumnSelector',

	render: function() {
		return (
           React.DOM.div( {className:"modal fade", id:"addQuestionFieldModal", tabindex:"-1", role:"dialog", 'aria-labelledby':"addQuestionFieldLabel", 'aria-hidden':"true"}, 
              React.DOM.div( {className:"modal-dialog"}, 
                React.DOM.div( {className:"modal-content"}, 
                  React.DOM.div( {className:"modal-header"}, 
                    React.DOM.button( {type:"button", className:"close", 'data-dismiss':"modal", 'aria-hidden':"true"}, "×"),
                    React.DOM.h4( {className:"modal-title", id:"myModalLabel"}, "Add Fields")
                  ),
                  React.DOM.div( {className:"modal-body"}, 
                    "..."
                  ),
                  React.DOM.div( {className:"modal-footer"}, 
                    React.DOM.button( {type:"button", className:"btn btn-primary"}, "Save changes"),
                    React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Close")
                  )
                )
              )
            )
			);
	}
});                  