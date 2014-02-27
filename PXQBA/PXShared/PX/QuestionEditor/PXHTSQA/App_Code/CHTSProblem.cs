using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CHTSProblem
	{
		private CHTSCalculator htsCalc;
		private Dictionary<string, CVardef> vars = new Dictionary<string, CVardef>();
		private Dictionary<string, CHTSIproStep> iprosteps = new Dictionary<string, CHTSIproStep>();
		private Dictionary<string, CHTSMultiChoice> mchoices = new Dictionary<string, CHTSMultiChoice>();
		private string sProblem;
		internal XDocument htsProblem;
		internal CHTSResponse htsResponse;
		internal string baseUrl;
		internal string maxPoints = "";
		internal StringCollection agilixAnswers;
		internal string p_results = "edit";
		internal string p_showcorrect = "yes";
		internal string p_hints = "no";
		internal string p_feedback = "no";
		internal string p_showfeedback = "no";
		internal string p_showsolution = "no";
		internal string p_showanswers = "no";
		internal string p_syntaxchecking = "on";
		internal bool showAllSteps;
		internal bool b_syntaxchecking = true;
		public Dictionary<string, CVardef> vardefs
		{
			get
			{
				return this.vars;
			}
		}
		public CHTSProblem(string jsMathPath, string bUrl)
		{
			this.baseUrl = bUrl;
			this.htsCalc = new CHTSCalculator(jsMathPath);
		}
		public void setParameters(string parameters)
		{
			string[] array = parameters.Split(new char[]
			{
				';'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'='
				});
				if (array2.Length > 1)
				{
					if (array2[0] == "results")
					{
						this.p_results = array2[1];
					}
					if (array2[0] == "showcorrect")
					{
						this.p_showcorrect = array2[1];
					}
					if (array2[0] == "feedback")
					{
						this.p_feedback = array2[1];
					}
					if (array2[0] == "showfeedback")
					{
						this.p_showfeedback = array2[1];
					}
					if (array2[0] == "showsolution")
					{
						this.p_showsolution = array2[1];
					}
					if (array2[0] == "showanswers")
					{
						this.p_showanswers = array2[1];
					}
					if (array2[0] == "hints")
					{
						this.p_hints = array2[1];
					}
					if (array2[0] == "syntaxchecking")
					{
						this.p_syntaxchecking = array2[1];
					}
				}
			}
			this.showAllSteps = (this.p_showcorrect == "yes" || this.p_showcorrect == "inline" || this.p_results == "show");
			this.b_syntaxchecking = ((this.p_results == "edit" || this.p_results == "ignore") && (this.p_syntaxchecking == "on" || this.p_syntaxchecking == "yes" || this.p_syntaxchecking == "true"));
		}
		public void doProblem(string psProblem, CHTSResponse response)
		{
			this.sProblem = psProblem;
			this.htsResponse = response;
			this.doProblem();
		}
		public void doProblem()
		{
			this.agilixAnswers = new StringCollection();
			this.mchoices = new Dictionary<string, CHTSMultiChoice>();
			this.htsProblem = XDocument.Load(new StringReader(this.sProblem));
			this.maxPoints = CUtils.getXElementAttribureValue(this.htsProblem.Root, "maxpoints", "0");
			if (this.htsResponse != null)
			{
				this.vars = this.htsResponse.vars;
			}
			else
			{
				this.getVardefs();
				this.doVariables();
			}
			this.replaceVariables();
			this.processHTSelements();
		}
		private void replaceVariables()
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (KeyValuePair<string, CVardef> current in this.vars)
				{
					CVardef value = current.Value;
					switch (value.varType)
					{
					case eVar_Type.eVar_NumericArray:
					case eVar_Type.eVar_MathArray:
					case eVar_Type.eVar_TextArray:
					{
						this.sProblem = this.sProblem.Replace("~" + value.Name + "[]\\", value.valueToString());
						this.sProblem = this.sProblem.Replace("~" + value.Name + "\\", value.valueToString());
						StringCollection stringCollection = value.Value as StringCollection;
						for (int j = 0; j < stringCollection.Count; j++)
						{
							this.sProblem = this.sProblem.Replace(string.Concat(new string[]
							{
								"~",
								value.Name,
								"[",
								(j + 1).ToString(),
								"]\\"
							}), (value.varType == eVar_Type.eVar_NumericArray) ? CUtils.minusCheck(stringCollection[j]) : stringCollection[j]);
						}
						continue;
					}
					}
					this.sProblem = this.sProblem.Replace("~" + value.Name + "\\", (value.varType == eVar_Type.eVar_Numeric) ? CUtils.minusCheck(value.valueToString()) : value.valueToString());
				}
			}
			this.htsProblem = XDocument.Load(new StringReader(this.sProblem));
			this.htsProblem.Root.SetAttributeValue("maxpoints", null);
		}
		public string varsToXmlString()
		{
			string text = "<vardefs>";
			foreach (KeyValuePair<string, CVardef> current in this.vars)
			{
				CVardef value = current.Value;
				text += value.toXMLString();
			}
			text += "</vardefs>";
			return text;
		}
		private void processHTSelements()
		{
			this.processClassAttribute();
			if (this.p_showsolution == "no")
			{
				IEnumerable<XElement> source = this.htsProblem.XPathSelectElements("//iprostep[@id=\"solution\"]");
				source.Remove<XElement>();
			}
			this.processGraphElement();
			IEnumerable<XElement> enumerable = this.htsProblem.XPathSelectElements("//iprostep");
			IEnumerable<XElement> enumerable2 = null;
			IEnumerable<XElement> enumerable3 = null;
			IEnumerable<XElement> enumerable4 = null;
			int num = 0;
			foreach (XElement current in enumerable)
			{
				string xElementAttribureValue = CUtils.getXElementAttribureValue(current, "id", "no");
				string xElementAttribureValue2 = CUtils.getXElementAttribureValue(current, "del", "no");
				enumerable3 = null;
				enumerable4 = null;
				enumerable2 = null;
				if (xElementAttribureValue2 != "yes")
				{
					CHTSIproStep cHTSIproStep = new CHTSIproStep(this);
					try
					{
						cHTSIproStep.stepID = CUtils.getXElementAttribureValue(current, "id", "step0");
						this.iprosteps.Add(cHTSIproStep.stepID, cHTSIproStep);
						cHTSIproStep.processStep(current);
					}
					catch
					{
					}
					IEnumerable<XElement> enumerable5 = current.XPathSelectElements(".//img");
					foreach (XElement current2 in enumerable5)
					{
						this.processImage(current2);
					}
					enumerable2 = current.XPathSelectElements(".//iproformula");
					foreach (XElement current3 in enumerable2)
					{
						this.processIproformula(current3);
					}
					enumerable2.Remove<XElement>();
					enumerable3 = current.XPathSelectElements(".//iproelement_short");
					foreach (XElement current4 in enumerable3)
					{
						this.processIproshort(current4, cHTSIproStep);
					}
					enumerable3.Remove<XElement>();
					enumerable4 = current.XPathSelectElements(".//iproelement_mc");
					foreach (XElement current5 in enumerable4)
					{
						this.setMCID(current5);
					}
					enumerable4 = current.XPathSelectElements(".//iproelement_mc");
					foreach (XElement current6 in enumerable4)
					{
						this.processMC(current6, current);
					}
					enumerable4.Remove<XElement>();
					current.Attributes().Remove();
					current.SetAttributeValue("id", "hts$QUESTIONID$." + xElementAttribureValue);
					if (num > 0 && !this.showAllSteps)
					{
						current.SetAttributeValue("style", "display:none");
					}
					num++;
					if (cHTSIproStep.b_syntaxcheckNeeded && cHTSIproStep.elemsForCheck.Count > 0)
					{
						string text = "";
						foreach (KeyValuePair<string, CHTSIproshort> current7 in cHTSIproStep.elemsForCheck)
						{
							text = text + ",'" + current7.Key + "'";
						}
						XElement xElement = new XElement("input");
						xElement.SetAttributeValue("class", "cq_hts_check");
						xElement.SetAttributeValue("hide", "no");
						xElement.SetAttributeValue("id", "hts$QUESTIONID$." + xElementAttribureValue + ".CheckSyntax");
						xElement.SetAttributeValue("onclick", string.Concat(new string[]
						{
							"javascript:checksyntax('hts$QUESTIONID$.",
							xElementAttribureValue,
							"'",
							text,
							")"
						}));
						xElement.SetAttributeValue("type", "button");
						xElement.SetAttributeValue("value", "Check Syntax");
						XElement xElement2 = new XElement("div");
						xElement2.SetAttributeValue("id", "hts$QUESTIONID$." + xElementAttribureValue + ".CheckSyntaxResult");
						current.Add(new XElement("p", new object[]
						{
							xElement,
							xElement2
						}));
					}
				}
			}
		}
		private void processClassAttribute()
		{
			IEnumerable<XElement> enumerable = this.htsProblem.XPathSelectElements("//*[@class]");
			foreach (XElement current in enumerable)
			{
				string text = CUtils.getXElementAttribureValue(current, "class", "");
				if (text != "" && !text.StartsWith("cq_hts_"))
				{
					text = "cq_hts_" + text;
					current.SetAttributeValue("class", text);
				}
			}
		}
		private void processGraphElement()
		{
			int num = 0;
			IEnumerable<XElement> enumerable = this.htsProblem.XPathSelectElements("//*[@class='cq_hts_graph']");
			foreach (XElement current in enumerable)
			{
				string xElementAttribureValue = CUtils.getXElementAttribureValue(current, "function", "");
				XElement xElement = new XElement("div", "FF");
				xElement.SetAttributeValue("id", "hts$QUESTIONID$.graph_" + num.ToString());
				xElement.SetAttributeValue("function", xElementAttribureValue);
				xElement.SetAttributeValue("style", "display:none");
				num++;
				current.AddAfterSelf(xElement);
			}
			enumerable.Remove<XElement>();
			num = 0;
			IEnumerable<XElement> enumerable2 = this.htsProblem.XPathSelectElements("//*[@classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000']");
			foreach (XElement current2 in enumerable2)
			{
				XElement xElement2 = current2.XPathSelectElement("//param[@name='movie']");
				if (xElement2 != null)
				{
					string text = this.baseUrl + CUtils.getXElementAttribureValue(xElement2, "value", "");
					if (text.IndexOf("?datafile=http:") < 0)
					{
						string str = "";
						text = text.Replace("?datafile=", "?datafile=" + str + "[~]/");
					}
					text = text + "&cqid=hts$QUESTIONID$.graph_" + num.ToString();
					xElement2.SetAttributeValue("value", text);
					XElement xElement3 = current2.XPathSelectElement("//embed");
					xElement3.SetAttributeValue("src", text);
				}
				num++;
			}
		}
		private void processImage(XElement image)
		{
			string text = CUtils.getXElementAttribureValue(image, "src", "");
			if (!text.StartsWith("http:"))
			{
				text = "[~]/" + text;
				image.SetAttributeValue("src", text);
			}
		}
		private void processIproformula(XElement iproFormula)
		{
			string text = iproFormula.ToString();
			text = text.Replace("<iproformula ", "<img ");
			text = text.Replace("</iproformula>", "</img>");
			XElement xElement = XElement.Load(new StringReader(text));
			string text2 = CUtils.getXElementAttribureValue(xElement, "src", "");
			string[] array = text2.Split(new char[]
			{
				'='
			}, 2);
			bool isExpr = array[1].IndexOf("exprtext=") != -1;
			xElement.SetAttributeValue("style", CUtils.getFormulaStyle(array[1], isExpr));
			xElement.SetAttributeValue("type", null);
			text2 = this.baseUrl + array[0] + "=" + HttpUtility.UrlEncode(array[1]);
			xElement.SetAttributeValue("src", text2);
			iproFormula.AddBeforeSelf(xElement);
		}
		private void processIproshort(XElement iproShort, CHTSIproStep step)
		{
			string stepID = step.stepID;
			CHTSIproshort cHTSIproshort = new CHTSIproshort(iproShort, step);
			if (this.b_syntaxchecking)
			{
				step.b_syntaxcheckNeeded = (cHTSIproshort.checkSyntaxNeeded() || step.b_syntaxcheckNeeded);
			}
			string text = "";
			string value = this.baseUrl + "pictures/mathinput.gif";
			string text2 = "hts$QUESTIONID$." + stepID + ".EnterField" + cHTSIproshort._elid;
			string inputid = stepID + ".EnterField" + cHTSIproshort._elid;
			string text3 = "hts$QUESTIONID$." + stepID + ".anchor" + cHTSIproshort._elid;
			string text4 = "";
			XElement xElement = null;
			string s;
			if (cHTSIproshort._type == "numeric" || cHTSIproshort._type == "text")
			{
				if (cHTSIproshort._type == "numeric")
				{
					this.agilixAnswers.Add("getNumericAnswer('" + text2 + "')");
				}
				if (cHTSIproshort._type == "text")
				{
					this.agilixAnswers.Add("getTextAnswer('" + text2 + "')");
				}
				if (this.p_results == "show")
				{
					text = this.getUserAnswer(inputid);
					text4 = " DISABLED=\"true\" ";
				}
				else
				{
					if (this.p_results == "edit")
					{
						text = this.getUserAnswer(inputid);
					}
				}
				if (this.p_showcorrect == "inline")
				{
					text = CUtils.getXElementAttribureValue(iproShort, "correct", "");
					text = Convert.ToString(this.doCalculate(text));
				}
				else
				{
					if (this.p_showcorrect == "icononly")
					{
						string text5 = this.checkUserAnswer(iproShort, stepID) ? "success" : "fail";
						string text6 = string.Concat(new string[]
						{
							"<A id=\"",
							text3,
							"\" name=\"",
							text3,
							"\" >"
						});
						string text7 = text6;
						text6 = string.Concat(new string[]
						{
							text7,
							"<img src=\"",
							this.baseUrl,
							"pictures/",
							text5,
							".gif\" />"
						});
						string str = "</A>";
						xElement = XElement.Load(new StringReader(text6 + str));
					}
					else
					{
						if (this.p_showcorrect == "yes")
						{
							string correctForShowCorrect = cHTSIproshort.getCorrectForShowCorrect();
							string text8 = this.checkUserAnswer(iproShort, stepID) ? "success" : "fail_input";
							string text9 = string.Concat(new string[]
							{
								"popupTextAndNum('",
								text3,
								"','",
								correctForShowCorrect,
								"')\""
							});
							string text6 = string.Concat(new string[]
							{
								"<A id=\"",
								text3,
								"\" name=\"",
								text3,
								"\" >"
							});
							string text10 = text6;
							text6 = string.Concat(new string[]
							{
								text10,
								"<![CDATA[<img onmouseover=\"",
								text9,
								"\" onmouseout=\"hidepopup()\" src=\"",
								this.baseUrl,
								"pictures/",
								text8,
								".gif\" />]]>"
							});
							string str = "</A>";
							xElement = XElement.Load(new StringReader(text6 + str));
						}
					}
				}
				string text11 = string.Concat(new string[]
				{
					"<SPAN class=\"field\"><INPUT TYPE=\"text\" onkeypress=\"javascript:onKeyPressed(event,this)\" id=\"",
					text2,
					"\" style=\"OVERFLOW: visible\" ",
					text4,
					" onchange=\"javascript:enterfieldAnswer('",
					text2,
					"'); \" size=\"",
					cHTSIproshort._size,
					"\" allowedwords=\"",
					cHTSIproshort._allowedwords,
					"\" value=\"",
					text,
					"\"/></SPAN>"
				});
				s = text11;
			}
			else
			{
				this.agilixAnswers.Add("getMathAnswer('" + text2 + "')");
				string str2 = "<A href=\"javascript:showEnterfieldAnswer('" + text2 + "');\" >";
				string str3 = "</A>";
				if (this.p_results == "show")
				{
					str2 = "";
					str3 = "";
					text = this.getUserAnswer(inputid);
				}
				else
				{
					if (this.p_results == "edit")
					{
						text = this.getUserAnswer(inputid);
					}
				}
				if (this.p_showcorrect == "inline")
				{
					text = CUtils.getXElementAttribureValue(iproShort, "correct", "");
				}
				else
				{
					if (this.p_showcorrect == "icononly")
					{
						string text12 = this.checkUserAnswer(iproShort, stepID) ? "success" : "fail";
						string text6 = string.Concat(new string[]
						{
							"<A id=\"",
							text3,
							"\" name=\"",
							text3,
							"\" >"
						});
						string text13 = text6;
						text6 = string.Concat(new string[]
						{
							text13,
							"<img src=\"",
							this.baseUrl,
							"pictures/",
							text12,
							".gif\" />"
						});
						string str = "</A>";
						xElement = XElement.Load(new StringReader(text6 + str));
					}
					else
					{
						if (this.p_showcorrect == "yes")
						{
							string correctForShowCorrect2 = cHTSIproshort.getCorrectForShowCorrect();
							string text14 = this.checkUserAnswer(iproShort, stepID) ? "success" : "fail_input";
							string text15 = string.Concat(new string[]
							{
								"popupMath('",
								text3,
								"','",
								correctForShowCorrect2,
								"')\""
							});
							string text6 = string.Concat(new string[]
							{
								"<A id=\"",
								text3,
								"\" name=\"",
								text3,
								"\" >"
							});
							string text16 = text6;
							text6 = string.Concat(new string[]
							{
								text16,
								"<![CDATA[<img onmouseover=\"",
								text15,
								"\" onmouseout=\"hidepopup()\" src=\"",
								this.baseUrl,
								"pictures/",
								text14,
								".gif\" />]]>"
							});
							string str = "</A>";
							xElement = XElement.Load(new StringReader(text6 + str));
						}
					}
				}
				if (text != string.Empty)
				{
					value = this.baseUrl + "geteq.ashx?eqtext=" + HttpUtility.UrlEncode(text) + "&doborder=true&bottom=8";
				}
				string str4 = string.Concat(new string[]
				{
					"<img id=\"",
					text2,
					"\" align=\"middle\" userAnswer=\"",
					HttpUtility.UrlEncode(text),
					"\" />"
				});
				s = str2 + str4 + str3;
			}
			try
			{
				XElement xElement2 = XElement.Load(new StringReader(s));
				if (cHTSIproshort._type == "math")
				{
					XElement xElement3 = xElement2.XPathSelectElement(".//img");
					if (xElement3 != null)
					{
						xElement3.SetAttributeValue("src", value);
					}
					else
					{
						xElement2.SetAttributeValue("src", value);
					}
				}
				iproShort.AddBeforeSelf(xElement2);
				if (xElement != null)
				{
					iproShort.AddBeforeSelf(xElement);
				}
			}
			catch (Exception)
			{
			}
		}
		private void setMCID(XElement mc)
		{
			string xElementAttribureValue = CUtils.getXElementAttribureValue(mc, "elid", "0");
			IEnumerable<XElement> enumerable = mc.XPathSelectElements("//ipro_mcchoice");
			foreach (XElement current in enumerable)
			{
				string xElementAttribureValue2 = CUtils.getXElementAttribureValue(current, "mcid", "");
				if (xElementAttribureValue2 == "")
				{
					current.SetAttributeValue("mcid", xElementAttribureValue);
				}
			}
		}
		private void processMC(XElement mc, XElement step)
		{
			CHTSMultiChoice cHTSMultiChoice = new CHTSMultiChoice(mc, this);
			try
			{
				this.mchoices.Add(cHTSMultiChoice._elid, cHTSMultiChoice);
				cHTSMultiChoice.processChoises(step);
			}
			catch
			{
			}
		}
		private string scrambleMultiChoices(string body)
		{
			foreach (KeyValuePair<string, CHTSMultiChoice> current in this.mchoices)
			{
				body = current.Value.doSubstitute(body);
			}
			return body;
		}
		public double doScore(string psProblem, string respData, string password, string sign)
		{
			CHTSResponse cHTSResponse = new CHTSResponse(this);
			try
			{
				cHTSResponse.parseResponse(respData, password, sign);
			}
			catch
			{
				return 0.0;
			}
			this.sProblem = psProblem;
			this.htsResponse = cHTSResponse;
			this.htsProblem = XDocument.Load(new StringReader(this.sProblem));
			this.vars = this.htsResponse.vars;
			this.replaceVariables();
			return this.getScore();
		}
		internal Point getStepScore(XElement step)
		{
			Point result = new Point(0.0, 0.0);
			int num = 0;
			int num2 = 0;
			string xElementAttribureValue = CUtils.getXElementAttribureValue(step, "id", "no");
			IEnumerable<XElement> enumerable = step.XPathSelectElements(".//iproelement_short");
			foreach (XElement current in enumerable)
			{
				num++;
				num2 += (this.checkUserAnswer(current, xElementAttribureValue) ? 1 : 0);
			}
			IEnumerable<XElement> enumerable2 = step.XPathSelectElements(".//iproelement_mc");
			foreach (XElement current2 in enumerable2)
			{
				num++;
				num2 += (this.checkUserMC(current2, xElementAttribureValue) ? 1 : 0);
			}
			result.X = (double)num;
			result.Y = (double)num2;
			return result;
		}
		public double getScore()
		{
			Point point = new Point(0.0, 0.0);
			if (this.htsResponse != null)
			{
				IEnumerable<XElement> enumerable = this.htsProblem.XPathSelectElements("//iprostep");
				foreach (XElement current in enumerable)
				{
					CHTSIproStep cHTSIproStep = new CHTSIproStep(this);
					cHTSIproStep.stepID = CUtils.getXElementAttribureValue(current, "id", "step0");
					try
					{
						this.iprosteps.Add(cHTSIproStep.stepID, cHTSIproStep);
					}
					catch
					{
					}
					Point stepScore = this.getStepScore(current);
					point.X += stepScore.X;
					point.Y += stepScore.Y;
				}
			}
			if (point.X == point.Y)
			{
				return 100.0;
			}
			if (point.Y == 0.0)
			{
				return 0.0;
			}
			return point.Y * 100.0 / point.X;
		}
		private bool checkUserMC(XElement mc, string stepID)
		{
			CHTSMultiChoice cHTSMultiChoice = new CHTSMultiChoice(mc, this);
			string str = string.Concat(new string[]
			{
				stepID,
				".Checkbox",
				cHTSMultiChoice.getElid(),
				"_",
				cHTSMultiChoice.getCorrectChoiceID()
			});
			XElement xElement = this.htsResponse.xmlResponse.XPathSelectElement("//userinput[@inputid=\"" + str + "\"]");
			return xElement != null;
		}
		internal bool checkUserMC(CHTSChoice mcAnsw, string stepID)
		{
			if (this.htsResponse == null)
			{
				return false;
			}
			string str = string.Concat(new string[]
			{
				stepID,
				".Checkbox",
				mcAnsw._mcid,
				"_",
				mcAnsw._intchoiceid.ToString()
			});
			XElement xElement = this.htsResponse.xmlResponse.XPathSelectElement("//userinput[@inputid=\"" + str + "\"]");
			return xElement != null;
		}
		internal bool checkUserAnswer(XElement shortAnswXML, string stepID)
		{
			CHTSIproStep step = null;
			this.iprosteps.TryGetValue(stepID, out step);
			CHTSIproshort cHTSIproshort = new CHTSIproshort(shortAnswXML, step);
			CUtils.getXElementAttribureValue(shortAnswXML, "type", "no");
			string xElementAttribureValue = CUtils.getXElementAttribureValue(shortAnswXML, "elid", "0");
			string inputid = stepID + ".EnterField" + xElementAttribureValue;
			string userAnswer = this.getUserAnswer(inputid);
			return !(userAnswer == string.Empty) && cHTSIproshort.checkAnswer(userAnswer);
		}
		internal string getUserAnswer(string inputid)
		{
			string text = string.Empty;
			if (this.htsResponse != null)
			{
				XElement xElement = this.htsResponse.xmlResponse.XPathSelectElement("//userinput[@inputid=\"" + inputid + "\"]");
				if (xElement != null)
				{
					text = CUtils.getXElementAttribureValue(xElement, "answer", "");
					text = HttpUtility.UrlDecode(text);
				}
			}
			return text;
		}
		public string getProblemBody()
		{
			IEnumerable<XElement> source = this.htsProblem.XPathSelectElements("//vardef");
			source.Remove<XElement>();
			IEnumerable<XElement> source2 = this.htsProblem.XPathSelectElements("//iprostep[@del=\"yes\"]");
			source2.Remove<XElement>();
			string text = this.htsProblem.ToString();
			text = text.Replace("<![CDATA[", "");
			text = text.Replace("]]>", "");
			text = text.Replace(new string('\u00a0', 1), "&#160;");
			text = this.scrambleMultiChoices(text);
			text = text.Replace("<iproblem>", "<div class=\"cq_hts_masterdiv\">");
			text = text.Replace("</iproblem>", "</div>");
			text = text.Replace("<iprostep ", "<div class=\"cq_hts_step\" ");
			return text.Replace("</iprostep>", "</div>");
		}
		public string getProblemPageForAgilix(string password, string sign)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(CUtils.jsScriptTag(this.baseUrl + "htsplayer/hts.js"));
			stringBuilder.AppendLine(CUtils.jsScriptTag(this.baseUrl + "htsplayer/qsutils_1.js"));
			stringBuilder.AppendLine(CUtils.jsScriptTag(this.baseUrl + "htsplayer/PopupWindow.js"));
			stringBuilder.AppendLine(CUtils.jsScriptTag(this.baseUrl + "htsplayer/divPopup.js"));
			stringBuilder.AppendLine("\r\n                <script  type='text/javascript'> ");
			stringBuilder.AppendLine("var _serverURL='" + this.baseUrl + "';");
			stringBuilder.AppendLine("function jsGetServerURL() \r\n            { return _serverURL; }");
			stringBuilder.AppendLine("\r\n                var resp$QUESTIONID$ = '<iproblem>" + this.getEncryptedResponse(password, sign) + "</iproblem>';");
			stringBuilder.AppendLine("\r\n            function saveMethod$QUESTIONID$()\r\n            {\r\n                saveInfo = CQ.getInfo($QUESTIONID$);\r\n                CQ.setAnswer($QUESTIONID$, '<response>' + resp$QUESTIONID$ + ");
			for (int i = 0; i < this.agilixAnswers.Count; i++)
			{
				stringBuilder.AppendLine(this.agilixAnswers[i] + " + ");
			}
			foreach (KeyValuePair<string, CHTSMultiChoice> current in this.mchoices)
			{
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"getMCAnswer('hts$QUESTIONID$.",
					current.Value.step_id,
					".Checkbox",
					current.Key,
					"') + "
				}));
			}
			stringBuilder.AppendLine("                    '</response>' \r\n                    );\r\n            }");
			stringBuilder.AppendLine("\r\n            info = CQ.getInfo($QUESTIONID$);\r\n            // Find the div that should contain the question body\r\n            myDiv = document.getElementById(info.divId);");
			stringBuilder.AppendLine("var divtext = '<div id=\"popupdiv$QUESTIONID$\" style=\"position:absolute;visibility:hidden;z-index:100;background-color:#FFFFFF;\"></div>' + ");
			string[] array = this.getProblemBody().Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int j = 0; j < array.Length; j++)
			{
				stringBuilder.AppendLine("'" + array[j].Replace("'", "\\'") + "' +");
			}
			stringBuilder.AppendLine("' ';");
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"var eqedit = '<div id=\"theLayer$QUESTIONID$\" style=\"position:absolute;left:220px;top:40px;display:none\">' +\r\n        '<table border=\"0\"  bgcolor=\"#0000FF\" cellspacing=\"0\" cellpadding=\"3\">' +\r\n        '<tr><td width=\"100%\">' +\r\n        '<table border=\"0\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" height=\"50\">' +\r\n        '<tr>' +\r\n        '<td id=\"titleBar$QUESTIONID$\" style=\"cursor:move\" width=\"98%\" height=\"20\">' +\r\n        '<ilayer width=\"100%\" onSelectStart=\"return false\">' +\r\n        '<layer width=\"100%\" onMouseover=\"isHot=true\" onMouseout=\"isHot=false\">' +\r\n        '<font face=\"Arial\" color=\"#FFFFFF\">Equation Editor</font>' +\r\n        '</layer>' +\r\n        '</ilayer>' +\r\n        '</td>' +\r\n        '</tr>' +\r\n        '<tr>' +\r\n        '<td width=\"100%\" bgcolor=\"#FFFFFF\"  colspan=\"2\">' +       \r\n        '<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" '\r\n        + 'id=\"eqeditor$QUESTIONID$\" width=\"500\" height=\"270\" '\r\n        + 'codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab\"> '\r\n        + '<param name=\"movie\" value=\"",
				this.baseUrl,
				"htsplayer/eqeditor.swf\" /><param name=\"allowScriptAccess\" value=\"always\" /><param name=\"quality\" value=\"high\" /><param name=\"bgcolor\" value=\"#ffffff\" />'\r\n        + '<embed src=\"",
				this.baseUrl,
				"htsplayer/eqeditor.swf\" quality=\"high\" bgcolor=\"#ffffff\" '\r\n        + 'width=\"500\" height=\"270\" name=\"eqeditor$QUESTIONID$\" align=\"middle\" '\r\n        + 'play=\"true\" '\r\n        + 'loop=\"false\" '\r\n        + 'quality=\"high\" '\r\n        + 'allowScriptAccess=\"always\" '\r\n        + 'type=\"application/x-shockwave-flash\" '\r\n        + 'pluginspage=\"http://www.macromedia.com/go/getflashplayer\">'\r\n        + '</embed>'\r\n        + '</object>' +   \r\n        '</td>' +\r\n        '</tr>' +\r\n        '</table>' + \r\n        '</td>' +\r\n        '</tr>' +\r\n        '</table>' +\r\n    '</div>';"
			}));
			stringBuilder.AppendLine("           \r\n            // Replace the div with the information retrieved above\r\n            myDiv.innerHTML = eqedit+divtext;\r\n            // Register save callback method\r\n            CQ.onBeforeSave(saveMethod$QUESTIONID$);  \r\n            </script>");
			return stringBuilder.ToString();
		}
		private void getVardefs()
		{
			IEnumerable<XElement> enumerable = this.htsProblem.XPathSelectElements("//vardef");
			foreach (XElement current in enumerable)
			{
				CVardef cVardef = new CVardef(current, this);
				this.vars.Add(cVardef.Name, cVardef);
			}
			enumerable.Remove<XElement>();
		}
		public string doVariables()
		{
			foreach (KeyValuePair<string, CVardef> current in this.vars)
			{
				CVardef value = current.Value;
				try
				{
					value.doVariable();
				}
				catch (Exception)
				{
				}
			}
			return "";
		}
		public object doCalculate(string expr)
		{
			return this.htsCalc.doCalculate(expr);
		}
		internal string exprReplace(string src)
		{
			if (src.IndexOf('~') < 0)
			{
				return src;
			}
			string text = src;
			for (int i = 0; i < 2; i++)
			{
				foreach (KeyValuePair<string, CVardef> current in this.vardefs)
				{
					CVardef value = current.Value;
					string name = value.Name;
					string text2 = string.Empty;
					switch (value.varType)
					{
					case eVar_Type.eVar_Numeric:
					case eVar_Type.eVar_Math:
					case eVar_Type.eVar_Text:
						text2 = value.valueToString();
						text2 = CUtils.minusCheck(text2);
						text = text.Replace("~" + name + "\\", text2);
						break;
					case eVar_Type.eVar_NumericArray:
					case eVar_Type.eVar_MathArray:
					case eVar_Type.eVar_TextArray:
						if (i == 1)
						{
							StringCollection stringCollection = (StringCollection)value.Value;
							if (stringCollection != null)
							{
								for (int j = 0; j < stringCollection.Count; j++)
								{
									text = text.Replace(string.Concat(new object[]
									{
										"~",
										name,
										"[",
										j + 1,
										"]\\"
									}), "(" + stringCollection[j] + ")");
									text = text.Replace(string.Concat(new object[]
									{
										"~",
										name,
										"[(",
										j + 1,
										")]\\"
									}), "(" + stringCollection[j] + ")");
								}
								text = text.Replace("~" + name + "[]\\", value.valueToString());
							}
						}
						break;
					}
				}
			}
			return text;
		}
		public string prepareResponseXML()
		{
			string text = string.Format("<iproblem results=\"{0}\" showcorrect=\"{1}\" hints=\"{2}\" feedback=\"{3}\" >", new object[]
			{
				this.p_results,
				this.p_showcorrect,
				this.p_hints,
				this.p_feedback
			});
			text += this.varsToXmlString();
			text += "<mc_scrambled>";
			foreach (KeyValuePair<string, CHTSMultiChoice> current in this.mchoices)
			{
				text += current.Value.getXMLScramble();
			}
			text += "</mc_scrambled></iproblem>";
			return text;
		}
		public string getEncryptedResponse(string password, string sign)
		{
			return CUtils.Encrypt(this.prepareResponseXML(), password, sign);
		}
		public string getDecryptedResponse(string cryptedResponse, string password, string sign)
		{
			return CUtils.Decrypt(cryptedResponse, password, sign);
		}
	}
}
