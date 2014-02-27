using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Net; // for WebRequest
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
//using System.Threading;
//using System.Globalization;
using System.Windows;

namespace HTS
{
    public class CHTSProblem
    {
        private CHTSCalculator htsCalc = null;
        private Dictionary<string, CVardef> vars = new Dictionary<string, CVardef>();
        private Dictionary<string, CHTSIproStep> iprosteps = new Dictionary<string, CHTSIproStep>();
        private Dictionary<string, CHTSMultiChoice> mchoices = new Dictionary<string, CHTSMultiChoice>();
        private string sProblem = null;
        
        internal XDocument htsProblem = null;
        internal CHTSResponse htsResponse = null;
        internal string baseUrl = null;

        internal string maxPoints = "";

        internal StringCollection agilixAnswers = null;

        //------------------- player parameters
        internal string p_results = "edit"; // "show", "edit", "ignore"
        internal string p_showcorrect = "yes";//"no", "yes", "icononly", "inline";
        internal string p_hints = "no";
        internal string p_feedback = "no";
        internal string p_showfeedback = "no";
        internal string p_showsolution = "no";
        internal string p_showanswers = "no";
        internal string p_syntaxchecking = "on";

        internal bool showAllSteps = false;
        internal bool b_syntaxchecking = true;


        public CHTSProblem(string jsMathPath,string bUrl)
        {
            baseUrl = bUrl;
            htsCalc = new CHTSCalculator(jsMathPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">semicolon separated list of parameters</param>
        public void setParameters(string parameters)
        {
            string[] ps = parameters.Split(new char[] { ';' });
            string[] oneParam = null;
            for (int i = 0; i < ps.Length; i++)
            {
                oneParam = ps[i].Split(new char[] { '=' });
                if (oneParam.Length > 1)
                {
                    if (oneParam[0] == "results") p_results = oneParam[1];
                    if (oneParam[0] == "showcorrect") p_showcorrect = oneParam[1];
                    if (oneParam[0] == "feedback") p_feedback = oneParam[1];
                    if (oneParam[0] == "showfeedback") p_showfeedback = oneParam[1];
                    if (oneParam[0] == "showsolution") p_showsolution = oneParam[1];
                    if (oneParam[0] == "showanswers") p_showanswers = oneParam[1];
                    if (oneParam[0] == "hints") p_hints = oneParam[1];
                    if (oneParam[0] == "syntaxchecking") p_syntaxchecking = oneParam[1];
                    
                }
            }
            showAllSteps = (p_showcorrect == "yes") || (p_showcorrect == "inline") || (p_results == "show");
            b_syntaxchecking = ((p_results == "edit") || (p_results == "ignore")) && ((p_syntaxchecking == "on")||(p_syntaxchecking == "yes")||(p_syntaxchecking == "true"));
        }
        public void doProblem(string psProblem, CHTSResponse response)
        {
            sProblem = psProblem;
            htsResponse = response;
            doProblem();
        }

        public void doProblem()
        {
            agilixAnswers = new StringCollection();

            mchoices = new Dictionary<string, CHTSMultiChoice>();
            htsProblem = XDocument.Load(new System.IO.StringReader(sProblem));
            maxPoints = CUtils.getXElementAttribureValue(htsProblem.Root, "maxpoints", "0");

            if (htsResponse != null)
            {
                // restore vars from response
                vars = htsResponse.vars;
            }
            else
            {
                getVardefs();
                doVariables();
            }
            replaceVariables();
            processHTSelements();
        }



        private void replaceVariables()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                foreach (KeyValuePair<string, CVardef> kvp in this.vars)
                {
                    CVardef vd = kvp.Value;
                    switch (vd.varType)
                    {
                        case eVar_Type.eVar_NumericArray:
                        case eVar_Type.eVar_MathArray:
                        case eVar_Type.eVar_TextArray:
                            sProblem = sProblem.Replace("~" + vd.Name + "[]\\", vd.valueToString());
                            sProblem = sProblem.Replace("~" + vd.Name + "\\", vd.valueToString());
                            StringCollection arr = vd.Value as StringCollection;
                            for (int i = 0; i < arr.Count; i++)
                            {
                                sProblem = sProblem.Replace("~" + vd.Name + "[" + (i + 1).ToString() + "]\\",
                                    vd.varType == eVar_Type.eVar_NumericArray ? CUtils.minusCheck(arr[i]) : arr[i]);
                            }
                            break;
                        case eVar_Type.eVar_Numeric:
                        case eVar_Type.eVar_Math:
                        case eVar_Type.eVar_Text:
                        default:
                            sProblem = sProblem.Replace("~" + vd.Name + "\\",
                                vd.varType == eVar_Type.eVar_Numeric ? CUtils.minusCheck(vd.valueToString()) : vd.valueToString());
                            break;
                    }
                }
            }
            htsProblem = XDocument.Load(new System.IO.StringReader(sProblem));
            htsProblem.Root.SetAttributeValue("maxpoints", null);


        }

