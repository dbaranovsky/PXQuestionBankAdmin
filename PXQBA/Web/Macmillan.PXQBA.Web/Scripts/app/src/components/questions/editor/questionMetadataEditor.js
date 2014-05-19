/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({
   
    renderRows: function(){
      var rows = [];
      
      var localFieldsName = [];
      var defaultFieldsName = [];

      for (var localName in this.props.question.localValues){
        if(localName!= "sequence" && localName != "productcourseid" && localName != "flag")
        localFieldsName.push(localName);
      }

      for (var defaultFieldName in this.props.question.defaultValues){
        defaultFieldsName.push(defaultFieldName);
      }


      var fieldsWithAnalouges = $.grep(localFieldsName, function(e){ return $.inArray(e, defaultFieldsName)!=-1;});
      var fieldsWithoutAnalouges = $.grep(localFieldsName, function(e){ return $.inArray(e, defaultFieldsName)==-1;});


      var self = this;    
      $.each(fieldsWithAnalouges, function( index, value ) {
             rows.push( <ShareMetadataEditorRow question={self.props.question} metadata={self.props.metadata} editHandler={self.props.editHandler} field={value} />);
      });


       $.each(fieldsWithoutAnalouges, function( index, value ) {
             rows.push( <ShareMetadataEditorRow question={self.props.question} isUnique={true} metadata={self.props.metadata} editHandler={self.props.editHandler} field={value} />);
      });


    
      return rows;
    },

    render: function() {
        var style = this.props.question.defaultValues != null? {} : {display: "none !important"};
        return ( <div className={this.props.question.defaultValues == null ? "local" : "local wide"}>
                      <div className="row header" style={style}>
                        <div className="cell"> <span className="label label-default metadata-info-label">Shared values</span></div>
                        <div className="cell control"></div>
                        <div className="cell"> <span className="label label-default metadata-info-label">Local values</span></div>
                      </div>
                      <div className="body-container">

                         {this.renderRows()}
                       
                        
                 
                     </div>          
                 </div>

           
         );
    }
});