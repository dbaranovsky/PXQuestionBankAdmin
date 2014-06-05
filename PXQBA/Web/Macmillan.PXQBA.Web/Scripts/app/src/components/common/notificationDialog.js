/**
* @jsx React.DOM
*/

var NotificationDialog =React.createClass({
   
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
            return (<div className="notification-body">
            			<p>{self.props.message}</p>
            			<input type="checkbox"/><span> Dont show again</span>
            			  <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-primary" data-dismiss="modal"  onClick={self.proceedHandler}>Proceed</button>
                                 <button type="button" className="btn btn-default" data-dismiss="modal" onClick={self.closeDialog}>Cancel</button>
                            </div>
            		</div>);
        };
  
        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             dialogId="notificationDialog"
                             closeDialogHandler = {this.closeDialog}
                             showOnCreate = {true}
                             preventDefaultClose ={true} />
                );
  
       
    }
});