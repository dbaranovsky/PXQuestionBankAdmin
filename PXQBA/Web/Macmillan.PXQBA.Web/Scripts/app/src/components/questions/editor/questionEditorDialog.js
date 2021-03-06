/**
* @jsx React.DOM
*/

var QuestionEditorDialog = React.createClass({

     getInitialState: function() {

      return { notesChanged: false};
    },

    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
         monitorChanges(".local", false);
         var self = this;
       
        $(this.getDOMNode()).on('hidden.bs.modal', function () {
            self.props.closeDialogHandler();
               if(self.state.notesChanged){
                      questionDataManager.resetState();
                 }

        })

    },
    finishSaving: function(e){

        questionDataManager.resetState();
     //   $(this.getDOMNode()).modal("hide");

        var text = this.props.caption == window.enums.dialogCaptions.editQuestion?  
                      window.enums.messages.succesUpdate :
                      window.enums.messages.succesCreate;

        notificationManager.showSuccess(text);
    },

    closeDialog: function(isClearResources){
         monitorChanges(".local", true);
         $(this.getDOMNode()).modal("hide");
         $('.modal-backdrop').remove(); 

         if(isClearResources){
            this.clearResources();
         }
       

         //quick workaround for IE11 issue with highlighting in qba-371
          if(!!navigator.userAgent.match(/Trident.*rv[ :]*11\./) && 
             this.props.caption == window.enums.dialogCaptions.duplicateQuestion &&
             this.props.hasContainsTextFilter) {
               location.reload();
         }

    },

    notesChangedHandler: function(){
       this.setState({notesChanged: true});
     },

     clearResources: function(response){
             var questionType = this.props.question.questionType == null? "" :this.props.question.questionType.toLowerCase();
             var needRemoveResources = false;
             if ( questionType != "hts" && questionType !="fma_graph"){
                needRemoveResources = true;
             }

              questionDataManager.clearResources(this.props.currentCourseId, this.props.question.realQuestionId, needRemoveResources);
     },

     closeModal: function(){
        this.closeDialog(true);
     },

    render: function() {
         var self = this;
        var renderHeaderText = function() {
            return self.props.caption;
        };
        var renderBody = function(){
            return (<QuestionEditor  currentCourseId = {self.props.currentCourseId}
                                     question={self.props.question}
                                     metadata={self.props.metadata}  
                                     editSourceQuestionHandler={self.props.editSourceQuestionHandler} 
                                     finishSaving = {self.finishSaving} 
                                     closeDialog={self.closeDialog} 
                                     isNew={self.props.isNew} 
                                     isDuplicate={self.props.caption === window.enums.dialogCaptions.duplicateQuestion}
                                     handlers={self.props.handlers}
                                     viewHistoryMode = {self.props.viewHistoryMode}
                                     isEditedInPlace = {self.props.isEditedInPlace}
                                     caption={self.props.caption}
                                     notesChangedHandler = {self.notesChangedHandler}
                                     clearResources = {self.clearResources}
                                     />);
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             renderFooterButtons={renderFooterButtons} 
                             closeDialogHandler = {this.closeModal}
                             dialogId="questionEditorModal"
                             isMainEditor={true}/>
                );
    }
});