/**
* @jsx React.DOM
*/
var QuestionEditorTabs = React.createClass({


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
                       <div className={this.props.question.sharedMetadata == null ? "tab-body" : "tab-body wide"}>
                            {this.renderSharingNotification()}
                           
                            <QuestionMetadataEditor metadata={this.props.metadata} question={this.props.question} editHandler={this.props.editHandler} isDuplicate={this.props.isDuplicate} />
                           
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