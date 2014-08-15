/**
* @jsx React.DOM
*/

var ImportFromTitleStep3Root = React.createClass({displayName: 'ImportFromTitleStep3Root',
  
    getInitialState: function() {
      return { 
      		loading: false,
      		imported: false,
      		questionImported:0,
      		titleIdImportedTo: null
       };
    },

    backHandler: function() {
    	var courseId = this.props.currentCourseId;
    	var url = window.actions.importActions.buildImportFromTitleStep2(courseId);
		window.location = url;
    },

    selectTitleHandler: function(titleId) {
    	this.setState({
    			loading: true,
    			titleIdImportedTo: titleId
    		});
		importDataManager.importQuestionsTo(titleId, this.props.key).done(this.importQuestionsToDoneHandler);
	},

	importQuestionsToDoneHandler: function(response) {
		this.setState({ loading: false});
		if(response.isError) {
			notificationManager.showDanger(response.errorMessage);
			return;
		}

		this.setState({
			   questionImported: response.questionImportedCount,
			   imported: true
			});
	},

    renderSelectorMenu: function() {
    	return(React.DOM.button({type: "button", className: "btn btn-default btn-sm", onClick: this.backHandler}, 
                "Back"
               ));
    },

    renderEndPage: function() {
    	          return ( ImportCompleteBox({questionImported: this.state.questionImported, titleId: this.state.titleIdImportedTo}));
    },

    getTetles: function() {
    	var self = this;
    	return this.props.response.titles.filter(function(title){ return title.id!=self.props.currentCourseId});;
    },

    renderTitleList: function() {
    	return (TitleListSelector({data: this.getTetles(), 
                     			   selectTitleHandler: this.selectTitleHandler, 
                     			   caption: "Select title to import to:", 
                     			   renderSelectorMenu: this.renderSelectorMenu}
                ));
    },

    render: function() {

       var content = null;
   
       if(!this.state.imported) {
       	  content=this.renderTitleList();
       }
       else {
       	  content=this.renderEndPage();
       }

       return (
                React.DOM.div(null, 
                      React.DOM.div(null, 
                     		 content, 
                     		 this.state.loading? Loader(null) : ""
           	         )
                )
            );
    }
});

