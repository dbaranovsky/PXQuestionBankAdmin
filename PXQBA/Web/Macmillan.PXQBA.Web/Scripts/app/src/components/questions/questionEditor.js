/**
* @jsx React.DOM
*/

var QuestionEditorDialog = React.createClass({

    
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

    render: function() {
        var renderHeaderText = function() {
            return "New Question";
        };
        var question = this.props.question;
        var renderBody = function(){
            return (<QuestionEditor question={question} />);
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             renderFooterButtons={renderFooterButtons} 
                             dialogId="questionEditorModal"/>
                );
    }
});

var QuestionEditor = React.createClass({

  
    render: function() {
       
        return (
            <div>
                      <div className="header-buttons">
                         <button className="btn btn-primary run-question" data-toggle="modal" >
                             <span className="glyphicon glyphicon-play"></span> Run Question
                        </button>
                        <button className="btn btn-default" data-toggle="modal" >
                             Cancel
                        </button>
                         <button className="btn btn-primary " data-toggle="modal" >
                             Save
                        </button>
                      </div>
                
                <div>
                  <QuestionEditorTabs question={this.props.question}  />
                </div>
         </div>);
    }
});


var QuestionEditorTabs = React.createClass({

    tabsInitializer: function (container) {
         container.find('a:first').tab('show')
    },

    componentDidMount: function() {
         this.tabsInitializer($(this.getDOMNode()));
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
                       <td dangerouslySetInnerHTML={{__html: this.props.question.preview}} />
                       </div>
                    </div>
                    <div className="tab-pane" id="metadata">
                        <div className="tab-body">
                           <label>Title</label>
                           <br />
                           <input type="text" value={this.props.question.title}/>
                            <br /><br />
                           <label>Chapter</label>
                           <br />
                           <input type="text" value={this.props.question.chapter}/>
                            <br /><br />
                           <label>Bank</label>
                           <br />
                           <input type="text" value={this.props.question.bank} />
                            <br /><br />
                           <label>Excercise</label>
                           <br />
                           <input type="text" value={this.props.question.excerciseNo}/> 
                           <br /><br />

                           <label>Format</label>
                           <br />
                            <textarea className="question-body-editor"  rows="10" type="text" placeholder="Enter text..." ref="text" value={this.props.question.guidance} />
                           </div>  
                           <br />


                          
                        </div>
                </div>
                <div className="tab-pane" id="history">
                       <div className="tab-body">
                       
                       </div>
                </div>

            </div>
            );
        }

});