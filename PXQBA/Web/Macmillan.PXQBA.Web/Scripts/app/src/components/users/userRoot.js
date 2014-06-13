/**
* @jsx React.DOM
*/

var UserRoot = React.createClass({

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

    return ( <RolesBox addRoleClickHandler={this.addRoleClickHandler} roles = {this.state.roles} isDisabled = {this.state.loading}  courseId ={this.state.currentCourse}/>);
   },

   renderAddRoleDialog: function(){
      if( !this.state.showAddRoleDialog){
        return null;
      }


      return (<AddRoleDialog closeAddRoleDialog={this.closeAddRoleDialog}  role={this.state.roles[0]} />);

   },

   addRoleClickHandler: function(){
      this.setState({showAddRoleDialog: true});
   },

   closeAddRoleDialog: function(){
      this.setState({showAddRoleDialog: false});
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
                         TBD
                    </div>
                    <div className="tab-pane" id="roles-tab">
                         <MetadataCourseSelector selectCourseHandler={this.selectCourseHandler} 
                                              availableCourses={this.props.availableCourses}
                                              currentCourse={this.state.currentCourse}      />
                        {this.renderRoles()}


                    </div>
                </div>
                {this.renderAddRoleDialog()}
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

var AddRoleDialog = React.createClass({

    closeDialog: function(){
         $(this.getDOMNode()).modal("hide");
         this.props.closeAddRoleDialog();
    },
   


    render: function() {
       var self = this;
        var renderHeaderText = function() {
             return "Add role";
        };

      
        var renderBody = function(){
        
            return (<div>

                       <AddRoleBox />
                    </div>
            );
        };

      var  renderFooterButtons = function(){
                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="addRoleModal">Cancel</button>
                          </div>);
                 }

   

        return (<ModalDialog  showOnCreate = {true} renderHeaderText={renderHeaderText} renderBody={renderBody}  closeDialogHandler = {this.closeDialog}  renderFooterButtons={renderFooterButtons} dialogId="addRoleModal"/>);
    }

});

var AddRoleBox = React.createClass({

dataChangeHandler:function(text){

},

render: function(){
    return(
      <div>
          <TextEditor dataChangeHandler={this.dataChangeHandler}/>
      </div>
      );
  }
});


var RolesBox = React.createClass({


  renderRoles: function(){
     var self= this;

     var rows = [];
     rows = this.props.roles.map(function (role, i) {
        
            return (<RoleRow role={role}  />);
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

  render: function() {
       return (
                <div className="role-row">

                      <div className="role-cell role-name"> {this.props.role.name} </div>
                      <div className="role-cell capabilities">{this.props.role.activeCapabiltiesCount} capabilit{this.props.role.activeCapabiltiesCount == 1? "y": "ies"} </div>
                      <div className="role-cell menu">

                          <div className="menu-container-main version-history">
                          <button type="button" className="btn btn-default btn-sm"  data-toggle="tooltip"  title="Edit Role" ><span className="glyphicon glyphicon-pencil"></span> </button>
                          <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip" title="Remove Role" onClick={this.props.renderPreview}><span className="glyphicon glyphicon-trash"></span></button>
                         
                       </div>

                       </div>
                        
                </div>
            );
    }

 });