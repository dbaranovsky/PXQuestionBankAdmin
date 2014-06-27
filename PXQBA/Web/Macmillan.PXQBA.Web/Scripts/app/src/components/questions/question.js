/**
* @jsx React.DOM
*/ 

var Question = React.createClass({

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

    isShared: function(){
            var titleCount = this.getTitleCount();
            return titleCount > 0;
    },

    renderMenu: function() {
            var questionId = this.props.metadata.data["id"];
            var questionIds = [];
            questionIds.push(questionId);

       
            return <QuestionListMenu
                        data={this.props.metadata.data} 
                        copyQuestionHandler={this.props.menuHandlers.copyQuestionHandler.bind(null, questionId)}
                        editQuestionHandler={this.props.menuHandlers.editQuestionHandler.bind(null, questionId)}
                        editNotesHandler={this.props.menuHandlers.editNotesHandler.bind(null, questionId)}
                        shareHandler ={this.props.menuHandlers.shareHandler.bind(null, questionIds)}
                        publishDraftHandler = {this.props.menuHandlers.publishDraftHandler.bind(null, questionId)}
                        createDraftHandler ={this.props.menuHandlers.createDraftHandler.bind(null, questionId)}
                        showNotification = {this.props.menuHandlers.showNotification}
                        showAll = {this.state.showMenu} 
                        isShared = {this.isShared()}
                        titleCount = {this.getTitleCount()}
                        draft = {this.props.draft}
                        capabilities = {this.props.capabilities}
                        metadataCapabilities= {{canCreateDraftFromAvailableQuestion: this.props.metadata.canCreateDraftFromAvailableQuestion,
                                                canChangeDraftStatus:  this.props.metadata.canChangeDraftStatus,
                                                canEditQuestion:  this.props.metadata.canEditQuestion}} />
                        
    },

    selectQuestionHandler: function(event) {
        var quesionId = this.props.metadata.data.id;
        var isSelected = event.target.checked;
        this.props.selectQuestionHandler(quesionId, isSelected, this.props.metadata.data.sharedWith!=="");
    },

 

    renderCell: function(metadataName, editorDescriptor, allowedEdit, canUpdateSharedValue) {
        return ( <QuestionCell value={this.props.metadata.data[metadataName]}
                               field={metadataName} 
                               questionId={this.props.metadata.data.id}
                               status ={this.props.metadata.data[window.consts.questionStatusName]}
                               editorDescriptor={editorDescriptor}
                               allowedEdit = {allowedEdit}
                               expanded = {this.props.expanded}
                               draft={this.props.draft}
                               expandPreviewQuestionHandler = {this.props.expandPreviewQuestionHandler}
                               canChangeDraftStatus = {this.props.metadata.canChangeDraftStatus}
                               capabilities = {this.props.capabilities}
                               canUpdateSharedValue = {canUpdateSharedValue}
                               isShared = {this.isShared()}
                                />);
    },
      
   renderGroupLine: function() {
        if(this.props.grouped) {
          return (<td className="grouped-cell"></td>);
        }

        return (<td className="grouped-cell-empty"></td>);
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
                var isAllowedInlineEditing = false;
                if(descriptor.metadataName==window.consts.questionStatusName) {
                    isAllowedInlineEditing = descriptor.isInlineEditingAllowed;
                }
                else {
                    isAllowedInlineEditing = self.props.metadata.canEditQuestion && descriptor.isInlineEditingAllowed;
                }
                return self.renderCell(descriptor.metadataName, descriptor.editorDescriptor, isAllowedInlineEditing, descriptor.canUpdateSharedValue)
            });

        return ( 
            <tr className={componentClass} 
                    onMouseOver={this.mouseOverHandler}
                    onMouseLeave={this.mouseLeaveHandler}>

                {this.renderGroupLine()}
                <td> 
                    <input type="checkbox" checked={this.props.selected} onChange={this.selectQuestionHandler}/>
                </td>
                 {cells}
                 <td className="actions-cloumn">  
                   <div className="actions-container">
                        {this.renderMenu()}
                   </div>
                </td>  
            </tr> 
            );
        }
});