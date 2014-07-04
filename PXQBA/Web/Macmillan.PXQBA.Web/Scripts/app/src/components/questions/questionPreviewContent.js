/**
* @jsx React.DOM
*/ 

var QuestionPreviewContent = React.createClass({

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
                   <div>
                   <div className="question-preview-container"></div>
                         <hr />
                   <div className="question-card-template" dangerouslySetInnerHTML={{__html: this.compileTemplate()}} />
                   </div>
             );
        }
});