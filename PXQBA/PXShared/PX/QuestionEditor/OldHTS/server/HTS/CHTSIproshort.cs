using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace HTS
{
    public class CHTSIproshort
    {
        internal CHTSIproStep _step = null;
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


        private Dictionary<int, CCorrect> answers = new Dictionary<int, CCorrect>();

        public CHTSIproshort(XElement xEl, CHTSIproStep step)
        {
            _step = step;
            _elid = CUtils.getXElementAttribureValue(xEl, "elid", "1");
            _format = CUtils.getXElementAttribureValue(xEl, "format", "");
            _type = CUtils.getXElementAttribureValue(xEl, "type", "numeric");
            _tolerance = CUtils.getXElementAttribureValue(xEl, "tolerance", "0.02");
            _rule = CUtils.getXElementAttribureValue(xEl, "answerrule", "any");
            _correct = CUtils.getXElementAttribureValue(xEl, "correct", "");
            _points = CUtils.getXElementAttribureValue(xEl, "points", "1");
            _checksyntax = CUtils.getXElementAttribureValue(xEl, "checksyntax", "on");
            _allowedwords = CUtils.getXElementAttribureValue(xEl, "allowedwords", "");
            _size = CUtils.getXElementAttribureValue(xEl, "size", "2");

            CCorrect correct = new CCorrect(this._step._problem, xEl, _type, _points, _tolerance, _rule);

            answers.Add(answers.Count, correct);
            IEnumerable<XElement> alt = xEl.XPathSelectElements(".//ipro_alt_correct");
            foreach (XElement el in alt)
            {
                CCorrect alt_correct = new CCorrect(this._step._problem, el, _type, _points, _tolerance, _rule);
                answers.Add(answers.Count, alt_correct);
            }
        }

        public bool checkAnswer(string uAnswer)
        {
            bool result = false;
            foreach (KeyValuePair<int, CCorrect> kvp in answers)
            {
                if (kvp.Value.checkAnswer(uAnswer))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        internal string getCorrectForShowCorrect()
        {
            int countAnswers = this.answers.Count;
            string s = countAnswers > 1 ? "<br />" : "";
            string plusminus = "&plusmn;";
            string showMode = "exprtext=";
            for (int i = 0; i < countAnswers; i++)
            {
                CCorrect correct = this.answers[i];
                if (this._type == "math")
                {
                    showMode = ((correct._rule == "exact") || (correct._rule == "similar")) ? "eqtext=" : "exprtext=";
                    if (i > 0) s += "<br />";
                    s += "<img src=" + this._step._problem.baseUrl + "geteq.ashx?" + showMode + HttpUtility.UrlEncode(this.answers[0]._correct) + "&bottom=8 align=middle /> ";
                }
                else
                {
                    string sCorr = correct._correct.Replace("'", "\\'");
                    sCorr = sCorr.Replace("\"", "\\'\\'");
                    if (this._type == "numeric")
                    {
                        if (correct.getTolerance(true) == 0) plusminus = "";
                        if (i > 0) s += "<br />";
                        s += sCorr + " <span style=color:Blue>(" + plusminus + correct.getTolerance(true).ToString() + ")</span>";
                    }
                    else
                    {
                        s += sCorr;
                    }
                }
            }
            return s;
        }
        internal bool checkSyntaxNeeded()
        {
            bool ret = (_type== "numeric") && ((_checksyntax == "on") || (_checksyntax == "yes") || (_checksyntax == "true"));
            if (ret)
            {
                this._step.elemsForCheck.Add("hts$QUESTIONID$." + this._step.stepID + ".EnterField" + this._elid, this);
            }
            return ret;
        }
    }

    public class CCorrect
    {
        internal string _correct = "";
        internal string _format = "";
        internal string _tolerance = "";
        internal string _type = "";
        internal string _rule = "any";
        internal string _points = "1";
        internal CHTSProblem _problem = null;

        public CCorrect(CHTSProblem problem, XElement xEl, string atype, string apoints, string atolerance, string arule)
        {
            _problem = problem;
            _format = CUtils.getXElementAttribureValue(xEl, "format", "");
            _type = atype;
            _tolerance = CUtils.getXElementAttribureValue(xEl, "tolerance", atolerance);
            _rule = CUtils.getXElementAttribureValue(xEl, "answerrule", arule);
            _correct = CUtils.getXElementAttribureValue(xEl, "correct", "");
            _correct = _correct.Trim();
            _points = CUtils.getXElementAttribureValue(xEl, "points", apoints);

        }

        internal double getTolerance(bool bForPopup)
        {
            double pTolerance = 0.02;
            try { pTolerance = Convert.ToDouble(_tolerance) + (bForPopup ? 0 : 0.0000000001); }
            catch (Exception ex)
            {
                try
                {
                    pTolerance = Convert.ToDouble(_tolerance.Replace(".", ",")) + (bForPopup ? 0 : 0.0000000001);
                }
                catch { }
            }
            return pTolerance;
        }
        public bool checkAnswer(string uAnswer)
        {
            bool isCorrect = false;
            double pTolerance = 0.02;
            string correct = _correct;
            string answerBrackets = "";
            string correctBrackets = "";
            int rightIndex = -1;

            uAnswer = uAnswer.Trim();
            if (correct == uAnswer) return true;

            switch (_type)
            {
                case "numeric":
                    pTolerance = getTolerance(false);
                    uAnswer = uAnswer.ToUpper();
                    uAnswer = CUtils.removeBlanks(uAnswer);
                    uAnswer = uAnswer.Replace(";", ",");
                    correct = CUtils.removeBlanks(correct);
                    answerBrackets = CUtils.detectBrackets(uAnswer);
                    correctBrackets = CUtils.detectBrackets(correct);
                    if (correct == uAnswer) return true;

                    if (correct.IndexOf(';') != -1)
                    {//answer_rules = "NON-ordered_list";
                        uAnswer = CUtils.removeBrackets(uAnswer);
                        correct = CUtils.removeBrackets(correct);
                        string[] correctsArray = correct.Split(new char []{ ';' });
                        StringCollection answersColl = new StringCollection();
                        answersColl.AddRange (uAnswer.Split(new char []{ ',' }));
                        try
                        {
                            if (correctsArray.Length == answersColl.Count)
                            {
                                int cc = 0;
                                int ac = 0;
                                rightIndex = -1;
                                for (cc = 0; cc < correctsArray.Length; cc++)
                                {
                                    double tempCorrectNum = 0;
                                    if (_problem != null)
                                    {
                                        object c = _problem.doCalculate(correctsArray[cc]);
                                        tempCorrectNum = Convert.ToDouble(c);
                                        tempCorrectNum = CUtils.formatNumber(tempCorrectNum, _format);

                                    }
                                    else
                                    {
                                        tempCorrectNum = Convert.ToDouble(correct);
                                    }

                                    double tempAnswerNum = 0;
                                    for (ac = 0; ac < answersColl.Count; ac++)
                                    {
                                        tempAnswerNum = Convert.ToDouble(answersColl[ac]);

                                        double tempResultNum = tempCorrectNum - tempAnswerNum;
                                        tempResultNum = Math.Abs(Math.Round(tempResultNum, 12));
                                        if (tempResultNum <= pTolerance)
                                        {
                                            rightIndex = ac;
                                            break;
                                        }
                                    }
                                    if (rightIndex == -1) return false;
                                    answersColl.RemoveAt(rightIndex);
                                }
                                return true;
                            }
                        }
                        catch { }
                        return false;
                    }
                    else if (correct.IndexOf(',') != -1)
                    {//answer_rules = "ordered_list";
                        uAnswer = CUtils.removeBrackets(uAnswer);
                        correct = CUtils.removeBrackets(correct);
                        string[] correctsArray = correct.Split(new char[] { ',' });
                        StringCollection answersColl = new StringCollection();
                        answersColl.AddRange(uAnswer.Split(new char[] { ',' }));
                        try
                        {
                            if (correctsArray.Length == answersColl.Count)
                            {
                                int cc = 0;
                                rightIndex = -1;
                                for (cc = 0; cc < correctsArray.Length; cc++)
                                {
                                    double tempCorrectNum = 0;
                                    if (_problem != null)
                                    {
                                        object c = _problem.doCalculate(correctsArray[cc]);
                                        tempCorrectNum = Convert.ToDouble(c);
                                        tempCorrectNum = CUtils.formatNumber(tempCorrectNum, _format);

                                    }
                                    else
                                    {
                                        tempCorrectNum = Convert.ToDouble(correct);
                                    }

                                    double tempAnswerNum = 0;
                                    tempAnswerNum = Convert.ToDouble(answersColl[cc]);

                                    double tempResultNum = tempCorrectNum - tempAnswerNum;
                                    tempResultNum = Math.Abs(Math.Round(tempResultNum, 12));
                                    if (tempResultNum > pTolerance)
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                        catch { }
                        return false;
                    }
                    else
                    { //answer rule=exact
                        double tempCorrectNum = 0; //Convert.ToDouble(correct);
                        double tempAnswerNum = 0;
                        try
                        {
                            tempAnswerNum = Convert.ToDouble(uAnswer);
                            if (_problem != null)
                            {
                                object c = _problem.doCalculate(correct);
                                tempCorrectNum = Convert.ToDouble(c);
                                tempCorrectNum = CUtils.formatNumber(tempCorrectNum, _format);

                            }
                            else
                            {
                                tempCorrectNum = Convert.ToDouble(correct);
                            }
                            double tempResultNum = tempCorrectNum - tempAnswerNum;
                            tempResultNum = Math.Abs(Math.Round(tempResultNum, 12));
                            return (tempResultNum <= pTolerance);
                        }
                        catch (Exception Exception)
                        { 
                        }
                        return false;
                    }

                    break;
                case "math":
                    string mathResult = CUtils.evalMath(correct, uAnswer, _rule);
                    return mathResult == "Correct";
                    break;
                case "text":
                    uAnswer = uAnswer.ToUpper();
                    correct = correct.ToUpper();
                    uAnswer = CUtils.mergeBlanks(uAnswer);
                    uAnswer = uAnswer.Replace(";", ",");
                    correct = CUtils.mergeBlanks(correct);
                    uAnswer = uAnswer.Replace(" ,", ",");
                    uAnswer = uAnswer.Replace(", ", ",");

                    if (correct == uAnswer) return true;

                    if (correct.IndexOf(';') != -1)
                    {//answer_rules = "NON-ordered_list";
                        correct = correct.Replace(" ;", ";");
                        correct = correct.Replace("; ", ";");
                        string[] correctsArray = correct.Split(new char[] { ';' });
                        StringCollection answersColl = new StringCollection();
                        answersColl.AddRange(uAnswer.Split(new char[] { ',' }));
                        try
                        {
                            if (correctsArray.Length == answersColl.Count)
                            {
                                int cc = 0;
                                int ac = 0;
                                rightIndex = -1;
                                for (cc = 0; cc < correctsArray.Length; cc++)
                                {
                                    for (ac = 0; ac < answersColl.Count; ac++)
                                    {
                                        if (correctsArray[cc] == answersColl[ac])
                                        {
                                            rightIndex = ac;
                                            break;
                                        }
                                    }
                                    if (rightIndex == -1) return false;
                                    answersColl.RemoveAt(rightIndex);
                                }
                                return true;
                            }
                        }
                        catch { }
                        return false;
                    }
                    else if (correct.IndexOf(',') != -1)
                    {//answer_rules = "ordered_list";
                        correct = correct.Replace(" ,", ",");
                        correct = correct.Replace(", ", ",");
                        string[] correctsArray = correct.Split(new char[] { ',' });
                        StringCollection answersColl = new StringCollection();
                        answersColl.AddRange(uAnswer.Split(new char[] { ',' }));
                        try
                        {
                            if (correctsArray.Length == answersColl.Count)
                            {
                                int cc = 0;
                                rightIndex = -1;
                                for (cc = 0; cc < correctsArray.Length; cc++)
                                {
                                    if (correctsArray[cc] != answersColl[cc])
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                        catch { }
                        return false;
                    }
                    else
                    { //answer rule=exact
                        try
                        {
                            return (correct == uAnswer);
                        }
                        catch { }
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return isCorrect;
        }

    }

}
