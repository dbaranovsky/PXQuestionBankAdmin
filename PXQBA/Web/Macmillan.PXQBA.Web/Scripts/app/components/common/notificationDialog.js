﻿/**
* @jsx React.DOM
*/

var NotificationDialog =React.createClass({displayName: 'NotificationDialog',
   
  getInitialState: function(){
    return ( {dontShow: false})
  },

   closeDialog: function(){
   	this.props.closeDialog();
   },

   proceedHandler: function(){
    if(this.state.dontShow){
      userManager.dontShowForCurrentUser(this.props.notification.notificationTypeId);
    }
   	this.props.proceedHandler();
   },
 
   changeShowHangler: function(){
      this.setState({dontShow: !this.state.dontShow});
   },

    render: function() {
      
       
        var self = this;
        var renderHeaderText = function() {
            return (self.props.notification.notificationTypeId == window.enums.notificationTypes.newDraftForAvailableToInstructors? "Notification" : "Warning");
        };
        
        var renderBody = function(){
            return (React.DOM.div( {className:"notification-body"}, 
            			React.DOM.p(null, self.props.notification.message),
            			React.DOM.input( {type:"checkbox", checked:self.state.dontShow, onChange:self.changeShowHangler}),React.DOM.span(null,  " Don't show again")
            		));
        };

        var renderFooterButtons = function(){
               return (React.DOM.div( {className:"modal-footer"},  
                         React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:self.proceedHandler}, "Proceed"),
                         React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", 'data-targer':"notificationDialog"}, "Cancel")
                      ));
        };
  
        return (ModalDialog( {renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             dialogId:"notificationDialog",
                             closeDialogHandler:  this.closeDialog,
                             showOnCreate:  true,
                             preventDefaultClose: this.props.isCustomCloseHandle,
                             renderFooterButtons:renderFooterButtons} )
                );
  
       
    }
});