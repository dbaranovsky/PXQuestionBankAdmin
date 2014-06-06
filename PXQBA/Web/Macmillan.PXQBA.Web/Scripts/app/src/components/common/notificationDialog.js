/**
* @jsx React.DOM
*/

var NotificationDialog =React.createClass({
   
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
            return (<div className="notification-body">
            			<p>{self.props.notification.message}</p>
            			<input type="checkbox" checked={self.state.dontShow} onChange={self.changeShowHangler}/><span> Dont show again</span>
            		</div>);
        };

        var renderFooterButtons;

        if (this.props.isCustomCloseHandle){
              renderFooterButtons = function(){
               return (<div className="modal-footer"> 
                         <button type="button" className="btn btn-primary"  onClick={self.proceedHandler}>Proceed</button>
                         <button type="button" className="btn btn-default" onClick={self.closeDialog}>Cancel</button>
                      </div>);
              };
        } else{

            var renderFooterButtons = function(){
                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-primary" data-dismiss="modal"  onClick={self.proceedHandler}>Proceed</button>
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-targer="notificationDialog">Cancel</button>
                          </div>);
            };
       }

        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             dialogId="notificationDialog"
                             closeDialogHandler = {this.closeDialog}
                             showOnCreate = {true}
                             preventDefaultClose = {this.props.isCustomCloseHandle}
                             renderFooterButtons={renderFooterButtons} />
                );
  
       
    }
});