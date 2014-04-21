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
                       <QuestionMetadataEditor  question={this.props.question} editHandler={this.props.editHandler} />
                           <br />

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