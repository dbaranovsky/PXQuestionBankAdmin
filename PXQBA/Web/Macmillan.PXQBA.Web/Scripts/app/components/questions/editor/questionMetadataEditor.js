/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({displayName: 'QuestionMetadataEditor',
   
    getInitialState: function(){
       return ({metadataLoaded: false, staticFieldsNames: ["title","chapter", "bank"]}); 
    },
    componentDidMount: function(){
      if (this.props.question.isShared){
        questionDataManager.getCourseMetadata(this.props.question.parentProductCourseId).done(this.setCourseMetadata);

      }else
      {
        this.setState({metadataLoaded: true, courseMetadata: []});
      }

    },

    setCourseMetadata: function(data){
        this.setState({metadataLoaded: true, courseMetadata: data});
    },


    renderRows: function(){
      var rows = this.addStaticRows();
      
      var localFieldsName = [];
      var defaultFieldsName = [];

      for(var i=0; i<this.props.metadata.length; i++) {
         if($.inArray(this.props.metadata[i].metadataName, [window.consts.sequenceName,
                                                            window.consts.questionCourseName, 
                                                            window.consts.flagName,  
                                                            window.consts.duplicateFromSharedName, 
                                                            window.consts.questionIdDuplicateFromShared,
                                                            window.consts.draftFromName]) ==-1) {
             if(this.props.question.localSection.dynamicValues.hasOwnProperty(this.props.metadata[i].metadataName)) {
                localFieldsName.push(this.props.metadata[i].metadataName);
             }
         }
      }


      if (this.props.question.defaultSection.dynamicValues == null || this.props.question.defaultSection.dynamicValues == 0 ){
             $.each(localFieldsName, function( index, value ) {
                 rows.push( ShareMetadataEditorRow( {question:self.props.question, metadata:self.props.metadata, courseMetadata:self.state.courseMetadata, editHandler:self.props.editHandler, field:value} ));
            });

             return rows;
      }


      for (var defaultFieldName in this.props.question.defaultSection.dynamicValues){
        defaultFieldsName.push(defaultFieldName);
      }

      var fieldsWithAnalouges = $.grep(localFieldsName, function(e){ return $.inArray(e, defaultFieldsName)!=-1;});
      var fieldsWithoutAnalouges = $.grep(localFieldsName, function(e){ return $.inArray(e, defaultFieldsName)==-1;});


      var self = this;    
      $.each(fieldsWithAnalouges, function( index, value ) {
             rows.push( ShareMetadataEditorRow( {question:self.props.question, courseMetadata:self.state.courseMetadata,  metadata:self.props.metadata, editHandler:self.props.editHandler, field:value} ));
      });


       $.each(fieldsWithoutAnalouges, function( index, value ) {
             rows.push( ShareMetadataEditorRow( {question:self.props.question, courseMetadata:self.state.courseMetadata,  isUnique:true, metadata:self.props.metadata, editHandler:self.props.editHandler, field:value} ));
      });


    
      return rows;
    },


    addStaticRows: function(){
      var rows=[];
      var self = this;
      rows.push(ShareMetadataEditorRow( {question:self.props.question, courseMetadata:self.state.courseMetadata,  isStatic:true,  isUnique:false, metadata:self.props.metadata, editHandler:self.props.editHandler, field:window.consts.questionTitleName} ));
      rows.push(ShareMetadataEditorRow( {question:self.props.question, courseMetadata:self.state.courseMetadata,  isStatic:true,  isUnique:false, metadata:self.props.metadata, editHandler:self.props.editHandler, field:window.consts.questionChapterName} ));
      rows.push(ShareMetadataEditorRow( {question:self.props.question, courseMetadata:self.state.courseMetadata,  isStatic:true,  isUnique:false, metadata:self.props.metadata, editHandler:self.props.editHandler, field:window.consts.questionBankName} ));

      return rows;
    },
    render: function() {
        var style = this.props.question.isShared? {} : {display: "none !important"};
        var localClass = "local";
        if (this.props.question.isShared){
          localClass+= " wide";
        } else if(this.props.question.sharedQuestionDuplicateFrom != null && this.props.isDuplicate){
          localClass +=" with-notification";
        }

        if(this.state.metadataLoaded){
           return ( React.DOM.div( {className:localClass}, 
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

        return (React.DOM.div( {className:"waiting"}))
       
    }
});