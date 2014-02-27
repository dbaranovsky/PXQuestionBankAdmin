using IPEQEval;
using IPEQGif;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
namespace HTS
{
	public class CUtils
	{
		public static string getXElementAttribureValue(XElement xEl, string attrName, string sdefault)
		{
			string result = sdefault;
			try
			{
				XAttribute xAttribute = xEl.Attribute(attrName);
				if (xAttribute != null && xAttribute.Value != "auto")
				{
					result = xAttribute.Value;
				}
			}
			catch
			{
			}
			return result;
		}
		public static void removeXElementAttribure(XElement xEl, string attrName)
		{
			try
			{
				xEl.SetAttributeValue(attrName, null);
			}
			catch
			{
			}
		}
		public static void setXElementAttribureValue(XElement xEl, string attrName, string svalue)
		{
			try
			{
				XAttribute xAttribute = xEl.Attribute(attrName);
				if (xAttribute != null)
				{
					xAttribute.Value = svalue;
				}
			}
			catch
			{
			}
		}
		public static eVar_Type varTypeFromString(string t)
		{
			string key;
			eVar_Type result;
			switch (key = t.ToLower())
			{
			case "math":
				result = eVar_Type.eVar_Math;
				return result;
			case "text":
				result = eVar_Type.eVar_Text;
				return result;
			case "numarray":
				result = eVar_Type.eVar_NumericArray;
				return result;
			case "matharray":
				result = eVar_Type.eVar_MathArray;
				return result;
			case "textarray":
				result = eVar_Type.eVar_TextArray;
				return result;
			}
			result = eVar_Type.eVar_Numeric;
			return result;
		}
		public static string varTypeNameToString(eVar_Type vartype)
		{
			string result = "numeric";
			switch (vartype)
			{
			case eVar_Type.eVar_Math:
				result = "math";
				break;
			case eVar_Type.eVar_Text:
				result = "text";
				break;
			case eVar_Type.eVar_NumericArray:
				result = "numarray";
				break;
			case eVar_Type.eVar_MathArray:
				result = "matharray";
				break;
			case eVar_Type.eVar_TextArray:
				result = "textarray";
				break;
			}
			return result;
		}
		public static eVar_ConditionType getConditionType(string scType)
		{
			string key;
			eVar_ConditionType result;
			switch (key = scType.ToLower())
			{
			case "lt":
				result = eVar_ConditionType.eVarCT_LT;
				return result;
			case "le":
				result = eVar_ConditionType.eVarCT_LE;
				return result;
			case "ge":
				result = eVar_ConditionType.eVarCT_GE;
				return result;
			case "gt":
				result = eVar_ConditionType.eVarCT_GT;
				return result;
			case "ne":
			case "neq":
				result = eVar_ConditionType.eVarCT_NE;
				return result;
			}
			result = eVar_ConditionType.eVarCT_EQ;
			return result;
		}
		public static string getRandomValueFromInclusion(ArrayList inclusions)
		{
			string arg_05_0 = string.Empty;
			Random random = new Random();
			int index = random.Next(inclusions.Count);
			return (string)inclusions[index];
		}
		public static double formatNumber(double num, string format)
		{
			if (format == string.Empty)
			{
				return num;
			}
			int num2 = format.IndexOf(".", 0);
			double num5;
			if (num2 != -1)
			{
				int num3 = format.Length - num2 - 1;
				int num4 = (int)Math.Pow(10.0, (double)num3);
				num *= (double)num4;
				num5 = Math.Round(num);
				if (Math.Abs(num - Math.Floor(num)) >= 0.5 && num > num5)
				{
					num5 += 1.0;
				}
				num5 /= (double)num4;
			}
			else
			{
				num5 = Math.Round(num);
			}
			return num5;
		}
		public static string formatNumber(string snum, string format)
		{
			try
			{
				double num = Convert.ToDouble(snum);
				return CUtils.formatNumber(num, format).ToString();
			}
			catch
			{
			}
			return "0";
		}
		public static string minusCheck(string numStr)
		{
			if (numStr.Length > 0)
			{
				char arg_13_0 = numStr[0];
			}
			return numStr;
		}
		public static double getNumberByCondition(double start, double end, string format)
		{
			double num;
			if (start == end)
			{
				num = start;
			}
			else
			{
				num = CConstraint.random.NextDouble() * (end - start) + start;
			}
			return CUtils.formatNumber(num, format);
		}
		public static string reduceMath(string math)
		{
			if (math != "")
			{
				IPEqGifServerClass iPEqGifServerClass = new IPEqGifServerClass();
				string reduced = iPEqGifServerClass.GetReduced(math);
				if (reduced != "ERROR!")
				{
					math = reduced;
				}
			}
			return math;
		}
		public static string getFormulaStyle(string math, bool isExpr, bool isEnterField)
		{
			string result = "";
			if (math != "")
			{
				IPEqGifServerClass iPEqGifServerClass = new IPEqGifServerClass();
				if (isExpr)
				{
					iPEqGifServerClass.ExprText = math;
					iPEqGifServerClass.EqText = "";
				}
				else
				{
					iPEqGifServerClass.EqText = math;
					iPEqGifServerClass.ExprText = "";
				}
				string imageInfo = iPEqGifServerClass.GetImageInfo();
				int num = 0;
				int num2 = 0;
				string[] array = imageInfo.Split(new char[]
				{
					';'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].StartsWith("baseline:"))
					{
						try
						{
							num = Convert.ToInt32(array[i].Substring(9));
						}
						catch
						{
						}
					}
					if (array[i].StartsWith("height:"))
					{
						try
						{
							num2 = Convert.ToInt32(array[i].Substring(7));
						}
						catch
						{
						}
					}
				}
				result = "MARGIN-BOTTOM: " + (num - num2 - (isEnterField ? 12 : 0)).ToString() + "px; VERTICAL-ALIGN: baseline";
			}
			return result;
		}
		public static string evalMath(string correctAnsw, string userAnsw, string answerRule)
		{
			IpEqEvaluatorClass ipEqEvaluatorClass = new IpEqEvaluatorClass();
			ipEqEvaluatorClass.ExprText1 = correctAnsw;
			ipEqEvaluatorClass.ExprText2 = userAnsw;
			int answerRuleID = 0;
			int listOptions = 0;
			if (answerRule == "similar")
			{
				answerRuleID = 1;
			}
			if (answerRule == "nodecimal")
			{
				answerRuleID = 10;
			}
			if (answerRule == "exact")
			{
				answerRuleID = 2;
			}
			if (answerRule == "list")
			{
				listOptions = 1;
			}
			if (answerRule == "ordered_list")
			{
				listOptions = 2;
			}
			if (answerRule == "ordered list")
			{
				listOptions = 2;
			}
			ipEqEvaluatorClass.AnswerRuleID = answerRuleID;
			ipEqEvaluatorClass.ListOptions = listOptions;
			return ipEqEvaluatorClass.Evaluate();
		}
		public static string removeBlanks(string str)
		{
			return str.Replace(" ", "");
		}
		public static string mergeBlanks(string str)
		{
			str = str.Trim();
			int length;
			do
			{
				length = str.Length;
				str = str.Replace("  ", " ");
			}
			while (length != str.Length);
			return str;
		}
		public static string detectBrackets(string p)
		{
			string str = "0";
			string str2 = "0";
			if (p[0] == '(')
			{
				str = "(";
			}
			if (p[0] == '[')
			{
				str = "[";
			}
			if (p[p.Length - 1] == ')')
			{
				str2 = ")";
			}
			if (p[p.Length - 1] == ']')
			{
				str2 = "]";
			}
			return str + str2;
		}
		public static string removeBrackets(string p)
		{
			if (p[0] == '(' || p[0] == '[')
			{
				p = p.Substring(1, p.Length - 1);
			}
			if (p[p.Length - 1] == ')' || p[p.Length - 1] == ']')
			{
				p = p.Substring(0, p.Length - 1);
			}
			return p;
		}
		public static string jsScriptTag(string jsScriptSrc)
		{
			return "<script src='" + jsScriptSrc + "' type='text/javascript'></script>";
		}
		public static string Encrypt(string dataToEncrypt, string password, string sign)
		{
			AesManaged aesManaged = null;
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			string result;
			try
			{
				Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(sign));
				aesManaged = new AesManaged();
				aesManaged.Key = rfc2898DeriveBytes.GetBytes(aesManaged.KeySize / 8);
				aesManaged.IV = rfc2898DeriveBytes.GetBytes(aesManaged.BlockSize / 8);
				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, aesManaged.CreateEncryptor(), CryptoStreamMode.Write);
				byte[] bytes = Encoding.UTF8.GetBytes(dataToEncrypt);
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			finally
			{
				if (cryptoStream != null)
				{
					cryptoStream.Close();
				}
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
				if (aesManaged != null)
				{
					aesManaged.Clear();
				}
			}
			return result;
		}
		public static string Decrypt(string dataToDecrypt, string password, string sign)
		{
			AesManaged aesManaged = null;
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			string result;
			try
			{
				Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(sign));
				aesManaged = new AesManaged();
				aesManaged.Key = rfc2898DeriveBytes.GetBytes(aesManaged.KeySize / 8);
				aesManaged.IV = rfc2898DeriveBytes.GetBytes(aesManaged.BlockSize / 8);
				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, aesManaged.CreateDecryptor(), CryptoStreamMode.Write);
				byte[] array = Convert.FromBase64String(dataToDecrypt);
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.FlushFinalBlock();
				byte[] array2 = memoryStream.ToArray();
				result = Encoding.UTF8.GetString(array2, 0, array2.Length);
			}
			catch
			{
				result = "<usersave></usersave>";
			}
			finally
			{
				try
				{
					if (cryptoStream != null)
					{
						cryptoStream.Close();
					}
				}
				catch
				{
				}
				try
				{
					if (memoryStream != null)
					{
						memoryStream.Close();
					}
				}
				catch
				{
				}
				if (aesManaged != null)
				{
					aesManaged.Clear();
				}
			}
			return result;
		}
	}
}
