/**
* @jsx React.DOM
*/ 

var QuestionTabs = React.createClass({

      render: function() {
        return ( 
            			<div> 
            			<div className="product-title"> {this.props.response.productTitle}</div>
                         <QuestionGrid response={this.props.response} handlers={this.props.handlers}/>
              			</div>
            );
        }

});