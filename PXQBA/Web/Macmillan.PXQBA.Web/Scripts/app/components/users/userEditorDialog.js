/**
* @jsx React.DOM
*/

var EditUserDialog  = React.createClass({displayName: 'EditUserDialog',

    getInitialState: function(){
        return({roles: [], loading: true});
    },


    componentDidMount: function(){
        var self = this;
        userManager.getUserRoles(this.props.user.id).done(this.setRoles).error(function(e){
            self.setState({loading: false});
        });
    },

    setRoles: function(roles){
   
        this.setState({roles: roles, loading: false});
    },


     closeDialog: function(){
        this.props.closeEditUserDialog();
    },  

    changeTitles: function(roles){
        this.setState({roles: roles});
    },

    saveUserRoles: function(){
        this.setState({loading: true});
        var self= this;
        userManager.saveUserRoles(this.state.roles)
                    .done(function(e){
                      self.setState({loading: false});
                      $(self.getDOMNode()).modal("hide");
                    })
                    .error(function(e){
                      self.setState({loading: false});
                    });
    
       this.props.updateAvailibleTitles(this.props.user.id, $.grep(this.state.roles.productCourses, function(el){return el.currentRole != null}).length);

    },


    render: function() {
       var self = this;
        var renderHeaderText = function() {
         
             return "User Editing — "+ self.props.user.fullName;
           
        };

      
        var renderBody = function(){
            return (React.DOM.div({className: "user-titles-container"}, 
              React.DOM.div({className: "title-table"}, 
                      UserTitlesBox({user: self.props.user, titles: self.state.roles, loading: self.state.loading, changeTitles: self.changeTitles})
                    )
                    )
            );
        };

     
      var  renderFooterButtons = function(){

                   return (React.DOM.div({className: "modal-footer"}, 
                             React.DOM.button({type: "button", className: "btn btn-primary", onClick: self.saveUserRoles}, "Save"), 
                             React.DOM.button({type: "button", className: "btn btn-default", 'data-dismiss': "modal", 'data-target': "editUserModal"}, "Close")
                          ));
             
                 };

   

        return (ModalDialog({showOnCreate: true, 
                              renderHeaderText: renderHeaderText, 
                              renderBody: renderBody, 
                              closeDialogHandler: this.closeDialog, 
                              renderFooterButtons: renderFooterButtons, 
                              dialogId: "editUserModal"}));
    }
});






