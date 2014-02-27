using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows.Forms;
using System.Security.Cryptography;
//using IPEQGif;
using IPEQEval;

namespace HTS
{
    public class CUtils
    {
        /// <summary>
        /// return value of xml attribute or "default" value if there is no attribute with the specified name
        /// </summary>
        /// <param name="xEl"></param>
        /// <param name="attrName"></param>
        /// <param name="sdefault"></param>
        /// <returns>value of xml attribute</returns>
        public static string getXElementAttribureValue(XElement xEl, string attrName, string sdefault)
        {
            string val = sdefault;
            try
            {
                XAttribute attr = xEl.Attribute(attrName);
                if (attr != null)
                    val = attr.Value;
            }
            catch
            {
            }
            return val;
        }
        public static void removeXElementAttribure(XElement xEl, string attrName)
        {
            try
            {
                //XAttribute attr = xEl.Attribute(attrName);
                //if (attr != null)
                //    attr.Remove;

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
                XAttribute attr = xEl.Attribute(attrName);
                if (attr != null)
                {
                    attr.Value = svalue;
                }
            }
            catch
            {
            }
        }



        public static eVar_Type varTypeFromString(string t)
        {
            eVar_Type _type = eVar_Type.eVar_Numeric;
 
            switch (t.ToLower())
            {
                case "math":
                    _type = eVar_Type.eVar_Math;
                    break;
                case "text":
                    _type = eVar_Type.eVar_Text;
                    break;
                case "numarray":
                    _type = eVar_Type.eVar_NumericArray;
                    break;
                case "matharray":
                    _type = eVar_Type.eVar_MathArray;
                    break;
                case "textarray":
                    _type = eVar_Type.eVar_TextArray;
                    break;
                case "numeric":
                default:
                    _type = eVar_Type.eVar_Numeric;
                    break;
            }
            return _type;

        }
        public static string varTypeNameToString(eVar_Type vartype)
        {
            string s = "numeric";

            switch (vartype)
            {
                case eVar_Type.eVar_Math:
                    s = "math";
                    break;
                case eVar_Type.eVar_Text:
                    s = "text";
                    break;
                case eVar_Type.eVar_NumericArray:
                    s = "numarray";
                    break;
                case eVar_Type.eVar_MathArray:
                    s = "matharray";
                    break;
                case eVar_Type.eVar_TextArray:
                    s = "textarray";
                    break;
                case eVar_Type.eVar_Numeric:
                default:
                    break;
            }
            return s;
        }
        public static eVar_ConditionType getConditionType(string scType)
        {
            eVar_ConditionType _conditionType = eVar_ConditionType.eVarCT_EQ;
            switch (scType.ToLower())
            {
                case "lt":
                    _conditionType = eVar_ConditionType.eVarCT_LT;
                    break;
                case "le":
                    _conditionType = eVar_ConditionType.eVarCT_LE;
                    break;
                case "ge":
                    _conditionType = eVar_ConditionType.eVarCT_GE;
                    break;
                case "gt":
                    _conditionType = eVar_ConditionType.eVarCT_GT;
                    break;
                case "ne":
                case "neq":
                    _conditionType = eVar_ConditionType.eVarCT_NE;
                    break;
                case "eq":
                default:
                    _conditionType = eVar_ConditionType.eVarCT_EQ;
                    break;
            }
            return _conditionType;
        }

        public static string getRandomValueFromInclusion(ArrayList inclusions)
        {
            string val = string.Empty;
            Random random = new Random();
            int index = random.Next(inclusions.Count);
            return (string)inclusions[index];
        }

        public static double formatNumber(double num , string format)
        {
			double number = 0;
            if (format == string.Empty) return num;
			int loc = format.IndexOf(".",0);
			if(loc != -1)
			{
				int powInt = format.Length - loc - 1;
				int pow  = (int)Math.Pow(10,powInt);
				num = num * pow;
				number = Math.Round(num);
                if ((Math.Abs(num - Math.Floor(num)) >= 0.5) && (num > number)) number++;
				number = number / pow;
			}
			else
			{
				number = Math.Round(num);
			}

			return number;
		}
        public static string formatNumber(string snum, string format)
        {
            double num = 0;
            try
            {
                num = Convert.ToDouble(snum);
                num = formatNumber(num, format);
                return num.ToString();
            }
            catch {}
            return "0";
        }
        
        public static string  minusCheck(string numStr )
		{
            if ((numStr.Length > 0) && (numStr[0] == '-'))
			{
				numStr = "("+ numStr + ")";
			}
			return numStr;
		}

        public static double getNumberByCondition(double start, double end, string format)
		{
			double num = 0;

			double startI = start;
			double endI = end;
			        
	        if(startI == endI)
	        {
		        num = start;
	        }
	        else
	        {
		        num = CConstraint.random.NextDouble() * (endI - startI) + startI;
	        }
	        num = CUtils.formatNumber(num,format);

            return num;
		}

