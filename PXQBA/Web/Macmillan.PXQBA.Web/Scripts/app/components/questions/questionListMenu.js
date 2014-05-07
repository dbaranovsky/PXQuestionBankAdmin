/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({displayName: 'QuestionListMenu',

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
      return(React.DOM.span( {className:"badge"}, this.state.titleCount));
    },

    renderSharedButtons: function(){
      if(this.props.showAll){


    return ( React.DOM.div( {className:"shared-placeholder"},  
              React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to", rel:"popover",  'data-content':this.props.data["sharedWith"]}, 
                 React.DOM.span( {className:"glyphicon icon-shared-to"} ),this.renderCourseCountBadge() 
               ),
               React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Share this question"}, React.DOM.span( {className:"glyphicon glyphicon-plus-sign"})), 
                    this.state.isShared?
                      React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Remove title"}, React.DOM.span( {className:"glyphicon glyphicon-minus-sign"})) :
                    ""
               ));
     }

      if(this.state.isShared){
      return(
         React.DOM.div( {className:"shared-placeholder"}, 
                           
                         
           React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to",  'data-content':this.props.data["sharedWith"]}, 
                    React.DOM.span( {className:"glyphicon icon-shared-to"} ),this.renderCourseCountBadge()
           ), 
           React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Add title"}, React.DOM.span( {className:"glyphicon glyphicon-plus-sign"})), 
             this.state.isShared?
              React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Add title"}, React.DOM.span( {className:"glyphicon glyphicon-minus-sign"})) :
               ""
                             
          ));
    } 

     

    return ( React.DOM.div( {className:"shared-placeholder"},  " " ));

    },

    renderEditMenu: function(){
                  if (!this.state.isShared){
                    return null;
                  }
                  return(
                     React.DOM.ul( {className:"dropdown-menu show-menu", role:"menu", 'aria-labelledby':"dropdownMenuType", onClick:this.changeEventHandler, 'aria-labelledby':"edit-question"}, 
                        React.DOM.li( {role:"presentation", className:"dropdown-header"}, "Edit options"),
                       React.DOM.li( {role:"presentation", className:"divider"}),
                       React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick:this.props.editQuestionHandler}, "Edit in ", this.state.titleCount == 1? "1 title" : "all "+this.state.titleCount+" titles")),
                       React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick:this.copyQuestionHandler}, "Create a copy"))
                     ));
    },


    render: function() {
      if (this.props.showAll){

        return ( 
                React.DOM.div(null, 
                  
                   this.renderSharedButtons(),
                   React.DOM.div( {className:"menu-container"}, 
                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Edit Notes"}, React.DOM.span( {className:"glyphicon glyphicon-list-alt"})
                     ), 
                     React.DOM.div( {className:"dropdown"}, 
                     React.DOM.button( {id:"edit-question", type:"button", className:"btn btn-default btn-sm", onClick:this.editQuestionHandler,  'data-target':"#", 'data-toggle':"dropdown", title:"Edit Question"}, 
                           React.DOM.span( {className:"glyphicon glyphicon-pencil", 'data-toggle':"tooltip", title:"Edit Question"})
                     ),
                      this.renderEditMenu()
                     ),


                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.copyQuestionHandler,  'data-toggle':"tooltip", title:"Duplicate Question"}, React.DOM.span( {className:"glyphicon glyphicon-copyright-mark"}))
                  )

                           
                )
            );
      }

       return (React.DOM.div(null,  " ", this.renderSharedButtons()));
  }
});