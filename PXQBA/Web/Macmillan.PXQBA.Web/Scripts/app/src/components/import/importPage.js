/**
* @jsx React.DOM
*/

var ImportPage = React.createClass({

    getInitialState: function() {
      return { loading: false, response: this.props.response, isImported: false, importResult: null};
    },

  

    handlerErros: function(e){
         notificationManager.showDanger("Error occured, please, try again later");
          this.setState({loading: false});
    },
  

    selectTitleHandler: function(titleId){
      var self = this;
      this.setState({loading: true});
      importDataManager.importFile(window.importParameters.currentFileId, titleId).done(
          function(importResult){
              if(importResult.notAllowed) {
                notificationManager.showDanger("You have no permission to import file of such type to this title");
                self.setState({loading: false});
                return;
              }
              self.setState({importResult: importResult, isImported: true});
              }).error(this.handlerErros);
    },

    cancelHandler: function(){
      
    },

    captionRender: function(){
      var message = window.importParameters.questionToImport == "1"? "1 question is" : window.importParameters.questionToImport+ " questions are";
      message+= " parsed and ready to import. Select title to import to:";
      return message;
    },


    render: function() {

      var self = this;
     
      if(this.state.importResult != null && this.state.importResult.fileNotFound){
         return (<div className="imported-note">
                    There is no file to import from.
                </div>)
      }


      if(this.state.importResult != null && this.state.importResult.alreadyImported){
         return (<div className="imported-note">
                    This file has been already imported.
                </div>)
      }

      if(this.state.isImported){
        return ( <ImportCompleteBox questionImported={this.state.importResult.questionCount} titleId={this.state.importResult.titleId} />);
      }

       return (
                <div>
                 <h2> Titles available:</h2>        

                     <TitleListSelector data={this.state.response.titles} 
                                        selectTitleHandler={this.selectTitleHandler} 
                                        caption={this.captionRender()}
                                        />
                     {this.state.loading? <Loader /> : ""}
        
                </div>
            );
    }
});
