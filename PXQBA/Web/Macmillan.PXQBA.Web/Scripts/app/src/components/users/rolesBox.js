/**
* @jsx React.DOM
*/

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