        //public static string reduceMath(string math)
        //{
        //    if (math != "")
        //    {
        //        IPEQGif.IPEqGifServerClass gifServer = new IPEqGifServerClass();
        //        string reduced = gifServer.GetReduced(math);
        //        if (reduced != "ERROR!")
        //        {
        //            math = reduced;
        //        }
        //    }
        //    return math;
        //}

        //public static string getFormulaStyle(string math, bool isExpr)
        //{
        //    string style = "";
        //    if (math != "")
        //    {
        //        IPEQGif.IPEqGifServerClass gifServer = new IPEqGifServerClass();
        //        if (isExpr)
        //        {
        //            gifServer.ExprText = math;
        //            gifServer.EqText = "";
        //        }
        //        else
        //        {
        //            gifServer.EqText = math;
        //            gifServer.ExprText = "";
        //        }

        //        string measures = gifServer.GetImageInfo();
        //        int baseline = 0;
        //        int height = 0;
        //        string[] info = measures.Split(new char[] { ';' });
        //        for (int i = 0; i < info.Length; i++)
        //        {
        //            if (info[i].StartsWith("baseline:"))
        //            {
        //                try
        //                {
        //                    baseline= Convert.ToInt32(info[i].Substring(9));
        //                }
        //                catch { }
        //            }
        //            if (info[i].StartsWith("height:"))
        //            {
        //                try
        //                {
        //                    height = Convert.ToInt32(info[i].Substring(7));
        //                }
        //                catch { }

        //            }
        //        }
        //        style = "MARGIN-BOTTOM: " + (baseline - height).ToString() +"px; VERTICAL-ALIGN: baseline";
        //    }
        //    return style;
        //}

        public static string evalMath(string correctAnsw, string userAnsw, string answerRule)
        {
            IPEQEval.IpEqEvaluatorClass evalServer = new IpEqEvaluatorClass();
            evalServer.ExprText1 = correctAnsw;
            evalServer.ExprText2 = userAnsw;
            int answerRuleID = 0;
            int listOption = 0;
            if (answerRule == "similar") answerRuleID = 1; 
            if (answerRule == "nodecimal") answerRuleID=10;
            if (answerRule == "exact") answerRuleID=2;
            if (answerRule == "list") listOption = 1;
            if (answerRule == "ordered_list") listOption = 2;
            if (answerRule == "ordered list") listOption = 2;
            evalServer.AnswerRuleID = answerRuleID;
            evalServer.ListOptions = listOption;
            string result =  evalServer.Evaluate();
            return result;
        }

        public static string removeBlanks(string str)
        {
            return str.Replace(" ", "");
        }

        public static string mergeBlanks(string str)
        {
            str = str.Trim();
            int len = 0;
            do
            {
                len = str.Length;
                str = str.Replace("  ", " ");
            } while (len != str.Length);

            return str;
        }
		public static string detectBrackets(string p)
		{
			string leftBrackets= "0";
			string rightBrackets= "0";
			if (p[0]=='(') leftBrackets = "(";
            if (p[0] == '[') leftBrackets = "[";
            if (p[p.Length-1] == ')') rightBrackets = ")";
            if (p[p.Length - 1] == ']') rightBrackets = "]";
			
			return leftBrackets+rightBrackets;
		}

   		public static string removeBrackets(string p)
		{
			//p = strReplace(p," ","");
			if ((p[0]=='(') || (p[0]=='['))
			{
				p = p.Substring(1,p.Length-1);
			} 
			if ((p[p.Length-1]== ')') || (p[p.Length-1]== ']'))
			{
				p = p.Substring(0,(p.Length - 1));
			} 	
		 	return p;
		}
        
        public static string jsScriptTag(string jsScriptSrc)
        {
            return "<script src='" + jsScriptSrc + "' type='text/javascript'></script>";
        }

        public static string Encrypt(string dataToEncrypt, string password, string sign)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(sign));

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

                //Encrypt Data 
                byte[] data = Encoding.UTF8.GetBytes(dataToEncrypt);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                //Return Base 64 String 
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            finally
            {
                if (cryptoStream != null)
                    cryptoStream.Close();

                if (memoryStream != null)
                    memoryStream.Close();

                if (aes != null)
                    aes.Clear();
            }
        }

        public static string Decrypt(string dataToDecrypt, string password, string sign)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(sign));

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

                //Decrypt Data 
                byte[] data = Convert.FromBase64String(dataToDecrypt);
                cryptoStream.Write(data, 0, data.Length);

                cryptoStream.FlushFinalBlock();

                //Return Decrypted String 
                byte[] decryptBytes = memoryStream.ToArray();
                return Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            }
            catch { return "<usersave></usersave>"; }
            finally
            {
                try
                {
                    if (cryptoStream != null)
                        cryptoStream.Close();
                }
                catch { }
                try
                {
                    if (memoryStream != null)
                        memoryStream.Close();
                }
                catch { }

                if (aes != null)
                    aes.Clear();
            }
        }


    }
}
