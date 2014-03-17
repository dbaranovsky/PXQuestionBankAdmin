/**
* @jsx React.DOM
*/ 

var QuestionTabs = React.createClass({

	tabsInitializer: function (container) {
         container.find('a:first').tab('show')
	},

	componentDidMount: function() {
         this.tabsInitializer($(this.getDOMNode()));
	},

	componentDidUpdate: function () {
		this.tabsInitializer($(this.getDOMNode()));
	},

	render: function() {
		return ( 
			<div>
				<ul className="nav nav-tabs">
		 		 	 <li className="active"> 
		                 <a href="#view" data-toggle="tab">View</a>
				 	 </li>
				  	 <li>
				         <a href="#editOrder" data-toggle="tab">Edit order</a>
				 	 </li>
				</ul>
 
				<div className="tab-content">
				  	<div className="tab-pane active" id="view">
                         <QuestionGrid
                         	 data={this.props.data}
                          	 currentPage={this.props.currentPage}
                         	 totalPages={this.props.totalPages}		
                          />
				  	</div>
				  	<div className="tab-pane" id="editOrder">...</div>
				</div>
			</div>
			);
		}

});