/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

        render: function() {
            return ( 
                  <tr>
                    <td colSpan={this.props.colSpan}>
                         <span dangerouslySetInnerHTML={{__html: this.props.preview}} />
                    </td>
                  </tr>
            );
        }
});