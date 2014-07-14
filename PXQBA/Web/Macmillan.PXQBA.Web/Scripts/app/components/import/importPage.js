/**
* @jsx React.DOM
*/

var ImportPage = React.createClass({displayName: 'ImportPage',

    getInitialState: function() {
      return { loading: false, response: this.props.response};
    },

  

    handlerErros: function(e){
         notificationManager.showSuccess("Error occured, please, try again later");
          this.setState({loading: false});
    },
  

    selectTitleHandler: function(titleId){
     
    },

    cancelHandler: function(){
      
    },

    render: function() {

      var self = this;
     

       return (
                React.DOM.div(null, 
                 React.DOM.h2(null,  " Titles available:"),        

                     TitleListSelector( {data:this.state.response.titles, selectTitleHandler:this.selectTitleHandler} ),
                     this.state.loading? Loader(null ) : ""
        
                )
            );
    }
});
