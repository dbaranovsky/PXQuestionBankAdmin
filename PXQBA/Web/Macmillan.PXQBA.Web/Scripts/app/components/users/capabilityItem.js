/**
* @jsx React.DOM
*/

var CapabilityItem = React.createClass({displayName: 'CapabilityItem',

  switchCapability: function(){
    var capability = this.props.capability;
    capability.isActive = !capability.isActive;
    this.props.editCapability(capability);
  },

   render: function(){
      return(
        React.DOM.div( {className:"capability-item"}, 
           React.DOM.input( {type:"checkbox", disabled:this.props.viewMode, checked:this.props.capability.isActive, onChange:this.switchCapability}), " ", React.DOM.span(null,  " ", this.props.capability.name)
        )
        );
    }
 }); 





