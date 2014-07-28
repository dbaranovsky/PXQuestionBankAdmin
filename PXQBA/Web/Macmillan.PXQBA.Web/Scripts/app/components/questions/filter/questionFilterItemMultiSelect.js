/**
* @jsx React.DOM
*/

var QuestionFilterItemMultiSelect = React.createClass({displayName: 'QuestionFilterItemMultiSelect',

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
     this.props.onChangeHandler(selectedOptions);
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
                             var values = $(selector).chosen().val();
                             self.changeHandler(values);
                         });
  },


    render: function() {
        return (
               React.DOM.select( {'data-placeholder':"No Filtration", multiple:true}, 
                  this.state.options  
          	  ) 
            );
        }
});