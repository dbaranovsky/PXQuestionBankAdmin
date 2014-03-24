/**
* @jsx React.DOM
*/ 

var Question = React.createClass({displayName: 'Question',

    getInitialState: function() {
        return { showMenu: false };
    },

    mouseOverHandler: function() {
        this.setState({ showMenu: true });
    },

    mouseLeaveHandler: function() {
        this.setState({ showMenu: false });
    },

    renderMenu: function() {
        if(this.state.showMenu) {
            return QuestionListMenu(null )
        }

        return null;
    },

    renderPreview: function(fiedl) {
        return (
                React.DOM.td( {className:"title"}, 
                    React.DOM.div(null, 
                    React.DOM.span( {className:"glyphicon glyphicon-chevron-right title-expander"}),
                    this.props.metadata.data[fiedl]
                    ),
                       QuestionPreview( {preview:this.props.metadata.data.questionHtmlInlinePreview})
                ));
    },
 
    renderCell: function(field) {
        if(field=='dlap_title') {
            return this.renderPreview(field);
        }
        return (React.DOM.td(null, 
                    this.props.metadata.data[field]
                ));
    },

    render: function() {
        var componentClass = React.addons.classSet({
                'question': true,
                 hover: this.state.showMenu
        });
        
        var renderCell = this.renderCell;

        var cells = this.props.columns.map(function(descriptor)
            {
                return renderCell(descriptor.metadataName)
            });

        return ( 
            React.DOM.tr( {className:componentClass, 
                    onMouseOver:this.mouseOverHandler,
                    onMouseLeave:this.mouseLeaveHandler}, 
                React.DOM.td(null,  
                    React.DOM.input( {type:"checkbox"})
                ),
                 cells,
                 React.DOM.td( {className:"actions"},   
                   React.DOM.div( {className:"actions-container"}, 
                        this.renderMenu()
                   )
                )  
            ) 
            );
        }
});