/**
* @jsx React.DOM
*/

var ImportFromTitleStep1Root = React.createClass({displayName: 'ImportFromTitleStep1Root',

    getInitialState: function() {
      return { loading: false };
    },


	selectTitleHandler: function(titleId) {
		alert('selected '+titleId);
	},

    render: function() {
       return (
                React.DOM.div(null, 
                      React.DOM.div(null, 
                 		  React.DOM.h2(null,  " Titles available:"),        

                     		TitleListSelector( {data:this.props.response.titles, 
                     						   selectTitleHandler:this.selectTitleHandler, 
                     						   caption:"Select title to import to:"}),
                     		 this.state.loading? Loader(null ) : ""
           	         )
                )
            );
    }
});

