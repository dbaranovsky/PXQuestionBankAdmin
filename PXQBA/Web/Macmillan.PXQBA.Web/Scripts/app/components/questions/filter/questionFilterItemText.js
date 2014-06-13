/**
* @jsx React.DOM
*/

var QuestionFilterItemText = React.createClass({displayName: 'QuestionFilterItemText',
    
    getInitialState: function() {
        return { value: this.props.currentValues[0] };
    },

    onChangeHandler: function(text) { 
        this.setState({value: text});
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

    render: function() {
        return (
                React.DOM.div(null,  
                   TextEditor( {value:this.state.value,
                               dataChangeHandler:this.onChangeHandler, 
                               classNameProps:"filter-text-input",
                               onKeyPressHandler:this.onKeyPress}

                               )
                )
            );
        }
});