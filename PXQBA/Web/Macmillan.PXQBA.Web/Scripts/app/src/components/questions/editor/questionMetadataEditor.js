/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({
   
    render: function() {
        var style = this.props.question.sourceQuestion != null? {} : {display: "none !important"};
        return ( <div className={this.props.question.sourceQuestion == null ? "local" : "local wide"}>
                      <div className="row" style={style}>
                        <div className="cell"> <span className="label label-info metadata-info-label">Shared values</span></div>
                        <div className="cell control"></div>
                        <div className="cell"> <span className="label label-info metadata-info-label">Local values</span></div>
                      </div>

                         
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="title" />
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="chapter" />
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="bank" />
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="keywords" />
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="suggestedUse" title="Suggested Use"/>
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="learningObjectives"  title="Learning Objective"/>
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="excerciseNo" title="Exercise Number"/>
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="difficulty" />
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="cognitiveLevel" title="Cognitive Level"/>
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="status" />
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="guidance" />            
                 </div>

           
         );
    }
});