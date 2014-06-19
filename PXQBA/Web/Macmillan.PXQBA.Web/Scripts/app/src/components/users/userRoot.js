﻿/**
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
                 users: [],
                 currentPage: 1,
                 totalPages: 1,
                 pages: [],
                 usersPerPage: 20,
                 page: null
               });
    },

 
   selectCourseHandler: function(items) {
      this.setState({currentCourse: items[0], loading: true});
      var self = this;
      userManager.getRolesForCourse(items[0]).done(this.setRoles).error(function(e){self.setState({loading: false});});
   
   },
    


    componentDidMount: function(){
      var self = this;
     userManager.getUsers().done(self.processUsers).error(function(e){self.setState({usersLoading: false});});
    },

    processUsers: function(users){
        var totalPages =  ~~(users.length/this.state.usersPerPage);
        if(users.length%this.state.usersPerPage != 0){
          totalPages++;
        } 
         var pages = [];
        for (i=0; i< totalPages; i++){
            pages.push(users.slice(i*this.state.usersPerPage, i*this.state.usersPerPage+this.state.usersPerPage));
        }
       this.setState({usersLoading: false,
                      pages: pages,
                      totalPages: totalPages,
                      page: pages[0]
                    });

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
                       removeRoleHandler ={this.removeRoleHandler}
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

   removeRoleHandler: function(role){
     this.setState({loading: true});
     var self = this;
     userManager.removeRole(role, this.state.currentCourse).done(this.reloadRoles).error(function(e){self.setState({loading: false});});
   },

   viewCapabilitiesHandler: function(role){
     this.setState({showRoleDialog: true, viewMode: true, roleDialogModel: role});
   },

   saveRoleHandler: function(role){
         var self = this;
         this.setState({loading: true});
         userManager.saveRole(role, this.state.currentCourse).done(this.reloadRoles).error(function(e){self.setState({loading: false});});
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
        if(this.state.usersLoading){
          return (<div className="waiting middle"></div>);
        }

        if(this.state.page == null || this.state.page.length == 0){
          return(<b>No data availible</b>);
        }
        return (<UserBox  users={this.state.page} showAvailibleTitlesHandler={this.showAvailibleTitlesHandler}  showUserEditDialog={this.showUserEditDialog}/>);
   },

   renderAvailibleTitlesDialog: function(){
      if(this.state.showAvailibleTitles){
         return( <AvailibleTitlesDialog user={this.state.user} closeAvailibleTitles={this.closeAvailibleTitles} />);
      }

      return null;
   },

   renderUserEditDialog: function(){
      if(this.state.showUserEditDialog){
         return( <EditUserDialog user={this.state.user} closeEditUserDialog={this.closeEditUserDialog} updateAvailibleTitles={this.updateAvailibleTitles} />);
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


        var pages = this.state.pages;
        var page = this.state.page;
        for(var i in page){
          if (page[i].id == userId){
              page[i].availibleTitlesCount = count;
              break;
          }
        }

        pages[this.state.currentPage-1] = page;


        this.setState({pages: pages, page: page});
  },

  changePage: function(page){
    this.setState({page: this.state.pages[page-1], currentPage: parseInt(page)});
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
                          <div className="question-grid-item"> 
                             <Paginator metadata={{
                                currentPage: this.state.currentPage,
                                totalPages: this.state.totalPages}} 
                                customPaginatorClickHandle={this.changePage}/>
                         </div> 
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