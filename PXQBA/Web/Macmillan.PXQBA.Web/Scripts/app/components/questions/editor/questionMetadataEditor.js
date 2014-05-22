/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({displayName: 'QuestionMetadataEditor',
   
    renderRows: function(){
      var rows = [];
      
      var localFieldsName = [];
      var defaultFieldsName = [];



      for (var localName in this.props.question.localValues){
        if(localName!= "sequence" && localName != "productcourseid" && localName != "flag")
        localFieldsName.push(localName);
      }

       if (this.props.question.defaultValues == null || this.props.question.defaultValues.length == 0 ){
             $.each(localFieldsName, function( index, value ) {
                 rows.push( ShareMetadataEditorRow( {question:self.props.question, metadata:self.props.metadata, editHandler:self.props.editHandler, field:value} ));
            });

             return rows;
      }


      for (var defaultFieldName in this.props.question.defaultValues){
        defaultFieldsName.push(defaultFieldName);
      }

      var fieldsWithAnalouges = $.grep(localFieldsName, function(e){ return $.inArray(e, defaultFieldsName)!=-1;});
      var fieldsWithoutAnalouges = $.grep(localFieldsName, function(e){ return $.inArray(e, defaultFieldsName)==-1;});


      var self = this;    
      $.each(fieldsWithAnalouges, function( index, value ) {
             rows.push( ShareMetadataEditorRow( {question:self.props.question, metadata:self.props.metadata, editHandler:self.props.editHandler, field:value} ));
      });


       $.each(fieldsWithoutAnalouges, function( index, value ) {
             rows.push( ShareMetadataEditorRow( {question:self.props.question, isUnique:true, metadata:self.props.metadata, editHandler:self.props.editHandler, field:value} ));
      });


    
      return rows;
    },

    render: function() {
        var style = this.props.question.isShared? {} : {display: "none !important"};
        return ( React.DOM.div( {className:this.props.question.isShared? "local wide" : "local" }, 
                      React.DOM.div( {className:"row header", style:style}, 
                        React.DOM.div( {className:"cell"},  " ", React.DOM.span( {className:"label label-default metadata-info-label"}, "Shared values")),
                        React.DOM.div( {className:"cell control"}),
                        React.DOM.div( {className:"cell"},  " ", React.DOM.span( {className:"label label-default metadata-info-label"}, "Local values"))
                      ),
                      React.DOM.div( {className:"body-container"}, 

                         this.renderRows()
                       
                        
                 
                     )          
                 )

           
         );
    }
});