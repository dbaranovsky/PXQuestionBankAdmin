// HTS Maths.js file
//

function log10(x){
   return Math.log(x)/Math.LN10;
}

function neg(x){
   return -x;
}

// NORMAL

function NormalP( x ) {
   // Abramowitz & Stegun 26.2.19
   var
      d1 = 0.0498673470,
      d2 = 0.0211410061,
      d3 = 0.0032776263,
      d4 = 0.0000380036,
      d5 = 0.0000488906,
      d6 = 0.0000053830;

   var a = Math.abs(x);
   var t = 1.0 + a*(d1+a*(d2+a*(d3+a*(d4+a*(d5+a*d6)))));

   // to 16th power
   t *= t;  t *= t;  t *= t;  t *= t;
   t = 1.0 / (t+t);  // the MINUS 16th

   if (x >= 0)  t = 1 - t;
   return t;
   }

function InvNormalP( p ) {
   // Odeh & Evans. 1974. AS 70. Applied Statistics. 23: 96-97
   var
      p0 = -0.322232431088,
      p1 = -1.0,
      p2 = -0.342242088547,
      p3 = -0.0204231210245,
      p4 = -0.453642210148E-4,
      q0 =  0.0993484626060,
      q1 =  0.588581570495,
      q2 =  0.531103462366,
      q3 =  0.103537752850,
      q4 =  0.38560700634E-2,
      pp, y, xp;

   // p: 0.0 .. 1.0 -> pp: 0.0 .. 0.5 .. 0.0
   if (p < 0.5)  pp = p;  else  pp = 1 - p;

   if (pp < 1E-12)
      xp = 99;
   else {
      y = Math.sqrt(Math.log(1/(pp*pp)));
      xp = y + ((((y * p4 + p3) * y + p2) * y + p1) * y + p0) /
               ((((y * q4 + q3) * y + q2) * y + q1) * y + q0);
      }

   if (p < 0.5)  return -xp;
   else  return  xp;
   }


   //samples from a normal distribution with mean mu and std. dev. sigma
   //
   function sampleNormal(mu, sigma) {
       return InvNormalP(Math.random())*sigma +mu;
   }

    //samples from a uniform distribution between -1 and +1
	//
   function sampleUniform1() {
       return 2*Math.random() - 1;
   }


	//calculates the mean of a data set
	//the data can be either an array or a list of parameters 
	//
   function mean(arg) {
	   var total = 0;
	   var n;
	   if (arg instanceof Array)
	   {
		   n = arg.length;
		   for (var i = 0; i < n; i++) {
				total =  total + arg[i];
			}
	   } else {
		    n = mean.arguments.length;
			for (var i = 0; i < n; i++) {
				total =  total + mean.arguments[i];
			}
	   }
	   return total/n;
   }

	//calculates the standard deviation of a data set
	//the data can be either an array or a list of parameters 
	//
    function stddev(arg) {
        var total = 0;
		var n;
		var a = new Array();
		if (arg instanceof Array){
			n = arg.length;
		   for (var i = 0; i < n; i++) {
				a[i]=  arg[i];
			}
		} else {
			n = stddev.arguments.length;
			for (var i = 0; i < n; i++) {
				a[i] = stddev.arguments[i];
			}
		}

	   var av=mean(a);
	   total = 0;
	   for (var i = 0; i < n; i++) {
		   var del = a[i]-av;
            total =  total + del*del;
	   }
	   return Math.sqrt(total/(n-1));
   }

	//calculates the correlation coefficient for a data set consisting of (x, y) pairs
	//the function accepts a list of x-values followed by the corespnding list of y-values
	// usage: called from the item xml using the syntax "correlation(~x[]\, ~y[]\)", where x and y are numarrays.
	//
   function correlation() {
		var x = new Array();
		var y = new Array();
		var n = correlation.arguments.length;
		var nn = n/2;
		for (var i = 0; i < nn; i++) {
            x[i]=correlation.arguments[i];
		} 
		for (var i = nn; i < n; i++) {
            y[i-nn]=correlation.arguments[i];
		}
		
		var xav = mean(x);
		var yav = mean(y);
		var sx = stddev(x);
		var sy = stddev(y);
      
		var total = 0;
		for (var i = 0; i < nn; i++) {
            total = total + (x[i]-xav)/sx*(y[i]-yav)/sy;
		} 
		
		return total / (nn -1);
   }
   
   //calculates the mean of a data set without outliner located at index p
	//the data can be either an array or a list of parameters 
	//
   function mean1() {
		var total = 0;
		var n = mean1.arguments.length - 1;
		var p = mean1.arguments[n];
		for (var i = 0; i < n; i++) {
			total =  total + mean1.arguments[i];
		}
		total = total -  mean1.arguments[p-1];
		return total/(n-1);
   }


	//calculates the standard deviation of a data set without outliner located at index p
	//the data can be either an array or a list of parameters 
	//
	function stddev1() {
		var a = new Array();
		var n = stddev1.arguments.length - 1;
		var p = stddev1.arguments[n];
		var	k = 0;
		for (var i = 0; i < n; i++) {
			if (i != (p-1)) {
				a[k] = stddev1.arguments[i];
				k = k+1;
			}
		}
		return stddev(a);
	}

	//calculates the correlation coefficient for a data set without outliner consisting of (x, y) pairs
	//the function accepts a list of x-values followed by the corespnding list of y-values
	// usage: called from the item xml using the syntax "correlation(~x[]\, ~y[]\)", where x and y are numarrays.
	//
	function correlation1() {
	    var x = new Array();
	    var y = new Array();
	    var n = correlation1.arguments.length - 1;
	    var nn = n/2;
		var	p = correlation1.arguments[n];
		var k = 0;
	    for (var i = 0; i < nn; i++) {
			if (i != (p-1)) {
	            x[k]=correlation1.arguments[i];
				k = k + 1;
			}
		}
		k = 0;
		for (var i = nn; i < n; i++) {
			if (i != (nn+p-1)) {
	            y[k]=correlation1.arguments[i];
				k = k + 1;
			}
		}
		
       var xav = mean(x);
	   var yav = mean(y);
	   var sx = stddev(x);
	   var sy = stddev(y);
      
	   var total = 0;
		for (var i = 0; i < (nn-1); i++) {
            total = total + (x[i]-xav)/sx*(y[i]-yav)/sy;
	   } 
		
		return total / (nn -2);
   }
   
	//calculates the sum of a data set
	//the data can be either an array or a list of parameters 
	//
   function sum(arg) {
	   var total = 0;
	   var n;
	   if (arg instanceof Array)
	   {
		   n = arg.length;
		   for (var i = 0; i < n; i++) {
				total =  total + arg[i];
			}
	   } else {
		    n = sum.arguments.length;
			for (var i = 0; i < n; i++) {
				total =  total + sum.arguments[i];
			}
	   }
	   return total;
   }
 
	//calculates the quartiles (q1, q2=median,q3) of a data set 
	//the data can be either an array or a list of parameters 
	//
	function quart(arg,t) {
		var n;
		var ret;
		var type;
		var a = new Array();
		//get array
		if (arg instanceof Array) {
			type = t;
			n = arg.length;
			for (var i = 0; i < n; i++) {
				a[i] = arg[i];
			}
		} else {
			n = quart.arguments.length-1;
			for (var i = 0; i < n; i++) {
				a[i] = quart.arguments[i];
			}
			type = quart.arguments[n];
		}
		var b = new Array();
		b = sorta(a,1);
		var n2;
		if ((n/2) == Math.ceil(n/2)) {
			if(type==2) ret = (b[n/2-1] + b[(n/2)])/2;
			n2 = n/2;
			if (n2/2 ==  Math.ceil(n2/2)) {
				if(type==1) ret = (b[n2/2-1] + b[(n2/2)])/2;
				if(type==3) ret = (b[n2+n2/2-1] + b[(n2+n2/2)])/2;
		 }else{		
				if(type==1) ret = b[Math.floor(n2/2)];
				if(type==3) ret = (b[Math.floor(n2+ n2/2)]);
			}
		}else{
			n2 = Math.floor(n/2);
			if(type==2) ret = b[n2];
			if (n2/2 ==  Math.ceil(n2/2)) {
				if(type==1) ret = (b[n2/2-1] + b[(n2/2)])/2;
				if(type==3) ret = (b[n2+n2/2] + b[(n2+1+n2/2)])/2;
			 }else{
				if(type==1) ret = b[Math.floor(n2/2)];
				if(type==3) ret = (b[Math.floor(n2+1+n2/2)]);
			}
		}
		return ret;
	}

	//sorts the number set up (dir=1) or down (dir=-1) 
	//the data can be either an array or a list of parameters 
	//
	function sorta(arg,dir) {
		var n;
		var d;
		var a = new Array();
		//get array
		if (arg instanceof Array) {
			d = dir;
			n = arg.length;
			for (var i = 0; i < n; i++) {
				a[i] = arg[i];
			}
		} else {
			n = sorta.arguments.length-1;
			for (var i = 0; i < n; i++) {
				a[i] = sorta.arguments[i];
			}
			d = sorta.arguments[n];
		}
		//sort array
		var v;
		var k = 0;
		do	{ 
			for (var i = 0; i < (n-1); i++) {
				k= i+1;
				if ( d*a[i+1] < d*a[i] ) { 
					v = a[i];
					a[i] = a[i+1];
					a[i+1] = v;
					k = i;
					break;
				}
			}
		} while (k<(n-1));
		return a;
	}

	//sorts the number set up (dir=1) or down (dir=-1) 
	//the data can be either an array or a list of parameters 
	//
	function maxmina(arg,dir) {
		var n;
		var d;
		var a = new Array();
		//get array
		if (arg instanceof Array) {
			d = dir;
			n = arg.length;
			for (var i = 0; i < n; i++) {
				a[i] = arg[i];
			}
		} else {
			n = maxmina.arguments.length-1;
			for (var i = 0; i < n; i++) {
				a[i] = maxmina.arguments[i];
			}
			d = maxmina.arguments[n];
		}
		//sort array
		var maxmin = a[0];
		var k = 0;
		for (var i = 0; i < n; i++) {
			if ( d*a[i] > d*maxmin ) maxmin = a[i]; 
		}
		return maxmin;
	}
	
