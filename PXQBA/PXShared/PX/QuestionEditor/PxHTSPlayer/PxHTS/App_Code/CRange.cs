using System;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CRange
	{
		private eVar_ConditionType _conditionType = eVar_ConditionType.eVarCT_GT;
		private string _expr = "";
		private CVardef _vardef;
		public string expression
		{
			get
			{
				return this._expr;
			}
			set
			{
				this._expr = value;
			}
		}
		public eVar_ConditionType conditionType
		{
			get
			{
				return this._conditionType;
			}
			set
			{
				this._conditionType = value;
			}
		}
		public CRange(XElement xEl, CVardef vardef)
		{
			this._vardef = vardef;
			string xElementAttribureValue = CUtils.getXElementAttribureValue(xEl, "type", "gt");
			this._conditionType = CUtils.getConditionType(xElementAttribureValue);
			XElement xElement = xEl.XPathSelectElement("./expr");
			this._expr = ((xElement != null) ? xElement.Value : "0");
		}
		public CRange(eVar_ConditionType conditionType, string _conditionExpr, CVardef vardef)
		{
			this._vardef = vardef;
			this._expr = _conditionExpr;
			this._conditionType = conditionType;
		}
		public bool doCheck(double val)
		{
			double num = 0.0;
			string text = this._expr;
			try
			{
				text = this._vardef._problem.exprReplace(text);
				text = this._vardef.doCalculate(text);
				num = Convert.ToDouble(text);
			}
			catch
			{
			}
			switch (this._conditionType)
			{
			case eVar_ConditionType.eVarCT_LT:
				return val < num;
			case eVar_ConditionType.eVarCT_LE:
				return val <= num;
			case eVar_ConditionType.eVarCT_GT:
				return val > num;
			case eVar_ConditionType.eVarCT_GE:
				return val >= num;
			default:
				return false;
			}
		}
	}
}
