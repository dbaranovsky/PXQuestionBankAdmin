/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorText = React.createClass({

  onAcceptEventHandler: function() {
        var value = this.state.value;
        this.props.saveVelueHandler(value)
  },

  onCancelEventHandler: function() {
      this.props.afterEditingHandler();
  },

  getInitialState: function() {
      return {value: this.props.metadata.currentValue};
  },

  handleChange: function(event) {
      this.setState({value: event.target.value});
  },

  render: function() {
        var value = this.state.value;
        return ( 
                <div className="text-editor-container"> 
                          <div className="input-group" rel="tooltip" data-toggle="popover">
                              <input type="text" value={value} onChange={this.handleChange} className="form-control" />
                              <span className="input-group-btn">
                                <button className="btn btn-default" type="button" onClick={this.onAcceptEventHandler}>Accept</button>
                                <button className="btn btn-default" type="button"onClick={this.onCancelEventHandler}>Cancel</button>
                             </span>
                           </div>
                </div>
            );
     }

});