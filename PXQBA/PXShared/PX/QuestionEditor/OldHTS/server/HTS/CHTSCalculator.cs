using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.JScript;

namespace HTS
{
    class CHTSCalculator
    {
        //public  WebBrowser wb = null;
        //public HtmlDocument hd = null;
        private string _jsMathPath = "";
        private string mathsJS = "";
        //private string _expr = "";
        //private object ob = null;
        private int loadIEComplete = 0;
        private Microsoft.JScript.Vsa.VsaEngine Engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();

        string [] mathsFuncs = {"abs","acos","asin","atan","atan2","ceil","cos","exp","floor","log","max","min","pow","random","round",
                                   "sin","sqrt","tan"};


        string[] mathsProps = { "LN2", "LN10", "LOG2E", "LOG10E", "PI", "SQRT1_2", "SQRT2" }; // "E",

        public CHTSCalculator(string jsMathPath)
        {
            _jsMathPath = jsMathPath;
            mathsJS = GetMathsJS();
            
        }

        /// <summary>
        /// calculate expr using javascript and functions from maths.js 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object doCalculate(string expr)
        {

            if (expr.EndsWith(";")) expr = expr.Substring(0, expr.Length - 1);
            expr = expr.Replace(" ", "");
            
            if (expr.StartsWith("doCalculated("))
            { // we need to eliminate doCalculated from expression
                expr = expr.Substring(14, expr.Length - 16);
            }

            string _expr = expr.Replace("(", "");
            _expr = _expr.Replace(")", "");
            double x = 0;

            // evaluating by JScript is very long time operation
            // try do this simple, if expr is constant
            try
            {
                x = System.Convert.ToDouble(_expr);
                return x;
            }
            catch { }

            // We need use JScript
            expr = PrepareExpr(expr); // translate ^ operation to Math.pow() function


            // Add Math. prefix for some functions and constants
            for (int i = 0; i < mathsFuncs.Count(); i++)
            {
                expr = expr.Replace(mathsFuncs[i] + "(", "Math." + mathsFuncs[i] + "(");
            }
            for (int i = 0; i < mathsProps.Count(); i++)
            {
                expr = expr.Replace(mathsProps[i], "Math." + mathsProps[i]);
            }
            expr = expr.Replace("Math.Math.", "Math.");

            object Result = null;
            try
            {
                Result = Microsoft.JScript.Eval.JScriptEvaluate(mathsJS +  expr , Engine);
            }
            catch (Exception ex)
            {
                return 0;
            }
            return Result;
        }

        /// <summary>
        /// read maths.js script for use in doCalculate method 
        /// </summary>
        /// <returns></returns>
        private string GetMathsJS()
        {
            string mathsJS = string.Empty;
            if (_jsMathPath != string.Empty)
            {
                try
                {
                    mathsJS = File.ReadAllText(_jsMathPath);
                }
                catch { }
            }
            return mathsJS;
        }
        
// rest functions moved here from old maths.js script

        private bool IsOperand(char item)
        {
           if ( (item >= '0' && item <= '9' ) )
               return true;

            return false;
        }

        private int GetNextTokenBack(string expr, int startIndex)
        {
           for ( int ii = startIndex; ii >= 0; ii-- )
           {
               char c = expr[ii];
               if ( !IsOperand( c ) && c != '.' )
               {
    	           return (ii+1);
               }
           }

           return 0;
        }

        private int  GetNextTokenForward(string expr, int startIndex)
        {
            int ii = 0;
           for ( ii = startIndex; ii < expr.Length; ii++ )
           {
               char c = expr[ii];
               if ( !IsOperand( c ) && c != '.' )
               {
	               return (ii);
               }
           }
           return ii;
        }


        //private int GetValue(int token)
        //{
        //   return (0 + token);
        //}


