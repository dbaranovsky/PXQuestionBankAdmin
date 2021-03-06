﻿/**
* @jsx React.DOM
*/

var SingleSelectSelector = React.createClass({

  getInitialState: function(){
      return ({options: this.renderMenuItems(this.props.allOptions)});
  },

  componentWillReceiveProps: function(nextProps) {
      this.setState({options: this.renderMenuItems(nextProps.allOptions)});
  },

  renderMenuItems: function(options) {
    var optionsHtml = [];
    for(var i=0; i<options.length; i++) {
              optionsHtml.push(<option value={options[i].value}>{options[i].text}</option>);
    }

    return optionsHtml;
  },


  changeHandler: function(selectedOptions) {
      this.props.onChangeHandler(selectedOptions);
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
                            if(params != undefined){
                              self.changeHandler([params.selected]);
                            }else{
                                 self.changeHandler([""]);
                            }

                         });
    $(selector).trigger('chosen:updated');
  },


    render: function() {
        return (
               <select data-placeholder={this.props.dataPlaceholder} disabled={this.props.disabled}>
                  {this.state.options}  
              </select> 
            );
        }
});