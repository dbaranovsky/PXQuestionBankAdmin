/**
* @jsx React.DOM
*/

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




