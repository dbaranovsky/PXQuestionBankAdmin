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

  addSiteBuilderRepository: function(){
      var url = this.props.siteBuilderLink + this.state.repositoryName;
      this.props.loadingHandler(true);
      titleDataManager.addSiteBuilderRepository(url)
                      .done(this.addSiteBuilderRepositoryDoneHanlder)
                      .error(this.addSiteBuilderRepositoryErrorHanlder);
  },

  addSiteBuilderRepositoryDoneHanlder: function(response) {
     this.props.loadingHandler(false);
     if(response.isError) {
         notificationManager.showWarning("Url is invalid. Please enter a valid url.");
         return;
     }

     var message = "Title created successfully. ";
     var url = actions.metadataCfg.buildMetadataCfgIndexUrl(response.courseId);
     var link = '<a href='+url+'> Go to the Metadata config page of the new title</a>';
 
     notificationManager.showSuccessHtml(message+link);
     this.refs.modelDialog.refs.cancelButton.getDOMNode().click();
  },

  addSiteBuilderRepositoryErrorHanlder: function(response) {
    this.props.loadingHandler(false);
    notificationManager.showDanger(window.enums.messages.errorMessage);
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
                    React.DOM.button( {type:"button", ref:"cancelButton", className:"btn btn-default", 'data-dismiss':"modal"}, "Cancel"),
                    React.DOM.button( {type:"button", className:"btn btn-primary", onClick:self.addSiteBuilderRepository}, "Add")
                 ));
    };
 
    return (ModalDialog(  {ref:"modelDialog",
                          showOnCreate:  true, 
                          renderHeaderText:renderHeaderText, 
                          renderBody:renderBody,  
                          closeDialogHandler:  this.closeDialog, 
                          renderFooterButtons:renderFooterButtons, 
                          dialogId:"AddSiteBuilderDialogId"})); 
    }
});