function array_element_list(arg) {
	 //alert("in list");
	   var list = "";
	   var n =  array_element_list.arguments.length-1; 
		list =  list + array_element_list.arguments[0]; 
		if (n > 0)
		{
			for (var i = 1; i < n; i++) {
					list =  list + ","+ array_element_list.arguments[i];
			}
		}
			//alert(list);
	   return list;
   }



    function a_i(arg) {
		var last = a_i.arguments.length-1;
	    var i =  a_i.arguments[last]; 
		return  a_i.arguments[i-1];
   }

//getGCD returns the Greatest Common Denominator of x and y
function getGCD(x, y) {
	var w, x, y;
	while (y != 0) {
		w = x % y;
		x = y;
		y = w;
	}
	return x;
}

//listLookup(val, a_1, a_2, a_3)  returns the index i such that a_i = val
function listLookup(){
	var result = 0;
	var n =  listLookup.arguments.length; 	
	for (var i = 1; i < n; i++) {		
		if (listLookup.arguments[i]==listLookup.arguments[0])
		{
			result  = i;
		}
	}
    return result;
}


function fact(n){
	if(n==0) return 1;
	var f = 1;
	    for (var i = 1; i<= n; i++) {
			f = f*i
		}
   return f;
}

function nCr(n,r){
	var ret = fact(n)/(fact(r)*fact(n-r))
	return ret;
}

