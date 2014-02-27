using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CVardef
	{
		private string _name = "";
		private string _format = "#";
		private eVar_Type _type = eVar_Type.eVar_Numeric;
		private ArrayList _constraint = new ArrayList();
		internal CHTSProblem _problem;
		private object _value = "";
		private bool validFlag;
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}
		public eVar_Type varType
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}
		public string Format
		{
			get
			{
				return this._format;
			}
			set
			{
				this._format = value;
			}
		}
		public object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}
		public CVardef(string name, CHTSProblem problem)
		{
			this._name = name;
			this._problem = problem;
		}
		public CVardef(XElement xEl, CHTSProblem problem)
		{
			this._problem = problem;
			this._name = CUtils.getXElementAttribureValue(xEl, "name", string.Empty);
			this._format = CUtils.getXElementAttribureValue(xEl, "format", "#");
			string xElementAttribureValue = CUtils.getXElementAttribureValue(xEl, "type", "numeric");
			this._type = CUtils.varTypeFromString(xElementAttribureValue);
			this.initValue();
			this.getConstraints(xEl);
		}
		public CVardef(XElement xEl, CHTSResponse response)
		{
			this._name = CUtils.getXElementAttribureValue(xEl, "name", string.Empty);
			string xElementAttribureValue = CUtils.getXElementAttribureValue(xEl, "type", "numeric");
			this._type = CUtils.varTypeFromString(xElementAttribureValue);
			this.initValue();
			switch (this._type)
			{
			case eVar_Type.eVar_NumericArray:
			case eVar_Type.eVar_MathArray:
			case eVar_Type.eVar_TextArray:
			{
				IEnumerable<XElement> enumerable = xEl.XPathSelectElements("./value");
				using (IEnumerator<XElement> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XElement current = enumerator.Current;
						(this._value as StringCollection).Add(current.Value);
					}
					return;
				}
				break;
			}
			}
			this._value = xEl.XPathSelectElement("value").Value;
		}
		private void initValue()
		{
			switch (this._type)
			{
			case eVar_Type.eVar_Numeric:
				this._value = "0";
				return;
			case eVar_Type.eVar_NumericArray:
			case eVar_Type.eVar_MathArray:
			case eVar_Type.eVar_TextArray:
				this._value = new StringCollection();
				return;
			}
			this._value = "";
		}
		private string typeToString()
		{
			return CUtils.varTypeNameToString(this._type);
		}
		private void getConstraints(XElement xEl)
		{
			IEnumerable<XElement> enumerable = xEl.XPathSelectElements("./constraint");
			foreach (XElement current in enumerable)
			{
				CConstraint value = new CConstraint(current, this);
				this._constraint.Add(value);
			}
		}
		internal bool doVariable()
		{
			Dictionary<string, CVardef> arg_0B_0 = this._problem.vardefs;
			switch (this._type)
			{
			case eVar_Type.eVar_Numeric:
				this._value = this.getNumericVardefValue();
				break;
			case eVar_Type.eVar_Math:
				this._value = this.getMathVardefValue();
				break;
			case eVar_Type.eVar_Text:
				this._value = this.getTextVardefValue();
				break;
			case eVar_Type.eVar_NumericArray:
				this._value = this.getNumericArrayVardefValue();
				break;
			case eVar_Type.eVar_MathArray:
				this._value = this.getMathArrayVardefValue();
				break;
			case eVar_Type.eVar_TextArray:
				this._value = this.getTextArrayVardefValue();
				break;
			}
			Console.WriteLine("var " + this._name + " = " + this._value.ToString());
			return this.validFlag;
		}
		public string toXMLString()
		{
			if (this._value == null)
			{
				return "<vardef></vardef>";
			}
			string text = string.Concat(new string[]
			{
				"<vardef name=\"",
				this._name,
				"\" type=\"",
				this.typeToString(),
				"\" >"
			});
			switch (this._type)
			{
			case eVar_Type.eVar_NumericArray:
			case eVar_Type.eVar_MathArray:
			case eVar_Type.eVar_TextArray:
			{
				StringCollection stringCollection = this._value as StringCollection;
				if (stringCollection != null)
				{
					for (int i = 0; i < stringCollection.Count; i++)
					{
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							"<value id=\"",
							i.ToString(),
							"\">",
							stringCollection[i],
							"</value>"
						});
					}
					goto IL_FB;
				}
				goto IL_FB;
			}
			}
			text = text + "<value id=\"1\">" + this._value.ToString() + "</value>";
			IL_FB:
			return text + "</vardef>";
		}
		public string valueToString()
		{
			if (this._value == null)
			{
				return "null";
			}
			string text = string.Empty;
			switch (this._type)
			{
			case eVar_Type.eVar_NumericArray:
			case eVar_Type.eVar_MathArray:
			case eVar_Type.eVar_TextArray:
			{
				StringCollection stringCollection = this._value as StringCollection;
				if (stringCollection == null)
				{
					return "null";
				}
				for (int i = 0; i < stringCollection.Count; i++)
				{
					if (i != 0)
					{
						text += ",";
					}
					text += stringCollection[i];
				}
				return text;
			}
			}
			return this._value.ToString();
		}
		internal string getNumericVardefValue()
		{
			this.validFlag = false;
			string text = "0";
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this._constraint.Count; i++)
			{
				CConstraint cConstraint = (CConstraint)this._constraint[i];
				if (cConstraint.isRangesPresent())
				{
					for (int j = 0; j < cConstraint._ranges.Count; j++)
					{
						CRange cRange = (CRange)cConstraint._ranges[j];
						string text2 = cRange.expression;
						text2 = this.minusCheck(text2);
						text2 = this._problem.exprReplace(text2);
						text = this.doCalculate(text2);
						CRange value = new CRange(cRange.conditionType, text, this);
						arrayList.Add(value);
					}
				}
				if (cConstraint.isInclusionsPresent() && !cConstraint.isConditionPresent())
				{
					string text3 = cConstraint.getRandomValueFromInclusion();
					text3 = this._problem.exprReplace(text3);
					text3 = this.minusCheck(text3);
					text = this.doCalculate(text3);
					if (this.checkRanges(Convert.ToDouble(text), arrayList))
					{
						this.validFlag = this.checkExclusion(text, cConstraint._exclusions, this._format);
					}
				}
				else
				{
					if (cConstraint.isRangesPresent() && !cConstraint.isConditionPresent())
					{
						int num = 0;
						while (!this.validFlag)
						{
							if (num >= 50)
							{
								break;
							}
							try
							{
								text = cConstraint.getRandomValueFromRange(this._format, arrayList);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							this.validFlag = this.checkExclusion(text, cConstraint._exclusions, this._format);
							if (!this.validFlag)
							{
								num++;
							}
							else
							{
								Console.WriteLine("~" + this._name + "\\ : " + text);
							}
						}
					}
					else
					{
						if (cConstraint.isInclusionsPresent() && cConstraint.isConditionPresent())
						{
							string text4 = cConstraint.getRandomValueFromInclusion();
							text4 = this._problem.exprReplace(text4);
							text4 = this.minusCheck(text4);
							if (cConstraint._condition.doCheck())
							{
								text = this.doCalculate(text4);
							}
						}
					}
				}
			}
			return text;
		}
		private StringCollection getNumericArrayVardefValue()
		{
			StringCollection stringCollection = new StringCollection();
			for (int i = 0; i < this._constraint.Count; i++)
			{
				CConstraint cConstraint = (CConstraint)this._constraint[i];
				for (int j = 0; j < cConstraint._inclusions.Count; j++)
				{
					string text = cConstraint._inclusions[j];
					text = this._problem.exprReplace(text);
					text = this.minusCheck(text);
					object obj = this.doCalculate(text);
					stringCollection.Add(obj.ToString());
				}
			}
			return stringCollection;
		}
		private object getMathVardefValue()
		{
			string math = this.valueToString();
			string text = "";
			for (int i = 0; i < this._constraint.Count; i++)
			{
				CConstraint cConstraint = (CConstraint)this._constraint[i];
				if (cConstraint.isInclusionsPresent())
				{
					text = cConstraint.getRandomValueFromInclusion();
					text = this._problem.exprReplace(text);
				}
				if (!cConstraint.isConditionPresent())
				{
					math = text;
				}
				else
				{
					if (cConstraint._condition.doCheck())
					{
						math = text;
					}
				}
			}
			return CUtils.reduceMath(math);
		}
		private StringCollection getMathArrayVardefValue()
		{
			StringCollection stringCollection = new StringCollection();
			for (int i = 0; i < this._constraint.Count; i++)
			{
				CConstraint cConstraint = (CConstraint)this._constraint[i];
				for (int j = 0; j < cConstraint._inclusions.Count; j++)
				{
					string text = cConstraint._inclusions[j];
					text = this._problem.exprReplace(text);
					text = CUtils.reduceMath(text);
					stringCollection.Add(text);
				}
			}
			return stringCollection;
		}
		private string getTextVardefValue()
		{
			string result = this.valueToString();
			string text = "";
			for (int i = 0; i < this._constraint.Count; i++)
			{
				CConstraint cConstraint = (CConstraint)this._constraint[i];
				if (cConstraint.isInclusionsPresent())
				{
					text = cConstraint.getRandomValueFromInclusion();
					text = this._problem.exprReplace(text);
				}
				if (!cConstraint.isConditionPresent())
				{
					result = text;
				}
				else
				{
					if (cConstraint._condition.doCheck())
					{
						result = text;
					}
				}
			}
			return result;
		}
		private StringCollection getTextArrayVardefValue()
		{
			StringCollection stringCollection = new StringCollection();
			for (int i = 0; i < this._constraint.Count; i++)
			{
				CConstraint cConstraint = (CConstraint)this._constraint[i];
				for (int j = 0; j < cConstraint._inclusions.Count; j++)
				{
					string text = cConstraint._inclusions[j];
					text = this._problem.exprReplace(text);
					stringCollection.Add(text);
				}
			}
			return stringCollection;
		}
		private bool checkExclusion(string numeric, StringCollection exclusions, string _format)
		{
			bool result = true;
			if (exclusions != null && exclusions.Count > 0)
			{
				for (int i = 0; i < exclusions.Count; i++)
				{
					string text = exclusions[i];
					text = this._problem.exprReplace(text);
					text = this.minusCheck(text);
					text = this.doCalculate(text);
					try
					{
						if (Convert.ToDouble(numeric) == Convert.ToDouble(text))
						{
							result = false;
							break;
						}
					}
					catch
					{
					}
				}
			}
			return result;
		}
		internal string doCalculate(string expr)
		{
			object obj = this._problem.doCalculate(expr);
			if (obj == null)
			{
				obj = "0";
			}
			obj = CUtils.formatNumber(obj.ToString(), this._format);
			return obj.ToString();
		}
		internal bool checkRanges(double num, ArrayList ranges)
		{
			if (ranges != null)
			{
				for (int i = 0; i < ranges.Count; i++)
				{
					CRange cRange = (CRange)ranges[i];
					if (!cRange.doCheck(num))
					{
						return false;
					}
				}
			}
			return true;
		}
		private StringCollection getArrayValue()
		{
			return this._value as StringCollection;
		}
		private string minusCheck(string expr)
		{
			if (expr != string.Empty && expr[0] == '-')
			{
				expr = "(" + expr + ")";
			}
			return expr;
		}
		public XElement getVardef()
		{
			XElement xElement = new XElement("constraint");
			if (this._type == eVar_Type.eVar_Math || this._type == eVar_Type.eVar_Text || this._type == eVar_Type.eVar_Numeric)
			{
				xElement.Add(new XElement("inclusion", new XElement("expr", this.valueToString())));
			}
			else
			{
				StringCollection arrayValue = this.getArrayValue();
				for (int i = 0; i < arrayValue.Count; i++)
				{
					xElement.Add(new XElement("inclusion", new XElement("expr", arrayValue[i])));
				}
			}
			return new XElement("vardef", new object[]
			{
				new XAttribute("name", this._name),
				new XAttribute("type", this.typeToString()),
				new XAttribute("format", this._format),
				xElement
			});
		}
	}
}
