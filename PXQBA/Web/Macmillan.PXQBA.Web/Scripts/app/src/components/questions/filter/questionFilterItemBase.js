/**
* @jsx React.DOM
*/

var QuestionFilterItemBase = React.createClass({
    render: function() {
        return (
            <div className="questionFilterItemBase">
                 <div> 
                     <span> {this.props.descriptor.caption}</span>
                     <QuestionFilterMultiSelect allValues={this.props.descriptor.allValues}  currentValues={this.props.descriptor.currentValues}/>
                </div>
            </div>
            );
        }
});