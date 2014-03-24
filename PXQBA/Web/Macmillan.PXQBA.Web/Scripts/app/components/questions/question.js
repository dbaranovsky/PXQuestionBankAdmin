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

    render: function() {
        var componentClass = React.addons.classSet({
                'question': true,
                 hover: this.state.showMenu
        });

        return ( 
            React.DOM.tr( {className:componentClass, 
                    onMouseOver:this.mouseOverHandler,
                    onMouseLeave:this.mouseLeaveHandler}, 
                React.DOM.td(null,  
                    React.DOM.input( {type:"checkbox"})
                ),

                React.DOM.td( {className:"eBookChapter"}, 
                    this.props.metadata.eBookChapter
                ),

                React.DOM.td( {className:"questionBank"}, 
                    this.props.metadata.questionBank
                ),

                React.DOM.td( {className:"questionSeq"}, 
                    this.props.metadata.questionSeq
                ),

                React.DOM.td( {className:"title"}, 
                    React.DOM.div(null, 
                    React.DOM.span( {className:"glyphicon glyphicon-chevron-right title-expander"}),
                    this.props.metadata.title
                    ),
                       QuestionPreview( {preview:this.props.metadata.questionHtmlInlinePreview})
                ),

                React.DOM.td( {className:"questionType"}, 
                    this.props.metadata.questionType
                ),

                React.DOM.td( {className:"actions"},   
                   React.DOM.div( {className:"actions-container"}, 
                        this.renderMenu()
                   )
                )  
            ) 
            );
        }
});