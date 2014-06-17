/**
* @jsx React.DOM
*/

var UserRoot = React.createClass({

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
        return (<div className="waiting small"></div>)
      }
      
      return null;
   },



   renderRoles: function(){
    if (this.state.roles == undefined || this.state.roles == null){
      return "";
    }

    return ( <RolesBox addRoleClickHandler={this.addRoleClickHandler} 
                       viewCapabilities={this.viewCapabilitiesHandler}  
                       editRole = {this.editRoleHandler}
                       roles = {this.state.roles} isDisabled = {this.state.loading}  courseId ={this.state.currentCourse}/>);
   },

   renderAddRoleDialog: function(){
      if( !this.state.showRoleDialog){
        return null;
      }


      return (<RoleDialog  saveRoleHandler={this.saveRoleHandler} closeAddRoleDialog={this.closeAddRoleDialog} viewMode={this.state.viewMode} courseId={this.state.currentCourse}  role={this.state.roleDialogModel} newRole={this.state.newRole} />);

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
          return (<div className="waiting middle"></div>);
        }

        if(this.state.users == null || this.state.users.length == 0){
          return(<b>No users loaded</b>);
        }
        return (<UserBox  users={this.state.users} showAvailibleTitlesHandler={this.showAvailibleTitlesHandler}  showUserEditDialog={this.showUserEditDialog}/>);
   },

   renderAvailibleTitlesDialog: function(){
      if(this.state.showAvailibleTitles){
         return( <AvailibleTitlesDialog user={this.state.user} closeAvailibleTitles={this.closeAvailibleTitles} />);
      }

      return null;
   },

   renderUserEditDialog: function(){
      if(this.state.showUserEditDialog){
         return( <EditUserDialog user={this.state.user} closeEditUserDialog={this.closeEditUserDialog} />);
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
    
    return(   <div>
              <ul className="nav nav-tabs">
                <li className="active"> 
                    <a href="#users-tab" data-toggle="tab">Users</a>
                </li>
                <li>
                    <a href="#roles-tab" data-toggle="tab">Roles</a>
                </li>
              </ul>

               <div className="tab-content">
                    <div className="tab-pane active" id="users-tab">
                         {this.renderUsers()}
                    </div>
                    <div className="tab-pane" id="roles-tab">
                         <MetadataCourseSelector selectCourseHandler={this.selectCourseHandler} 
                                              availableCourses={this.props.availableCourses}
                                              currentCourse={this.state.currentCourse}      />
                        {this.renderRoles()}


                    </div>
                </div>
                {this.renderAddRoleDialog()}
                {this.renderAvailibleTitlesDialog()}
                {this.renderUserEditDialog()}
            </div>);
   },


 

    render: function() {
       return (
                <div>  
                       {this.renderLoader()}
                       {this.renderTabs()}    
                </div>
            );
    }
});

var EditUserDialog  = React.createClass({

  

    render: function() {
       var self = this;
        var renderHeaderText = function() {
         
             return "User Editing — "+ self.props.user.userName;
           
        };

      
        var renderBody = function(){
            return (<div className="title-table"> 
                      <UserTitlesBox user={self.props.user} />
                    </div>
            );
        };

     
      var  renderFooterButtons = function(){

                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-primary">Save</button>
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="editUserModal">Close</button>
                          </div>);
             
                 };

   

        return (<ModalDialog  showOnCreate = {true} 
                              renderHeaderText={renderHeaderText} 
                              renderBody={renderBody}  
                              closeDialogHandler = {this.closeDialog} 
                              renderFooterButtons={renderFooterButtons} 
                              dialogId="editUserModal"/>);
    }
});

var UserTitlesBox = React.createClass({


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
        return (<div className="waiting middle"></div>);
      }

     var rows = [];
     rows = this.state.roles.map(function (userRole, i) {
        
                return (<div className="role-row">
                          <div className="role-cell">{userRole.titleName}</div>
                         </div>);
          });

     if (rows.length == 0){
       return (<b>No titles are availible</b>);
     }

     return rows;

    },
  render: function(){

      return (<div className="user-titles-container">
                  <div className="roles-table">
                  {this.renderRows()}
                  </div>
              </div>);

  }
});

 var AvailibleTitlesDialog  = React.createClass({

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
          return (<div className="waiting middle"/>);
      }

     var rows = [];
     rows = this.state.titles.map(function (title, i) {
        
            return ( <div className="title-row">
                        <div className="title-cell">{title.titleName}</div>
                        <div className="title-cell"><i>{title.currentRole.name}</i></div>
                      </div>);
          });

     if (rows.length == 0){
       return (<b>No titles are availible</b>);
     }

     return rows;

    },

    render: function() {
       var self = this;

        var renderHeaderText = function() {
         
             return "Titles availible for "+ self.props.user.userName;
           
        };

      
        var renderBody = function(){
            return (<div className="title-table"> 
                     {self.renderRows()}
                    </div>
            );
        };

     
      var  renderFooterButtons = function(){

                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="roleModal">Close</button>
                          </div>);
             
                 };

   

        return (<ModalDialog  showOnCreate = {true} 
                              renderHeaderText={renderHeaderText} 
                              renderBody={renderBody}  
                              closeDialogHandler = {this.closeDialog} 
                              renderFooterButtons={renderFooterButtons} 
                              dialogId="titlesModal"/>);
    }
});




