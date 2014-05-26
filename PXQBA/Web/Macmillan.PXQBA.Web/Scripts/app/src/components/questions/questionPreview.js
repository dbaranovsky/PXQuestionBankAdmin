/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

      
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
                  <tr>
                    <td colSpan={this.props.colSpan}>
                       <div className={className}></div>
                         <hr />
                         <div className="question-card-template" dangerouslySetInnerHTML={{__html: this.compileTemplate()}} />
                    </td>
                  </tr>
            );
        }
});