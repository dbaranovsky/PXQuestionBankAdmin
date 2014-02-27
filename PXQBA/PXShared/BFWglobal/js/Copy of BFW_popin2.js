// *************************************************************
// Functions for "pop-in" windows (DHTML simulations of pop-up windows)
// Author: Pepper Williams (pwilliams@bfwpub.com)
// *************************************************************

// *************************************************************
// Public functions

var shadowWidth = 6;
var shadowHeight = 6;

// Define pop-in window HTML
popInWindowHTML = '<style type="text/css">'
	+ '.popInWindowS {visibility:hidden; position:absolute; z-index:20000; left:20; top:20; width:300px}'
	+ '.popInInnerS {position:relative; left:' + shadowWidth + 'px; top:' + shadowHeight + 'px; border:1px solid #000; background-color:#fff; font-size:12px; text-align:left; color:#000; font-family:Verdana, sans-serif}'
	+ '.popInTitleS {font-size:11px; font-family:Verdana, sans-serif; background: #434343 url("/BCSv3_cc/images/settingsbox-headbg.gif") repeat-x bottom left; padding: 6px 10px; color:#fff; text-transform:uppercase; white-space:nowrap; cursor:move}'
	+ 'a.popInCloseLinkS {float:right; width:13px; height:0; padding-top:13px; overflow:hidden; background: url("/BCSv3_cc/images/settingsbox-close.gif") no-repeat; cursor:pointer}'
	+ '</style>'
	+ '<div id="popInWindow" class="popInWindowS"><div class="popInInnerS">'
	+ '    <table border="0" cellspacing="0" cellpadding="0" width="100%"><tr>'
	+ '        <td class="popInTitleS" id="popInTitle" style="width:95%;">Title</td>'
	+ '        <td class="popInTitleS" style="padding-right:5px; cursor:default"><a class="popInCloseLinkS" href="Javascript:ClosePopInWindow()">Close Box</a></td>'
	+ '    </tr></table>'
	+ '    <div style="padding:5px">'
	+ '        <div id="popInContent">content</div>'
	+ '        <div id="popInLinkHolder" style="margin-top:5px; display:none"><b>&raquo;</b> <span id="popInLink">link</span></div>'
	+ '    </div>'
	+ '</div></div>';


// Write the pop-in window
function WritePopInWindow() {
	document.write(popInWindowHTML);
}

// Show something in the popin window
function ShowPopInWindow(title, content, link, width, top, left, fn) {
	// Initialize if necessary
	if (popInContent == null) {
		InitializePopInWindow();
	}
	
	// If already open, close
	if (popInTitleShowing != null) {
		ClosePopInWindow();
	}
	
	// Set the title, conent, and link
	popInTitle.innerHTML = title;	// "<b>" + title + "</b>";
	popInContent.innerHTML = content;
	
	// If link is null, there isn't one, so hide the div
	if (link == null) {
		popInLinkHolder.style.display = "none";
	} else {
		popInLinkHolder.style.display = "block";
		popInLink.innerHTML = link;
	}

	// Position the popInWindow in the same position as it was last left,
	// relative to the current scroll position -- unless a specific position
	// was sent in

	// Find out what the current scroll position is
	var currentScrollY;
	if (document.documentElement && document.documentElement.scrollTop) {
		currentScrollY = document.documentElement.scrollTop;
	} else {
		currentScrollY = document.body.scrollTop;
	}
	
	// If top/left are set, scroll to that position
	// If left is null set it to -1, meaning that we'll stay wherever we were
	if (left == null) {
		left = -1;
	}
	// If top is null, shift so that it's near the top of the screen, regardless of scrolling
	if (top == null) {
		top = currentScrollY + 10;
	}

	// Adjust left/top to deal with margins for shadows
	if (left != -1) {
		left = left * 1 - shadowWidth;
	}
	if (top != -1) {
		top = top * 1 - shadowHeight;
	}
	
	ShiftPopInWindow(left, top, true);

	// Record the current scroll position
	lastScrollY = currentScrollY;
	
	// Set width to provided or default width
	if (width != null) {
		popInWindow.style.width = width + "px";
	} else {
		popInWindow.style.width = defaultWidth + "px";
	}	
	
	// Show the popInWindow
	popInWindow.style.visibility = "visible";
	
	// record popInTitleShowing
	popInTitleShowing = title;
	
	// Record the clean up function, if one was sent.
	cleanUpFunction = fn;
}

