using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bfw.PX.XBkPlayer.Biz.DataContracts
{
    public class XPathDocument: System.Xml.XPath.XPathDocument
    {
        public XPathDocument(string xmlString):base(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
        {
        }
    }
}
