/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

       renderGroupLine: function() {
        if(this.props.grouped) {
          return (<td className="grouped-cell"></td>);
        }

        return (<td className="grouped-cell-empty"></td>);
       },

       render: function() {
            return ( 
                  <tr className='grid-question-preview'>
                    {this.renderGroupLine()}
                    <td colSpan={this.props.colSpan}>
                      <QuestionPreviewContent 
                         metadata={this.props.metadata} 
                         preview={this.props.preview} 
                        questionCardTemplate={this.props.questionCardTemplate}
                      />
                    </td>
                  </tr>
            );
        }
});


 