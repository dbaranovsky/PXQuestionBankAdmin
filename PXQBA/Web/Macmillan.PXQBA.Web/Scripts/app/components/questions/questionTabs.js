/**
* @jsx React.DOM
*/ 

var QuestionTabs = React.createClass({displayName: 'QuestionTabs',

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
            React.DOM.div(null, 
                React.DOM.ul( {className:"nav nav-tabs"}, 
                     React.DOM.li( {className:"active"},  
                         React.DOM.a( {href:"#view", 'data-toggle':"tab"}, "View")
                     ),
                     React.DOM.li(null, 
                         React.DOM.a( {href:"#editOrder", 'data-toggle':"tab"}, "Edit order")
                     )
                ),
 
                React.DOM.div( {className:"tab-content"}, 
                    React.DOM.div( {className:"tab-pane active", id:"view"}, 
                         QuestionGrid(
                             {data:this.props.data,
                             currentPage:this.props.currentPage,
                             totalPages:this.props.totalPages,  
                             order:this.props.order}   
                          )
                    ),
                    React.DOM.div( {className:"tab-pane", id:"editOrder"}, "...")
                )
            )
            );
        }

});