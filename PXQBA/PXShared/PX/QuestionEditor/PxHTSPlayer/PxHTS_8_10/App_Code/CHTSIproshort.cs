using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CHTSIproshort
	{
		internal CHTSIproStep _step;
		internal string _elid = "";
		internal string _correct = "";
		internal string _format = "";
		internal string _tolerance = "";
		internal string _type = "";
		internal string _rule = "any";
		internal string _points = "1";
		internal string _allowedwords = "";
		internal string _checksyntax = "";
		internal string _size = "";
		internal string _tolerancepercent = "";
		private Dictionary<int, CCorrect> answers = new Dictionary<int, CCorrect>();
		public CHTSIproshort(XElement xEl, CHTSIproStep step)
		{
			this._step = step;
			this._elid = CUtils.getXElementAttribureValue(xEl, "elid", "1");
			this._format = CUtils.getXElementAttribureValue(xEl, "format", "");
			this._type = CUtils.getXElementAttribureValue(xEl, "type", "numeric");
			this._tolerance = CUtils.getXElementAttribureValue(xEl, "tolerance", "0.02");
			this._rule = CUtils.getXElementAttribureValue(xEl, "answerrule", "any");
			this._correct = CUtils.getXElementAttribureValue(xEl, "correct", "");
			this._points = CUtils.getXElementAttribureValue(xEl, "points", "1");
			this._checksyntax = CUtils.getXElementAttribureValue(xEl, "checksyntax", "on");
			this._allowedwords = CUtils.getXElementAttribureValue(xEl, "allowedwords", "");
			this._size = CUtils.getXElementAttribureValue(xEl, "size", "2");
			CCorrect value = new CCorrect(this._step._problem, xEl, this._type, this._points, this._tolerance, this._rule);
			this.answers.Add(this.answers.Count, value);
			IEnumerable<XElement> enumerable = xEl.XPathSelectElements(".//ipro_alt_correct");
			foreach (XElement current in enumerable)
			{
				CCorrect value2 = new CCorrect(this._step._problem, current, this._type, this._points, this._tolerance, this._rule);
				this.answers.Add(this.answers.Count, value2);
			}
		}
		public bool checkAnswer(string uAnswer)
		{
			bool result = false;
			foreach (KeyValuePair<int, CCorrect> current in this.answers)
			{
				if (current.Value.checkAnswer(uAnswer))
				{
					result = true;
					break;
				}
			}
			return result;
		}
		internal string getCorrectForShowCorrect()
		{
			int count = this.answers.Count;
			string text = (count > 1) ? "<br />" : "";
			string text2 = "&plusmn;";
			for (int i = 0; i < count; i++)
			{
				CCorrect cCorrect = this.answers[i];
				if (this._type == "math")
				{
					string text3 = (cCorrect._rule == "exact" || cCorrect._rule == "similar") ? "eqtext=" : "exprtext=";
					if (i > 0)
					{
						text += "<br />";
					}
					string text4 = text;
					text = string.Concat(new string[]
					{
						text4,
						"<img src=",
						this._step._problem.baseUrl,
						"geteq.ashx?",
						text3,
						HttpUtility.UrlEncode(cCorrect._correct),
						"&bottom=8 align=middle /> "
					});
				}
				else
				{
					string text5 = cCorrect._correct.Replace("'", "\\'");
					text5 = text5.Replace("\"", "\\'\\'");
					if (this._type == "numeric")
					{
						if (cCorrect.getToleranceForPopup() == "0" || cCorrect.getToleranceForPopup() == "0%")
						{
							text2 = "";
						}
						if (i > 0)
						{
							text += "<br />";
						}
						string text6 = text;
						text = string.Concat(new string[]
						{
							text6,
							text5,
							" <span style=color:Blue>(",
							text2,
							cCorrect.getToleranceForPopup(),
							")</span>"
						});
					}
					else
					{
						if (this._type == "text")
						{
							if (cCorrect.getToleranceForPopup() == "0" || cCorrect.getToleranceForPopup() == "0%")
							{
								text2 = "";
							}
							if (i > 0)
							{
								text += "<br />";
							}
							text += text5;
						}
						else
						{
							text += text5;
						}
					}
				}
			}
			return text;
		}
		internal bool checkSyntaxNeeded()
		{
			bool flag = this._type == "numeric" && (this._checksyntax == "on" || this._checksyntax == "yes" || this._checksyntax == "true");
			if (flag)
			{
				this._step.elemsForCheck.Add("hts$QUESTIONID$." + this._step.stepID + ".EnterField" + this._elid, this);
			}
			return flag;
		}
	}
}
