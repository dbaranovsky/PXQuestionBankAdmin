<%@ WebHandler Language="C#" Class="PxPlayer" %>

using System;
using System.IO;
using System.Net; // for WebRequest
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using BFW;
using HTS;


public class PxPlayer : IHttpHandler
{
    bool debugMode = true; //on/off saving requests/responses
    string baseUrl = BFW.ItemServer.BaseUrl; // base url from web.config
    string mathJS = string.Empty; // path to maths.js file. We need use the javascript functions from this file to generate values of variables

    string encryptKey = BFW.ItemServer.EncryptionKey; // EncryptionKey from web.config
    Guid problemXMLguid = Guid.NewGuid(); //for generate unique signKey

    bool isActive = false;
    bool isReview = false;
    bool isPrint = false;
    bool isPrintKey = false;
    bool isNone = true;

    const int ef_ShowCorrectChoice = 0x800;
    const int ef_ShowFeedback = 0x1000;
    
    public void ProcessRequest(HttpContext context) 
    {
        context.Response.Expires = 0;
        context.Response.ContentType = "text/plain";
        context.Response.Write(Question(context));
        context.Response.End();
                
    }


    public string Question(HttpContext context)
    {

        mathJS = context.Server.MapPath("~\\" + "htsplayer\\maths.js");
        
        System.IO.Stream body = context.Request.InputStream;
        System.Text.Encoding encoding = context.Request.ContentEncoding;
        System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
        string s = reader.ReadToEnd();

        //save request for debug
        if (debugMode)
        {
            string fname = context.Server.MapPath("~\\" + "htsplayer\\session") + "\\" + "request_" + problemXMLguid + ".xml";
            using (StreamWriter f = File.CreateText(fname))
            {
                f.Write(s);
                f.Close();
            }
        }

        XDocument request = XDocument.Load(new System.IO.StringReader(s));

        XDocument custom =
            new XDocument(
                new XElement("custom",
                    new XElement("version", "1"),
            // Grade this question on the server
            new XElement("score", baseUrl + "PxScore.ashx"),
                    new XElement("display", "Active,Review,Print,PrintKey"),
                    new XElement("privatedata", getSignKey(request.Root)),
                    new XElement("body", new XCData(GetBody(request.Root)))
                )
            );

        //save answer for debug
        if (debugMode)
        {
            string fname = context.Server.MapPath("~\\" + "htsplayer\\session") + "\\" + "response_" + problemXMLguid + ".xml";
            using (StreamWriter f = File.CreateText(fname))
            {
                f.Write(custom.ToString());
                f.Close();
            }
        }
        return custom.ToString();

    }
    
    private string GetProblemXML(XElement info)
    {
        string problemXML = "";
        try
        {
            problemXML = info.XPathSelectElement("question/interaction/data").Value;
        }
        catch { problemXML = "<div>Bad request: cannot read .../interaction/data element</div>"; }
        return problemXML.ToString();
    }

    private string GetBody(XElement info)
    {
        CHTSProblem hts = new CHTSProblem(mathJS, baseUrl);
        int examflags = 0;
        // Get the information we need from the request
        string id = info.Attribute("id").Value;
        isActive = info.Attribute("mode").Value == "Active";
        isReview = info.Attribute("mode").Value == "Review";
        isPrint = info.Attribute("mode").Value == "Print";
        isPrintKey = info.Attribute("mode").Value == "PrintKey";
        isNone = info.Attribute("mode").Value == "None";

        try
        {
            string s_examflags = info.XPathSelectElement("item/data/examflags").Value;
            examflags = Convert.ToInt32(s_examflags);
        }
        catch { }
                
        if (isNone)
        {
            return "";
        }
        
        // Generate params list for the player
        if (isActive)
        {
            hts.setParameters("results=edit;showcorrect=no;feedback=no;showfeedback=no;showsolution=no;showanswers=no;hints=yes");
        }
        if (isReview)
        {
            hts.setParameters("results=show;showcorrect=yes;feedback=yes;showfeedback=yes;showsolution=yes;showanswers=yes;hints=yes");
            if (examflags != 0)
            {
                hts.setParameters("showcorrect=" + ((examflags & ef_ShowCorrectChoice) != 0 ? "yes" : "no"));
                hts.setParameters("showsolution=" + ((examflags & ef_ShowCorrectChoice) != 0 ? "yes" : "no"));
                hts.setParameters("showfeedback=" + ((examflags & ef_ShowFeedback) != 0 ? "yes" : "no"));
            }
        }
        if (isPrint)
        {
            hts.setParameters("results=show;showcorrect=no;feedback=no;showfeedback=no;showsolution=no;showanswers=no;hints=no;syntaxchecking=off");
        }
        if (isPrintKey)
        {
            hts.setParameters("results=ignore;showcorrect=inline;feedback=no;showfeedback=no;showsolution=yes;showanswers=no;hints=no;syntaxchecking=off");
        }
        string htsProblemXML = GetProblemXML(info);

        string p = htsProblemXML;
        CHTSResponse response = new CHTSResponse(hts);
        try
        {
            try
            {
                string respData = info.XPathSelectElement("submission/answer").Value;
                response.parseResponse(respData, encryptKey, getSignKey(info));
            }
            catch { response = null; }

            hts.doProblem(htsProblemXML, response);

            p = hts.getProblemPageForAgilix(encryptKey, getSignKey(info));
        }
        catch { }
        return p;
        
    }

    private string getSignKey(XElement info)
    {
        string signKey = "";
        try
        {
            XElement node = info.XPathSelectElement("submission/attemptquestion/customprivatedata");
            if (node != null)
            {
                signKey = node.Value;
            }
        }
        catch { }
        if (signKey == "")
        {
            signKey = problemXMLguid.ToString();
        }
        return signKey;

    }
   
    public bool IsReusable {
        get {
            return false;
        }
    }

}