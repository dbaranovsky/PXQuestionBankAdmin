/**
* @jsx React.DOM
*/

var EditUserDialog  = React.createClass({

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
        userManager.saveUserRoles(this.props.user.id, $.grep(this.state.roles, function(el){return el.isChanged}))
                    .done(function(e){
                      self.setState({loading: false});
                      $(self.getDOMNode()).modal("hide");
                    })
                    .error(function(e){
                      self.setState({loading: false});
                    });
    
       this.props.updateAvailibleTitles(this.props.user.id, $.grep(this.state.roles, function(el){return el.currentRole != null}).length);

    },


    render: function() {
       var self = this;
        var renderHeaderText = function() {
         
             return "User Editing — "+ self.props.user.userName;
           
        };

      
        var renderBody = function(){
            return (<div className="user-titles-container">
              <div className="title-table"> 
                      <UserTitlesBox user={self.props.user} titles={self.state.roles} loading={self.state.loading} changeTitles={self.changeTitles} />
                    </div>
                    </div>
            );
        };

     
      var  renderFooterButtons = function(){

                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-primary" onClick={self.saveUserRoles}>Save</button>
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="editUserModal">Close</button>
                          </div>);
             
                 };

   

        return (<ModalDialog  showOnCreate = {true} 
                              renderHeaderText={renderHeaderText} 
                              renderBody={renderBody}  
                              closeDialogHandler = {this.closeDialog} 
                              renderFooterButtons={renderFooterButtons} 
                              dialogId="editUserModal"/>);
    }
});






