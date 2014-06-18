/**
* @jsx React.DOM
*/

var UserBox = React.createClass({displayName: 'UserBox',


    renderUsers: function(){
          var self = this;
         var rows = [];
         rows = this.props.users.map(function (user, i) {
        
            return (UserRow( {user:user, showAvailibleTitlesHandler:self.props.showAvailibleTitlesHandler, showUserEditDialog:self.props.showUserEditDialog}));
          });

     return rows;
    },
    render: function() {
       return (React.DOM.div(null, 
                React.DOM.div( {className:"roles-table"},  

                  this.renderUsers()

                )
               
                )
            );
    }
});

