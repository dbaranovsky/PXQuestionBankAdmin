/* 
*********************************************************
NavTree Objects

REQUIRES: jquery-1.3.2.min.js

*********************************************************
*/

// Temporary: auto id for id
var auto_id = 0;
var NavTrees = new Object();

function NavTree_Toggle (tree_id, id) {
	var tree = NavTrees[tree_id];
	tree.ToggleItem(id);
//	NavTree_CtrlWin.NavTree_Site_Resize();
}



// *********************************************************
// NavTree
// *********************************************************
// Constructor
function NavTree (id, containerTypes) {
	var arrTypes = containerTypes.split(',');
	this.containerTypes = new Object();
	for (var t=0; t<arrTypes.length; t++) this.containerTypes[arrTypes[t]] = arrTypes[t];
	this.item_ar = new Object();
	this.item_ct = 0;
	this.root = null;
	this.id = id;
	NavTrees[this.id] = this;
}

NavTree.prototype.CreateRoot = function(
	type,
	expanded,
	myItem
) {
//alert('CreateRoot');
	return this.AddItem(null, null, type, expanded, myItem);
}

NavTree.prototype.AddItem = function(
	parent_id,
	sequence,
	type,
	expanded,
	myItem
) {
//alert('AddItem');
	var item = new NavTree_Item(this, parent_id, sequence, type, expanded, myItem);
//alert('new myItem:'+ item.myItem);
	this.item_ar[item.id] = item;
	this.item_ct += 1;
	return item;
}

NavTree.prototype.ToggleItem = function( id ) {
	var item = this.GetItemById(id);
	item.expanded = ! item.expanded;
	NavTree_CtrlWin.NavTrees_ExpandedItems[this.id+'_'+item.id] = item.expanded;

	var itemDiv = $('#NavTree_'+ this.id +'_item_'+ item.id);
	itemDiv.html( item.GetInnerHTML() );
	NavTree_CtrlWin.NavTree_Site_Resize();
/*
	var itemDiv = document.getElementById('NavTree_'+ this.id +'_item_'+ item.id);
	itemDiv.innerHTML = item.GetInnerHTML();
	NavTree_CtrlWin.NavTree_Site_Resize();
*/
/*
	var itemContentsDiv = document.getElementById('NavTree_'+ this.id +'_item_contents_'+ item.id);
	var html = '';
	if (! item.expanded) {
		for (var ci in item.children) {if (item.children.hasOwnProperty(ci)) {
			html += item.children[ci].GetHTML();
		}}
	}
	itemContentsDiv.innerHTML = html;
	this.SetItemDisplay( itemContentsDiv );
	item.expanded = ! item.expanded;
*/
}

NavTree.prototype.GetItemById = function( id ) {
	return this.item_ar[id];
}

NavTree.prototype.SetItemDisplay = function (l, d) {
	if (l != null) {
		if (d == null) {
			d = (l.style.display == "block") ? "none" : "block";
		}
		l.style.display = d;
	}
	return d;
}

NavTree.prototype.GetItemsByMyItem = function( myItem ) {
	var arr = new Array();
	for (var i in this.item_ar) {if (this.item_ar.hasOwnProperty(i)) {
//alert(this.item_ar[i].myItem == myItem);
		if (this.item_ar[i].myItem == myItem) {
			arr[arr.length] = this.item_ar[i];
		}
	}}
	return arr;
}



// ----------------------------------------------
// Error handling
NavTree.prototype.reportErrors = false;
NavTree.prototype.SetReportErrors = function(reportErrors) {
	NavTree.prototype.reportErrors = reportErrors;
}

NavTree.prototype.ReportError = function(string) {
	if (this.reportErrors) {
		alert('Error with item titled "' + this.title + '": ' + string);
	}
}


// *********************************************************
// NavTree_Item
// *********************************************************
// Constructor
function NavTree_Item(
	NavTree,
	parent_id,
	sequence,
	type,
	expanded,
	myItem
) {
//alert('NavTree_Item: '+ NavTree.id +' -- '+ parent_id +' -- '+ type);
	var pid = null;
	var pitem
	for (var item_i in NavTree.item_ar) {if (NavTree.item_ar.hasOwnProperty(item_i)) {
		if (parent_id == NavTree.item_ar[item_i].id) {
			pid = NavTree.item_ar[item_i].id;
			pitem = NavTree.item_ar[item_i];
		}
	}}
	if ( pid == null && NavTree.item_ct!=0 ) {
alert('NO PARENT');
		return null;
	}

	if ( NavTree.item_ct==0 ) {
		NavTree.root = this;
	} else {
		if (sequence >= pitem.children.length) {
			sequence = pitem.children.length;
		}
		for (var i=pitem.children.length; i>sequence; i--) {
			pitem.children[i-1].sequence += 1;
			pitem.children[i] = pitem.children[i-1];
		}
		pitem.children[sequence] = this;
	}
	this.NavTree = NavTree;
	this.id = auto_id;
	auto_id += 1;
	this.parent_id = parent_id;
	this.sequence = sequence;

	this.type = type;


	this.expanded = (expanded != null) ? expanded : (NavTree_CtrlWin.NavTrees_ExpandedItems[this.NavTree.id+'_'+this.id]) ? NavTree_CtrlWin.NavTrees_ExpandedItems[this.NavTree.id+'_'+this.id] : false;
	NavTree_CtrlWin.NavTrees_ExpandedItems[this.NavTree.id+'_'+this.id] = this.expanded;

	this.myItem = myItem

	this.children = new Array();

	return this;
}

