using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using System;
using System.IO;
using System.Linq;
namespace HTS
{
	internal class CHTSCalculator
	{
		private string _jsMathPath = "";
		private string mathsJS = "";
		private int loadIEComplete;
		private VsaEngine Engine = VsaEngine.CreateEngine();
		private string[] mathsFuncs = new string[]
		{
			"abs",
			"acos",
			"asin",
			"atan",
			"atan2",
			"ceil",
			"cos",
			"exp",
			"floor",
			"log",
			"max",
			"min",
			"pow",
			"random",
			"round",
			"sin",
			"sqrt",
			"tan"
		};
		private string[] mathsProps = new string[]
		{
			"LN2",
			"LN10",
			"LOG2E",
			"LOG10E",
			"PI",
			"SQRT1_2",
			"SQRT2"
		};
		public CHTSCalculator(string jsMathPath)
		{
			this._jsMathPath = jsMathPath;
			this.mathsJS = this.GetMathsJS();
		}
		public object doCalculate(string expr)
		{
			if (expr.EndsWith(";"))
			{
				expr = expr.Substring(0, expr.Length - 1);
			}
			expr = expr.Replace(" ", "");
			if (expr.StartsWith("doCalculated("))
			{
				expr = expr.Substring(14, expr.Length - 16);
			}
			string text = expr.Replace("(", "");
			text = text.Replace(")", "");
			try
			{
				double num = System.Convert.ToDouble(text);
				object result = num;
				return result;
			}
			catch
			{
			}
			expr = this.PrepareExpr(expr);
			for (int i = 0; i < this.mathsFuncs.Count<string>(); i++)
			{
				expr = expr.Replace(this.mathsFuncs[i] + "(", "Math." + this.mathsFuncs[i] + "(");
			}
			for (int j = 0; j < this.mathsProps.Count<string>(); j++)
			{
				expr = expr.Replace(this.mathsProps[j], "Math." + this.mathsProps[j]);
			}
			expr = expr.Replace("Math.Math.", "Math.");
			object result2 = null;
			try
			{
				result2 = Eval.JScriptEvaluate(this.mathsJS + expr, this.Engine);
			}
			catch (Exception)
			{
				object result = 0;
				return result;
			}
			return result2;
		}
		private string GetMathsJS()
		{
			string result = string.Empty;
			if (this._jsMathPath != string.Empty)
			{
				try
				{
					result = File.ReadAllText(this._jsMathPath);
				}
				catch
				{
				}
			}
			return result;
		}
		private bool IsOperand(char item)
		{
			return item >= '0' && item <= '9';
		}
		private int GetNextTokenBack(string expr, int startIndex)
		{
			for (int i = startIndex; i >= 0; i--)
			{
				char c = expr[i];
				if (!this.IsOperand(c) && c != '.')
				{
					return i + 1;
				}
			}
			return 0;
		}
		private int GetNextTokenForward(string expr, int startIndex)
		{
			int i;
			for (i = startIndex; i < expr.Length; i++)
			{
				char c = expr[i];
				if (!this.IsOperand(c) && c != '.')
				{
					return i;
				}
			}
			return i;
		}
		private int findConsecutivesBack(string expr, int startIndex, char pattern)
		{
			int num = startIndex;
			while (num >= 0 && expr[num] == pattern)
			{
				num--;
			}
			if (num == startIndex)
			{
				num = -1;
			}
			if (num >= 0)
			{
				num++;
			}
			return num;
		}
		private int findConsecutivesForward(string expr, int startIndex, char pattern)
		{
			int num = startIndex;
			while (num < expr.Length && expr[num] == pattern)
			{
				num++;
			}
			if (num == startIndex)
			{
				num = -1;
			}
			return num;
		}
		private int findClosingLBracket(string expr, int startIndex)
		{
			int num = startIndex;
			int num2 = 1;
			if (expr[startIndex] != ')')
			{
				return -1;
			}
			while (num2 != 0 && num >= 0)
			{
				num--;
				if (expr[num] == '(')
				{
					num2--;
				}
				else
				{
					if (expr[num] == ')')
					{
						num2++;
					}
				}
			}
			if (expr[num] != '(')
			{
				return -1;
			}
			return num;
		}
		private int findClosingRBracket(string expr, int startIndex)
		{
			int num = startIndex;
			int num2 = 1;
			if ('(' != expr[startIndex])
			{
				return -1;
			}
			while (num2 != 0 && num < expr.Length)
			{
				num++;
				if (')' == expr[num])
				{
					num2--;
				}
				else
				{
					if ('(' == expr[num])
					{
						num2++;
					}
				}
			}
			if (')' != expr[num])
			{
				return -1;
			}
			return num + 1;
		}
		private string PrepareExpr(string expr)
		{
			try
			{
				int num;
				while ((num = expr.IndexOf("^")) >= 0)
				{
					int num2 = num - 1;
					int num3 = num2 - 1;
					int num4 = num + 1;
					if (num <= 0)
					{
						string result = expr;
						return result;
					}
					num2 = num - 1;
					num3 = this.findClosingLBracket(expr, num2);
					int num5;
					if (num3 < 0)
					{
						num5 = this.GetNextTokenBack(expr, num - 1);
					}
					else
					{
						if (num3 >= num2)
						{
							string result = "";
							return result;
						}
						num5 = num3;
					}
					num3 = num + 1;
					num2 = this.findClosingRBracket(expr, num3);
					if (num2 < 0)
					{
						num4 = this.GetNextTokenForward(expr, num + 1);
					}
					else
					{
						if (num2 <= num3)
						{
							string result = "";
							return result;
						}
						num4 = num2;
					}
					if (num5 < 0 || num4 < 0)
					{
						string result = expr;
						return result;
					}
					string text = expr.Substring(num5, num - num5);
					string text2 = expr.Substring(num + 1, num4 - num - 1);
					expr = string.Concat(new string[]
					{
						expr.Substring(0, num5),
						"Math.pow(",
						text,
						", ",
						text2,
						")",
						expr.Substring(num4, expr.Length)
					});
				}
			}
			catch
			{
			}
			return expr;
		}
	}
}
