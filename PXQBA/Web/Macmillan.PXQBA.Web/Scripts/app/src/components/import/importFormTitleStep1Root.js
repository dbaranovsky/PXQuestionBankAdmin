/**
* @jsx React.DOM
*/

var ImportFromTitleStep1Root = React.createClass({

    getInitialState: function() {
      return { loading: false };
    },


	selectTitleHandler: function(titleId) {
		alert('selected '+titleId);
	},

    render: function() {
       return (
                <div>
                      <div>
                 		  <h2> Titles available:</h2>        

                     		<TitleListSelector data={this.props.response.titles} 
                     						   selectTitleHandler={this.selectTitleHandler} 
                     						   caption="Select title to import to:"/>
                     		 {this.state.loading? <Loader /> : ""}
           	         </div>
                </div>
            );
    }
});

