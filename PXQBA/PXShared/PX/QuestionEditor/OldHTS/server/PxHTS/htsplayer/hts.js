var winEqEditor = null;
var popupObj = null;;
var _eqForEqEditor="";
var _idForEqEditor="";
var _eqVarsForEqEditor=""; //rvg
//var _serverURL = 'http://dev.px.bfwpub.com:83/PxHTS/'; //this value will by replaced by HTS player (jsSetServerURL(...))

function jsFunc(graphID) 
{
	var gfunc = "";
	var el = document.getElementById(graphID);
	if (el != null) gfunc = el.getAttribute("function");
	return gfunc; 
}

// rvg - 06/16/2009
// problem ID#3 - Allow answer blanks to scale to contents in review/edit mode in Firefox.
function isFireFox()
{
	var ua = navigator.userAgent.toLowerCase();
	return (ua.indexOf("gecko") != -1); // Gecko = Mozilla + Firefox + Netscape
}
function onKeyPressed(e,inp) // this function attached to input text tag - onkeypress="onKeyPressed(event,this)"
{							 // see function getIproelementData in XMLParser.as
	if (isFireFox())
	{
		inp.size = inp.value.length + (inp.value.length>15 ? 2: 1);
		if (inp.size < 4)
		{
			inp.size =4;
		}
	}
}
//>>>> end >>>	rvg - 06/16/2009
	
function next(btn, step) 
{
	var div = document.getElementById(step);
	if (div != null) div.style.display = "inline";
	
	var b = document.getElementById(btn);
	if (b != null) b.style.display = "none";

}
	

function jsDisplay(displayStep, divStr) 
{
	var div = document.getElementById(displayStep);
	div.innerHTML =divStr;
	if (window.ResizeIFrame) 
	{ 
		  ResizeIFrame();
	}	
}

function mult_MC_Answer(mcid,id, step) 
{
	for ( i=1; i<=20; i++)  
	{
		if ( document.getElementById(step+mcid+"_"+i) != null)
		{
			document.getElementById(step+mcid+"_"+i).checked = false;
		} 
	} 
	document.getElementById(step+mcid+"_"+id).checked = true;
}
	 
function enterfieldAnswer(inputId) 
{
    var elem = document.getElementById(inputId); 
    if (( elem!=null) && (elem.value != null))
    {
        onKeyPressed(null,elem); // still needed? - rvg
    }
}

function message(str) 
{
	alert(str);
}

function getMCAnswer(id) 
{
    var rid = id;
    if (rid.indexOf(".") > -1)
    {
        rid = rid.substring(rid.indexOf(".")+1,rid.length);
    }

    var uanswer = "<iproelement><userinput type=\"mc\" inputid=\"" + rid + "\" answer=\"unchecked\" choiceid=\"-1\"></userinput></iproelement>";
	for ( i=1; i<=20; i++)  
	{
	    var elem = document.getElementById(id+"_"+i);
		if (( elem != null) && (elem.checked == true))
		{
	        uanswer = "<iproelement><userinput type=\"mc\" inputid=\"" + rid + "_" + i + "\" answer=\"checked\" choiceid=\"" + i +"\"></userinput></iproelement>";	    
	        break;
		} 
	} 
    return uanswer;
}

function getNumericAnswer(id)
{
		var el = document.getElementById(id);
		if (el != null)
		{
		    var val = encodeURIComponent(el.value);
		    if (id.indexOf(".") > -1)
            {
                id = id.substring(id.indexOf(".")+1,id.length);
                return "<iproelement><userinput type=\"numeric\" inputid=\"" + id + "\" answer=\"" + val +"\"></userinput></iproelement>"
            }

		}
		else
		{
		    return "";
		}
}
function getTextAnswer(id)
{
		var el = document.getElementById(id);
		if (el != null)
		{
		    var val = encodeURIComponent(el.value);
		    
		    if (id.indexOf(".") > -1)
            {
                id = id.substring(id.indexOf(".")+1,id.length);
                return "<iproelement><userinput type=\"text\" inputid=\"" + id + "\" answer=\"" + val +"\"></userinput></iproelement>";
            }

		}
		else
		{
		    return "";
		}
}

function getMathAnswer(id)
{
		var el = document.getElementById(id);
		if (el != null)
		{
		    var val = el.getAttribute("userAnswer");
		    if (id.indexOf(".") > -1)
            {
                id = id.substring(id.indexOf(".")+1,id.length);
                return "<iproelement><userinput type=\"math\" inputid=\"" + id + "\" answer=\"" + val +"\"></userinput></iproelement>"
            }

		}
		else
		{
		    return "";
		}
}

