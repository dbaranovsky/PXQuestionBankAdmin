/**
* @jsx React.DOM
*/
var QuestionEditorTabs = React.createClass({displayName: 'QuestionEditorTabs',

    tabsInitializer: function (container) {
       //  container.find('a:first').tab('show')
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
                                 React.DOM.a( {href:"#body", 'data-toggle':"tab"}, "Body")
                             ),
                             React.DOM.li(null, 
                                 React.DOM.a( {href:"#metadata", 'data-toggle':"tab"}, "Metadata")
                             ),
                              React.DOM.li(null, 
                                 React.DOM.a( {href:"#history", 'data-toggle':"tab"}, "History")
                             )
                        ),
             

                  
             
                React.DOM.div( {className:"tab-content"}, 
                    React.DOM.div( {className:"tab-pane active", id:"body"}, 
                       React.DOM.div( {className:"tab-body"}, 
                       React.DOM.td( {dangerouslySetInnerHTML:{__html: this.props.question.preview}} )
                       )
                    ),
                    React.DOM.div( {className:"tab-pane", id:"metadata"}, 
                       QuestionMetadataEditor(  {question:this.props.question, editHandler:this.props.editHandler} ),
                           React.DOM.br(null )

                    )
                ),
                React.DOM.div( {className:"tab-pane", id:"history"}, 
                       React.DOM.div( {className:"tab-body"}
                       
                       )
                )

            )
            );
        }

});