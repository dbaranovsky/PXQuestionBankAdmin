/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorBase = React.createClass({
    
    renderSpecificEditor: function() {
        switch (this.props.metadata.editorType) {
          case window.enums.editorType.text:
            return (<QuestionInlineEditorText afterEditingHandler={this.props.afterEditingHandler} 
                    metadata={this.props.metadata}/>);
          case window.enums.editorType.status:
            return (<QuestionInlineEditorStatus afterEditingHandler={this.props.afterEditingHandler}
                        metadata={this.props.metadata}
                        statusEnum={window.enums.questionStatus} />);
          case window.enums.editorType.number:
            return (<QuestionInlineEditorNumber afterEditingHandler={this.props.afterEditingHandler}
                       metadata={this.props.metadata}
                       statusEnum={window.enums.questionStatus} />);
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