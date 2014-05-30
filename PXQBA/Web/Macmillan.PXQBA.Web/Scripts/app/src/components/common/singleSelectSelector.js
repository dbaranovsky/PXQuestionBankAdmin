/**
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
  },

  componentDidMount: function(){
    var self = this;
    var selector = self.getDOMNode();
    var chosenOptions = {width: "100%", hide_dropdown: true, allow_add_new_values: true};
    $(selector).val(this.props.currentValues)
                         .chosen(chosenOptions)
                         .change(function(e, params){
                           self.changeHandler(e.currentTarget.selectedOptions);
                         });
  },


    render: function() {
        return (
               <select data-placeholder={this.props.dataPlaceholder}>
                  {this.state.options}  
              </select> 
            );
        }
});