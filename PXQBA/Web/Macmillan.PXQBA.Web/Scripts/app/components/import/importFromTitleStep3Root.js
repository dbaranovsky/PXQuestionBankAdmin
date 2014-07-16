﻿/**
* @jsx React.DOM
*/

var ImportFromTitleStep3Root = React.createClass({displayName: 'ImportFromTitleStep3Root',
  
    getInitialState: function() {
      return { loading: false };
    },

    backHandler: function() {
    	var courseId = this.props.currentCourseId;
    	var url = window.actions.importActions.buildImportFromTitleStep2(courseId);
		window.location = url;
    },

    selectTitleHandler: function(titleId) {
		importDataManager.importQuestionsTo(titleId).done(this.importQuestionsToDoneHandler);
	},

	importQuestionsToDoneHandler: function(response) {
		notificationManager.showSuccess("Imported "+ response.questionImportedCount + " question(s).");
	},

    renderSelectorMenu: function() {
    	return(React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.backHandler}, 
                "Back"
               ));
    },

    render: function() {
       return (
                React.DOM.div(null, 
                      React.DOM.div(null, 
                     		TitleListSelector( {data:this.props.response.titles, 
                     						   selectTitleHandler:this.selectTitleHandler, 
                     						   caption:"Select title to import to:",
                     						   renderSelectorMenu:this.renderSelectorMenu}
                     						   ),
                     		 this.state.loading? Loader(null ) : ""
           	         )
                )
            );
    }
});

