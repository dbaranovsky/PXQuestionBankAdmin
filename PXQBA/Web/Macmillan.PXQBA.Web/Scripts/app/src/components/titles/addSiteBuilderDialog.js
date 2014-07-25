/**
* @jsx React.DOM
*/


var AddSiteBuilderDialog  = React.createClass({

   getInitialState: function(){
       return({repositoryName: null});
   },


  dataChangeHandler: function(name){
      this.setState({repositoryName: name})
  },

  addRepository: function(){
    
  },

   closeDialog: function(){
        this.props.closeDialogHandler();
   },  

   render: function() {
        
    var self = this;
    var renderHeaderText = function() {
        return "New title";
    };

    var renderBody = function(){
        return (<div>
                  <table>
                      <tr>
                          <td>
                              {self.props.siteBuilderLink} 
                          </td>
                          <td>
                              <TextEditor dataChangeHandler={self.dataChangeHandler} value={self.state.repositoryName}/>
                          </td>
                      </tr>
                  </table>
                </div>
                );
    };

    var  renderFooterButtons = function(){
         return (<div className="modal-footer"> 
                    <button type="button" className="btn btn-default" data-dismiss="modal">Cancel</button>
                    <button type="button" className="btn btn-primary" onClick={self.addRepository}>Add</button>
                 </div>);
    };
 
    return (<ModalDialog  showOnCreate = {true} 
                          renderHeaderText={renderHeaderText} 
                          renderBody={renderBody}  
                          closeDialogHandler = {this.closeDialog} 
                          renderFooterButtons={renderFooterButtons} 
                          dialogId="AddSiteBuilderDialogId"/>); 
    }
});