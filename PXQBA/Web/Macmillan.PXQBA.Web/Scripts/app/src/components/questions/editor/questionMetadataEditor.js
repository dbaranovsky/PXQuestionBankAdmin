/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({
   
    render: function() {
        var style = this.props.question.sharedMetadata != null? {} : {display: "none !important"};
        return ( <div className={this.props.question.sharedMetadata == null ? "local" : "local wide"}>
                      <div className="row header" style={style}>
                        <div className="cell"> <span className="label label-default metadata-info-label">Shared values</span></div>
                        <div className="cell control"></div>
                        <div className="cell"> <span className="label label-default metadata-info-label">Local values</span></div>
                      </div>
                      <div className="body-container">

                         
                        <ShareMetadataEditorRow question={this.props.question} metadata={this.props.metadata} editHandler={this.props.editHandler} field="title" />
                 
                     </div>          
                 </div>

           
         );
    }
});