        public string varsToXmlString()
        {
            string xml = "<vardefs>";
                foreach (KeyValuePair<string, CVardef> kvp in this.vars)
                {
                    CVardef vd = kvp.Value;
                    xml += vd.toXMLString();
                }
                xml += "</vardefs>";
                return xml;
        }

        private void processHTSelements()
        {
            processClassAttribute();
            if (p_showsolution == "no")
            {
                IEnumerable<XElement> stepsol = htsProblem.XPathSelectElements("//iprostep[@id=\"solution\"]");
                stepsol.Remove();

            }

            processGraphElement();

            IEnumerable<XElement> steps = htsProblem.XPathSelectElements("//iprostep");
            IEnumerable<XElement> images = null;
            IEnumerable<XElement> iproFormulas = null;
            IEnumerable<XElement> iproShorts = null;
            IEnumerable<XElement> multiChoices = null;

            int stepCnt = 0;
            
            foreach (XElement el in steps)
            {
                string stepID = CUtils.getXElementAttribureValue(el, "id", "no");
                string stepForDeleting = CUtils.getXElementAttribureValue(el, "del", "no");
                iproShorts = null;
                multiChoices = null;
                iproFormulas = null;
                images = null;
                if (stepForDeleting != "yes")
                {
                    CHTSIproStep st = new CHTSIproStep(this);
                    try
                    {
                        st.stepID = CUtils.getXElementAttribureValue(el, "id", "step0");
                        this.iprosteps.Add(st.stepID, st);
                        st.processStep(el);
                    }
                    catch { }

                    // process images
                    images = el.XPathSelectElements(".//img");
                    foreach (XElement img in images)
                    {
                        processImage(img);
                    }


                    // process Iproformula
                    iproFormulas = el.XPathSelectElements(".//iproformula");
                    foreach (XElement iproFormula in iproFormulas)
                    {
                        processIproformula(iproFormula);
                    }
                    iproFormulas.Remove();

                    // process iproelement_short
                    iproShorts = el.XPathSelectElements(".//iproelement_short");
                    foreach (XElement iproShort in iproShorts)
                    {
                        processIproshort(iproShort, st);
                    }
                    iproShorts.Remove();

                    // process iproelement_mc
                    multiChoices = el.XPathSelectElements(".//iproelement_mc");
                    foreach (XElement mc in multiChoices)
                    {
                        setMCID(mc);
                    }

                    multiChoices = el.XPathSelectElements(".//iproelement_mc");
                    foreach (XElement mc in multiChoices)
                    {
                        processMC(mc, el);
                    }
                    multiChoices.Remove();

                    el.Attributes().Remove();
                    el.SetAttributeValue("id", "hts$QUESTIONID$." + stepID);
                    if ((stepCnt > 0) && (!this.showAllSteps))
                    {
                        el.SetAttributeValue("style", "display:none");
                    }
                    stepCnt++;

                    if (st.b_syntaxcheckNeeded)
                    {
                        if (st.elemsForCheck.Count > 0)
                        {
                            string it = "";
                            foreach (KeyValuePair<string, CHTSIproshort> kvp in st.elemsForCheck)
                            {
                                it += "," + "\'" + kvp.Key + "'";
                            }
                            XElement chkBtn = new XElement("input");
                            chkBtn.SetAttributeValue("class", "cq_hts_check");
                            chkBtn.SetAttributeValue("hide", "no");
                            chkBtn.SetAttributeValue("id", "hts$QUESTIONID$." + stepID + ".CheckSyntax");
                            chkBtn.SetAttributeValue("onclick", "javascript:checksyntax(\'hts$QUESTIONID$." + stepID + "\'" + it +")");
                            chkBtn.SetAttributeValue("type", "button");
                            chkBtn.SetAttributeValue("value", "Check Syntax");

                            XElement chkDIV = new XElement("div");
                            chkDIV.SetAttributeValue("id", "hts$QUESTIONID$." + stepID + ".CheckSyntaxResult");

                            el.Add(new XElement("p",
                                chkBtn,
                                chkDIV
                                ));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// rename all classname to cq_hts_classname
        /// </summary>
        private void processClassAttribute()
        {
            IEnumerable<XElement> elemsWithClass = htsProblem.XPathSelectElements("//*[@class]");
            foreach (XElement el in elemsWithClass)
            {
                string cls = CUtils.getXElementAttribureValue(el, "class", "");
                if ((cls != "") && (!cls.StartsWith("cq_hts_")))
                {
                    cls = "cq_hts_" + cls;
                    el.SetAttributeValue("class", cls);
                }
            }
        }

        /// <summary>
        /// process graph (note: one graph per problem) //more graphs- not tested
        /// </summary>
        private void processGraphElement()
        {
            int cnt = 0;
            IEnumerable<XElement> elemsGraph = htsProblem.XPathSelectElements("//*[@class='cq_hts_graph']");
            foreach (XElement el in elemsGraph)
            {
                string func = CUtils.getXElementAttribureValue(el, "function", "");
                XElement div = new XElement("div", "FF"); //for some reson we need generate <div></div> instead of <div />
                div.SetAttributeValue("id", "hts$QUESTIONID$.graph_" + cnt.ToString());
                div.SetAttributeValue("function", func);
                div.SetAttributeValue("style", "display:none");
                cnt++;
                el.AddAfterSelf(div);
            }
            elemsGraph.Remove();

            cnt = 0;
            IEnumerable<XElement> elemsObject = htsProblem.XPathSelectElements("//*[@classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000']");
            foreach (XElement el in elemsObject)
            {
                XElement param = el.XPathSelectElement("//param[@name='movie']");
                if (param != null)
                {
                    string datafile = this.baseUrl + CUtils.getXElementAttribureValue(param, "value", "");
                    if (datafile.IndexOf("?datafile=http:") < 0 )
                    {
                        //string tmp = "http://root.dev.brainhoney.bfwpub.com";
                        string tmp = "";
                        datafile = datafile.Replace("?datafile=", "?datafile="+tmp+"[~]/");
                    }
                    datafile += "&cqid=hts$QUESTIONID$.graph_" + cnt.ToString();
                    param.SetAttributeValue("value", datafile);

                    XElement embed = el.XPathSelectElement("//embed");
                    embed.SetAttributeValue("src", datafile);

                }
                cnt++;
            }
        }

        /// <summary>
        /// add [~]/ to relative paths
        /// </summary>
        /// <param name="image"></param>
        private void processImage(XElement image)
        {
            string src = CUtils.getXElementAttribureValue(image, "src", "");
            if (!src.StartsWith("http:"))
            {
                src = "[~]/" + src;
                image.SetAttributeValue("src", src);
            }
        }

        /// <summary>
        /// convert iproformula to IMG tag
        /// </summary>
        /// <param name="iproFormula"></param>
        private void processIproformula(XElement iproFormula)
        {
            string strFormula = iproFormula.ToString();
            strFormula = strFormula.Replace("<iproformula ", "<img ");
            strFormula = strFormula.Replace("</iproformula>", "</img>");
            XElement htmlFormula = XElement.Load(new System.IO.StringReader(strFormula));

            string fsrc = CUtils.getXElementAttribureValue(htmlFormula, "src", "");
            string[] frmSrc = fsrc.Split(new char[] { '=' }, 2);
            // get style attribute for formula image
            bool isExpr = (frmSrc[1].IndexOf("exprtext=") != -1);
            //htmlFormula.SetAttributeValue("style", CUtils.getFormulaStyle(frmSrc[1], isExpr));
            htmlFormula.SetAttributeValue("type", null);

            //get src attribute for formula image
            fsrc = baseUrl + frmSrc[0] + "=" + HttpUtility.UrlEncode(frmSrc[1]);
            htmlFormula.SetAttributeValue("src", fsrc);
            iproFormula.AddBeforeSelf(htmlFormula);
        }

        /// <summary>
        /// convert iproelement_short to HTML presentation
        /// </summary>
        /// <param name="iproShort"></param>
        /// <param name="stepID"></param>
        private void processIproshort(XElement iproShort, CHTSIproStep step)
        {
            string stepID = step.stepID;

            CHTSIproshort ciproShort = new CHTSIproshort(iproShort, step);
            if (this.b_syntaxchecking)
            {
                step.b_syntaxcheckNeeded = (ciproShort.checkSyntaxNeeded() || step.b_syntaxcheckNeeded );
            }
            
            string val = "";
            string mathInputUrl = this.baseUrl + "pictures/mathinput.gif";
            string inputID = "hts$QUESTIONID$." + stepID + ".EnterField" + ciproShort._elid;
            string answerInputID = stepID + ".EnterField" + ciproShort._elid;
            string anchorID = "hts$QUESTIONID$." + stepID + ".anchor" + ciproShort._elid;
            string readOnly = "";
            XElement showCorrect = null;

            string correctaTag = "";
            string correctendaTag = "";

            string inp = "";
            if ((ciproShort._type == "numeric") || (ciproShort._type == "text"))
            {
                if (ciproShort._type == "numeric") this.agilixAnswers.Add("getNumericAnswer('" + inputID + "')");
                if (ciproShort._type == "text") this.agilixAnswers.Add("getTextAnswer('" + inputID + "')");
                if (p_results == "show")
                {
                    val = this.getUserAnswer(answerInputID);
                    //readOnly = " DISABLED=\"true\" ";
                    readOnly = " DISABLED=\"true\" ";
                }
                else if (p_results == "edit")
                {
                    val = this.getUserAnswer(answerInputID);
                }
                // else if (p_results == "ignore") do nothing

                if (p_showcorrect == "inline")
                {
                    val = CUtils.getXElementAttribureValue(iproShort, "correct", "");
                    val = Convert.ToString(this.doCalculate(val));
                }
                else if (p_showcorrect == "icononly")
                {
                    string picture = this.checkUserAnswer(iproShort, stepID) ? "success" : "fail";
                    correctaTag = "<A id=\"" + anchorID + "\" name=\"" + anchorID + "\" >";
                    correctaTag += "<img src=\"" + this.baseUrl + "pictures/" + picture + ".gif\" />";
                    correctendaTag = "</A>";
                    showCorrect = XElement.Load(new System.IO.StringReader(correctaTag + correctendaTag));
                }
                else if (p_showcorrect == "yes")
                { //icon and popup
                    string popupCorrect = ciproShort.getCorrectForShowCorrect();
                    string picture = this.checkUserAnswer(iproShort, stepID) ? "success" : "fail_input";
                    string onmouseover = "popupTextAndNum('" + anchorID + "','" + popupCorrect + "')\"";
                    correctaTag = "<A id=\"" + anchorID + "\" name=\"" + anchorID + "\" >";
                    correctaTag += "<![CDATA[<img onmouseover=\"" + onmouseover + "\" onmouseout=\"hidepopup()\" src=\"" + this.baseUrl + "pictures/" + picture + ".gif\" />]]>";
                    correctendaTag = "</A>";
                    showCorrect = XElement.Load(new System.IO.StringReader(correctaTag + correctendaTag));
                }

                string spanTag = "<SPAN class=\"field\">" +
                    "<INPUT TYPE=\"text\" onkeypress=\"javascript:onKeyPressed(event,this)\" id=\"" + inputID + "\" " +
                    "style=\"OVERFLOW: visible\" " + readOnly +
                    " onchange=\"javascript:enterfieldAnswer('" + inputID + "'); \" size=\"" + ciproShort._size + "\"" +
                    " allowedwords=\"" + ciproShort._allowedwords + "\"" +
                    " value=\"" + val + "\"/>" +
                    "</SPAN>";
                inp = spanTag;
            }
            else //math
            {
                this.agilixAnswers.Add("getMathAnswer('" + inputID + "')");
                string aTag = "<A href=\"javascript:showEnterfieldAnswer('" + inputID + "');\" >";
                string endaTag = "</A>";

                if (p_results == "show")
                {
                    aTag = "";
                    endaTag = ""; //disable editing
                    val = this.getUserAnswer(answerInputID);
                }
                else if (p_results == "edit")
                {
                    val = this.getUserAnswer(answerInputID);
                }
                // else if (p_results == "ignore") do nothing

                if (p_showcorrect == "inline") // overwrite userAnswer with correct answer
                {
                    val = CUtils.getXElementAttribureValue(iproShort, "correct", "");
                }
                else if (p_showcorrect == "icononly")
                {
                    string picture = this.checkUserAnswer(iproShort,stepID) ? "success" : "fail";
                    correctaTag = "<A id=\"" + anchorID + "\" name=\"" + anchorID + "\" >";
                    correctaTag += "<img src=\"" + this.baseUrl + "pictures/" + picture + ".gif\" />";
                    correctendaTag = "</A>";
                    showCorrect = XElement.Load(new System.IO.StringReader(correctaTag + correctendaTag));
                }
                else if (p_showcorrect == "yes")
                { //icon and popup
                    string popupCorrect = ciproShort.getCorrectForShowCorrect();
                    string picture = this.checkUserAnswer(iproShort, stepID) ? "success" : "fail_input";
                    string onmouseover = "popupMath('" + anchorID +"','" + popupCorrect + "')\"";
                    correctaTag = "<A id=\"" + anchorID + "\" name=\"" + anchorID + "\" >";
                    correctaTag += "<![CDATA[<img onmouseover=\"" + onmouseover + "\" onmouseout=\"hidepopup()\" src=\"" + this.baseUrl + "pictures/" + picture + ".gif\" />]]>";
                    correctendaTag = "</A>";
                    showCorrect = XElement.Load(new System.IO.StringReader(correctaTag + correctendaTag));
                }

                if (val != string.Empty)
                {
                    mathInputUrl = this.baseUrl + "geteq.ashx?eqtext=" + HttpUtility.UrlEncode(val) + "&doborder=true&bottom=8";
                }

                string imgTag = "<img id=\"" + inputID + "\"" +
                    " align=\"middle\"" +
                    " userAnswer=\"" + HttpUtility.UrlEncode(val) + "\" />";
                inp = aTag + imgTag + endaTag ; 
            }
            try
            {
                XElement htsInput = XElement.Load(new System.IO.StringReader(inp));
                if (ciproShort._type == "math")
                {
                    XElement img = htsInput.XPathSelectElement(".//img");
                    if (img != null)
                    {
                        img.SetAttributeValue("src", mathInputUrl);
                    }
                    else
                    {
                        htsInput.SetAttributeValue("src", mathInputUrl);
                    }
                }
                iproShort.AddBeforeSelf(htsInput);
                if (showCorrect != null) iproShort.AddBeforeSelf(showCorrect);
            }
            catch (Exception ex) 
            { 
            }

        }

        private void setMCID(XElement mc)
        {
            string elid = CUtils.getXElementAttribureValue(mc, "elid", "0");
            IEnumerable<XElement> ipro_mcchoices = mc.XPathSelectElements("//ipro_mcchoice");
            foreach (XElement mChoice in ipro_mcchoices)
            {
                string mcid = CUtils.getXElementAttribureValue(mChoice, "mcid", "");
                if (mcid == "")
                {
                    mChoice.SetAttributeValue("mcid", elid);
                }
            }
        }

        private void processMC(XElement mc, XElement step)
        {
            CHTSMultiChoice MC = new CHTSMultiChoice(mc,this);
            try
            {
                this.mchoices.Add(MC._elid, MC);
                MC.processChoises(step);
            }
            catch { }

        }

        private string scrambleMultiChoices(string body)
        {
            foreach (KeyValuePair<string, CHTSMultiChoice> kvp in mchoices)
            {
                body = kvp.Value.doSubstitute(body);
            }
            return body;
        }

        public double doScore(string psProblem, string respData, string password, string sign)        
        {
            //CHTSResponse response
            CHTSResponse response = new CHTSResponse(this);
            try
            {
                response.parseResponse(respData,password, sign);
            }
            catch { return 0; }

            sProblem = psProblem;
            htsResponse = response;
            htsProblem = XDocument.Load(new System.IO.StringReader(sProblem));
                // restore vars from response
            vars = htsResponse.vars;

            replaceVariables();
            return getScore();
        }

        internal Point getStepScore(XElement step)
        {
            Point score = new Point(0, 0);

            int questionCount = 0;
            int correctResponsesCount = 0;
            IEnumerable<XElement> shortAnswers = null;
            IEnumerable<XElement> MultiChoices = null;

            string stepID = CUtils.getXElementAttribureValue(step, "id", "no");
            shortAnswers = null;
            shortAnswers = step.XPathSelectElements(".//iproelement_short");
            foreach (XElement shortAnsw in shortAnswers)
            {
                questionCount++;
                correctResponsesCount += this.checkUserAnswer(shortAnsw, stepID) ? 1 : 0; ;
            }
            MultiChoices = null;
            MultiChoices = step.XPathSelectElements(".//iproelement_mc");
            foreach (XElement mc in MultiChoices)
            {
                questionCount++;
                correctResponsesCount += this.checkUserMC(mc, stepID) ? 1 : 0; ;
            }
            score.X = questionCount;
            score.Y = correctResponsesCount;

            return score;


        }

        public double getScore()
        {
            double result = 0.0;
            Point score = new Point(0, 0);

            if (this.htsResponse != null)
            {
                IEnumerable<XElement> steps = htsProblem.XPathSelectElements("//iprostep");
                foreach (XElement el in steps)
                {
                    CHTSIproStep step = new CHTSIproStep(this);
                    step.stepID = CUtils.getXElementAttribureValue(el, "id", "step0");
                    try
                    {
                        iprosteps.Add(step.stepID, step);
                    }
                    catch { }
                    Point stepScore = getStepScore(el);
                    score.X += stepScore.X;
                    score.Y += stepScore.Y;
                }
            }
            if (score.X == score.Y) return 100;
            if (score.Y == 0) return 0;

            result = (score.Y * 100.0) / (score.X);
            return result;
        }

        private bool checkUserMC(XElement mc, string stepID)
        {
            CHTSMultiChoice mcAnsw = new CHTSMultiChoice(mc, this);
            string att = stepID + "." + "Checkbox" + mcAnsw.getElid() + "_" + mcAnsw.getCorrectChoiceID();
            XElement userAnswer = this.htsResponse.xmlResponse.XPathSelectElement("//userinput[@inputid=\"" + att + "\"]");
            return (userAnswer != null);
        }

        internal bool checkUserMC(CHTSChoice mcAnsw, string stepID)
        {
            if (this.htsResponse == null) return false;

            string att = stepID + "." + "Checkbox" + mcAnsw._mcid + "_" + mcAnsw._intchoiceid.ToString();
            XElement userAnswer = this.htsResponse.xmlResponse.XPathSelectElement("//userinput[@inputid=\"" + att + "\"]");
            return (userAnswer != null);
        }

        internal bool checkUserAnswer(XElement shortAnswXML, string stepID)
        {
            CHTSIproStep step = null;
            this.iprosteps.TryGetValue(stepID, out step);
            CHTSIproshort shortAnsw = new CHTSIproshort(shortAnswXML, step);

            string answType = CUtils.getXElementAttribureValue(shortAnswXML, "type", "no");
            string elid = CUtils.getXElementAttribureValue(shortAnswXML, "elid", "0");
            string att = stepID + "." + "EnterField" + elid;
            string answ = getUserAnswer(att);
            if (answ == string.Empty) return false;
            return shortAnsw.checkAnswer(answ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputid">stepID + ".EnterField" + elid</param>
        /// <returns></returns>
        internal string getUserAnswer(string inputid)
        {
            string uAnswer = string.Empty;
            if (this.htsResponse != null)
            {
                XElement userAnswer = this.htsResponse.xmlResponse.XPathSelectElement("//userinput[@inputid=\"" + inputid + "\"]");
                if (userAnswer != null)
                {
                    uAnswer = CUtils.getXElementAttribureValue(userAnswer, "answer", "");
                    uAnswer = HttpUtility.UrlDecode(uAnswer); 
                }
            }
            return uAnswer;
        }

        public string getProblemBody()
        {
            IEnumerable<XElement> vardefs = htsProblem.XPathSelectElements("//vardef");
            vardefs.Remove();
            IEnumerable<XElement> steps = htsProblem.XPathSelectElements("//iprostep[@del=\"yes\"]");
            steps.Remove();

            string s= htsProblem.ToString();
            s = s.Replace("<![CDATA[", "");
            s = s.Replace("]]>","");

            s = s.Replace(new string((char)160,1), "&#160;");
            s = scrambleMultiChoices(s);
            s = s.Replace("<iproblem>", "<div class=\"cq_hts_masterdiv\">");
            s = s.Replace("</iproblem>", "</div>");
            s = s.Replace("<iprostep ", "<div class=\"cq_hts_step\" ");
            s = s.Replace("</iprostep>", "</div>");

            return s;
        }

        public string getProblemPageForAgilix(string password, string sign)
        {
            StringBuilder body = new StringBuilder();
            //body.AppendLine(header);
            //body.AppendLine(CUtils.jsScriptTag(baseUrl + "htsplayer/maths.js"));
            body.AppendLine(CUtils.jsScriptTag(baseUrl + "htsplayer/hts.js"));
            //body.AppendLine(CUtils.jsScriptTag(baseUrl + "htsplayer/angel.js"));
            body.AppendLine(CUtils.jsScriptTag(baseUrl + "htsplayer/qsutils_1.js"));
            body.AppendLine(CUtils.jsScriptTag(baseUrl + "htsplayer/PopupWindow.js"));
            body.AppendLine(CUtils.jsScriptTag(baseUrl + "htsplayer/divPopup.js"));
            body.AppendLine(@"
                <script  type='text/javascript'> ");
            body.AppendLine(@"var _serverURL='" + baseUrl + "';");
            body.AppendLine(@"function jsGetServerURL() 
            { return _serverURL; }");

            body.AppendLine(@"
                var resp$QUESTIONID$ = '<iproblem>" + this.getEncryptedResponse(password,sign) +"</iproblem>';" );

            body.AppendLine(@"
            function saveMethod$QUESTIONID$()
            {
                saveInfo = CQ.getInfo($QUESTIONID$);
                CQ.setAnswer($QUESTIONID$, '<response>' + resp$QUESTIONID$ + ");

            for (int i=0; i<agilixAnswers.Count; i++)
            {
                body.AppendLine (agilixAnswers[i] + " + ");
            }
            foreach (KeyValuePair<string, CHTSMultiChoice> kvp in mchoices)
            {

                body.AppendLine("getMCAnswer('hts$QUESTIONID$." + kvp.Value.step_id + ".Checkbox" + kvp.Key + "') + ");
            }
            body.AppendLine(@"                    '</response>' 
                    );
            }");

            body.AppendLine(@"
            info = CQ.getInfo($QUESTIONID$);
            // Find the div that should contain the question body
            myDiv = document.getElementById(info.divId);"
                );

        body.AppendLine(@"var divtext = '<div id=""popupdiv$QUESTIONID$"" style=""position:absolute;visibility:hidden;z-index:100;background-color:#FFFFFF;""></div>' + ");

        string[] problemBody = this.getProblemBody().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < problemBody.Length; i++)
        {
            body.AppendLine ("'" + problemBody[i].Replace("'","\\'") + "' +");
        }
        body.AppendLine("' ';");

        // add ref to Equation Editor
        body.AppendLine(@"var eqedit = '<div id=""theLayer$QUESTIONID$"" style=""position:absolute;left:220px;top:40px;display:none"">' +
        '<table border=""0""  bgcolor=""#0000FF"" cellspacing=""0"" cellpadding=""3"">' +
        '<tr><td width=""100%"">' +
        '<table border=""0"" width=""100%"" cellspacing=""0"" cellpadding=""0"" height=""50"">' +
        '<tr>' +
        '<td id=""titleBar$QUESTIONID$"" style=""cursor:move"" width=""98%"" height=""20"">' +
        '<ilayer width=""100%"" onSelectStart=""return false"">' +
        '<layer width=""100%"" onMouseover=""isHot=true"" onMouseout=""isHot=false"">' +
        '<font face=""Arial"" color=""#FFFFFF"">Equation Editor</font>' +
        '</layer>' +
        '</ilayer>' +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td width=""100%"" bgcolor=""#FFFFFF""  colspan=""2"">' +       
        '<object classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" '
        + 'id=""eqeditor$QUESTIONID$"" width=""500"" height=""270"" '
        + 'codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab""> '
        + '<param name=""movie"" value=""" + baseUrl + @"htsplayer/eqeditor.swf"" /><param name=""allowScriptAccess"" value=""always"" /><param name=""quality"" value=""high"" /><param name=""bgcolor"" value=""#ffffff"" />'
        + '<embed src=""" + baseUrl + @"htsplayer/eqeditor.swf"" quality=""high"" bgcolor=""#ffffff"" '
        + 'width=""500"" height=""270"" name=""eqeditor$QUESTIONID$"" align=""middle"" '
        + 'play=""true"" '
        + 'loop=""false"" '
        + 'quality=""high"" '
        + 'allowScriptAccess=""always"" '
        + 'type=""application/x-shockwave-flash"" '
        + 'pluginspage=""http://www.macromedia.com/go/getflashplayer"">'
        + '</embed>'
        + '</object>' +   
        '</td>' +
        '</tr>' +
        '</table>' + 
        '</td>' +
        '</tr>' +
        '</table>' +
    '</div>';"
            );

            body.AppendLine(@"           
            // Replace the div with the information retrieved above
            myDiv.innerHTML = eqedit+divtext;
            // Register save callback method
            CQ.onBeforeSave(saveMethod$QUESTIONID$);  
            </script>");

            return body.ToString();

        }

        private void getVardefs()
        {
            IEnumerable<XElement> vardefs = htsProblem.XPathSelectElements("//vardef");
            foreach (XElement el in vardefs)
            {
                CVardef vardef = new CVardef(el, this);
                vars.Add(vardef.Name, vardef);
            }
            vardefs.Remove();
        }

        public string doVariables()
        {
            foreach (KeyValuePair<string, CVardef> kvp in vars)
            {
                CVardef vardef = kvp.Value;
                try
                {
                    vardef.doVariable();
                }
                catch (Exception ex)
                {
                }
            }
            return "";
        }

        public Dictionary<string, CVardef> vardefs
        {
            get { return vars; }
        }

        public object doCalculate(string expr)
        {
            return htsCalc.doCalculate(expr);
        }

        /// <summary>
        /// replace var names (~var\) in the src string with the current variables values
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        internal string exprReplace(string src)
        {
            if (src.IndexOf('~') < 0) return src; // nothing to replace

            string expr = src;

            for (int pass = 0; pass < 2; pass++) //!PM need two passes!!
            {
                foreach (KeyValuePair<string, CVardef> kvp in this.vardefs)
                {
                    CVardef vardef = kvp.Value;
                    string name = vardef.Name;
                    string val = string.Empty;
                    switch (vardef.varType)
                    {
                        case eVar_Type.eVar_Numeric:
                        case eVar_Type.eVar_Math:
                        case eVar_Type.eVar_Text:
                            val = vardef.valueToString();
                            val = CUtils.minusCheck(val);
                            expr = expr.Replace("~" + name + "\\", val);
                            break;
                        case eVar_Type.eVar_NumericArray:
                        case eVar_Type.eVar_TextArray:
                        case eVar_Type.eVar_MathArray:
                            if (pass == 1)
                            {
                                StringCollection valArray = (StringCollection)vardef.Value;
                                if (valArray != null)
                                {
                                    for (int k = 0; k < valArray.Count; k++)
                                    {
                                        expr = expr.Replace("~" + name + "[" + (k + 1) + "]" + "\\", "(" + (string)(valArray[k]) + ")");
                                        expr = expr.Replace("~" + name + "[(" + (k + 1) + ")]" + "\\", "(" + (string)(valArray[k]) + ")");
                                    }
                                    expr = expr.Replace("~" + name + "[]" + "\\", vardef.valueToString());
                                }

                            }
                            break;
                        default:
                            break;
                    }

                }
            }
            return expr;
        }

        /// <summary>
        /// preparing problem init data (vars and scrambling) to send to Agilix
        /// </summary>
        /// <returns></returns>
        public string prepareResponseXML()
        {
            string resp = string.Format("<iproblem results=\"{0}\" showcorrect=\"{1}\" hints=\"{2}\" feedback=\"{3}\" >",
                p_results,p_showcorrect,p_hints,p_feedback);
            // add vardefs
            resp += this.varsToXmlString();

            resp += "<mc_scrambled>";
            foreach (KeyValuePair<string, CHTSMultiChoice> kvp in mchoices)
            {
                resp += kvp.Value.getXMLScramble();
            }
            resp += "</mc_scrambled></iproblem>";

            return resp;
        }

        public string getEncryptedResponse(string password, string sign)
        {
            return CUtils.Encrypt(prepareResponseXML(), password, sign);
            //return prepareResponseXML();
        }

        public string getDecryptedResponse(string cryptedResponse,string password, string sign)
        {
            return CUtils.Decrypt(cryptedResponse, password, sign);
        }
    }
}


