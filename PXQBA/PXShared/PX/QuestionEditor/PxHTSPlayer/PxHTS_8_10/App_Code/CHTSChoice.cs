using System;
using System.Xml.Linq;
namespace HTS
{
	public class CHTSChoice
	{
		internal string _choiceid = "";
		internal int userAnswer = -1;
		internal int _intchoiceid = -1;
		internal string _fixed = "";
		internal string _choiceText = "";
		internal string _mcid = "";
		internal int origOrder;
		internal int scrambleOrder;
		internal bool numberIsoccupied;
		private string stepID = "step0";
		private CHTSMultiChoice _MC;
		public CHTSChoice(XElement choice, int order, string pstepID, CHTSMultiChoice MC)
		{
			this._MC = MC;
			this.stepID = pstepID;
			this._choiceid = CUtils.getXElementAttribureValue(choice, "choiceid", "0");
			this._fixed = CUtils.getXElementAttribureValue(choice, "fixed", "no");
			this._mcid = CUtils.getXElementAttribureValue(choice, "mcid", "");
			this.origOrder = order;
			if (this._fixed == "yes")
			{
				this.numberIsoccupied = true;
				this.scrambleOrder = this.origOrder;
			}
			try
			{
				this._intchoiceid = Convert.ToInt32(this._MC._problem.doCalculate(this._choiceid));
			}
			catch
			{
			}
			string text = string.Concat(new string[]
			{
				"hts$QUESTIONID$.",
				this.stepID,
				".Checkbox",
				this._mcid,
				"_",
				this._intchoiceid.ToString()
			});
			string text2 = string.Concat(new string[]
			{
				"hts$QUESTIONID$.",
				this.stepID,
				".check",
				this._mcid,
				"_",
				this._intchoiceid.ToString()
			});
			choice.RemoveAttributes();
			this._choiceText = choice.ToString();
			this._choiceText = this._choiceText.Replace("<ipro_mcchoice>", "");
			this._choiceText = this._choiceText.Replace("</ipro_mcchoice>", "");
			this._choiceText = this._choiceText.Replace("<ipro_mcchoice/>", "");
			string str = "<SPAN id=\"" + text2 + "\">";
			string p_showcorrect = this._MC._problem.p_showcorrect;
			if ((p_showcorrect == "yes" || p_showcorrect == "icononly" || p_showcorrect == "inline") && this._intchoiceid == this._MC._intcorrect)
			{
				str = str + "<IMG height=\"16\" width=\"16\" src=\"" + this._MC._problem.baseUrl + "pictures/showcorrect_success.gif\" />";
			}
			string text3 = " disabled ";
			string text4 = "";
			if (this._MC._problem.p_results != "show")
			{
				text3 = string.Concat(new string[]
				{
					" onclick=\"javascript:mult_MC_Answer(",
					this._mcid,
					",",
					this._intchoiceid.ToString(),
					",'hts$QUESTIONID$.",
					this.stepID,
					".Checkbox')\""
				});
			}
			if ((this._MC._problem.p_results != "show" || this._MC._problem.p_results != "edit") && this._MC._problem.checkUserMC(this, this.stepID))
			{
				text4 = " checked ";
				if (p_showcorrect == "yes" || p_showcorrect == "icononly" || p_showcorrect == "inline")
				{
					if (this._intchoiceid == this._MC._intcorrect)
					{
						str = string.Concat(new string[]
						{
							"<SPAN id=\"",
							text2,
							"\"><IMG height=\"16\" width=\"16\" src=\"",
							this._MC._problem.baseUrl,
							"pictures/success.gif\" />"
						});
					}
					else
					{
						str = str + "<IMG height=\"16\" width=\"16\" src=\"" + this._MC._problem.baseUrl + "pictures/fail.gif\" />";
					}
				}
			}
			str += "</SPAN>";
			string str2 = string.Concat(new string[]
			{
				"<INPUT type=\"radio\" id=\"",
				text,
				"\"",
				text3,
				text4,
				" />"
			});
			this._choiceText = str + str2 + this._choiceText;
			choice.RemoveAll();
			choice.SetValue(this.getStringForSubstitute(this.origOrder));
		}
		internal string getStringForSubstitute(int order)
		{
			return string.Concat(new string[]
			{
				this.stepID,
				".CHOICE_",
				this._mcid,
				"_",
				order.ToString()
			});
		}
		internal string doScrambleSubstitute(string str)
		{
			return str.Replace("<ipro_mcchoice>" + this.getStringForSubstitute(this.scrambleOrder) + "</ipro_mcchoice>", this._choiceText);
		}
	}
}
