/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({

    getInitialState: function(){
      return ({ isShared: this.props.data.sharedWith != "",
                titleCount:  this.props.data.sharedWith.split("<br />").length});
    },

    editNotesHandler: function(){
      this.props.editNotesHandler();
    },

    copyQuestionHandler: function() {
      this.props.copyQuestionHandler();
    },

    editQuestionHandler: function() {
        if(!this.state.isShared){
          this.props.editQuestionHandler();
          return;
        }


        
    },

    componentDidUpdate: function(){

      this.initializePopovers();
    },

     componentDidMount: function(){
    
      this.initializePopovers();
    },

    initializePopovers: function(){
        if (this.state.isShared){
          $(this.getDOMNode()).popover({title: 'Shared with:',
                                        selector: '[rel="popover"]',
                                        trigger: 'click', 
                                        placement:'bottom', 
                                       
                                        html: true});  
          return;
        } 
            $(this.getDOMNode()).popover({
                                        selector: '[rel="popover"]',
                                        trigger: 'click', 
                                        placement:'bottom', 
                                        content: '<b>Not Shared</b>',
                                        html: true
                                        });  
        
        
      
    },

    renderCourseCountBadge: function(){
      if (!this.state.isShared){
        return "";
      }
      return(<span className="badge">{this.state.titleCount}</span>);
    },

    renderSharedButtons: function(){
      if(this.props.showAll){


    return ( <div className="shared-placeholder"> 
              <button type="button" className="btn btn-default btn-sm custom-btn shared-to" rel="popover"  data-content={this.props.data["sharedWith"]}>
                 <span className="glyphicon icon-shared-to" ></span>{this.renderCourseCountBadge()} 
               </button>
               <button type="button" className="btn btn-default btn-sm tiny" onClick={this.editNotesHandler} data-toggle="tooltip" title="Share this question"><span className="glyphicon glyphicon-plus-sign"></span></button> 
                    {this.state.isShared?
                      <button type="button" className="btn btn-default btn-sm tiny" onClick={this.editNotesHandler} data-toggle="tooltip" title="Remove title"><span className="glyphicon glyphicon-minus-sign"></span></button> :
                    ""}
               </div>);
     }

      if(this.state.isShared){
      return(
         <div className="shared-placeholder">
                           
                         
           <button type="button" className="btn btn-default btn-sm custom-btn shared-to"  data-content={this.props.data["sharedWith"]}>
                    <span className="glyphicon icon-shared-to" ></span>{this.renderCourseCountBadge()}
           </button> 
           <button type="button" className="btn btn-default btn-sm tiny" onClick={this.editNotesHandler} data-toggle="tooltip" title="Add title"><span className="glyphicon glyphicon-plus-sign"></span></button> 
            { this.state.isShared?
              <button type="button" className="btn btn-default btn-sm tiny" onClick={this.editNotesHandler} data-toggle="tooltip" title="Add title"><span className="glyphicon glyphicon-minus-sign"></span></button> :
               ""}
                             
          </div>);
    } 

     

    return ( <div className="shared-placeholder"> </div>);

    },

    renderEditMenu: function(){
                  if (!this.state.isShared){
                    return null;
                  }
                  return(
                     <ul className="dropdown-menu show-menu" role="menu" aria-labelledby="dropdownMenuType" onClick={this.changeEventHandler} aria-labelledby="edit-question">
                        <li role="presentation" className="dropdown-header">Edit options</li>
                       <li role="presentation" className="divider"></li>
                       <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.props.editQuestionHandler}>Edit in {this.state.titleCount == 1? "1 title" : "all "+this.state.titleCount+" titles"}</a></li>
                       <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.copyQuestionHandler}>Create a copy</a></li>
                     </ul>);
    },


    render: function() {
      if (this.props.showAll){

        return ( 
                <div>
                  
                   {this.renderSharedButtons()}
                   <div className="menu-container">
                     <button type="button" className="btn btn-default btn-sm" onClick={this.editNotesHandler} data-toggle="tooltip" title="Edit Notes"><span className="glyphicon glyphicon-list-alt"></span>
                     </button> 
                     <div className="dropdown">
                     <button id="edit-question" type="button" className="btn btn-default btn-sm" onClick={this.editQuestionHandler}  data-target="#" data-toggle="dropdown" title="Edit Question">
                           <span className="glyphicon glyphicon-pencil" data-toggle="tooltip" title="Edit Question"></span>
                     </button>
                      {this.renderEditMenu()}
                     </div>


                     <button type="button" className="btn btn-default btn-sm" onClick={this.copyQuestionHandler}  data-toggle="tooltip" title="Duplicate Question"><span className="glyphicon glyphicon-copyright-mark"></span></button>
                  </div>

                           
                </div>
            );
      }

       return (<div> {this.renderSharedButtons()}</div>);
  }
});