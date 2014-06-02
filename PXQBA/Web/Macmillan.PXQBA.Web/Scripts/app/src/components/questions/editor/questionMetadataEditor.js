/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({
   
    getInitialState: function(){
       return ({metadataLoaded: false}); 
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
      var rows = [];
      
      var localFieldsName = [];
      var defaultFieldsName = [];



      for (var localName in this.props.question.localValues){
        if($.inArray(localName,["sequence", "productcourseid", "flag", "questionIdDuplicateFromShared"]) ==-1)
        localFieldsName.push(localName);
      }

       if (this.props.question.defaultValues == null || this.props.question.defaultValues.length == 0 ){
             $.each(localFieldsName, function( index, value ) {
                 rows.push( <ShareMetadataEditorRow question={self.props.question} metadata={self.props.metadata} courseMetadata={self.state.courseMetadata} editHandler={self.props.editHandler} field={value} />);
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
             rows.push( <ShareMetadataEditorRow question={self.props.question} courseMetadata={self.state.courseMetadata}  metadata={self.props.metadata} editHandler={self.props.editHandler} field={value} />);
      });


       $.each(fieldsWithoutAnalouges, function( index, value ) {
             rows.push( <ShareMetadataEditorRow question={self.props.question} courseMetadata={self.state.courseMetadata}  isUnique={true} metadata={self.props.metadata} editHandler={self.props.editHandler} field={value} />);
      });


    
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
           return ( <div className={localClass}>
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

        return (<div className="waiting"/>)
       
    }
});