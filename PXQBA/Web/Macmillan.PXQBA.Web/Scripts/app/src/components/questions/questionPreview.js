/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

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
            var template = Handlebars.compile(this.props.questionCardTemplate);
            var html = template(this.props.metadata);
            return html;
       },

       renderGroupLine: function() {
        if(this.props.grouped) {
          return (<td className="grouped-cell"></td>);
        }

        return (<td className="grouped-cell-empty"></td>);
       },

       render: function() {
            return ( 
                  <tr>
                    {this.renderGroupLine()}
                    <td colSpan={this.props.colSpan}>
                       <div className="question-preview-container"></div>
                         <hr />
                         <div className="question-card-template" dangerouslySetInnerHTML={{__html: this.compileTemplate()}} />
                    </td>
                  </tr>
            );
        }
});