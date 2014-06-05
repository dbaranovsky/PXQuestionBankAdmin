/**
* @jsx React.DOM
*/ 

var Question = React.createClass({displayName: 'Question',

    getInitialState: function() {
        return { showMenu: false, expanded: false};
    },

    mouseOverHandler: function() {
        this.setState({ showMenu: true });
    },

    mouseLeaveHandler: function() {
        this.setState({ showMenu: false });
    },

    getTitleCount: function() {
        var isShared = true;
        var shareWith = this.props.metadata.data[window.consts.questionSharedWithName];
        var titleCount=0;
        if(shareWith==null) {
            isShared = false;
        }
        else {
            isShared =  shareWith != "";
        }
        if(isShared) {
            titleCount = shareWith.split("<br>").length;
        }

        return titleCount;
    },

    renderMenu: function() {
            var questionId = this.props.metadata.data["id"];
            var questionIds = [];
            questionIds.push(questionId);

           var titleCount = this.getTitleCount();
            var isShared = titleCount > 0;
            return QuestionListMenu(
                        {data:this.props.metadata.data, 
                        copyQuestionHandler:this.props.menuHandlers.copyQuestionHandler.bind(null, questionId),
                        editQuestionHandler:this.props.menuHandlers.editQuestionHandler.bind(null, questionId),
                        editNotesHandler:this.props.menuHandlers.editNotesHandler.bind(null, questionId),
                        shareHandler: this.props.menuHandlers.shareHandler.bind(null, questionIds),
                        publishDraftHandler:  this.props.menuHandlers.publishDraftHandler.bind(null, questionId),
                        createDraftHandler: this.props.menuHandlers.createDraftHandler.bind(null, questionId),
                        showAll:  this.state.showMenu, 
                        isShared:  isShared,
                        titleCount:  titleCount,
                        draft:  this.props.draft}
                         )
    },

    selectQuestionHandler: function(event) {
        var quesionId = this.props.metadata.data.id;
        var isSelected = event.target.checked;
        this.props.selectQuestionHandler(quesionId, isSelected, this.props.metadata.data.sharedWith!=="");
    },

    renderCell: function(metadataName, editorDescriptor, allowedEdit) {
        return ( QuestionCell( {value:this.props.metadata.data[metadataName],
                               field:metadataName, 
                               questionId:this.props.metadata.data.id,
                               editorDescriptor:editorDescriptor,
                               allowedEdit:  allowedEdit,
                               expanded:  this.props.expanded,
                               draft:this.props.draft,
                               expandPreviewQuestionHandler:  this.props.expandPreviewQuestionHandler} ));
    },
      
   renderGroupLine: function() {
        if(this.props.grouped) {
          return (React.DOM.td( {className:"grouped-cell"}));
        }

        return (React.DOM.td( {className:"grouped-cell-empty"}));
    },

    render: function() {
        var self = this;
        var componentClass = React.addons.classSet({
                'question': true,
                'question-draft': this.props.draft && !this.state.showMenu,
                'hover': this.state.showMenu,
                'question-selected': this.props.selected,
            });
        
        var cells = this.props.columns.map(function(descriptor) {
                return self.renderCell(descriptor.metadataName, descriptor.editorDescriptor, descriptor.isInlineEditingAllowed)
            });

        return ( 
            React.DOM.tr( {className:componentClass, 
                    onMouseOver:this.mouseOverHandler,
                    onMouseLeave:this.mouseLeaveHandler}, 

                this.renderGroupLine(),
                React.DOM.td(null,  
                    React.DOM.input( {type:"checkbox", checked:this.props.selected, onChange:this.selectQuestionHandler})
                ),
                 cells,
                 React.DOM.td( {className:"actions-cloumn"},   
                   React.DOM.div( {className:"actions-container"}, 
                        this.renderMenu()
                   )
                )  
            ) 
            );
        }
});