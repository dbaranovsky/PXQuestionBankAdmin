/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({


     getInitialState: function() {

       return { isFlagged: this.props.data.flag == window.enums.flag.flagged };
    },
    editNotesHandler: function(){
      this.props.editNotesHandler();
    },

    copyQuestionHandler: function() {
      this.props.copyQuestionHandler();
    },

    editQuestionHandler: function() {
        if(!this.props.isShared){
          this.props.editQuestionHandler();
          return;
        }
    },

    removeTitleHandler: function(){
      if(confirm("Are you sure you want to remove this question from the current title?")){
         questionDataManager.removeTitle(this.props.data.id);
      }
    },

    shareHandler: function(){
        this.props.shareHandler();
    },

    toggleFlag: function(){

      questionDataManager.flagQuestion(this.props.data.id, !this.state.isFlagged);
      this.setState( {isFlagged: !this.state.isFlagged});

    },



    componentDidUpdate: function(){
      
      this.initializePopovers();
    },

    componentDidMount: function(){
    },

   
    initializePopovers: function(){
      
       $(this.getDOMNode()).find('[rel="popover"]').popover('destroy'); 
       $(this.getDOMNode()).popover({
                                        selector: '[rel="popover"]',
                                        trigger: 'click', 
                                        placement:'bottom',           
                                        html: true,
                                        container: 'body'
                                        });  
       if(!this.props.showAll){
         $('.popover').remove();
       }


               
    },

    renderCourseCountBadge: function(){
      if (!this.props.isShared){
        return "";
      }
      return(<span className="badge">{this.props.titleCount}</span>);
    },

    renderSharedMenu: function(){
      if(this.props.showAll){


    return ( <div className="shared-placeholder" > 
              <button type="button" className="btn btn-default btn-sm custom-btn shared-to" rel="popover" onClick={this.showPopover}  data-toggle="popover"  data-title={this.props.isShared? "Shared with:" : ""}  data-content={this.props.isShared? this.props.data[window.consts.questionSharedWithName] : "<b>Not Shared</b>"} >
                 <span className="glyphicon icon-shared-to" ></span>{this.renderCourseCountBadge()} 
               </button>
               <button type="button" className="btn btn-default btn-sm tiny" onClick={this.shareHandler} data-toggle="tooltip" title="Share this question"><span className="glyphicon glyphicon-plus-sign"></span></button> 
                    {this.props.isShared?
                      <button type="button" className="btn btn-default btn-sm tiny" onClick={this.removeTitleHandler} data-toggle="tooltip" title="Remove from title"><span className="glyphicon glyphicon-minus-sign"></span></button> :
                    ""}
               </div>);
     }

      if(this.props.isShared){
      return(
         <div className="shared-placeholder">
                    
                         
           <button type="button" className="btn btn-default btn-sm custom-btn shared-to"  rel="popover" >
                    <span className="glyphicon icon-shared-to" ></span>{this.renderCourseCountBadge()}
           </button> 
         
                             
          </div>);
    } 

     

    return ( <div className="shared-placeholder"> </div>);

    },

    renderEditMenu: function(){
                  if (!this.props.isShared){
                    return null;
                  }
                  return(
                     <ul className="dropdown-menu show-menu" role="menu" aria-labelledby="dropdownMenuType" onClick={this.changeEventHandler} aria-labelledby="edit-question">
                        <li role="presentation" className="dropdown-header">Edit options</li>
                       <li role="presentation" className="divider"></li>
                       <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.props.editQuestionHandler}>Edit in {this.props.titleCount == 1? "1 title" : "all "+this.props.titleCount+" titles"}</a></li>
                       <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.copyQuestionHandler}>Create a copy</a></li>
                     </ul>);
    },

    renderMenu: function(){
      if (this.props.showAll){
      return(<div className="menu-container-main">

               <div className="dropdown">
                  <button id="edit-question" type="button" className="btn btn-default btn-sm" onClick={this.editQuestionHandler}  data-target="#" data-toggle="dropdown" title="Edit Question">
                         <span className="glyphicon glyphicon-pencil" data-toggle="tooltip" title="Edit Question"></span>
                  </button>
                    {this.renderEditMenu()}
                </div>
                <button type="button" className="btn btn-default btn-sm" onClick={this.copyQuestionHandler}  data-toggle="tooltip" title="Duplicate Question"><span className="glyphicon glyphicon-copyright-mark"></span></button>
               <button type="button" className="btn btn-default btn-sm" onClick={this.editNotesHandler} data-toggle="tooltip" title="Edit Notes"><span className="glyphicon glyphicon-list-alt"></span> </button> 
               </div>);
     }

      return (<div className="menu-container-main"></div>);

    },

    renderFlagMenu: function(){
        if (this.props.showAll){
          return(<div className="menu-container-flag">
                     <button type="button" className="btn btn-default btn-sm" onClick={this.toggleFlag} data-toggle="tooltip" title={this.state.isFlagged? "Unflag question" : "Flag question"}>
                     <span className={this.state.isFlagged? "glyphicon glyphicon-flag flagged" : "glyphicon glyphicon-flag"}></span>
                     </button> 
                  </div>);
      }

      return (<div className="menu-container-flag">
                <div className="notification-icons-container">
                    {this.state.isFlagged ? <span className="glyphicon glyphicon-flag flagged"></span> : ""}
                </div>
              </div>);
    },



    render: function() {

        return ( 
                <div onmouseover={this.hidePopover}>
                   {this.renderMenu()}  
                   {this.renderFlagMenu()}  
                   {this.renderSharedMenu()}
                 </div>
            );
    


  }
});