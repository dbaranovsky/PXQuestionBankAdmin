/**
* @jsx React.DOM
*/

var QuestionShareDialog = React.createClass({displayName: 'QuestionShareDialog',

   
  getInitialState: function() {
      return { 
              waiting: true
             };
    },

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
        this.setState({metadata: data, waiting: false});       
    },

    shareQuestion: function(shareViewModel){
        this.setState({waiting: true, shareViewModel: shareViewModel})
        questionDataManager.bulk.shareTitle(this.props.questionIds, shareViewModel).done(this.finishShare); 
    },

     getUrlToList: function(titleId, chapterId) {
      return window.actions.questionList.buildQuestionListIndexUrl(titleId, chapterId);
    },

    finishShare: function(){
            var message = this.props.questionIds.length ==1 ?
                         "Question was shared successfully. This question may require metadata editing." : 
                          this.props.questionIds.length+" question were shared successfully. These questions may require metadata editing. ";
            var url = this.getUrlToList(this.state.shareViewModel.course);
            var link = '<a href='+url+'>Go to the target title </a>'
           var notifyOptions = {
            message: { html: message+link},
            type: 'success',
            fadeOut: { enabled: false}
        };
        $('.top-center').notify(notifyOptions).show();
          this.props.closeDialogHandler();
    },

     getUrlToList: function(titleId, chapterId) {
      return window.actions.questionList.buildQuestionListIndexUrl(titleId, chapterId)
    },
 
   
    render: function() {
       var self = this;
        var renderHeaderText = function() {
            if(self.props.questionIds.length>1){
                return "Share question";
            }
            return "Share questions";
        };

      
        var renderBody = function(){
          if (self.state.waiting){
            return (React.DOM.div(null,  " ", React.DOM.div( {className:"waiting"} )));
          }

            return (React.DOM.div(null, 

                        ShareQuestionBox( {metadata:self.state.metadata, shareHandler:self.shareQuestion, closeDialogHandler:self.props.closeDialogHandler})
                    )
            );
        };

   

        return (ModalDialog( {renderHeaderText:renderHeaderText, renderBody:renderBody,  dialogId:"shareQuestionModal"}));
    }
});

var ShareQuestionBox = React.createClass({displayName: 'ShareQuestionBox',

 
   getInitialState: function() {
      return { 
               shareViewModel: {course: 0, chapter:"", bank:""},
               metadata: this.props.metadata,
               setDefaults: true,
               loading: false
             };
    },


   editHandler: function(question){

    this.setState({shareViewModel: question});
   },

   productTitleEditHandler: function(shareViewModel){
     this.setState({loading: true});
     questionDataManager.getCourseMetadata(shareViewModel.course).done(this.changeCourseMetadata.bind(this, shareViewModel.course));
   },

   changeCourseMetadata: function(courseId, metadata){
      this.setState({
               shareViewModel: {course: courseId, chapter:"", bank:""},
               metadata: metadata,
               setDefaults: false,
               loading: false
      });
   },
    shareQuestion: function(){
      this.props.shareHandler(this.state.shareViewModel);
   } ,
    renderWaiter: function(){
        if (this.state.loading){
           return (React.DOM.div( {className:"waiting small"}));
        }

        return (React.DOM.div(null));
    },

  

    render: function() {
            return (React.DOM.div(null, 
                           
                           MetadataFieldEditor( {question:this.state.shareViewModel, metadata:this.state.metadata, setDefault:this.state.setDefaults, editHandler:this.productTitleEditHandler, field:"course", title:"Target title"}),
                           this.renderWaiter(),
                           MetadataFieldEditor( {question:this.state.shareViewModel, metadata:this.state.metadata, setDefault:true, isDisabled:this.state.loading, reload:true, editHandler:this.editHandler, field:"chapter", title:"Target chapter"}),
                           MetadataFieldEditor( {question:this.state.shareViewModel, metadata:this.state.metadata, setDefault:true, isDisabled:this.state.loading,  reload:true, editHandler:this.editHandler, field:"bank", title:"Target bank"}),

                            React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", onClick:this.props.closeDialogHandler}, "Cancel"),
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:this.shareQuestion}, "Share")
                            )
                   )
               );
    }
});