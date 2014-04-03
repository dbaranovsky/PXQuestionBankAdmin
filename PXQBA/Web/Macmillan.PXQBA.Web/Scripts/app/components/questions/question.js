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
            var questionId = this.props.metadata.data["id"];
            return QuestionListMenu( {questionId:questionId, renderNotes:this.props.renderNotes.bind(null, questionId)})
        }

        return null;
    },

    renderCell: function(metadataName, editorType) {
        // Hardcoded: only title has a preview
        // Refactor this after re-implement preview
        var previewInfo = { hasPriview: false };

        if(metadataName=='dlap_title') {
            previewInfo.hasPriview = true;
            previewInfo.classNameForCell = "title";
            previewInfo.htmlPreview = this.props.metadata.data.questionHtmlInlinePreview;
        }
        return ( QuestionCell( {value:this.props.metadata.data[metadataName],
                               field:metadataName, 
                               questionId:this.props.metadata.data.id,
                               editorType:editorType,
                               previewInfo:previewInfo}
                                ));
    },

    render: function() {
        var self = this;

        var componentClass = React.addons.classSet({
                'question': true,
                 hover: this.state.showMenu
            });
        
        var cells = this.props.columns.map(function(descriptor) {
                return self.renderCell(descriptor.metadataName, descriptor.editorType)
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