var UserBox = React.createClass({


    renderUsers: function(){
          var self = this;
         var rows = [];
         rows = this.props.users.map(function (user, i) {
        
            return (<UserRow user={user} showAvailibleTitlesHandler={self.props.showAvailibleTitlesHandler} showUserEditDialog={self.props.showUserEditDialog}/>);
          });

     return rows;
    },
    render: function() {
       return (
                <div className="roles-table"> 

                  {this.renderUsers()}

                </div>
            );
    }
});

var UserRow = React.createClass({

    showAvailibleTitlesHandler: function(){
        this.props.showAvailibleTitlesHandler(this.props.user);
    },

    editUser: function(){
        this.props.showUserEditDialog(this.props.user);
    },

    render: function() {
       return (
                <div className="role-row"> 

                 <div className="role-cell role-name">{this.props.user.userName}</div>
                 <div className="role-cell capabilities "> <span className="capabilities-link" onClick={this.showAvailibleTitlesHandler}>{this.props.user.availibleTitlesCount} {this.props.user.availibleTitlesCount == 1? "title" : "titles"}</span></div>
                 <div className="role-cell menu">
                    <button type="button" className="btn btn-default btn-sm"  data-toggle="tooltip"  title="Edit Role" onClick={this.editUser}><span className="glyphicon glyphicon-pencil"></span> </button>
                 </div>
                </div>
            );
     }

});

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
              return (<div className="waiting"></div>);
            }

            if(self.state.role == null){
              return (<b>No capabilities are loaded</b>);
            }


            return (<div>

                       <EditRoleBox role={self.state.role} newRole={self.props.newRole} viewMode={self.props.viewMode} editRoleHandler={self.editRoleHandler}/>
                    </div>
            );
        };

     
      var  renderFooterButtons = function(){
                  if(self.props.viewMode){
                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="roleModal">Close</button>
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

var EditRoleBox = React.createClass({

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
     return(<div className="role-name-editor">
              Role name
              <input type="text" className="form-control" value={this.props.role.name} onChange={this.dataChangeHandler}  placeholder="Enter role name"/>
             </div>);
  },

  renderCapabilities: function(){

      if(this.props.role.capabilityGroups == undefined || this.props.role.capabilityGroups.length == 0){
        return (<b>There are no capabilities</b>);
      }

     return (<CapabilitiesBox role={this.props.role} editRoleHandler={this.props.editRoleHandler} viewMode={this.state.viewMode} />);  
  },
  render: function(){
      return(
        <div>
            {this.renderRoleNameEditor()}
            {this.renderCapabilities()}
        </div>
        );
    }
});

var CapabilitiesBox = React.createClass({


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
            return (<CapabilityGroup capabilityGroup={capabilityGroup} editCapabilityGroup={self.editCapabilityGroup} viewMode={self.props.viewMode}/>);
          });

        return capabilityGroups;
    },

    render: function(){
    return(
      <div className="capabilities-box">
          {this.renderCapabilities()}
      </div>
      );
  }
});

var CapabilityGroup = React.createClass({


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
            return (<CapabilityItem capability={capability} editCapability={self.editCapability} viewMode={self.props.viewMode}/>);
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
        <div className="capabilities-group">
           <input type="checkbox"  disabled={this.props.viewMode} checked={this.isGroupSelected()} onChange={this.switchGroup} /> <b> {this.props.capabilityGroup.name}</b>
           {this.renderCapabilities()}
        </div>
        );
    }

 }); 


var CapabilityItem = React.createClass({

  switchCapability: function(){
    var capability = this.props.capability;
    capability.isActive = !capability.isActive;
    this.props.editCapability(capability);
  },

   render: function(){
      return(
        <div className="capability-item">
           <input type="checkbox" disabled={this.props.viewMode} checked={this.props.capability.isActive} onChange={this.switchCapability}/> <span> {this.props.capability.name}</span>
        </div>
        );
    }
 }); 


var RolesBox = React.createClass({

  renderRoles: function(){
     var self= this;

     var rows = [];
     rows = this.props.roles.map(function (role, i) {
        
            return (<RoleRow role={role} editRole = {self.props.editRole}  viewCapabilities = {self.props.viewCapabilities} />);
          });

     return rows;

  },

  addRoleClickHandler: function(){
      this.props.addRoleClickHandler();
  },

  render: function() {
       return (
                <div className = "roles-box"> 
                        <button className="btn btn-primary " data-toggle="modal"  title="Add role" onClick={this.addRoleClickHandler} >
                             Add role
                        </button>
                        <br /><br />
                        <div className="roles-table">
                          {this.renderRoles()}
                        </div> 
                        
                </div>
            );
    }

  });


var RoleRow = React.createClass({

  viewCapabilities: function(){
     this.props.viewCapabilities(this.props.role);
  },

  editRole: function(){
     this.props.editRole(this.props.role);
  },

  render: function() {
       return (
                <div className="role-row">

                      <div className="role-cell role-name"> {this.props.role.name} </div>
                      <div className="role-cell capabilities">
                          <span className="capabilities-link" onClick={this.viewCapabilities}>{this.props.role.activeCapabiltiesCount} capabilit{this.props.role.activeCapabiltiesCount == 1? "y": "ies"} </span>
                      </div>
                      <div className="role-cell menu">

                          <div className="menu-container-main version-history">
                          <button type="button" className="btn btn-default btn-sm"  data-toggle="tooltip"  title="Edit Role" onClick={this.editRole}><span className="glyphicon glyphicon-pencil"></span> </button>
                          <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip" title="Remove Role"><span className="glyphicon glyphicon-trash"></span></button>
                         
                       </div>

                       </div>
                        
                </div>
            );
    }

 });