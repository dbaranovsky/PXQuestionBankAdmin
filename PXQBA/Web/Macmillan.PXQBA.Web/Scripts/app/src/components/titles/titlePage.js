/**
* @jsx React.DOM
*/

var TitlePage = React.createClass({

    getInitialState: function() {
      return { loading: false, response: this.props.response, showAddRepoDialog: false};
    },

    renderAddRepoDialog: function(){
      if(this.state.showAddRepoDialog){
        return (<AddRepositoryDialog  titles ={this.props.response.titles} closeDialogHandler={this.closeDialogHandler} addNewRepository={this.addNewRepository}/>)
      }

      return null;

    },

    showAddRepoDialog: function(){
      this.setState({showAddRepoDialog: true});
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
               <h2> Titles available:</h2>        

                     <TitleList data={this.state.response.titles} />
                     {this.state.loading? <Loader /> : ""}
                     {this.renderAddRepoDialog()}
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
    if(this.state.newRepoName==""){
      notificationManager.showWarning("Please, enter repository name");
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







