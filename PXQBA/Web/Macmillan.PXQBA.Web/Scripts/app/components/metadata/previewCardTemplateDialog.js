/**
* @jsx React.DOM
*/

var PreviewCardTemplateDialog = React.createClass({displayName: 'PreviewCardTemplateDialog',


    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Question card preview";
        };
      
        var renderBody = function(){
             return (React.DOM.div(null, 
                        React.DOM.div(null, 
                            React.DOM.div(null,  
                                React.DOM.div( {className:"question-card-template", dangerouslySetInnerHTML:{__html: self.props.cardHtml}} )
                             )  
                        ),
                         React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", onClick:self.props.closeDialogHandler}, "Close")
                            )
                    )
            );
        };

        return (ModalDialog( {showOnCreate:true,
                             renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             closeDialogHandler:  this.props.closeDialogHandler,
                             dialogId:"questionCardPreview"}));
    }
});




