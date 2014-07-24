/**
* @jsx React.DOM
*/

var RoleRow = React.createClass({

  viewCapabilities: function(){
     this.props.viewCapabilities(this.props.role);
  },

  editRole: function(){
     this.props.editRole(this.props.role);
  },

  removeRole: function(){
     this.props.removeRole(this.props.role);
  },

  renderMenu: function(){
    if(this.props.role.canEdit){
      return(   <div className="menu-container-main version-history">
                          <button type="button" className="btn btn-default btn-sm"  data-toggle="tooltip"  title="Edit Role" onClick={this.editRole}><span className="icon-pencil-1"></span> </button>
                          <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip" title="Remove Role" onClick={this.removeRole}><span className="glyphicon glyphicon-trash"></span></button>
                         
                       </div>
      );
    }

    return(<div className="menu-container-main version-history" />);
  },
  render: function() {
       return (
                <div className="role-row">

                      <div className="role-cell role-name"> {this.props.role.name} </div>
                      <div className="role-cell capabilities">
                          <span className="capabilities-link" onClick={this.viewCapabilities}>{this.props.role.activeCapabiltiesCount} capabilit{this.props.role.activeCapabiltiesCount == 1? "y": "ies"} </span>
                      </div>
                      <div className="role-cell menu">
                        {this.renderMenu()}
                       
                       </div>
                        
                </div>
            );
    }

 });