// Show an iframe in the popin window
function ShowPopInIFrame(title, url, width, height, top, left, fn) {
	// Initialize if necessary
	if (popInContent == null) {
		InitializePopInWindow();
	}
	
	// If already open, close
	if (popInTitleShowing != null) {
		ClosePopInWindow();
	}
	
	// Set the title
	popInTitle.innerHTML = "<b>" + title + "</b>";
	
	// hide the link div
	popInLinkHolder.style.display = "none";

	// Position the popInWindow in the same position as it was last left,
	// relative to the current scroll position -- unless a specific position
	// was sent in

	// Find out what the current scroll position is
	var currentScrollY;
	if (document.documentElement && document.documentElement.scrollTop) {
		currentScrollY = document.documentElement.scrollTop;
	} else {
		currentScrollY = document.body.scrollTop;
	}
	
	// If top/left are set, scroll to that position
	// If left is null set it to -1, meaning that we'll stay wherever we were
	if (left == null) {
		left = -1;
	}
	// If top is null, shift so that it's near the top of the screen, regardless of scrolling
	if (top == null) {
		top = currentScrollY + 10;
	}
	
	ShiftPopInWindow(left, top, true);

	// Record the current scroll position
	lastScrollY = currentScrollY;
	
	// Set width to provided or default width
	// For this version, the width refers to the iframe width;
	// for the width of the pop-in, we add on an additional 10px for padding
	if (width == null) {
		width = defaultWidth;
	}
	popInWindow.style.width = (width + 10) + "px";
	
	if (height == null) {
		height = 300;	// default height
	}

	// Include the iframe
	popInContent.innerHTML = '<iframe id="popInIFrame" name="popInIFrame" src="' + url + '" frameborder="0" allowTransparency="true" style="background-color:transparent; width:' + width + 'px; height:' + height + 'px"></iframe>';
	
	// Show the popInWindow
	popInWindow.style.visibility = "visible";
	
	// record popInTitleShowing
	popInTitleShowing = title;
	
	// Record the clean up function, if one was sent.
	cleanUpFunction = fn;
}

// Hide the popin window
function ClosePopInWindow() {
	if (popInWindow == null) {
		return;
	}
	
	popInWindow.style.visibility = "hidden";
	
	// Set popInTitleShowing to null
	popInTitleShowing = null;
	
	// run cleanUpFunction if it was sent
	if (cleanUpFunction != null) {
		cleanUpFunction();
	}
}

// Keep track of the title we're currently showing in the pop-in window
var popInTitleShowing = null;
function PopInShowing() {
	return popInTitleShowing;
}

function ResizePopInWindow() {
	return;
	
	if (popInWindow == null) {
		return;
	}
	
	var oldWidth = popInWindow.style.width
	popInWindow.style.width = "";
	var width = popInContent.offsetWidth;
	alert(width);
	popInWindow.style.width = oldWidth;
}

// Move the pop-in to a position relative to el
function MovePopInToLocation(el, relativeLeft, relativeTop) {
	var elPos = PopIn_findPos(el);
	if (elPos == null) {
		return;
	}
	
	var left = elPos[0] - 6;
	var top = elPos[1] + elPos[3] - 6;
	if (relativeLeft != null) {
		left += relativeLeft;
	}
	if (relativeTop != null) {
		top += relativeTop;
	}
	
	ShiftPopInWindow(left, top, true);
	
	// then make sure we didn't move off the window
	MovePopInIntoWindow();
}



// *************************************************************
// internal functions

// Convenience variables to keep track of the DOM elements we need to manipulate
var popInWindow;
var popInTitle;
var popInContent;
var popInLinkHolder;
var popInLink;

// Global variables for moving the window around
var startX, startY;		// position of mouse the last time we executed MovePopInWindow
var mouseX, mouseY;		// current position of mouse
var lastScrollY = 0;	// scroll position the last time we moved
var popInWindowX, popInWindowY;	// current position of the popInWindow div

// Default width for pop-in window, and default background color
var defaultWidth = 325;

// Add the following constant to the width value sent in to ShowPopInWindow
// for an image
popinMarginOffset = 20;

// Hold a function to run when the pop-in window is closed
var cleanUpFunction = null;

