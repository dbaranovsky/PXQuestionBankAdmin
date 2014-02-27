using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CCondition
	{
		private eVar_ConditionType _conditionType = eVar_ConditionType.eVarCT_EQ;
		private string _expr1 = "";
		private string _expr2 = "";
		private CVardef _vardef;
		public CCondition(XElement xEl, CVardef vardef)
		{
			this._vardef = vardef;
			string xElementAttribureValue = CUtils.getXElementAttribureValue(xEl, "type", "gt");
			this._conditionType = CUtils.getConditionType(xElementAttribureValue);
			IEnumerable<XElement> enumerable = xEl.XPathSelectElements("./expr");
			if (enumerable != null)
			{
				XElement[] array = enumerable.ToArray<XElement>();
				this._expr1 = array[0].Value;
				this._expr2 = array[1].Value;
			}
		}
		public bool doCheck()
		{
			string text = this._expr1;
			text = this._vardef._problem.exprReplace(text);
			text = this._vardef.doCalculate(text);
			double num = Convert.ToDouble(text);
			text = this._expr2;
			text = this._vardef._problem.exprReplace(text);
			text = this._vardef.doCalculate(text);
			double num2 = Convert.ToDouble(text);
			switch (this._conditionType)
			{
			case eVar_ConditionType.eVarCT_EQ:
				return num == num2;
			case eVar_ConditionType.eVarCT_NE:
				return num != num2;
			case eVar_ConditionType.eVarCT_LT:
				return num < num2;
			case eVar_ConditionType.eVarCT_LE:
				return num <= num2;
			case eVar_ConditionType.eVarCT_GT:
				return num > num2;
			case eVar_ConditionType.eVarCT_GE:
				return num >= num2;
			default:
				return false;
			}
		}
	}
}
