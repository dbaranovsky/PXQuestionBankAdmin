/**
* @jsx React.DOM
*/

var CapabilityItem = React.createClass({

  switchCapability: function(){
    var capability = this.props.capability;
    capability.isActive = !capability.isActive;
    this.props.editCapability(capability);
  },

   render: function(){
      return(
        <div className="capability-item">
           <input type="checkbox" disabled={this.props.viewMode} checked={this.props.capability.isActive} onChange={this.switchCapability}/> <span> {this.props.capability.name}</span>
        </div>
        );
    }
 }); 





