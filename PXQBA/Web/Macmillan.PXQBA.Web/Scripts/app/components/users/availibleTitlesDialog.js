/**
* @jsx React.DOM
*/

 var AvailibleTitlesDialog  = React.createClass({displayName: 'AvailibleTitlesDialog',

    getInitialState: function(){
        return({titles: [], loading: true});
    },
    closeDialog: function(){
        this.props.closeAvailibleTitles();
    },  

    componentDidMount: function(){
        var self = this;
        userManager.getAvailibleTitles(this.props.user.id).done(this.setTitles).error(function(e){
            self.setState({loading: false});
        });
    },

    setTitles: function(titles){
        this.setState({titles: titles, loading: false});
    },

    renderRows: function(){
     var self= this;

      if(this.state.loading){
          return (React.DOM.div( {className:"waiting middle"}));
      }

     var rows = [];
     rows = this.state.titles.productCourses.map(function (title, i) {
        
            return ( React.DOM.div( {className:"title-row"}, 
                        React.DOM.div( {className:"title-cell"}, title.name),
                        React.DOM.div( {className:"title-cell"}, React.DOM.i(null, title.currentRole.name))
                      ));
          });

     if (rows.length == 0){
       return (React.DOM.b(null, "No titles are availible"));
     }

     return rows;

    },

    render: function() {
       var self = this;

        var renderHeaderText = function() {
         
             return "Titles availible for "+ self.props.user.fullName;
           
        };

      
        var renderBody = function(){
            return (React.DOM.div( {className:"title-table"},  
                     self.renderRows()
                    )
            );
        };

     
      var  renderFooterButtons = function(){

                   return (React.DOM.div( {className:"modal-footer"},  
                             React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", 'data-target':"roleModal"}, "Close")
                          ));
             
                 };

   

        return (ModalDialog(  {showOnCreate:  true, 
                              renderHeaderText:renderHeaderText, 
                              renderBody:renderBody,  
                              closeDialogHandler:  this.closeDialog, 
                              renderFooterButtons:renderFooterButtons, 
                              dialogId:"titlesModal"}));
    }
});