        private int   findConsecutivesBack(string expr, int startIndex, char pattern)
        {
           int ii = startIndex;
           while ( ii >= 0 && expr[ii] == pattern ) 
           {
              ii--;
           } 
         
           if ( ii == startIndex )  ii = -1;

           if ( ii >= 0 )  ii++;

           return ii;
        }

        private int  findConsecutivesForward(string expr, int startIndex, char pattern)
        {
           int ii = startIndex;
           while ( ii < expr.Length && expr[ii] == pattern ) 
           {
              ii++;
           } 
          
           if ( ii == startIndex )  ii = -1;

           return ii;
        }

        // ** Find the closing bracket for the bracket at the start index
        private int  findClosingLBracket(string expr, int startIndex)
        {
           int  loc = startIndex;
           int  count = 1;

           if ( expr[startIndex] != ')' )
           {
               //alert("Error: not start at an open bracket.");
               return -1;
           }

           while ( count != 0 && loc >= 0 )
           { 
               //alert("Found " + count + " ) at loc " + loc);
               loc--;
               if ( expr[loc] == '(' )
                   count--;

               else if ( expr[loc] == ')' )
                   count++;
           }

           if ( expr[loc] != '(' )
           {
               //alert("Error: unmatched ( at " + loc);
               return -1;
           }  

           return loc;    
        }


        // ** Find the closing bracket for the bracket at the start index
        private int findClosingRBracket(string expr, int startIndex)
        {
           int  loc = startIndex;
           int  count = 1;
         
           if ( '(' != expr[startIndex] )
           {
               //alert("Error: not start at an open bracket.");
               return -1;
           }

           while ( count != 0 && loc < expr.Length )
           {
               //alert("Found " + count + " ('s" + " at " + loc + " = " + expr.charAt(loc));
               loc++;
               if ( ')' == expr[loc] )
                   count--;

               else if ( '(' == expr[loc] )
                   count++;
           }

           if ( ')' != expr[loc] )
           {
               return -1;
           }

           return loc+1; // *** including the ) 
        }

        /// <summary>
        /// process ^operation
        /// moved here from maths.js
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private string PrepareExpr(string expr)
        {
            try
            {
                int loc = 0;

                while ((loc = expr.IndexOf("^")) >= 0)
                {
                    int locRBracket = loc - 1;
                    int locLBracket = locRBracket - 1;
                    int locOp1 = 0;
                    int locOp2 = loc + 1;

                    // *** If no power involved, no change
                    if (loc <= 0)
                    {
                        return expr;
                    }

                    // *** Now figure out first operand, first
                    locRBracket = loc - 1;

                    // *** If there're right brackets
                    locLBracket = findClosingLBracket(expr, locRBracket);

                    if (locLBracket < 0)
                    {
                        locOp1 = GetNextTokenBack(expr, loc - 1);
                    }
                    else if (locLBracket < locRBracket)
                    {
                        locOp1 = locLBracket;
                    }
                    else
                    {
                        return "";
                    }

                    // *** Figure out the second operand
                    locLBracket = loc + 1;

                    locRBracket = findClosingRBracket(expr, locLBracket);
                    //alert("Found closing Right Bracket at " + locRBracket);

                    if (locRBracket < 0)
                    {
                        locOp2 = GetNextTokenForward(expr, loc + 1);
                    }
                    else if (locRBracket > locLBracket)
                    {
                        locOp2 = locRBracket;
                    }
                    else
                    {
                        return "";
                    }

                    if (locOp1 < 0 || locOp2 < 0)
                    {
                        return expr;
                    }

                    var Op1 = expr.Substring(locOp1, loc - locOp1);
                    var Op2 = expr.Substring(loc + 1, locOp2 - loc - 1);

                    expr = ("" + expr.Substring(0, locOp1) + "Math.pow(" + Op1 + ", " + Op2 + ")" + expr.Substring(locOp2, expr.Length));//source code

                }
            }
            catch { }
           return expr;
        }
    }
}