// Initialize the def window.  This has to be done every time we load a new section
function InitializePopInWindow() {
	popInWindow = document.getElementById("popInWindow");
	popInContent = document.getElementById("popInContent");
	popInTitle = document.getElementById("popInTitle");
	popInLinkHolder = document.getElementById("popInLinkHolder");
	popInLink = document.getElementById("popInLink");
	popInWindowX = 10;	// initial values set according to the hard-coded DIV values
	popInWindowY = 10;

	// Set up event handlers to allow the window to be dragged
	popInTitle.onmousedown = EngagePopInWindow;
	document.onmouseup = Disengage;
}


// Shift the popInWindow.  If absolute = true, go to x, y (but don't shift to a -1 location). 
// If absolute = false or undefined, move by x, y
function ShiftPopInWindow(x, y, absolute) {
	if (absolute) {
		if (x != -1) {
			popInWindowX = x;
		}
		if (y != -1) {
			popInWindowY = y;
		}
	} else {
		popInWindowX += x;
		popInWindowY += y;
	}
	
	// Technically, the style should be "10px", not just "10".
	popInWindow.style.left = popInWindowX + "px";
	popInWindow.style.top = popInWindowY + "px";
}

// Make sure the window isn't off the right side of the window
function MovePopInIntoWindow() {
	var windowWidth = window.innerWidth;
	if (windowWidth == null) {
		windowWidth = document.body.clientWidth;
	}
	
	windowWidth = document.body.scrollWidth;
	
	var popWidth = popInWindow.offsetWidth + shadowWidth;
	var popLeft = popInWindow.offsetLeft;
	
	if ((popLeft + popWidth) > windowWidth) {
		popInWindow.style.left = windowWidth - popWidth + "px";
	}
	
//	alert('offsetLeft: ' + popInWindow.offsetLeft + ' / offsetWidth: ' + popInWindow.offsetWidth + ' / window width: ' + windowWidth);	
}

// Get the current mouse position, taking account off any scrolling
function SetMousePositions(e) {
	// Netscape-compatible browsers send the event in as a parameter to the function,
	// but Explorer-compatible browsers require us to get the event.
	if (!e) var e = window.event;
	
	// See the bottom of http://www.quirksmode.org/index.html?/js/events_compinfo.html
	// for a version of this code that will work with all browsers; we only care about IE, NS, and Safari

	mouseX = e.clientX + document.body.scrollLeft;
	mouseY = e.clientY + document.body.scrollTop;
}

// This function is set as an event handler that's called
// when the user first clicks on the definition window
function EngagePopInWindow(e) {
	// Register an onmousemove event handler at the document level to move the window
	document.onmousemove = MovePopInWindow;

	// Get the mouse event (see note in SetMousePositions)
	if (!e) var e = window.event;
	
	// set the initial values of startX/Y
	SetMousePositions(e);
	startX = mouseX;
	startY = mouseY;
	
	// Also set the values for lastScrollY
	// Note that we don't bother with X scrolling
	if (document.documentElement && document.documentElement.scrollTop) {
		lastScrollY = document.documentElement.scrollTop;
	} else {
		lastScrollY = document.body.scrollTop;
	}
	
	return StopEvent(e);
}

// MovePopInWindow: called when the mouse is moved after engaging the window
function MovePopInWindow(e) {
	// Get the mouse event	
	if (!e) var e = window.event;

	// Get the new mouse position
	SetMousePositions(e);
	
	// Move the window
	ShiftPopInWindow(mouseX - startX, mouseY - startY, false);

	// Set startX/Y so that the next time this function is called we'll move appropriately
	startX = mouseX;
	startY = mouseY;

	return StopEvent(e);
}


// Disengage: called when the mouse button is released
function Disengage() {
	// Clear the onmousemove event handler
	document.onmousemove = null;
}

// Stop the event e from bubbling up to higher layers of the DOM
// see http://www.quirksmode.org/index.html?/js/events_compinfo.html
function StopEvent(e) {
	e.cancelBubble = true;		// IE
	if (e.stopPropagation) 		// everyone else
		e.stopPropagation();
	
	// In NS, we (also?) have to return false to cancel the default action from occurring.
	// StopEvent returns false so that the caller can say "return StopEvent(e);"
	// to cover all eventualities.
	return false;
}

