/**
* @jsx React.DOM
*/

var RoleRow = React.createClass({displayName: 'RoleRow',

  viewCapabilities: function(){
     this.props.viewCapabilities(this.props.role);
  },

  editRole: function(){
     this.props.editRole(this.props.role);
  },

  render: function() {
       return (
                React.DOM.div( {className:"role-row"}, 

                      React.DOM.div( {className:"role-cell role-name"},  " ", this.props.role.name, " " ),
                      React.DOM.div( {className:"role-cell capabilities"}, 
                          React.DOM.span( {className:"capabilities-link", onClick:this.viewCapabilities}, this.props.role.activeCapabiltiesCount, " capabilit",this.props.role.activeCapabiltiesCount == 1? "y": "ies", " " )
                      ),
                      React.DOM.div( {className:"role-cell menu"}, 

                          React.DOM.div( {className:"menu-container-main version-history"}, 
                          React.DOM.button( {type:"button", className:"btn btn-default btn-sm",  'data-toggle':"tooltip",  title:"Edit Role", onClick:this.editRole}, React.DOM.span( {className:"glyphicon glyphicon-pencil"}), " " ),
                          React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip", title:"Remove Role"}, React.DOM.span( {className:"glyphicon glyphicon-trash"}))
                         
                       )

                       )
                        
                )
            );
    }

 });