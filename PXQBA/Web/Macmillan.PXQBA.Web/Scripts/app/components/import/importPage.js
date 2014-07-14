/**
* @jsx React.DOM
*/

var ImportPage = React.createClass({displayName: 'ImportPage',

    getInitialState: function() {
      return { loading: false, response: this.props.response, isImported: false, importResult: null};
    },

  

    handlerErros: function(e){
         notificationManager.showSuccess("Error occured, please, try again later");
          this.setState({loading: false});
    },
  

    selectTitleHandler: function(titleId){
      var self = this;
      this.setState({loading: true});
     importDataManager.importFile(window.importParameters.currentFileId, titleId).done(function(importResult){
        self.setState({importResult: importResult, isImported: true});
       }).error(this.handlerErros);
    },

    cancelHandler: function(){
      
    },

     getUrlToList: function() {
      return window.actions.questionList.buildQuestionListIndexUrl(this.state.importResult.titleId, null);
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
          return (React.DOM.div( {className:"imported-note"}, 
                   this.state.importResult.questionCount==1? "1 question was" : this.state.importResult.questionCount+" questions were", " imported successfully.",
                    this.state.importResult.questionCount==1? "This question" :  "These questions ", " may require metadata editing.",
                   React.DOM.a( {href:this.getUrlToList()}, "Go to target title >")

                ));
      }

       return (
                React.DOM.div(null, 
                 React.DOM.h2(null,  " Titles available:"),        

                     TitleListSelector( {data:this.state.response.titles, selectTitleHandler:this.selectTitleHandler} ),
                     this.state.loading? Loader(null ) : ""
        
                )
            );
    }
});
