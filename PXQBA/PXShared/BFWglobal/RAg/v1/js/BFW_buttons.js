function ButtonHTML(text, url, id, typ, block, size, extra) {
	var html, typStyle;
	
	if (size == "" || size == null) {
		size = "medium";
	}
	
	if (typ == "link" || typ == "primary" || typ == "settingBlue") {
		typStyle = "link";
	} else {
		typStyle = "setting";
	}
	
	html = "<a class='button_" + size + " button_" + size + "_" + typStyle + "'";
	// html = "<a class='squarebutton'";

	if (extra != "" && extra != null) {
		html += " " + extra;	// ' i.e. " style='float:right'"
	}
	
	if (id != "" && id != null) {	
		html += " id='" + id + "'";
	}
	
	// Note that for javascript urls, *double* quotes need to be escaped
	// We do this because in vb script, it's easier to code single quotes without escaping
	html += ' href="' + url + '"><span>' + text;
	
	if (typ == "setting" || typ == "settingBlue") {
		html += "<img class='starr' src='/BFW/pics/launch-arrow.gif' alt='' />";
	}
	
	html += "</span></a>";
	
	if (block == true) {
		html = "<div class='buttonwrapper'>" + html + "</div><div style='clear: both;'></div>";
	}
	
	return html;
}

function ChangeButtonLabel(id, newLabel) {
	var d = document.getElementById(id);
	if (d != null && d.firstChild != null) {
		d.firstChild.innerHTML = newLabel;
	}
}

var curClickedButton = null;
function SettingButtonClicked(id) {
	// De-highlight old button if necessary
	ResetSettingButton();
	
	// alert('setting ' + id);

	curClickedButton = document.getElementById(id);
	
	// Old class must be gray, but we don't know if it's big, medium, or small
	curClickedButton.className = curClickedButton.className + "_on";
	
	// Return a handle to the button
	return curClickedButton;
}

function ResetSettingButton() {
	// alert('resetting ' + curClickedButton);
	if (curClickedButton == null) {
		return;
	}
	var className = curClickedButton.className;
	className = className.substr(0, className.length - 3);	// strip out _on from class name
	curClickedButton.className = className;
	
	curClickedButton = null;
}

function ButtonPopIn(buttonId, title, content, link, width, fn) {
	ClosePopInWindow();

	// Highlight the clicked button
	var btnObj = SettingButtonClicked(buttonId);
	
	// Find its position and position the pop-in window below it.
	var pos = BFW_buttons_FindPos(btnObj);
	var left = Math.floor(pos[0] - (width / 2));
	var top = pos[1] + btnObj.offsetHeight;

	ShowPopInWindow(title, content, link, width, top, left, fn);
	
	// Make sure the pop-in isn't off the right side of the window
	MovePopInIntoWindow();
}

// Find the position of an element in the page
// From www.quirksmode.com
function BFW_buttons_FindPos(obj) {
	var curleft = curtop = 0;
	if (obj.offsetParent) {
		curleft = obj.offsetLeft
		curtop = obj.offsetTop
		while (obj = obj.offsetParent) {
			curleft += obj.offsetLeft
			curtop += obj.offsetTop
		}
	}
	return [curleft,curtop];
}


var curClickedButton = null;
function SettingButtonClicked(id) {
	// De-highlight old button if necessary
	ResetSettingButton();
	
	curClickedButton = document.getElementById(id);
	
	// Old class must be gray, but we don't know if it's big, medium, or small
	curClickedButton.className = curClickedButton.className + "_on";
	
	// Return a handle to the button
	return curClickedButton;
}

function ResetSettingButton() {
	if (curClickedButton == null) {
		return;
	}
	var className = curClickedButton.className;
	className = className.substr(0, className.length - 3);	// strip out _on from class name
	curClickedButton.className = className;
	
	curClickedButton = null;
}

function ButtonPopIn(buttonId, title, content, link, width, fn) {
	ClosePopInWindow();

	// Highlight the clicked button
	var btnObj = SettingButtonClicked(buttonId);
	
	// Find its position and position the pop-in window below it.
	var pos = BFW_buttons_FindPos(btnObj);
	var left = Math.floor(pos[0] - (width / 2));
	var top = pos[1] + btnObj.offsetHeight;

	ShowPopInWindow(title, content, link, width, top, left, fn);
	
	// Make sure the pop-in isn't off the right side of the window
	MovePopInIntoWindow();
}

// Find the position of an element in the page
// From www.quirksmode.com
function BFW_buttons_FindPos(obj) {
	var curleft = curtop = 0;
	if (obj.offsetParent) {
		curleft = obj.offsetLeft
		curtop = obj.offsetTop
		while (obj = obj.offsetParent) {
			curleft += obj.offsetLeft
			curtop += obj.offsetTop
		}
	}
	return [curleft,curtop];
}

