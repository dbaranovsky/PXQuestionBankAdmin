/**
* @jsx React.DOM
*/

var NotificationDialog =React.createClass({displayName: 'NotificationDialog',
   
   closeDialog: function(){
   	this.props.closeDialog();
   },

   proceedHandler: function(){
   	this.props.proceedHandler();
   },
 
    render: function() {
      
       
        var self = this;
        var renderHeaderText = function() {
            return (self.props.notificationType == window.enums.notificationTypes.newDraftForAvailableToInstructors? "Notification" : "Warning");
        };
        
        var renderBody = function(){
            return (React.DOM.div( {className:"notification-body"}, 
            			React.DOM.p(null, self.props.message),
            			React.DOM.input( {type:"checkbox"}),React.DOM.span(null,  " Dont show again"),
            			  React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:self.proceedHandler}, "Proceed"),
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", onClick:self.closeDialog}, "Cancel")
                            )
            		));
        };
  
        return (ModalDialog( {renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             dialogId:"notificationDialog",
                             closeDialogHandler:  this.closeDialog,
                             showOnCreate:  true,
                             preventDefaultClose: true} )
                );
  
       
    }
});