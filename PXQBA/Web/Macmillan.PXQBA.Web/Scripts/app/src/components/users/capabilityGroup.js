/**
* @jsx React.DOM
*/

var CapabilityGroup = React.createClass({


    editCapability: function(capability){
        var capabilityGroup = this.props.capabilityGroup;
        var capabilities = capabilityGroup.capabilities;
        var newCapablilies = [];

          $.each(capabilities, function(index, value){
          if (value.id == capability.id){
            newCapablilies.push(capability);
          }else{
            newCapablilies.push(value);
          }
         });  


        capabilityGroup.capabilities = newCapablilies;
        this.props.editCapabilityGroup(capabilityGroup);

    },

    renderCapabilities: function(){
        var capabilities = [];
        var self = this;
          capabilities = this.props.capabilityGroup.capabilities.map(function (capability, i) {
            return (<CapabilityItem capability={capability} editCapability={self.editCapability} viewMode={self.props.viewMode}/>);
          });

        return capabilities;

    },   


    isGroupSelected: function(){
      var isActiveInGroup = $.grep(this.props.capabilityGroup.capabilities, function(el){return el.isActive;}).length;
      var capabilitiesCount = this.props.capabilityGroup.capabilities.length;

      return isActiveInGroup == capabilitiesCount;
    },

    switchGroup: function(){
        var capabilityGroup = this.props.capabilityGroup;
        var capabilities = capabilityGroup.capabilities;
        var isGroupSelected = !this.isGroupSelected();
        var newCapablilies = [];

          $.each(capabilities, function(index, value){
            value.isActive = isGroupSelected;
            newCapablilies.push(value);    
         });  

        capabilityGroup.capabilities = newCapablilies;
        this.props.editCapabilityGroup(capabilityGroup);
    },

    render: function(){
     
      return(
        <div className="capabilities-group">
           <input type="checkbox"  disabled={this.props.viewMode} checked={this.isGroupSelected()} onChange={this.switchGroup} /> <b> {this.props.capabilityGroup.name}</b>
           {this.renderCapabilities()}
        </div>
        );
    }

 }); 



