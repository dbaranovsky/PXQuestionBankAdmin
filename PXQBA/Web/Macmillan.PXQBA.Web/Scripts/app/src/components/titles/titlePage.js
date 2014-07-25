/**
* @jsx React.DOM
*/

var TitlePage = React.createClass({

    getInitialState: function() {
      return { 
        loading: false,
        response: this.props.response, 
        showAddRepoDialog: false,
        showAddSiteBuilderDialog: false,
      };
    },

    renderAddDialogs: function(){
      if(this.state.showAddRepoDialog){
        return (<AddRepositoryDialog  titles ={this.props.response.titles} closeDialogHandler={this.closeDialogHandler} addNewRepository={this.addNewRepository}/>)
      }

      if(this.state.showAddSiteBuilderDialog) {
         return (<AddSiteBuilderDialog  siteBuilderLink={this.props.siteBuilderLink} closeDialogHandler={this.closeAddSiteBuilderRepoDialog} addNewRepository={this.addNewRepository}/>);
      }

      return null;

    },

    showAddRepoDialog: function(){
      this.setState({showAddRepoDialog: true});
    },

    showAddSiteBuilderRepoDialog: function() {
       this.setState({showAddSiteBuilderDialog: true});
    },

    closeAddSiteBuilderRepoDialog: function() {
       this.setState({showAddSiteBuilderDialog: false});
    },

    closeDialogHandler: function(){
      this.setState({showAddRepoDialog: false});
    },


    addNewRepository: function(name){
      this.setState({loading:true, showAddRepoDialog: false});
      var self=this;
      titleDataManager.addNewRepository(name).done(function(){
        notificationManager.showSuccess("Draft repository successfully created");
        titleDataManager.getTitles().done(self.setTitles).error(self.handlerErrors);
      }).error(self.handlerErrors);
    },

    handlerErros: function(e){
         notificationManager.showSuccess("Error occured, please, try again later");
          this.setState({loading: false});
    },
  
  setTitles: function(response){
    this.setState({response: response, loading: false, showAddRepoDialog: false})
  } ,

    render: function() {
       return (
                <div>
                        <button className="btn btn-primary add-repository" onClick={this.showAddRepoDialog}>
                           Add repository
                        </button>
                        <button className="btn btn-primary add-repository" onClick={this.showAddSiteBuilderRepoDialog}>
                           Add SiteBuilder repository
                        </button>
               <h2> Titles available:</h2>        

                     <TitleList data={this.state.response.titles} />
                     {this.state.loading? <Loader /> : ""}
                     {this.renderAddDialogs()}
                </div>
            );
    }
});

var AddRepositoryDialog  = React.createClass({

   getInitialState: function(){
       return({titles: this.props.titles, newRepoName: ""});
   },


  dataChangeHandler: function(name){
      this.setState({newRepoName: name})
  },

  addRepository: function(){
    var newRepoName = this.state.newRepoName;
    if(newRepoName==""){
      notificationManager.showWarning("Please, enter repository name");
      return;
    }
   
    var existingNames = [];
    existingNames = $.grep(this.props.titles, function(el){
      return el.title == newRepoName;
    });

    if(existingNames.length > 0){
      notificationManager.showWarning("This name already exist");
      return;
    }
    this.props.addNewRepository(this.state.newRepoName);
    $(this.getDOMNode()).modal("hide");
  },

   closeDialog: function(){

        this.props.closeDialogHandler();
   },  

 render: function() {
        
           var self = this;
        var renderHeaderText = function() {
            
            return "Add repository";
        };

      
        var renderBody = function(){
      

            return (<div>
                      <p>Enter repository name:</p>
                     <TextEditor dataChangeHandler={self.dataChangeHandler} value={self.state.newRepoName}/>
                    </div>
            );
        };




          var  renderFooterButtons = function(){

                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-primary" onClick={self.addRepository}>Save</button>
                             <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                          </div>);
             
                 };
 

        return (<ModalDialog  showOnCreate = {true} 
                              renderHeaderText={renderHeaderText} 
                              renderBody={renderBody}  
                              closeDialogHandler = {this.closeDialog} 
                              renderFooterButtons={renderFooterButtons} 
                              dialogId="addRepoDialogId"/>);
    }
});







