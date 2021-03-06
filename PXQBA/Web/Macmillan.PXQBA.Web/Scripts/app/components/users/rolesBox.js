﻿/**
* @jsx React.DOM
*/

var RolesBox = React.createClass({displayName: 'RolesBox',

  renderRoles: function(){
     var self= this;

     var rows = [];
     rows = this.props.roles.roles.map(function (role, i) {
        
            return (RoleRow({role: role, editRole: self.props.editRole, removeRole: self.props.removeRoleHandler, viewCapabilities: self.props.viewCapabilities}));
          });

     return rows;

  },

  addRoleClickHandler: function(){
      this.props.addRoleClickHandler();
  },

  render: function() {
       return (
                React.DOM.div({className: "roles-box"}, 
                        React.DOM.button({className: "btn btn-primary ", 'data-toggle': "modal", disabled: !this.props.roles.canDefineNewRole, title: "Add role", onClick: this.addRoleClickHandler}, 
                             "Add role"
                        ), 
                        React.DOM.br(null), React.DOM.br(null), 
                        React.DOM.div({className: "roles-table"}, 
                          this.renderRoles()
                        )
                        
                )
            );
    }

  });




