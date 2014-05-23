/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

       componentDidUpdate: function(){
          //  $(this.getDOMNode()).find(".question-preview").html(this.props.preview);
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
                      //   <span className="question-preview" dangerouslySetInnerHTML={{__html: this.props.preview}} />
                       <span className="question-preview"></span>
                         <hr />
                         <div className="question-card-template" dangerouslySetInnerHTML={{__html: this.compileTemplate()}} />
                    </td>
                  </tr>
            );
        }
});