/**
* @jsx React.DOM
*/
var QuestionEditorTabs = React.createClass({

  
    getInitialState: function() {
      return { metadata: []};
    },

    
    loadMetadata: function(data)
    {
        this.setState({metadata: data});
    },

    tabsInitializer: function (container) {
       //  container.find('a:first').tab('show')
    },

    componentDidMount: function() {
         var tabs = this.getDOMNode();
         this.tabsInitializer($(tabs));
         $(tabs).find('iframe').load(function(){
           $(tabs).find('.iframe-waiting').hide();
           $(tabs).find('iframe').show();
        });
          
    },

    componentWillMount: function(){
      questionDataManager.getMetadataFields().done(this.loadMetadata); 
    },
    componentDidUpdate: function () {
        this.tabsInitializer($(this.getDOMNode()));
    },

    loadSourceQuestion: function(event){
      event.preventDefault();
      this.props.getSourceQuestion();
    },

    renderSharingNotification: function(){
      if (this.props.question.isDuplicateOfSharedQuestion && this.props.isDuplicate) {
        return (<div className="shared-note">This question is a duplicate of a &nbsp;
                    <a className="shared-question-link" href="" onClick={this.loadSourceQuestion}>shared question</a>
                    <a href="" onClick={this.loadSourceQuestion}>Delete question</a>
               </div>);
      }

      return null;
    },

    renderOverideControls: function(){

      if ( this.props.question.sourceQuestion !=  null) {
        return (<OverideControls />);
      }
      return null; 

    },


  renderSharedMetadataEditor: function(){
      if ( this.props.question.sourceQuestion != null) {
         return(<SharedMetadataEditor metadata={this.state.metadata}   question={this.props.question} editHandler={this.props.editHandler} isDuplicate={this.props.isDuplicate} />);   
      }
      return null;
  },

    render: function() {
        return ( 
                <div>
                  
                   
                        <ul className="nav nav-tabs">
                             <li className="active"> 
                                 <a href="#body" data-toggle="tab">Body</a>
                             </li>
                             <li>
                                 <a href="#metadata" data-toggle="tab">Metadata</a>
                             </li>
                              <li>
                                 <a href="#history" data-toggle="tab">History</a>
                             </li>
                        </ul>
             

                  
             
                <div className="tab-content">
                    <div className="tab-pane active" id="body">
                       <div className="tab-body">
                          <div className="iframe-waiting" />
                          <iframe src={this.props.question.editorUrl} />
                          
                       </div>
                    </div>
                    <div className="tab-pane" id="metadata">
                       <div className={this.props.question.sourceQuestion == null ? "tab-body" : "tab-body wide"}>
                            {this.renderSharingNotification()}
                           
                            {this.renderSharedMetadataEditor()}
                             {this.renderOverideControls()}
                           <QuestionMetadataEditor  metadata={this.state.metadata} question={this.props.question} editHandler={this.props.editHandler} isDuplicate={this.props.isDuplicate} />
                           <br />
                      </div>
                    </div>
                     <div className="tab-pane" id="history">
                       <div className="tab-body">
                       Lorem Ipsum
                       </div>
                </div>
                </div>
               

            </div>
            );
        }

});