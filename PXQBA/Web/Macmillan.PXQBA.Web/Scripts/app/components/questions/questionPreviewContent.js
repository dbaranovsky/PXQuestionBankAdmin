/**
* @jsx React.DOM
*/ 

var QuestionPreviewContent = React.createClass({displayName: 'QuestionPreviewContent',

       reloadPreview: function(){
            $(this.getDOMNode()).find(".question-preview-container").html(this.props.preview);
       },

       componentDidUpdate: function(){
            this.reloadPreview();
       },

       componentDidMount: function(){
          this.reloadPreview();
       },

       compileTemplate: function(){
            var templateRaw = this.props.questionCardTemplate;
            if(templateRaw==null) {
                  return "";
            }
            var template = Handlebars.compile(templateRaw);
            var html = template(this.props.metadata);
            return html;
       },

       render: function() {
            return ( 
                   React.DOM.div(null, 
                   React.DOM.div({className: "question-preview-container"}), 
                         React.DOM.hr(null), 
                   React.DOM.div({className: "question-card-template", dangerouslySetInnerHTML: {__html: this.compileTemplate()}})
                   )
             );
        }
});