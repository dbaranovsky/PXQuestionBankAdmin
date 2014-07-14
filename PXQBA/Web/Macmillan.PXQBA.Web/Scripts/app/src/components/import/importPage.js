/**
* @jsx React.DOM
*/

var ImportPage = React.createClass({

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
                <div>
                 <h2> Titles available:</h2>        

                     <TitleListSelector data={this.state.response.titles} selectTitleHandler={this.selectTitleHandler} />
                     {this.state.loading? <Loader /> : ""}
        
                </div>
            );
    }
});
