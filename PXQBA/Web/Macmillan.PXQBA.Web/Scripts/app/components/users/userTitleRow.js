/**
* @jsx React.DOM
*/

 var UserTitleRow  = React.createClass({displayName: 'UserTitleRow',
    selectorChangeHandler: function(items){
        var title = this.props.title;

        if(items[0] == ""){
          if(title.currentRole != null){
            title.currentRole = null;
            title.isChanged = true;
            this.props.changeTitleHandler(title);
            return;
          }
        }

        if(title.currentRole == null){
          title.currentRole= { id: "0"};
        }
       
       if(items[0] != title.currentRole.id){
            title.currentRole.id = items[0];
            title.isChanged = true;
            this.props.changeTitleHandler(title);
       }
    },

    getAllOptions: function(availableChoice) {
        if(availableChoice==null) {
             availableChoice =[];
        }
        var options = [];
        options.push({value: "", text:""});

        $.each(availableChoice, function(i, el){
           options.push({ value: el.id,
                          text: el.name
           });
         });

        return options;
   },

      render: function(){

          var userTitle = this.props.title;
          var self = this;
           var currentValue = userTitle.currentRole == null? "" : userTitle.currentRole.id;
            return (React.DOM.div( {className:"role-row"}, 
                          React.DOM.div( {className:"role-cell"}, userTitle.name),
                            React.DOM.div( {className:"role-cell selector"}, 
                                    SingleSelectSelector( {disabled:!userTitle.canSetRoles,  allowNewValues:false, currentValues:currentValue,  allowDeselect:true, allOptions:self.getAllOptions(userTitle.availableRoles), onChangeHandler:self.selectorChangeHandler})
                            )
                         ));
      }


  });

