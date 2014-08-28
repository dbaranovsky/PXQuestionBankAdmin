/**
* @jsx React.DOM
*/

var QuestionFilterItemText = React.createClass({
    
    getInitialState: function() {
        return { value: this.getTextFromCurrentValues(this.props.currentValues) };
    },

    onChangeHandler: function(text) { 
        var encodedValue =  encodeURIComponent(text);
        this.setState({value: encodedValue});
    },

    componentWillReceiveProps: function(nextProps) {
      this.setState({value: this.getTextFromCurrentValues(nextProps.currentValues)});
    },

    getTextFromCurrentValues: function(currentValues) {
      var text = "";
      if(currentValues[0]!=null) {
        text=currentValues[0];
      }
      return text;
    },

    onBlurHandler: function() {
      this.setState({value: this.getTextFromCurrentValues(this.props.currentValues)});
    },

    onKeyPress: function(event) {
      if(event.key=="Enter") {
        this.props.onChangeHandler([this.state.value]);
      }
    },

    onCancelEventHandler: function() {
      this.props.onChangeHandler([""]);  
    },

    render: function() {
        return (
                <div> 
                    <table className="filter-text-table">
                      <tr>
                        <td>
                         <TextEditor value={decodeURIComponent(this.state.value)}
                               dataChangeHandler={this.onChangeHandler} 
                               classNameProps="filter-text-input"
                               onKeyPressHandler={this.onKeyPress}
                               />
                        </td>
                        <td>
                         <button type="button" className="btn btn-default btn-sm" onClick={this.onCancelEventHandler} data-toggle="tooltip" title="Cancel">
                                  <span className="icon-cancel"></span>
                          </button>
                        </td>
                      </tr>
                    </table>
                     
                          
                </div>
            );
        }
});