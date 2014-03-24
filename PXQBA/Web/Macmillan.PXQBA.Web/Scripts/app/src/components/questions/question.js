/**
* @jsx React.DOM
*/ 

var Question = React.createClass({

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
            return <QuestionListMenu />
        }

        return null;
    },

    render: function() {
        var componentClass = React.addons.classSet({
                'question': true,
                 hover: this.state.showMenu
        });

        return ( 
            <tr className={componentClass} 
                    onMouseOver={this.mouseOverHandler}
                    onMouseLeave={this.mouseLeaveHandler}>
                <td> 
                    <input type="checkbox"/>
                </td>

                <td className="eBookChapter">
                    {this.props.metadata.eBookChapter}
                </td>

                <td className="questionBank">
                    {this.props.metadata.questionBank}
                </td>

                <td className="questionSeq">
                    {this.props.metadata.questionSeq}
                </td>

                <td className="title">
                    <div>
                    <span className="glyphicon glyphicon-chevron-right title-expander"></span>
                    {this.props.metadata.title}
                    </div>
                       <QuestionPreview preview={this.props.metadata.questionHtmlInlinePreview}/>
                </td>

                <td className="questionType">
                    {this.props.metadata.questionType}
                </td>

                <td className="actions">  
                   <div className="actions-container">
                        {this.renderMenu()}
                   </div>
                </td>  
            </tr> 
            );
        }
});