function replaceCode(str)
{
	var re = "";
	if(str.indexOf(",") != -1)
	{
		re = /,/g;
		str = str.replace(re,"+\',\'+");
	}

	if(str.indexOf(";") != -1)
	{
		re = /;/g;
		str = str.replace(re,"+\';\'+");
	}

	return str;
}

function trim(str)
{
    return str.replace(/^\s+|\s+$/g, '') ;
}
	
function checksyntax(step,elems)
{
    var n=checksyntax.arguments.length;
    var allowedwords = "";
    var i,j,k;
    var val;
    var Cnt_Non_numeric = 0;
    
    for (i=1; i<n; i++)
    {
        var isCorrect = true;
        var id = checksyntax.arguments[i];
        var el = document.getElementById(id);
        if (el != null)
        {
            allowedwords = trim(el.getAttribute("allowedwords"));
            if (allowedwords != "")
            {
                allowedwords = allowedwords.split(',');
            }
            
            val = el.value;
            val = trim(val); 
            if (val.length != 0)
            {
                isCorrect = false;
                if (((val.charAt(0)=='(') || (val.charAt(0)=='[')) && ((val.charAt(val.length-1)==')') || (val.charAt(val.length-1)==']')))
                {
                    val = val.substr(1,val.length-2); //remove brackets
                }
            
                val = val.replace(';',',');
                val = trim(val); 
                
                var vals = val.split(',')
                for (j=0; j < vals.length; j++)
                {
                    var v = trim(vals[j]);
                    for (k=0; k<allowedwords.length; k++)
                    {
                        isCorrect = (v.toLowerCase()==trim(allowedwords[k].toLowerCase()));
                        if (isCorrect) break;
                    }
                    if ( !isCorrect ) 
                    {
                        //isCorrect = !isNaN(parseFloat(v));
                        isCorrect = v.match(/^[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$/);
                    }
                    if (!isCorrect) break;
                }
            }
            if (!isCorrect) Cnt_Non_numeric++;
            setSyntaxCorrectness(id,isCorrect);
        }
    }
	var divObj = document.getElementById(step+".CheckSyntaxResult");
	var s = (Cnt_Non_numeric == 0) ? "Syntax OK" : "Non-numeric values in " + Cnt_Non_numeric + " answer blanks";
	divObj.innerHTML = s;
}	


function showEnterfieldAnswer(inputId)
{
    var el = document.getElementById(inputId);
    if (el != null)
    {
        var eqtext = decodeURIComponent(el.getAttribute("userAnswer"));
        displayEqEditor(inputId,eqtext,"");
    }
    
}

function getEqEditorAnswer(inputId, value) 
{
	var el = document.getElementById(inputId);
	if (el != null) 
	{
		el.innerHTML=""; //this is needed? rvg
		el.src = _serverURL+ "geteq.ashx?eqtext=" + encodeURIComponent(value) + "&doborder=true&bottom=8";
		el.setAttribute("userAnswer",encodeURIComponent(value));
	}
}


	
//to restore numeric and text short answers
//function setValue_sa(input_id,sa_value,step)
//{
//	//if (_showUserAnswer) //rvg
//	//{
//		document.getElementById(input_id).value = sa_value;
//		var id = input_id.replace(step + ".EnterField","");
//		enterfieldAnswer(id, step);
//	//}
//}

// set value of enterField only
// do not checking the answer	
//function setValueOnly_sa(input_id,sa_value,step)
//{
//	document.getElementById(input_id).value = sa_value;
//	onKeyPressed(null,document.getElementById(input_id));
//}
//	
function setSyntaxCorrectness(input_id, bIsSyntaxCorrect)
{
	if (bIsSyntaxCorrect)
	{	
		document.getElementById(input_id).style.borderColor='LightGreen' ;
		document.getElementById(input_id).style.borderStyle='solid' ;
	}
	else
	{
		document.getElementById(input_id).style.borderColor='Red' ;
		document.getElementById(input_id).style.borderStyle='solid' ;			
	}
} 
	
//to restore multiple-choice answers
function setValue_mc(input_id,id,step)
{
	//if (_showUserAnswer) //rvg
	//{
		document.getElementById(input_id).checked = true;
		var id = parseInt(input_id.replace(step + ".Checkbox",""));
		var str = input_id.substring(input_id.indexOf("Checkbox")+8,input_id.length);
		var index = str.indexOf("_");

		if(index != -1)
		{
			var arr = str.split("_");
			mult_MC_Answer(arr[0],arr[1], step);
		}
		else
		{
			multchoiceAnswer(id, step);
		}
	//}
}  

//function popupEqEditorWin(x,y, width, height, step, id, eq)
//{
//	//basePopupPath--still needed for Mac???
//	//first check for a ? in the path and get rid of it and everything to the right
//	var idx = location.href.indexOf("?")
//	var basePopupPath = idx < 0 ? location.href : location.href.substring(0,idx);
//	//Now truncate up to last slash
//	basePopupPath = basePopupPath.substring(0,basePopupPath.lastIndexOf("/")+1);

//	var newWin = winEqEditor;
//	var winName = "Equation_Editor";		
//	var winURL =basePopupPath+"eqEditor.html?step="+step+"&id="+id+"&eq="+eq; 
//	if (!winEqEditor || winEqEditor==null || winEqEditor.closed)  
//	{
//    	winEqEditor = window.open(winURL,winName,"resizable=yes,height="+height+",width="+width+",scrollbars=yes"+",left="+x+",top="+y);
//		winEqEditor.focus();
//	} 
//	else  
//	{
//		if (winURL != newWin.document.location)
//		{
//			winEqEditor.document.location = winURL;
//		}
//		winEqEditor.moveTo(x,y);
//		winEqEditor.focus();
//	}
//	return winEqEditor;
//}

//function closeWinEq() 
//{
//	if (winEqEditor && ! winEqEditor.closed)
//	{
//	    winEqEditor.close();
//	    winEqEditor = null;
//	}
//}

//******* functions used to interface with equation editor

 //controller calls this function when an equation is clicked.
 //    step, id:  identifies the  eqution, have to be passed back to eqSubmit
 //    eqtext:  current equation text.
function displayEqEditor(inputId, eqtext, eqvars)
{
	_eqForEqEditor = eqtext;
	_idForEqEditor = inputId;
    _eqVarsForEqEditor = eqvars;
     var anchor =  document.getElementById(inputId);
	 var idh='';
    if (inputId.indexOf(".") > -1)
    {
        idh = inputId.substring(3,inputId.indexOf("."));
    }
    //if (anchor==null)alert("anchor==null");
	 showEqEditor(anchor,idh); 
	 
     window.setInterval("checkEqEditorPos("+idh+")", 550); 
}


//function to be called by the eq editor to update equation
//    step, id: passed back from from displayEqEditor
//    eqtext:  new equation text
function eqSubmit1(stepId, inputId, eqtext)
{
	getEqEditorAnswer(inputId, eqtext);
}
	
function getEqForEqEditor()
{
	return _eqForEqEditor;
}
	
function getIdForEqEditor()
{
	return _idForEqEditor;
}
	
function getStepForEqEditor()
{
	return _idForEqEditor;
}
	
function getVarsForEqEditor() 
{
	return _vars;
}
	
//*******
function eqEditorInitText() 
{
	return _eqForEqEditor;
}
	
function eqCancel()
{
	var idh='';
    if (_idForEqEditor.indexOf(".") > -1)
    {
        idh = _idForEqEditor.substring(3,_idForEqEditor.indexOf("."));
    }
	
	hideEqEditor(idh);
}
	
var _pageLoaded = false;
	
function pageLoaded() 
{
	_pageLoaded = true;
	//if (window.ResizeIFrame) 
	//{ 
	//	ResizeIFrame();
	//}
	// 2010-06-11 SH: Added hook to call optional CQReady function in BFW code.
	//if (window.CQReady) 
	//{
	//	window.CQReady();
	//}
}
	
function isPageReady() 
{
	return _pageLoaded;
}
	
function eqEditorLoaded() 
{
}	
	
function eqSubmit(eqText) 
{
	if (eqText != _eqForEqEditor) 
	{
		_eqForEqEditor = eqText;
	    eqSubmit1( _idForEqEditor,_idForEqEditor, eqText);	
	}
	var idh='';
    if (_idForEqEditor.indexOf(".") > -1)
    {
        idh = _idForEqEditor.substring(3,_idForEqEditor.indexOf("."));
    }
	
	hideEqEditor(idh);
}

// Create an object for a WINDOW popup and show correct answer in popup DIV. -- Wayne
function popupTextAndNum(anchorname , text) 
{
	var idh='';
    if (anchorname.indexOf(".") > -1)
    {
        idh = anchorname.substring(3,anchorname.indexOf("."));
    }

	popupObj = new PopupWindow("popupdiv"+idh);
	popupObj.offsetX =20;
	popupObj.offsetY = -30;
	popupObj.autoHide();
	popupObj.populate("<DIV STYLE='background-color: #ffffff; border:blue 2px solid;padding: 5px '><i>Correct answer is&#160;</i>" + text+"</DIV>");
	popupObj.showPopup(anchorname);
}

function popupMath(anchorname , url) 
{
	var idh='';
    if (anchorname.indexOf(".") > -1)
    {
        idh = anchorname.substring(3,anchorname.indexOf("."));
    }

	popupObj = new PopupWindow("popupdiv"+idh);
	popupObj.offsetX = 20;
	popupObj.offsetY = -30;
	popupObj.autoHide();
	// rvg 2009 - Iproelement_short.getCorrectForShowCorrect() prepare fully complete <div> body except STYLE and <i>correct answer is&#160;</i> text
	// because a structure for showing multiple correct answers is very complex.  
	popupObj.populate("<div style='background-color: #ffffff; border:blue 2px solid; padding: 5px'><i>correct answer is&#160;</i>" +  url + "</div>");
	popupObj.showPopup(anchorname);
    //window.setTimeout("pictAnswerLoaded('"+ anchorname + "')", 800); //rvg - for fixing PP-735 
    	// (For long answers on review, correct answer window gets cut and full answer is not visible  )
    	// function pictAnswerLoaded shifts answer window if needed
}

function hidepopup()
{
	popupObj.hidePopup();
}

function refreshPopup(anchorname)
{
	hidepopup();
	popupObj.showPopup(anchorname);
}

function checkEqEditorPos(id)
{
//  if (isIE||isNN) 
//  {
//  	if( document.getElementById("theLayer").style.display!="block") return;
//  	var left = parseInt(document.getElementById("theLayer").style.left);
//  	var top = parseInt(document.getElementById("theLayer").style.top);
//  	var bodies = document.getElementsByTagName("body");
//  	var body = bodies[0];
//  	
//  	if (( left +20 > body.clientWidth ) || (left < 0) || (top < 0) || (top+20 >body.clientHeight))
//  	{ 
//  		document.getElementById("theLayer").style.left = 300 +'px';
//  		document.getElementById("theLayer").style.top = 200 +'px';
//  	}
//  }
//  else if (isN4) 
//  {
//  	if (document.theLayer.visibility!="show") return;
//  	var left = parseInt(document.theLayer.style.left);
//  	var top = parseInt(document.theLayer.style.top);
//  	var bodies = document.getElementsByTagName("body");
//  	var body = bodies[0];
//  	if (( left +20 > body.clientWidth ) || (left < 0) || (top < 0) || (top+20 >body.clientHeight))
//  	{ 
//  		document.theLayer.style.left = 300 +'px';
//  		document.theLayer.style.top = 200 +'px';
//  	}
//  }
try
{
    if (window.document.eqeditor) 
    {
        //window.document.eqeditor.focus();
    }
}
catch(e){}

  	var isIE = (navigator.appName.indexOf("Microsoft") != -1);
  	var eqed = (isIE) ? document.getElementById('eqeditor'+id) : document['eqeditor'+id];
//	(isIE) ? document.getElementById('eqeditor'+id).focus() : document['eqeditor' + id].focus();
if (eqed)
{
    try{
        eqed.focus();
    }
    catch(e){}
}
	

  //document.getElementById('eqeditor0').focus();
}

/// rvg - for fixing PP-735
function pictAnswerLoaded(anchorname)
{
	var coordinates=getAnchorPosition(anchorname);
	var x = coordinates.x;
	var wx= document.getElementById('popupdiv').scrollWidth;
	if (wx+x+ popupObj.offsetX > document.body.clientWidth)
	{
		var offX = document.body.clientWidth - wx -x;
		if (offX < (-x) ) 
		{
			offX =-x;
		}
		popupObj.offsetX = offX;
		popupObj.offsetY = 20;
		refreshPopup(anchorname);	    
	}
}
	
//*******
function QuestionInitializePage() 
{
    pageLoaded();
}

//needed to avoid JS errors in ipro environment, function must be fully commented in portal version of file 
function ResizeIFrame() {}

//for Generating a list of variables


function setImgFormulaMargin(id, margin)
{
	el = document.getElementById(id);
	el.style.verticalAlign='baseline';
	el.style.marginBottom=margin;	
}

function onProblemLoaded() 
{
	if (window.ResizeIFrame) 
	{ 
		ResizeIFrame();
	}
	// 2010-06-11 SH: Added hook to call optional CQReady function in BFW code.
	if (window.CQReady) 
	{
		window.CQReady();
	}
}
