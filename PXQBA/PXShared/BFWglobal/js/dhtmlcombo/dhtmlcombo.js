var combodropimage='downbox.gif' //path to "drop down" image
var combodropoffsetY=2 //offset of drop down menu vertically from default location (in px)
var combozindex=100

if (combodropimage!="")
	combodropimage='<img class="downimage" src="'+combodropimage+'" title="Select an option" />'

var comboX = new Object();
var comboY = new Object();
function dhtmlselect(selectid, selectwidth, optionwidth){
	var selectbox=document.getElementById(selectid)
	comboX["dhtml_"+selectid] = false;
	comboY["dhtml_"+selectid] = false;
	document.write('<div id="dhtml_'+selectid+'" class="dhtmlselect">'+selectbox.title+" "+combodropimage+'<div class="dropdown">')
	for (var i=0; i<selectbox.options.length; i++)
		document.write('<a href="'+selectbox.options[i].value+'">'+selectbox.options[i].text+'</a>')
	document.write('</div></div>')
	selectbox.style.display="none"
	var dhtmlselectbox=document.getElementById("dhtml_"+selectid)
	dhtmlselectbox.style.zIndex=combozindex
	combozindex--
	if (typeof selectwidth!="undefined")
		dhtmlselectbox.style.width=selectwidth
	if (typeof optionwidth!="undefined")
		dhtmlselectbox.getElementsByTagName("div")[0].style.width=optionwidth
	dhtmlselectbox.getElementsByTagName("div")[0].style.top=dhtmlselectbox.offsetHeight-combodropoffsetY+"px"
	if (combodropimage!="")
		dhtmlselectbox.getElementsByTagName("img")[0].style.left=dhtmlselectbox.offsetWidth+3+"px"
	dhtmlselectbox.onclick=function(){
		this.getElementsByTagName("div")[0].style.display="block";
		comboX[this.id] = true;
	}
	dhtmlselectbox.onmouseover=function(){
		comboX[this.id] = true;
	}
	dhtmlselectbox.onmouseout=function(){
		setTimeout('comboGoOut(\''+this.id+'\')',100);
		comboX[this.id] = false;
	}
	dhtmlselectbox.getElementsByTagName("div")[0].onmouseover=function(){
//alert(this.parentNode.id);
		comboY[this.parentNode.id] = true;
	}
	dhtmlselectbox.getElementsByTagName("div")[0].onmouseout=function(){
//alert(1);
		setTimeout('comboGoOut(\''+this.parentNode.id+'\')',100);
		comboY[this.parentNode.id] = false;
	}
}

function comboGoOut (comboId) {
//alert(!comboX[comboId] +' && '+ !comboY[comboId]);
	if (!comboX[comboId] && !comboY[comboId]) {
		document.getElementById(comboId).getElementsByTagName("div")[0].style.display="none"
	}
}


