/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

       componentDidMount: function(){
            
       },
       compileTemplate: function(){

            var template = Handlebars.compile(this.props.questionCardTemplate);
            var html = template(this.props.metadata);
            return html;
       },
        render: function() {
            return ( 
                  <tr>
                    <td colSpan={this.props.colSpan}>
                         <span dangerouslySetInnerHTML={{__html: this.props.preview}} />
                         <hr />
                         <div className="question-card-template" dangerouslySetInnerHTML={{__html: this.compileTemplate()}} />
                    </td>
                  </tr>
            );
        }
});