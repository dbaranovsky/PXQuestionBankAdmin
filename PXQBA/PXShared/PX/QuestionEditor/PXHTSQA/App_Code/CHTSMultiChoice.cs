using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CHTSMultiChoice
	{
		internal string _elid = "";
		internal string _correct = "";
		internal int _intcorrect;
		internal string _points = "";
		internal string _scramble = "yes";
		internal string step_id = "";
		internal string _realOrder = "";
		internal CHTSProblem _problem;
		private static Random random = new Random();
		private Dictionary<string, CHTSChoice> choices = new Dictionary<string, CHTSChoice>();
		public CHTSMultiChoice(XElement MC, CHTSProblem problem)
		{
			this._problem = problem;
			this._elid = CUtils.getXElementAttribureValue(MC, "elid", "0");
			this._correct = CUtils.getXElementAttribureValue(MC, "correct", "1");
			this._points = CUtils.getXElementAttribureValue(MC, "points", "1");
			this._scramble = CUtils.getXElementAttribureValue(MC, "scramble", "yes");
			try
			{
				this._intcorrect = Convert.ToInt32(this._problem.doCalculate(this._correct));
			}
			catch
			{
			}
		}
		internal void processChoises(XElement step)
		{
			this.step_id = CUtils.getXElementAttribureValue(step, "id", "step0");
			int num = 1;
			this._realOrder = this.getSavedScramble();
			string[] array = null;
			if (this._realOrder != "")
			{
				array = this._realOrder.Split(new char[]
				{
					','
				});
			}
			int num2 = 0;
			IEnumerable<XElement> enumerable = step.XPathSelectElements("//ipro_mcchoice[@mcid=\"" + this._elid + "\"]");
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				if (array != null && enumerable.Count<XElement>() != array.Length)
				{
					array = null;
				}
				foreach (XElement current in enumerable)
				{
					try
					{
						CHTSChoice cHTSChoice = new CHTSChoice(current, num, this.step_id, this);
						this.choices.Add(cHTSChoice._choiceid, cHTSChoice);
						if (array != null)
						{
							cHTSChoice.scrambleOrder = Convert.ToInt32(array[num - 1]);
						}
						else
						{
							if (this._scramble == "no")
							{
								cHTSChoice.scrambleOrder = cHTSChoice.origOrder;
							}
						}
						if (cHTSChoice.scrambleOrder == 0)
						{
							num2++;
						}
						num++;
					}
					catch
					{
					}
				}
			}
			if (num2 != 0)
			{
				foreach (KeyValuePair<string, CHTSChoice> current2 in this.choices)
				{
					if (current2.Value.scrambleOrder == 0)
					{
						int num3 = CHTSMultiChoice.random.Next(0, num2);
						int num4 = 0;
						foreach (KeyValuePair<string, CHTSChoice> current3 in this.choices)
						{
							if (!current3.Value.numberIsoccupied)
							{
								if (num4 == num3)
								{
									current3.Value.numberIsoccupied = true;
									current2.Value.scrambleOrder = current3.Value.origOrder;
									num2--;
									break;
								}
								num4++;
							}
						}
					}
					if (num2 == 0)
					{
						break;
					}
				}
			}
		}
		public string getCorrectChoiceID()
		{
			return this._correct;
		}
		internal string getElid()
		{
			return this._elid;
		}
		internal string doSubstitute(string body)
		{
			foreach (KeyValuePair<string, CHTSChoice> current in this.choices)
			{
				body = current.Value.doScrambleSubstitute(body);
			}
			return body;
		}
		internal string getSavedScramble()
		{
			string result = "";
			if (this._problem.htsResponse != null)
			{
				XElement xElement = this._problem.htsResponse.xmlResponse.XPathSelectElement(string.Concat(new string[]
				{
					"//iproelement_mc_scrambled[@elid=\"",
					this.step_id,
					"_",
					this._elid,
					"\"]"
				}));
				if (xElement != null)
				{
					result = xElement.Value;
				}
			}
			return result;
		}
		internal string getScrambledOrder()
		{
			string text = "";
			foreach (KeyValuePair<string, CHTSChoice> current in this.choices)
			{
				if (text != "")
				{
					text += ",";
				}
				text += current.Value.scrambleOrder.ToString();
			}
			return text;
		}
		internal string getXMLScramble()
		{
			return string.Concat(new string[]
			{
				"<iproelement_mc_scrambled elid=\"",
				this.step_id,
				"_",
				this._elid,
				"\">",
				this.getScrambledOrder(),
				"</iproelement_mc_scrambled>"
			});
		}
	}
}
