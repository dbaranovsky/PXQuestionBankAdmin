/**
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

    return ( RolesBox( {addRoleClickHandler:this.addRoleClickHandler, roles:  this.state.roles, isDisabled:  this.state.loading,  courseId: this.state.currentCourse}));
   },

   renderAddRoleDialog: function(){
      if( !this.state.showAddRoleDialog){
        return null;
      }


      return (AddRoleDialog( {closeAddRoleDialog:this.closeAddRoleDialog,  role:this.state.roles[0]} ));

   },

   addRoleClickHandler: function(){
      this.setState({showAddRoleDialog: true});
   },

   closeAddRoleDialog: function(){
      this.setState({showAddRoleDialog: false});
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

var AddRoleDialog = React.createClass({displayName: 'AddRoleDialog',

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
        
            return (React.DOM.div(null, 

                       AddRoleBox(null )
                    )
            );
        };

      var  renderFooterButtons = function(){
                   return (React.DOM.div( {className:"modal-footer"},  
                             React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", 'data-target':"addRoleModal"}, "Cancel")
                          ));
                 }

   

        return (ModalDialog(  {showOnCreate:  true, renderHeaderText:renderHeaderText, renderBody:renderBody,  closeDialogHandler:  this.closeDialog,  renderFooterButtons:renderFooterButtons, dialogId:"addRoleModal"}));
    }

});

var AddRoleBox = React.createClass({displayName: 'AddRoleBox',

dataChangeHandler:function(text){

},

render: function(){
    return(
      React.DOM.div(null, 
          TextEditor( {dataChangeHandler:this.dataChangeHandler})
      )
      );
  }
});


var RolesBox = React.createClass({displayName: 'RolesBox',


  renderRoles: function(){
     var self= this;

     var rows = [];
     rows = this.props.roles.map(function (role, i) {
        
            return (RoleRow( {role:role}  ));
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

  render: function() {
       return (
                React.DOM.div( {className:"role-row"}, 

                      React.DOM.div( {className:"role-cell role-name"},  " ", this.props.role.name, " " ),
                      React.DOM.div( {className:"role-cell capabilities"}, this.props.role.activeCapabiltiesCount, " capabilit",this.props.role.activeCapabiltiesCount == 1? "y": "ies", " " ),
                      React.DOM.div( {className:"role-cell menu"}, 

                          React.DOM.div( {className:"menu-container-main version-history"}, 
                          React.DOM.button( {type:"button", className:"btn btn-default btn-sm",  'data-toggle':"tooltip",  title:"Edit Role"} , React.DOM.span( {className:"glyphicon glyphicon-pencil"}), " " ),
                          React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip", title:"Remove Role", onClick:this.props.renderPreview}, React.DOM.span( {className:"glyphicon glyphicon-trash"}))
                         
                       )

                       )
                        
                )
            );
    }

 });