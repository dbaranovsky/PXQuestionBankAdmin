/**
* @jsx React.DOM
*/

var UserRoot = React.createClass({displayName: 'UserRoot',

   getInitialState: function() {
        return ({ 
                 loading: false,
                 currentCourse: null,
                 roles: null,
                 showAddRoleDialog: false,
                 usersLoading: true,
                 users: []
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

     userManager.getUsers().done(function(users){
          self.setState({users: users, usersLoading: false});
     }).error(function(e){self.setState({usersLoading: false});});

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


      return (RoleDialog(  {saveRoleHandler:this.saveRoleHandler, closeAddRoleDialog:this.closeAddRoleDialog, viewMode:this.state.viewMode, courseId:this.state.currentCourse,  role:this.state.roleDialogModel, newRole:this.state.newRole} ));

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

   showAvailibleTitlesHandler: function(user){
      this.setState({showAvailibleTitles: true, user: user});
   },

   showUserEditDialog: function(user){
      this.setState({showUserEditDialog: true, user: user});
   },



   renderUsers: function(){
        if(this.state.usersLoading){
          return (React.DOM.div( {className:"waiting middle"}));
        }

        if(this.state.users == null || this.state.users.length == 0){
          return(React.DOM.b(null, "No users loaded"));
        }
        return (UserBox(  {users:this.state.users, showAvailibleTitlesHandler:this.showAvailibleTitlesHandler,  showUserEditDialog:this.showUserEditDialog}));
   },

   renderAvailibleTitlesDialog: function(){
      if(this.state.showAvailibleTitles){
         return( AvailibleTitlesDialog( {user:this.state.user, closeAvailibleTitles:this.closeAvailibleTitles} ));
      }

      return null;
   },

   renderUserEditDialog: function(){
      if(this.state.showUserEditDialog){
         return( EditUserDialog( {user:this.state.user, closeEditUserDialog:this.closeEditUserDialog} ));
      }

      return null;
   },

   closeAvailibleTitles: function(){
      this.setState({showAvailibleTitles: false, user: null});
   },

   closeEditUserDialog: function(){
       this.setState({showUserEditDialog: false, user: null});
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
                         this.renderUsers()
                    ),
                    React.DOM.div( {className:"tab-pane", id:"roles-tab"}, 
                         MetadataCourseSelector( {selectCourseHandler:this.selectCourseHandler, 
                                              availableCourses:this.props.availableCourses,
                                              currentCourse:this.state.currentCourse}      ),
                        this.renderRoles()


                    )
                ),
                this.renderAddRoleDialog(),
                this.renderAvailibleTitlesDialog(),
                this.renderUserEditDialog()
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

var EditUserDialog  = React.createClass({displayName: 'EditUserDialog',

  

    render: function() {
       var self = this;
        var renderHeaderText = function() {
         
             return "User Editing — "+ self.props.user.userName;
           
        };

      
        var renderBody = function(){
            return (React.DOM.div( {className:"title-table"},  
                      UserTitlesBox( {user:self.props.user} )
                    )
            );
        };

     
      var  renderFooterButtons = function(){

                   return (React.DOM.div( {className:"modal-footer"},  
                             React.DOM.button( {type:"button", className:"btn btn-primary"}, "Save"),
                             React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", 'data-target':"editUserModal"}, "Close")
                          ));
             
                 };

   

        return (ModalDialog(  {showOnCreate:  true, 
                              renderHeaderText:renderHeaderText, 
                              renderBody:renderBody,  
                              closeDialogHandler:  this.closeDialog, 
                              renderFooterButtons:renderFooterButtons, 
                              dialogId:"editUserModal"}));
    }
});

var UserTitlesBox = React.createClass({displayName: 'UserTitlesBox',


    getInitialState: function(){
        return({roles: [], loading: true});
    },
    closeDialog: function(){
        this.props.closeEditUserDialog();
    },  

    componentDidMount: function(){
        var self = this;
        userManager.getUserRoles(this.props.user.id).done(this.setRoles).error(function(e){
            self.setState({loading: false});
        });
    },

    setRoles: function(roles){
        this.setState({roles: roles, loading: false});
    },

    renderRows: function(){
     var self= this;

     if (this.state.loading){
        return (React.DOM.div( {className:"waiting middle"}));
      }

     var rows = [];
     rows = this.state.roles.map(function (userRole, i) {
        
                return (React.DOM.div( {className:"role-row"}, 
                          React.DOM.div( {className:"role-cell"}, userRole.titleName)
                         ));
          });

     if (rows.length == 0){
       return (React.DOM.b(null, "No titles are availible"));
     }

     return rows;

    },
  render: function(){

      return (React.DOM.div( {className:"user-titles-container"}, 
                  React.DOM.div( {className:"roles-table"}, 
                  this.renderRows()
                  )
              ));

  }
});

 var AvailibleTitlesDialog  = React.createClass({displayName: 'AvailibleTitlesDialog',

    getInitialState: function(){
        return({titles: [], loading: true});
    },
    closeDialog: function(){
        this.props.closeAvailibleTitles();
    },  

    componentDidMount: function(){
        var self = this;
        userManager.getAvailibleTitles(this.props.user.id).done(this.setTitles).error(function(e){
            self.setState({loading: false});
        });
    },

    setTitles: function(titles){
        this.setState({titles: titles, loading: false});
    },

    renderRows: function(){
     var self= this;

      if(this.state.loading){
          return (React.DOM.div( {className:"waiting middle"}));
      }

     var rows = [];
     rows = this.state.titles.map(function (title, i) {
        
            return ( React.DOM.div( {className:"title-row"}, 
                        React.DOM.div( {className:"title-cell"}, title.titleName),
                        React.DOM.div( {className:"title-cell"}, React.DOM.i(null, title.currentRole.name))
                      ));
          });

     if (rows.length == 0){
       return (React.DOM.b(null, "No titles are availible"));
     }

     return rows;

    },

    render: function() {
       var self = this;

        var renderHeaderText = function() {
         
             return "Titles availible for "+ self.props.user.userName;
           
        };

      
        var renderBody = function(){
            return (React.DOM.div( {className:"title-table"},  
                     self.renderRows()
                    )
            );
        };

     
      var  renderFooterButtons = function(){

                   return (React.DOM.div( {className:"modal-footer"},  
                             React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", 'data-target':"roleModal"}, "Close")
                          ));
             
                 };

   

        return (ModalDialog(  {showOnCreate:  true, 
                              renderHeaderText:renderHeaderText, 
                              renderBody:renderBody,  
                              closeDialogHandler:  this.closeDialog, 
                              renderFooterButtons:renderFooterButtons, 
                              dialogId:"titlesModal"}));
    }
});




var UserBox = React.createClass({displayName: 'UserBox',


    renderUsers: function(){
          var self = this;
         var rows = [];
         rows = this.props.users.map(function (user, i) {
        
            return (UserRow( {user:user, showAvailibleTitlesHandler:self.props.showAvailibleTitlesHandler, showUserEditDialog:self.props.showUserEditDialog}));
          });

     return rows;
    },
    render: function() {
       return (
                React.DOM.div( {className:"roles-table"},  

                  this.renderUsers()

                )
            );
    }
});

var UserRow = React.createClass({displayName: 'UserRow',

    showAvailibleTitlesHandler: function(){
        this.props.showAvailibleTitlesHandler(this.props.user);
    },

    editUser: function(){
        this.props.showUserEditDialog(this.props.user);
    },

    render: function() {
       return (
                React.DOM.div( {className:"role-row"},  

                 React.DOM.div( {className:"role-cell role-name"}, this.props.user.userName),
                 React.DOM.div( {className:"role-cell capabilities " },  " ", React.DOM.span( {className:"capabilities-link", onClick:this.showAvailibleTitlesHandler}, this.props.user.availibleTitlesCount, " ", this.props.user.availibleTitlesCount == 1? "title" : "titles")),
                 React.DOM.div( {className:"role-cell menu"}, 
                    React.DOM.button( {type:"button", className:"btn btn-default btn-sm",  'data-toggle':"tooltip",  title:"Edit Role", onClick:this.editUser}, React.DOM.span( {className:"glyphicon glyphicon-pencil"}), " " )
                 )
                )
            );
     }

});

var RoleDialog = React.createClass({displayName: 'RoleDialog',

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
        
            if (self.state.loading){
              return (React.DOM.div( {className:"waiting"}));
            }

            if(self.state.role == null){
              return (React.DOM.b(null, "No capabilities are loaded"));
            }


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