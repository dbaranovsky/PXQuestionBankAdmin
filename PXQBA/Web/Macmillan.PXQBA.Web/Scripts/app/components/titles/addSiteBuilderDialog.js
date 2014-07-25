/**
* @jsx React.DOM
*/


var AddSiteBuilderDialog  = React.createClass({displayName: 'AddSiteBuilderDialog',

   getInitialState: function(){
       return({repositoryName: null});
   },


  dataChangeHandler: function(name){
      this.setState({repositoryName: name})
  },

  addRepository: function(){
    
  },

   closeDialog: function(){
        this.props.closeDialogHandler();
   },  

   render: function() {
        
    var self = this;
    var renderHeaderText = function() {
        return "New title";
    };

    var renderBody = function(){
        return (React.DOM.div(null, 
                  React.DOM.table(null, 
                      React.DOM.tr(null, 
                          React.DOM.td(null, 
                              self.props.siteBuilderLink 
                          ),
                          React.DOM.td(null, 
                              TextEditor( {dataChangeHandler:self.dataChangeHandler, value:self.state.repositoryName})
                          )
                      )
                  )
                )
                );
    };

    var  renderFooterButtons = function(){
         return (React.DOM.div( {className:"modal-footer"},  
                    React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Cancel"),
                    React.DOM.button( {type:"button", className:"btn btn-primary", onClick:self.addRepository}, "Add")
                 ));
    };
 
    return (ModalDialog(  {showOnCreate:  true, 
                          renderHeaderText:renderHeaderText, 
                          renderBody:renderBody,  
                          closeDialogHandler:  this.closeDialog, 
                          renderFooterButtons:renderFooterButtons, 
                          dialogId:"AddSiteBuilderDialogId"})); 
    }
});