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
    finishSaving: function(e){

        questionDataManager.resetState();
        $(this.getDOMNode()).modal("hide");

        var text = this.props.caption == window.enums.dialogCaptions.editQuestion?  
                      window.enums.messages.succesUpdate :
                      window.enums.messages.succesCreate;


        var notifyOptions = {message: { text: text }, 
                             type: 'success',
                             fadeOut: { enabled: true, delay: 3000 } };
        if(e.status != 200)
        {
            notifyOptions.type = 'danger';
            notifyOptions.message.text = window.enums.messages.errorMessage;
        }
        $('.top-center').notify(notifyOptions).show();
    },

    closeDialog: function(){
         $(this.getDOMNode()).modal("hide");
    },


    render: function() {
         var self = this;
        var renderHeaderText = function() {
            return self.props.caption;
        };
        var renderBody = function(){
            return (<QuestionEditor question={self.props.question} finishSaving = {self.finishSaving} closeDialog={self.closeDialog}/>);
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
       // if(this.props.isNew)
       // {
       // }
        var finishSaving = this.props.finishSaving;
        questionDataManager.updateQuestion(this.state.question).done(finishSaving);

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
       //   <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"status"} />
        return (
             <div className="tab-body">
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"title"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"chapter"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"bank"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"excerciseNo"} title="Excercise Number"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"difficulty"} />
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"cognitiveLevel"} title="CognitiveLevel"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"status"} />
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
            value = node.selectedOptions[0].value;
            if (text.toLowerCase()!= value.toLowerCase())
            {
              text = value;
            }
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
        if(label.toLowerCase()== value.toLowerCase()){
          return (<option value={label}>{label}</option>);
        }
        return (<option value={value}>{label}</option>);
    },

     renderBody: function(){


       var field = this.props.field;
       var metadataField = $.grep(this.props.metadata, function(e){ return (e.metadataName === field) || (e.metadataName === "dlap_q_"+field); });
       var editorType = metadataField.length>0 ? metadataField[0].editorDescriptor.editorType : 0;
       var currentValue = this.props.question[this.props.field];
       switch (editorType) {
          case window.enums.editorType.singleSelect:
             return (<select ref="editor" onChange={this.editHandler} value={currentValue}> {this.renderMenuItems(metadataField[0].editorDescriptor.availableChoice)} </select> );
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