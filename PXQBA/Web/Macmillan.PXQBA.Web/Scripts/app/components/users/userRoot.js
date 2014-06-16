﻿/**
* @jsx React.DOM
*/

var UserRoot = React.createClass({displayName: 'UserRoot',

   getInitialState: function() {
        return ({ 
                 loading: false,
                 currentCourse: null,
                 roles: null,
                 showAddRoleDialog: false
               });
    },

 
   selectCourseHandler: function(items) {
      this.setState({currentCourse: items[0], loading: true});
      var self = this;
      userManager.getRolesForCourse(items[0]).done(this.setRoles).error(function(e){self.setState({loading: false});});
   
   },
    

    componentDidMount: function(){
      var self = this;
     userManager.getNewRoleTemplate().done(function(template){
          self.setState({newRoleTemplate: template});
      });
    },


    setRoles: function(data){
      this.setState({loading: false, roles: data});
    },
  
   renderLoader: function() {
      if(this.state.loading) {
        return (React.DOM.div( {className:"waiting small"}))
      }
      
      return null;
   },



   renderRoles: function(){
    if (this.state.roles == undefined || this.state.roles == null){
      return "";
    }

    return ( RolesBox( {addRoleClickHandler:this.addRoleClickHandler, 
                       viewCapabilities:this.viewCapabilitiesHandler,  
                       editRole:  this.editRoleHandler,
                       roles:  this.state.roles, isDisabled:  this.state.loading,  courseId: this.state.currentCourse}));
   },

   renderAddRoleDialog: function(){
      if( !this.state.showRoleDialog){
        return null;
      }


      return (RoleDialog(  {saveRoleHandler:this.saveRoleHandler, closeAddRoleDialog:this.closeAddRoleDialog, viewMode:this.state.viewMode,  role:this.state.roleDialogModel, newRole:this.state.newRole} ));

   },

   addRoleClickHandler: function(){
      this.setState({showRoleDialog: true, viewMode: false, roleDialogModel: this.state.newRoleTemplate, newRole: true});
   },

   closeAddRoleDialog: function(){
      this.setState({showRoleDialog: false, newRole: false});
   },

   editRoleHandler: function(role){
      this.setState({showRoleDialog: true, viewMode: false, roleDialogModel: role});
   },

   viewCapabilitiesHandler: function(role){
     this.setState({showRoleDialog: true, viewMode: true, roleDialogModel: role});
   },

   saveRoleHandler: function(role){
         this.setState({loading: true});
         userManager.saveRole(role, this.state.currentCourse).done(this.doneSaving).error(function(e){self.setState({loading: false});});
   },

   doneSaving: function(){
          userManager.getRolesForCourse(this.state.currentCourse).done(this.setRoles).error(function(e){self.setState({loading: false});});
   },
   renderTabs: function() {
    
    return(   React.DOM.div(null, 
              React.DOM.ul( {className:"nav nav-tabs"}, 
                React.DOM.li( {className:"active"},  
                    React.DOM.a( {href:"#users-tab", 'data-toggle':"tab"}, "Users")
                ),
                React.DOM.li(null, 
                    React.DOM.a( {href:"#roles-tab", 'data-toggle':"tab"}, "Roles")
                )
              ),

               React.DOM.div( {className:"tab-content"}, 
                    React.DOM.div( {className:"tab-pane active", id:"users-tab"}, 
                         "TBD"
                    ),
                    React.DOM.div( {className:"tab-pane", id:"roles-tab"}, 
                         MetadataCourseSelector( {selectCourseHandler:this.selectCourseHandler, 
                                              availableCourses:this.props.availableCourses,
                                              currentCourse:this.state.currentCourse}      ),
                        this.renderRoles()


                    )
                ),
                this.renderAddRoleDialog()
            ));
   },


 

    render: function() {
       return (
                React.DOM.div(null,   
                       this.renderLoader(),
                       this.renderTabs()    
                )
            );
    }
});

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

var EditRoleBox = React.createClass({displayName: 'EditRoleBox',

 getInitialState: function(){
  return ({
    editMode: this.props.role == undefined || this.props.viewMode? false : true,
    viewMode: this.props.viewMode == undefined? false: this.props.viewMode,
    newRole: this.props.newRole == undefined? false : this.props.newRole
  });
 },


dataChangeHandler:function(event){
    var role = this.props.role;
    role.name = event.target.value;
    this.props.editRoleHandler(role);
},



renderRoleNameEditor: function(){
  if (this.state.viewMode){
    return null;
  }
   return(React.DOM.div( {className:"role-name-editor"}, 
            "Role name",
            React.DOM.input( {type:"text", className:"form-control", value:this.props.role.name, onChange:this.dataChangeHandler,  placeholder:"Enter role name"})
           ));
},

renderCapabilities: function(){

    if(this.props.role.capabilityGroups == undefined || this.props.role.capabilityGroups.length == 0){
      return (React.DOM.b(null, "There are no capabilities"));
    }

   return (CapabilitiesBox( {role:this.props.role, editRoleHandler:this.props.editRoleHandler, viewMode:this.state.viewMode} ));  
},
render: function(){
    return(
      React.DOM.div(null, 
          this.renderRoleNameEditor(),
          this.renderCapabilities()
      )
      );
  }
});

