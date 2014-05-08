/**
* @jsx React.DOM
*/

var QuestionShareDialog = React.createClass({

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
            return (<div> <div className="waiting" /></div>);
          }

            return (<div>

                        <ShareQuestionBox metadata={self.state.metadata} questionIds={self.props.questionIds} closeDialogHandler={self.props.closeDialogHandler}/>
                    </div>
            );
        };

   

        return (<ModalDialog renderHeaderText={renderHeaderText} renderBody={renderBody}  dialogId="shareQuestionModal"/>);
    }
});

var ShareQuestionBox = React.createClass({

 
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

   componentDidMount: function()   {

   },
    
    shareQuestion: function(){
        questionDataManager.bulk.shareTitle(this.props.questionIds, this.state.shareViewModel);
        this.props.closeDialogHandler();
    },

    renderWaiter: function(){
        if (this.state.loading){
           return (<div className="waiting small"></div>);
        }

        return (<div></div>);
    },
    render: function() {
            return (<div>
                           
                           <MetadataFieldEditor question={this.state.shareViewModel} metadata={this.state.metadata} setDefault={this.state.setDefaults} editHandler={this.productTitleEditHandler} field={"course"} title={"Target title"}/>
                           {this.renderWaiter()}
                           <MetadataFieldEditor question={this.state.shareViewModel} metadata={this.state.metadata} setDefault={true} isDisabled={this.state.loading} reload={true} editHandler={this.editHandler} field={"chapter"} title={"Target chapter"}/>
                           <MetadataFieldEditor question={this.state.shareViewModel} metadata={this.state.metadata} setDefault={true} isDisabled={this.state.loading}  reload={true} editHandler={this.editHandler} field={"bank"} title={"Target bank"}/>

                            <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-default" data-dismiss="modal" onClick={this.props.closeDialogHandler}>Cancel</button>
                                 <button type="button" className="btn btn-primary" data-dismiss="modal"  onClick={this.shareQuestion}>Share</button>
                            </div>
                   </div>
               );
    }
});