NavTree_Item.prototype.AddItem = function(
	sequence,
	type,
	expanded,
	myItem
) {
//alert('Item AddItem');
	var item = null;
	if (this.IsContainer()) {
		item = this.NavTree.AddItem( this.id, sequence, type, expanded, myItem);
	}
//alert(item.myItem.title);
	return item;
}

// Is this item a root?
NavTree_Item.prototype.IsRoot = function() {
//alert('isroot('+ this.type +'): '+ (this.parent_id == null).toString());
	return (this.parent_id == null);
//	return (this.type == "root");
}

// Is this item a container for other items (i.e. a folder/node)?
NavTree_Item.prototype.IsContainer = function() {
	var yes = false;
	if (this.NavTree.containerTypes[this.type]) yes = true;
	return yes;
}

// -------------------------------------------
// Write functions

NavTree_Item.prototype.GetHTML = function( fnInner, fnNode, fnContent, fnContents ) {
	if ( fnInner ) this.GetInnerHTML = fnInner;
	if ( fnNode ) this.GetNodeHTML = fnNode;
	if ( fnContent ) this.GetContentHTML = fnContent;
	if ( fnContents ) this.GetContentsHTML = fnContents;

	if (this.IsContainer() && this.NavTree.item_ar[this.parent_id].expandAll) {
		this.expandAll = true;
		this.expanded = true;
	}

	var html = "";

	html += '<div class="NavTree_'+ this.NavTree.id +'_item" id="NavTree_'+ this.NavTree.id +'_item_'+ this.id +'">';

	html += this.GetInnerHTML();

	html += '</div>';

	return html;

}

NavTree_Item.prototype.GetInnerHTML = function() {

	var html = "";

	html += this.GetNodeHTML();
	html += this.GetContentHTML();
	html += this.GetContentsHTML();

	return html;

}

NavTree_Item.prototype.GetNodeHTML = function() {
	var html = "";
	if (this.IsContainer()) {
		html += '<a class="NavTree_'+ this.NavTree.id +'_item_toggleLink" href="JavaScript:NavTree_Toggle(\''+ this.NavTree.id +'\', \''+ this.id +'\');">';
		if (this.expanded) {
			html += '-';
		} else {
			html += '+';
		}
		html += '</a> ';
	} else {
		html += '*';
	}
	return html;
}

NavTree_Item.prototype.GetContentHTML = function() {
	var html = "";
	html += 'ITEM('+ this.type +') <br/>&nbsp;&nbsp;&nbsp;&nbsp;id:'+ this.id +' - expanded:'+ this.expanded +'<br/>&nbsp;&nbsp;&nbsp;&nbsp;parent id:'+ this.parent_id +' - sequence:'+ this.sequence +'<br/>&nbsp;&nbsp;&nbsp;&nbsp;child count:'+ this.children.length +'<br/>&nbsp;&nbsp;&nbsp;&nbsp;myItem:'+ this.myItem;
	return html;
}

NavTree_Item.prototype.GetContentsHTML = function() {
	var html = "";
	html += '<div class="NavTree_'+ this.NavTree.id +'_item_contents" id="NavTree_'+ this.NavTree.id +'_item_contents_'+ this.id +'"';
	if (this.IsContainer() && this.expanded) {
		html += ' style="display:block;">';
		for (var ci in this.children) {if (this.children.hasOwnProperty(ci)) {
			html += this.children[ci].GetHTML( this.GetInnerHTML, this.GetNodeHTML, this.GetContentHTML, this.GetContentsHTML );
		}}
		if (this.children.length==0) html += '<span style="color:#cc0000"><i>no items under this container</i></span>'
	} else {
		html += ' style="display:none;">';
	}
	html += '</div>';
	return html;
}







// ----------------------------------------------
// Error handling

// class-level properties
NavTree_Item.prototype.reportErrors = false;

NavTree_Item.prototype.SetReportErrors = function(reportErrors) {
	NavTree_Item.prototype.reportErrors = reportErrors;
}

NavTree_Item.prototype.ReportError = function(string) {
	if (this.reportErrors) {
		alert('Error with item titled "' + this.title + '": ' + string);
	}
}

