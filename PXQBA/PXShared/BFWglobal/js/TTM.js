// Temporary: auto id for item_id
var auto_item_id = 10000;
var TTMs_objs = new Object();

// *******************************************************************
// *******************************************************************
// TTMssss
// *******************************************************************

// Constructor
function TTMs (id) {
	this.item_ar = new Array();
	this.DisplayMode = 'Display';
	this.root = null;
	this.id = id;
	TTMs_objs[this.id] = this;
}

// ----------------------------------------------
// Error handling

TTMs.prototype.reportErrors = false;

TTMs.prototype.SetReportErrors = function(reportErrors) {
	TTM.prototype.reportErrors = reportErrors;
}

TTMs.prototype.ReportError = function(string) {
	if (this.reportErrors) {
		alert( 'Error with site items : ' + string );
	}
}

// ----------------------------------------------
// public methods


TTMs.prototype.SetDisplayMode = function( mode ) {
	this.DisplayMode = mode;
}


TTMs.prototype.AddItem = function (
		item_id,
		parent_id,
		sequence,
		company,
		discipline,
		subdiscipline,
		title,
		parent_ISBN,
		ISBN,
		product_title,
		product_category,
		product_status,
		product_URL,
		product_bsi_site_id
	) {
//alert('add:'+ item_type +', '+ item_subtype);	
	var item = new TTM(this, item_id, parent_id, sequence, company, discipline, subdiscipline, title, parent_ISBN, ISBN, product_title, product_category, product_status, product_URL, product_bsi_site_id);

	this.item_ar[this.item_ar.length] = item;
	return this.item_ar[this.item_ar.length-1];
}

TTMs.prototype.GetItemsByDiscipline = function ( discipline, subdiscipline ) {
//alert('TTMs.prototype.GetItemsByDiscipline');
	var x = new Array();
	for (var i=0;i<this.item_ar.length;i++) {
		if (this.item_ar[i].discipline==discipline && (this.item_ar[i].subdiscipline==subdiscipline || subdiscipline==null) ) {
			x[x.length] = this.item_ar[i];
		}
	}

	if (x.length==0) this.ReportError( 'no items found' );
	return x;
}



// *******************************************************************
// *******************************************************************
// TTM
// *******************************************************************


// Constructor

function TTM (
		TTMs,
		item_id,
		parent_id,
		sequence,
		company,
		discipline,
		subdiscipline,
		title,
		parent_ISBN,
		ISBN,
		product_title,
		product_category,
		product_status,
		product_URL,
		product_bsi_site_id
	) {

	var pid = null;
	var pitem
	for (var i=0;i<TTMs.item_ar.length;i++) {
		if (parent_id == TTMs.item_ar[i].item_id) {
			pid = TTMs.item_ar[i].item_id;
			pitem = TTMs.item_ar[i];
		}
	}

	if ( pid == null && TTMs.item_ar.length != 0 ) {
alert('new:'+ company +', '+ discipline +', '+ subdiscipline +', '+ title +', '+ product_title);	
		TTMs.ReportError( 'parent not found.' );
		return null;
	} 
	if (TTMs.item_ar.length==0) {
		TTMs.root = this;
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
	this.TTMs = TTMs;

	if (item_id=='' || item_id==null) {
		item_id = auto_item_id;
		auto_item_id += 1;
	}
	this.item_id = item_id;
	this.parent_id = parent_id;
	this.sequence = sequence;
	this.company = company;
	this.discipline = discipline;
	this.subdiscipline = subdiscipline;
	this.title = title;
	this.product_title = product_title;
	this.parent_ISBN = parent_ISBN;
	this.ISBN = ISBN;
	this.product_category = product_category;
	this.product_status = product_status;
	this.product_URL = product_URL;
	this.product_bsi_site_id = product_bsi_site_id;

	this.children = new Array();
	this.all_children_loaded = false;
}


