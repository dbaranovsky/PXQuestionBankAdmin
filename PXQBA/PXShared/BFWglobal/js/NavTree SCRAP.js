
// If item has a parent, add item as a child of the parent
NavTree_Item.prototype.AttachToParent = function(root) {	
	if (this.parent_id != null) {
		var parent = root.GetItemFromId(this.parent_id);
		if (parent == null) {
			this.ReportError('Couldn\'t get parent item in AttachToParent; parent_id = ' + this.parent_id);
			return;
		}
		parent.AddChild(this);
	}
}

// -----------------------------------------------
// Functions for roots only

// Add an item to the current tree
NavTree_Item.prototype.xAddItem = function(item) {
	// You can only add items to roots
	if (this.type != 'root') {
		this.ReportError('Tried to add an item to a non-root item');
		return;
	}
	
	// Add to the id_array
	this.id_array[item.bfw_uid] = item;
	
	// Quick link up to the root_node
	item.root_node = this;
	
    // @Shubhranshu - We assume we are always ready. Adding the data to tree at runtime 
	// Attach to parent if we're ready to do so
	if (this.toc_initially_built) {
		item.AttachToParent(this);
	}
}

// Function to establish all the children arrays.
// Have to call this once everything's done, in case children are loaded
// before parents
NavTree_Item.prototype.CreateTree = function() {
	// You can only create trees for roots
	if (this.type != 'root') {
		this.ReportError('Tried to create the tree structure of a non-root item');
		return;
	}
	
	// Also, we don't want to run this function more than once
	// (although we could just check to make sure we only add everything once
	
    if (this.toc_initially_built) {
		this.ReportError('Tried to call CreateTree twice');
		return;
	}

    // Clear the array so that we don't have duplicate Entries
	cleanUpRootNode(this);
        
	// Go through every item in the bsi_by_id array
	for (bfw_uid in this.id_array) {
		var item = this.id_array[bfw_uid];

        //item.
		// Add each item to its parent
		item.AttachToParent(this);
	}
	
	// We only want to run this function once
	//this.toc_initially_built = true;
}

// Retreive an item via its id
NavTree_Item.prototype.GetItemFromId = function(bfw_uid) {
	// If this isn't the root item, get the root
	var root;
	if (this.type == 'root') {
		root = this;
	} else {
		root = this.root_node;
	}
	
	return root.id_array[bfw_uid];
}

// -------------------------------------------
// Container functions

// Is the container hidden -- i.e., not available to end users in the navigation tree?
NavTree_Item.prototype.IsHidden = function() {
	return (this.hidden == 1);
}

// Add a child to a container
NavTree_Item.prototype.AddChild = function(item) {
	this.children[this.children.length] = item;
	
	// If the item had null set for sequence, add the sequence here
	if (item.sequence == null) {
		item.sequence = (this.children.length - 1);
	}
	
	// If the new child's sequence is < the next-to-last child's sequence,
	// then we need to sort
	if (this.children.length > 1 && item.sequence < this.children[this.children.length - 2].sequence) {
		this.children.sort(NavTree_Item_Child_Sort);
	}
}


//@Shubhranshu
function cleanUpRootNode(_root){
	if(_root.children){
		for(var j = 0 ; j < _root.children.length ; j++)
			cleanUpRootNode(_root.children[j]);
	}
	_root.children = new Array();
/*    for(var j = 0 ; j < _root.children.length ; j++){
        for(var k = 0 ; k < _root.children[j].children.length ; k++){
            _root.children[j].children[k].children = new Array();
        }
        _root.children[j].children = new Array();
    }

    _root.children = new Array();
	*/
}

function NavTree_Item_Child_Sort(a, b) {
	return a.sequence - b.sequence;
}

// Return the number of children for an item
NavTree_Item.prototype.ChildCount = function() {
	if (this.children == null) {
		return 0;
	} else {
		return this.children.length;
	}
}

// Delete an item
NavTree_Item.prototype.Delete = function() {
	// Take the item out of its parent.children array
	var parent_item = this.GetItemFromId(this.parent_id);
	
	var swap_to = 0;
	var found_it = false;
	for (var i = 0; i < parent_item.children.length; ++i) {
		if (parent_item.children[i] == this) {
			++swap_to;
			found_it = true;
		}
		parent_item.children[i] = parent_item.children[swap_to];
		
		++swap_to;
	}
	
	if (found_it) {
		parent_item.children.length -= 1;
	}
	
	// Need to eliminate shortcuts to item and its decendents...
}


NavTree_Item.prototype.ItemLink = function(editMode) {
	var html = "";
	
	html += "<div class='bsi_item' id='bsi_item_title_" + this.id + "'>";
	
	// Include output from a custom function if defined
	if (window.ItemLinkPrefix != null) {
		html += ItemLinkPrefix(this);
	}
	
	//@ Shubhranshu
	// Handle the PV rendition
	// New method for fetching content
//	if( (this.link_url != null) && (this.link_url.search(".xml") == (this.link_url.length - 4) ) ){
//		html += "<span class='bsi_item_icon'>" + ItemIcon(this) + "</span><a target='_blank' style='cursor:pointer' onClick='javascript:window.open(\"getPVRendition.html?link_url="+ this.link_url.replace("'","&#39;") +"&htmpv_id="+ this.id +"\")' class='bsi_item_link'>";
//	}else{
//		html += "<span class='bsi_item_icon'>" + ItemIcon(this) + "</span><a target='_blank' style='cursor:pointer' onClick='javascript:window.open(\""+ this.link_url.replace("'","&#39;") +"\")' class='bsi_item_link'>";
		html += "<span class='bsi_item_icon'>" + ItemIcon(this) + "</span><a target='_blank' style='cursor:pointer' onClick='javascript:window.open(\"getContent.html?bsi_id=" + this.id + "\")' class='bsi_item_link'>";
//	}
	
	// Add a style if this is an instructor-only item
	if (this.access_level >= 40) {
		html += "<span class='bsi_instructor_only'>" + this.title + "</span>";
	} else {			
		html += this.title;
	}
	
	html += "</a>";

	if (this.subtitle != "") {
		html += "<div class='bsi_item_subtitle'>" + this.subtitle + "</div>";
	}

	if (editMode) {
		html += this.EditLink();
	}
	
	html += "</div>";

	return html;
}

NavTree_Item.prototype.EditLink = function() {
	var html = " <a href='javascript:EditItem(\"" + this.id + "\")' class='bsi_edit_link' id='bsi_edit_link_" + this.id + "'>[Edit]</a>";
	
	return html;
}

