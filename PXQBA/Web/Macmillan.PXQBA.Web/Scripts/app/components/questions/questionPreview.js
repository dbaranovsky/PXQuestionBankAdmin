/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',

       componentDidMount: function(){
            
       },
       compileTemplate: function(){

            var template = Handlebars.compile(this.props.questionCardTemplate);
        var html = template(this.props.metadata);
            return html;
       },
        render: function() {
            return ( 
                  React.DOM.tr(null, 
                    React.DOM.td( {colSpan:this.props.colSpan}, 
                         React.DOM.span( {dangerouslySetInnerHTML:{__html: this.props.preview}} ),
                         React.DOM.hr(null ),
                         React.DOM.div( {className:"question-card-template", dangerouslySetInnerHTML:{__html: this.compileTemplate()}} )
                    )
                  )
            );
        }
});