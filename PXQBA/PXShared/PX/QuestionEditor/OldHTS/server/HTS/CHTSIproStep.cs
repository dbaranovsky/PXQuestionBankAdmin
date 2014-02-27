using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows;

namespace HTS
{
    public class CHTSIproStep
    {
        internal CHTSProblem _problem = null;
        private Dictionary<string, CHTSIproNav> ipronavs = new Dictionary<string, CHTSIproNav>();
        internal string stepID = "";
        internal bool b_syntaxcheckNeeded = false;
        internal Dictionary<string, CHTSIproshort> elemsForCheck = new Dictionary<string, CHTSIproshort>();

        public CHTSIproStep(CHTSProblem problem)
        {
            this._problem = problem;
        }

        public void processStep(XElement xEl)
        {
            this.stepID = CUtils.getXElementAttribureValue(xEl, "id", "step0");
            IEnumerable<XElement> iproNavs = xEl.XPathSelectElements(".//ipronav");
            foreach (XElement iproNav in iproNavs)
            {
                string navtype = CUtils.getXElementAttribureValue(iproNav, "navtype", "");
                string nextStepID = CUtils.getXElementAttribureValue(iproNav, "stepid", "");
                if (nextStepID == "")
                {
                    nextStepID = CUtils.getXElementAttribureValue(iproNav, "next", "");
                }
                try
                {
                    CHTSIproNav nav = new CHTSIproNav(navtype, nextStepID);
                    this.ipronavs.Add(nav.navtype, nav);
                    if ((navtype == "correct") || (navtype == "incorrect"))
                    {
                        if (this._problem.p_showfeedback != "yes")
                        {
                            IEnumerable<XElement> iproCSteps = this._problem.htsProblem.Root.XPathSelectElements(".//iprostep[@id=\"" + nextStepID + "\"]");
                            foreach (XElement el in iproCSteps)
                            {
                                el.SetAttributeValue("del", "yes");
                            }
                        }
                        else
                        {
                            Point stepScore = this._problem.getStepScore(xEl);
                            if (((stepScore.X == stepScore.Y) && (navtype == "incorrect")) 
                                || ((stepScore.X != stepScore.Y) && (navtype == "correct")))
                            {
                                IEnumerable<XElement> iproCSteps = this._problem.htsProblem.Root.XPathSelectElements(".//iprostep[@id=\"" + nextStepID + "\"]");
                                foreach (XElement el in iproCSteps)
                                {
                                    el.SetAttributeValue("del", "yes");
                                }
                            }

                        }
                    }


                }
                catch { }
            }
            iproNavs.Remove();
            IEnumerable<XElement> iproInputs = xEl.XPathSelectElements(".//input");
            foreach (XElement iproInput in iproInputs)
            {
                string iclass = CUtils.getXElementAttribureValue(iproInput, "class", "");
                CHTSIproNav inav = this.getIpronav(iclass);
                if (inav == null)
                {
                    iproInput.SetAttributeValue("del", "yes");
                }
                else
                {

                    if (iclass == "cq_hts_next")
                    {
                        if (!this._problem.showAllSteps)
                        {
                            if (inav != null)
                            {
                                iproInput.SetAttributeValue("id", "hts$QUESTIONID$." + this.stepID + "." + iclass);
                                iproInput.SetAttributeValue("onclick", "javascript:next('hts$QUESTIONID$." + this.stepID + "." + iclass + "','hts$QUESTIONID$." + inav.nextStepID + "')");
                            }
                        }
                        else
                        {
                            iproInput.SetAttributeValue("del", "yes");
                        }
                    }
                    else if (iclass == "cq_hts_hint")
                    {
                        if (this._problem.p_hints == "yes" && (!this._problem.showAllSteps))
                        {
                            if (inav != null)
                            {
                                iproInput.SetAttributeValue("id", "hts$QUESTIONID$." + this.stepID + "." + iclass);
                                iproInput.SetAttributeValue("onclick", "javascript:next('hts$QUESTIONID$." + this.stepID + "." + iclass + "','hts$QUESTIONID$." + inav.nextStepID+"')");
                            }
                        }
                        else
                        {
                            iproInput.SetAttributeValue("del", "yes");
                        }
                    }
                    else{
                    }
                }
            }
            iproInputs = xEl.XPathSelectElements(".//input[@del=\"yes\"]");
            iproInputs.Remove();
            
        }

        private CHTSIproNav getIpronav(string navtype)
        {
            CHTSIproNav inav = null;
            foreach (KeyValuePair<string, CHTSIproNav> kvp in ipronavs)
            {
                if ((kvp.Value.navtype == navtype) || (("cq_hts_" + kvp.Value.navtype) == navtype))
                {
                    inav = kvp.Value;
                    break;
                }
            }
            return inav;
        }
    }

    public class CHTSIproNav
    {
        internal string navtype = "";
        internal string nextStepID = "";

        public CHTSIproNav(string nav, string next)
        {
            navtype = nav;
            nextStepID = next;
        }

    }
}
