/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({displayName: 'QuestionListMenu',

    editNotesHandler: function(){
      this.props.editNotesHandler();
    },

    copyQuestionHandler: function() {
      this.props.copyQuestionHandler();
    },

    editQuestionHandler: function() {
        this.props.editQuestionHandler();
    },

    componentDidUpdate: function(){
      this.initializePopovers();
    },

     componentDidMount: function(){
       this.initializePopovers();
    },

    initializePopovers: function(){
        $(this.getDOMNode()).find('.shared-to').popover({title: 'Shared to:', trigger: 'hover', placement:'bottom', container:'body', html: true});
        $(this.getDOMNode()).find('.shared-from').popover({title: 'Unsubscribe from:', trigger: 'hover', placement:'bottom', container:'body', html: true});
    },


    render: function() {
      if (this.props.showAll){
        return ( 
                React.DOM.div(null, 

                     
                    this.props.data["sharedTo"] != "" ?
                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to", 'data-content':this.props.data["sharedTo"]}, React.DOM.span( {className:"glyphicon icon-shared-to"} )) :
                    "",
                 

                    
                    this.props.data["sharedFrom"] != "" ?
                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-from", 'data-content':this.props.data["sharedFrom"]}, React.DOM.span( {className:"glyphicon icon-shared-from"})) :
                    "",
                 

                
                
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Edit Notes"}, React.DOM.span( {className:"glyphicon glyphicon-list-alt"})),	
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.editQuestionHandler,  'data-toggle':"tooltip", title:"Edit Question"}, React.DOM.span( {className:"glyphicon glyphicon-pencil"})),
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.copyQuestionHandler,  'data-toggle':"tooltip", title:"Duplicate Question"}, React.DOM.span( {className:"glyphicon glyphicon-copyright-mark"})),
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip", title:"Delete Question"}, React.DOM.span( {className:"glyphicon glyphicon-trash"}))
                )
            );
      }

      return (React.DOM.div(null, 
                   
                    this.props.data["sharedTo"] != "" ?
                    React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to", 'data-content':this.props.data["sharedTo"]}, React.DOM.span( {className:"glyphicon icon-shared-to"})) :
                    "",
                 

                    
                    this.props.data["sharedFrom"] != "" ?
                    React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-from", 'data-content':this.props.data["sharedFrom"]}, React.DOM.span( {className:"glyphicon icon-shared-from"})) :
                    ""
                 
              

             ));
  }
});