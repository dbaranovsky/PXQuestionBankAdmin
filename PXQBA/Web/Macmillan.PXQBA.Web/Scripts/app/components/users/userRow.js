/**
* @jsx React.DOM
*/
var UserRow = React.createClass({displayName: 'UserRow',

    showAvailibleTitlesHandler: function(){
        this.props.showAvailibleTitlesHandler(this.props.user);
    },

    editUser: function(){
        this.props.showUserEditDialog(this.props.user);
    },

    render: function() {
       return (
                React.DOM.div( {className:"role-row"},  

                 React.DOM.div( {className:"role-cell role-name"}, this.props.user.userName),
                 React.DOM.div( {className:"role-cell capabilities " },  " ", React.DOM.span( {className:"capabilities-link", onClick:this.showAvailibleTitlesHandler}, this.props.user.availibleTitlesCount, " ", this.props.user.availibleTitlesCount == 1? "title" : "titles")),
                 React.DOM.div( {className:"role-cell menu"}, 
                    React.DOM.button( {type:"button", className:"btn btn-default btn-sm",  'data-toggle':"tooltip",  title:"Edit Role", onClick:this.editUser}, React.DOM.span( {className:"glyphicon glyphicon-pencil"}), " " )
                 )
                )
            );
     }

});



