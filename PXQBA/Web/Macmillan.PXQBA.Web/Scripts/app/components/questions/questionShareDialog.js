/**
* @jsx React.DOM
*/

var QuestionShareDialog = React.createClass({displayName: 'QuestionShareDialog',

    componentDidMount: function(){
        
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
        var closeDialogHandler = this.props.closeDialogHandler;
         $(this.getDOMNode()).on('hidden.bs.modal', function () {
           closeDialogHandler();
        })
    },

    componentWillMount: function(){
        questionDataManager.getMetadataFields().done(this.loadMetadata);
    },

    loadMetadata: function(data){
      this.setState({metadata: data});
    },

   
    render: function() {
        var renderHeaderText = function() {
            return "Share question";
        };

       var self = this;
        var renderBody = function(){
          if (self.state === undefined || self.state == null){
            return (React.DOM.div(null));
          }

            return (React.DOM.div(null, 

                        ShareQuestionBox( {metadata:self.state.metadata, questionIds:self.props.questionIds, closeDialogHandler:self.props.closeDialogHandler})
                    )
            );
        };

   

        return (ModalDialog( {renderHeaderText:renderHeaderText, renderBody:renderBody,  dialogId:"shareQuestionModal"}));
    }
});

var ShareQuestionBox = React.createClass({displayName: 'ShareQuestionBox',

 
   getInitialState: function() {
      return { 
               shareViewModel: {course: 0, chapter:"", bank:""}
             };
    },


   editHandler: function(question){

    this.setState({shareViewModel: question});
   },

   productTitleEditHandler: function(question){
    //get titles
   },

   componentDidMount: function()   {

   },
    
    shareQuestion: function(){
        questionDataManager.bulk.shareTitle(this.props.questionIds, this.state.shareViewModel);
        this.props.closeDialogHandler();
    },

    render: function() {
            return (React.DOM.div(null, 
                           
                           MetadataFieldEditor( {question:this.state.shareViewModel, metadata:this.props.metadata, setDefault:true, editHandler:this.productTitleEditHandler, field:"course", title:"Target title"}),
                           MetadataFieldEditor( {question:this.state.shareViewModel, metadata:this.props.metadata, setDefault:true,  editHandler:this.editHandler, field:"chapter", title:"Target chapter"}),
                           MetadataFieldEditor( {question:this.state.shareViewModel, metadata:this.props.metadata, setDefault:true,  editHandler:this.editHandler, field:"bank", title:"Target bank"}),

                            React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", onClick:this.props.closeDialogHandler}, "Cancel"),
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:this.shareQuestion}, "Share")
                            )
                   )
               );
    }
});