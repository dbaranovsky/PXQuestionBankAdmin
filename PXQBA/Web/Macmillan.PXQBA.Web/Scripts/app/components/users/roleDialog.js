/**
* @jsx React.DOM
*/

var RoleDialog = React.createClass({displayName: 'RoleDialog',

  getInitialState: function(){
      return({
         role: this.convertRoleModel()
      });
  },

    closeDialog: function(){
         $(this.getDOMNode()).modal("hide");
         this.props.closeAddRoleDialog();
    },
   
  convertRoleModel: function(){
    var role = this.props.role == undefined? null : $.extend(true, {}, this.props.role);
    if(this.props.newRole){
      $.each(role.capabilityGroups, function(i, group){
         $.each(group.capabilities, function(i,capability){
            capability.isActive = false;
         });
      });
    }
    return role;
  },

  editRoleHandler: function(role){
    this.setState({role: role});
   },

  saveRoleHandler: function(){
    if(this.state.role.name == ""){
      userManager.showWarningPopup("Please, enter role name");
      return;
    }
    this.props.saveRoleHandler(this.state.role);
  },

    render: function() {
       var self = this;
        var renderHeaderText = function() {

            if (self.props.viewMode){
             return "Capabilities availible for "+ self.state.role.name;
            }

            if(self.props.newRole){
              return "Add Role";
            }

            return "Edit Role";
        };

      
        var renderBody = function(){
        
            return (React.DOM.div(null, 

                       EditRoleBox( {role:self.state.role, newRole:self.props.newRole, viewMode:self.props.viewMode, editRoleHandler:self.editRoleHandler})
                    )
            );
        };

     
      var  renderFooterButtons = function(){
                  if(self.props.viewMode){
                   return (React.DOM.div( {className:"modal-footer"},  
                             React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", 'data-target':"roleModal"}, "Close")
                          ));
                  }

                   return (React.DOM.div( {className:"modal-footer"},  
                             React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal", 'data-target':"roleModal", onClick:self.saveRoleHandler}, "Save"),
                             React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", 'data-target':"roleModal"}, "Cancel")
                          ));
                 }

   

        return (ModalDialog(  {showOnCreate:  true, renderHeaderText:renderHeaderText, renderBody:renderBody,  closeDialogHandler:  this.closeDialog,  renderFooterButtons:renderFooterButtons, dialogId:"addRoleModal"}));
    }

});




