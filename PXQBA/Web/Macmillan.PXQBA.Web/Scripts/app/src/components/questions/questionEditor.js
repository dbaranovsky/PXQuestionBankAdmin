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
    finishSaving: function(){
        questionDataManager.resetState();
        $(this.getDOMNode()).modal("hide");
        $('.top-center').notify({message: { text: 'Question created!' },
                                 fadeOut: { enabled: true, delay: 3000 } }).show();
    },

    closeDialog: function(){
         $(this.getDOMNode()).modal("hide");
    },


    render: function() {
        var renderHeaderText = function() {
            return "New Question";
        };
        var question = this.props.question;
        var finishSaving = this.finishSaving;
        var closeDialog = this.closeDialog;
        var renderBody = function(){
            return (<QuestionEditor question={question} finishSaving = {finishSaving} closeDialog={closeDialog}/>);
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

   getInitialState: function() {

      return { question: this.props.question };
    },


    saveQuestion: function(){
        if(this.props.isNew)
        {
            //create
        }

        //save existing
        //for duplicate and create new should be called create question. Save question should be implemented
        var finishSaving = this.props.finishSaving;
        questionDataManager.createQuestion("1", this.state.question).always(function(e){
            finishSaving();

        });

    },

    editHandler: function(editedQuestion){
      this.setState({question: editedQuestion});
    },

    componentDidMount: function(){
       
    },

    render: function() {
        return (
            <div>
                      <div className="header-buttons">
                         <button className="btn btn-primary run-question" data-toggle="modal" >
                             <span className="glyphicon glyphicon-play"></span> Run Question
                        </button>
                        <button className="btn btn-default" data-toggle="modal" onClick={this.props.closeDialog}>
                             Cancel
                        </button>
                         <button className="btn btn-primary " data-toggle="modal" onClick={this.saveQuestion} >
                             Save
                        </button>
                      </div>
                
                <div>
                  <QuestionEditorTabs question={this.state.question} editHandler={this.editHandler} />
                </div>
         </div>);
    }
});


var QuestionEditorTabs = React.createClass({

    tabsInitializer: function (container) {
       //  container.find('a:first').tab('show')
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
                       <QuestionMetadataEditor  question={this.props.question} editHandler={this.props.editHandler} />
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

var QuestionMetadataEditor = React.createClass({

    getInitialState: function() {
      return { metadata: []};
    },

    
    loadMetadata: function(data)
    {
        this.setState({metadata: data});
    },

    componentDidMount: function(){
       questionDataManager.getMetadataFields().done(this.loadMetadata); 
    },
    render: function() {
       
        return (
             <div className="tab-body">
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"title"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"chapter"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"bank"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"excerciseNo"} title="Excercise Number"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"difficulty"} />
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"cognitiveLevel"} title="CognitiveLevel"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"version"} />
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"guidance"} isMultiline={true}/>
             </div> 
         );
    }
});

var MetadataFieldEditor = React.createClass({

     saveValueHandler: function(){

     },

     editHandler: function(){
      
       var node = this.refs.editor.getDOMNode();
       var text = "";
       if (node.selectedOptions !== undefined){
            text = node.selectedOptions[0].text;
       } 
       else {
            text = node.value;
       }

      var question = this.props.question;
      if (question[this.props.field] !== text)
      {
        question[this.props.field] = text;
        this.props.editHandler(question);
      }

     },

    renderMenuItems: function(availableChoices) {
        var items = [];
        for (var propertyName in availableChoices) {
            availableChoice = availableChoices[propertyName];
            items.push(this.renderMenuItem(availableChoice, propertyName));
        }
        return items;
    },

    renderMenuItem: function(label, value) {
        return (<option value={label}>{label}</option>);
    },

     renderBody: function(){


       var field = this.props.field;
       var metadataField = $.grep(this.props.metadata, function(e){ return e.name === field; });
       var editorType = metadataField.length>0 ? metadataField[0].typeDescriptor.type : 0;
       var currentValue = this.props.question[this.props.field];
       switch (editorType) {
          //case window.enums.editorType.singleSelect:
          // Magic number! Do something with that!
          case 1:
             return (<select ref="editor" onChange={this.editHandler} value={currentValue}> {this.renderMenuItems(metadataField[0].typeDescriptor.availableChoice)} </select> );
          default: 
            if(!this.props.isMultiline){
                 return (<input type="text" onChange={this.editHandler} ref="editor" value={currentValue}/>)
             }
            return ( <textarea onChange={this.editHandler}  ref="editor" className="question-body-editor"  rows="10" type="text" placeholder="Enter text..." value={currentValue} />);
             
        }
    },



    render: function() {
        return (

            <div className="metadata-field-editor">
                   <label>{this.props.title === undefined ? this.props.field : this.props.title}</label>
                   <br />
                    {this.renderBody()}
                   <br />
                          
            </div> 
         );
    }

});