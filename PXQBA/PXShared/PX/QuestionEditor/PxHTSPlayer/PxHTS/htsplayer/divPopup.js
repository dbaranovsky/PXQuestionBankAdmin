

// Script Source: CodeLifter.com
// Copyright 2003
// Do not remove this header

var isIE=document.all;
var isNN=!document.all&&document.getElementById;
var isN4=document.layers;
var isHot=false;
var theLayerID = "";

function ddInit(e){
  topDog=isIE ? "BODY" : "HTML";
  whichDog=isIE ? document.all.theLayer : document.getElementById("theLayer"+theLayerID);  
  hotDog=isIE ? event.srcElement : e.target;  
  while ((hotDog.id!= "titleBar"+theLayerID) && hotDog.tagName!=topDog){
    hotDog=isIE ? hotDog.parentElement : hotDog.parentNode;
  }  
  if (hotDog.id==("titleBar"+theLayerID)){
    offsetx=isIE ? event.clientX : e.clientX;
    offsety=isIE ? event.clientY : e.clientY;
    nowX=parseInt(whichDog.style.left);
    nowY=parseInt(whichDog.style.top);
    ddEnabled=true;
    document.onmousemove=dd;
  }
}

function dd(e){
  if (!ddEnabled) return;
  whichDog.style.left=isIE ? nowX+event.clientX-offsetx+"px" : nowX+e.clientX-offsetx+"px"; 
  whichDog.style.top=isIE ? nowY+event.clientY-offsety +"px": nowY+e.clientY-offsety+"px";
  if (parseInt(whichDog.style.top) < 10) 
  {
	  whichDog.style.top ="10px";
  }
  //ResizeIFrame(); 
  return false;  
}

function ddN4(whatDog){
  if (!isN4) return;
  N4=eval(whatDog);
  N4.captureEvents(Event.MOUSEDOWN|Event.MOUSEUP);
  N4.onmousedown=function(e){
    N4.captureEvents(Event.MOUSEMOVE);
    N4x=e.x;
    N4y=e.y;
  }
  N4.onmousemove=function(e){
    if (isHot){
      N4.moveBy(e.x-N4x,e.y-N4y);
      return false;
    }
  }
  N4.onmouseup=function(){
    N4.releaseEvents(Event.MOUSEMOVE);
  }
}

function hideEqEditor(id)
{
  if (isIE||isNN) document.getElementById("theLayer"+id).style.display="none";
  theLayerID = "";
  //ResizeIFrame(); 
}

function showEqEditor(anchor, id)
{
  var coord = null;
  theLayerID = id;
  var el = document.getElementById("theLayer"+id);
  if (isIE||isNN) document.getElementById("theLayer"+id).style.display="block";
  if (anchor != null ) 
  {
		x=AnchorPosition_getPageOffsetLeft(anchor)+20;
        y=AnchorPosition_getPageOffsetTop(anchor)+ anchor.offsetHeight;
		document.getElementById("theLayer"+id).style.left = "50px";
		document.getElementById("theLayer"+id).style.top = y + "px";

  }
  //ResizeIFrame(); 
}

document.onmousedown=ddInit;
document.onmouseup=Function("ddEnabled=false");

