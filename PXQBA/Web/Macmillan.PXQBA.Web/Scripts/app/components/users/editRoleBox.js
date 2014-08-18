/**
* @jsx React.DOM
*/

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
    if (this.state.viewMode || this.props.deleting){
      return null;
    }
     return(React.DOM.div({className: "role-name-editor"}, 
              "Role name", 
              React.DOM.input({type: "text", className: "form-control", value: this.props.role.name, onChange: this.dataChangeHandler, placeholder: "Enter role name"})
             ));
  },

  renderCapabilities: function(){

      if(this.props.role.capabilityGroups == undefined || this.props.role.capabilityGroups.length == 0){
        return (React.DOM.b(null, "There are no capabilities"));
      }

     return (CapabilitiesBox({role: this.props.role, editRoleHandler: this.props.editRoleHandler, viewMode: this.state.viewMode || this.props.deleting}));  
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




