/**
* @jsx React.DOM
*/

var QuestionFilterMultiSelect = React.createClass({displayName: 'QuestionFilterMultiSelect',

	renderMenuItems: function() {
		 var options = [];
		 for(var i=0; i<this.props.allValues.length; i++) {
               options.push(React.DOM.option( {value:this.props.allValues[i]}, this.props.allValues[i]));
		 }
         return options;
	},


	changeHandler: function() {
		 
	},

	componentDidUpdate: function() {
		var selector = this.getDOMNode();
		$(selector).trigger('chosen:updated');
		$(selector).val(this.props.currentValues);
	},

    componentDidMount: function(){
        var self = this;
        var selector = self.getDOMNode();
        var chosenOptions = {width: "100%", hide_dropdown: true};
        $(selector).val(this.props.currentValues)
                          .chosen(chosenOptions)
                          .change(function(e, params){
                             self.changeHandler(e.currentTarget.selectedOptions);
                           });
    },


    render: function() {
        return (
               React.DOM.select( {'data-placeholder':"No Filtration", multiple:true}, 
                  this.renderMenuItems()  
          	  ) 
            );
        }
});