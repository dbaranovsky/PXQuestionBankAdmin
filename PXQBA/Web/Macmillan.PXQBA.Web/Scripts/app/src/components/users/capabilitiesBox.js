/**
* @jsx React.DOM
*/

var CapabilitiesBox = React.createClass({


    editCapabilityGroup: function(capabilityGroup){
      var role = this.props.role;
      var capabilityGroups = this.props.role.capabilityGroups;
      var newCapablilityGroups = [];
      $.each(capabilityGroups, function(index, value){
          if (value.name == capabilityGroup.name){
            newCapablilityGroups.push(capabilityGroup);
          }else{
            newCapablilityGroups.push(value);
          }
      });

       role.capabilityGroups = newCapablilityGroups;
      this.props.editRoleHandler(role);
    },

    renderCapabilities: function(){
        var capabilityGroups = [];
        var self = this;
        capabilityGroups = this.props.role.capabilityGroups.map(function (capabilityGroup, i) {
            return (<CapabilityGroup capabilityGroup={capabilityGroup} editCapabilityGroup={self.editCapabilityGroup} viewMode={self.props.viewMode}/>);
          });

        return capabilityGroups;
    },

    render: function(){
    return(
      <div className="capabilities-box">
          {this.renderCapabilities()}
      </div>
      );
  }
});



