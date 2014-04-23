/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({

        render: function() {
            return ( 
                  <tr>
                    <td colSpan={this.props.colSpan}>
                         <span dangerouslySetInnerHTML={{__html: this.props.preview}} />
                         <hr />
                         <div>I love the smell of napalm in the morning. It smells like victory.</div>
                    </td>
                  </tr>
            );
        }
});