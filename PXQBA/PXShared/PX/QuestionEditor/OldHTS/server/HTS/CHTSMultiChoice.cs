using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace HTS
{
    public class CHTSMultiChoice
    {
        internal string _elid = "";
        internal string _correct = "";
        internal int _intcorrect = 0;
        internal string _points = "";
        internal string _scramble = "yes";
        internal string step_id = "";
        internal string _realOrder = "";
        internal CHTSProblem _problem = null;

        private static Random random = new Random();

        private Dictionary<string,CHTSChoice> choices = new Dictionary<string,CHTSChoice>();

        public CHTSMultiChoice(XElement MC, CHTSProblem problem)
        {
            _problem = problem;
            _elid = CUtils.getXElementAttribureValue(MC, "elid", "0");
            _correct = CUtils.getXElementAttribureValue(MC, "correct", "1");
            _points = CUtils.getXElementAttribureValue(MC, "points", "1");
            _scramble = CUtils.getXElementAttribureValue(MC, "scramble", "yes");
            try
            {
                _intcorrect = Convert.ToInt32(_problem.doCalculate(_correct));
            }
            catch { }

        }

        internal void processChoises(XElement step)
        {
            step_id = CUtils.getXElementAttribureValue(step, "id", "step0"); 
            int order = 1;

            this._realOrder = getSavedScramble();

            string [] realOrder = null;
            if (this._realOrder != "")
            {
                realOrder = this._realOrder.Split(new char[] {','});
            }
            int countChoicesForScrambling = 0;

            IEnumerable<XElement> ipro_mcchoices = step.XPathSelectElements("//ipro_mcchoice[@mcid=\"" + _elid + "\"]");
            if ((ipro_mcchoices != null) && (ipro_mcchoices.Count() > 0))
            {

                if ((realOrder != null) && (ipro_mcchoices.Count() != realOrder.Length)) realOrder = null;

                foreach (XElement mChoice in ipro_mcchoices)
                {
                    try
                    {
                        CHTSChoice choice = new CHTSChoice(mChoice, order, step_id, this);
                        choices.Add(choice._choiceid, choice);
                        if (realOrder != null)
                        {
                            choice.scrambleOrder = Convert.ToInt32(realOrder[order-1]);
                        }
                        else if (this._scramble == "no")
                        {
                            choice.scrambleOrder = choice.origOrder;
                        }

                        if (choice.scrambleOrder == 0) countChoicesForScrambling++;
                        order++;
                    }
                    catch { }
                }
            }

            if (countChoicesForScrambling != 0)
            { // do new scrambling

                foreach (KeyValuePair<string, CHTSChoice> kvp in this.choices)
                {
                    if (kvp.Value.scrambleOrder == 0)
                    {
                        int rOrder = random.Next(0, countChoicesForScrambling);
                        int cnt = 0;
                        foreach (KeyValuePair<string, CHTSChoice> kvpo in this.choices)
                        {
                            if (! kvpo.Value.numberIsoccupied )
                            {
                                if (cnt == rOrder)
                                {
                                    kvpo.Value.numberIsoccupied = true;
                                    kvp.Value.scrambleOrder = kvpo.Value.origOrder;
                                    countChoicesForScrambling--;
                                    break;
                                }
                                else
                                {
                                    cnt++;
                                }
                            }
                        }
                    }
                    if (countChoicesForScrambling == 0) break;
                }
            }
        }
        public string getCorrectChoiceID()
        {
            return _correct;
        }

        internal string getElid()
        {
            return _elid;
        }

        internal string doSubstitute(string body)
        {
            foreach (KeyValuePair<string, CHTSChoice> kvp in choices)
            {
                body = kvp.Value.doScrambleSubstitute(body);
            }
            return body;
        }
        /*<usersave responseid="test_review" results="ignore" showcorrect="yes" hints="yes" feedback="no" complete="no" percentgrade="100">
         * <iproblem><vardefs><vardef name="fnc" type="text"><value id="1">sinx</value></vardef><vardef name="a" type="numeric"><value id="1">2</value></vardef><vardef name="b" type="numeric"><value id="1">-1</value></vardef><vardef name="func" type="math"><value id="1">2x-sin x</value></vardef><vardef name="" type="numeric"><value id="1">0</value></vardef></vardefs><iprosteps><iprostep id="step0" hint="no" feedback="correct" times="1"><iproelement iproelementid="1" type="mc">
         * <userinput answer="checked" choiceid="1" inputid="step0.Checkbox1_1"/></iproelement></iprostep></iprosteps></iproblem>
         * <mc_scrambled><iproelement_mc_scrambled elid="step0_1">0,0,</iproelement_mc_scrambled></mc_scrambled>
         * </usersave>
         * 
         * */
        internal string getSavedScramble()
        {
            string scramble = "";
            if (this._problem.htsResponse != null)
            {
                XElement iproelement_mc_scrambled = this._problem.htsResponse.xmlResponse.XPathSelectElement("//iproelement_mc_scrambled[@elid=\"" + step_id + "_" + _elid + "\"]");
                if (iproelement_mc_scrambled != null)
                {
                    scramble = iproelement_mc_scrambled.Value;
                }
            }
            return scramble;
        }
        internal string getScrambledOrder()
        {
            string scramble = "";
            foreach (KeyValuePair<string, CHTSChoice> kvp in this.choices)
            {
                if (scramble != "") scramble += ",";

                scramble += kvp.Value.scrambleOrder.ToString();
            }
            return scramble;
        }

        internal string getXMLScramble()
        {
            return "<iproelement_mc_scrambled elid=\"" + this.step_id + "_" + this._elid +"\">" +
                getScrambledOrder() + "</iproelement_mc_scrambled>";
        }

    }
    public class CHTSChoice
    {
        internal string _choiceid = "";
        internal int userAnswer = -1;
        internal int _intchoiceid = -1;
        internal string _fixed = "";
        internal string _choiceText = "";
        internal string _mcid = "";

        internal int origOrder = 0;
        internal int scrambleOrder = 0;
        internal bool numberIsoccupied = false;

        private string stepID = "step0";
        private CHTSMultiChoice _MC = null;

        public CHTSChoice(XElement choice, int order, string pstepID, CHTSMultiChoice MC)
        {
            _MC = MC;
            stepID = pstepID;
            _choiceid = CUtils.getXElementAttribureValue(choice, "choiceid", "0");
            _fixed = CUtils.getXElementAttribureValue(choice, "fixed", "no");
            _mcid = CUtils.getXElementAttribureValue(choice, "mcid", "");
            origOrder = order;
            if (_fixed == "yes")
            {
                numberIsoccupied = true;
                scrambleOrder = origOrder;
            }

            try
            {
                this._intchoiceid = Convert.ToInt32(_MC._problem.doCalculate(_choiceid));
            }
            catch { }

            string inputID = "hts$QUESTIONID$." + stepID + ".Checkbox" + _mcid + "_" + this._intchoiceid.ToString();
            string anchorID = "hts$QUESTIONID$." + stepID + ".check" + _mcid + "_" + this._intchoiceid.ToString();

            choice.RemoveAttributes();
            /*
             * ><SPAN id=step0.check1_1><IMG height=16 src="pictures/showcorrect_success.gif" width=16></SPAN>
             * <INPUT id=step0.Checkbox1_1 onclick="javascript:mult_MC_Answer(1,1,'step0'); " type=radio>
             * <IMG src="http://coursesdev.bfwpub.com/intellipro/geteq.ashx?eqtext=f(x)"> is continuous
             * 
             * <img alt=" " src="http://192.168.78.92:83/PxHTS/geteq.ashx?eqtext=f(x)" style="MARGIN-BOTTOM: -4px; VERTICAL-ALIGN: baseline" /> is discontinuous
             * */
            _choiceText = choice.ToString();
            _choiceText = _choiceText.Replace("<ipro_mcchoice>", "");
            _choiceText = _choiceText.Replace("</ipro_mcchoice>", "");
            _choiceText = _choiceText.Replace("<ipro_mcchoice/>", "");

            string spancorrect = "<SPAN id=\"" + anchorID + "\">";
            string showcorrect = _MC._problem.p_showcorrect;
            if ((showcorrect == "yes") || (showcorrect == "icononly") || (showcorrect == "inline"))
            {
                if (_intchoiceid == _MC._intcorrect)
                {
                    spancorrect += "<IMG height=\"16\" width=\"16\" src=\"" + _MC._problem.baseUrl + "pictures/showcorrect_success.gif\" />";
                }
                else
                {
                }
            }
            string onclick = " disabled ";
            string ischecked = "";

            if (_MC._problem.p_results != "show")
            {
                onclick = " onclick=\"javascript:mult_MC_Answer(" + this._mcid + "," + _intchoiceid.ToString() + ",'" + "hts$QUESTIONID$." + stepID + ".Checkbox')\"";
            }
            if ((_MC._problem.p_results != "show")||(_MC._problem.p_results != "edit"))
            {
                if (_MC._problem.checkUserMC(this, stepID))
                {
                    ischecked = " checked ";
                    if ((showcorrect == "yes") || (showcorrect == "icononly") || (showcorrect == "inline"))
                    {
                        if (_intchoiceid == _MC._intcorrect)
                        {
                            spancorrect = "<SPAN id=\"" + anchorID + "\">"+"<IMG height=\"16\" width=\"16\" src=\"" + _MC._problem.baseUrl + "pictures/success.gif\" />";
                        }
                        else
                        {
                            spancorrect += "<IMG height=\"16\" width=\"16\" src=\"" + _MC._problem.baseUrl + "pictures/fail.gif\" />";
                        }
                    }
                }
            }
            spancorrect += "</SPAN>";

            string inputfld = "<INPUT type=\"radio\" id=\"" + inputID + "\"" + onclick + ischecked + " />";
            _choiceText = spancorrect + inputfld + _choiceText;


            choice.RemoveAll();
            choice.SetValue(getStringForSubstitute(origOrder));
        }

        internal string getStringForSubstitute(int order)
        {
            return stepID + ".CHOICE_" + this._mcid + "_" + order.ToString();
        }

        internal string doScrambleSubstitute(string str)
        {
            return str.Replace("<ipro_mcchoice>" + getStringForSubstitute(scrambleOrder) + "</ipro_mcchoice>", _choiceText);

        }
    }


}
