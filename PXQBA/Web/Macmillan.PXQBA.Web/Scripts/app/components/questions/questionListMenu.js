/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({displayName: 'QuestionListMenu',


     getInitialState: function() {

       return { isFlagged: false };
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

   
    initializePopovers: function(){

        if (!this.props.showAll){
          return;
        }

       
            $(this.getDOMNode()).popover({
                                        selector: '[rel="popover"]',
                                        trigger: 'click', 
                                        placement:'bottom',           
                                        html: true
                                        });  
        
        
      
    },

    renderCourseCountBadge: function(){
      if (!this.props.isShared){
        return "";
      }
      return(React.DOM.span( {className:"badge"}, this.props.titleCount));
    },


    renderSharedButtons: function(){
      if(this.props.showAll){


    return ( React.DOM.div( {className:"shared-placeholder"},  
              React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to", rel:"popover",  'data-title':this.props.isShared? "Shared with:" : "",  'data-content':this.props.isShared? this.props.data["sharedWith"] : "<b>Not Shared</b>"} , 
                 React.DOM.span( {className:"glyphicon icon-shared-to"} ),this.renderCourseCountBadge() 
               ),
               React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.shareHandler, 'data-toggle':"tooltip", title:"Share this question"}, React.DOM.span( {className:"glyphicon glyphicon-plus-sign"})), 
                    this.props.isShared?
                      React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.removeTitleHandler, 'data-toggle':"tooltip", title:"Remove from title"}, React.DOM.span( {className:"glyphicon glyphicon-minus-sign"})) :
                    ""
               ));
     }

      if(this.props.isShared){
      return(
         React.DOM.div( {className:"shared-placeholder"}, 
                           
                         
           React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to"}, 
                    React.DOM.span( {className:"glyphicon icon-shared-to"} ),this.renderCourseCountBadge()
           ), 
           React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.shareHandler, 'data-toggle':"tooltip", title:"Share this question"}, React.DOM.span( {className:"glyphicon glyphicon-plus-sign"})), 
             this.props.isShared?
              React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.removeTitleHandler, 'data-toggle':"tooltip", title:"Remove from title"}, React.DOM.span( {className:"glyphicon glyphicon-minus-sign"})) :
               ""
                             
          ));
    } 

     

    return ( React.DOM.div( {className:"shared-placeholder"},  " " ));

    },

    renderEditMenu: function(){
                  if (!this.props.isShared){
                    return null;
                  }
                  return(
                     React.DOM.ul( {className:"dropdown-menu show-menu", role:"menu", 'aria-labelledby':"dropdownMenuType", onClick:this.changeEventHandler, 'aria-labelledby':"edit-question"}, 
                        React.DOM.li( {role:"presentation", className:"dropdown-header"}, "Edit options"),
                       React.DOM.li( {role:"presentation", className:"divider"}),
                       React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick:this.props.editQuestionHandler}, "Edit in ", this.props.titleCount == 1? "1 title" : "all "+this.props.titleCount+" titles")),
                       React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick:this.copyQuestionHandler}, "Create a copy"))
                     ));
    },

    renderMenu: function(){
      if (this.props.showAll){
      return(React.DOM.div( {className:"menu-container"}, 
                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Edit Notes"}, React.DOM.span( {className:"glyphicon glyphicon-list-alt"})
                     ), 
                     React.DOM.div( {className:"dropdown"}, 
                     React.DOM.button( {id:"edit-question", type:"button", className:"btn btn-default btn-sm", onClick:this.editQuestionHandler,  'data-target':"#", 'data-toggle':"dropdown", title:"Edit Question"}, 
                           React.DOM.span( {className:"glyphicon glyphicon-pencil", 'data-toggle':"tooltip", title:"Edit Question"})
                     ),
                      this.renderEditMenu()
                     ),


                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.copyQuestionHandler,  'data-toggle':"tooltip", title:"Duplicate Question"}, React.DOM.span( {className:"glyphicon glyphicon-copyright-mark"}))
                  ));
     }

      return (React.DOM.div( {className:"menu-container"}));

    },

    renderStaticMenu: function(){
        if (this.props.showAll){
          return(React.DOM.div( {className:"menu-container static"}, 
                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.toggleFlag, 'data-toggle':"tooltip", title:this.state.isFlagged? "Unflag question" : "Flag question"}, 
                     React.DOM.span( {className:this.state.isFlagged? "glyphicon glyphicon-flag flagged" : "glyphicon glyphicon-flag"})
                     ) 
                  ));
      }

      return (React.DOM.div( {className:"static-menu-container"}, 
                React.DOM.div( {className:"notification-icons-container"}, 
                    this.state.isFlagged ? React.DOM.span( {className:"glyphicon glyphicon-flag flagged"}) : ""
                )
              ));
    },



    render: function() {

        return ( 
                React.DOM.div(null, 
                   this.renderSharedButtons(),
                   this.renderStaticMenu(),
                   this.renderMenu()      
                )
            );
    


  }
});