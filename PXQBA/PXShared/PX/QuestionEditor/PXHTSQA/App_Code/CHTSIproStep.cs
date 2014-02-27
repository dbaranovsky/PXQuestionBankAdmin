using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CHTSIproStep
	{
		internal CHTSProblem _problem;
		private Dictionary<string, CHTSIproNav> ipronavs = new Dictionary<string, CHTSIproNav>();
		internal string stepID = "";
		internal bool b_syntaxcheckNeeded;
		internal Dictionary<string, CHTSIproshort> elemsForCheck = new Dictionary<string, CHTSIproshort>();
		public CHTSIproStep(CHTSProblem problem)
		{
			this._problem = problem;
		}
		public void processStep(XElement xEl)
		{
			this.stepID = CUtils.getXElementAttribureValue(xEl, "id", "step0");
			IEnumerable<XElement> enumerable = xEl.XPathSelectElements(".//ipronav");
			foreach (XElement current in enumerable)
			{
				string xElementAttribureValue = CUtils.getXElementAttribureValue(current, "navtype", "");
				string xElementAttribureValue2 = CUtils.getXElementAttribureValue(current, "stepid", "");
				if (xElementAttribureValue2 == "")
				{
					xElementAttribureValue2 = CUtils.getXElementAttribureValue(current, "next", "");
				}
				try
				{
					CHTSIproNav cHTSIproNav = new CHTSIproNav(xElementAttribureValue, xElementAttribureValue2);
					this.ipronavs.Add(cHTSIproNav.navtype, cHTSIproNav);
					if (xElementAttribureValue == "correct" || xElementAttribureValue == "incorrect")
					{
						if (this._problem.p_showfeedback != "yes")
						{
							IEnumerable<XElement> enumerable2 = this._problem.htsProblem.Root.XPathSelectElements(".//iprostep[@id=\"" + xElementAttribureValue2 + "\"]");
							using (IEnumerator<XElement> enumerator2 = enumerable2.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									XElement current2 = enumerator2.Current;
									current2.SetAttributeValue("del", "yes");
								}
								continue;
							}
						}
						Point stepScore = this._problem.getStepScore(xEl);
						if ((stepScore.X == stepScore.Y && xElementAttribureValue == "incorrect") || (stepScore.X != stepScore.Y && xElementAttribureValue == "correct"))
						{
							IEnumerable<XElement> enumerable3 = this._problem.htsProblem.Root.XPathSelectElements(".//iprostep[@id=\"" + xElementAttribureValue2 + "\"]");
							foreach (XElement current3 in enumerable3)
							{
								current3.SetAttributeValue("del", "yes");
							}
						}
					}
				}
				catch
				{
				}
			}
			enumerable.Remove<XElement>();
			IEnumerable<XElement> enumerable4 = xEl.XPathSelectElements(".//input");
			foreach (XElement current4 in enumerable4)
			{
				string xElementAttribureValue3 = CUtils.getXElementAttribureValue(current4, "class", "");
				CHTSIproNav ipronav = this.getIpronav(xElementAttribureValue3);
				if (ipronav == null)
				{
					current4.SetAttributeValue("del", "yes");
				}
				else
				{
					if (xElementAttribureValue3 == "cq_hts_next")
					{
						if (!this._problem.showAllSteps)
						{
							if (ipronav != null)
							{
								current4.SetAttributeValue("id", "hts$QUESTIONID$." + this.stepID + "." + xElementAttribureValue3);
								current4.SetAttributeValue("onclick", string.Concat(new string[]
								{
									"javascript:next('hts$QUESTIONID$.",
									this.stepID,
									".",
									xElementAttribureValue3,
									"','hts$QUESTIONID$.",
									ipronav.nextStepID,
									"')"
								}));
							}
						}
						else
						{
							current4.SetAttributeValue("del", "yes");
						}
					}
					else
					{
						if (xElementAttribureValue3 == "cq_hts_hint")
						{
							if (this._problem.p_hints == "yes" && !this._problem.showAllSteps)
							{
								if (ipronav != null)
								{
									current4.SetAttributeValue("id", "hts$QUESTIONID$." + this.stepID + "." + xElementAttribureValue3);
									current4.SetAttributeValue("onclick", string.Concat(new string[]
									{
										"javascript:next('hts$QUESTIONID$.",
										this.stepID,
										".",
										xElementAttribureValue3,
										"','hts$QUESTIONID$.",
										ipronav.nextStepID,
										"')"
									}));
								}
							}
							else
							{
								current4.SetAttributeValue("del", "yes");
							}
						}
					}
				}
			}
			enumerable4 = xEl.XPathSelectElements(".//input[@del=\"yes\"]");
			enumerable4.Remove<XElement>();
		}
		private CHTSIproNav getIpronav(string navtype)
		{
			CHTSIproNav result = null;
			foreach (KeyValuePair<string, CHTSIproNav> current in this.ipronavs)
			{
				if (current.Value.navtype == navtype || "cq_hts_" + current.Value.navtype == navtype)
				{
					result = current.Value;
					break;
				}
			}
			return result;
		}
	}
}
