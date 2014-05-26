/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',

      
        reloadPreview: function(){
            $(this.getDOMNode()).find(".question-preview").html(this.props.preview);
        },
       componentDidUpdate: function(){
            this.reloadPreview();
       },

       componentDidMount: function(){
          this.reloadPreview();
       },
       compileTemplate: function(){

            var template = Handlebars.compile(this.props.questionCardTemplate);
            var html = template(this.props.metadata);
            return html;
       },
        render: function() {
          var questionType = this.props.metadata[window.consts.questionTypeName];
          var isCustom = false;
          var className = "question-preview";


          if (questionType== window.enums.questionType.graphDisplayName) {
              className += " graph";
              isCustom = true;
          }

          if (questionType == window.enums.questionType.htsDisplayName){
            className+= " hts";
            isCustom = true;
          }

            return ( 
                  React.DOM.tr(null, 
                    React.DOM.td( {colSpan:this.props.colSpan}, 
                       React.DOM.div( {className:className}),
                         React.DOM.hr(null ),
                         React.DOM.div( {className:"question-card-template", dangerouslySetInnerHTML:{__html: this.compileTemplate()}} )
                    )
                  )
            );
        }
});