function BinomPDF(n,k,p){
	var ret = nCr(n,k)*Math.pow(p,k)*Math.pow((1-p),(n-k))
	return ret;
}

function BinomCDF(n,k,p){
	var ret = 0;
	for (var i = 0; i<= k; i++) {
		ret = ret + BinomPDF(n,i,p)
	}
	return ret;
}

// 2009.10.08 added by rvg
// original http://statpages.org
//Student' t-distribution function 
// t - the numeric value at which to evaluate the distribution
// n - an integer indicating the number of degrees of freedom that characterize the distribution
function StudT(t,n) {
    var Pi=Math.PI; 
    var PiD2=Pi/2; 
    t=Math.abs(t); 
    var w=t/Math.sqrt(n); 
    var th=Math.atan(w);
    if(n==1) { return 1-th/PiD2 };
    var sth=Math.sin(th); 
    var cth=Math.cos(th);
    if((n%2)==1)
        { return 1-(th+sth*cth*StatCom(cth*cth,2,n-3,-1))/PiD2 }
        else
        { return 1-sth*StatCom(cth*cth,1,n-3,-1) }
    }

function StatCom(q,i,j,b) {
    var zz=1; 
    var z=zz; 
    var k=i; 
    while(k<=j) { zz=zz*q*k/(k-b); z=z+zz; k=k+2 };
    return z;
    
    }