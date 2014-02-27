using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace HTS
{
    public class CHTSResponse
    {
        public Dictionary<string, CVardef> vars = new Dictionary<string, CVardef>();
        private CHTSProblem htsProblem = null;
        public XDocument xmlResponse = null;

        public CHTSResponse(CHTSProblem problem)
        {
            htsProblem = problem;
        }

        private void getVardefs()
        {
            IEnumerable<XElement> vardefs = xmlResponse.XPathSelectElements("//vardef");
            foreach (XElement el in vardefs)
            {
                CVardef vardef = new CVardef(el, this);
                vars.Add(vardef.Name, vardef);
            }

        }

        public void parseResponse(string response, string password, string sign)
        {
            xmlResponse = XDocument.Load(new System.IO.StringReader(response));

            string resp = xmlResponse.XPathSelectElement("//iproblem").Value;
            resp = CUtils.Decrypt(resp, password, sign);
            xmlResponse.XPathSelectElement("//iproblem").Remove();
            resp = resp.Replace("<iproblem>", "");
            try
            {
                XDocument dec = XDocument.Load(new System.IO.StringReader(resp));

                xmlResponse.Root.AddFirst(dec.Nodes());
            }
            catch (Exception ex)
            {
            }
            

            getVardefs();
            //doVariables();
            //replaceVariables();

        }

    }
}
