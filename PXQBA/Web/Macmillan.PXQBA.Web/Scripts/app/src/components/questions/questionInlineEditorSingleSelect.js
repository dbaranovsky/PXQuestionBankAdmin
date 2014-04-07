/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorSingleSelect = React.createClass({

    changeEventHandler: function(event) {
        var value = event.target.getAttribute("data-value");
         this.props.saveVelueHandler(value)
    },

    renderMenuItems: function() {
        var items = [];
        for (var propertyName in this.props.values) {
            items.push(this.renderMenuItem(this.props.values[propertyName], propertyName));
        }
        return items;
    },

    renderMenuItem: function(label, value) {
        return (<li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" data-value={value}>{label}</a></li>);
    },

    render: function() {
        return ( 
            <div>
                <div className="dropdown">
                    <button className="btn dropdown-toggle sr-only" type="button" id="dropdownMenuType" data-toggle="dropdown">
                       Dropdown
                    <span className="caret"></span>
                    </button>
                    <ul className="dropdown-menu menu-show" role="menu" aria-labelledby="dropdownMenuType" onClick={this.changeEventHandler}>
                       {this.renderMenuItems()}
                     </ul>
                 </div>
            </div>
            );
        }
});