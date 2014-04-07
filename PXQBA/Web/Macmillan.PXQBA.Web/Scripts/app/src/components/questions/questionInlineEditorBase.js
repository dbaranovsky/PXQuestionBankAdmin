/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorBase = React.createClass({
    
    saveVelueHandler: function(value) {
        if(value != null) {
          questionDataManager.saveQuestionData(this.props.metadata.questionId,
                                               this.props.metadata.field,
                                               value);
 
        }
        this.props.afterEditingHandler();
    },

    renderSpecificEditor: function() {
        switch (this.props.metadata.editorDescriptor.editorType) {
          case window.enums.editorType.text:
            return (<QuestionInlineEditorText saveVelueHandler={this.saveVelueHandler} 
                    afterEditingHandler={this.props.afterEditingHandler}
                    metadata={this.props.metadata}/>);
          case window.enums.editorType.singleSelect:
            return (<QuestionInlineEditorSingleSelect saveVelueHandler={this.saveVelueHandler} 
                        metadata={this.props.metadata}
                        values={this.props.metadata.editorDescriptor.availableChoice} />);
          case window.enums.editorType.number:
            return (<QuestionInlineEditorNumber saveVelueHandler={this.saveVelueHandler} 
                       afterEditingHandler={this.props.afterEditingHandler}
                       metadata={this.props.metadata} />);
          default:
            return null;
        }
    },

     render: function() {
        return ( 
            <div>
               {this.renderSpecificEditor()}
            </div>
            );
     }

});