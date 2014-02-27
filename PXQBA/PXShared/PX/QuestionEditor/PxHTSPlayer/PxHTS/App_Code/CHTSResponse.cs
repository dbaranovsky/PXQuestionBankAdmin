using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
	public class CHTSResponse
	{
		public Dictionary<string, CVardef> vars = new Dictionary<string, CVardef>();
		private CHTSProblem htsProblem;
		public XDocument xmlResponse;
		public CHTSResponse(CHTSProblem problem)
		{
			this.htsProblem = problem;
		}
		private void getVardefs()
		{
			IEnumerable<XElement> enumerable = this.xmlResponse.XPathSelectElements("//vardef");
			foreach (XElement current in enumerable)
			{
				CVardef cVardef = new CVardef(current, this);
				this.vars.Add(cVardef.Name, cVardef);
			}
		}
		public void parseResponse(string response, string password, string sign)
		{
			this.xmlResponse = XDocument.Load(new StringReader(response));
			string text = this.xmlResponse.XPathSelectElement("//iproblem").Value;
			text = CUtils.Decrypt(text, password, sign);
			this.xmlResponse.XPathSelectElement("//iproblem").Remove();
			text = text.Replace("<iproblem>", "");
			try
			{
				XDocument xDocument = XDocument.Load(new StringReader(text));
				this.xmlResponse.Root.AddFirst(xDocument.Nodes());
			}
			catch (Exception)
			{
			}
			this.getVardefs();
		}
	}
}
