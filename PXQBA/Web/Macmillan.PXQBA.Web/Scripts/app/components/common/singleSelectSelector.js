/**
* @jsx React.DOM
*/

var SingleSelectSelector = React.createClass({displayName: 'SingleSelectSelector',

  getInitialState: function(){
      return ({options: this.renderMenuItems(this.props.allOptions)});
  },

  componentWillReceiveProps: function(nextProps) {
      this.setState({options: this.renderMenuItems(nextProps.allOptions)});
  },

  renderMenuItems: function(options) {
    var optionsHtml = [];
    for(var i=0; i<options.length; i++) {
              optionsHtml.push(React.DOM.option( {value:options[i].value}, options[i].text));
    }

    return optionsHtml;
  },


  changeHandler: function(selectedOptions) {
    var items = [];
    $.each(selectedOptions, function(i, option){
        items.push(option.value);
    }); 
    this.props.onChangeHandler(items);
  },

  componentDidUpdate: function() {
    var selector = this.getDOMNode();
    //Chosen complonent isert new options in html, need to remove them 
    $(selector).find('option:not([data-reactid])').remove();
    $(selector).trigger('chosen:updated');
    $(selector).val(this.props.currentValues);
    $(selector).trigger('chosen:updated');
  },

  componentDidMount: function(){
    var self = this;
    var selector = self.getDOMNode();
    var chosenOptions = {width: "100%", hide_dropdown: true, allow_add_new_values: true};
    if (self.props.allowDeselect != undefined && self.props.allowNewValues != undefined){
         chosenOptions = {width: "100%", 
                            hide_dropdown: true, 
                            allow_add_new_values: self.props.allowNewValues, 
                            allow_single_deselect: self.props.allowDeselect,
                            placeholder_text_single: "None"
                          };
    }

    $(selector).val(this.props.currentValues)
                         .chosen(chosenOptions)
                         .change(function(e, params){
                           self.changeHandler(e.currentTarget.selectedOptions);
                         });
    $(selector).trigger('chosen:updated');
  },


    render: function() {
        return (
               React.DOM.select( {'data-placeholder':this.props.dataPlaceholder, disabled:this.props.disabled}, 
                  this.state.options  
              ) 
            );
        }
});