var CapabilitiesBox = React.createClass({displayName: 'CapabilitiesBox',


    editCapabilityGroup: function(capabilityGroup){
      var role = this.props.role;
      var capabilityGroups = this.props.role.capabilityGroups;
      var newCapablilityGroups = [];
      $.each(capabilityGroups, function(index, value){
          if (value.name == capabilityGroup.name){
            newCapablilityGroups.push(capabilityGroup);
          }else{
            newCapablilityGroups.push(value);
          }
      });

       role.capabilityGroups = newCapablilityGroups;
      this.props.editRoleHandler(role);
    },

    renderCapabilities: function(){
        var capabilityGroups = [];
        var self = this;
        capabilityGroups = this.props.role.capabilityGroups.map(function (capabilityGroup, i) {
            return (CapabilityGroup( {capabilityGroup:capabilityGroup, editCapabilityGroup:self.editCapabilityGroup, viewMode:self.props.viewMode}));
          });

        return capabilityGroups;
    },

    render: function(){
    return(
      React.DOM.div( {className:"capabilities-box"}, 
          this.renderCapabilities()
      )
      );
  }
});

var CapabilityGroup = React.createClass({displayName: 'CapabilityGroup',


    editCapability: function(capability){
        var capabilityGroup = this.props.capabilityGroup;
        var capabilities = capabilityGroup.capabilities;
        var newCapablilies = [];

          $.each(capabilities, function(index, value){
          if (value.id == capability.id){
            newCapablilies.push(capability);
          }else{
            newCapablilies.push(value);
          }
         });  


        capabilityGroup.capabilities = newCapablilies;
        this.props.editCapabilityGroup(capabilityGroup);

    },

    renderCapabilities: function(){
        var capabilities = [];
        var self = this;
          capabilities = this.props.capabilityGroup.capabilities.map(function (capability, i) {
            return (CapabilityItem( {capability:capability, editCapability:self.editCapability, viewMode:self.props.viewMode}));
          });

        return capabilities;

    },   


    isGroupSelected: function(){
      var isActiveInGroup = $.grep(this.props.capabilityGroup.capabilities, function(el){return el.isActive;}).length;
      var capabilitiesCount = this.props.capabilityGroup.capabilities.length;

      return isActiveInGroup == capabilitiesCount;
    },

    switchGroup: function(){
        var capabilityGroup = this.props.capabilityGroup;
        var capabilities = capabilityGroup.capabilities;
        var isGroupSelected = !this.isGroupSelected();
        var newCapablilies = [];

          $.each(capabilities, function(index, value){
            value.isActive = isGroupSelected;
            newCapablilies.push(value);    
         });  

        capabilityGroup.capabilities = newCapablilies;
        this.props.editCapabilityGroup(capabilityGroup);
    },

    render: function(){
     
      return(
        React.DOM.div( {className:"capabilities-group"}, 
           React.DOM.input( {type:"checkbox",  disabled:this.props.viewMode, checked:this.isGroupSelected(), onChange:this.switchGroup} ), " ", React.DOM.b(null,  " ", this.props.capabilityGroup.name),
           this.renderCapabilities()
        )
        );
    }

 }); 


var CapabilityItem = React.createClass({displayName: 'CapabilityItem',

  switchCapability: function(){
    var capability = this.props.capability;
    capability.isActive = !capability.isActive;
    this.props.editCapability(capability);
  },

   render: function(){
      return(
        React.DOM.div( {className:"capability-item"}, 
           React.DOM.input( {type:"checkbox", disabled:this.props.viewMode, checked:this.props.capability.isActive, onChange:this.switchCapability}), " ", React.DOM.span(null,  " ", this.props.capability.name)
        )
        );
    }
 }); 


var RolesBox = React.createClass({displayName: 'RolesBox',

  renderRoles: function(){
     var self= this;

     var rows = [];
     rows = this.props.roles.map(function (role, i) {
        
            return (RoleRow( {role:role, editRole:  self.props.editRole,  viewCapabilities:  self.props.viewCapabilities} ));
          });

     return rows;

  },

  addRoleClickHandler: function(){
      this.props.addRoleClickHandler();
  },

  render: function() {
       return (
                React.DOM.div( {className:  "roles-box"},  
                        React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal",  title:"Add role", onClick:this.addRoleClickHandler} , 
                             "Add role"
                        ),
                        React.DOM.br(null ),React.DOM.br(null ),
                        React.DOM.div( {className:"roles-table"}, 
                          this.renderRoles()
                        ) 
                        
                )
            );
    }

  });


var RoleRow = React.createClass({displayName: 'RoleRow',

  viewCapabilities: function(){
     this.props.viewCapabilities(this.props.role);
  },

  editRole: function(){
     this.props.editRole(this.props.role);
  },

  render: function() {
       return (
                React.DOM.div( {className:"role-row"}, 

                      React.DOM.div( {className:"role-cell role-name"},  " ", this.props.role.name, " " ),
                      React.DOM.div( {className:"role-cell capabilities"}, 
                          React.DOM.span( {className:"capabilities-link", onClick:this.viewCapabilities}, this.props.role.activeCapabiltiesCount, " capabilit",this.props.role.activeCapabiltiesCount == 1? "y": "ies", " " )
                      ),
                      React.DOM.div( {className:"role-cell menu"}, 

                          React.DOM.div( {className:"menu-container-main version-history"}, 
                          React.DOM.button( {type:"button", className:"btn btn-default btn-sm",  'data-toggle':"tooltip",  title:"Edit Role", onClick:this.editRole}, React.DOM.span( {className:"glyphicon glyphicon-pencil"}), " " ),
                          React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip", title:"Remove Role"}, React.DOM.span( {className:"glyphicon glyphicon-trash"}))
                         
                       )

                       )
                        
                )
            );
    }

 });