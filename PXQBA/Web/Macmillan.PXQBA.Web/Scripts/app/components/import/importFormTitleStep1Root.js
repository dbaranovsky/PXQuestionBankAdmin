/**
* @jsx React.DOM
*/

var ImportFromTitleStep1Root = React.createClass({displayName: 'ImportFromTitleStep1Root',

    getInitialState: function() {
      return { loading: false };
    },


	selectTitleHandler: function(titleId) {
		var url = window.actions.importActions.buildImportFromTitleStep2(titleId);
		window.location = url;
	},

    render: function() {
       return (
                React.DOM.div(null, 
                      React.DOM.div(null, 
                     		TitleListSelector( {data:this.props.response.titles, 
                     						   selectTitleHandler:this.selectTitleHandler, 
                     						   caption:"Select title to import from:"}),
                     		 this.state.loading? Loader(null ) : ""
           	         )
                )
            );
    }
});

