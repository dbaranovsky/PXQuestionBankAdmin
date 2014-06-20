/**
* @jsx React.DOM
*/

var RoleDialog = React.createClass({

  getInitialState: function(){
      return({
         loading: true,
         role: this.props.role
      });
  },

    closeDialog: function(){
         $(this.getDOMNode()).modal("hide");
         this.props.closeAddRoleDialog();
    },
   
  componentDidMount: function(){
    var self=this;
      userManager.getRolesCapabilities(this.props.role == undefined? "" : this.props.role.id, this.props.courseId).done(this.setRole).error(function(e){self.setState({loading: false})});
  },

  setRole: function(role){
    this.setState({loading: false, role: role});
  },


  editRoleHandler: function(role){
    this.setState({role: role});
   },

  saveRoleHandler: function(){
    if(this.state.role.name == ""){
      userManager.showWarningPopup("Please, enter role name");
      return;
    }
    this.props.saveRoleHandler(this.state.role, this.props.newRole);
  },

  removeRoleHandler: function(){
      this.props.removeRoleHandler(this.state.role);
  },

    render: function() {
       var self = this;
        var renderHeaderText = function() {

            if(self.props.deleting){
               return "Are you sure you want to delete role "+ self.state.role.name + " with following capabilities?";
            }

            if (self.props.viewMode){
             return "Capabilities availible for "+ self.state.role.name;
            }

            if(self.props.newRole){
              return "Add Role";
            }

            return "Edit Role";
        };

      
        var renderBody = function(){
            if (self.state.loading){
              return (<div className="waiting"></div>);
            }

            if(self.state.role == null){
              return (<b>No capabilities are loaded</b>);
            }
        
            return (<div>

                       <EditRoleBox role={self.state.role} newRole={self.props.newRole} viewMode={self.props.viewMode} editRoleHandler={self.editRoleHandler} deleting={self.props.deleting}/>
                    </div>
            );
        };

     
      var  renderFooterButtons = function(){

                  if(self.props.deleting){
                   return (<div className="modal-footer"> 
                              <button type="button" className="btn btn-primary" data-dismiss="modal" data-target="roleModal" onClick={self.removeRoleHandler}>Delete</button>
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="roleModal">Close</button>
                          </div>);
                  }
                  if(self.props.viewMode){
                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="roleModal">Close</button>
                          </div>);
                  }

                  if(self.state.role != undefined && self.state.role.name==""){
                       return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-primary"  onClick={self.saveRoleHandler}>Save</button>
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="roleModal">Cancel</button>
                          </div>);
                  }

                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-primary" data-dismiss="modal" data-target="roleModal" onClick={self.saveRoleHandler}>Save</button>
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="roleModal">Cancel</button>
                          </div>);
                 }

   

        return (<ModalDialog  showOnCreate = {true} renderHeaderText={renderHeaderText} renderBody={renderBody}  closeDialogHandler = {this.closeDialog}  renderFooterButtons={renderFooterButtons} dialogId="addRoleModal"/>);
    }

});




