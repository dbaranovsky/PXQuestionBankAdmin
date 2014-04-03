/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorStatus = React.createClass({

    chengeStatusEventHandler: function(event) {
        var value = event.target.getAttribute("data-value");
        if(value != null) {
            questionDataManager.saveQuestionData(this.props.metadata.questionId,
                                                 this.props.metadata.field,
                                                 value);
 
        }
        this.props.afterEditingHandler();
    },

    renderMenuItems: function() {
        var items = [];
        for (var propertyName in this.props.statusEnum) {
            items.push(this.renderMenuItem(this.props.statusEnum[propertyName]));
        }
        return items;
    },

    renderMenuItem: function(value) {
        return (<li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" data-value={value}>{value}</a></li>);
    },

    render: function() {
        return ( 
            <div>
                <div className="dropdown">
                    <button className="btn dropdown-toggle sr-only" type="button" id="dropdownMenuType" data-toggle="dropdown">
                       Dropdown
                    <span className="caret"></span>
                    </button>
                    <ul className="dropdown-menu menu-show" role="menu" aria-labelledby="dropdownMenuType" onClick={this.chengeStatusEventHandler}>
                       {this.renderMenuItems()}
                     </ul>
                 </div>
            </div>
            );
        }
});