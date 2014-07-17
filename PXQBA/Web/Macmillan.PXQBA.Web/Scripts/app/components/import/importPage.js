/**
* @jsx React.DOM
*/

var ImportPage = React.createClass({displayName: 'ImportPage',

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
     importDataManager.importFile(window.importParameters.currentFileId, titleId).done(function(importResult){
        if(importResult.notAllowed){
          notificationManager.showDanger("You have no permission to import file of such type to this title");
          this.setState({loading: false});
        }
        self.setState({importResult: importResult, isImported: true});
       }).error(this.handlerErros);
    },

    cancelHandler: function(){
      
    },


    render: function() {

      var self = this;
     
      if(this.state.importResult != null && this.state.importResult.fileNotFound){
         return (React.DOM.div( {className:"imported-note"}, 
                    "There is no file to import from."
                ))
      }


      if(this.state.importResult != null && this.state.importResult.alreadyImported){
         return (React.DOM.div( {className:"imported-note"}, 
                    "This file has been already imported."
                ))
      }

      if(this.state.isImported){
        return ( ImportCompleteBox( {questionImported:this.state.importResult.questionCount, titleId:this.state.importResult.titleId} ));
      }

       return (
                React.DOM.div(null, 
                 React.DOM.h2(null,  " Titles available:"),        

                     TitleListSelector( {data:this.state.response.titles, 
                                        selectTitleHandler:this.selectTitleHandler, 
                                        caption:"Select title to import to:"}
                                        ),
                     this.state.loading? Loader(null ) : ""
        
                )
            );
    }
});
