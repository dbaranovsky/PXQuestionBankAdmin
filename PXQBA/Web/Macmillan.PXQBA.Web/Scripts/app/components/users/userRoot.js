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
                 currentPage: 1,
                 totalPages: 1,
                 page: null,
                 deleteRole: false
               });
    },

 
   selectCourseHandler: function(items) {
      this.setState({currentCourse: items[0], loading: true});
      var self = this;
      userManager.getRolesForCourse(items[0]).done(this.setRoles).error(function(e){self.setState({loading: false});});
   
   },
    


    componentDidMount: function(){
      this.getPage(1);
    },

    getPage: function(page){
          this.setState({usersLoading: true});
         var self = this;
         userManager.getUsers(page).done(self.processUsers).error(function(e){self.setState({usersLoading: false});});
    },

    processUsers: function(response){
       
       this.setState({usersLoading: false,
                      totalPages: response.totalPages,
                      page: response.users,
                      currentPage: response.currentPage
                    });

    },

    setRoles: function(data){
      this.setState({loading: false, roles: data});
    },
  
   renderLoader: function() {
      if(this.state.loading) {
        return (React.DOM.div({className: "waiting small"}))
      }
      
      return null;
   },



   renderRoles: function(){
    if (this.state.roles == undefined || this.state.roles == null){
      return "";
    }

    return ( RolesBox({addRoleClickHandler: this.addRoleClickHandler, 
                       removeRoleHandler: this.showRemoveRoleDialog, 
                       viewCapabilities: this.viewCapabilitiesHandler, 
                       editRole: this.editRoleHandler, 
                       roles: this.state.roles, isDisabled: this.state.loading, courseId: this.state.currentCourse}));
   },

   renderAddRoleDialog: function(){
      if( !this.state.showRoleDialog){
        return null;
      }


      return (RoleDialog({saveRoleHandler: this.saveRoleHandler, removeRoleHandler: this.removeRoleHandler, closeAddRoleDialog: this.closeAddRoleDialog, 
                            viewMode: this.state.viewMode, courseId: this.state.currentCourse, role: this.state.roleDialogModel, newRole: this.state.newRole, 
                            deleting: this.state.deleteRole}));

   },

   addRoleClickHandler: function(){
      this.setState({showRoleDialog: true, viewMode: false, roleDialogModel: this.state.newRoleTemplate, newRole: true});
   },

   closeAddRoleDialog: function(){
      this.setState({showRoleDialog: false, newRole: false, deleteRole: false});
   },

   editRoleHandler: function(role){
      this.setState({showRoleDialog: true, viewMode: false, roleDialogModel: role});
   },

   showRemoveRoleDialog: function(role){
      this.setState({showRoleDialog: true, viewMode: false,newRole: false, deleteRole: true, roleDialogModel: role});
   },

   removeRoleHandler: function(role){
     this.setState({loading: true});
     var self = this;
     userManager.removeRole(role, this.state.currentCourse).done(this.reloadRoles).error(function(e){self.setState({loading: false});});
   },

   viewCapabilitiesHandler: function(role){
     this.setState({showRoleDialog: true, viewMode: true, roleDialogModel: role});
   },

   saveRoleHandler: function(role, isNew){
         var self = this;
         this.setState({loading: true});
         userManager.saveRole(role, this.state.currentCourse, isNew).done(this.reloadRoles).error(function(e){self.setState({loading: false});});
   },

   reloadRoles: function(){
          var self = this;
          userManager.getRolesForCourse(this.state.currentCourse).done(this.setRoles).error(function(e){self.setState({loading: false});});
   },

   showAvailibleTitlesHandler: function(user){
      this.setState({showAvailibleTitles: true, user: user});
   },

   showUserEditDialog: function(user){
      this.setState({showUserEditDialog: true, user: user});
   },



   renderUsers: function(){

        if(this.state.page == null || this.state.page.length == 0){
          return(React.DOM.b(null, "No data availible"));
        }
        return (UserBox({users: this.state.page, showAvailibleTitlesHandler: this.showAvailibleTitlesHandler, showUserEditDialog: this.showUserEditDialog}));
   },

   renderAvailibleTitlesDialog: function(){
      if(this.state.showAvailibleTitles){
         return( AvailibleTitlesDialog({user: this.state.user, closeAvailibleTitles: this.closeAvailibleTitles}));
      }

      return null;
   },

   renderUserEditDialog: function(){
      if(this.state.showUserEditDialog){
         return( EditUserDialog({user: this.state.user, closeEditUserDialog: this.closeEditUserDialog, updateAvailibleTitles: this.updateAvailibleTitles}));
      }

      return null;
   },

   closeAvailibleTitles: function(){
      this.setState({showAvailibleTitles: false, user: null});
   },

   closeEditUserDialog: function(){
       this.setState({showUserEditDialog: false, user: null});
   },

  updateAvailibleTitles: function(userId, count){


        var page = this.state.page;
        for(var i in page){
          if (page[i].id == userId){
              page[i].productCoursesCount = count;
              break;
          }
        }

        this.setState({page: page});
  },

  changePage: function(page){
   this.getPage(page);
  },

   renderTabs: function() {
    return(   React.DOM.div(null, 
              React.DOM.ul({className: "nav nav-tabs"}, 
                React.DOM.li({className: "active"}, 
                    React.DOM.a({href: "#users-tab", 'data-toggle': "tab"}, "Users")
                ), 
                React.DOM.li(null, 
                    React.DOM.a({href: "#roles-tab", 'data-toggle': "tab"}, "Roles")
                )
              ), 

               React.DOM.div({className: "tab-content"}, 
                    React.DOM.div({className: "tab-pane active", id: "users-tab"}, 
                         this.renderUsers(), 
                          React.DOM.div({className: "question-grid-item"}, 
                             Paginator({metadata: {
                                currentPage: this.state.currentPage,
                                totalPages: this.state.totalPages}, 
                                customPaginatorClickHandle: this.changePage})
                         )
                    ), 
                    React.DOM.div({className: "tab-pane", id: "roles-tab"}, 
                         MetadataCourseSelector({selectCourseHandler: this.selectCourseHandler, 
                                              availableCourses: this.props.availableCourses, 
                                              currentCourse: this.state.currentCourse}), 
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
                     this.state.usersLoading ? Loader(null) :"", 
                       this.renderLoader(), 
                       this.renderTabs()
                )
            );
    }
});