/**
* @jsx React.DOM
*/

 var AvailibleTitlesDialog  = React.createClass({

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
          return (<div className="waiting middle"/>);
      }

     var rows = [];
     rows = this.state.titles.map(function (title, i) {
        
            return ( <div className="title-row">
                        <div className="title-cell">{title.titleName}</div>
                        <div className="title-cell"><i>{title.currentRole.name}</i></div>
                      </div>);
          });

     if (rows.length == 0){
       return (<b>No titles are availible</b>);
     }

     return rows;

    },

    render: function() {
       var self = this;

        var renderHeaderText = function() {
         
             return "Titles availible for "+ self.props.user.userName;
           
        };

      
        var renderBody = function(){
            return (<div className="title-table"> 
                     {self.renderRows()}
                    </div>
            );
        };

     
      var  renderFooterButtons = function(){

                   return (<div className="modal-footer"> 
                             <button type="button" className="btn btn-default" data-dismiss="modal" data-target="roleModal">Close</button>
                          </div>);
             
                 };

   

        return (<ModalDialog  showOnCreate = {true} 
                              renderHeaderText={renderHeaderText} 
                              renderBody={renderBody}  
                              closeDialogHandler = {this.closeDialog} 
                              renderFooterButtons={renderFooterButtons} 
                              dialogId="titlesModal"/>);
    }
});




