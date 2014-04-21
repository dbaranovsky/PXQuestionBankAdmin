/**
* @jsx React.DOM
*/

var QuestionFilterMultiSelect = React.createClass({

  getInitialState: function(){
         return ({options: this.renderMenuItems(this.props.allValues)});
  },

  componentWillReceiveProps: function(nextProps) {
     this.setState({options: this.renderMenuItems(nextProps.allValues)});
  },

	renderMenuItems: function(values) {
		 var options = [];
		 for(var i=0; i<values.length; i++) {
               options.push(<option value={values[i]}>{values[i]}</option>);
		 }
      return options;
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
               <select data-placeholder="No Filtration" multiple>
                  {this.state.options}  
          	  </select> 
            );
        }
});